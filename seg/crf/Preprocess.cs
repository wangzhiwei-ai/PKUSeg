using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.IO;

namespace Program1
{
    class ProcessData
    {
        public static Dictionary<string, int> unigramword = new Dictionary<string, int>();
        public static Dictionary<string, int> bigramword = new Dictionary<string, int>();
        //替换关键字
        public static string keywordtransfer(string words)
        {
            if (words == "-" || words == "." || words == "_" || words == "," || words == "|" || words == "/" || words == "*" || words == ":")
            {
                return "&";
            }
            else
                return words;
        }
        //public static void collectwords(string path)
        //{
        //    StreamReader sr = new StreamReader(path);
        //    string line = "";
        //    while ((line = sr.ReadLine()) != null)
        //    {
        //        if (line.Trim() != "")
        //        {
        //            string[] words = line.Split(' ');
        //            for (int i = 0; i < words.Length; i++)
        //            {
        //                if (words[i].Trim() != " ")
        //                {
        //                    if (!unigramword.Keys.Contains(words[i].Trim()))
        //                    {
        //                        unigramword.Add(words[i].Trim(), 0);
        //                    }

        //                }
        //            }
        //        }

        //    }
        //    sr.Close();
        //}

        //public static void collectbigramwords(string path)
        //{
        //    StreamReader sr = new StreamReader(path);
        //    string line = "";
        //    while ((line = sr.ReadLine()) != null)
        //    {
        //        if (line.Trim() != "")
        //        {
        //            string[] words = Regex.Split(line, @"\s{1,}");

        //            for (int i = 0; i < words.Length-1; i++)
        //            {
        //                if (words[i].Trim() != " " && words[i+1].Trim() != " ")
        //                {
        //                    if (!bigramword.Keys.Contains(words[i].Trim()+"__"+words[i+1].Trim()) )
        //                    {
        //                        bigramword.Add(words[i].Trim() + "__" + words[i + 1].Trim(), 0);
        //                    }
        //                }
        //            }
        //        }

        //    }
        //    sr.Close();
        //}

        //抽特征
        public static void extracfeature(string filename, string writefile)
        {
            StreamReader sr = new StreamReader(filename);
            StreamWriter fw = new StreamWriter(writefile);


            string line = "";
            List<string> sentence = new List<string>();
            List<string> tag = new List<string>();
            while ((line = sr.ReadLine()) != null)
            {
                if (line.Trim() != "")
                {
                    string[] words = line.Split(' ');
                    sentence.Add(keywordtransfer(words[0]));
                    tag.Add(words[1]);
                }
                else
                {

                    for (int i = 0; i < sentence.Count; i++)
                    {
                        fw.Write((sentence[i] + ' '));
                        fw.Write(("c0." + sentence[i] + ' '));
                        

                        if (i >= 2)
                        {
                            fw.Write(("c-2.c-1." + sentence[i - 2] + '.' + sentence[i - 1] + ' '));
                            fw.Write(("c-2." + sentence[i - 2] + ' '));
                        }
                        else
                        {
                            fw.Write("/ ");
                            fw.Write("/ ");
                        }
                        if (i >= 1)
                        {
                            fw.Write("c-1.c0." + sentence[i - 1] + '.' + sentence[i] + ' ');
                            fw.Write("c-1." + sentence[i - 1] + ' ');
                        }
                        else
                        {
                            fw.Write("/ ");
                            fw.Write("/ ");
                        }
                        if (i <= sentence.Count - 2)
                        {
                            fw.Write("c0.c1." + sentence[i] + '.' + sentence[i + 1] + ' ');
                            fw.Write("c1." + sentence[i + 1] + ' ');
                        }
                        else
                        {
                            fw.Write("/ ");
                            fw.Write("/ ");
                        }
                        if (i <= sentence.Count - 3)
                        {
                            fw.Write("c1.c2." + sentence[i + 1] + '.' + sentence[i + 2] + ' ');
                            fw.Write("c2." + sentence[i + 2] + ' ');
                        }
                        else
                        {
                            fw.Write("/ ");
                            fw.Write("/ ");
                        }
                        if (i >= 1 && i <= sentence.Count - 2)
                        {
                            fw.Write("c-1.c1." + sentence[i - 1] + '.' + sentence[i + 1] + ' ');
                        }
                        else
                        {
                            fw.Write("/ ");
                        }
                        fw.Write(tag[i] + '\n');

                    }
                    fw.WriteLine();
                    sentence = new List<string>();
                    tag = new List<string>();
                }

            }


            fw.Close();

        }

        public static void tocrfinput_without_tag(string read1path, string wirtedatapath, string rawdatapath)
        {
            StreamReader sr = new StreamReader(read1path, Encoding.UTF8);
            StreamWriter sw = new StreamWriter(wirtedatapath);
            StreamWriter sw1 = new StreamWriter(rawdatapath);
            string line;
            string[] words;
            while ((line = sr.ReadLine()) != null)
            {
                string line1 = "";

                foreach (char word in line)
                {
                    string sword = word + "";
                    if (sword.Trim() == " " || sword.Trim() == "")
                    {
                        continue;

                    }
                    line1 += "" + word;
                    sw.WriteLine(word + " B_");

                }
                sw1.WriteLine(line1);
                sw.WriteLine();
                sw.Flush();

            }
            sw.Close();
            sw1.Close();


        }

        
        public static void tocrfinput_tag(string read1path, string wirtedatapath, string rawdatdapath)
        {
            StreamReader sr = new StreamReader(read1path, Encoding.UTF8);
            StreamWriter sw = new StreamWriter(wirtedatapath);
            StreamWriter sw1 = new StreamWriter(rawdatdapath);
            string line;
            string[] words;
            while ((line = sr.ReadLine()) != null)
            {
                string line1 = "";
                words = line.Split(' ');
                int index = 0;
                foreach (string sword in words)
                {
                    if (sword.Trim() == " " || sword.Trim() == "")
                    {
                        continue;

                    }
                    index = 0;
                    foreach (char c in sword.Trim())
                    {
                        if ((c + " ").Trim() == "")
                        {
                            continue;
                        }
                        line1 += c + "";

                        if (index == 0 && sword.Trim().Length == 1)
                        {
                            sw.WriteLine(c + " B_single");
                            index++;
                            continue;
                        }
                        else if (index == 0 && sword.Trim().Length != 1)
                        {
                            sw.WriteLine(c + " B_");
                            index++;
                            continue;
                        }
                        else if (index == sword.Trim().Length - 1 && sword.Trim().Length != 1)
                        {
                            sw.WriteLine(c + " I_end");
                            index++;
                            continue;
                        }
                        else if (index != 0 && index != sword.Trim().Length - 1)
                        {
                            sw.WriteLine(c + " I_");
                            index++;
                            continue;
                        }
                    }
                }
                sw1.WriteLine(line1);
                sw.WriteLine();
                sw.Flush();
            }
            sw.Close();
            sw1.Close();
            sr.Close();
        }
        public static void tocrfoutput(string readpath, string wirtedatapath, string rawdatdapath)
        {
            StreamReader tag_file = new StreamReader(Global.modelDir + "/tagIndex.txt");
            Dictionary<int, string> tags = new Dictionary<int, string>();

            string line2;
            while ((line2 = tag_file.ReadLine()) != null)
            {
                string[] wordTags = line2.Split(' ');
                tags.Add(int.Parse(wordTags[1]), wordTags[0]);
            }

            tag_file.Close();

            StreamReader outputtag = new StreamReader(readpath, Encoding.UTF8);
            StreamWriter sw = new StreamWriter(wirtedatapath, false, Encoding.UTF8);
            StreamReader rawtext = new StreamReader(rawdatdapath,Encoding.UTF8);
            string line;
            string[] linetag;


            while ((line = outputtag.ReadLine()) != null)
            {
                string write_string = "";
                string raw = rawtext.ReadLine();
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
                sw.WriteLine(write_string.Trim());
            }
            sw.Close();
            outputtag.Close();
            rawtext.Close();

        }
    }
}
