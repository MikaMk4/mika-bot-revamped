using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MikaBotRevamped
{
    public enum WaifuQuality
    {
        [EnumProbability(6089)]
        [EnumColor("#6b6a68")]
        Common,
        [EnumProbability(2500)]
        [EnumColor("#1a822d")]
        Uncommon,
        [EnumProbability(1000)]
        [EnumColor("#0b288f")]
        Rare,
        [EnumProbability(300)]
        [EnumColor("#6f0580")]
        Epic,
        [EnumProbability(100)]
        [EnumColor("#b5780e")]
        Legendary,
        [EnumProbability(10)]
        [EnumColor("#a61111")]
        Mythic,
        [EnumProbability(1)]
        [EnumColor("#2febd5")]
        Godly
    }

    [AttributeUsage(AttributeTargets.Field)]
    public class EnumColorAttribute : Attribute
    {
        public Color Color { get; }

        public EnumColorAttribute(string hex)
        {
            Color = ColorTranslator.FromHtml(hex);
        }
    }

    [AttributeUsage(AttributeTargets.Field)]
    public class EnumProbabilityAttribute : Attribute
    {
        public int Probability { get; }

        public EnumProbabilityAttribute(int probability)
        {
            Probability = probability;
        }
    }

    public class WaifuQualityMethods
    {
        public static IEnumerable<(WaifuQuality Value, double Probability)> GetEnumValueProbabilities()
        {
            Array values = Enum.GetValues(typeof(WaifuQuality));

            foreach (var value in values)
            {
                var fieldInfo = typeof(WaifuQuality).GetField(value.ToString());
                var probabilityAttribute = fieldInfo.GetCustomAttributes(typeof(EnumProbabilityAttribute), false).FirstOrDefault() as EnumProbabilityAttribute;
                int probability = probabilityAttribute?.Probability ?? 1;

                yield return ((WaifuQuality)value, probability);
            }
        }

        public static Color GetColor(WaifuQuality value)
        {
            var fieldInfo = typeof(WaifuQuality).GetField(value.ToString());
            var colorAttribute = fieldInfo.GetCustomAttributes(typeof(EnumColorAttribute), false).FirstOrDefault() as EnumColorAttribute;

            return colorAttribute?.Color ?? Color.White;
        }
    }
}
