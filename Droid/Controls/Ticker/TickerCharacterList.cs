using System;
using System.Collections.Generic;
using System.Linq;

namespace XamControls.Droid.Controls
{
    public class TickerCharacterList
    {
        private int numOriginalCharacters;
        // The saved character list will always be of the format: EMPTY, list, list
        private char[] characterList;
        // A minor optimization so that we can cache the indices of each character.
        private Dictionary<char, int> characterIndicesMap;

        public TickerCharacterList(string characterList)
        {
            if (characterList.Contains(TickerUtils.EMPTY_CHAR.ToString()))
            {
                throw new Exception(
                        "You cannot include TickerUtils.EMPTY_CHAR in the character list.");
            }

            char[] charsArray = characterList.ToCharArray();
            int length = charsArray.Length;
            this.numOriginalCharacters = length;

            characterIndicesMap = new Dictionary<char, int>(length);
            for (int i = 0; i < length; i++)
            {
                characterIndicesMap.Add(charsArray[i], i);
            }

            this.characterList = new char[length * 2 + 1];
            this.characterList[0] = TickerUtils.EMPTY_CHAR;
            for (int i = 0; i < length; i++)
            {
                this.characterList[1 + i] = charsArray[i];
                this.characterList[1 + length + i] = charsArray[i];
            }
        }

        public CharacterIndices getCharacterIndices(char start, char end)
        {
            int startIndex = getIndexOfChar(start);
            int endIndex = getIndexOfChar(end);
            if (startIndex < 0 || endIndex < 0)
            {
                return null;
            }

            // see if the wrap-around animation is shorter distance than the original animation
            if (start != TickerUtils.EMPTY_CHAR && end != TickerUtils.EMPTY_CHAR)
            {
                if (endIndex < startIndex)
                {
                    // If we are potentially going backwards
                    int nonWrapDistance = startIndex - endIndex;
                    int wrapDistance = numOriginalCharacters - startIndex + endIndex;
                    if (wrapDistance < nonWrapDistance)
                    {
                        endIndex += numOriginalCharacters;
                    }
                }
                else if (startIndex < endIndex)
                {
                    // If we are potentially going forwards
                    int nonWrapDistance = endIndex - startIndex;
                    int wrapDistance = numOriginalCharacters - endIndex + startIndex;
                    if (wrapDistance < nonWrapDistance)
                    {
                        startIndex += numOriginalCharacters;
                    }
                }
            }
            return new CharacterIndices(startIndex, endIndex);
        }

        public List<char> getSupportedCharacters()
        {
            return characterIndicesMap.Keys.ToList<char>();
        }

        public char[] getCharacterList()
        {
            return characterList;
        }

        private int getIndexOfChar(char c)
        {
            if (c == TickerUtils.EMPTY_CHAR)
            {
                return 0;
            }
            else if (characterIndicesMap.ContainsKey(c))
            {
                return characterIndicesMap.GetValueOrDefault(c) + 1;
            }
            else
            {
                return -1;
            }
        }

        public class CharacterIndices
        {
            public int startIndex;
            public int endIndex;

            public CharacterIndices(int startIndex, int endIndex)
            {
                this.startIndex = startIndex;
                this.endIndex = endIndex;
            }
        }
    }
}
