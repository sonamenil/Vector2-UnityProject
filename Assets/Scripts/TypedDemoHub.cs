using BestHTTP.SignalR.Hubs;
using BestHTTP.SignalR.Messages;
using UnityEngine;

internal class TypedDemoHub : Hub
{
	private string typedEchoResult = string.Empty;

	private string typedEchoClientResult = string.Empty;

	public TypedDemoHub()
		: base("typeddemohub")
	{
		On("Echo", Echo);
	}

	private void Echo(Hub hub, MethodCallMessage methodCall)
	{
		typedEchoClientResult = string.Format("{0} #{1} triggered!", methodCall.Arguments[0], methodCall.Arguments[1]);
	}

	public void Echo(string msg)
	{
		Call("echo", OnEcho_Done, msg);
	}

	private void OnEcho_Done(Hub hub, ClientMessage originalMessage, ResultMessage result)
	{
		typedEchoResult = "TypedDemoHub.Echo(string message) invoked!";
	}

	public void Draw()
	{
		GUILayout.Label("Typed callback");
		GUILayout.BeginHorizontal();
		GUILayout.Space(20f);
		GUILayout.BeginVertical();
		GUILayout.Label(typedEchoResult);
		GUILayout.Label(typedEchoClientResult);
		GUILayout.EndVertical();
		GUILayout.EndHorizontal();
		GUILayout.Space(10f);
	}
}
