using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.IO;

namespace Program
{
    public class param
    {
        public ManualResetEvent mre;
        public DataSeq seq;
        public List<DataStep> datastep;
        //public StreamWriter sw;
        //public string write_string="";
        
        public param(DataSeq seq=null)
        {
            this.seq = seq;
            datastep = seq.datasteps;
        }


    }
    class TrainThread
    {
        
        //public List<DataSeq> _X;//a dataSequence is a window
        public Object thislock = new object();
        public bool _train;
        public double _correct = 0;
        public double _total = 0;
        public double _prTotal = 0;


        public double _correct4 = 0;
        public double _total4 = 0;
        public double _prTotal4 = 0;



        public double accword = 0;
        public double totalword = 0;
        public double acc4word = 0;
   

        public double _decayRate = Global.decayRate;
        public double _smoothEpsilon = Global.SmoothEpsilon;
        public double _gradientClipValue = Global.GradientClipValue;
        public double _regularization = Global.L2Reg;

        public TrainThread(bool train)
        {
            //this._X = X;
            this._train = train;
        }

        public List<Matrix> reverse(List<Matrix> x)
        {
            List<Matrix> x1 = new List<Matrix>();
            for (int i = 0; i < x.Count(); i++)
            {
                x1.Add(x[x.Count() - 1 - i]);
            }

            return x1;

        }
        public void run(object param)
        {
            param pa = (param)param;
           List<DataStep> x1 = pa.datastep;
            List<Matrix> xpro = new List<Matrix>();
            ForwdBackwdProp g = new ForwdBackwdProp(_train);
            for (int i = 0; i < x1.Count; i++)
            {
                DataStep x = x1[i];

                List<Matrix> add = new List<Matrix>();
                for (int k = 0; k < 5; k++)
                {
                   
                   add.Add(Global.wordEmbedding[x.inputs[k]]);
                   
                    
                }
                List<Matrix> returnObj2 = Global.GRNNLayer1.activate(add, g);

                List<Matrix> returnObj3 = Global.GRNNLayer2.activate(returnObj2, g);
                List<Matrix> returnObj4 = Global.GRNNLayer3.activate(returnObj3, g);
                List<Matrix> returnObj5 = Global.GRNNLayer4.activate(returnObj4, g);

                xpro.Add(returnObj5[0]);
            }

            List<Matrix> returnObj6 = Global.upLSTMLayer.activate(xpro, g);

            List<Matrix> returnObj7 = Global.upLSTMLayerr.activate(reverse(xpro), g);


            List<Matrix> sum = new List<Matrix>();
            for (int inde = 0; inde < returnObj6.Count(); inde++)
            {
                sum.Add(g.Add(returnObj6[inde], returnObj7[returnObj7.Count - inde - 1]));
            }

            for (int i = 0; i < returnObj6.Count; i++)
            {
                Matrix returnObj9 = Global.feedForwardLayer.Activate(sum[i], g);
                double loss = LossSoftmax.getLoss(returnObj9, x1[i].goldOutput);
                if (double.IsNaN(loss) || double.IsInfinity(loss))
                {
                    Console.WriteLine("WARNING!!!");
                    Global.swLog.WriteLine("WARNING!!!");
                    pa.mre.Set();
                    return;
                }
                LossSoftmax.getGrad(returnObj9, x1[i].goldOutput);

            }
            g.backwardProp();
            pa.mre.Set();

        }
        public void runtest(object param)
        {
            param pa = (param)param;
            List<DataStep> x1 = pa.datastep;
            int[] ires4, igold4, wordindeis;
            string str = "", str1 = "";         
            igold4 = new int[x1.Count];
            ires4 = new int[x1.Count];
            wordindeis = new int[x1.Count];
            int index = 0, arraynum = 0;
            ForwdBackwdProp g = new ForwdBackwdProp(_train);
              
            int dim = 0;
              
            dim = x1.Count;
               

            Matrix[] xpro = new Matrix[dim];
            List<int> ires_model = new List<int>();


            //Parallel.For(0, temp.Count, i =>
            for (int i = 0; i < x1.Count; i++)
            {
                List<Matrix> add = new List<Matrix>();

                for (int k = 0; k < 5; k++)
                {
                    add.Add(Global.wordEmbedding[x1[i].inputs[k]]);
                        
                }
                List<Matrix> returnObj2 = Global.GRNNLayer1.activate(add, g);
                List<Matrix> returnObj3 = Global.GRNNLayer2.activate(returnObj2, g);
                List<Matrix> returnObj4 = Global.GRNNLayer3.activate(returnObj3, g);
                List<Matrix> returnObj5 = Global.GRNNLayer4.activate(returnObj4, g);
                xpro[i] = returnObj5[0];
            }//);
            List<Matrix> returnObj6 = Global.upLSTMLayer.activate(xpro.ToList(), g);
            List<Matrix> returnObj7 = Global.upLSTMLayerr.activate(reverse(xpro.ToList()), g);
            List<Matrix> sum = new List<Matrix>();
            for (int inde = 0; inde < returnObj6.Count(); inde++)
            {
                sum.Add(g.Add(returnObj6[inde], returnObj7[returnObj7.Count - inde - 1]));
            }

            for (int i = 0; i < xpro.Length; i++)
            {
                Matrix returnObj9 = Global.feedForwardLayer.Activate(sum[i], g);                   
                igold4[i] = LossSoftmax.getMax(x1[i].goldOutput);
                ires4[i]=LossSoftmax.getMax(returnObj9);
            }         

            //);

            
            //fscore.backprocess(wordindeis, ires4);       
            
            pa.seq.write_string = "BOS O O" + "\n";
            //pa.sw.WriteLine("BOS O O");
            for (int i = 0; i < ires4.Count(); i++)
            {
                pa.seq.write_string+=(Global.word[x1[i].wordindex] + " " + ires4[i] + " " + igold4[i]+"\n");
            }
            pa.seq.write_string += ("EOS O O"+"\n");
            pa.seq.write_string += "\n";


            List<string> res = fscore.getChunks4(ires4);
            str1 = fscore.calcorrect(fscore.getChunks4(igold4), res);

            lock (thislock)
            {

                string[] strs1 = str1.Split();
                _total4 += Int32.Parse(strs1[0]);
                _prTotal4 += Int32.Parse(strs1[1]);
                _correct4 += Int32.Parse(strs1[2]);
            }

        }
    }
}
