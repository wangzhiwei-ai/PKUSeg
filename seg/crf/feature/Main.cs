using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Collections;


namespace Program1
{
    namespace Feature
    {
        class Program2
        {

            //static void runProcess(string trainFile, string testFile)
            //{
            //    //Global.log = new StreamWriter("data/temp/log.txt");

            //    process(trainFile, testFile);

            //    //Global.log.Close();

            //}

            public static void processTest(string testFile)
            {
                Global.trainLexiconSet = new baseHashSet<string>();
                Global.trainBigramSet = new baseHashSet<string>();
                readUnigram(Program1.Global.modelDir + "/unigram_word.txt");
                readBigramFeature(Program1.Global.modelDir + "/bigram_word.txt");
                Feature.readFeature(Program1.Global.modelDir + "/featureSet.txt");
                convertTest(testFile, "data/temp/c.test.txt", "data/temp/test.raw.txt");
                Feature.processFile("data/temp/c.test.txt", "data/temp/test.txt");

            }
            public static void processTrain(string trainFile)
            {
                Global.trainLexiconSet = new baseHashSet<string>();
                Global.trainBigramSet = new baseHashSet<string>();
                //baseHashMap<string, int> featureIndexMap = new baseHashMap<string, int>();
                //baseHashMap<string, int> tagIndexMap = new baseHashMap<string, int>();

                convertTrain(trainFile, "data/temp/c.train.txt", true);
                saveBigramFeature(Program1.Global.modelDir + "/bigram_word.txt");
                saveUnigram(Program1.Global.modelDir + "/unigram_word.txt");

                Feature.getFeatureSet("data/temp/c.train.txt");
                Feature.saveFeature(Program1.Global.modelDir + "/featureSet.txt");
                //now process for the test-inputfile, note that test-infile should share the same featureMap and labelMap
                Feature.processFile("data/temp/c.train.txt", "data/temp/train.txt");
            }


            public static string keywordTransfer(string words)
            {
                if (words == "-" || words == "." || words == "_" || words == "," || words == "|" || words == "/" || words == "*" || words == ":")
                {
                    return "&";
                }
                else
                    return words;
            }

            static void convertTest(string testFile, string outFile, string rawText)
            {
                string B = "B";
                string I_first = "I_first";
                string I = "I";
                string I_end = "I_end";
                string B_single = "B_single";

                StreamReader sr2 = new StreamReader(testFile, Encoding.UTF8);
                StreamWriter sw2 = new StreamWriter(outFile, false, Encoding.UTF8);
                StreamWriter sw3 = new StreamWriter(rawText, false, Encoding.UTF8);
                string txt = sr2.ReadToEnd();
                txt = txt.Replace("\r", "");
                txt = txt.Replace("\t", " ");

                string[] ary = txt.Split(Global.lineEndAry, StringSplitOptions.RemoveEmptyEntries);
                foreach (string im in ary) //for each sentence
                {
                    string[] imAry = im.Split(Global.blankAry, StringSplitOptions.RemoveEmptyEntries);
                    foreach (string w in imAry) //for each word
                    {
                        char[] cAry = w.Trim().ToCharArray();

                        int position = 0;
                        foreach (char c in cAry) //for each char
                        {
                            if ((c + " ").Trim() == "")
                            {
                                continue;
                            }
                            string sc = keywordTransfer(c + "");
                            sw3.Write(sc);
                            if (w.Length == 1)
                            {
                                string tri = sc.ToString() + " " + B_single;
                                sw2.WriteLine(tri);
                            }
                            else if (position == 0)
                            {
                                string tri = sc.ToString() + " " + B;
                                sw2.WriteLine(tri);
                            }
                            else if (position == w.Length - 1)
                            {
                                string tri = sc.ToString() + " " + I_end;
                                sw2.WriteLine(tri);
                            }
                            else if (position == 1)
                            {
                                string tri = sc.ToString() + " " + I_first;
                                sw2.WriteLine(tri);
                            }
                            /*
                       else if (position == 2)
                       {
                           string tri = c.ToString() +" "+ "B2";
                           sw1.WriteLine(tri);
                       }
                        * */
                            else
                            {
                                string tri = sc.ToString() + " " + I;
                                sw2.WriteLine(tri);
                            }

                            position++;
                        }
                    }
                    sw2.WriteLine();
                    sw3.WriteLine();

                }
                sr2.Close();
                sw2.Close();
                sw3.Close();

            }

            static void saveUnigram(string file)
            {
                StreamWriter sw = new StreamWriter(file);
                foreach (string word in Global.trainLexiconSet)
                {
                    sw.WriteLine(word);
                }
                sw.Close();
            }


            static void readUnigram(string file)
            {
                StreamReader sr = new StreamReader(file);
                string line = "";
                while ((line = sr.ReadLine()) != null)
                {
                    Global.trainLexiconSet.Add(line.Trim());
                }
                sr.Close();
            }

            static void saveBigramFeature(string file)
            {
                StreamWriter sw = new StreamWriter(file);
                foreach (string word in Global.trainBigramSet)
                {
                    sw.WriteLine(word);
                }
                sw.Close();
            }

            static void readBigramFeature(string file)
            {
                StreamReader sr = new StreamReader(file);
                string line = "";
                while ((line = sr.ReadLine()) != null)
                {
                    Global.trainBigramSet.Add(line.Trim());
                }
                sr.Close();
            }

            public static void tocrfoutput(string readPath, string wirtePath, string rawPath)
            {
                StreamReader tagFile = new StreamReader(Program1.Global.modelDir + "/tagIndex.txt");
                Dictionary<int, string> tags = new Dictionary<int, string>();

                string line2;
                while ((line2 = tagFile.ReadLine()) != null)
                {
                    string[] wordTags = line2.Split(' ');
                    tags.Add(int.Parse(wordTags[1]), wordTags[0]);
                }

                tagFile.Close();

                StreamReader outputTag = new StreamReader(readPath, Encoding.UTF8);
                StreamWriter sw = new StreamWriter(wirtePath);
                StreamReader rawText = new StreamReader(rawPath);
                string line;
                string[] linetag;


                while ((line = outputTag.ReadLine()) != null)
                {
                    string write_string = "";
                    string raw = rawText.ReadLine();
                    linetag = line.Split(',');

                    for (int i = 0; i < raw.Length; i++)
                    {
                        if (tags[int.Parse(linetag[i])].Contains("B"))
                        {
                            write_string += " " + raw[i];

                        }
                        else
                        {
                            write_string += raw[i] + "";
                        }
                    }
                    sw.WriteLine(write_string);
                }
                sw.Close();
                outputTag.Close();
                rawText.Close();

            }

            static void convertTrain(string trainFile, string outFile, bool collectInfo)
            {
                string file1 = trainFile;
                StreamReader sr1 = new StreamReader(file1, Encoding.UTF8);
                StreamWriter sw1 = new StreamWriter(outFile,false, Encoding.UTF8);
                string txt = sr1.ReadToEnd();
                txt = txt.Replace("\r", "");
                txt = txt.Replace("\t", " ");
                //normalize num and letter, should be consistent with the normalization during feature generation.
                char[] numAry = Global.num.ToCharArray();

                string B, I, I_end, B_single, I_first;
                if (Global.nLabel == 2)
                {
                    B = B_single = "B";
                    I_first = I = I_end = "I";
                }
                else if (Global.nLabel == 3)
                {
                    B = B_single = "B";
                    I = I_first = "I";
                    I_end = "I_end";
                }
                else if (Global.nLabel == 4)
                {
                    B = "B";
                    I = I_first = "I";
                    I_end = "I_end";
                    B_single = "B_single";
                }
                else if (Global.nLabel == 5)
                {
                    B = "B";
                    I_first = "I_first";
                    I = "I";
                    I_end = "I_end";
                    B_single = "B_single";
                }
                else
                    throw new Exception();

                int[] lengthCountAry = new int[20];
                string[] ary = txt.Split(Global.lineEndAry, StringSplitOptions.RemoveEmptyEntries);
                foreach (string im in ary) //for each sentence
                {
                    string[] imAry = im.Split(Global.blankAry, StringSplitOptions.RemoveEmptyEntries);
                    foreach (string w in imAry) //for each word
                    {
                        if (collectInfo)
                        {
                            //collect the lexicon from training data
                            Global.trainLexiconSet.Add(w);
                            if (w.Length <= 15)
                            {
                                lengthCountAry[w.Length]++;
                            }
                        }

                        char[] cAry = w.ToCharArray();
                        int position = 0;
                        foreach (char c in cAry) //for each char
                        {
                            if ((c + " ").Trim() == "")
                            {
                                continue;
                            }
                            string sc = keywordTransfer(c + "");
                            if (w.Length == 1)
                            {
                                string tri = sc.ToString() + " " + B_single;
                                sw1.WriteLine(tri);
                            }
                            else if (position == 0)
                            {
                                string tri = sc.ToString() + " " + B;
                                sw1.WriteLine(tri);
                            }
                            else if (position == w.Length - 1)
                            {
                                string tri = sc.ToString() + " " + I_end;
                                sw1.WriteLine(tri);
                            }
                            else if (position == 1)
                            {
                                string tri = sc.ToString() + " " + I_first;
                                sw1.WriteLine(tri);
                            }
                            /*
                       else if (position == 2)
                       {
                           string tri = c.ToString() +" "+ "B2";
                           sw1.WriteLine(tri);
                       }
                        * */
                            else
                            {
                                string tri = sc.ToString() + " " + I;
                                sw1.WriteLine(tri);
                            }

                            position++;
                        }
                    }
                    sw1.WriteLine();

                    if (collectInfo)
                    {
                        //collect bigrams
                        for (int i = 1; i < imAry.Length; i++)
                        {
                            string wd1 = imAry[i - 1];
                            string wd2 = imAry[i];
                            string bigram = wd1 + "*" + wd2;
                            Global.trainBigramSet.Add(bigram);
                        }
                    }

                }
                sr1.Close();
                sw1.Close();

                if (collectInfo)
                {
                    for (int i = 1; i < 16; i++)
                    {
                        Console.WriteLine("length = " + i.ToString() + " :" + lengthCountAry[i]);
                    }
                }


            }


        }
    }
}

