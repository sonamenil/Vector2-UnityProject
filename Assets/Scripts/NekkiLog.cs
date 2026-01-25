using System;
using System.Collections;
using System.IO;
using System.Text;
using UnityEngine;

public class NekkiLog : MonoBehaviour
{
	public enum LogLevels : byte
	{
		Log = 0,
		Warning = 1,
		Error = 2,
		Exception = 3,
		Assert = 4
	}

	private static string _pathToLog = string.Empty;

	private static StringBuilder _items = new StringBuilder();

	private static DateTime _now;

	private static NekkiLog _instance;

	private static bool _catchSystemLog;

	private static string _fileName = "log";

	private static bool _stoped = true;

	private static bool _dateInFileName = true;

	private static bool _timeInEachRecord = true;

	private static LogLevels _minimumLevel = LogLevels.Log;

	private bool _writting;

	private static readonly object LOCKER = new object();

	private static string FileName
	{
		get
		{
			if (!_dateInFileName)
			{
				return string.Format("{0}.nekkilog", _fileName);
			}
			return string.Format("{3} {0}.{1}.{2}.nekkilog", _now.Year.ToString("0000"), _now.Month.ToString("00"), _now.Day.ToString("00"), _fileName);
		}
	}

	private static string Path
	{
		get
		{
			if (string.IsNullOrEmpty(_pathToLog))
			{
				return System.IO.Path.Combine(Application.persistentDataPath, FileName);
			}
			return System.IO.Path.Combine(_pathToLog, FileName);
		}
	}

	public static void Init(string pathToLog, string fileName, bool catchSystemLog, bool dateInFileName, bool timeInEachRecord, LogLevels minimumLevel)
	{
		if ((bool)_instance)
		{
			_instance.Write();
		}
		_pathToLog = pathToLog.TrimEnd('/').TrimEnd('\\');
		_minimumLevel = minimumLevel;
		_dateInFileName = dateInFileName;
		_timeInEachRecord = timeInEachRecord;
		_catchSystemLog = catchSystemLog;
		if (!string.IsNullOrEmpty(fileName))
		{
			_fileName = fileName;
		}
		CheckPersistance();
		_stoped = false;
		if (!Directory.Exists(pathToLog))
		{
			Directory.CreateDirectory(pathToLog);
		}
		FileInfo fileInfo = new FileInfo(Path);
		if (!fileInfo.Exists)
		{
			FileStream fileStream = fileInfo.Create();
			fileStream.Close();
		}
	}

	private static void CheckPersistance()
	{
		if (!_instance)
		{
			GameObject gameObject = new GameObject("_log");
			_instance = gameObject.AddComponent<NekkiLog>();
			UnityEngine.Object.DontDestroyOnLoad(gameObject);
			_instance.Update();
			_instance.StartCoroutine(_instance.SaveToFile());
			Application.logMessageReceived += _unityLogCallback;
		}
	}

	private void OnDestroy()
	{
		Stop();
	}

	private void OnEnable()
	{
		StopAllCoroutines();
		StartCoroutine(SaveToFile());
	}

	private static void _unityLogCallback(string condition, string stacktrace, LogType type)
	{
		if (_catchSystemLog)
		{
			switch (type)
			{
			case LogType.Error:
				Error(condition, stacktrace);
				break;
			case LogType.Assert:
				Assert(condition, stacktrace);
				break;
			case LogType.Warning:
				Warning(condition, stacktrace);
				break;
			case LogType.Log:
				Log(condition, stacktrace);
				break;
			case LogType.Exception:
				Exception(condition, stacktrace);
				break;
			}
		}
	}

	private void Update()
	{
		if (_timeInEachRecord || _dateInFileName)
		{
			_now = DateTime.Now;
		}
	}

	public static void Log(object message, string stacktrace = null)
	{
		PostMesage(LogLevels.Log, message, stacktrace);
	}

	public static void Warning(object message, string stacktrace = null)
	{
		PostMesage(LogLevels.Warning, message, stacktrace);
	}

	public static void Error(object message, string stacktrace = null)
	{
		PostMesage(LogLevels.Error, message, stacktrace);
	}

	public static void Exception(Exception ex)
	{
		PostMesage(LogLevels.Exception, ex.Message, ex.StackTrace);
	}

	private static void Exception(object message, string stacktrace)
	{
		PostMesage(LogLevels.Exception, message, stacktrace);
	}

	public static void Assert(object message, string stacktrace = null)
	{
		PostMesage(LogLevels.Assert, message, stacktrace);
	}

	public static void Stop()
	{
		if (!(_instance == null))
		{
			_instance.Write();
			_stoped = true;
		}
	}

	private static string FormatMessage(LogLevels level, object condition, string stacktrace = null)
	{
		object obj = ((!string.IsNullOrEmpty(stacktrace)) ? string.Format("{0} at: {1}", condition, stacktrace) : condition);
		if (!_timeInEachRecord)
		{
			return string.Format("{0}\n", obj);
		}
		return string.Format("[{3}] [{0}:{1}:{2}] {4}\n", _now.Hour.ToString("00"), _now.Minute.ToString("00"), _now.Second.ToString("00"), level, obj);
	}

	private static void PostMesage(LogLevels level, object message, string stacktrace = null)
	{
		if (_stoped)
		{
			AdvLog.LogWarning("you must init log system first!");
		}
		else if ((int)level >= (int)_minimumLevel)
		{
			CheckPersistance();
			lock (LOCKER)
			{
				_items.Append(FormatMessage(level, message, stacktrace));
			}
		}
	}

	private IEnumerator SaveToFile()
	{
		while ((bool)base.transform)
		{
			yield return new WaitForSeconds(1f);
			Write();
		}
	}

	private void Write()
	{
		if (_writting)
		{
			return;
		}
		_writting = true;
		string value;
		lock (LOCKER)
		{
			value = _items.ToString();
			_items = new StringBuilder();
		}
		try
		{
			FileInfo fileInfo = new FileInfo(Path);
			if (!fileInfo.Exists)
			{
				FileStream fileStream = fileInfo.Create();
				fileStream.Close();
			}
			StreamWriter streamWriter = fileInfo.AppendText();
			streamWriter.Write(value);
			streamWriter.Close();
		}
		catch (Exception ex)
		{
			MonoBehaviour.print(ex.Message);
		}
		_writting = false;
	}
}
