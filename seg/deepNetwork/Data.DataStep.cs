using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Program
{
    public class DataStep
    {
        public Matrix goldOutput = null;//1-hot gold output, a vector of # of tags
        public List<int> inputs = null;//inputs of word embedings
        public int wordindex = 0;
        public List<int> bigram  = null;
        public List<int> bigramlast = null;
        public List<int> bigram1= null;
        public List<int> bigramlast1 = null;
     

        public DataStep()
        {
            inputs = new List<int>();
            bigram = new List<int>();
            bigramlast = new List<int>();
            bigram1 = new List<int>();
            bigramlast1 = new List<int>();
        }

        public DataStep(List<int> input,  Matrix targetOutput, int wordindex, List<int> big=null, List<int> big1=null, List<int> biglast=null, List<int> biglast1=null)
        {
            this.inputs = input;
            this.bigram = big;
            this.bigramlast = biglast;
            this.bigram1 = big1;
            this.bigramlast1 = biglast1;
            this.wordindex = wordindex;
            if (targetOutput != null)
            {
                this.goldOutput = targetOutput;
            }
        }



    }
}
