using AARPG.Core.Mechanics;
using AARPG.Core.Utility.Extensions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;

namespace AARPG.Core.JSON{
	public class ModifierConverter : JsonConverter{
		//Helpful links:
		// https://stackoverflow.com/a/55370666/8420233
		// https://csharp.hotexamples.com/examples/Newtonsoft.Json/JsonReader/-/php-jsonreader-class-examples.html

		public override bool CanConvert(Type objectType)
			=> objectType == typeof(Modifier);

		public override bool CanRead => true;

		public override bool CanWrite => true;

		public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer){
			if(value is not Modifier obj)
				return;

			writer.WriteStartObject();
			writer.WritePropertyName("add");
			writer.WriteValue(obj.add);
			writer.WritePropertyName("mult");
			writer.WriteValue(obj.mult);
			writer.WritePropertyName("flat");
			writer.WriteEndObject();
		}

		public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer){
			if(reader.TokenType == JsonToken.Null)
				return null;

			Modifier obj = new();
			ExtensionMethods.InitializeWithDefaultValueAttributes(ref obj);
			JObject o = JObject.ReadFrom(reader) as JObject;

			obj.add = o.Value<float>("add");
			obj.mult = o.Value<float>("mult");
			obj.flat = o.Value<float>("flat");

			return obj;
		}
	}
}
