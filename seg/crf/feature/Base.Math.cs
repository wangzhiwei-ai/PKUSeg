//
//  CRF-ADF Toolkit v1.0
//
//  Copyright(C) Xu Sun <xusun@pku.edu.cn> http://klcl.pku.edu.cn/member/sunxu/index.htm
//

using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Collections;
using System.IO.Compression;


namespace Program1
{
    namespace Feature
    {
        class MathTool
        {
            private static int[] _primesBelow30 = new int[] { 1, 7, 11, 13, 17, 19, 23, 29 };

            public static double cos(List<double> a, List<double> b)
            {
                if (a.Count != b.Count)
                    throw new Exception("error");
                double kern = 0.0;

                double miNorm = 0, mjNorm = 0, innerProduct = 0;
                for (int k = 0; k < a.Count; k++)
                {
                    miNorm += a[k] * a[k];
                    mjNorm += b[k] * b[k];
                    innerProduct += a[k] * b[k];
                }
                miNorm = Math.Sqrt(miNorm);
                mjNorm = Math.Sqrt(mjNorm);
                kern = innerProduct / (miNorm * mjNorm);
                return kern;
            }


            public static int nextPrime(int a)
            {
                if (a <= 2) return 2;
                if (a <= 3) return 3;
                if (a <= 5) return 5;

                int i = a / 30;
                for (int j = 0; j < 8; j++)
                {
                    int c = 30 * i + _primesBelow30[j];
                    if (c >= a && isPrime(c)) return c;
                }

                while (true)
                {
                    i++;
                    for (int j = 0; j < 8; j++)
                    {
                        int c = 30 * i + _primesBelow30[j];
                        if (isPrime(c)) return c;
                    }
                }
            }

            public static bool isPrime(int n)
            {
                if (n == 1) return false;
                if (n > 2 && n % 2 == 0) return false;
                if (n > 3 && n % 3 == 0) return false;
                if (n > 5 && n % 5 == 0) return false;
                for (int i = 1; i < 8; i++)
                {
                    if (n > _primesBelow30[i] && n % _primesBelow30[i] == 0)
                        return false;
                }
                int limit = (int)Math.Sqrt(n) / 30;
                for (int i = 1; i <= limit; i++)
                {
                    for (int j = 0; j < 8; j++)
                    {
                        if (n % (30 * i + _primesBelow30[j]) == 0)
                            return false;
                    }
                }
                return true;
            }

        }

    }

}
