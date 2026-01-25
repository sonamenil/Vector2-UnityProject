using System.Collections.Generic;
using LitJson;

namespace BestHTTP.SignalR.JsonEncoders
{
	public sealed class LitJsonEncoder : IJsonEncoder
	{
		public string Encode(object obj)
		{
			JsonWriter jsonWriter = new JsonWriter();
			JsonMapper.ToJson(obj, jsonWriter);
			return jsonWriter.ToString();
		}

		public IDictionary<string, object> DecodeMessage(string json)
		{
			JsonReader reader = new JsonReader(json);
			return JsonMapper.ToObject<Dictionary<string, object>>(reader);
		}
	}
}
