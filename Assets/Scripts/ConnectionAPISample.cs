using System;
using BestHTTP.Cookies;
using BestHTTP.JSON;
using BestHTTP.SignalR;
using BestHTTP.SignalR.JsonEncoders;
using UnityEngine;

public sealed class ConnectionAPISample : MonoBehaviour
{
	private enum MessageTypes
	{
		Send = 0,
		Broadcast = 1,
		Join = 2,
		PrivateMessage = 3,
		AddToGroup = 4,
		RemoveFromGroup = 5,
		SendToGroup = 6,
		BroadcastExceptMe = 7
	}

	private readonly Uri URI = new Uri("http://besthttpsignalr.azurewebsites.net/raw-connection/");

	private Connection signalRConnection;

	private string ToEveryBodyText = string.Empty;

	private string ToMeText = string.Empty;

	private string PrivateMessageText = string.Empty;

	private string PrivateMessageUserOrGroupName = string.Empty;

	private GUIMessageList messages = new GUIMessageList();

	private void Start()
	{
		if (PlayerPrefs.HasKey("userName"))
		{
			CookieJar.Set(URI, new Cookie("user", PlayerPrefs.GetString("userName")));
		}
		signalRConnection = new Connection(URI);
		signalRConnection.JsonEncoder = new LitJsonEncoder();
		signalRConnection.OnStateChanged += signalRConnection_OnStateChanged;
		signalRConnection.OnNonHubMessage += signalRConnection_OnGeneralMessage;
		signalRConnection.Open();
	}

	private void OnGUI()
	{
		GUIHelper.DrawArea(GUIHelper.ClientArea, true, delegate
		{
			GUILayout.BeginVertical();
			GUILayout.Label("To Everybody");
			GUILayout.BeginHorizontal();
			ToEveryBodyText = GUILayout.TextField(ToEveryBodyText, GUILayout.MinWidth(100f));
			if (GUILayout.Button("Broadcast"))
			{
				Broadcast(ToEveryBodyText);
			}
			if (GUILayout.Button("Broadcast (All Except Me)"))
			{
				BroadcastExceptMe(ToEveryBodyText);
			}
			if (GUILayout.Button("Enter Name"))
			{
				EnterName(ToEveryBodyText);
			}
			if (GUILayout.Button("Join Group"))
			{
				JoinGroup(ToEveryBodyText);
			}
			if (GUILayout.Button("Leave Group"))
			{
				LeaveGroup(ToEveryBodyText);
			}
			GUILayout.EndHorizontal();
			GUILayout.Label("To Me");
			GUILayout.BeginHorizontal();
			ToMeText = GUILayout.TextField(ToMeText, GUILayout.MinWidth(100f));
			if (GUILayout.Button("Send to me"))
			{
				SendToMe(ToMeText);
			}
			GUILayout.EndHorizontal();
			GUILayout.Label("Private Message");
			GUILayout.BeginHorizontal();
			GUILayout.Label("Message:");
			PrivateMessageText = GUILayout.TextField(PrivateMessageText, GUILayout.MinWidth(100f));
			GUILayout.Label("User or Group name:");
			PrivateMessageUserOrGroupName = GUILayout.TextField(PrivateMessageUserOrGroupName, GUILayout.MinWidth(100f));
			if (GUILayout.Button("Send to user"))
			{
				SendToUser(PrivateMessageUserOrGroupName, PrivateMessageText);
			}
			if (GUILayout.Button("Send to group"))
			{
				SendToGroup(PrivateMessageUserOrGroupName, PrivateMessageText);
			}
			GUILayout.EndHorizontal();
			GUILayout.Space(20f);
			if (signalRConnection.State == ConnectionStates.Closed)
			{
				if (GUILayout.Button("Start Connection"))
				{
					signalRConnection.Open();
				}
			}
			else if (GUILayout.Button("Stop Connection"))
			{
				signalRConnection.Close();
			}
			GUILayout.Space(20f);
			GUILayout.Label("Messages");
			GUILayout.BeginHorizontal();
			GUILayout.Space(20f);
			messages.Draw(Screen.width - 20, 0f);
			GUILayout.EndHorizontal();
			GUILayout.EndVertical();
		});
	}

	private void OnDestroy()
	{
		signalRConnection.Close();
	}

	private void signalRConnection_OnGeneralMessage(Connection manager, object data)
	{
		string text = Json.Encode(data);
		messages.Add("[Server Message] " + text);
	}

	private void signalRConnection_OnStateChanged(Connection manager, ConnectionStates oldState, ConnectionStates newState)
	{
		messages.Add(string.Format("[State Change] {0} => {1}", oldState.ToString(), newState.ToString()));
	}

	private void Broadcast(string text)
	{
		signalRConnection.Send(new
		{
			Type = MessageTypes.Broadcast,
			Value = text
		});
	}

	private void BroadcastExceptMe(string text)
	{
		signalRConnection.Send(new
		{
			Type = MessageTypes.BroadcastExceptMe,
			Value = text
		});
	}

	private void EnterName(string name)
	{
		signalRConnection.Send(new
		{
			Type = MessageTypes.Join,
			Value = name
		});
	}

	private void JoinGroup(string groupName)
	{
		signalRConnection.Send(new
		{
			Type = MessageTypes.AddToGroup,
			Value = groupName
		});
	}

	private void LeaveGroup(string groupName)
	{
		signalRConnection.Send(new
		{
			Type = MessageTypes.RemoveFromGroup,
			Value = groupName
		});
	}

	private void SendToMe(string text)
	{
		signalRConnection.Send(new
		{
			Type = MessageTypes.Send,
			Value = text
		});
	}

	private void SendToUser(string userOrGroupName, string text)
	{
		signalRConnection.Send(new
		{
			Type = MessageTypes.PrivateMessage,
			Value = string.Format("{0}|{1}", userOrGroupName, text)
		});
	}

	private void SendToGroup(string userOrGroupName, string text)
	{
		signalRConnection.Send(new
		{
			Type = MessageTypes.SendToGroup,
			Value = string.Format("{0}|{1}", userOrGroupName, text)
		});
	}
}
