using System;
using System.Collections;
using System.Collections.Generic;
using Nekki.Vector.Core;
using Nekki.Vector.Core.DataValidation;
using Nekki.Vector.Core.Game;
using Nekki.Vector.Core.Notifications;
using Nekki.Vector.Core.Payment;
using SimpleJSON;
using UnityEngine;

public class ServerProvider : ServerProviderBase
{
	public delegate void ServerResponseDelegate(bool p_result, string p_data, object p_userData);

	private static ServerProvider _Instance;

	public string PutServer
	{
		get
		{
			return GetServer() + "/put.php";
		}
	}

	public static ServerProvider Instance
	{
		get
		{
			if (!_Instance)
			{
				_Instance = ServerProviderBase.Init<ServerProvider>();
				_Instance.Init();
			}
			return _Instance;
		}
	}

	private static string ProjectId
	{
		get
		{
			return (!ApplicationController.IsPaidBundleID) ? "vector2" : "vector2_paid";
		}
	}

	protected override string GetServer()
	{
		return string.Empty;
	}

	protected string GetVerifyServer()
	{
		return "https://payservice.nekkimobile.ru";
	}

	protected override void Init()
	{
	}

	private IEnumerator SendPutRequest(string p_server, WWWForm p_form, ServerResponseDelegate p_delegate = null, object p_userData = null)
	{
		Dictionary<string, string> headers = new Dictionary<string, string> { { "User-Agent", "Nekki-mobile" } };
		WWW www = new WWW(p_server, p_form.data, headers);
		yield return www;
		if (p_delegate == null)
		{
			yield break;
		}
		if (!string.IsNullOrEmpty(www.error))
		{
			p_delegate(false, www.error, p_userData);
			yield break;
		}
		try
		{
			p_delegate(true, www.text, p_userData);
		}
		catch (Exception ex)
		{
			p_delegate(false, ex.Message, p_userData);
			Debug.Log("Exception: " + ex.Message + "\nST: " + ex.StackTrace);
		}
	}

	public void OfferAction()
	{
		Form form = new Form();
		PutDeviceData(form);
		form.Add("action", "offer");
		form.Add("platform", DeviceInformation.Platform);
		form.Add("release", (!Settings.IsReleaseBuild) ? "0" : "1");
		form.Add("timezone", TimeManager.CurrentTimeZoneOffset.TotalHours);
		StartCoroutine(SendPutRequest(PutServer, form, ServerResponseReceiver.OfferResponse));
	}

	public void UserAction()
	{
		Form form = new Form();
		form.Add("action", "user");
		PutDeviceData(form);
		PutDeviceSpecsData(form);
		DebugUtils.Log(form.ToString());
		StartCoroutine(SendPutRequest(PutServer, form, ServerResponseReceiver.UserResponse));
	}

	public void CBT_AccessAction(string p_email, ServerResponseDelegate p_delegate)
	{
		Form form = new Form();
		form.Add("action", "cbt_access");
		form.Add("email", p_email);
		PutDeviceData(form);
		DebugUtils.Log(form.ToString());
		StartCoroutine(SendPutRequest(PutServer, form, p_delegate));
	}

	public void SaveJsonLogAction(string p_data, ServerResponseDelegate p_delegate)
	{
		Form form = new Form();
		form.Add("action", "save_json_log");
		form.Add("log", p_data);
		PutDeviceData(form);
		StartCoroutine(SendPutRequest(PutServer, form, p_delegate));
	}

	public void PurchaseAction(PaymentInfo p_info, string p_revenue)
	{
		Form form = new Form();
		form.Add("action", "payment");
		form.Add("payment_id", (p_info.PaymentID == null) ? "null" : p_info.PaymentID);
		form.Add("product_id", p_info.ProductID);
		form.Add("revenue", p_revenue);
		PutDeviceData(form);
		DebugUtils.Log(form.ToString());
		StartCoroutine(SendPutRequest(PutServer, form));
	}

	public void VerifyPurchaseAction(PaymentInfo p_payment, string p_receipt, string p_signature, string p_platform, ServerResponseDelegate p_delegate)
	{
		Form form = new Form();
		form.Add("cmd", "verifyReceipt");
		form.Add("platform", p_platform);
		form.Add("project", ProjectId);
		Form form2 = new Form();
		PutDeviceData(form2);
		JSONClass jSONClass = form2.ToJSONClass();
		jSONClass["receipt"] = p_receipt;
		jSONClass["pack_id"] = p_payment.ProductID;
		if (p_payment.Signature != null)
		{
			jSONClass["signature"] = p_payment.Signature;
		}
		jSONClass["device_id"] = DeviceInformation.GUID;
		form.Add("data", jSONClass.ToString());
		DebugUtils.Log(form.ToString());
		StartCoroutine(SendPutRequest(GetVerifyServer(), form, p_delegate, p_payment));
	}

	public void ConfirmVerificationAction(PaymentInfo p_payment, string p_platform, ServerResponseDelegate p_delegate)
	{
		Form form = new Form();
		form.Add("cmd", "confirmReceipt");
		form.Add("platform", p_platform);
		form.Add("project", ProjectId);
		Form form2 = new Form();
		PutDeviceData(form2);
		JSONClass jSONClass = form2.ToJSONClass();
		jSONClass["receiptId"] = p_payment.PaymentID;
		jSONClass["device_id"] = DeviceInformation.GUID;
		form.Add("data", jSONClass.ToString());
		DebugUtils.Log(form.ToString());
		StartCoroutine(SendPutRequest(GetVerifyServer(), form, p_delegate, p_payment));
	}

	private static void PutDeviceData(Form p_form)
	{
		p_form.Add("build_version", ApplicationController.BuildVersion);
		p_form.Add("data_version", ResourcesValidator.GamedataVersion);
		p_form.Add("type", DeviceInformation.DeviceType);
		p_form.Add("os", DeviceInformation.OS);
		p_form.Add("paid", (!ApplicationController.IsPaidBundleID) ? "0" : "1");
		if (DeviceInformation.IsAndroid)
		{
			string deviceUniqueIdentifier = SystemInfo.deviceUniqueIdentifier;
			p_form.Add("imei", deviceUniqueIdentifier);
		}
		p_form.Add("token", DeviceInformation.DeviceToken);
		p_form.Add("uniqueID", DeviceInformation.GetServerUniqueID);
	}

	private static void PutDeviceSpecsData(Form p_form)
	{
		string pushToken = RemoteNotifications.PushToken;
		if (!string.IsNullOrEmpty(pushToken))
		{
			p_form.Add("push_token", pushToken);
		}
		p_form.Add("dev", DeviceInformation.DeviceModel);
		p_form.Add("corecount", DeviceInformation.CoreCount);
		p_form.Add("ram", DeviceInformation.RamSize);
		p_form.Add("display_width", DeviceInformation.ScreenWidth);
		p_form.Add("display_height", DeviceInformation.ScreenHeight);
	}

	public void DownloadFile(string p_url, Action<byte[], string> p_onDownloadComplete)
	{
		if (InternetUtils.IsInternetAvailable)
		{
			StartCoroutine(DownloadFileRoutine(p_url, p_onDownloadComplete));
		}
		else
		{
			p_onDownloadComplete(new byte[0], "Internet is not available");
		}
	}

	private IEnumerator DownloadFileRoutine(string p_url, Action<byte[], string> p_onDownloadComplete)
	{
		WWW www = new WWW(p_url);
		while (!www.isDone)
		{
			yield return null;
		}
		p_onDownloadComplete(www.bytes, www.error);
	}
}
