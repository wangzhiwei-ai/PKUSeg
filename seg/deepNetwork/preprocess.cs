using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Text.RegularExpressions;
using System.Collections;

namespace Program
{
    class preprocess
    {
        static Dictionary<string, int> word = new Dictionary<string, int>();
        static string ToSBC(string input)//single byte charactor
        {
            char[] c = input.ToCharArray();
            for (int i = 0; i < c.Length; i++)
            {
                if (c[i] == 12288)//全角空格为12288，半角空格为32
                {
                    c[i] = (char)32;
                    continue;
                }
                if (c[i] > 65280 && c[i] < 65375)//其他字符半角(33-126)与全角(65281-65374)的对应关系是：均相差65248
                    c[i] = (char)(c[i] - 65248);
            }
            return new string(c);
        }
        public static void readword()
        {
            StreamReader sr = new StreamReader("data/deepNetwork/word.txt", Encoding.Default);
            string line; int i = 0;
            while ((line = sr.ReadLine()) != null)
            {
                word.Add(line.ToString(), i);
                i++;
            }
        }
        public static int lookup(string str)
        {
            int i = 0;

            if (word.Keys.Contains(str.Trim()))
            {
                return word[str.Trim()];
            }
            return word.Count - 1;

        }
        //public static void romdomcuttrain()
        //{
        //    StreamReader sw = new StreamReader("data/trainData.txt");
        //    while ((line = sr.ReadLine()) != null)
        //    {
        //    }

        //    Hashtable hashtable = new Hashtable();
        //    Random rm = new Random();
        //    int RmNum = 10;
        //    for (int i = 0; hashtable.Count < RmNum; i++)
        //    {
        //        int nValue = rm.Next(100);
        //        if (!hashtable.ContainsValue(nValue) && nValue != 0)
        //        {
        //            hashtable.Add(nValue, nValue);
        //            Console.WriteLine(nValue.ToString());
        //        }
        //    }
        //}

        
        public static void tonumberdata(string read1path, string wirtedatapath, string writetagpath, string writerawpath)
        {
            StreamReader sr = new StreamReader(read1path, Encoding.UTF8);
            StreamWriter sw = new StreamWriter(wirtedatapath);
            StreamWriter sw_string = new StreamWriter(writerawpath);
            StreamWriter sw_label = new StreamWriter(writetagpath);

            string line;

            string[] words;
            while ((line = sr.ReadLine()) != null)
            {
                string line1 = "";
                string line1_string = "";
                line = ToSBC(line);

                String s1 = Regex.Replace(line, "([A-Za-z] )+|[A-Za-z]+", "e");
                String s2 = Regex.Replace(s1, "\\d+|(\\d )+", "d");

                words = s2.Split(' ');
                string label1 = "";
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
                        line1 += (lookup(c + "") + " ");
                        line1_string += (c);
                        if (index == 0 && sword.Trim().Length == 1)
                        {
                            label1 += (1 + " ");
                            index++;
                            continue;
                        }
                        else if (index == 0 && sword.Trim().Length != 1)
                        {
                            label1 += (2 + " ");
                            index++;
                            continue;
                        }
                        else if (index == sword.Trim().Length - 1 && sword.Trim().Length != 1)
                        {
                            label1 += (4 + " ");
                            index++;
                            continue;
                        }
                        else if (index != 0 && index != sword.Trim().Length - 1)
                        {
                            label1 += (3 + " ");
                            index++;
                            continue;
                        }




                    }


                }
                if(line1_string.Trim() != ""  && line1.Trim() != "" && label1.Trim() != "")
                {
                    sw_string.WriteLine(line1_string);
                    sw.WriteLine(line1.Trim());
                    sw_label.WriteLine(label1.Trim());
                    sw.Flush();
                    sw_label.Flush();
                }
               

            }
            sw.Close();
            sw_label.Close();

        }




        public static void tonumberdatabackground(string read1path, string wirtedatapath, string writetagpath)
        {
            StreamReader sr = new StreamReader(read1path, Encoding.UTF8);
            StreamWriter sw = new StreamWriter(wirtedatapath);

            StreamWriter sw_label = new StreamWriter(writetagpath);

            string line;

            char[] words;
            while ((line = sr.ReadLine()) != null)
            {
                if (line.Trim() == "")
                {
                    continue;
                }
                line = ToSBC(line);
                String s1 = Regex.Replace(line, "([A-Za-z] )+|[A-Za-z]+", "e");
                String s2 = Regex.Replace(s1, "\\d+|(\\d )+", "d");

                string line1 = "";
                words = s2.ToCharArray();
                string label1 = "";
                int index = 0;
                foreach (char sword1 in words)
                {
                    string sword = sword1 + "";
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
                        line1 += (lookup(c + "") + " ");
                        if (index == 0 && sword.Trim().Length == 1)
                        {
                            label1 += (1 + " ");
                            index++;
                            continue;
                        }
                        else if (index == 0 && sword.Trim().Length != 1)
                        {
                            label1 += (2 + " ");
                            index++;
                            continue;
                        }
                        else if (index == sword.Trim().Length - 1 && sword.Trim().Length != 1)
                        {
                            label1 += (4 + " ");
                            index++;
                            continue;
                        }
                        else if (index != 0 && index != sword.Trim().Length - 1)
                        {
                            label1 += (3 + " ");
                            index++;
                            continue;
                        }




                    }


                }
                sw.WriteLine(line1.Trim());
                sw_label.WriteLine(label1.Trim());
                sw.Flush();
                sw_label.Flush();

            }
            sw.Close();
            sw_label.Close();

        }

        public static void tonumberdatabackground(string read1path, string wirtedatapath)
        {
            StreamReader sr = new StreamReader(read1path, Encoding.UTF8);
            StreamWriter sw = new StreamWriter(wirtedatapath);


            string line;

            char[] words;
            while ((line = sr.ReadLine()) != null)
            {
                // String s1 = Regex.Replace(line, "([A-Za-z] )+|[A-Za-z]+", "e");
                String s2 = Regex.Replace(line, "\\d+|(\\d )+", "d");

                string line1 = "";
                words = s2.ToCharArray();
                string label1 = "";
                int index = 0;
                foreach (char sword1 in words)
                {
                    string sword = sword1 + "";
                    if (sword.Trim() == " " || sword.Trim() == "")
                    {
                        continue;
                    }
                    index = 0;
                    foreach (char c in sword.Trim())
                    {
                        line1 += (lookup(c + "") + " ");
                        if (index == 0 && sword.Trim().Length == 1)
                        {
                            label1 += (1 + " ");
                            index++;
                            continue;
                        }
                        else if (index == 0 && sword.Trim().Length != 1)
                        {
                            label1 += (2 + " ");
                            index++;
                            continue;
                        }
                        else if (index == sword.Trim().Length - 1 && sword.Trim().Length != 1)
                        {
                            label1 += (4 + " ");
                            index++;
                            continue;
                        }
                        else if (index != 0 && index != sword.Trim().Length - 1)
                        {
                            label1 += (3 + " ");
                            index++;
                            continue;
                        }




                    }


                }
                sw.WriteLine(line1.Trim());

                sw.Flush();


            }
            sw.Close();


        }


    }
}
