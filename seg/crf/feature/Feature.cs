using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Collections;


namespace Program1
{
    namespace Feature
    {
        class Feature
        {
            static baseHashSet<string> featureSet = new baseHashSet<string>();

            public static void saveFeature(string file)
            {
                StreamWriter sw = new StreamWriter(file);
                foreach (string word in featureSet)
                {
                    sw.WriteLine(word);
                }
                sw.Close();
            }


            public static void readFeature(string file)
            {
                StreamReader sr = new StreamReader(file);
                string line = "";
                while ((line = sr.ReadLine()) != null)
                {
                    featureSet.Add(line.Trim());
                }
                sr.Close();
            }

            public static void getFeatureSet(string fname)
            {
                featureSet.Clear();
                //string file = "c." + fname + ".train.txt";
                Console.Error.WriteLine("getting feature set...");

                List<string> wordsList = new List<string>();
                List<string> tagsList = new List<string>();
                normalize(fname, wordsList, tagsList);

                //deal with featureMap and tagMap for train-input. No need for test-input
                baseHashMap<string, int> featureFreqMap = new baseHashMap<string, int>();
                for (int i = 0; i < wordsList.Count; i++)
                {
                    string words = wordsList[i];
                    string[] wordAry = words.Split(Global.blankAry);

                    for (int k = 0; k < wordAry.Length; k++)
                    {
                        string word = wordAry[k];
                        List<string> nodeFeatures = new List<string>();
                        getNodeFeatures(k, wordAry, ref nodeFeatures);

                        foreach (string f in nodeFeatures)
                        {
                            if (f == "/")
                                continue;
                            string[] fAry = f.Split(Global.slashAry);
                            string id = fAry[0];
                            featureFreqMap[id]++;
                        }
                    }
                }

                //build featureSet
                foreach (baseHashMap<string, int>.KeyValuePair kv in featureFreqMap)
                {
                    if (kv.Value > Global.featureTrim)
                        featureSet.Add(kv.Key);
                }
            }


            static void normalize(string file, List<string> wordsList, List<string> tagsList)
            {
                StreamReader sr1 = new StreamReader(file);
                string a = sr1.ReadToEnd();
                sr1.Close();
                a = a.Replace("\t", " ");
                a = a.Replace("\r", "");
                a = a.Replace("/", "$");//this is important, "/" is a special char in format

                string[] ary = a.Split(Global.biLineEndAry, StringSplitOptions.RemoveEmptyEntries);
                foreach (string im in ary)
                {
                    List<string> wordList = new List<string>();
                    List<string> tagList = new List<string>();
                    string[] imAry = im.Split(Global.lineEndAry, StringSplitOptions.RemoveEmptyEntries);

                    //now deal with the compound
                    string preTag = Global.B;
                    foreach (string imm in imAry)
                    {
                        string[] biAry = imm.Split(Global.blankAry);
                        //for sentence, normalize 0.83 to xu_num
                        if (Global.numLetterNorm)
                        {
                            string tmp1 = biAry[0];
                            string[] tmpAry = tmp1.Split(Global.num.ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                            if (tmpAry.Length == 0)
                                biAry[0] = "**Num";//number
                            tmpAry = tmp1.Split(Global.letter.ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                            if (tmpAry.Length == 0)
                                biAry[0] = "**Letter";//letter
                        }

                        //markov order
                        string tmp2 = biAry[1];
                        if (Global.order == 2)
                        {
                            biAry[1] = preTag + Global.mark + tmp2;
                        }
                        preTag = tmp2;

                        wordList.Add(biAry[0]);
                        tagList.Add(biAry[1]);
                    }
                    string words = string.Join(" ", wordList.ToArray());
                    string tags = string.Join(" ", tagList.ToArray());

                    wordsList.Add(words);
                    tagsList.Add(tags);
                }
            }


            public static void processFile(string file, string file1)
            {
                Console.Error.WriteLine("running...");
                List<string> wordSeqList = new List<string>();
                List<string> tagSeqList = new List<string>();
                normalize(file, wordSeqList, tagSeqList);
                Console.WriteLine("{0} size: {1}", file, wordSeqList.Count);

                Feature.writeFeaturesTag(wordSeqList, tagSeqList, file1);
            }


            public static void writeFeaturesTag(List<string> wordSeqList, List<string> tagSeqList, string file)
            {
                StreamWriter swFeatureFile = new StreamWriter(file);

                //count length dist
                baseHashMap<int, int> lengthCountMap = new baseHashMap<int, int>();

                int interval = wordSeqList.Count / 10;
                for (int i = 0; i < wordSeqList.Count; i++)
                {
                    if (i % interval == 0)
                    {
                        double percent = (double)i / (double)wordSeqList.Count * 100.0;
                        Console.WriteLine("{0}: sentence #{1} --> {2}%", file, i, percent.ToString("f2"));
                    }

                    string wordSeq = wordSeqList[i];
                    string[] wordAry = wordSeq.Split(Global.blankAry);
                    string tagSeq = tagSeqList[i];
                    string[] tagAry = tagSeq.Split(Global.blankAry);

                    int length = wordAry.Length;
                    lengthCountMap[length]++;

                    for (int k = 0; k < wordAry.Length; k++)
                    {
                        List<string> nodeFeatures = new List<string>();
                        getNodeFeatures(k, wordAry, ref nodeFeatures);

                        swFeatureFile.Write(wordAry[k] + " ");//word
                        foreach (string f in nodeFeatures)//features
                        {
                            if (f == "/")
                                swFeatureFile.Write("/ ");
                            else
                            {
                                string[] fAry = f.Split(Global.slashAry);
                                string id = fAry[0];
                                if (featureSet.Contains(id))
                                    swFeatureFile.Write(f + " ");
                                else
                                    swFeatureFile.Write("/ ");
                            }
                        }
                        swFeatureFile.Write(tagAry[k]);//tag
                        swFeatureFile.WriteLine();
                    }
                    swFeatureFile.WriteLine();
                }
                swFeatureFile.Close();

                //output length dist
                List<string> sortList2 = new List<string>();
                foreach (baseHashMap<int, int>.KeyValuePair kv in lengthCountMap)
                {
                    double v = (double)kv.Value / (double)wordSeqList.Count * 100.0;
                    sortList2.Add(string.Format("{0}  count:{1} --> {2}%", kv.Key, kv.Value, v.ToString("f2")));
                }
                sortList2.Sort(ListSortFunc.compareKV_key);
                Console.WriteLine("length distribution:");
                foreach (string im in sortList2)
                    Console.WriteLine(im);
            }


            public static string getCharSeq(string[] sentence, int i, int length)
            {
                if (i < 0 || i > sentence.Length - 1)
                    return "";
                if (i + length - 1 > sentence.Length - 1)
                    return "";

                string seq = "";
                for (int k = 0; k < length; k++)
                {
                    int pos = i + k;
                    if (pos > sentence.Length - 1)
                        break;
                    seq += sentence[pos];
                }
                return seq;
            }


            public static void getNodeFeatures(int n, string[] wordAry, ref List<string> fList)
            {
                string w = wordAry[n];
                string feature;

                //to deal with empty feature vector
                feature = "$$";
                fList.Add(feature);

                //w_i
                feature = "c." + w;
                fList.Add(feature);

                //w_i-1
                if (n >= 1)
                {
                    feature = "c-1." + wordAry[n - 1];
                    fList.Add(feature);
                }
                else
                    fList.Add("/");

                //w_i+1
                if (n <= wordAry.Length - 2)
                {
                    feature = "c1." + wordAry[n + 1];
                    fList.Add(feature);
                }
                else
                    fList.Add("/");

                //w_i-2
                if (n >= 2)
                {
                    feature = "c-2." + wordAry[n - 2];
                    fList.Add(feature);
                }
                else
                    fList.Add("/");

                //w_i+2
                if (n <= wordAry.Length - 3)
                {
                    feature = "c2." + wordAry[n + 2];
                    fList.Add(feature);
                }
                else
                    fList.Add("/");

                //w_i-1, w_i
                if (n >= 1)
                {
                    feature = "c-1c." + wordAry[n - 1] + Global.delimInFeature + w;
                    fList.Add(feature);
                }
                else
                    fList.Add("/");

                //w_i+1, w_i
                if (n <= wordAry.Length - 2)
                {
                    feature = "cc1." + w + Global.delimInFeature + wordAry[n + 1];
                    fList.Add(feature);
                }
                else
                    fList.Add("/");

                //w_i-2, w_i-1
                if (n >= 2)
                {
                    feature = "c-2c-1." + wordAry[n - 2] + Global.delimInFeature + wordAry[n - 1];
                    fList.Add(feature);
                }
                else
                    fList.Add("/");

                //num/letter based features
                if (false)
                {
                    //w_i
                    if (Global.chnNum.Contains(w))
                    {
                        feature = "c." + "chnNum";
                        fList.Add(feature);
                    }
                    else if (Global.engNum.Contains(w))
                    {
                        feature = "c." + "engNum";
                        fList.Add(feature);
                    }
                    else if (Global.letter.Contains(w))
                    {
                        feature = "c." + "letter";
                        fList.Add(feature);
                    }
                    else
                        fList.Add("/");
                }


                //word features and word-bigram features
                if (Global.wordFeature)
                {
                    List<string> preList_in = new List<string>();
                    for (int length = Global.wordMax; length >= Global.wordMin; length--)
                    {
                        string tmp = getCharSeq(wordAry, n - length + 1, length);
                        if (tmp != "")
                        {
                            if (Global.trainLexiconSet.Contains(tmp))
                            {
                                feature = "w-1." + tmp;
                                fList.Add(feature);
                                preList_in.Add(tmp);
                            }
                            else
                            {
                                fList.Add("/");
                                preList_in.Add("**noWord");
                            }
                        }
                        else
                        {
                            fList.Add("/");
                            preList_in.Add("**noWord");
                        }
                    }

                    List<string> postList_in = new List<string>();
                    for (int length = Global.wordMax; length >= Global.wordMin; length--)
                    {
                        string tmp = getCharSeq(wordAry, n, length);
                        if (tmp != "")
                        {
                            if (Global.trainLexiconSet.Contains(tmp))
                            {
                                feature = "w1." + tmp;
                                fList.Add(feature);
                                postList_in.Add(tmp);
                            }
                            else
                            {
                                fList.Add("/");
                                postList_in.Add("**noWord");
                            }
                        }
                        else
                        {
                            fList.Add("/");
                            postList_in.Add("**noWord");
                        }
                    }

                    List<string> preList_ex = new List<string>();
                    for (int length = Global.wordMax; length >= Global.wordMin; length--)
                    {
                        //tmp will not include the current char on pos.
                        string tmp = getCharSeq(wordAry, n - length, length);
                        if (tmp != "")
                        {
                            if (Global.trainLexiconSet.Contains(tmp))
                                preList_ex.Add(tmp);
                            else
                                preList_ex.Add("**noWord");
                        }
                        else
                            preList_ex.Add("**noWord");
                    }

                    List<string> postList_ex = new List<string>();
                    for (int length = Global.wordMax; length >= Global.wordMin; length--)
                    {
                        //tmp will not include the current char on pos.
                        string tmp = getCharSeq(wordAry, n + 1, length);
                        if (tmp != "")
                        {
                            if (Global.trainLexiconSet.Contains(tmp))
                                postList_ex.Add(tmp);
                            else
                                postList_ex.Add("**noWord");
                        }
                        else
                            postList_ex.Add("**noWord");
                    }


                    //word-bigram check
                    bool bigramHitCk;
                    foreach (string pre in preList_ex)
                    {
                        foreach (string post in postList_in)
                        {
                            string bigram = pre + "*" + post;
                            if (Global.trainBigramSet.Contains(bigram))//&&bigram.Length>3)
                            {
                                bigramHitCk = true;
                                feature = "ww.l." + bigram;
                                fList.Add(feature);
                                //break;
                            }
                            else
                                fList.Add("/");
                        }
                        //if (bigramHitCk)
                        //  break;
                    }

                    foreach (string pre in preList_in)
                    {
                        foreach (string post in postList_ex)
                        {
                            string bigram = pre + "*" + post;
                            if (Global.trainBigramSet.Contains(bigram))//&&bigram.Length>3)
                            {
                                bigramHitCk = true;
                                feature = "ww.r." + bigram;
                                fList.Add(feature);
                                //break;
                            }
                            else
                                fList.Add("/");
                        }
                    }
                }

                /*
                //nt,ns,nr features
                if (true)
                {
                    string w = sentence[pos];
                    string wm1, wm2, wp1;
                    wm1 = wm2 = wp1 = "$%^&*";
                    if (pos - 1 >= 0)
                        wm1 = sentence[pos - 1];
                    if (pos - 2 >= 0)
                        wm2 = sentence[pos - 2];
                    if (pos + 1 <= sentence.Length - 1)
                        wp1 = sentence[pos + 1];

                    string mode = "";
                    if (Pan.orgAppndx.Contains(wm2))
                    {
                        feature = "wm2.nt1." + wm2;
                        fList.Add(feature);
                    }
                    else if (Pan.nsLastWords.Contains(wm2))
                    {
                        feature = "wm2.ns." + wm2;
                        fList.Add(feature);

                    }
                    else if (Pan.ntLastWords.Contains(wm2))
                    {
                        feature = "wm2.nt2." + wm2;
                        fList.Add(feature);
                    }

                    if (Pan.orgAppndx.Contains(wm1))
                    {
                        feature = "wm1.nt1." + wm1;
                        fList.Add(feature);
                    }
                    else if (Pan.nsLastWords.Contains(wm1))
                    {
                        feature = "wm1.ns." + wm1;
                        fList.Add(feature);
                    }
                    else if (Pan.ntLastWords.Contains(wm1))
                    {
                        feature = "wm1.nt2." + wm1;
                        fList.Add(feature);
                    }

                    if (Pan.orgAppndx.Contains(w))
                    {
                        feature = "w.nt1." + w;
                        fList.Add(feature);
                    }
                    else if (Pan.nsLastWords.Contains(w))
                    {
                        feature = "w.ns." + w;
                        fList.Add(feature);
                    }
                    else if (Pan.ntLastWords.Contains(w))
                    {
                        feature = "w.nt2." + w;
                        fList.Add(feature);
                    }

                    if (Pan.orgAppndx.Contains(wp1))
                    {
                        feature = "wp1.nt1." + wp1;
                        fList.Add(feature);
                    }
                    else if (Pan.nsLastWords.Contains(wp1))
                    {
                        feature = "wp1.ns." + wp1;
                        fList.Add(feature);
                    }
                    else if (Pan.ntLastWords.Contains(wp1))
                    {
                        feature = "wp1.nt2." + wp1;
                        fList.Add(feature);
                    }
                }
                */

            }

        }
    }

}
