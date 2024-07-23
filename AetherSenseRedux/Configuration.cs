using AetherSenseReduxToo.Toy.Pattern;
using Dalamud.Configuration;
using Dalamud.Plugin.Services;
using Dalamud.Plugin;
using System;
using System.Collections.Generic;
using AetherSenseReduxToo.Game.Trigger;

namespace AetherSenseReduxToo
{
    [Serializable]
    public class Configuration : IPluginConfiguration
    {
        public int Version { get; set; } = 2;
        public bool FirstRun = true;
        public bool ShowChatLogs = false;
        public bool LogChat { get; set; } = false;
        public int LogLevel { get; set; } = 1;
        public bool Reconnect = false;
        public string llName(int o)
        {
            switch (o)
            {
                case 0: return "No Logs";
                case 1: return "Errors Only";
                case 2: return "Errors and Info";
                case 3: return "Full";
            }
            return "None";
        }
        public string Address { get; set; } = "ws://127.0.0.1:12345";
        public List<string> SeenDevices { get; set; } = new();
        public List<dynamic> Triggers { get; set; } = new List<dynamic>();

        [NonSerialized]
        private IDalamudPluginInterface? pluginInterface;

        /// <summary>
        /// Stores a reference to the _plugin interface to allow us to save this configuration and reload it from disk.
        /// </summary>
        /// <param name="pluginInterface">The DalamudPluginInterface instance in this _plugin</param>
        public void Initialize(IDalamudPluginInterface pluginInterface)
        {
            this.pluginInterface = pluginInterface;
        }

        /// <summary>
        /// Deep copies the trigger list while ensuring that everything has the correct type.
        /// </summary>
        public void FixDeserialization()
        {
            List<TriggerConfig> triggers = new();
            foreach (dynamic t in Triggers)
            {
                triggers.Add(TriggerFactory.GetTriggerConfigFromObject(t));
            }
            Triggers = new List<dynamic>();

            foreach (TriggerConfig t in triggers)
            {
                Triggers.Add(t);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public void LoadDefaults()
        {
            Version = 2;
            LogLevel = 1;
            FirstRun = false;
            Triggers = new List<dynamic>() {
                new ChatTriggerConfig()
                {
                    Name = "Casted",
                    EnabledDevices = new List<string>(),
                    PatternSettings = new ConstantPatternConfig()
                    {
                        Level = 1,
                        Duration = 200
                    },
                    Regex = "You cast",
                    RetriggerDelay = 0
                },
                new ChatTriggerConfig()
                {

                    Name = "Casting",
                    EnabledDevices = new List<string>(),
                    PatternSettings = new RampPatternConfig()
                    {
                        Start = 0,
                        End = 0.75,
                        Duration = 2500
                    },
                    Regex = "You begin casting",
                    RetriggerDelay = 250
                }
            };
        }

        /// <summary>
        /// 
        /// </summary>
        public void Save()
        {
            pluginInterface!.SavePluginConfig(this);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="o"></param>
        public void Import(dynamic o)
        {
            try
            {
                if (o.Version != 2)
                {
                    return;
                }
                FirstRun = o.FirstRun;
                LogChat = o.LogChat;
                LogLevel = o.LogLevel;
                Reconnect = o.Reconnect;
                Address = o.Address;
                SeenDevices = new List<string>(o.SeenDevices);
                Triggers = o.Triggers;
                FixDeserialization();
            }
            catch (Exception ex)
            {
                Service.LogError(ex, "Attempted to import a bad configuration.");
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        /// <exception cref="NullReferenceException"></exception>
        public Configuration CloneConfigurationFromDisk()
        {
            if (pluginInterface == null)
            {
                throw new NullReferenceException("Attempted to load the _plugin configuration from a clone.");
            }
            var config = pluginInterface!.GetPluginConfig() as Configuration ?? throw new NullReferenceException("No configuration exists on disk.");
            config.FixDeserialization();
            return config;
        }
    }
}
