using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AetherSenseReduxToo.Toy.Pattern;

namespace AetherSenseReduxToo.Game.Trigger
{
    internal interface ITrigger
    {
        bool Enabled { get; set; }
        string Name { get; init; }
        string Type { get; }
        public bool UseRandom { get; set; }
        public bool UseSkip { get; set; }
        public bool UseAll { get; set; }
        public int SkipChance { get; set; }

        Task MainLoop();

    }
    [Serializable]
    public abstract class TriggerConfig
    {
        public bool UseRandom { get; set; }
        public bool UseSkip { get; set; }
        public bool UseAll { get; set; }
        public int SkipChance { get; set; }
        public abstract string Type { get; }
        public abstract string Name { get; set; }
        public List<string> EnabledDevices { get; set; } = new List<string>();
        public dynamic? PatternSettings { get; set; }
    }
}
