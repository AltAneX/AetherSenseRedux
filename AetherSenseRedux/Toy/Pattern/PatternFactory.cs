﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AetherSenseReduxToo.Toy.Pattern
{
    internal class PatternFactory
    {
        public static IPattern GetPatternFromObject(PatternConfig settings)
        {
            switch (settings.Type)
            {
                case "Constant":
                    return new ConstantPattern((ConstantPatternConfig)settings);
                case "Ramp":
                    return new RampPattern((RampPatternConfig)settings);
                case "Saw":
                    return new SawPattern((SawPatternConfig)settings);
                case "Random":
                    return new RandomPattern((RandomPatternConfig)settings);
                case "Square":
                    return new SquarePattern((SquarePatternConfig)settings);
                default:
                    throw new ArgumentException(string.Format("Invalid pattern {0} specified", settings.Type));
            }
        }

        public static PatternConfig GetDefaultsFromString(string name)
        {
            switch (name)
            {
                case "Constant":
                    return ConstantPattern.GetDefaultConfiguration();
                case "Ramp":
                    return RampPattern.GetDefaultConfiguration();
                case "Saw":
                    return SawPattern.GetDefaultConfiguration();
                case "Random":
                    return RandomPattern.GetDefaultConfiguration();
                case "Square":
                    return SquarePattern.GetDefaultConfiguration();
                default:
                    throw new ArgumentException(string.Format("Invalid pattern {0} specified", name));
            }
        }

        public static PatternConfig GetPatternConfigFromObject(dynamic o)
        {
            switch ((string)o.Type)
            {
                case "Constant":
                    return new ConstantPatternConfig()
                    {
                        Duration = (long)o.Duration,
                        Level = (double)o.Level
                    };
                case "Ramp":
                    return new RampPatternConfig()
                    {
                        Duration = (long)o.Duration,
                        Start = (double)o.Start,
                        End = (double)o.End
                    };
                case "Saw":
                    return new SawPatternConfig()
                    {
                        Duration = (long)o.Duration,
                        Start = (double)o.Start,
                        End = (double)o.End,
                        Duration1 = (long)o.Duration1
                    };
                case "Random":
                    return new RandomPatternConfig()
                    {
                        Duration = (long)o.Duration,
                        Minimum = (double)o.Minimum,
                        Maximum = (double)o.Maximum
                    };
                case "Square":
                    return new SquarePatternConfig()
                    {
                        Duration = (long)o.Duration,
                        Duration1 = (long)o.Duration1,
                        Duration2 = (long)o.Duration2,
                        Level1 = (double)o.Level1,
                        Level2 = (double)o.Level2,
                        Offset = (long)o.Offset
                    };
                default:
                    throw new ArgumentException(string.Format("Invalid pattern {0} specified", o.Type));
            }
        }
    }
}
