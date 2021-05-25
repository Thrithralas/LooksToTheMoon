using System;
using System.IO;
using Newtonsoft.Json;

namespace LooksToTheMoon {
    public static class AppConfiguration {

        public static string Key;
        public static string UserExperiencePath;
        public static string SchedulingPath;
        public static string UserPreferencePath;
        public static string Prefix;
        public static bool PrefixCaseSensitive;
        public static ulong MonklandRoleID;
        public static ulong JollyCoopRoleID;
        public static ulong EventRoleID;
        
        public static void FromJson(string jsonPath = "appconfig.json") {

            string fileContent = File.ReadAllText(jsonPath);
            dynamic jsonConfig = JsonConvert.DeserializeObject(fileContent);
            if (jsonConfig is null) {
                throw new ArgumentNullException("Failed to load JSON configuration at path " + jsonPath);
            }
            
            Key = (string) jsonConfig.Key;
            UserExperiencePath = (string) jsonConfig.UserExperiencePath;
            SchedulingPath = (string) jsonConfig.SchedulingPath;
            UserPreferencePath = (string) jsonConfig.UserPreferencePath;
            Prefix = (string) jsonConfig.Prefix;
            PrefixCaseSensitive = (bool) jsonConfig.PrefixCaseSensitive;
            MonklandRoleID = (ulong) jsonConfig.Monkland;
            JollyCoopRoleID = (ulong) jsonConfig.JollyCoop;
            EventRoleID = (ulong) jsonConfig.Events;

        }
    }
}