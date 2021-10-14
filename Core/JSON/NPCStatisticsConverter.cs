using AARPG.Core.Mechanics;
using AARPG.Core.Utility;
using AARPG.Core.Utility.Extensions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;

namespace AARPG.Core.JSON{
	public class NPCStatisticsConverter : JsonConverter{
		//Helpful links:
		// https://stackoverflow.com/a/55370666/8420233
		// https://csharp.hotexamples.com/examples/Newtonsoft.Json/JsonReader/-/php-jsonreader-class-examples.html

		public override bool CanConvert(Type objectType)
			=> objectType == typeof(NPCStatistics);

		public override bool CanRead => true;

		public override bool CanWrite => true;

		public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer){
			if(value is not NPCStatistics stat)
				return;

			writer.WriteStartObject();

			writer.WritePropertyName("level");
			writer.WriteValue(stat.level);
			writer.WritePropertyName("xp");
			writer.WriteValue(stat.xp);
			writer.WritePropertyName("mod");

			writer.WriteStartObject();
			writer.WritePropertyName("hp");
			serializer.Serialize(writer, stat.healthModifier);
			writer.WritePropertyName("defense");
			serializer.Serialize(writer, stat.defenseModifier);
			writer.WritePropertyName("endure");
			serializer.Serialize(writer, stat.enduranceModifier);
			writer.WritePropertyName("scale");
			serializer.Serialize(writer, stat.scaleModifier);
			writer.WritePropertyName("value");
			serializer.Serialize(writer, stat.valueModifier);
			writer.WriteEndObject();

			writer.WriteEndObject();
		}

		public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer){
			if(reader.TokenType == JsonToken.Null)
				return null;

			NPCStatistics obj = new();
			JObject o = JObject.ReadFrom(reader) as JObject;
			
			obj.level = o.GetObject<int>("level");
			obj.xp = o.GetObject<int>("xp");
			obj.healthModifier = o.GetObject<Modifier>("mod.hp");
			obj.defenseModifier = o.GetObject<Modifier>("mod.defense");
			obj.enduranceModifier = o.GetObject<Modifier>("mod.endure");
			obj.scaleModifier = o.GetObject<Modifier>("mod.scale");
			obj.valueModifier = o.GetObject<Modifier>("mod.value");

			return obj;
		}
	}
}
