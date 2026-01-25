using System;
using BestHTTP;
using BestHTTP.WebSocket;
using UnityEngine;

public class WebSocketSample : MonoBehaviour
{
	private string address = "ws://echo.websocket.org";

	private string msgToSend = "Hello World!";

	private string Text = string.Empty;

	private WebSocket webSocket;

	private Vector2 scrollPos;

	private void OnDestroy()
	{
		if (webSocket != null)
		{
			webSocket.Close();
		}
	}

	private void OnGUI()
	{
		GUIHelper.DrawArea(GUIHelper.ClientArea, true, delegate
		{
			scrollPos = GUILayout.BeginScrollView(scrollPos);
			GUILayout.Label(Text);
			GUILayout.EndScrollView();
			GUILayout.Space(5f);
			GUILayout.FlexibleSpace();
			address = GUILayout.TextField(address);
			if (webSocket == null && GUILayout.Button("Open Web Socket"))
			{
				webSocket = new WebSocket(new Uri(address));
				if (HTTPManager.Proxy != null)
				{
					webSocket.InternalRequest.Proxy = new HTTPProxy(HTTPManager.Proxy.Address, HTTPManager.Proxy.Credentials, false);
				}
				WebSocket obj = webSocket;
				obj.OnOpen = (OnWebSocketOpenDelegate)Delegate.Combine(obj.OnOpen, new OnWebSocketOpenDelegate(OnOpen));
				WebSocket obj2 = webSocket;
				obj2.OnMessage = (OnWebSocketMessageDelegate)Delegate.Combine(obj2.OnMessage, new OnWebSocketMessageDelegate(OnMessageReceived));
				WebSocket obj3 = webSocket;
				obj3.OnClosed = (OnWebSocketClosedDelegate)Delegate.Combine(obj3.OnClosed, new OnWebSocketClosedDelegate(OnClosed));
				WebSocket obj4 = webSocket;
				obj4.OnError = (OnWebSocketErrorDelegate)Delegate.Combine(obj4.OnError, new OnWebSocketErrorDelegate(OnError));
				webSocket.Open();
				Text += "Opening Web Socket...\n";
			}
			if (webSocket != null && webSocket.IsOpen)
			{
				GUILayout.Space(10f);
				GUILayout.BeginHorizontal();
				msgToSend = GUILayout.TextField(msgToSend);
				if (GUILayout.Button("Send", GUILayout.MaxWidth(70f)))
				{
					Text += "Sending message...\n";
					webSocket.Send(msgToSend);
				}
				GUILayout.EndHorizontal();
				GUILayout.Space(10f);
				if (GUILayout.Button("Close"))
				{
					webSocket.Close(1000, "Bye!");
				}
			}
		});
	}

	private void OnOpen(WebSocket ws)
	{
		Text += string.Format("-WebSocket Open!\n");
	}

	private void OnMessageReceived(WebSocket ws, string message)
	{
		Text += string.Format("-Message received: {0}\n", message);
	}

	private void OnClosed(WebSocket ws, ushort code, string message)
	{
		Text += string.Format("-WebSocket closed! Code: {0} Message: {1}\n", code, message);
		webSocket = null;
	}

	private void OnError(WebSocket ws, Exception ex)
	{
		string text = string.Empty;
		if (ws.InternalRequest.Response != null)
		{
			text = string.Format("Status Code from Server: {0} and Message: {1}", ws.InternalRequest.Response.StatusCode, ws.InternalRequest.Response.Message);
		}
		Text += string.Format("-An error occured: {0}\n", (ex == null) ? ("Unknown Error " + text) : ex.Message);
		webSocket = null;
	}
}
