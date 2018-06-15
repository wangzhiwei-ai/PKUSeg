using System;

namespace Program
{
    [Serializable]
    public class ActivateFunc
    {
        public static double sigForward(double x)
        {
            return (double)(1 / (1 + Math.Exp(-x)));
        }

        //y*(1-y)
        public static double sigBackward(double x)
        {
            double act = sigForward(x);
            return act * (1 - act);
        }



        public static double tanhForward(double x)
        {
            return (double)(Math.Tanh(x));
        }

        public static double tanhBackward(double x)
        {
            double coshx = (double)(Math.Cosh(x));
            double denom = (double)(Math.Cosh(2 * x) + 1);
            return 4 * coshx * coshx / (denom * denom);
        }
    }
}
