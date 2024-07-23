﻿
using System;
using System.Collections.Generic;
using AetherSenseReduxToo.Toy.Pattern;

namespace AetherSenseReduxToo.Toy.Pattern
{
    internal class ConstantPattern : IPattern
    {
        public DateTime Expires { get; set; }
        private readonly double level;

        public ConstantPattern(ConstantPatternConfig config)
        {
            level = config.Level;
            Expires = DateTime.UtcNow + TimeSpan.FromMilliseconds(config.Duration);
        }

        public double GetIntensityAtTime(DateTime time)
        {
            if (Expires < time)
            {
                throw new PatternExpiredException();
            }
            return level;
        }
        public static PatternConfig GetDefaultConfiguration()
        {
            return new ConstantPatternConfig();
        }
    }
    [Serializable]
    public class ConstantPatternConfig : PatternConfig
    {
        public override string Type { get; } = "Constant";
        public double Level { get; set; } = 1;
    }
}
