using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Program
{
    class Postprocessing
    {
        static String fullWidth2halfWidth(string fullWidthStr)
        {
            if (null == fullWidthStr || fullWidthStr.Length <= 0)
            {
                return "";
            }
            char[] charArray = fullWidthStr.ToCharArray();
            //对全角字符转换的char数组遍历
            for (int i = 0; i < charArray.Length; ++i)
            {
                int charIntValue = (int)charArray[i];
                //如果符合转换关系,将对应下标之间减掉偏移量65248;如果是空格的话,直接做转换
                if (charIntValue >= 65281 && charIntValue <= 65374)
                {
                    charArray[i] = (char)(charIntValue - 65248);
                }
                else if (charIntValue == 12288)
                {
                    charArray[i] = (char)32;
                }
            }
            return new String(charArray);
        }
        public static void transfer(string path)
        {

            String path1 = "data/temp/answer.txt"; // change to the path which the test script outputs
            //String path = "msr_test_gold.utf8" + "_string";
            StreamReader reader = new StreamReader(path, Encoding.UTF8);
            StreamReader reader1 = new StreamReader(path1, Encoding.UTF8);
            StreamWriter writer = new StreamWriter(Global.outputFile, false, Encoding.UTF8); // change to the output path  

            string str1 = "";
            while ((str1 = reader.ReadLine()) != null)
            {
                //System.out.println(str1);
                string halfstr1 = fullWidth2halfWidth(str1);
                string word = ""; int i = 0; string writerstr1 = "";
                string str = "";
                while ((str = reader1.ReadLine()) != null)
                {
                    if (str.Contains("EOS"))
                    {
                        break;
                    }
                    if (str.Contains("BOS"))
                    {
                        continue;
                    }
                    string[] wordstr = str.Split(' ');
                    word = wordstr[0];
                    if (word.Length == 0 || word.Trim().Equals(""))
                    {
                        continue;
                    }

                    string label = wordstr[1];

                    if (label.Contains("1"))
                    {
                        label = "b";
                    }
                    else if (label.Contains("0"))
                    {
                        label = "s";
                    }
                    else if (label.Contains("2"))
                    {
                        label = "m";
                    }
                    else if (label.Contains("3"))
                    {
                        label = "e";
                    }

                    if (word.Contains("</s>"))
                    {
                        continue;
                    }
                    if (!word.Contains("d") && !word.Contains("e"))
                    {

                        //System.out.println(word+label);
                        //if(label.equals("b")&&writerstr1.length()>=1&&!writerstr1.endsWith((char)12288+"")){
                        //	writerstr1+=(char)12288+"";
                        //}
                        //Console.WriteLine(str1+" "+" "+str1[i]); 
                        //if(word.contains("做")) System.out.println(label);
                        writerstr1 += str1[i] + "";
                        if (label.Equals("s") || label.Equals("e"))
                        {
                            writerstr1 += " ";
                        }
                    }
                    else if (word.Contains("d"))
                    {
                        String numbers = "1234567890"; String numword = "";
                        //if(label.equals("b")&&writerstr1.length()>=1&&!writerstr1.endsWith((char)12288+"")){
                        //	writerstr1+=(char)12288+"";
                        //}
                        while (i < str1.Length && numbers.Contains(halfstr1[i].ToString() + ""))
                        {
                            //System.out.println(str1.charAt(i)+" "+i);
                            numword += (str1[i] + "");
                            i++;
                        }
                        i--;
                        writerstr1 += numword;
                        if (label.Equals("s") || label.Equals("e"))
                        {
                            writerstr1 += " ";
                        }

                    }
                    else if (word.Contains("e"))
                    {
                        String numbers = "abcdefghigklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";
                        String enumwrod = "";
                        //if(label.equals("b")&&writerstr1.length()>=1&&!writerstr1.endsWith((char)12288+"")){
                        //	writerstr1+=(char)12288+"";
                        //}
                        while (i < str1.Length && numbers.Contains(halfstr1[i] + ""))
                        {
                            enumwrod += (str1[i] + "");
                            i++;
                        }
                        writerstr1 += enumwrod;
                        if (label.Equals("s") || label.Equals("e"))
                        {
                            writerstr1 += " ";
                        }
                        i--;
                    }
                    i++;

                }


                writer.WriteLine(writerstr1);

            }
            writer.Close();
            reader.Close();
            reader1.Close();

        }
    }

}