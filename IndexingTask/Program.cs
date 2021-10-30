using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;
using HtmlAgilityPack;
using StopWord;
using Porter2Stemmer;
using NetSpell.SpellChecker.Dictionary;

namespace IndexingTask
{
    class Program
    {
        static Porter2Stemmer.EnglishPorter2Stemmer stemmer1 = new EnglishPorter2Stemmer();
        static List<db> pages;
        //static PorterStemmer stemmer = new PorterStemmer();
        private static List<char> delimeters = new List<char>() { '…', '’', '”', '\n', '€', '™', '°', ',', '%', '-', '£', '!', '?', '@', '#', '&', '(', ')', '–', '[', '{', '}', ']', ':', ';', '/', '*', '`', '~', '$', '^', '+', '=', '<', '>', '“', ' ', '.', '"', '\'', '\\', '|', '‘' };
        //private static List<string> Stopwords = new List<string>() { "i", "me", "my", "myself", "we", "our", "ours", "ourselves", "you", "your", "yours", "yourself", "yourselves", "he", "him", "his", "himself", "she", "her", "hers", "herself", "it", "its", "itself", "they", "them", "their", "theirs", "themselves", "what", "which", "who", "whom", "this", "that", "these", "those", "am", "is", "are", "was", "were", "be", "been", "being", "have", "has", "had", "having", "do", "does", "did", "doing", "a", "an", "the", "and", "but", "if", "or", "because", "as", "until", "while", "of", "at", "by", "for", "with", "about", "against", "between", "into", "through", "during", "before", "after", "above", "below", "to", "from", "up", "down", "in", "out", "on", "off", "over", "under", "again", "further", "then", "once", "here", "there", "when", "where", "why", "how", "all", "any", "both", "each", "few", "more", "most", "other", "some", "such", "no", "nor", "not", "only", "own", "same", "so", "than", "too", "very", "s", "t", "can", "will", "just", "don", "should", "now", "don’t", "’ll", "’s", "n’t", "according", "accordingly", "across", "actually", "adj", "after", "afterwards", "few", "whom", "t", "being", "if", "theirs", "my", "against", "a", "by", "doing", "it", "how", "further", "then", "that", "because", "what", "over", "why", "so", "can", "did", "not", "now", "under"};
        private static int no_pages = 0; 

        static WordDictionary oDict = new NetSpell.SpellChecker.Dictionary.WordDictionary();
        static NetSpell.SpellChecker.Spelling spelling;

        static void Main(string[] args)
        {
            pages = db.SqlConn();
            bool saved = false;
            oDict.DictionaryFile = "en-US.dic";
            oDict.Initialize();
            foreach (db doc in pages)
            {
                if (no_pages < 1501)
                {
                    string body = Extract_text(doc);
                    //Console.WriteLine(doc.db_url);
                    //Console.WriteLine(body);
                    List<string> Tokenizer_list = Tokenize_body(body);
                    Dictionary<string, string> linguistics_word = Apply_linguistics(Tokenizer_list);
                    foreach (KeyValuePair<string, string> entry in linguistics_word) 
                    { 
                        spelling = new NetSpell.SpellChecker.Spelling();
                        spelling.Dictionary = oDict;

                        if (spelling.TestWord(entry.Key))
                        {
                            saved = true;
                            KeyValuePair<int, string>freqPos = GetFrequencyAndPosition(entry.Key, body);
                            Console.WriteLine(entry.Key);
                            Console.WriteLine(entry.Value);

                            //trems_berfore, terms_after, doc_id, freq, pos
                            db.store(entry.Key, entry.Value, doc.db_id, freqPos.Key, freqPos.Value);
                        }
                        spelling.Dispose();
                    }
                    if(saved)
                    { no_pages++; Console.WriteLine(no_pages); }
                    saved = false;

                }
                else
                    break;
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////
        private static string Extract_text(db content)
        {
            var doc = new HtmlDocument();
            //Convert string to HtmlDocument
            doc.LoadHtml(content.db_content);
            string txt = doc.DocumentNode.SelectNodes("//body")[0].InnerText;
            return txt;
        }

        /*private static string Extract_text(db content)
        {
            var doc = new HtmlDocument();
            //Convert string to HtmlDocument
            doc.LoadHtml(content.db_content);
            var root = doc.DocumentNode;
            var txt = new StringBuilder();
            foreach(var node in root.DescendantNodesAndSelf())
            {
                if(!node.HasChildNodes)
                {
                    string text = node.InnerText;
                    if (!string.IsNullOrEmpty(text))
                        txt.Append(text.Trim());
                }
            }
            return txt.ToString();
        }*/
        ///////////////////////////////////////////////////////////////////////////////////////////////////////
        private static List<string> Tokenize_body(string extracted_text)
        {
            int cntr=0; 
            List<string> Tokenizer = new List<string>();
            List<string> WordsWithoutDelimeters = extracted_text.Split(delimeters.ToArray()).ToList();
            foreach (string word in WordsWithoutDelimeters)
            {
                string saved = null;
                //not empty or newline
                if (word != string.Empty && word != Environment.NewLine)
                {
                    foreach(char character in word)
                    {
                        if(character != ' ' && character.ToString() != "   ")
                        {
                            saved += character;
                            cntr++;
                        }
                    }
                    if (cntr > 1)
                        Tokenizer.Add(saved);
                    cntr = 0;
                }
            }
            return Tokenizer;
        }
        //////////////////////////////////////////////////////////////////////////////////////////////////////
        private static Dictionary<string, string> Apply_linguistics(List<string> tokens)
        {

            // where key is word before stemming and value is word after stemming
            Dictionary<string, string> terms = new Dictionary<string, string>();
            Dictionary<string, string> check_term = new Dictionary<string, string>();
            var stopWords = StopWords.GetStopWords("en");
            for (int i = 0; i < tokens.Count(); i++)
            {
                string word = tokens[i].ToLower();
                if (!stopWords.Contains(word) && !check_term.ContainsKey(word) && word.Length > 2)
                {
                    //word=> before applying linguistics
                    //stemmer.StemWord(word)=> after applying linguistics
                    //terms.Add(new KeyValuePair<string, string>(tokens[i], stemmer.StemWord(word));
                    string before = stemmer1.Stem(word).Unstemmed;
                    string after = stemmer1.Stem(word).Value;
                    terms.Add(before,after);
                    check_term.Add(word, "");
                }
            }
            return terms;
        }
        /////////////////////////////////////////////////////////////////////////////////////////////////////
        private static KeyValuePair<int, string> GetFrequencyAndPosition(string word, string body)
        {
            string positions = null;
            int frequency = 0;
            List<string> tokens = Tokenize_body(body);
            for (int i = 0; i < tokens.Count; i++)
            {
                if (tokens[i].ToLower() == word.ToLower())
                {
                    positions += (i + 1).ToString() + ",";
                    frequency++;
                }
            }
            return (new KeyValuePair<int, string>(frequency, positions));
        }
    }
}
