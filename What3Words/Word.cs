using CitizenFX.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace What3Words
{
    public class Word
    {
        public string WordOne, WordTwo, WordThree;

        public Vector2 Coords;

        public Word(string wordOne, string wordTwo, string wordThree)
        {
            WordOne = wordOne;
            WordTwo = wordTwo;
            WordThree = wordThree;
        }

        public string FullWord => WordOne + "." + WordTwo + "." + WordThree;

        public override bool Equals(object obj)
        {
            var word = obj as Word;
            return word != null &&
                   WordOne == word.WordOne &&
                   WordTwo == word.WordTwo &&
                   WordThree == word.WordThree;
        }

        public override int GetHashCode()
        {
            var hashCode = -1420882389;
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(WordOne);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(WordTwo);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(WordThree);
            return hashCode;
        }
    }
}
