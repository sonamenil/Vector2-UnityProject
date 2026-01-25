using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using Nekki.Social;
using Newtonsoft.Json;
using SimpleJSON;
using UnityEngine;

public abstract class ServerProviderBase : MonoBehaviour
{
	public class Select
	{
		private string _table;

		private Condition[] _conditions;

		private string[] _order;

		private int? _limit;

		public string[] Fields { get; private set; }

		public Select(string table, string[] fields, Condition[] conditions, string[] order, int? limit)
		{
			_table = table;
			Fields = fields;
			_conditions = conditions;
			_order = order;
			_limit = limit;
		}

		private Form ToWebString()
		{
			Form form = new Form();
			form.Add("table", _table);
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("[");
			for (int i = 0; i < Fields.Length; i++)
			{
				stringBuilder.Append(string.Format("\"{0}\"", Fields[i]));
				if (i < Fields.Length - 1)
				{
					stringBuilder.Append(",");
				}
			}
			stringBuilder.Append("]");
			form.Add("fields", stringBuilder.ToString());
			if (_conditions != null)
			{
				form.Add("where", Condition.ToWebString(_conditions));
			}
			if (_order != null)
			{
				StringBuilder stringBuilder2 = new StringBuilder();
				stringBuilder2.Append("[");
				for (int j = 0; j < _order.Length; j++)
				{
					stringBuilder2.Append(string.Format("\"{0}\"", _order[j]));
					if (j < _order.Length - 1)
					{
						stringBuilder2.Append(",");
					}
				}
				stringBuilder2.Append("]");
				form.Add("order", stringBuilder2.ToString());
			}
			int? limit = _limit;
			if (limit.HasValue)
			{
				form.Add("limit", _limit.Value);
			}
			return form;
		}

		public static implicit operator WWWForm(Select query)
		{
			return query.ToWebString();
		}
	}

	public class Condition
	{
		private string field;

		private string sign;

		private string value;

		private Condition()
		{
		}

		public static Condition Greather(string field, object value)
		{
			Condition condition = new Condition();
			condition.field = field;
			condition.sign = ">";
			condition.value = value.ToString();
			return condition;
		}

		public static Condition Lower(string field, object value)
		{
			Condition condition = new Condition();
			condition.field = field;
			condition.sign = "<";
			condition.value = value.ToString();
			return condition;
		}

		public static Condition Equals(string field, object value)
		{
			Condition condition = new Condition();
			condition.field = field;
			condition.sign = "=";
			condition.value = value.ToString();
			return condition;
		}

		public static Condition Raw(string field, string sig, object value)
		{
			Condition condition = new Condition();
			condition.field = field;
			condition.sign = sig;
			condition.value = value.ToString();
			return condition;
		}

		public string ToWebString()
		{
			return string.Format("[\"{0}\",\"{1}\",\"{2}\"]", field, sign, value);
		}

		public static string ToWebString(Condition[] conditions)
		{
			string text = "[";
			for (int i = 0; i < conditions.Length; i++)
			{
				text += conditions[i].ToWebString();
				if (i != conditions.Length - 1)
				{
					text += ",";
				}
			}
			return text + "]";
		}
	}

	public class Value
	{
		private readonly string _value;

		public string AsString
		{
			get
			{
				return _value;
			}
		}

		public float AsFloat
		{
			get
			{
				return float.Parse(_value);
			}
		}

		public int AsInt
		{
			get
			{
				return int.Parse(_value);
			}
		}

		public Value(string value)
		{
			_value = value ?? string.Empty;
		}

		public override string ToString()
		{
			return AsString;
		}
	}

	public class Row
	{
		private readonly Dictionary<string, Value> _content;

		public Value this[string index]
		{
			get
			{
				if (!_content.ContainsKey(index.ToLower()))
				{
					AdvLog.LogWarning("there no data for key " + index.ToLower());
				}
				return _content[index.ToLower()];
			}
		}

		public Row(Dictionary<string, Value> content)
		{
			_content = content;
		}
	}

	public class QResponse
	{
		private readonly List<Row> _data = new List<Row>();

		public Row this[int index]
		{
			get
			{
				return _data[index];
			}
		}

		public int Count
		{
			get
			{
				return _data.Count;
			}
		}

		public bool Empty
		{
			get
			{
				return _data.Count == 0;
			}
		}

		public QResponse(JSONArray json, Select query)
		{
			for (int i = 0; i < json.Count; i++)
			{
				Dictionary<string, Value> dictionary = new Dictionary<string, Value>();
				for (int j = 0; j < query.Fields.Length; j++)
				{
					dictionary.Add(query.Fields[j].ToLower(), new Value(json[i].AsArray[j]));
				}
				_data.Add(new Row(dictionary));
			}
		}
	}

	public class Form
	{
		private readonly List<KeyValuePair<string, string>> _data = new List<KeyValuePair<string, string>>();

		private static string _key = "DGgim7dg7cbknRCxVOAlXfGVtjOPyZls";

		public Form()
		{
			Add("rand", UnityEngine.Random.Range(0, int.MaxValue));
		}

		public void Add(string key, object value)
		{
			_data.Add(new KeyValuePair<string, string>(key, value.ToString()));
		}

		private static int Comparison(KeyValuePair<string, string> k1, KeyValuePair<string, string> k2)
		{
			return string.Compare(k1.Key, k2.Key, StringComparison.Ordinal);
		}

		public JSONClass ToJSONClass()
		{
			JSONClass jSONClass = new JSONClass();
			for (int i = 0; i < _data.Count; i++)
			{
				jSONClass[_data[i].Key] = _data[i].Value;
			}
			return jSONClass;
		}

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			for (int i = 0; i < _data.Count; i++)
			{
				stringBuilder.Append(string.Format("{0}={1}\n", _data[i].Key, _data[i].Value));
			}
			return stringBuilder.ToString();
		}

		public static implicit operator WWWForm(Form form)
		{
			form._data.Sort(Comparison);
			WWWForm wWWForm = new WWWForm();
			StringBuilder stringBuilder = new StringBuilder();
			for (int i = 0; i < form._data.Count; i++)
			{
				stringBuilder.Append(string.Format("{0}={1}", form._data[i].Key, form._data[i].Value));
				wWWForm.AddField(form._data[i].Key, form._data[i].Value);
			}
			stringBuilder.Append(_key);
			MD5 mD = MD5.Create();
			byte[] array = mD.ComputeHash(Encoding.UTF8.GetBytes(stringBuilder.ToString()));
			wWWForm.AddField("sig", BitConverter.ToString(array).Replace("-", string.Empty).ToLower());
			return wWWForm;
		}
	}

	protected class ServerResponce
	{
		public string response;

		public string error;

		public static ServerResponce Get(string json)
		{
			return JsonConvert.DeserializeObject<ServerResponce>(json);
		}
	}

	private static GameObject _nestedObject;

	private readonly List<IEnumerator> _holdRoutine = new List<IEnumerator>();

	protected static GameObject NestedObject
	{
		get
		{
			return _nestedObject;
		}
	}

	protected abstract string GetServer();

	protected void OnDestroy()
	{
		_nestedObject = null;
	}

	internal void Update()
	{
		if (_holdRoutine.Count > 0)
		{
			for (int i = 0; i < _holdRoutine.Count; i++)
			{
				StartCoroutine(_holdRoutine[i]);
			}
			_holdRoutine.Clear();
		}
	}

	protected static T Init<T>() where T : ServerProviderBase
	{
		_nestedObject = GameObject.Find("_server");
		if (!_nestedObject)
		{
			_nestedObject = new GameObject("_server");
			UnityEngine.Object.DontDestroyOnLoad(_nestedObject);
		}
		T val = _nestedObject.GetComponent<T>();
		if (!val)
		{
			val = _nestedObject.AddComponent<T>();
		}
		return val;
	}

	protected abstract void Init();

	protected bool Check()
	{
		Init();
		if (!_nestedObject)
		{
			AdvLog.LogWarning("ServerCall terminated. you should call Init<T>() from your Init() method! (where T is your inherited class)");
			return false;
		}
		return true;
	}

	public virtual void Join(Action<bool> onDone, Action<string> onError)
	{
		if (Check())
		{
			IEnumerator enumerator = JoinRoutine(onDone, onError);
			if (!Social.CurrentUser)
			{
				AdvLog.LogWarning("ServerProvider.Join(..) hold while SocialWrapper.CurrentUser is null");
				_holdRoutine.Add(enumerator);
			}
			else
			{
				StartCoroutine(enumerator);
			}
		}
	}

	protected virtual IEnumerator JoinRoutine(Action<bool> onDone, Action<string> onError)
	{
		Form form = new Form();
		form.Add("uid", Social.CurrentUser.UserID);
		form.Add("photo", Social.CurrentUser.PhotoURL);
		form.Add("fname", Social.CurrentUser.FirstName);
		form.Add("lname", Social.CurrentUser.LastName);
		WWW www = new WWW(string.Format("{0}/join.php", GetServer()), form);
		yield return www;
		if (!string.IsNullOrEmpty(www.error))
		{
			onError(www.error);
			yield break;
		}
		try
		{
			ServerResponce responce = ServerResponce.Get(www.text);
			if (string.IsNullOrEmpty(responce.error))
			{
				onDone(responce.response.Equals("new"));
			}
			else
			{
				onError(responce.error ?? string.Empty);
			}
		}
		catch (Exception ex)
		{
			onError(ex.Message);
		}
	}

	public virtual void SaveData(string alias, string data, Action onDone, Action<string> onError)
	{
		if (Check())
		{
			IEnumerator enumerator = SaveDataRoutine(alias, data, onDone, onError);
			if (!Social.CurrentUser)
			{
				AdvLog.LogWarning("ServerProvider.SaveData(..) hold while SocialWrapper.CurrentUser is null");
				_holdRoutine.Add(enumerator);
			}
			else
			{
				StartCoroutine(enumerator);
			}
		}
	}

	protected virtual IEnumerator SaveDataRoutine(string alias, string data, Action onDone, Action<string> onError)
	{
		Form form = new Form();
		form.Add("uid", SocialWrapper.CurrentUser.UserID);
		form.Add("alias", alias);
		form.Add("data", data);
		WWW www = new WWW(string.Format("{0}/put.php", GetServer()), form);
		yield return www;
		if (!string.IsNullOrEmpty(www.error))
		{
			onError(www.error);
			yield break;
		}
		try
		{
			ServerResponce responce = ServerResponce.Get(www.text);
			if (string.IsNullOrEmpty(responce.error))
			{
				onDone();
			}
			else
			{
				onError(responce.error ?? string.Empty);
			}
		}
		catch (Exception ex)
		{
			onError(ex.Message);
		}
	}

	public virtual void LoadData(string alias, Action<string> onDone, Action<string> onError)
	{
		if (Check())
		{
			IEnumerator enumerator = LoadDataRoutine(alias, onDone, onError);
			if (!Social.CurrentUser)
			{
				AdvLog.LogWarning("ServerProvider.LoadData(..) holden while SocialWrapper.CurrentUser is null");
				_holdRoutine.Add(enumerator);
			}
			else
			{
				StartCoroutine(enumerator);
			}
		}
	}

	protected virtual IEnumerator LoadDataRoutine(string alias, Action<string> onDone, Action<string> onError)
	{
		Form form = new Form();
		form.Add("uid", Social.CurrentUser.UserID);
		form.Add("alias", alias);
		WWW www = new WWW(string.Format("{0}/get.php", GetServer()), form);
		yield return www;
		if (!string.IsNullOrEmpty(www.error))
		{
			onError(www.error);
			yield break;
		}
		try
		{
			ServerResponce responce = ServerResponce.Get(www.text);
			if (string.IsNullOrEmpty(responce.error))
			{
				onDone(Unescape(responce.response));
			}
			else
			{
				onError(responce.error ?? string.Empty);
			}
		}
		catch (Exception ex)
		{
			onError(ex.Message);
		}
	}

	protected string Unescape(string result)
	{
		if (string.IsNullOrEmpty(result))
		{
			return string.Empty;
		}
		return result.Replace("\\/", "/").Replace("\\\"", "\"").Replace("\\r\\n", "\n");
	}

	public virtual void TimeSync(Action<long> onDone, Action<string> onError)
	{
		if (Check())
		{
			IEnumerator routine = TimeSyncRoutine(onDone, onError);
			StartCoroutine(routine);
		}
	}

	protected virtual IEnumerator TimeSyncRoutine(Action<long> onDone, Action<string> onError)
	{
		Form form = new Form();
		WWW www = new WWW(string.Format("{0}/time.php", GetServer()), form);
		yield return www;
		if (!string.IsNullOrEmpty(www.error))
		{
			onError(www.error);
			yield break;
		}
		try
		{
			ServerResponce responce = ServerResponce.Get(www.text);
			if (string.IsNullOrEmpty(responce.error))
			{
				onDone(long.Parse(responce.response.Trim()));
			}
			else
			{
				onError(responce.error ?? string.Empty);
			}
		}
		catch (Exception ex)
		{
			onError(ex.Message);
		}
	}

	public virtual void WipeUser(Action onDone, Action<string> onError)
	{
		if (Check())
		{
			IEnumerator enumerator = WipeUserRoutine(onDone, onError);
			if (!Social.CurrentUser)
			{
				AdvLog.LogWarning("ServerProvider.WipeUser(..) hold while SocialWrapper.CurrentUser is null");
				_holdRoutine.Add(enumerator);
			}
			else
			{
				StartCoroutine(enumerator);
			}
		}
	}

	protected virtual IEnumerator WipeUserRoutine(Action onDone, Action<string> onError)
	{
		Form form = new Form();
		form.Add("uid", Social.CurrentUser.UserID);
		WWW www = new WWW(string.Format("{0}/wipe.php", GetServer()), form);
		yield return www;
		if (!string.IsNullOrEmpty(www.error))
		{
			onError(www.error);
			yield break;
		}
		try
		{
			ServerResponce responce = ServerResponce.Get(www.text);
			if (string.IsNullOrEmpty(responce.error))
			{
				onDone();
			}
			else
			{
				onError(responce.error ?? string.Empty);
			}
		}
		catch (Exception ex)
		{
			onError(ex.Message);
		}
	}

	public virtual QResponse Query(Select select)
	{
		WWW wWW = new WWW(string.Format("{0}/query.php", GetServer()), select);
		while (!wWW.isDone && string.IsNullOrEmpty(wWW.error))
		{
		}
		if (!string.IsNullOrEmpty(wWW.error))
		{
			throw new Exception(wWW.error);
		}
		string aJSON = wWW.text.Replace("\\\"", "\"");
		JSONNode jSONNode = JSON.Parse(aJSON);
		if (string.IsNullOrEmpty(jSONNode["error"]))
		{
			return new QResponse(jSONNode["response"].AsArray, select);
		}
		throw new Exception(jSONNode["error"]);
	}

	public virtual void Query(Select select, Action<QResponse> onDone, Action<string> onError)
	{
		StartCoroutine(QueryRoutine(select, onDone, onError));
	}

	protected virtual IEnumerator QueryRoutine(Select select, Action<QResponse> onDone, Action<string> onError)
	{
		WWW www = new WWW(string.Format("{0}/query.php", GetServer()), select);
		yield return www;
		if (!string.IsNullOrEmpty(www.error))
		{
			onError(www.error);
			yield break;
		}
		try
		{
			string rText = www.text.Replace("\\\"", "\"");
			JSONNode responce = JSON.Parse(rText);
			if (string.IsNullOrEmpty(responce["error"]))
			{
				onDone(new QResponse(responce["response"].AsArray, select));
			}
			else
			{
				onError(responce["error"]);
			}
		}
		catch (Exception ex)
		{
			onError(ex.Message);
		}
	}
}
