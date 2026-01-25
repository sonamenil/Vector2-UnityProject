using System.Collections.Generic;
using BestHTTP.SignalR.Hubs;
using BestHTTP.SignalR.Messages;
using UnityEngine;

internal class BaseHub : Hub
{
	private string Title;

	private GUIMessageList messages = new GUIMessageList();

	public BaseHub(string name, string title)
		: base(name)
	{
		Title = title;
		On("joined", Joined);
		On("rejoined", Rejoined);
		On("left", Left);
		On("invoked", Invoked);
	}

	private void Joined(Hub hub, MethodCallMessage methodCall)
	{
		Dictionary<string, object> dictionary = methodCall.Arguments[2] as Dictionary<string, object>;
		messages.Add(string.Format("{0} joined at {1}\n\tIsAuthenticated: {2} IsAdmin: {3} UserName: {4}", methodCall.Arguments[0], methodCall.Arguments[1], dictionary["IsAuthenticated"], dictionary["IsAdmin"], dictionary["UserName"]));
	}

	private void Rejoined(Hub hub, MethodCallMessage methodCall)
	{
		messages.Add(string.Format("{0} reconnected at {1}", methodCall.Arguments[0], methodCall.Arguments[1]));
	}

	private void Left(Hub hub, MethodCallMessage methodCall)
	{
		messages.Add(string.Format("{0} left at {1}", methodCall.Arguments[0], methodCall.Arguments[1]));
	}

	private void Invoked(Hub hub, MethodCallMessage methodCall)
	{
		messages.Add(string.Format("{0} invoked hub method at {1}", methodCall.Arguments[0], methodCall.Arguments[1]));
	}

	public void InvokedFromClient()
	{
		Call("invokedFromClient", OnInvoked, OnInvokeFailed);
	}

	private void OnInvoked(Hub hub, ClientMessage originalMessage, ResultMessage result)
	{
		AdvLog.Log(hub.Name + " invokedFromClient success!");
	}

	private void OnInvokeFailed(Hub hub, ClientMessage originalMessage, FailureMessage result)
	{
		AdvLog.LogWarning(hub.Name + " " + result.ErrorMessage);
	}

	public void Draw()
	{
		GUILayout.Label(Title);
		GUILayout.BeginHorizontal();
		GUILayout.Space(20f);
		messages.Draw(Screen.width - 20, 100f);
		GUILayout.EndHorizontal();
	}
}
