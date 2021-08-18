using Microsoft.Xna.Framework;
using Newtonsoft.Json.Linq;
using System;
using Terraria;
using Terraria.Chat;
using Terraria.ID;
using Terraria.Localization;

namespace AARPG.Core.Utility{
	public static class MiscUtils{
		public static void SendMessage(string message, Color? color = null){
			color ??= Color.White;

			if(Main.netMode == NetmodeID.SinglePlayer)
				Main.NewText(message, color);
			else
				ChatHelper.BroadcastChatMessage(NetworkText.FromLiteral(message), color.Value);
		}

		public static T GetObject<T>(this JObject obj, string property)
			=> obj.FindObjectMember(property).ToObject<T>();

		private static JToken FindObjectMember(this JObject obj, string member){
			if(!member.Contains(".")){
				if(obj.TryGetValue(member, out JToken token))
					return token;
			}else{
				//Periods being in the "member" string means that the path to the property is nested
				string[] properties = member.Split(new char[]{ '.' }, StringSplitOptions.RemoveEmptyEntries);

				JToken token = obj;
				foreach(string property in properties){
					JObject iter = token as JObject;
					if(!iter.TryGetValue(property, out token))
						goto error;
				}

				return token;
			}

error:
			throw new ArgumentException($"Could not find JObject member \"{member}\"");
		}
	}
}
