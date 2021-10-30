using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IndexingTask
{
    class PorterStemmer
    {
        /// <summary>
        /// The Stemmer class transforms a word into its root form.
        /// Implementing the Porter Stemming Algorithm
        /// </summary>
        /// <remarks>
        /// Modified from: http://tartarus.org/martin/PorterStemmer/csharp2.txt
        /// </remarks>
        /// <example>
        /// var stemmer = new PorterStemmer();
        /// var stem = stemmer.StemWord(word);
        /// </example>
        /// 
        private char[] wordArray;
        private int endIndex;
        private int stemIndex;
        /// <summary>
        /// Stem the passed in word.
        /// </summary>
        /// <param name="word">Word to evaluate</param>
        /// <returns></returns>
        public string StemWord(string word)
        {
            if (string.IsNullOrWhiteSpace(word) || word.Length <= 2)
            {
                return word;
            }
            else
            {
                wordArray = word.ToCharArray();
                stemIndex = 0;
                endIndex = word.Length - 1;
                Step1(); 
                Step2();
                Step3();
                Step4();  
                Step5();  
                Step6();  
                var length = endIndex + 1;
                string after = new String(wordArray, 0, length);
                return after;
            }
        }


        // Step1() gets rid of plurals and -ed or -ing.
        /* Examples:
			   caresses  ->  caress
			   ponies    ->  poni
			   ties      ->  ti
			   caress    ->  caress
			   cats      ->  cat

			   feed      ->  feed
			   agreed    ->  agree
			   disabled  ->  disable

			   matting   ->  mat
			   mating    ->  mate
			   meeting   ->  meet
			   milling   ->  mill
			   messing   ->  mess

			   meetings  ->  meet  		*/
        private void Step1()
        {
            if (wordArray[endIndex] == 's')
            {
                if (EndsWith("sses"))
                {
                    endIndex -= 2;
                }
                else if (EndsWith("ies"))
                {
                    SetEnd("i");
                }
                else if (wordArray[endIndex - 1] != 's')
                {
                    endIndex--;
                }
            }
            if (EndsWith("eed"))
            {
                if (MeasureConsontantSequence() > 0)
                    endIndex--;
            }
            else if ((EndsWith("ed") || EndsWith("ing")) && VowelInStem())
            {
                endIndex = stemIndex;
                if (EndsWith("at"))
                    SetEnd("ate");
                else if (EndsWith("bl"))
                    SetEnd("ble");
                else if (EndsWith("iz"))
                    SetEnd("ize");
                else if (IsDoubleConsontant(endIndex))
                {
                    endIndex--;
                    int ch = wordArray[endIndex];
                    if (ch == 'l' || ch == 's' || ch == 'z')
                        endIndex++;
                }
                else if (MeasureConsontantSequence() == 1 && IsCVC(endIndex)) SetEnd("e");
            }
        }
        private void Step2()
        {
            if (EndsWith("y") && VowelInStem())
                wordArray[endIndex] = 'i';
        }

        private void Step3()
        {
            if (endIndex == 0) return;
            switch (wordArray[endIndex - 1])
            {
                case 'a':
                    if (EndsWith("ational")) { ReplaceEnd("ate"); break; }
                    if (EndsWith("tional")) { ReplaceEnd("tion"); }
                    break;
                case 'c':
                    if (EndsWith("enci")) { ReplaceEnd("ence"); break; }
                    if (EndsWith("anci")) { ReplaceEnd("ance"); }
                    break;
                case 'e':
                    if (EndsWith("izer")) { ReplaceEnd("ize"); }
                    break;
                case 'l':
                    if (EndsWith("bli")) { ReplaceEnd("ble"); break; }
                    if (EndsWith("alli")) { ReplaceEnd("al"); break; }
                    if (EndsWith("entli")) { ReplaceEnd("ent"); break; }
                    if (EndsWith("eli")) { ReplaceEnd("e"); break; }
                    if (EndsWith("ousli")) { ReplaceEnd("ous"); }
                    break;
                case 'o':
                    if (EndsWith("ization")) { ReplaceEnd("ize"); break; }
                    if (EndsWith("ation")) { ReplaceEnd("ate"); break; }
                    if (EndsWith("ator")) { ReplaceEnd("ate"); }
                    break;
                case 's':
                    if (EndsWith("alism")) { ReplaceEnd("al"); break; }
                    if (EndsWith("iveness")) { ReplaceEnd("ive"); break; }
                    if (EndsWith("fulness")) { ReplaceEnd("ful"); break; }
                    if (EndsWith("ousness")) { ReplaceEnd("ous"); }
                    break;
                case 't':
                    if (EndsWith("aliti")) { ReplaceEnd("al"); break; }
                    if (EndsWith("iviti")) { ReplaceEnd("ive"); break; }
                    if (EndsWith("biliti")) { ReplaceEnd("ble"); }
                    break;
                case 'g':
                    if (EndsWith("logi"))
                    {
                        ReplaceEnd("log");
                    }
                    break;
            }
        }
        private void Step4()
        {
            switch (wordArray[endIndex])
            {
                case 'e':
                    if (EndsWith("ate")) { ReplaceEnd("at"); break; }
                    if (EndsWith("icate")) { ReplaceEnd("ic"); break; }
                    if (EndsWith("ative")) { ReplaceEnd(""); break; }
                    if (EndsWith("alize")) { ReplaceEnd("al"); }
                    break;
                case 'i':
                    if (EndsWith("iciti")) { ReplaceEnd("ic"); }
                    break;
                case 'l':
                    if (EndsWith("ical")) { ReplaceEnd("ic"); break; }
                    if (EndsWith("ful")) { ReplaceEnd(""); }
                    break;
                case 's':
                    if (EndsWith("ness")) { ReplaceEnd(""); }

                    break;
            }
        }
        private void Step5()
        {
            if (endIndex == 0) return;

            switch (wordArray[endIndex - 1])
            {
                case 'a':
                    if (EndsWith("al")) break; return;
                case 'c':
                    if (EndsWith("ance")) break;
                    if (EndsWith("ence")) break; return;
                case 'e':
                    if (EndsWith("er")) break; return;
                case 'i':
                    if (EndsWith("ic")) break; return;
                case 'l':
                    if (EndsWith("able")) break;
                    if (EndsWith("ible")) break; return;
                case 'n':
                    if (EndsWith("ant")) break;
                    if (EndsWith("ement")) break;
                    if (EndsWith("ment")) break;
                    if (EndsWith("ent")) break; return;
                case 'o':
                    if (EndsWith("ion") && stemIndex >= 0 && (wordArray[stemIndex] == 's' || wordArray[stemIndex] == 't')) break;
                    if (EndsWith("ou")) break; return;
                case 's':
                    if (EndsWith("ism")) break; return;
                case 't':
                    if (EndsWith("ate")) break;
                    if (EndsWith("iti")) break; return;
                case 'u':
                    if (EndsWith("ous")) break; return;
                case 'v':
                    if (EndsWith("ive")) break; return;
                case 'z':
                    if (EndsWith("ize")) break; return;
                default:
                    return;
            }
            if (MeasureConsontantSequence() > 1)
                endIndex = stemIndex;
        }
        private void Step6()
        {
            stemIndex = endIndex;

            if (wordArray[endIndex] == 'e')
            {
                var a = MeasureConsontantSequence();
                if (a > 1 || a == 1 && !IsCVC(endIndex - 1))
                    endIndex--;
            }
            if (wordArray[endIndex] == 'l' && IsDoubleConsontant(endIndex) && MeasureConsontantSequence() > 1)
                endIndex--;
        }
        private bool IsConsonant(int index)
        {
            var c = wordArray[index];
            if (c == 'a' || c == 'e' || c == 'i' || c == 'o' || c == 'u') return false;
            return c != 'y' || (index == 0 || !IsConsonant(index - 1));
        }
        private int MeasureConsontantSequence()
        {
            var n = 0;
            var index = 0;
            while (true)
            {
                if (index > stemIndex) return n;
                if (!IsConsonant(index)) break; index++;
            }
            index++;
            while (true)
            {
                while (true)
                {
                    if (index > stemIndex) return n;
                    if (IsConsonant(index)) break;
                    index++;
                }
                index++;
                n++;
                while (true)
                {
                    if (index > stemIndex) return n;
                    if (!IsConsonant(index)) break;
                    index++;
                }
                index++;
            }
        }

        private bool VowelInStem()
        {
            int i;
            for (i = 0; i <= stemIndex; i++)
            {
                if (!IsConsonant(i)) return true;
            }
            return false;
        }
        private bool IsDoubleConsontant(int index)
        {
            if (index < 1) return false;
            return wordArray[index] == wordArray[index - 1] && IsConsonant(index);
        }
        private bool IsCVC(int index)
        {
            if (index < 2 || !IsConsonant(index) || IsConsonant(index - 1) || !IsConsonant(index - 2)) return false;
            var c = wordArray[index];
            return c != 'w' && c != 'x' && c != 'y';
        }
        private bool EndsWith(string s)
        {
            var length = s.Length;
            var index = endIndex - length + 1;
            if (index < 0) return false;

            for (var i = 0; i < length; i++)
            {
                if (wordArray[index + i] != s[i]) return false;
            }
            stemIndex = endIndex - length;
            return true;
        }
        private void SetEnd(string s)
        {
            var length = s.Length;
            var index = stemIndex + 1;
            for (var i = 0; i < length; i++)
            {
                wordArray[index + i] = s[i];
            }
            endIndex = stemIndex + length;
        }
        private void ReplaceEnd(string s)
        {
            if (MeasureConsontantSequence() > 0) SetEnd(s);
        }
    }
}
