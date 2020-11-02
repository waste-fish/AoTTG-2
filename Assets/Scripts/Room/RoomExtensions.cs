﻿using Assets.Scripts.Gamemode.Settings;
using System.Linq;

namespace Assets.Scripts.Room
{
    public static class RoomExtensions
    {
        public static string GetName(this RoomInfo room)
        {
            return room.CustomProperties["name"].ToString();
        }

        public static string GetLevel(this RoomInfo room)
        {
            return room.CustomProperties["level"].ToString();
        }

        public static string GetGamemode(this RoomInfo room)
        {
            return room.CustomProperties["gamemode"].ToString();
        }

        public static Level GetLevel(this global::Room room)
        {
            var level = room.CustomProperties["level"].ToString();
            return LevelBuilder.GetAllLevels().Single(x => x.Name == level);
        }

        public static bool GetSecure(this RoomInfo room)
        {
            if (!room.CustomProperties.ContainsKey("secure")) return false;
            return (bool)room.CustomProperties["secure"];
        }


        public static GamemodeSettings GetGamemodeSetting(this global::Room room, Level level)
        {
            var gamemode = room.CustomProperties["gamemode"].ToString();
            return level.Gamemodes.Single(x => x.Name == gamemode);
        }
    }
}
