using System;
using BestHTTP.SignalR;
using BestHTTP.SignalR.Hubs;
using BestHTTP.SignalR.JsonEncoders;
using BestHTTP.SignalR.Messages;
using UnityEngine;

internal class DemoHubSample : MonoBehaviour
{
	private readonly Uri URI = new Uri("http://besthttpsignalr.azurewebsites.net/signalr");

	private Connection signalRConnection;

	private DemoHub demoHub;

	private TypedDemoHub typedDemoHub;

	private Hub vbDemoHub;

	private string vbReadStateResult = string.Empty;

	private Vector2 scrollPos;

	private void Start()
	{
		demoHub = new DemoHub();
		typedDemoHub = new TypedDemoHub();
		vbDemoHub = new Hub("vbdemo");
		signalRConnection = new Connection(URI, demoHub, typedDemoHub, vbDemoHub);
		signalRConnection.JsonEncoder = new LitJsonEncoder();
		signalRConnection.OnConnected += delegate
		{
			var anon = new
			{
				Name = "Foo",
				Age = 20,
				Address = new
				{
					Street = "One Microsoft Way",
					Zip = "98052"
				}
			};
			demoHub.ReportProgress("Long running job!");
			demoHub.AddToGroups();
			demoHub.GetValue();
			demoHub.TaskWithException();
			demoHub.GenericTaskWithException();
			demoHub.SynchronousException();
			demoHub.DynamicTask();
			demoHub.PassingDynamicComplex(anon);
			demoHub.SimpleArray(new int[3] { 5, 5, 6 });
			demoHub.ComplexType(anon);
			demoHub.ComplexArray(new object[3] { anon, anon, anon });
			demoHub.Overload();
			demoHub.State["name"] = "Testing state!";
			demoHub.ReadStateValue();
			demoHub.PlainTask();
			demoHub.GenericTaskWithContinueWith();
			typedDemoHub.Echo("Typed echo callback");
			vbDemoHub.Call("readStateValue", delegate(Hub hub, ClientMessage msg, ResultMessage result)
			{
				vbReadStateResult = string.Format("Read some state from VB.NET! => {0}", (result.ReturnValue != null) ? result.ReturnValue.ToString() : "undefined");
			});
		};
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
			demoHub.Draw();
			typedDemoHub.Draw();
			GUILayout.Label("Read State Value");
			GUILayout.BeginHorizontal();
			GUILayout.Space(20f);
			GUILayout.Label(vbReadStateResult);
			GUILayout.EndHorizontal();
			GUILayout.Space(10f);
			GUILayout.EndVertical();
			GUILayout.EndScrollView();
		});
	}
}
