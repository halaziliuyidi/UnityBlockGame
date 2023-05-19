using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AnnulusGames.LucidTools.RandomKit
{
    public static class RandomExtensions
    {
        public static T RandomElement<T>(this IEnumerable<T> self)
        {
            return LucidRandom.RandomElement(self);
        }

        public static List<T> RandomDisticncElements<T>(this IEnumerable<T> self, int count)
        {
            return LucidRandom.RandomDisticncElements(self,count);
        }

        public static IEnumerable<T> Shuffle<T>(this IEnumerable<T> self)
        {
            return LucidRandom.Shuffle(self);
        }

        public static Color RandomColor(this Gradient gradient)
        {
            return LucidRandom.RandomColor(gradient);
        }
    }

}