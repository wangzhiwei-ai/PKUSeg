using System;
using System.Collections.Generic;
using System.Text;


namespace Program1
{
    namespace Feature
    {
        class ListSortFunc
        {
            public static int compareKV_value(string a, string b)
            {
                string[] aAry = a.Split(Global.blankAry, StringSplitOptions.RemoveEmptyEntries);
                string[] bAry = b.Split(Global.blankAry, StringSplitOptions.RemoveEmptyEntries);
                double aProb = double.Parse(aAry[aAry.Length - 1]);
                double bProb = double.Parse(bAry[bAry.Length - 1]);
                if (aProb > bProb)
                    return 1;
                else if (aProb < bProb)
                    return -1;
                else return 0;
            }


            public static int compareKV_key(string a, string b)
            {
                string[] aAry = a.Split(Global.blankAry, StringSplitOptions.RemoveEmptyEntries);
                string[] bAry = b.Split(Global.blankAry, StringSplitOptions.RemoveEmptyEntries);
                double aProb = double.Parse(aAry[0]);
                double bProb = double.Parse(bAry[0]);
                if (aProb > bProb)
                    return 1;
                else if (aProb < bProb)
                    return -1;
                else return 0;
            }
        }


        class Tools
        {
            public static bool containLetter(string w)
            {
                string w2 = w.ToLower();
                w2 = "*" + w2 + "*";
                string[] ary = w2.Split(Global.normalLetter.ToCharArray());
                if (ary.Length > 1)
                    return true;
                else
                    return false;
            }

            public static bool isAllUpper(string w)
            {
                if (containLetter(w) == false)
                    return false;

                string W = w.ToUpper();
                if (w == W)
                    return true;
                else
                    return false;
            }


        }
    }
}

