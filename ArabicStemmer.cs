using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArabicStemmer
{
    public class Stemmer
    {
        // for the get_root function
        private static List<List<string>> staticFiles;
        public static string pathToStemmerFiles = "StemmerFiles/";

        private static bool rootFound = false;
        private static bool stopwordFound = false;
        private static bool fromSuffixes = false;
        private static string[][] stemmedDocument = new string[10000][]; //3
        private static int wordNumber = 0;
        private static int stopwordNumber = 0;
        private static int stemmedWordsNumber = 0;
        private static ArrayList listStemmedWords = new ArrayList();
        private static ArrayList listRootsFound = new ArrayList();
        private static ArrayList listNotStemmedWords = new ArrayList();
        private static ArrayList listStopwordsFound = new ArrayList();
        private static ArrayList listOriginalStopword = new ArrayList();
        private static bool rootNotFound = false;
        private static ArrayList wordsNotStemmed = new ArrayList();
        static int number = 0;
        private static string[][] possibleRoots = new string[10000][];
        private static string roots = "";
        private static Dictionary<string, string[]> cache = new Dictionary<string, string[]>();

        public Stemmer(string stemFilesPath)
        {
            pathToStemmerFiles = stemFilesPath;

            initComponents();
            cache[stemFilesPath + "definite_article.txt"] = File.ReadAllLines(stemFilesPath + "definite_article.txt");
            cache[stemFilesPath + "diacritics.txt"] = File.ReadAllLines(stemFilesPath + "diacritics.txt");
            cache[stemFilesPath + "duplicate.txt"] = File.ReadAllLines(stemFilesPath + "duplicate.txt");
            cache[stemFilesPath + "first_waw.txt"] = File.ReadAllLines(stemFilesPath + "first_waw.txt");
            cache[stemFilesPath + "first_yah.txt"] = File.ReadAllLines(stemFilesPath + "first_yah.txt");
            cache[stemFilesPath + "last_alif.txt"] = File.ReadAllLines(stemFilesPath + "last_alif.txt");
            cache[stemFilesPath + "last_hamza.txt"] = File.ReadAllLines(stemFilesPath + "last_hamza.txt");
            cache[stemFilesPath + "last_maksoura.txt"] = File.ReadAllLines(stemFilesPath + "last_maksoura.txt");
            cache[stemFilesPath + "last_yah.txt"] = File.ReadAllLines(stemFilesPath + "last_yah.txt");
            cache[stemFilesPath + "mid_waw.txt"] = File.ReadAllLines(stemFilesPath + "mid_waw.txt");
            cache[stemFilesPath + "mid_yah.txt"] = File.ReadAllLines(stemFilesPath + "mid_yah.txt");
            cache[stemFilesPath + "prefixes.txt"] = File.ReadAllLines(stemFilesPath + "prefixes.txt");
            cache[stemFilesPath + "punctuation.txt"] = File.ReadAllLines(stemFilesPath + "punctuation.txt");
            cache[stemFilesPath + "quad_roots.txt"] = File.ReadAllLines(stemFilesPath + "quad_roots.txt");
            cache[stemFilesPath + "stopwords.txt"] = File.ReadAllLines(stemFilesPath + "stopwords.txt");
            cache[stemFilesPath + "strange.txt"] = File.ReadAllLines(stemFilesPath + "strange.txt");
            cache[stemFilesPath + "suffixes.txt"] = File.ReadAllLines(stemFilesPath + "suffixes.txt");
            cache[stemFilesPath + "tri_patt.txt"] = File.ReadAllLines(stemFilesPath + "tri_patt.txt");
            cache[stemFilesPath + "tri_roots.txt"] = File.ReadAllLines(stemFilesPath + "tri_roots.txt");
        }

        private static void initComponents()
        {
            rootFound = false;
            stopwordFound = false;
            fromSuffixes = false;
            stemmedDocument = new string[10000][]; // 3
            wordNumber = 0;
            stopwordNumber = 0;
            stemmedWordsNumber = 0;
            listStemmedWords = new ArrayList();
            listRootsFound = new ArrayList();
            listNotStemmedWords = new ArrayList();
            listStopwordsFound = new ArrayList();
            listOriginalStopword = new ArrayList();
            rootNotFound = false;
            wordsNotStemmed = new ArrayList();
            number = 0;

            for (int i = 0; i < 10000; i++)
                stemmedDocument[i] = new string[3];

            for (int i = 0; i < 10000; i++)
                possibleRoots[i] = new string[100];
        }

        /// <summary>
        /// read in the contents of a file, put it into a vector, and add that vector
        /// to the vector composed of vectors containing the static files
        /// </summary>
        protected static bool addVectorFromFile(string fileName)
        {
            bool returnValue;
            try
            {
                // the vector we are going to fill
                var vectorFromFile = new List<string>();

                // read in the text a line at a time
                string[] lines = cache[fileName];

                // split text lines into clips based on space
                var clips = lines.SelectMany(line => line.Split(" ", StringSplitOptions.RemoveEmptyEntries)).ToArray();

                vectorFromFile.AddRange(clips);

                // add the vector to the vector composed of vectors containing the
                // static files
                staticFiles.Add(vectorFromFile);
                returnValue = true;
            }
            catch (Exception ex)
            {
                Debug.Print("fileName={0}\r\n{1}", fileName, ex.ToString());
                returnValue = false;
            }
            return returnValue;
        }

        /// <summary>
        /// check and remove any prefixes
        /// </summary>
        private static string checkForPrefixes(string word)
        {
            Debug.Print("Enter checkForPrefix " + word);
            staticFiles = new List<List<string>>();
            if (addVectorFromFile(pathToStemmerFiles + "definite_article.txt"))
                if (addVectorFromFile(pathToStemmerFiles + "duplicate.txt"))
                    if (addVectorFromFile(pathToStemmerFiles + "first_waw.txt"))
                        if (addVectorFromFile(pathToStemmerFiles + "first_yah.txt"))
                            if (addVectorFromFile(pathToStemmerFiles + "last_alif.txt"))
                                if (addVectorFromFile(pathToStemmerFiles + "last_hamza.txt"))
                                    if (addVectorFromFile(pathToStemmerFiles + "last_maksoura.txt"))
                                        if (addVectorFromFile(pathToStemmerFiles + "last_yah.txt"))
                                            if (addVectorFromFile(pathToStemmerFiles + "mid_waw.txt"))
                                                if (addVectorFromFile(pathToStemmerFiles + "mid_yah.txt"))
                                                    if (addVectorFromFile(pathToStemmerFiles + "prefixes.txt"))
                                                        if (addVectorFromFile(pathToStemmerFiles + "punctuation.txt"))
                                                            if (addVectorFromFile(pathToStemmerFiles + "quad_roots.txt"))
                                                                if (addVectorFromFile(pathToStemmerFiles + "stopwords.txt"))
                                                                    if (addVectorFromFile(pathToStemmerFiles + "suffixes.txt"))
                                                                        if (addVectorFromFile(pathToStemmerFiles + "tri_patt.txt"))
                                                                            if (addVectorFromFile(pathToStemmerFiles + "tri_roots.txt"))
                                                                                if (addVectorFromFile(pathToStemmerFiles + "diacritics.txt"))
                                                                                    if (addVectorFromFile(pathToStemmerFiles + "strange.txt"))
                                                                                    {
                                                                                        // the vector was successfully created
                                                                                        //System.out.println( "read in files successfully" );
                                                                                    }
            //System.out.println("Start checkForPrefix");


            string prefix = "";
            string modifiedWord = word;
            var prefixes = staticFiles[10];
            //System.out.println("for checkForPrefix");
            // for every prefix in the list
            for (int i = 0; i < prefixes.Count; i++)
            {
                prefix = (string)prefixes[i];
                // if the prefix was found
                if (modifiedWord.StartsWith(prefix))
                //if (prefix.regionMatches(0, modifiedWord, 0, prefix.length()))
                {
                    modifiedWord = modifiedWord.Substring(prefix.Length);

                    // check to see if the word is a stopword
                    if (checkStopwords(modifiedWord))
                        return modifiedWord;

                    // check to see if the word is a root of three or four letters
                    // if the word has only two letters, test to see if one was removed
                    if (modifiedWord.Length == 2)
                        modifiedWord = isTwoLetters(modifiedWord);
                    else if (modifiedWord.Length == 3 && !rootFound)
                        modifiedWord = isThreeLetters(modifiedWord);
                    else if (modifiedWord.Length == 4)
                        isFourLetters(modifiedWord);

                    // if the root hasn't been found, check for patterns
                    if (!rootFound && modifiedWord.Length > 2)
                        modifiedWord = checkPatterns(modifiedWord);

                    // if the root STILL hasn't been found
                    if (!rootFound && !stopwordFound && !fromSuffixes)
                    {
                        // check for suffixes
                        modifiedWord = checkForSuffixes(modifiedWord);
                    }

                    if (stopwordFound)
                        return modifiedWord;

                    // if the root was found, return the modified word
                    if (rootFound && !stopwordFound)
                    {
                        return modifiedWord;
                    }
                }
            }
            return word;
        }

        private static bool checkStopwords(string currentWord)
        {
            if (staticFiles == null || staticFiles.Count == 0)
                throw new InvalidOperationException("Arabic stemmer Initialization did not done properly");

            var v = staticFiles[13];

            if (stopwordFound = v.Contains(currentWord))
            {
                stemmedDocument[wordNumber][1] = currentWord;
                stemmedDocument[wordNumber][2] = "STOPWORD";
                stopwordNumber++;
                listStopwordsFound.Add(currentWord);
                listOriginalStopword.Add(stemmedDocument[wordNumber][0]);

            }
            return stopwordFound;
        }

        private static string isTwoLetters(string word)
        {
            // if the word consists of two letters, then this could be either
            // - because it is a root consisting of two letters (though I can't think of any!)
            // - because a letter was deleted as it is duplicated or a weak middle or last letter.
            staticFiles = new List<List<string>>();
            if (addVectorFromFile(pathToStemmerFiles + "definite_article.txt"))
                if (addVectorFromFile(pathToStemmerFiles + "duplicate.txt"))
                    if (addVectorFromFile(pathToStemmerFiles + "first_waw.txt"))
                        if (addVectorFromFile(pathToStemmerFiles + "first_yah.txt"))
                            if (addVectorFromFile(pathToStemmerFiles + "last_alif.txt"))
                                if (addVectorFromFile(pathToStemmerFiles + "last_hamza.txt"))
                                    if (addVectorFromFile(pathToStemmerFiles + "last_maksoura.txt"))
                                        if (addVectorFromFile(pathToStemmerFiles + "last_yah.txt"))
                                            if (addVectorFromFile(pathToStemmerFiles + "mid_waw.txt"))
                                                if (addVectorFromFile(pathToStemmerFiles + "mid_yah.txt"))
                                                    if (addVectorFromFile(pathToStemmerFiles + "prefixes.txt"))
                                                        if (addVectorFromFile(pathToStemmerFiles + "punctuation.txt"))
                                                            if (addVectorFromFile(pathToStemmerFiles + "quad_roots.txt"))
                                                                if (addVectorFromFile(pathToStemmerFiles + "stopwords.txt"))
                                                                    if (addVectorFromFile(pathToStemmerFiles + "suffixes.txt"))
                                                                        if (addVectorFromFile(pathToStemmerFiles + "tri_patt.txt"))
                                                                            if (addVectorFromFile(pathToStemmerFiles + "tri_roots.txt"))
                                                                                if (addVectorFromFile(pathToStemmerFiles + "diacritics.txt"))
                                                                                    if (addVectorFromFile(pathToStemmerFiles + "strange.txt"))
                                                                                    {
                                                                                        // the vector was successfully created
                                                                                        //System.out.println( "read in files successfully" );
                                                                                    }

            word = duplicate(word);

            // check if the last letter was weak
            if (!rootFound)
                word = lastWeak(word);

            // check if the first letter was weak
            if (!rootFound)
                word = firstWeak(word);

            // check if the middle letter was weak
            if (!rootFound)
                word = middleWeak(word);

            return word;
        }

        // if the word consists of three letters
        private static string isThreeLetters(string word)
        {
            if (staticFiles == null || staticFiles.Count == 0)
                throw new InvalidOperationException("Arabic stemmer Initialization did not done properly");

            StringBuilder modifiedWord = new StringBuilder(word);
            string root = "";
            // if the first letter is a 'ا', 'ؤ'  or 'ئ'
            // then change it to a 'أ'
            if (word.Length > 0)
            {
                if (word[0] == '\u0627' || word[0] == '\u0624' || word[0] == '\u0626')
                {
                    modifiedWord.Clear();
                    modifiedWord.Append('\u0623');
                    modifiedWord.Append(word.Substring(1));
                    root = modifiedWord.ToString();
                }

                // if the last letter is a weak letter or a hamza
                // then remove it and check for last weak letters
                if (word[2] == '\u0648' || word[2] == '\u064a' || word[2] == '\u0627' ||
                     word[2] == '\u0649' || word[2] == '\u0621' || word[2] == '\u0626')
                {
                    root = word.Substring(0, 2);
                    root = lastWeak(root);
                    if (rootFound)
                    {
                        return root;
                    }
                }

                // if the second letter is a weak letter or a hamza
                // then remove it
                if (word[1] == '\u0648' || word[1] == '\u064a' || word[1] == '\u0627' || word[1] == '\u0626')
                {
                    root = word.Substring(0, 1);
                    root = root + word.Substring(2);

                    root = middleWeak(root);
                    if (rootFound)
                    {
                        return root;
                    }
                }

                // if the second letter has a hamza, and it's not on a alif
                // then it must be returned to the alif
                if (word[1] == '\u0624' || word[1] == '\u0626')
                {
                    if (word[2] == '\u0645' || word[2] == '\u0632' || word[2] == '\u0631')
                    {
                        root = word.Substring(0, 1);
                        root = root + '\u0627';
                        root = root + word.Substring(2);
                    }
                    else
                    {
                        root = word.Substring(0, 1);
                        root = root + '\u0623';
                        root = root + word.Substring(2);
                    }
                }

                // if the last letter is a shadda, remove it and
                // duplicate the last letter
                if (word[2] == '\u0651')
                {
                    root = word.Substring(0, 1);
                    root = root + word.Substring(1, 2);
                }
            }

            // if word is a root, then rootFound is true
            if (root.Length == 0)
            {
                if (staticFiles[16].Contains(word))
                {
                    rootFound = true;
                    stemmedDocument[wordNumber][1] = word;
                    stemmedDocument[wordNumber][2] = "ROOT";
                    stemmedWordsNumber++;
                    listStemmedWords.Add(stemmedDocument[wordNumber][0]);
                    listRootsFound.Add(word);
                    if (rootNotFound)
                    {
                        for (int i = 0; i < number; i++)
                            wordsNotStemmed.RemoveAt(wordsNotStemmed.Count - 1);
                        rootNotFound = false;
                    }
                    return word;
                }
            }
            // check for the root that we just derived
            else if (staticFiles[16].Contains(root))
            {
                rootFound = true;
                stemmedDocument[wordNumber][1] = root;
                stemmedDocument[wordNumber][2] = "ROOT";
                stemmedWordsNumber++;
                listStemmedWords.Add(stemmedDocument[wordNumber][0]);
                listRootsFound.Add(word);
                if (rootNotFound)
                {
                    for (int i = 0; i < number; i++)
                        wordsNotStemmed.RemoveAt(wordsNotStemmed.Count - 1);
                    rootNotFound = false;
                }
                return root;
            }

            if (root.Length == 3)
            {
                possibleRoots[number][1] = root;
                possibleRoots[number][0] = stemmedDocument[wordNumber][0];
                number++;
            }
            else
            {
                //            possibleRoots[number][1] = word;
                //        possibleRoots[number][0] = stemmedDocument[wordNumber][0];
                number++;
            }
            return word;
        }

        // if the word has four letters
        private static void isFourLetters(string word)
        {
            staticFiles = new List<List<string>>();
            if (addVectorFromFile(pathToStemmerFiles + "definite_article.txt"))
                if (addVectorFromFile(pathToStemmerFiles + "duplicate.txt"))
                    if (addVectorFromFile(pathToStemmerFiles + "first_waw.txt"))
                        if (addVectorFromFile(pathToStemmerFiles + "first_yah.txt"))
                            if (addVectorFromFile(pathToStemmerFiles + "last_alif.txt"))
                                if (addVectorFromFile(pathToStemmerFiles + "last_hamza.txt"))
                                    if (addVectorFromFile(pathToStemmerFiles + "last_maksoura.txt"))
                                        if (addVectorFromFile(pathToStemmerFiles + "last_yah.txt"))
                                            if (addVectorFromFile(pathToStemmerFiles + "mid_waw.txt"))
                                                if (addVectorFromFile(pathToStemmerFiles + "mid_yah.txt"))
                                                    if (addVectorFromFile(pathToStemmerFiles + "prefixes.txt"))
                                                        if (addVectorFromFile(pathToStemmerFiles + "punctuation.txt"))
                                                            if (addVectorFromFile(pathToStemmerFiles + "quad_roots.txt"))
                                                                if (addVectorFromFile(pathToStemmerFiles + "stopwords.txt"))
                                                                    if (addVectorFromFile(pathToStemmerFiles + "suffixes.txt"))
                                                                        if (addVectorFromFile(pathToStemmerFiles + "tri_patt.txt"))
                                                                            if (addVectorFromFile(pathToStemmerFiles + "tri_roots.txt"))
                                                                                if (addVectorFromFile(pathToStemmerFiles + "diacritics.txt"))
                                                                                    if (addVectorFromFile(pathToStemmerFiles + "strange.txt"))
                                                                                    {
                                                                                        // the vector was successfully created
                                                                                        //System.out.println( "read in files successfully" );
                                                                                    }

            // if word is a root, then rootFound is true
            if (staticFiles[12].Contains(word))
            {
                rootFound = true;

                stemmedDocument[wordNumber][1] = word;
                stemmedDocument[wordNumber][2] = "ROOT";
                stemmedWordsNumber++;
                listStemmedWords.Add(stemmedDocument[wordNumber][0]);
                listRootsFound.Add(word);
            }
        }

        // check if the word matches any of the patterns
        private static string checkPatterns(string word)
        {
            if (staticFiles == null || staticFiles.Count == 0)
                throw new InvalidOperationException("Arabic stemmer Initialization did not done properly");

            StringBuilder root = new StringBuilder("");
            // if the first letter is a hamza, change it to an alif
            if (word.Length > 0)
                if (word[0] == '\u0623' || word[0] == '\u0625' || word[0] == '\u0622')
                {
                    root.Append("");
                    root.Insert(0, '\u0627');
                    root.Append(word.Substring(1));
                    word = root.ToString();
                }

            // try and find a pattern that matches the word
            var patterns = staticFiles[15];
            int numberSameLetters = 0;
            string pattern = "";
            string modifiedWord = "";

            // for every pattern
            for (int i = 0; i < patterns.Count; i++)
            {
                pattern = patterns[i];
                root.Clear();
                // if the length of the words are the same
                if (pattern.Length == word.Length)
                {
                    numberSameLetters = 0;
                    // find out how many letters are the same at the same index
                    // so long as they're not a fa, ain, or lam
                    for (int j = 0; j < word.Length; j++)
                        if (pattern[j] == word[j] && pattern[j] != '\u0641' && pattern[j] != '\u0639' && pattern[j] != '\u0644')
                            numberSameLetters++;

                    // test to see if the word matches the pattern افعلا
                    if (word.Length == 6 && word[3] == word[5] && numberSameLetters == 2)
                    {
                        root.Append(word[1]);
                        root.Append(word[2]);
                        root.Append(word[3]);
                        modifiedWord = root.ToString();
                        modifiedWord = isThreeLetters(modifiedWord);
                        if (rootFound)
                            return modifiedWord;
                        else
                            root.Clear();
                    }


                    // if the word matches the pattern, get the root
                    if (word.Length - 3 <= numberSameLetters)
                    {
                        // derive the root from the word by matching it with the pattern
                        for (int j = 0; j < word.Length; j++)
                            if (pattern[j] == '\u0641' || pattern[j] == '\u0639' || pattern[j] == '\u0644')
                                root.Append(word[j]);

                        modifiedWord = root.ToString();
                        modifiedWord = isThreeLetters(modifiedWord);

                        if (rootFound)
                        {
                            word = modifiedWord;
                            return word;
                        }
                    }
                }
            }
            return word;
        }


        // METHOD CHECKFORSUFFIXES
        private static string checkForSuffixes(string word)
        {
            if (staticFiles == null || staticFiles.Count == 0)
                throw new InvalidOperationException("Arabic stemmer Initialization did not done properly");

            string modifiedWord = word;
            var suffixes = staticFiles[14];
            fromSuffixes = true;

            // for every suffix in the list
            foreach (var suffix in suffixes)
            {
                // if the suffix was found
                if (modifiedWord.EndsWith(suffix))
                {
                    modifiedWord = modifiedWord.Substring(0, modifiedWord.Length - suffix.Length);

                    // check to see if the word is a stopword
                    if (checkStopwords(modifiedWord))
                    {
                        fromSuffixes = false;
                        return modifiedWord;
                    }

                    // check to see if the word is a root of three or four letters
                    // if the word has only two letters, test to see if one was removed
                    if (modifiedWord.Length == 2)
                    {
                        modifiedWord = isTwoLetters(modifiedWord);
                    }
                    else if (modifiedWord.Length == 3)
                    {
                        modifiedWord = isThreeLetters(modifiedWord);
                    }
                    else if (modifiedWord.Length == 4)
                    {
                        isFourLetters(modifiedWord);
                    }

                    // if the root hasn't been found, check for patterns
                    if (!rootFound && modifiedWord.Length > 2)
                    {
                        modifiedWord = checkPatterns(modifiedWord);
                    }

                    if (stopwordFound)
                    {
                        fromSuffixes = false;
                        return modifiedWord;
                    }

                    // if the root was found, return the modified word
                    if (rootFound)
                    {
                        fromSuffixes = false;
                        return modifiedWord;
                    }
                }
            }
            fromSuffixes = false;
            return word;
        }

        // handle duplicate letters in the word
        private static string duplicate(string word)
        {
            staticFiles = new List<List<string>>();
            if (addVectorFromFile(pathToStemmerFiles + "definite_article.txt"))
                if (addVectorFromFile(pathToStemmerFiles + "duplicate.txt"))
                    if (addVectorFromFile(pathToStemmerFiles + "first_waw.txt"))
                        if (addVectorFromFile(pathToStemmerFiles + "first_yah.txt"))
                            if (addVectorFromFile(pathToStemmerFiles + "last_alif.txt"))
                                if (addVectorFromFile(pathToStemmerFiles + "last_hamza.txt"))
                                    if (addVectorFromFile(pathToStemmerFiles + "last_maksoura.txt"))
                                        if (addVectorFromFile(pathToStemmerFiles + "last_yah.txt"))
                                            if (addVectorFromFile(pathToStemmerFiles + "mid_waw.txt"))
                                                if (addVectorFromFile(pathToStemmerFiles + "mid_yah.txt"))
                                                    if (addVectorFromFile(pathToStemmerFiles + "prefixes.txt"))
                                                        if (addVectorFromFile(pathToStemmerFiles + "punctuation.txt"))
                                                            if (addVectorFromFile(pathToStemmerFiles + "quad_roots.txt"))
                                                                if (addVectorFromFile(pathToStemmerFiles + "stopwords.txt"))
                                                                    if (addVectorFromFile(pathToStemmerFiles + "suffixes.txt"))
                                                                        if (addVectorFromFile(pathToStemmerFiles + "tri_patt.txt"))
                                                                            if (addVectorFromFile(pathToStemmerFiles + "tri_roots.txt"))
                                                                                if (addVectorFromFile(pathToStemmerFiles + "diacritics.txt"))
                                                                                    if (addVectorFromFile(pathToStemmerFiles + "strange.txt"))
                                                                                    {
                                                                                        // the vector was successfully created
                                                                                        //System.out.println( "read in files successfully" );
                                                                                    }

            // check if a letter was duplicated
            if (staticFiles[1].Contains(word))
            {
                // if so, then return the deleted duplicate letter
                word = word + word.Substring(1);

                // root was found, so set variable
                rootFound = true;

                stemmedDocument[wordNumber][1] = word;
                stemmedDocument[wordNumber][2] = "ROOT";
                stemmedWordsNumber++;
                listStemmedWords.Add(stemmedDocument[wordNumber][0]);
                listRootsFound.Add(word);

                return word;
            }
            return word;
        }

        // check if the last letter of the word is a weak letter
        private static string lastWeak(string word)
        {
            staticFiles = new List<List<string>>();
            if (addVectorFromFile(pathToStemmerFiles + "definite_article.txt"))
                if (addVectorFromFile(pathToStemmerFiles + "duplicate.txt"))
                    if (addVectorFromFile(pathToStemmerFiles + "first_waw.txt"))
                        if (addVectorFromFile(pathToStemmerFiles + "first_yah.txt"))
                            if (addVectorFromFile(pathToStemmerFiles + "last_alif.txt"))
                                if (addVectorFromFile(pathToStemmerFiles + "last_hamza.txt"))
                                    if (addVectorFromFile(pathToStemmerFiles + "last_maksoura.txt"))
                                        if (addVectorFromFile(pathToStemmerFiles + "last_yah.txt"))
                                            if (addVectorFromFile(pathToStemmerFiles + "mid_waw.txt"))
                                                if (addVectorFromFile(pathToStemmerFiles + "mid_yah.txt"))
                                                    if (addVectorFromFile(pathToStemmerFiles + "prefixes.txt"))
                                                        if (addVectorFromFile(pathToStemmerFiles + "punctuation.txt"))
                                                            if (addVectorFromFile(pathToStemmerFiles + "quad_roots.txt"))
                                                                if (addVectorFromFile(pathToStemmerFiles + "stopwords.txt"))
                                                                    if (addVectorFromFile(pathToStemmerFiles + "suffixes.txt"))
                                                                        if (addVectorFromFile(pathToStemmerFiles + "tri_patt.txt"))
                                                                            if (addVectorFromFile(pathToStemmerFiles + "tri_roots.txt"))
                                                                                if (addVectorFromFile(pathToStemmerFiles + "diacritics.txt"))
                                                                                    if (addVectorFromFile(pathToStemmerFiles + "strange.txt"))
                                                                                    {
                                                                                        // the vector was successfully created
                                                                                        //System.out.println( "read in files successfully" );
                                                                                    }

            StringBuilder stemmedWord = new StringBuilder("");
            // check if the last letter was an alif
            if (staticFiles[4].Contains(word))
            {
                stemmedWord.Append(word);
                stemmedWord.Append("\u0627");
                word = stemmedWord.ToString();
                stemmedWord.Clear();

                // root was found, so set variable
                rootFound = true;

                stemmedDocument[wordNumber][1] = word;
                stemmedDocument[wordNumber][2] = "ROOT";
                stemmedWordsNumber++;
                listStemmedWords.Add(stemmedDocument[wordNumber][0]);
                listRootsFound.Add(word);

                return word;
            }
            // check if the last letter was an hamza
            else if (staticFiles[5].Contains(word))
            {
                stemmedWord.Append(word);
                stemmedWord.Append("\u0623");
                word = stemmedWord.ToString();
                stemmedWord.Clear();

                // root was found, so set variable
                rootFound = true;

                stemmedDocument[wordNumber][1] = word;
                stemmedDocument[wordNumber][2] = "ROOT";
                stemmedWordsNumber++;
                listStemmedWords.Add(stemmedDocument[wordNumber][0]);
                listRootsFound.Add(word);

                return word;
            }
            // check if the last letter was an maksoura
            else if (staticFiles[6].Contains(word))
            {
                stemmedWord.Append(word);
                stemmedWord.Append("\u0649");
                word = stemmedWord.ToString();
                stemmedWord.Clear();

                // root was found, so set variable
                rootFound = true;

                stemmedDocument[wordNumber][1] = word;
                stemmedDocument[wordNumber][2] = "ROOT";
                stemmedWordsNumber++;
                listStemmedWords.Add(stemmedDocument[wordNumber][0]);
                listRootsFound.Add(word);

                return word;
            }
            // check if the last letter was an yah
            else if (staticFiles[7].Contains(word))
            {
                stemmedWord.Append(word);
                stemmedWord.Append("\u064a");
                word = stemmedWord.ToString();
                stemmedWord.Clear();

                // root was found, so set variable
                rootFound = true;

                stemmedDocument[wordNumber][1] = word;
                stemmedDocument[wordNumber][2] = "ROOT";
                stemmedWordsNumber++;
                listStemmedWords.Add(stemmedDocument[wordNumber][0]);
                listRootsFound.Add(word);

                return word;
            }
            return word;
        }

        //--------------------------------------------------------------------------

        // check if the first letter is a weak letter
        private static string firstWeak(string word)
        {
            staticFiles = new List<List<string>>();
            if (addVectorFromFile(pathToStemmerFiles + "definite_article.txt"))
                if (addVectorFromFile(pathToStemmerFiles + "duplicate.txt"))
                    if (addVectorFromFile(pathToStemmerFiles + "first_waw.txt"))
                        if (addVectorFromFile(pathToStemmerFiles + "first_yah.txt"))
                            if (addVectorFromFile(pathToStemmerFiles + "last_alif.txt"))
                                if (addVectorFromFile(pathToStemmerFiles + "last_hamza.txt"))
                                    if (addVectorFromFile(pathToStemmerFiles + "last_maksoura.txt"))
                                        if (addVectorFromFile(pathToStemmerFiles + "last_yah.txt"))
                                            if (addVectorFromFile(pathToStemmerFiles + "mid_waw.txt"))
                                                if (addVectorFromFile(pathToStemmerFiles + "mid_yah.txt"))
                                                    if (addVectorFromFile(pathToStemmerFiles + "prefixes.txt"))
                                                        if (addVectorFromFile(pathToStemmerFiles + "punctuation.txt"))
                                                            if (addVectorFromFile(pathToStemmerFiles + "quad_roots.txt"))
                                                                if (addVectorFromFile(pathToStemmerFiles + "stopwords.txt"))
                                                                    if (addVectorFromFile(pathToStemmerFiles + "suffixes.txt"))
                                                                        if (addVectorFromFile(pathToStemmerFiles + "tri_patt.txt"))
                                                                            if (addVectorFromFile(pathToStemmerFiles + "tri_roots.txt"))
                                                                                if (addVectorFromFile(pathToStemmerFiles + "diacritics.txt"))
                                                                                    if (addVectorFromFile(pathToStemmerFiles + "strange.txt"))
                                                                                    {
                                                                                        // the vector was successfully created
                                                                                        //System.out.println( "read in files successfully" );
                                                                                    }

            StringBuilder stemmedWord = new StringBuilder("");
            // check if the firs letter was a waw
            if (staticFiles[2].Contains(word))
            {
                stemmedWord.Append("\u0648");
                stemmedWord.Append(word);
                word = stemmedWord.ToString();
                stemmedWord.Clear();

                // root was found, so set variable
                rootFound = true;

                stemmedDocument[wordNumber][1] = word;
                stemmedDocument[wordNumber][2] = "ROOT";
                stemmedWordsNumber++;
                listStemmedWords.Add(stemmedDocument[wordNumber][0]);
                listRootsFound.Add(word);

                return word;
            }
            else if (staticFiles[3].Contains(word)) // check if the first letter was a yah
            {
                stemmedWord.Append("\u064a");
                stemmedWord.Append(word);
                word = stemmedWord.ToString();
                stemmedWord.Clear();

                // root was found, so set variable
                rootFound = true;

                stemmedDocument[wordNumber][1] = word;
                stemmedDocument[wordNumber][2] = "ROOT";
                stemmedWordsNumber++;
                listStemmedWords.Add(stemmedDocument[wordNumber][0]);
                listRootsFound.Add(word);

                return word;
            }
            return word;
        }

        //--------------------------------------------------------------------------

        // check if the middle letter of the root is weak
        private static string middleWeak(string word)
        {
            staticFiles = new List<List<string>>();
            if (addVectorFromFile(pathToStemmerFiles + "definite_article.txt"))
                if (addVectorFromFile(pathToStemmerFiles + "duplicate.txt"))
                    if (addVectorFromFile(pathToStemmerFiles + "first_waw.txt"))
                        if (addVectorFromFile(pathToStemmerFiles + "first_yah.txt"))
                            if (addVectorFromFile(pathToStemmerFiles + "last_alif.txt"))
                                if (addVectorFromFile(pathToStemmerFiles + "last_hamza.txt"))
                                    if (addVectorFromFile(pathToStemmerFiles + "last_maksoura.txt"))
                                        if (addVectorFromFile(pathToStemmerFiles + "last_yah.txt"))
                                            if (addVectorFromFile(pathToStemmerFiles + "mid_waw.txt"))
                                                if (addVectorFromFile(pathToStemmerFiles + "mid_yah.txt"))
                                                    if (addVectorFromFile(pathToStemmerFiles + "prefixes.txt"))
                                                        if (addVectorFromFile(pathToStemmerFiles + "punctuation.txt"))
                                                            if (addVectorFromFile(pathToStemmerFiles + "quad_roots.txt"))
                                                                if (addVectorFromFile(pathToStemmerFiles + "stopwords.txt"))
                                                                    if (addVectorFromFile(pathToStemmerFiles + "suffixes.txt"))
                                                                        if (addVectorFromFile(pathToStemmerFiles + "tri_patt.txt"))
                                                                            if (addVectorFromFile(pathToStemmerFiles + "tri_roots.txt"))
                                                                                if (addVectorFromFile(pathToStemmerFiles + "diacritics.txt"))
                                                                                    if (addVectorFromFile(pathToStemmerFiles + "strange.txt"))
                                                                                    {
                                                                                        // the vector was successfully created
                                                                                        //System.out.println( "read in files successfully" );
                                                                                    }

            StringBuilder stemmedWord = new StringBuilder("j");
            // check if the middle letter is a waw
            if (staticFiles[8].Contains(word))
            {
                // return the waw to the word
                stemmedWord.Insert(0, word[0]);
                stemmedWord.Append("\u0648");
                stemmedWord.Append(word.Substring(1));
                word = stemmedWord.ToString();
                stemmedWord.Clear();

                // root was found, so set variable
                rootFound = true;

                stemmedDocument[wordNumber][1] = word;
                stemmedDocument[wordNumber][2] = "ROOT";
                stemmedWordsNumber++;
                listStemmedWords.Add(stemmedDocument[wordNumber][0]);
                listRootsFound.Add(word);

                return word;
            }
            // check if the middle letter is a yah
            else if (staticFiles[9].Contains(word))
            {
                // return the waw to the word
                stemmedWord.Insert(0, word[0]);
                stemmedWord.Append("\u064a");
                stemmedWord.Append(word.Substring(1));
                word = stemmedWord.ToString();
                stemmedWord.Clear();

                // root was found, so set variable
                rootFound = true;

                stemmedDocument[wordNumber][1] = word;
                stemmedDocument[wordNumber][2] = "ROOT";
                stemmedWordsNumber++;
                listStemmedWords.Add(stemmedDocument[wordNumber][0]);
                listRootsFound.Add(word);

                return word;
            }
            return word;
        }

        public string[][] returnPossibleRoots()
        {
            return possibleRoots;
        }

        // stem the word
        public string stemWord(string word)
        {
            initComponents();
            staticFiles = new List<List<string>>();
            if (addVectorFromFile(pathToStemmerFiles + "definite_article.txt"))
                if (addVectorFromFile(pathToStemmerFiles + "duplicate.txt"))
                    if (addVectorFromFile(pathToStemmerFiles + "first_waw.txt"))
                        if (addVectorFromFile(pathToStemmerFiles + "first_yah.txt"))
                            if (addVectorFromFile(pathToStemmerFiles + "last_alif.txt"))
                                if (addVectorFromFile(pathToStemmerFiles + "last_hamza.txt"))
                                    if (addVectorFromFile(pathToStemmerFiles + "last_maksoura.txt"))
                                        if (addVectorFromFile(pathToStemmerFiles + "last_yah.txt"))
                                            if (addVectorFromFile(pathToStemmerFiles + "mid_waw.txt"))
                                                if (addVectorFromFile(pathToStemmerFiles + "mid_yah.txt"))
                                                    if (addVectorFromFile(pathToStemmerFiles + "prefixes.txt"))
                                                        if (addVectorFromFile(pathToStemmerFiles + "punctuation.txt"))
                                                            if (addVectorFromFile(pathToStemmerFiles + "quad_roots.txt"))
                                                                if (addVectorFromFile(pathToStemmerFiles + "stopwords.txt"))
                                                                    if (addVectorFromFile(pathToStemmerFiles + "suffixes.txt"))
                                                                        if (addVectorFromFile(pathToStemmerFiles + "tri_patt.txt"))
                                                                            if (addVectorFromFile(pathToStemmerFiles + "tri_roots.txt"))
                                                                                if (addVectorFromFile(pathToStemmerFiles + "diacritics.txt"))
                                                                                    if (addVectorFromFile(pathToStemmerFiles + "strange.txt"))
                                                                                    {
                                                                                        // the vector was successfully created
                                                                                        //System.out.println( "read in files successfully" );
                                                                                    }

            // check if the word consists of two letters
            // and find it's root
            if (word.Length == 2)
                word = isTwoLetters(word);

            // if the word consists of three letters
            if (word.Length == 3 && !rootFound)
                // check if it's a root
                word = isThreeLetters(word);

            // if the word consists of four letters
            if (word.Length == 4)
                // check if it's a root
                isFourLetters(word);

            // if the root hasn't yet been found
            if (!rootFound)
            {
                // check if the word is a pattern
                word = checkPatterns(word);
            }

            // if the root still hasn't been found
            if (!rootFound)
            {
                // check for a definite article, and remove it
                word = checkDefiniteArticle(word);
            }

            // if the root still hasn't been found
            if (!rootFound && !stopwordFound)
            {
                // check for the prefix waw
                word = checkPrefixWaw(word);
            }

            // if the root STILL hasnt' been found
            if (!rootFound && !stopwordFound)
            {
                // check for suffixes
                word = checkForSuffixes(word);
            }

            // if the root STILL hasn't been found
            if (!rootFound && !stopwordFound)
            {
                // check for prefixes
                word = checkForPrefixes(word);
            }
            return word;
        }

        // check and remove the definite article
        private static string checkDefiniteArticle(string word)
        {
            if (staticFiles == null || staticFiles.Count == 0)
                throw new InvalidOperationException("Arabic stemmer Initialization did not done properly");

            // looking through the vector of definite articles
            // search through each definite article, and try and
            // find a match
            string definiteArticle = "";
            string modifiedWord = "";
            var definiteArticles = staticFiles[0];

            // for every definite article in the list
            for (int i = 0; i < definiteArticles.Count; i++)
            {
                definiteArticle = definiteArticles[i];
                // if the definite article was found
                if (word.StartsWith(definiteArticle))
                //if (definiteArticle.regionMatches(0, word, 0, definiteArticle.length()))
                {
                    // remove the definite article
                    modifiedWord = word.Substring(definiteArticle.Length);

                    // check to see if the word is a stopword
                    if (checkStopwords(modifiedWord))
                        return modifiedWord;

                    // check to see if the word is a root of three or four letters
                    // if the word has only two letters, test to see if one was removed
                    if (modifiedWord.Length == 2)
                        modifiedWord = isTwoLetters(modifiedWord);
                    else if (modifiedWord.Length == 3 && !rootFound)
                        modifiedWord = isThreeLetters(modifiedWord);
                    else if (modifiedWord.Length == 4)
                        isFourLetters(modifiedWord);

                    // if the root hasn't been found, check for patterns
                    if (!rootFound && modifiedWord.Length > 2)
                        modifiedWord = checkPatterns(modifiedWord);

                    // if the root STILL hasnt' been found
                    if (!rootFound && !stopwordFound)
                    {
                        // check for suffixes
                        modifiedWord = checkForSuffixes(modifiedWord);
                    }

                    // if the root STILL hasn't been found
                    if (!rootFound && !stopwordFound)
                    {
                        // check for prefixes
                        modifiedWord = checkForPrefixes(modifiedWord);
                    }


                    if (stopwordFound)
                        return modifiedWord;


                    // if the root was found, return the modified word
                    if (rootFound && !stopwordFound)
                    {
                        return modifiedWord;
                    }
                }
            }
            if (modifiedWord.Length > 3)
                return modifiedWord;
            return word;
        }

        // check and remove the special prefix (waw)
        private static string checkPrefixWaw(string word)
        {
            if (staticFiles == null || staticFiles.Count == 0)
                throw new InvalidOperationException("Arabic stemmer Initialization did not done properly");

            string modifiedWord = "";

            if (word.Length > 3 && word[0] == '\u0648')
            {
                modifiedWord = word.Substring(1);

                // check to see if the word is a stopword
                if (checkStopwords(modifiedWord))
                    return modifiedWord;

                // check to see if the word is a root of three or four letters
                // if the word has only two letters, test to see if one was removed
                if (modifiedWord.Length == 2)
                    modifiedWord = isTwoLetters(modifiedWord);
                else if (modifiedWord.Length == 3 && !rootFound)
                    modifiedWord = isThreeLetters(modifiedWord);
                else if (modifiedWord.Length == 4)
                    isFourLetters(modifiedWord);

                // if the root hasn't been found, check for patterns
                if (!rootFound && modifiedWord.Length > 2)
                    modifiedWord = checkPatterns(modifiedWord);

                // if the root STILL hasnt' been found
                if (!rootFound && !stopwordFound)
                {
                    // check for suffixes
                    modifiedWord = checkForSuffixes(modifiedWord);
                }

                // iIf the root STILL hasn't been found
                if (!rootFound && !stopwordFound)
                {
                    // check for prefixes
                    modifiedWord = checkForPrefixes(modifiedWord);
                }

                if (stopwordFound)
                    return modifiedWord;

                if (rootFound && !stopwordFound)
                {
                    return modifiedWord;
                }
            }
            return word;
        }
    }
}
