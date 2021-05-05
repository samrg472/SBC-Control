using System;
using System.IO;
using System.Xml.Serialization;

namespace SBC_Control
{
    public class GlobalConfig
    {
        private static GlobalConfig _config;

        public static GlobalConfig Config
        {
            get
            {
                if (_config != null)
                    return _config;

                var configPath = GetConfigPath();
                var serializer = new XmlSerializer(typeof(GlobalConfig));
                using var stream = new FileStream(configPath, FileMode.OpenOrCreate, FileAccess.ReadWrite);

                if (stream.Length == 0)
                {
                    var config = new GlobalConfig();
                    serializer.Serialize(stream, config);
                    ConfigIsNew = true;
                    _config = config;
                    return _config;
                }

                _config = (GlobalConfig) serializer.Deserialize(stream);
                return _config;
            }
        }

        public static bool ConfigIsNew { get; private set; }

        public float HeadphoneMasterVolume;
        public bool HeadphoneMasterMute;

        public float SrsMasterVolume;
        public bool SrsMasterMute;

        public void Save()
        {
            var configPath = GetConfigPath();
            var serializer = new XmlSerializer(typeof(GlobalConfig));
            using var stream = new FileStream(configPath, FileMode.Open, FileAccess.Write);
            stream.SetLength(0);
            serializer.Serialize(stream, this);
        }

        private static string GetConfigPath()
        {
            var appData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            var configDir = Path.Combine(appData, "SBC_Control");
            if (!Directory.Exists(configDir))
            {
                Directory.CreateDirectory(configDir);
            }

            return Path.Combine(configDir, "config.xml");
        }
    }
}
