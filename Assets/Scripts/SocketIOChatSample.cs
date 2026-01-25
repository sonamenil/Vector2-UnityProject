using System;
using System.Collections.Generic;
using BestHTTP.SocketIO;
using UnityEngine;

public sealed class SocketIOChatSample : MonoBehaviour
{
	private enum ChatStates
	{
		Login = 0,
		Chat = 1
	}

	private readonly TimeSpan TYPING_TIMER_LENGTH = TimeSpan.FromMilliseconds(700.0);

	private SocketManager Manager;

	private ChatStates State;

	private string userName = string.Empty;

	private string message = string.Empty;

	private string chatLog = string.Empty;

	private Vector2 scrollPos;

	private bool typing;

	private DateTime lastTypingTime = DateTime.MinValue;

	private List<string> typingUsers = new List<string>();

	private void Start()
	{
		State = ChatStates.Login;
		SocketOptions socketOptions = new SocketOptions();
		socketOptions.AutoConnect = false;
		Manager = new SocketManager(new Uri("http://chat.socket.io/socket.io/"), socketOptions);
		Manager.Socket.On("login", OnLogin);
		Manager.Socket.On("new message", OnNewMessage);
		Manager.Socket.On("user joined", OnUserJoined);
		Manager.Socket.On("user left", OnUserLeft);
		Manager.Socket.On("typing", OnTyping);
		Manager.Socket.On("stop typing", OnStopTyping);
		Manager.Socket.On(SocketIOEventTypes.Error, delegate(Socket socket, Packet packet, object[] args)
		{
			AdvLog.LogError(string.Format("Error: {0}", args[0].ToString()));
		});
		Manager.Open();
	}

	private void OnDestroy()
	{
		Manager.Close();
	}

	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.Escape))
		{
			SampleSelector.SelectedSample.DestroyUnityObject();
		}
		if (typing)
		{
			DateTime utcNow = DateTime.UtcNow;
			TimeSpan timeSpan = utcNow - lastTypingTime;
			if (timeSpan >= TYPING_TIMER_LENGTH)
			{
				Manager.Socket.Emit("stop typing");
				typing = false;
			}
		}
	}

	private void OnGUI()
	{
		switch (State)
		{
		case ChatStates.Login:
			DrawLoginScreen();
			break;
		case ChatStates.Chat:
			DrawChatScreen();
			break;
		}
	}

	private void DrawLoginScreen()
	{
		GUIHelper.DrawArea(GUIHelper.ClientArea, true, delegate
		{
			GUILayout.BeginVertical();
			GUILayout.FlexibleSpace();
			GUIHelper.DrawCenteredText("What's your nickname?");
			userName = GUILayout.TextField(userName);
			if (GUILayout.Button("Join"))
			{
				SetUserName();
			}
			GUILayout.FlexibleSpace();
			GUILayout.EndVertical();
		});
	}

	private void DrawChatScreen()
	{
		GUIHelper.DrawArea(GUIHelper.ClientArea, true, delegate
		{
			GUILayout.BeginVertical();
			scrollPos = GUILayout.BeginScrollView(scrollPos);
			GUILayout.Label(chatLog, GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));
			GUILayout.EndScrollView();
			string text = string.Empty;
			if (typingUsers.Count > 0)
			{
				text += string.Format("{0}", typingUsers[0]);
				for (int i = 1; i < typingUsers.Count; i++)
				{
					text += string.Format(", {0}", typingUsers[i]);
				}
				text = ((typingUsers.Count != 1) ? (text + " are typing!") : (text + " is typing!"));
			}
			GUILayout.Label(text);
			GUILayout.Label("Type here:");
			GUILayout.BeginHorizontal();
			message = GUILayout.TextField(message);
			if (GUILayout.Button("Send", GUILayout.MaxWidth(100f)))
			{
				SendMessage();
			}
			GUILayout.EndHorizontal();
			if (GUI.changed)
			{
				UpdateTyping();
			}
			GUILayout.EndVertical();
		});
	}

	private void SetUserName()
	{
		if (!string.IsNullOrEmpty(userName))
		{
			State = ChatStates.Chat;
			Manager.Socket.Emit("add user", userName);
		}
	}

	private void SendMessage()
	{
		if (!string.IsNullOrEmpty(message))
		{
			Manager.Socket.Emit("new message", message);
			chatLog += string.Format("{0}: {1}\n", userName, message);
			message = string.Empty;
		}
	}

	private void UpdateTyping()
	{
		if (!typing)
		{
			typing = true;
			Manager.Socket.Emit("typing");
		}
		lastTypingTime = DateTime.UtcNow;
	}

	private void addParticipantsMessage(Dictionary<string, object> data)
	{
		int num = Convert.ToInt32(data["numUsers"]);
		if (num == 1)
		{
			chatLog += "there's 1 participant\n";
			return;
		}
		string text = chatLog;
		chatLog = text + "there are " + num + " participants\n";
	}

	private void addChatMessage(Dictionary<string, object> data)
	{
		string arg = data["username"] as string;
		string arg2 = data["message"] as string;
		chatLog += string.Format("{0}: {1}\n", arg, arg2);
	}

	private void AddChatTyping(Dictionary<string, object> data)
	{
		string item = data["username"] as string;
		typingUsers.Add(item);
	}

	private void RemoveChatTyping(Dictionary<string, object> data)
	{
		string username = data["username"] as string;
		int num = typingUsers.FindIndex((string name) => name.Equals(username));
		if (num != -1)
		{
			typingUsers.RemoveAt(num);
		}
	}

	private void OnLogin(Socket socket, Packet packet, params object[] args)
	{
		chatLog = "Welcome to Socket.IO Chat — \n";
		addParticipantsMessage(args[0] as Dictionary<string, object>);
	}

	private void OnNewMessage(Socket socket, Packet packet, params object[] args)
	{
		addChatMessage(args[0] as Dictionary<string, object>);
	}

	private void OnUserJoined(Socket socket, Packet packet, params object[] args)
	{
		Dictionary<string, object> dictionary = args[0] as Dictionary<string, object>;
		string arg = dictionary["username"] as string;
		chatLog += string.Format("{0} joined\n", arg);
		addParticipantsMessage(dictionary);
	}

	private void OnUserLeft(Socket socket, Packet packet, params object[] args)
	{
		Dictionary<string, object> dictionary = args[0] as Dictionary<string, object>;
		string arg = dictionary["username"] as string;
		chatLog += string.Format("{0} left\n", arg);
		addParticipantsMessage(dictionary);
	}

	private void OnTyping(Socket socket, Packet packet, params object[] args)
	{
		AddChatTyping(args[0] as Dictionary<string, object>);
	}

	private void OnStopTyping(Socket socket, Packet packet, params object[] args)
	{
		RemoveChatTyping(args[0] as Dictionary<string, object>);
	}
}
