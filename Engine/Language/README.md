![header_b](https://user-images.githubusercontent.com/119130949/226001193-387086ae-734a-4c01-b01a-2098ff1c4900.png)

# Procedural Language Generation Library

> No language is justly studied merely as an aid to other purposes. It will in fact better serve other purposes, philological or historical, when it is studied for love, for itself. — J. R. R. Tolkien


[NuGet page](https://www.nuget.org/packages/PLGL/)

## 1 — Introduction

Procedural Language Generation Library (PLGL) is a code library designed for game developers who want consistent, fictional languages for their game's cultures and peoples, *without the time needed to create one*. The language author constructs the alphabet, letter groups, syllable structures, affixes, character filtering for sentence deconstruction, and other constraints; then, the generator processes a regular sentence, and returns a new, stylized sentence from your fictional language.


## 2 — Contents

1. **Introduction**
2. **Contents**
3. [Examples](https://github.com/Highverve/PLGL#3--examples)
4. [Code Layout](https://github.com/Highverve/PLGL#4--code-layout)
5. [Theory & Process](https://github.com/Highverve/PLGL#5--theory--process)
    - 5.1 [Deconstruction](https://github.com/Highverve/PLGL#51--deconstruction)
    - 5.2 [Construction](https://github.com/Highverve/PLGL#52--construction)
    - 5.3 [Generating Sentences](https://github.com/Highverve/PLGL#53--generating-sentences)
6. [Setting Up](https://github.com/Highverve/PLGL#6--setting-up)
7. [Future Updates](https://github.com/Highverve/PLGL#7--future-updates)
8. [Useful Resources](https://github.com/Highverve/PLGL#8--useful-resources)


## 3 — Examples

*Qen* is more of a scandinavian/english language, whereas *Jabanese* attempts to mimic Japanese. Qen has complex syllables, and a moderate amount of exclusion rules. Jabanese has simpler syllable structures, yet relies on letter exclusion a bit more. You can convert the generated sentence into hiragana with the built-in `ToHiragana()`; otherwise, I recommend using `ToRomaji()`.

In English:
```
So in this pleasant vale we stand again,
The field of Enna, now once more ablaze
With flowers that brighten as thy footstep falls
```

In the fictional language called *Qen* (with seed offset at 2):
```
Fras sil iren yadïnem nigüŋ mel spil nälis,
Lo flapäs ha Enna, om kelbu nölën numëmo
Nob sallesen on sölez egel hän borsüspöŋ ŝlonen
```

Qen, this time with the seed offset at 15:
```
Ik öl fonen mëöbür femöm füp be fume,
Lo manäs ha Enna, saŋ nebö fökum belidëf
Fe skenälen sof ürur egel ü mërrublan ëlen
```

Now in a fictional Japanese language (seed offset at 6 — Romaji and Hiragana):
```
pa mene ta wadagomo sudere ba ki musaha、
ぱ めね た わだごも すでれ ば き むさは、

chusa kugucho giyu gita、 shoku kazubi dayuke kozopeyo
ちゅさ くぐちょ ぎゆ ぎた、 しょく かづび だゆけ こぞぺよ

sote pumaya shu sushorya chi muki pobihe na。
そて ぷまや しゅ すしょりゃ ち むき ぽびへ な。
```


## 4 — Code Layout

It's really simple to quickly set up a testing environment for languages.

```c#
LanguageGenerator lg = new LanguageGenerator();

//Found in PLGL.Examples. Set this to your own language (derived from Language).
Qen qen = new Qen();
lg.Language = qen;

Console.WriteLine(lg.GenerateClean("Your sentence goes here!"));
```

`GenerateClean` only returns a string, whereas `GenerateDebug` returns the string and a `List<WordInfo>` for debugging. `GenerateRaw` only returns the list.

- **LanguageGenerator.cs**. Processes the sentence according to the constraints found in the referenced Language.
- **Deconstructor.cs**. Deconstructs the input sentence according to the specified Language's filters.
- **Language.cs**. Contains all classes found in the Languages folder and other settings. It's passed to and used by LanguageGenerator.
- **LanguageManager.cs**. Intended for use by anyone who needs to implement this code into their project. Not utilized by any PLGL classes.
- **Processing**. All the classes in this folder are utilized by LanguageGenerator. They're left public in case a language author needs to debug their language.
- **Data**. Data utilized by other classes found in Language.
- **Examples**. Two example languages for testing the generator and as a model for your language.


## 5 — Theory & Process

The generation process can be divided into two parts: *Deconstruction* and *construction*. Deconstruction breaks down a sentence into segments (specified by custom character filters). This greatly helps the construction process, which is responsible for handling how each filter block is processed. Since filters—and how they function—are defined by the language author, there is immense flexibility.

### 5.1 — **Deconstruction**.

The deconstruction process loops through the characters in your string, checking if the character matches any characters in any filter. In this case, if it's a letter, it starts counting. When it encounters a character from a different filter, it splits off the string, adds it to the list, and starts counting through the new filter block.

![plgl](https://user-images.githubusercontent.com/119130949/219261964-e5f91306-edc7-4e63-b995-ddf4b0e6a73f.png)

You could write these block separations plainly as: "The| |field| |of| |Enna|,|". It's from this list of character blocks that the constructor operates on (specifically, after they're added to a WordInfo class). You don't have to define every character; however, any unlisted character will be included anyway under the "UNDEFINED" filter, and will appear in the returned string.

After the deconstructor breakes the sentence down (the *first pass*), the new list of character blocks are looped through again, and the Deconstruct event is called on each. This is the second pass, processing all functions set by the language author. Some circumstances may require a block's filter to be changed, or three blocks to be merged into one. Words such as "let's", or numbers with commas or decimals, or even word flagging. I've included methods that help merge character blocks based on the specified criteria.

### 5.2 — **Construction**.

The OnConstruct event is the most crucial to implement. This is where you tell the generator how you want each filter to be processed. The LanguageGenerator class comes with a few common generation methods to speed up language authoring: `CONSTRUCT_Hide`, `CONSTRUCT_KeepAsIs`, `CONSTRUCT_Replace`, `CONSTRUCT_Within`, and `CONSTRUCT_Generate`. These methods start with `CONSTRUCT_` for clarity, so that auto-suggestion groups them together. If you plan to add any custom functionality (and you likely will), here's what KeepAsIs looks like:
```c#
public void CONSTRUCT_KeepAsIs(WordInfo word, string filter)
{
    //Make sure the filter matches and the word hasn't already been processed.
    if (word.Filter.Name.ToUpper() == filter.ToUpper() && word.IsProcessed == false)
    {
        //Set the final word to what the word started as.
        word.WordFinal = word.WordActual;
        word.IsProcessed = true;
    }
}
```

The filter check is the most important part. If it's not included, the method is applied to every word. We'll look at `CONSTRUCT_Generate` in the next section.

### 5.3 — **Generating Sentences**.

The generator starts by finding the root word by extracting any affixes. If none are found, the original word is the root word. If the root matches a key in Lexicon.Roots, the generated word will be set to its value. Then, the Random seed is set to the root (which is converted from a string to an integer).

Next up, the generator must select the syllable structure. Language.OnSyllableSelection is called, excluding any undesired syllables, and the remaining syllables are selected by weight. A custom syllable structure will be set if the word matches a key found in Lexicon.Syllables.

With the syllable structure set, the letters are chosen according to each syllable's letter group. For each letter group, Language.OnLetterSelection is called, excluding any undesired letters, and the remaining letters are selected by weight.

![plgl2_b](https://user-images.githubusercontent.com/119130949/226001137-0695c1a7-b07d-45f9-9ce6-b60a3bf6919e.png)

The affixes that were extracted earlier are processed and assembled by order. Language.OnPrefix and Language.OnSuffix is called during this process. This is useful if the affix needs to add a letter to make the word flow easier.

The final word is assembled with its prefixes, generated word, and suffixes put together. The word is memorized, so that it doesn't have to be processed twice (if enabled), and the case of the word is set to match the original word (if enabled). Now you have your new word. Unless you make changes to your language, or adjust the seed offset, it will make the same choices for that word every time.


## 6 — Setting Up

You should check out the Examples folder for ideas on authoring a language.

1. Initial setup.
    - Add a class to your project which derives from Language.
    - Put all methods in the constructor.
    - Fill in your language's metadata: name, description, author.
    - Set additional properties found in Language.Options.
2. Structuring.
    - Add consonants and vowels to your alphabet.
    - Add letter groups. These are the building blocks of syllables.
    - Add syllables.
3. Deconstruction.
    - Add character filters. Unlisted charactered are added to Undefined filter when a sentence is processed. Examples:
        - **Delimiter**. Usually just space. Highly recommended.
        - **Letters**. a-z, A-Z. This filter is essentially required.
        - **Numbers**. 0-9. Not required, but recommended.
        - **Punctuation**. Optional, but recommended.
        - **Flags**. Also needs FlagsOpen and FlagsClose during construction.
        - **Escape**. Allows the surrounded block to escape it's filter (e.g, "[Generate]" results in "Generate"). This can be added with flagging, so it's optional.
    - Add deconstruct events. This is the second pass, and corrects blocks through a stronger contextual lens. Some suggestions:
        - Absorb single apostrophe into Letters, decimals and commas into Numbers, and Letters into Escape filter with `DECONSTRUCT_MergeBlocks`.
        - For the Flags filter, use `DECONSTRUCT_ContainWithin`.
4. Construction. Add construct events (Language.Construct).
    - Keep Undefined and Delimiter with `CONSTRUCT_KeepAsIs`.
    - Set Letters to Generate with `CONSTRUCT_Generate`. This is essential.
    - Set Punctuation to `Punctuation.Process`.
    - Set Flags to `Flags.Process`.
5. Other options.
    - Add punctuation. Alternative punctuation marks, or a particle system, for stronger language style. `Punctuation.Add`
    - Add flags (<Hide, Hide>, NoGen, ). There are a few default actions in the Language.Flags class.


## 7 — Future Updates

- [x] Improve how affixes are handled.
- [x] Stronger control over syllable selection.
- [x] Better control over letters (perhaps with consonant doubling, diphthongs, or some other rules).
- [x] Easier, or less tedious, letter pathing.
- [x] Add generation logging to help authors diagnose and fix their language.
- [x] Add syllable rarity estimation, which returns the most (or least) likely syllables your language generates.
- [ ] Custom base conversion for numbers (low priority).
- [ ] More supporting methods in Diagnostics.
- [ ] Improve existing languages; especially exclusion rules and affixes.
- [ ] Support for creating "child" languages—taking two languages and blending them, skewed in different ways.
- [ ] Create new languages.


## 8 — Useful Resources

- Understanding vowels. [https://en.wikipedia.org/wiki/Vowel](https://en.wikipedia.org/wiki/Vowel)
- Pulmonic consonants for human-centric phonemes. [https://en.wikipedia.org/wiki/Pulmonic_consonant](https://en.wikipedia.org/wiki/Pulmonic_consonant)
- English phonology. [https://en.wikipedia.org/wiki/English_phonology](https://en.wikipedia.org/wiki/English_phonology)
- Emic units. [https://en.wikipedia.org/wiki/Emic_unit](https://en.wikipedia.org/wiki/Emic_unit)
- "Identifying Types of Affixes in English and Bahasa Indonesia". [http://eprints.binadarma.ac.id/12808/1/723-775-1-PB.pdf](http://eprints.binadarma.ac.id/12808/1/723-775-1-PB.pdf)
