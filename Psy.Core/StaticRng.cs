using System;
using System.Collections.Generic;

namespace Psy.Core
{
    public static class StaticRng
    {
        private static Random _random;

        public static Random Random
        {
            get { return _random ?? (_random = new Random()); }
        }
    }

    public static class RandomCollectionExtensionMethods
    {
        public static T RandomItem<T>(this T[] items)
        {
            var itemIndex = StaticRng.Random.Next(items.Length);
            return items[itemIndex];
        }

        public static T RandomItem<T>(this List<T> items)
        {
            var itemIndex = StaticRng.Random.Next(items.Count);
            return items[itemIndex];
        }
    }

    public static class RandomExtensionMethods
    {

        public static T Choice<T>(this Random random, T[] items)
        {
            var itemIndex = random.Next(items.Length);
            return items[itemIndex];
        }

        public static bool NextBool(this Random random)
        {
            return random.NextDouble() > 0.5f;
        }

        public static float NextFloat(this Random random, float min, float max)
        {
            var range = max - min;

            return (float)(random.NextDouble()*range) + min;
        }

        public static double NextDouble(this Random random, double min, double max)
        {
            var range = max - min;

            return random.NextDouble() * range + min;
        }

        public static double RandomBell(this Random random, float min, float middle, float max)
        {
            var increase = random.NextBool();
            var sigma = (increase ? (max - middle) : (middle - min))*5;

            var value = random.NextDouble() * 100;
            if (!increase)
                value = -value;

            var ret = 1 / (sigma * Math.Sqrt(2 * Math.PI));
            var power = (value - middle)/sigma;
            power = power*power;

            ret *= Math.Exp(-0.5*power);

            if (ret > max)
                ret = max;
            else if (ret < min)
                ret = min;
            else if (double.IsNaN(ret))
                ret = middle;

            return ret;
        }
    }
}
