using System;
using BestHTTP.SignalR;
using BestHTTP.SignalR.Authentication;
using UnityEngine;

internal class AuthenticationSample : MonoBehaviour
{
	private readonly Uri URI = new Uri("https://besthttpsignalr.azurewebsites.net/signalr");

	private Connection signalRConnection;

	private string userName = string.Empty;

	private string role = string.Empty;

	private Vector2 scrollPos;

	private void Start()
	{
		signalRConnection = new Connection(URI, new BaseHub("noauthhub", "Messages"), new BaseHub("invokeauthhub", "Messages Invoked By Admin or Invoker"), new BaseHub("authhub", "Messages Requiring Authentication to Send or Receive"), new BaseHub("inheritauthhub", "Messages Requiring Authentication to Send or Receive Because of Inheritance"), new BaseHub("incomingauthhub", "Messages Requiring Authentication to Send"), new BaseHub("adminauthhub", "Messages Requiring Admin Membership to Send or Receive"), new BaseHub("userandroleauthhub", "Messages Requiring Name to be \"User\" and Role to be \"Admin\" to Send or Receive"));
		if (!string.IsNullOrEmpty(userName) && !string.IsNullOrEmpty(role))
		{
			signalRConnection.AuthenticationProvider = new HeaderAuthenticator(userName, role);
		}
		signalRConnection.OnConnected += signalRConnection_OnConnected;
		signalRConnection.Open();
	}

	private void OnDestroy()
	{
		signalRConnection.Close();
	}

	private void OnGUI()
	{
		GUIHelper.DrawArea(GUIHelper.ClientArea, true, delegate
		{
			scrollPos = GUILayout.BeginScrollView(scrollPos, false, false);
			GUILayout.BeginVertical();
			if (signalRConnection.AuthenticationProvider == null)
			{
				GUILayout.BeginHorizontal();
				GUILayout.Label("Username (Enter 'User'):");
				userName = GUILayout.TextField(userName, GUILayout.MinWidth(100f));
				GUILayout.EndHorizontal();
				GUILayout.BeginHorizontal();
				GUILayout.Label("Roles (Enter 'Invoker' or 'Admin'):");
				role = GUILayout.TextField(role, GUILayout.MinWidth(100f));
				GUILayout.EndHorizontal();
				if (GUILayout.Button("Log in"))
				{
					Restart();
				}
			}
			for (int i = 0; i < signalRConnection.Hubs.Length; i++)
			{
				(signalRConnection.Hubs[i] as BaseHub).Draw();
			}
			GUILayout.EndVertical();
			GUILayout.EndScrollView();
		});
	}

	private void signalRConnection_OnConnected(Connection manager)
	{
		for (int i = 0; i < signalRConnection.Hubs.Length; i++)
		{
			(signalRConnection.Hubs[i] as BaseHub).InvokedFromClient();
		}
	}

	private void Restart()
	{
		signalRConnection.OnConnected -= signalRConnection_OnConnected;
		signalRConnection.Close();
		signalRConnection = null;
		Start();
	}
}
