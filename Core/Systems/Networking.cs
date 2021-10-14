using Microsoft.Xna.Framework;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AARPG.Core.Systems{
	public static class Networking{
		public enum Message{
			SpawnExperienceOrb
		}

		public static void HandlePacket(BinaryReader reader, int sender){
			Message message = (Message)reader.ReadByte();

			switch(message){
				case Message.SpawnExperienceOrb:
					ReceiveSpawnExperienceOrb(reader, sender);
					break;
			}
		}

		public static void SendSpawnExperienceOrbs(int sender, int target, int xp, Vector2 spawn, float velocityLength){
			if(xp <= 0)
				return;

			ModPacket packet = GetPacket(Message.SpawnExperienceOrb);

			packet.Write((byte)target);
			packet.Write(xp);
			packet.WriteVector2(spawn);
			packet.Write(velocityLength);

			packet.Send(ignoreClient: sender);
		}

		private static void ReceiveSpawnExperienceOrb(BinaryReader reader, int sender){
			byte target = reader.ReadByte();
			int xp = reader.ReadInt32();
			Vector2 spawn = reader.ReadVector2();
			float velocityLength = reader.ReadSingle();

			if(Main.netMode == NetmodeID.Server)
				SendSpawnExperienceOrbs(sender, target, xp, spawn, velocityLength);
			else
				ExperienceTracker.SpawnExperience(xp, spawn, velocityLength, target);
		}

		private static ModPacket GetPacket(Message type){
			var packet = CoreMod.Instance.GetPacket();
			packet.Write((byte)type);
			return packet;
		}
	}
}
