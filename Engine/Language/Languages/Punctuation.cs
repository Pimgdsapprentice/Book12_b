﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Engine.Language.Processing;

namespace Engine.Language.Languages
{
    public class Punctuation
    {
        public SortedDictionary<string, Func<WordInfo, string>> Marks { get; set; }
            = new SortedDictionary<string, Func<WordInfo, string>>(new LengthComparer());

        public void Process(LanguageGenerator lg, WordInfo word, string filterName)
        {
            if (word.Filter.Name.ToUpper() == filterName && word.IsProcessed == false)
            {
                foreach (string s in Marks.Keys)
                {
                    if (word.WordActual.Contains(s))
                    {
                        string result = Marks[s](word);
                        word.WordFinal = word.WordActual.Replace(s, result);

                        if (lg.Diagnostics.IsConstructLog == true && lg.Diagnostics.FilterEventExclusion.Contains(word.Filter.Name) == false)
                        {
                            lg.Diagnostics.LOG_Subheader($"PUNCTUATION: {word.WordActual} -> {word.WordFinal}");
                            lg.Diagnostics.LogBuilder.AppendLine();
                        }
                    }
                }
            }
        }

        public void Add(string key, Func<WordInfo, string> value)
        {
            if (Marks.ContainsKey(key) == false)
            {
                Marks.Add(key, value);
            }
        }
    }

    /// <summary>
    /// Code borrowed from: https://stackoverflow.com/questions/20993877/how-to-sort-a-dictionary-by-key-string-length
    /// Code author: Boluc Papuccuoglu
    /// </summary>
    internal class LengthComparer : IComparer<string>
    {
        public int Compare(string x, string y)
        {
            int lengthComparison = x.Length.CompareTo(y.Length);
            if (lengthComparison == 0) return x.CompareTo(y);
            else return lengthComparison;
        }
    }
}
