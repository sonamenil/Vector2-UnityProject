using System;
using System.Collections.Generic;
using BestHTTP.Logger;

namespace BestHTTP.SocketIO.Transports
{
	internal sealed class PollingTransport : ITransport
	{
		private HTTPRequest LastRequest;

		private HTTPRequest PollRequest;

		private Packet PacketWithAttachment;

		public TransportStates State { get; private set; }

		public SocketManager Manager { get; private set; }

		public bool IsRequestInProgress
		{
			get
			{
				return LastRequest != null;
			}
		}

		public PollingTransport(SocketManager manager)
		{
			Manager = manager;
		}

		public void Open()
		{
			HTTPRequest hTTPRequest = new HTTPRequest(new Uri(string.Format("{0}?EIO={1}&transport=polling&t={2}-{3}&sid={4}{5}&b64=true", Manager.Uri.ToString(), 4, Manager.Timestamp.ToString(), Manager.RequestCounter++.ToString(), Manager.Handshake.Sid, Manager.Options.QueryParamsOnlyForHandshake ? string.Empty : Manager.Options.BuildQueryParams())), OnRequestFinished);
			hTTPRequest.DisableCache = true;
			hTTPRequest.DisableRetry = true;
			hTTPRequest.Send();
			State = TransportStates.Opening;
		}

		public void Close()
		{
			if (State != TransportStates.Closed)
			{
				State = TransportStates.Closed;
			}
		}

		public void Send(Packet packet)
		{
			Send(new List<Packet> { packet });
		}

		public void Send(List<Packet> packets)
		{
			if (State != TransportStates.Open)
			{
				throw new Exception("Transport is not in Open state!");
			}
			if (IsRequestInProgress)
			{
				throw new Exception("Sending packets are still in progress!");
			}
			byte[] array = null;
			try
			{
				array = packets[0].EncodeBinary();
				for (int i = 1; i < packets.Count; i++)
				{
					byte[] array2 = packets[i].EncodeBinary();
					Array.Resize(ref array, array.Length + array2.Length);
					Array.Copy(array2, 0, array, array.Length - array2.Length, array2.Length);
				}
				packets.Clear();
			}
			catch (Exception ex)
			{
				((IManager)Manager).EmitError(SocketIOErrors.Internal, ex.Message + " " + ex.StackTrace);
				return;
			}
			LastRequest = new HTTPRequest(new Uri(string.Format("{0}?EIO={1}&transport=polling&t={2}-{3}&sid={4}{5}&b64=true", Manager.Uri.ToString(), 4, Manager.Timestamp.ToString(), Manager.RequestCounter++.ToString(), Manager.Handshake.Sid, Manager.Options.QueryParamsOnlyForHandshake ? string.Empty : Manager.Options.BuildQueryParams())), HTTPMethods.Post, OnRequestFinished);
			LastRequest.DisableCache = true;
			LastRequest.SetHeader("Content-Type", "application/octet-stream");
			LastRequest.RawData = array;
			LastRequest.Send();
		}

		private void OnRequestFinished(HTTPRequest req, HTTPResponse resp)
		{
			LastRequest = null;
			if (State == TransportStates.Closed)
			{
				return;
			}
			string text = null;
			switch (req.State)
			{
			case HTTPRequestStates.Finished:
				if (HTTPManager.Logger.Level <= Loglevels.All)
				{
					HTTPManager.Logger.Verbose("PollingTransport", "OnRequestFinished: " + resp.DataAsText);
				}
				if (resp.IsSuccess)
				{
					ParseResponse(resp);
					break;
				}
				text = string.Format("Polling - Request finished Successfully, but the server sent an error. Status Code: {0}-{1} Message: {2} Uri: {3}", resp.StatusCode, resp.Message, resp.DataAsText, req.CurrentUri);
				break;
			case HTTPRequestStates.Error:
				text = ((req.Exception == null) ? "No Exception" : (req.Exception.Message + "\n" + req.Exception.StackTrace));
				break;
			case HTTPRequestStates.Aborted:
				text = string.Format("Polling - Request({0}) Aborted!", req.CurrentUri);
				break;
			case HTTPRequestStates.ConnectionTimedOut:
				text = string.Format("Polling - Connection Timed Out! Uri: {0}", req.CurrentUri);
				break;
			case HTTPRequestStates.TimedOut:
				text = string.Format("Polling - Processing the request({0}) Timed Out!", req.CurrentUri);
				break;
			}
			if (!string.IsNullOrEmpty(text))
			{
				((IManager)Manager).OnTransportError((ITransport)this, text);
			}
		}

		public void Poll()
		{
			if (PollRequest == null && State != TransportStates.Paused)
			{
				PollRequest = new HTTPRequest(new Uri(string.Format("{0}?EIO={1}&transport=polling&t={2}-{3}&sid={4}{5}&b64=true", Manager.Uri.ToString(), 4, Manager.Timestamp.ToString(), Manager.RequestCounter++.ToString(), Manager.Handshake.Sid, Manager.Options.QueryParamsOnlyForHandshake ? string.Empty : Manager.Options.BuildQueryParams())), HTTPMethods.Get, OnPollRequestFinished);
				PollRequest.DisableCache = true;
				PollRequest.DisableRetry = true;
				PollRequest.Send();
			}
		}

		private void OnPollRequestFinished(HTTPRequest req, HTTPResponse resp)
		{
			PollRequest = null;
			if (State == TransportStates.Closed)
			{
				return;
			}
			string text = null;
			switch (req.State)
			{
			case HTTPRequestStates.Finished:
				if (HTTPManager.Logger.Level <= Loglevels.All)
				{
					HTTPManager.Logger.Verbose("PollingTransport", "OnPollRequestFinished: " + resp.DataAsText);
				}
				if (resp.IsSuccess)
				{
					ParseResponse(resp);
					break;
				}
				text = string.Format("Polling - Request finished Successfully, but the server sent an error. Status Code: {0}-{1} Message: {2} Uri: {3}", resp.StatusCode, resp.Message, resp.DataAsText, req.CurrentUri);
				break;
			case HTTPRequestStates.Error:
				text = ((req.Exception == null) ? "No Exception" : (req.Exception.Message + "\n" + req.Exception.StackTrace));
				break;
			case HTTPRequestStates.Aborted:
				text = string.Format("Polling - Request({0}) Aborted!", req.CurrentUri);
				break;
			case HTTPRequestStates.ConnectionTimedOut:
				text = string.Format("Polling - Connection Timed Out! Uri: {0}", req.CurrentUri);
				break;
			case HTTPRequestStates.TimedOut:
				text = string.Format("Polling - Processing the request({0}) Timed Out!", req.CurrentUri);
				break;
			}
			if (!string.IsNullOrEmpty(text))
			{
				((IManager)Manager).OnTransportError((ITransport)this, text);
			}
		}

		private void OnPacket(Packet packet)
		{
			if (packet.AttachmentCount != 0 && !packet.HasAllAttachment)
			{
				PacketWithAttachment = packet;
				return;
			}
			TransportEventTypes transportEvent = packet.TransportEvent;
			if (transportEvent == TransportEventTypes.Message && packet.SocketIOEvent == SocketIOEventTypes.Connect && State == TransportStates.Opening)
			{
				State = TransportStates.Open;
				if (!((IManager)Manager).OnTransportConnected((ITransport)this))
				{
					return;
				}
			}
			((IManager)Manager).OnPacket(packet);
		}

		private void ParseResponse(HTTPResponse resp)
		{
			try
			{
				if (resp == null || resp.Data == null || resp.Data.Length < 1)
				{
					return;
				}
				string dataAsText = resp.DataAsText;
				if (dataAsText == "ok")
				{
					return;
				}
				int num = dataAsText.IndexOf(':', 0);
				int num2 = 0;
				while (num >= 0 && num < dataAsText.Length)
				{
					int num3 = int.Parse(dataAsText.Substring(num2, num - num2));
					string text = dataAsText.Substring(++num, num3);
					if (text.Length > 2 && text[0] == 'b' && text[1] == '4')
					{
						byte[] data = Convert.FromBase64String(text.Substring(2));
						if (PacketWithAttachment != null)
						{
							PacketWithAttachment.AddAttachmentFromServer(data, true);
							if (PacketWithAttachment.HasAllAttachment)
							{
								try
								{
									OnPacket(PacketWithAttachment);
								}
								catch (Exception ex)
								{
									HTTPManager.Logger.Exception("PollingTransport", "ParseResponse - OnPacket with attachment", ex);
									((IManager)Manager).EmitError(SocketIOErrors.Internal, ex.Message + " " + ex.StackTrace);
								}
								finally
								{
									PacketWithAttachment = null;
								}
							}
						}
					}
					else
					{
						try
						{
							Packet packet = new Packet(text);
							OnPacket(packet);
						}
						catch (Exception ex2)
						{
							HTTPManager.Logger.Exception("PollingTransport", "ParseResponse - OnPacket", ex2);
							((IManager)Manager).EmitError(SocketIOErrors.Internal, ex2.Message + " " + ex2.StackTrace);
						}
					}
					num2 = num + num3;
					num = dataAsText.IndexOf(':', num2);
				}
			}
			catch (Exception ex3)
			{
				((IManager)Manager).EmitError(SocketIOErrors.Internal, ex3.Message + " " + ex3.StackTrace);
				HTTPManager.Logger.Exception("PollingTransport", "ParseResponse", ex3);
			}
		}
	}
}
