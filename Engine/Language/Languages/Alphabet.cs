﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Engine.Language.Data;

namespace Engine.Language.Languages
{
    public record class Alphabet
    {
        public Dictionary<char, Consonant> Consonants { get; private set; } = new Dictionary<char, Consonant>();
        public Dictionary<char, Vowel> Vowels { get; private set; } = new Dictionary<char, Vowel>();

        public Consonant AddConsonant(char key, (char lower, char upper) cases)
        {
            return AddConsonant(string.Empty, key, cases);
        }
        public Vowel AddVowel(char key, (char lower, char upper) cases)
        {
            return AddVowel(string.Empty, key, cases);
        }

        public Consonant AddConsonant(string name, char key, (char lower, char upper) cases, string pronunciation = "")
        {
            if (Consonants.ContainsKey(key) == false)
                Consonants.Add(key, new Consonant(name, key, cases, pronunciation));
            return Consonants[key];
        }
        public Vowel AddVowel(string name, char key, (char lower, char upper) cases, string pronunciation = "")
        {
            if (Vowels.ContainsKey(key) == false)
                Vowels.Add(key, new Vowel(name, key, cases, pronunciation));
            return Vowels[key];
        }

        public Letter? Find(char key)
        {
            if (Consonants.ContainsKey(key))
                return Consonants[key];
            else if (Vowels.ContainsKey(key))
                return Vowels[key];
            return null;
        }
        /// <summary>
        /// Returns the custom upper case of the letter key. If none are found, it returns char.ToUpper.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public char Upper(char key)
        {
            Letter l = Find(key);
            if (l != null)
                return l.Case.upper;
            return char.ToUpper(key);
        }
        public List<Letter> Letters()
        {
            List<Letter> result = new List<Letter>();
            result.AddRange(Consonants.Values);
            result.AddRange(Vowels.Values);

            return result.OrderBy(l => l.Key).ToList();
        }

        internal void ResetWeights()
        {
            foreach (Consonant c in Consonants.Values) c.WeightMultiplier = 1.0;
            foreach (Vowel v in Vowels.Values) v.WeightMultiplier = 1.0;
        }
    }
}
