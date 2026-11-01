using System;
using System.Collections.Generic;
using System.Linq;
using Template.Scripts.DataHolders;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Template.Scripts.Utils
{
    public static class RandomUtils
    {
        public static T MakeChoiceWithChances<T>(RandomChoiceObject<T>[] values, T defaultValue = default)
        {
            var randomValue = Random.Range(0f, 1f);

            double chanceSum = 0;

            foreach (var value in values)
            {
                chanceSum += value.Chances;

                if (randomValue <= chanceSum)
                {
                    return value.Value;
                }
            }

            return defaultValue;
        }

        public static T MakeChoiceWithChances<T>(List<RandomChoiceObject<T>> values, T defaultValue = default)
        {
            var randomValue = Random.Range(0f, 1f);

            double chanceSum = 0;

            foreach (var value in values)
            {
                chanceSum += value.Chances;

                if (randomValue <= chanceSum)
                {
                    return value.Value;
                }
            }

            return defaultValue;
        }
        
        public static T MakeChoiceWithChances<T>(IEnumerable<RandomChoiceObject<T>> values, T defaultValue = default)
        {
            var randomValue = Random.Range(0f, 1f);

            double chanceSum = 0;

            foreach (var value in values)
            {
                if (value.Chances == 0)
                {
                    continue;
                }
                
                chanceSum += value.Chances;

                if (randomValue <= chanceSum)
                {
                    return value.Value;
                }
            }

            return defaultValue;
        }
        
        public static T Pick<T>(this T[] values, System.Random seed = null)
        {
            seed ??= new System.Random();
            int randomIndex = seed.Next(0, values.Length);

            return values[randomIndex];
        }
        
        public static T Pick<T>(this List<T> values, System.Random seed = null)
        {
            seed ??= new System.Random();
            int randomIndex = seed.Next(0, values.Count);

            return values[randomIndex];
        }
        
        public static T Pick<T>(this IReadOnlyList<T> values, System.Random seed = null)
        {
            seed ??= new System.Random();
            int randomIndex = seed.Next(0, values.Count);

            return values[randomIndex];
        }

        public static T PickRandom<T>(this IEnumerable<T> enumerable, System.Random seed = null)
        {
            seed ??= new System.Random();
            int index = seed.Next(0, enumerable.Count());

            return enumerable.ElementAt(index);
        }

        public static List<T> PickDifferentItems<T>(T[] values, int amount)
        {
            if (amount <= 0 || amount > values.Length)
            {
                return default;
            }

            bool[] valueIsTaken = new bool[values.Length];
            List<T> result = new List<T>(amount);

            for (int i = 0; i < amount; i++)
            {
                int randomIndex = Random.Range(0, values.Length);

                while (valueIsTaken[randomIndex])
                {
                    randomIndex = (randomIndex + 1) % values.Length;
                }

                result.Add(values[randomIndex]);
                valueIsTaken[randomIndex] = true;
            }

            return result;
        }

        public static List<T> PickDifferentItems<T>(IReadOnlyList<T> values, int amount, System.Random seed = null)
        {
            if (amount <= 0 || amount > values.Count)
            {
                return default;
            }

            seed ??= new System.Random();
            
            bool[] valueIsTaken = new bool[values.Count];
            List<T> result = new List<T>(amount);

            for (int i = 0; i < amount; i++)
            {
                int randomIndex = seed.Next(0, values.Count);

                while (valueIsTaken[randomIndex])
                {
                    randomIndex = (randomIndex + 1) % values.Count;
                }

                result.Add(values[randomIndex]);
                valueIsTaken[randomIndex] = true;
            }

            return result;
        }

        public static T PickWhere<T>(T[] values, Predicate<T> predicate, bool useRandomInsteadOfDefault = false)
        {
            int randomIndex = Random.Range(0, values.Length);
            int randomDirection = Random.Range(0, 2) * 2 - 1;

            int maxRepetitionsAmount = values.Length;
            while (predicate(values[randomIndex]) == false && maxRepetitionsAmount > 0)
            {
                randomIndex = (int)Mathf.Repeat(randomIndex + randomDirection, values.Length);
                maxRepetitionsAmount--;
            }

            if (maxRepetitionsAmount == 0)
            {
                if (useRandomInsteadOfDefault)
                {
                    return values[randomIndex];
                }

                return default;
            }

            return values[randomIndex];
        }

        public static T PickWhere<T>(List<T> values, Predicate<T> predicate)
        {
            int randomIndex = Random.Range(0, values.Count);
            int randomDirection = Random.Range(0, 2) * 2 - 1;

            int maxRpetitionsAmount = values.Count;
            while (predicate(values[randomIndex]) == false && maxRpetitionsAmount > 0)
            {
                randomIndex = (int)Mathf.Repeat(randomIndex + randomDirection, values.Count);
                maxRpetitionsAmount--;
            }

            if (maxRpetitionsAmount == 0)
            {
                return default;
            }
            else
            {
                return values[randomIndex];
            }
        }

        public static int PickIndexWhere<T>(T[] values, Predicate<T> predicate)
        {
            int randomIndex = Random.Range(0, values.Length);
            int randomDirection = Random.Range(0, 2) * 2 - 1;

            int maxRpetitionsAmount = values.Length;
            while (predicate(values[randomIndex]) == false && maxRpetitionsAmount > 0)
            {
                randomIndex = (int)Mathf.Repeat(randomIndex + randomDirection, values.Length);
                maxRpetitionsAmount--;
            }

            if (maxRpetitionsAmount == 0)
            {
                return -1;
            }
            else
            {
                return randomIndex;
            }
        }

        public static int PickIndexWhere<T>(List<T> values, Predicate<T> predicate)
        {
            int randomIndex = Random.Range(0, values.Count);
            int randomDirection = Random.Range(0, 2) * 2 - 1;

            int maxRpetitionsAmount = values.Count;
            while (predicate(values[randomIndex]) == false && maxRpetitionsAmount > 0)
            {
                randomIndex = (int)Mathf.Repeat(randomIndex + randomDirection, values.Count);
                maxRpetitionsAmount--;
            }

            if (maxRpetitionsAmount == 0)
            {
                return -1;
            }
            else
            {
                return randomIndex;
            }
        }
    }
}