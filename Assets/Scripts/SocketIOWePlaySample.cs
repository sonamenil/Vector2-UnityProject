using System;
using System.Collections.Generic;
using BestHTTP.SocketIO;
using UnityEngine;

public sealed class SocketIOWePlaySample : MonoBehaviour
{
	private enum States
	{
		Connecting = 0,
		WaitForNick = 1,
		Joined = 2
	}

	private const float ratio = 1.5f;

	private string[] controls = new string[8] { "left", "right", "a", "b", "up", "down", "select", "start" };

	private int MaxMessages = 50;

	private States State;

	private Socket Socket;

	private string Nick = string.Empty;

	private string messageToSend = string.Empty;

	private int connections;

	private List<string> messages = new List<string>();

	private Vector2 scrollPos;

	private Texture2D FrameTexture;

	private void Start()
	{
		SocketOptions socketOptions = new SocketOptions();
		socketOptions.AutoConnect = false;
		SocketManager socketManager = new SocketManager(new Uri("http://io.weplay.io/socket.io/"), socketOptions);
		Socket = socketManager.Socket;
		Socket.On(SocketIOEventTypes.Connect, OnConnected);
		Socket.On("joined", OnJoined);
		Socket.On("connections", OnConnections);
		Socket.On("join", OnJoin);
		Socket.On("move", OnMove);
		Socket.On("message", OnMessage);
		Socket.On("reload", OnReload);
		Socket.On("frame", OnFrame, false);
		Socket.On(SocketIOEventTypes.Error, OnError);
		socketManager.Open();
		State = States.Connecting;
	}

	private void OnDestroy()
	{
		Socket.Manager.Close();
	}

	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.Escape))
		{
			SampleSelector.SelectedSample.DestroyUnityObject();
		}
	}

	private void OnGUI()
	{
		switch (State)
		{
		case States.Connecting:
			GUIHelper.DrawArea(GUIHelper.ClientArea, true, delegate
			{
				GUILayout.BeginVertical();
				GUILayout.FlexibleSpace();
				GUIHelper.DrawCenteredText("Connecting to the server...");
				GUILayout.FlexibleSpace();
				GUILayout.EndVertical();
			});
			break;
		case States.WaitForNick:
			GUIHelper.DrawArea(GUIHelper.ClientArea, true, delegate
			{
				DrawLoginScreen();
			});
			break;
		case States.Joined:
			GUIHelper.DrawArea(GUIHelper.ClientArea, true, delegate
			{
				if (FrameTexture != null)
				{
					GUILayout.Box(FrameTexture);
				}
				DrawControls();
				DrawChat(true);
			});
			break;
		}
	}

	private void DrawLoginScreen()
	{
		GUILayout.BeginVertical();
		GUILayout.FlexibleSpace();
		GUIHelper.DrawCenteredText("What's your nickname?");
		Nick = GUILayout.TextField(Nick);
		if (GUILayout.Button("Join"))
		{
			Join();
		}
		GUILayout.FlexibleSpace();
		GUILayout.EndVertical();
	}

	private void DrawControls()
	{
		GUILayout.BeginHorizontal();
		GUILayout.Label("Controls:");
		for (int i = 0; i < controls.Length; i++)
		{
			if (GUILayout.Button(controls[i]))
			{
				Socket.Emit("move", controls[i]);
			}
		}
		GUILayout.Label(" Connections: " + connections);
		GUILayout.EndHorizontal();
	}

	private void DrawChat(bool withInput = true)
	{
		GUILayout.BeginVertical();
		scrollPos = GUILayout.BeginScrollView(scrollPos, false, false);
		for (int i = 0; i < messages.Count; i++)
		{
			GUILayout.Label(messages[i], GUILayout.MinWidth(Screen.width));
		}
		GUILayout.EndScrollView();
		if (withInput)
		{
			GUILayout.Label("Your message: ");
			GUILayout.BeginHorizontal();
			messageToSend = GUILayout.TextField(messageToSend);
			if (GUILayout.Button("Send", GUILayout.MaxWidth(100f)))
			{
				SendMessage();
			}
			GUILayout.EndHorizontal();
		}
		GUILayout.EndVertical();
	}

	private void AddMessage(string msg)
	{
		messages.Insert(0, msg);
		if (messages.Count > MaxMessages)
		{
			messages.RemoveRange(MaxMessages, messages.Count - MaxMessages);
		}
	}

	private void SendMessage()
	{
		if (!string.IsNullOrEmpty(messageToSend))
		{
			Socket.Emit("message", messageToSend);
			AddMessage(string.Format("{0}: {1}", Nick, messageToSend));
			messageToSend = string.Empty;
		}
	}

	private void Join()
	{
		PlayerPrefs.SetString("Nick", Nick);
		Socket.Emit("join", Nick);
	}

	private void Reload()
	{
		FrameTexture = null;
		if (Socket != null)
		{
			Socket.Manager.Close();
			Socket = null;
			Start();
		}
	}

	private void OnConnected(Socket socket, Packet packet, params object[] args)
	{
		if (PlayerPrefs.HasKey("Nick"))
		{
			Nick = PlayerPrefs.GetString("Nick", "NickName");
			Join();
		}
		else
		{
			State = States.WaitForNick;
		}
		AddMessage("connected");
	}

	private void OnJoined(Socket socket, Packet packet, params object[] args)
	{
		State = States.Joined;
	}

	private void OnReload(Socket socket, Packet packet, params object[] args)
	{
		Reload();
	}

	private void OnMessage(Socket socket, Packet packet, params object[] args)
	{
		if (args.Length == 1)
		{
			AddMessage(args[0] as string);
		}
		else
		{
			AddMessage(string.Format("{0}: {1}", args[1], args[0]));
		}
	}

	private void OnMove(Socket socket, Packet packet, params object[] args)
	{
		AddMessage(string.Format("{0} pressed {1}", args[1], args[0]));
	}

	private void OnJoin(Socket socket, Packet packet, params object[] args)
	{
		string arg = ((args.Length <= 1) ? string.Empty : string.Format("({0})", args[1]));
		AddMessage(string.Format("{0} joined {1}", args[0], arg));
	}

	private void OnConnections(Socket socket, Packet packet, params object[] args)
	{
		connections = Convert.ToInt32(args[0]);
	}

	private void OnFrame(Socket socket, Packet packet, params object[] args)
	{
		if (State == States.Joined)
		{
			if (FrameTexture == null)
			{
				FrameTexture = new Texture2D(0, 0, TextureFormat.RGBA32, false);
				FrameTexture.filterMode = FilterMode.Point;
			}
			byte[] data = packet.Attachments[0];
			FrameTexture.LoadImage(data);
		}
	}

	private void OnError(Socket socket, Packet packet, params object[] args)
	{
		AddMessage(string.Format("--ERROR - {0}", args[0].ToString()));
	}
}
