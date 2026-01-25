using System;
using System.IO;
using System.Net;
using System.Net.Mail;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using UnityEngine;

public static class AdvLog
{
	public enum LogLevel
	{
		Log = 0,
		Warn = 1,
		Error = 2
	}

	private static bool _logNow;

	private static string _filePath;

	public static bool LogNow
	{
		get
		{
			return _logNow;
		}
		set
		{
			if (_logNow != value)
			{
				_logNow = value;
				if (_logNow)
				{
					Init();
				}
			}
		}
	}

	private static void Init()
	{
		if (string.IsNullOrEmpty(_filePath))
		{
			Application.logMessageReceived += ApplicationLogSubsctiption;
			_filePath = Path.Combine(GlobalPaths.ExternalPath, string.Format("log_{0}.log", DateTime.Now.ToString("yy_MM_dd__hh_mm_ss")));
			File.WriteAllText(_filePath, string.Empty);
		}
	}

	private static void ApplicationLogSubsctiption(string condition, string stacktrace, LogType type)
	{
		switch (type)
		{
		case LogType.Log:
			LogMessage(LogLevel.Log, condition + " - " + stacktrace);
			break;
		case LogType.Warning:
			LogMessage(LogLevel.Warn, condition + " - " + stacktrace);
			break;
		case LogType.Error:
		case LogType.Assert:
		case LogType.Exception:
			LogMessage(LogLevel.Error, condition + " - " + stacktrace);
			break;
		}
	}

	public static void EmailLog(string mailAddres, string displayName)
	{
		if (!string.IsNullOrEmpty(_filePath))
		{
			MailMessage mailMessage = new MailMessage();
			mailMessage.From = new MailAddress("logs@nekkimobile.ru", displayName);
			mailMessage.To.Add(mailAddres);
			mailMessage.Attachments.Add(new Attachment(_filePath));
			mailMessage.Subject = string.Format("Log from [{0}:{1}:{2}] {3}", SystemInfo.deviceModel, SystemInfo.deviceName, SystemInfo.deviceType, SystemInfo.deviceUniqueIdentifier);
			mailMessage.Body = "see log in attachment";
			SmtpClient smtpClient = new SmtpClient("mail.nekkimobile.ru");
			smtpClient.Port = 587;
			smtpClient.Credentials = new NetworkCredential("logs@nekkimobile.ru", "o99hSASo");
			smtpClient.EnableSsl = true;
			ServicePointManager.ServerCertificateValidationCallback = (object s, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors) => true;
			smtpClient.Send(mailMessage);
			Debug.Log("success");
		}
	}

	private static void LogMessage(LogLevel level, object message)
	{
		if (message != null && !string.IsNullOrEmpty(message.ToString()) && LogNow)
		{
			PushToFile(string.Format("[{0}:{1}] {2}", DateTime.Now, level, message));
		}
	}

	private static void PushToFile(string message)
	{
		File.AppendAllText(_filePath, message);
	}

	public static void Log(object message)
	{
		if (LogNow)
		{
			Debug.Log(message);
		}
	}

	public static void Log(object message, UnityEngine.Object context)
	{
		if (LogNow)
		{
			Debug.Log(message, context);
		}
	}

	public static void LogFormat(UnityEngine.Object context, string format, params object[] args)
	{
		if (LogNow)
		{
			Debug.LogFormat(context, format, args);
		}
	}

	public static void LogFormat(string format, params object[] args)
	{
		if (LogNow)
		{
			Debug.LogFormat(format, args);
		}
	}

	public static void LogWarning(object message)
	{
		if (LogNow)
		{
			Debug.LogWarning(message);
		}
	}

	public static void LogWarning(object message, UnityEngine.Object context)
	{
		if (LogNow)
		{
			Debug.LogWarning(message, context);
		}
	}

	public static void LogWarningFormat(UnityEngine.Object context, string format, params object[] args)
	{
		if (LogNow)
		{
			Debug.LogWarningFormat(context, format, args);
		}
	}

	public static void LogWarningFormat(string format, params object[] args)
	{
		if (LogNow)
		{
			Debug.LogWarningFormat(format, args);
		}
	}

	public static void LogError(object message)
	{
		if (LogNow)
		{
			Debug.LogError(message);
		}
	}

	public static void LogError(object message, UnityEngine.Object context)
	{
		if (LogNow)
		{
			Debug.LogError(message, context);
		}
	}

	public static void LogErrorFormat(UnityEngine.Object context, string format, params object[] args)
	{
		if (LogNow)
		{
			Debug.LogErrorFormat(context, format, args);
		}
	}

	public static void LogErrorFormat(string format, params object[] args)
	{
		if (LogNow)
		{
			Debug.LogErrorFormat(format, args);
		}
	}

	public static void LogException(Exception ex)
	{
		if (LogNow)
		{
			Debug.LogException(ex);
		}
	}

	public static void LogException(Exception ex, UnityEngine.Object context)
	{
		if (LogNow)
		{
			Debug.LogException(ex, context);
		}
	}

	public static void Assert(bool condition)
	{
		if (LogNow)
		{
		}
	}

	public static void Assert(bool condition, string message)
	{
		if (LogNow)
		{
		}
	}

	public static void Assert(bool condition, string format, params object[] args)
	{
		if (LogNow)
		{
		}
	}
}
