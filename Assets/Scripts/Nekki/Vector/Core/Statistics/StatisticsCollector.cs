using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using SimpleJSON;
using UnityEngine;

namespace Nekki.Vector.Core.Statistics
{
	public class StatisticsCollector
	{
		private enum LoggerStatus
		{
			NotLogging = 0,
			Logging = 1,
			Undecided = 2
		}

		private const int MAX_LINE_IN_NOT_LOGGING = 200;

		private const string _LogFileName = "statistics.json";

		private const string _DataFileName = "statistics_data.xml";

		private const int _DeltaSendTime = 900000;

		private static StatisticsCollector _Current;

		private int _CurrentEventID;

		private int _RunCount;

		private int _SendDataLenght;

		private bool _RetrySendAfterResetFile;

		private LoggerStatus _Status = LoggerStatus.Undecided;

		private StringBuilder _Data = new StringBuilder();

		private long _LastSendDate;

		private static string PathLogFile
		{
			get
			{
				return VectorPaths.Statistics + "/statistics.json";
			}
		}

		private static string PathDataFile
		{
			get
			{
				return VectorPaths.Statistics + "/statistics_data.xml";
			}
		}

		public static StatisticsCollector Current
		{
			get
			{
				if (_Current == null)
				{
					_Current = new StatisticsCollector();
				}
				return _Current;
			}
		}

		public static int NextEventID
		{
			get
			{
				Current._CurrentEventID++;
				_Current.SaveDataFile();
				return _Current._CurrentEventID;
			}
		}

		private StatisticsCollector()
		{
			LoadDataFile();
			ApplicationController.OnAppPauseCallBack += OnApplicationPause;
			Send();
		}

		public static void SetEvent(StatisticsEvent.EventType p_type, ArgsDict args = null)
		{
			if (Current._Data.Length != 0)
			{
				_Current._Data.Append("\n");
			}
			Current._Data.Append(StatisticsEvent.GetJSONDataEvent(p_type, args));
			if (StatisticsEvent.IsSaveEvent(p_type))
			{
				_Current.SaveToFile();
			}
			if (StatisticsEvent.IsSendNow(p_type))
			{
				_Current.Send();
			}
		}

		public void SetLogger(bool p_logger)
		{
			if (p_logger)
			{
				_Status = LoggerStatus.Logging;
				Send();
			}
			else
			{
				_Status = LoggerStatus.NotLogging;
			}
		}

		private void LoadDataFile()
		{
			if (!File.Exists(PathDataFile))
			{
				_CurrentEventID = 0;
				_RunCount = 0;
				return;
			}
			XmlDocument xmlDocument = XmlUtils.OpenXMLDocument(PathDataFile, string.Empty, XmlUtils.OpenXmlType.ForcedExternal);
			XmlNode xmlNode = xmlDocument["Data"];
			_CurrentEventID = XmlUtils.ParseInt(xmlNode["EventID"].Attributes["Value"]);
			_RunCount = XmlUtils.ParseInt(xmlNode["RunCount"].Attributes["Value"]);
		}

		private void SaveDataFile()
		{
			XmlDocument xmlDocument = new XmlDocument();
			xmlDocument.AppendChild(xmlDocument.CreateXmlDeclaration("1.0", "UTF-8", null));
			XmlElement xmlElement = xmlDocument.CreateElement("Data");
			xmlDocument.AppendChild(xmlElement);
			XmlElement xmlElement2 = xmlDocument.CreateElement("EventID");
			xmlElement.AppendChild(xmlElement2);
			xmlElement2.SetAttribute("Value", _CurrentEventID.ToString());
			XmlElement xmlElement3 = xmlDocument.CreateElement("RunCount");
			xmlElement.AppendChild(xmlElement3);
			xmlElement3.SetAttribute("Value", _RunCount.ToString());
			xmlDocument.Save(PathDataFile);
		}

		private void SaveToFile()
		{
			if (_Data.Length == 0)
			{
				return;
			}
			string value = _Data.ToString();
			_Data.Length = 0;
			try
			{
				FileInfo fileInfo = new FileInfo(PathLogFile);
				if (!fileInfo.Exists)
				{
					FileStream fileStream = fileInfo.Create();
					fileStream.Close();
				}
				StreamWriter streamWriter = fileInfo.AppendText();
				if (fileInfo.Exists && fileInfo.Length != 0L)
				{
					streamWriter.Write("\n");
				}
				streamWriter.Write(value);
				streamWriter.Close();
			}
			catch (Exception ex)
			{
				Debug.Log(ex.Message);
			}
		}

		private void Send()
		{
			if (_SendDataLenght != 0)
			{
				_RetrySendAfterResetFile = true;
				return;
			}
			if (_Status == LoggerStatus.NotLogging || _Status == LoggerStatus.Undecided)
			{
				if (_Status == LoggerStatus.NotLogging)
				{
					SliceFile();
				}
				return;
			}
			_LastSendDate = TimeManager.UTCTime;
			SaveToFile();
			FileInfo fileInfo = new FileInfo(PathLogFile);
			if (fileInfo.Exists)
			{
				StreamReader streamReader = fileInfo.OpenText();
				string text = streamReader.ReadToEnd();
				streamReader.Close();
				_SendDataLenght = text.Length;
				if (_SendDataLenght != 0)
				{
					ServerProvider.Instance.SaveJsonLogAction(text, OnServerResponse);
				}
			}
		}

		public void OnServerResponse(bool p_result, string p_data, object p_userData)
		{
			if (p_result)
			{
				JSONNode jSONNode = JSON.Parse(p_data);
				if (jSONNode != null && jSONNode["data"] != null && jSONNode["data"].Value == "ok")
				{
					ResetFile();
				}
				else
				{
					_SendDataLenght = 0;
				}
			}
			else
			{
				_SendDataLenght = 0;
			}
		}

		private void ResetFile()
		{
			SaveToFile();
			FileInfo fileInfo = new FileInfo(PathLogFile);
			if (!fileInfo.Exists)
			{
				return;
			}
			StreamReader streamReader = fileInfo.OpenText();
			string text = streamReader.ReadToEnd();
			streamReader.Close();
			if (_SendDataLenght == text.Length)
			{
				fileInfo.Create().Close();
				_SendDataLenght = 0;
				_RetrySendAfterResetFile = false;
				return;
			}
			if (text[_SendDataLenght] == '\n')
			{
				_SendDataLenght++;
			}
			_Data.Append(text.Substring(_SendDataLenght));
			_SendDataLenght = 0;
			SaveToFile();
			if (_RetrySendAfterResetFile)
			{
				Send();
			}
		}

		private void SliceFile()
		{
			if (!File.Exists(PathLogFile))
			{
				return;
			}
			string[] array = File.ReadAllLines(PathLogFile);
			if (array.Length <= 200)
			{
				return;
			}
			int count = array.Length - 200;
			IEnumerable<string> enumerable = array.Skip(count);
			StringBuilder stringBuilder = new StringBuilder();
			foreach (string item in enumerable)
			{
				stringBuilder.AppendLine(item);
			}
			stringBuilder.Length--;
			File.WriteAllText(PathLogFile, stringBuilder.ToString());
			_LastSendDate = TimeManager.UTCTime;
		}

		public void OnApplicationPause(bool p_pause)
		{
			if (!p_pause && TimeManager.UTCTime - _LastSendDate > 900000)
			{
				Send();
			}
		}
	}
}
