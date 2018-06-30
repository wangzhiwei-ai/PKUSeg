//
//  CRF-ADF Toolkit v1.0
//
//  Copyright(C) Xu Sun <xusun@pku.edu.cn> http://klcl.pku.edu.cn/member/sunxu/index.htm
//

using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Linq;
using System.Collections;
using System.IO.Compression;
using System.Threading.Tasks;
using System.Diagnostics;

namespace Program1
{
    class dataFormat
    {
        Dictionary<string, int> featureIndexMap = new Dictionary<string, int>();
        baseHashMap<string, int> tagIndexMap = new baseHashMap<string, int>();

        public void convert()
        {
            if (Global.runMode.Contains("train"))
            {
                getMaps(Global.fTrain);
                saveFeature(Global.modelDir + "/featureIndex.txt");
                convertFile(Global.fTrain);

            }
            else
            {
                //getMaps(Global.fTrain);
                Stopwatch timer = new Stopwatch();
                //timer.Start();
                readFeature(Global.modelDir + "/featureIndex.txt");
                //saveFeature(Global.modelDir + "/featureIndex.txt"); 
                readTag(Global.modelDir + "/tagIndex.txt");
                //timer.Stop();
                //Console.WriteLine("read featureIndex" + timer.Elapsed);
            }

            convertFile(Global.fTest);
            if (Global.dev)
                convertFile(Global.fDev);
        }



        public void saveFeature(string file)
        {
            List<string> featureList = featureIndexMap.Keys.ToList();
            int number = (int)(featureList.Count / 10);

            Parallel.For(0, 10, i =>
            {
                if (i == 9)
                {
                    StreamWriter sw = new StreamWriter(file + "_" + i.ToString());
                    foreach (string word in featureList.GetRange(i * number, featureList.Count - i * number))
                    {
                        sw.WriteLine(word+" "+featureIndexMap[word].ToString());
                    }
                    sw.Close();
                }
                else
                {
                    StreamWriter sw = new StreamWriter(file + "_" + i.ToString());
                    foreach (string word in featureList.GetRange(i * number, (i + 1) * number - i * number))
                    {
                        sw.WriteLine(word + " " + featureIndexMap[word].ToString());
                    }
                    sw.Close();
                }

            });

        }


        public void readFeature(string file)
        {
            List<string>[] featureList = new List<string>[10];
            Parallel.For(0, 10, i =>
            {
                featureList[i] = new List<string>();
                StreamReader sr = new StreamReader(file + "_" + i.ToString());
                string line = "";
                while ((line = sr.ReadLine()) != null)
                {
                    featureList[i].Add(line.Trim());
                }
                sr.Close();

            });
            List<string> feature = new List<string>();
            //Parallel.For(0, 10, i =>
            for (int i = 0; i < featureList.Length; i++)
            {
                for (int k = 0; k < featureList[i].Count; k++)
                {
                    string[] wordIndex = featureList[i][k].Split();
                    featureIndexMap.Add(wordIndex[0], int.Parse(wordIndex[1]));
                }

            }//);


        }


        public void readFeatureNormal(string path)
        {
            StreamReader sr = new StreamReader(path);
            string line = "";
            while ((line = sr.ReadLine()) != null)
            {
                string[] features = line.Split();
                featureIndexMap[features[0]] = int.Parse(features[1]);
            }

        }

        public void readTag(string path)
        {
            StreamReader sr = new StreamReader(path);
            string line = "";
            while ((line = sr.ReadLine()) != null)
            {
                string[] tags = line.Split();
                tagIndexMap[tags[0]] = int.Parse(tags[1]);
            }

        }

        public void getMaps(string file)
        {
            if (!File.Exists(file))
            {
                Console.WriteLine("file {0} no exist!", file);
                return;
            }
            Console.WriteLine("file {0} converting...", file);
            StreamReader sr = new StreamReader(file);

            baseHashMap<string, int> featureFreqMap = new baseHashMap<string, int>();
            baseHashSet<string> tagSet = new baseHashSet<string>();

            //get feature-freq info and tagset
            int nFeatTemp = 0;
            while (!sr.EndOfStream)
            {
                string line = sr.ReadLine();
                line = line.Replace("\t", " ");
                line = line.Replace("\r", "");

                if (line == "")
                    continue;

                string[] ary = line.Split(Global.blankAry, StringSplitOptions.RemoveEmptyEntries);
                nFeatTemp = ary.Length - 2;
                for (int i = 1; i < ary.Length - 1; i++)
                {

                    if (ary[i] == "/")//no feature here
                        continue;

                    if (Global.weightRegMode == "GL")
                    {
                        if (Global.GL_init == false && Global.groupTrim[i - 1])//this feature is removed in GL 1st step
                            continue;
                    }

                    string[] ary2 = ary[i].Split(Global.slashAry, StringSplitOptions.RemoveEmptyEntries);//for real-value features
                    string feature = i.ToString() + "." + ary2[0];
                    featureFreqMap[feature]++;
                }

                string tag = ary[ary.Length - 1];
                tagSet.Add(tag);
            }

            //sort features
            List<string> sortList = new List<string>();
            foreach (baseHashMap<string, int>.KeyValuePair kv in featureFreqMap)
                sortList.Add(kv.Key + " " + kv.Value);
            if (Global.weightRegMode == "GL")//sort based on feature templates
            {
                sortList.Sort(listSortFunc.compareKV_key);
                //sortList.Reverse();

                StreamWriter sw = new StreamWriter("featureTemp_sorted.txt");
                foreach (string f in sortList)
                    sw.WriteLine(f);
                sw.Close();

                Global.groupStart = new List<int>();
                Global.groupEnd = new List<int>();
                Global.groupStart.Add(0);
                for (int k = 1; k < sortList.Count; k++)
                {
                    string[] thisAry = sortList[k].Split(Global.dotAry, StringSplitOptions.RemoveEmptyEntries);
                    string[] preAry = sortList[k - 1].Split(Global.dotAry, StringSplitOptions.RemoveEmptyEntries);
                    string str = thisAry[0], preStr = preAry[0];
                    if (str != preStr)
                    {
                        Global.groupStart.Add(k);
                        Global.groupEnd.Add(k);
                    }
                }
                Global.groupEnd.Add(sortList.Count);
            }
            else//sort based on feature frequency
            {
                sortList.Sort(listSortFunc.compareKV_value);//sort feature based on freq, for 1)compress .txt file 2)better edge features
                sortList.Reverse();
            }

            if (Global.weightRegMode == "GL" && Global.GL_init)
            {
                if (nFeatTemp != Global.groupStart.Count)
                    throw new Exception("inconsistent # of features per line, check the feature file for consistency!");
            }

            //feature index should begin from 0
            StreamWriter swFeat = new StreamWriter(Global.modelDir + "/featureIndex.txt");
            for (int i = 0; i < sortList.Count; i++)
            {
                string[] ary = sortList[i].Split(Global.blankAry);
                featureIndexMap[ary[0]] = i;
                swFeat.WriteLine("{0} {1}", ary[0].Trim(), i);
            }
            swFeat.Close();

            //label index should begin from 0
            StreamWriter swTag = new StreamWriter(Global.modelDir + "/tagIndex.txt");
            List<string> tagSortList = new List<string>();
            foreach (string tag in tagSet)
                tagSortList.Add(tag);
            tagSortList.Sort();//sort tags
            for (int i = 0; i < tagSortList.Count; i++)
            {
                tagIndexMap[tagSortList[i]] = i;
                swTag.WriteLine("{0} {1}", tagSortList[i], i);
            }
            swTag.Close();

            sr.Close();
        }
        //public void getMaps(string file)
        //{


        //    StreamReader swFeat = new StreamReader(Global.modelDir + "/featureIndex.txt");
        //    string line = "";
        //    int i = 0;
        //    while ((line = swFeat.ReadLine()) != null)
        //    {
        //        string[] strs = line.Split(' ');

        //        featureIndexMap[strs[0]] = i;
        //        i += 1;
        //    }

        //    swFeat.Close();

        //    StreamReader swTag = new StreamReader(Global.modelDir + "/tagIndex.txt");
        //    line = "";
        //    i = 0;
        //    while ((line = swTag.ReadLine()) != null)
        //    {
        //        string[] strs = line.Split(' ');

        //        tagIndexMap[strs[0]] = i;
        //        i += 1;
        //    }

        //    swTag.Close();

        //}



        //for small memory load, should read line by line
        public void convertFile(string  file)
        {
            if (!File.Exists(file))
            {
                Console.WriteLine("file {0} no exist!", file);
                return;
            }
            //Console.WriteLine("file {0} converting...", file);
            Console.WriteLine("file converting...");
            StreamReader sr = new StreamReader(file);

            //convert to files of new format
            StreamWriter swFeature, swGold;
            if (file == Global.fTrain)
            {
                swFeature = new StreamWriter(Global.fFeatureTrain);
                swGold = new StreamWriter(Global.fGoldTrain);
            }
            else
            {
                swFeature = new StreamWriter(Global.fFeatureTest);
                swGold = new StreamWriter(Global.fGoldTest);
            }


            swFeature.WriteLine(featureIndexMap.Count);
            swFeature.WriteLine();
            swGold.WriteLine(tagIndexMap.Count);
            swGold.WriteLine();

            List<string> readLines = new List<string>();
           
            while(!sr.EndOfStream)
            {
                readLines.Add(sr.ReadLine());
            }
            sr.Close();
            string[] featureList = new string[readLines.Count];
            string[] goldList = new string[readLines.Count];
            Parallel.For(0, readLines.Count, k =>
            //for(int k = 0;k<readLines.Count;k++)
            {
                string line = readLines[k];
                line = line.Replace("\t", " ");
                line = line.Replace("\r", "\r");
                StringBuilder featureLine = new StringBuilder();
                StringBuilder goldLine = new StringBuilder();
                if (line == "")//end of a sample
                {
                    featureLine.Append("\r\n");
                    goldLine.Append("\r\n\r\n");
                    featureList[k]=(featureLine.ToString());
                    goldList[k]=(goldLine.ToString());
                    //swFeature.WriteLine();
                    //swGold.WriteLine();
                    //swGold.WriteLine();
                    return;
                }
                int flag = 0;
                string[] ary = line.Split(Global.blankAry, StringSplitOptions.RemoveEmptyEntries);
                for (int i = 1; i < ary.Length - 1; i++)
                {
                    if (ary[i] == "/")//no feature here
                        continue;
                    string[] ary2 = ary[i].Split(Global.slashAry, StringSplitOptions.RemoveEmptyEntries);//for real-value features
                    string feature = i.ToString() + "." + ary2[0];
                    string value = "";
                    bool real = false;
                    if (ary2.Length > 1)
                    {
                        value = ary2[1];
                        real = true;
                    }

                    if (featureIndexMap.ContainsKey(feature) == false)
                        continue;
                    flag = 1;
                    int fIndex = featureIndexMap[feature];
                    if (!real)
                        featureLine.Append(fIndex.ToString() + ",");
                    //swFeature.Write("{0},", fIndex);
                    else
                        featureLine.Append(fIndex.ToString() + "/" + value.ToString() + ","); //("{0}/{1},", fIndex, value);
                    //swFeature.Write("{0}/{1},", fIndex, value);
                }
                if (flag == 0)
                {
                    featureLine.Append("0");
                    //swFeature.Write("0");
                }
                //swFeature.WriteLine();
                featureLine.Append("\n");
                string tag = ary[ary.Length - 1];
                int tIndex = tagIndexMap[tag];
                goldLine.Append(tIndex.ToString() + ",");
                featureList[k] = (featureLine.ToString());
                goldList[k] = (goldLine.ToString());
                //swGold.Write("{0},", tIndex);
            });

            //sr.Close();
            for(int i = 0;i<featureList.Length;i++)
            {
                swFeature.Write(featureList[i]);
                swGold.Write(goldList[i]);
            }
            swFeature.Close();
            swGold.Close();
        }
    }


        class datasetList : List<dataSet>
        {
            protected int _nTag;
            protected int _nFeature;

            public datasetList(string fileFeature, string fileTag)
            {
                StreamReader srfileFeature = new StreamReader(fileFeature);
                StreamReader srfileTag = new StreamReader(fileTag);

                string txt = srfileFeature.ReadToEnd();
                txt = txt.Replace("\r", "");
                string[] fAry = txt.Split(Global.triLineEndAry, StringSplitOptions.RemoveEmptyEntries);

                txt = srfileTag.ReadToEnd();
                txt = txt.Replace("\r", "");
                string[] tAry = txt.Split(Global.triLineEndAry, StringSplitOptions.RemoveEmptyEntries);

                if (fAry.Length != tAry.Length)
                    throw new Exception("error");

                _nFeature = int.Parse(fAry[0]);
                _nTag = int.Parse(tAry[0]);

                for (int i = 1; i < fAry.Length; i++)
                {
                    string fBlock = fAry[i];
                    string tBlock = tAry[i];
                    dataSet ds = new dataSet();
                    string[] fbAry = fBlock.Split(Global.biLineEndAry, StringSplitOptions.RemoveEmptyEntries);
                    string[] lbAry = tBlock.Split(Global.biLineEndAry, StringSplitOptions.RemoveEmptyEntries);

                    for (int k = 0; k < fbAry.Length; k++)
                    {
                        string fm = fbAry[k];
                        string tm = lbAry[k];
                        dataSeq seq = new dataSeq();
                        seq.read(fm, tm);
                        ds.Add(seq);
                    }
                    Add(ds);
                }
                srfileFeature.Close();
                srfileTag.Close();
            }
        }

        class dataSet : List<dataSeq>
        {
            protected int _nTag;
            protected int _nFeature;

            public dataSet()
            {
            }

            public dataSet(int nTag, int nFeature)
            {
                _nTag = nTag;
                _nFeature = nFeature;
            }

            public dataSet(string fileFeature, string fileTag)
            {
                load(fileFeature, fileTag);
            }

            public dataSet randomShuffle()
            {
                List<int> ri = randomTool<int>.getShuffledIndexList(this.Count);
                dataSet X = new dataSet(this.NTag, this.NFeature);
                foreach (int i in ri)
                    X.Add(this[i]);
                return X;
            }

            virtual public int[,] EdgeFeature()
            {
                throw new Exception("error");
            }

            virtual public void load(string fileFeature, string fileTag)
            {
                StreamReader srfileFeature = new StreamReader(fileFeature, Encoding.GetEncoding("utf-8"));
                StreamReader srfileTag = new StreamReader(fileTag, Encoding.GetEncoding("utf-8"));

                string txt = srfileFeature.ReadToEnd();
                txt = txt.Replace("\r", "");
                string[] fAry = txt.Split(Global.biLineEndAry, StringSplitOptions.RemoveEmptyEntries);

                txt = srfileTag.ReadToEnd();
                txt = txt.Replace("\r", "");
                string[] tAry = txt.Split(Global.biLineEndAry, StringSplitOptions.RemoveEmptyEntries);

                if (fAry.Length != tAry.Length)
                    throw new Exception("error");

                _nFeature = int.Parse(fAry[0]);
                _nTag = int.Parse(tAry[0]);
                for (int i = 1; i < fAry.Length; i++)
                {
                    string features = fAry[i];
                    string tags = tAry[i];
                    dataSeq seq = new dataSeq();
                    seq.read(features, tags);
                    Add(seq);
                }
                srfileFeature.Close();
                srfileTag.Close();
            }

            public int NTag
            {
                get { return _nTag; }
                set { _nTag = value; }
            }

            public int NFeature
            {
                get { return _nFeature; }
                set { _nFeature = value; }
            }

            public void setDataInfo(dataSet X)
            {
                _nTag = X.NTag;
                _nFeature = X.NFeature;
            }

        }

        class dataSeqTest
        {
            public dataSeq _x;
            public List<int> _yOutput;

            public dataSeqTest(dataSeq x, List<int> yOutput)
            {
                _x = x;
                _yOutput = yOutput;
            }
        }

        class dataSeq
        {
            protected List<List<featureTemp>> featureTemps = new List<List<featureTemp>>();
            protected List<int> yGold = new List<int>();

            public dataSeq()
            {
            }

            public dataSeq(List<List<featureTemp>> feat, List<int> y)
            {
                featureTemps = new List<List<featureTemp>>(feat);
                for (int i = 0; i < feat.Count; i++)
                    featureTemps[i] = new List<featureTemp>(feat[i]);
                yGold = new List<int>(y);
            }

            public dataSeq(dataSeq x, int n, int length)
            {
                int end = 0;
                if (n + length < x.Count)
                    end = n + length;
                else
                    end = x.Count;
                for (int i = n; i < end; i++)
                {
                    featureTemps.Add(x.featureTemps[i]);
                    yGold.Add(x.yGold[i]);
                }
            }

            virtual public List<List<int>> getNodeFeature(int n)
            {
                throw new Exception("error");
            }

            virtual public void read(string a, int nState, string b)
            {
                throw new Exception("error");
            }

            public void read(string a, string b)
            {
                //features
                string[] lineAry = a.Split(Global.lineEndAry, StringSplitOptions.RemoveEmptyEntries);
                foreach (string im in lineAry)
                {
                    List<featureTemp> nodeList = new List<featureTemp>();
                    string[] imAry = im.Split(Global.commaAry, StringSplitOptions.RemoveEmptyEntries);
                    foreach (string imm in imAry)
                    {
                        if (imm.Contains("/"))
                        {
                            string[] biAry = imm.Split(Global.slashAry, StringSplitOptions.RemoveEmptyEntries);
                            featureTemp ft = new featureTemp(int.Parse(biAry[0]), double.Parse(biAry[1]));
                            nodeList.Add(ft);
                        }
                        else
                        {
                            featureTemp ft = new featureTemp(int.Parse(imm), 1);
                            nodeList.Add(ft);
                        }
                    }
                    featureTemps.Add(nodeList);
                }
                //yGold
                lineAry = b.Split(Global.commaAry, StringSplitOptions.RemoveEmptyEntries);
                foreach (string im in lineAry)
                {
                    yGold.Add(int.Parse(im));
                }
            }

            virtual public int Count
            {
                get { return featureTemps.Count; }
            }

            public List<List<featureTemp>> getFeatureTemp()
            {
                return featureTemps;
            }

            public List<featureTemp> getFeatureTemp(int node)
            {
                return featureTemps[node];
            }

            public int getTags(int node)
            {
                return yGold[node];
            }

            public List<int> getTags()
            {
                return yGold;
            }

            public void setTags(List<int> list)
            {
                if (list.Count != yGold.Count)
                    throw new Exception("error");
                for (int i = 0; i < list.Count; i++)
                    yGold[i] = list[i];
            }

        }
    

}