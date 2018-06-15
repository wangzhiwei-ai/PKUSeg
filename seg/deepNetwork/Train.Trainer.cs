using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Security.Cryptography;
using System.Diagnostics;
using System.Threading.Tasks;


namespace Program
{
    public class Trainer
    {
        public static double testAccuracy = 0, testAccuracy1 = 0;
        public static void train(DataSet X) 
        {
            if (Global.mode == "test")
            {
                Console.WriteLine("begain testing......");
                testAccuracy1 = runtestIteration(X.Testing, false);
                Console.WriteLine("Epoch test  f-score: {0}", (testAccuracy1 * 100).ToString("f3"));
                //Console.WriteLine("Epoch test  best f-score: {0}", (testAccuracy * 100).ToString("f3"));
                Global.swLog.WriteLine("Epoch test fscore: {0}", (testAccuracy1 * 100).ToString("f3"));
                //Global.swLog.WriteLine("Epoch test best fscore: {0}", (testAccuracy * 100).ToString("f3"));
                Postprocessing.transfer("data/temp/" +  "test_raw.txt");

                Console.WriteLine("Finished");

            }


            //else if (Global.mode == "test")
            //{
            //    Console.WriteLine("begain testing......");
            //    testAccuracy1 = runtestIteration(X.Testing, false);
            //    Postprocessing.transfer(Global.readFile);
            //    Console.WriteLine("predict fininshed");
            //    Global.swLog.WriteLine("predict fininshed");

            //}
            else if (Global.mode == "train")
            {

                for (int iter = 0; iter < Global.trainIter; iter++)
                {
                    DateTime begin = DateTime.Now;

                    Console.WriteLine("\niter: {0}", iter + 1);
                    Global.swLog.WriteLine("\niter: {0}", iter + 1);


                    double trainAccuracy = runtrainIteration(X.Training, X.Testing, true, iter);

                    if (double.IsNaN(trainAccuracy) || double.IsInfinity(trainAccuracy))
                    {
                        Console.WriteLine("WARNING: invalid value for training loss. Try lowering learning rate.");
                    }

                    //test

                    testAccuracy1 = runtestIteration(X.Testing, false);

                    if (testAccuracy <= testAccuracy1)
                    {
                        LSTMLayer.SerializeWordembedding("model//deepNetwork//embedding");
                        //LSTMLayer.SerializeBigramWordembedding();
                        Global.upLSTMLayer.saveLSTM("model//deepNetwork//lstmmodel.txt");
                        Global.upLSTMLayerr.saveLSTM("model//deepNetwork//lstmmodelr.txt");
                        Global.GRNNLayer1.saveGRNN("model//deepNetwork//grnnmodel1.txt");
                        Global.GRNNLayer2.saveGRNN("model//deepNetwork//grnnmodel1.txt");
                        Global.GRNNLayer3.saveGRNN("model//deepNetwork//grnnmodel1.txt");
                        Global.GRNNLayer4.saveGRNN("model//deepNetwork//grnnmodel1.txt");                        
                        Global.feedForwardLayer.saveFFmodel("model//deepNetwork//feedforwardmodel.txt");
                        testAccuracy = testAccuracy1;
                    }

                    DateTime end = DateTime.Now;

                    // Console.WriteLine("train f-score: {0}", (trainAccuracy*100).ToString("f3"));
                    Console.WriteLine("test f-score: {0}", (testAccuracy * 100).ToString("f3"));
                    Console.WriteLine("test f-score: {0}", (testAccuracy1 * 100).ToString("f3"));
                    Console.WriteLine("time used: {0}", end - begin);

                    //Global.swLog.WriteLine("train f-score: {0}", (trainAccuracy * 100).ToString("f3"));
                    Global.swLog.WriteLine("test f-score: {0}", (testAccuracy * 100).ToString("f3"));
                    Global.swLog.WriteLine("test f-score: {0}", (testAccuracy1 * 100).ToString("f3"));
                    Global.swLog.WriteLine("time used: {0}", end - begin);



                }
            }
        }

        public static int random()
        {
            Random r = new Random();
            return r.Next(2);
        }
        public static List<DataSeq> shuffle(List<DataSeq> list)
        {
            Random rand = new Random();
            List<DataSeq> returnObject = new List<DataSeq>();

            for (int i = 0; i < list.Count; i++)
            {

                int length = list[i].datasteps.Count;
                int index = 0;

                //while (index < length)
                //{
                    //DataSeq temp = new DataSeq();
                    //int dim = 0;
                    ////if (random() == 0)
                    ////{
                    ////    dim = 1;
                    ////}
                    ////else
                    ////{
                    ////    dim = 2;
                    ////}
                    // dim = length;
                    //for (int k = 0; k < dim && index < length; k++, index++)
                    //{
                    //    temp.Add(list[i]);
                    //}
                    returnObject.Add(list[i]);
                //}
            }
            //for (int i = 0; i < list.Count; i++)
            //{
            //    for(int j=0;j<list[i].datasteps.Count;j++){
            //        returnObject.Add(list[i].datasteps[j]);
            //    }
            //}
            int n = returnObject.Count;
            while (n > 1)
            {
                n--;
                int k = rand.Next(n + 1);
                DataSeq value = returnObject[k];
                returnObject[k] = returnObject[n];
                returnObject[n] = value;
            }
            return returnObject;
        }
       
        public static double runtrainIteration(List<DataSeq> X, List<DataSeq> Xtest, bool train,int iter)
        {
          

            List<DataSeq> x = new List<DataSeq>();
            if (train)
            {
              x=shuffle(X);//shuffle every window (point)
            }       
    
            TrainThread runThread = new TrainThread(train);
            
             List<ManualResetEvent> manualEvents = new List<ManualResetEvent>();
             List<DataStep> temp = new List<DataStep>();
             
             int i = 0, j = 0;
             int length=x.Count();
             while (i<length)
             {
                 if (i!=0&&i % 16 == 0)
                 {
                     testAccuracy1 = runtestIteration(Xtest, false);

                     if (testAccuracy <= testAccuracy1)
                     {
                        LSTMLayer.SerializeWordembedding("model//deepNetwork//embedding");
                        //LSTMLayer.SerializeBigramWordembedding();
                        Global.upLSTMLayer.saveLSTM("model//deepNetwork//lstmmodel.txt");
                        Global.upLSTMLayerr.saveLSTM("model//deepNetwork//lstmmodelr.txt");
                        Global.GRNNLayer1.saveGRNN("model//deepNetwork//grnnmodel1.txt");
                        Global.GRNNLayer2.saveGRNN("model//deepNetwork//grnnmodel1.txt");
                        Global.GRNNLayer3.saveGRNN("model//deepNetwork//grnnmodel1.txt");
                        Global.GRNNLayer4.saveGRNN("model//deepNetwork//grnnmodel1.txt");
                        Global.feedForwardLayer.saveFFmodel("model//deepNetwork//feedforwardmodel.txt");
                        testAccuracy = testAccuracy1;


                    }


                     Console.WriteLine("test f-score: {0}", (testAccuracy * 100).ToString("f3"));
                     Console.WriteLine("test1 f-score: {0}", (testAccuracy1 * 100).ToString("f3"));

                     Global.swLog.WriteLine("test f-score: {0}", (testAccuracy * 100).ToString("f3"));
                     Global.swLog.WriteLine("test f-score: {0}", (testAccuracy1 * 100).ToString("f3"));

                 }
                for (int k = 0; k < Global.nThread && i < length; k++, i++)
                {
                    ManualResetEvent mre = new ManualResetEvent(false);
                    param pa = new param(x[i]);
                    pa.mre = mre;
                    pa.datastep = x[i].datasteps;
                    manualEvents.Add(mre);
                    for (int m = 0; m < x[i].datasteps.Count; m++)
                    {
                        temp.Add(x[i].datasteps[m]);
                    }
                    ThreadPool.QueueUserWorkItem(new WaitCallback(runThread.run), pa);
                }
                WaitHandle.WaitAll(manualEvents.ToArray());
                 if (train)
                 {
                     UpdateWeight_rmProp(temp);
                 }
                 manualEvents.Clear();
                 temp.Clear();
             }
        
     
             return runThread.accword / runThread.totalword;
        }
        public static double runtestIteration(List<DataSeq> X, bool train)
        {
            double _total = 0, _prTotal = 0, _correct = 0;
           
            double total = 0, accuracy = 0, recall = 0, f_score = 0;
            double _total4 = 0, _prTotal4 = 0, _correct4 = 0, accuracy4 = 0, f_score4 = 0, recall4 = 0;

            Global.threadList = new List<TrainThread>();


            TrainThread runThread = new TrainThread(train);


            List<ManualResetEvent> manualEvents = new List<ManualResetEvent>();
            List<DataStep> temp = new List<DataStep>();
            StreamWriter sw = new StreamWriter("data/temp/answer.txt");


            string[] write_strings = new string[X.Count];
            Parallel.ForEach(X, (ex) =>
            {
                param pa = new param(ex);
                runThread.runtest(pa);

            });
            for (int i = 0; i < X.Count; i++)
            {
                sw.Write(X[i].write_string);
            }

            
            sw.Close();
            accuracy = runThread._correct / runThread._prTotal;
            recall = runThread._correct / runThread._total;

            accuracy4 = runThread._correct4 / runThread._prTotal4;
            recall4 = runThread._correct4 / runThread._total4;

            f_score = 2 * accuracy * recall / (accuracy + recall);
            f_score4 = 2 * accuracy4 * recall4 / (accuracy4 + recall4);
            //Console.WriteLine("total windows: " + _total);
            //Console.WriteLine("acc: " + (accuracy4 * 100).ToString("f3"));
            //Console.WriteLine("recall: " + (recall4 * 100).ToString("f3"));
            //Console.WriteLine("fscore: " + f_score4);
            //Global.swLog.WriteLine("total windows: " + total);
            //Global.swLog.WriteLine("acc: " + (accuracy4 * 100).ToString("f3"));
            //Global.swLog.WriteLine("recall: " + (recall4 * 100).ToString("f3"));
            //Global.swLog.WriteLine("fscore: " + f_score4);
            //Global.swLog.Flush();
            return f_score4;
        }

        public static void UpdateWeight_rmProp(List<DataStep> x)
        {
          
            foreach (Matrix m in Global.GRNNLayer1.GetParameters(x))
            {
                for (int i = 0; i < m.W.Length; i++)
                {
                    double gradWi = m.gradW[i] / Global.nThread;//*x.Dw[i];
                    if (gradWi > Global.GradientClipValue)
                    {
                        gradWi = Global.GradientClipValue;
                    }
                    if (gradWi < -Global.GradientClipValue)
                    {
                        gradWi = -Global.GradientClipValue;
                    }
                    m.rmPropStepCache[i] = m.rmPropStepCache[i] * Global.decayRate + (1 - Global.decayRate) * gradWi * gradWi;//rmProp update routine
                    m.rmPropStepCache2[i] = m.rmPropStepCache2[i] * Global.decayRate + (1 - Global.decayRate) * gradWi;
                    m.rmPropStepCache3[i] = (double)(0.9 * m.rmPropStepCache3[i] - 1e-4 * gradWi / Math.Sqrt(m.rmPropStepCache[i] - m.rmPropStepCache2[i] * m.rmPropStepCache2[i] + Global.SmoothEpsilon));
                    m.W[i] +=(double) (1.5 * m.rmPropStepCache3[i]);//- _regularization * m.W[i];//rmProp update routine
                    m.gradW[i] = 0;
                }
            }
            foreach (Matrix m in Global.GRNNLayer2.GetParameters())
            {
                for (int i = 0; i < m.W.Length; i++)
                {
                    double gradWi = m.gradW[i] / Global.nThread;//*x.Dw[i];
                    if (gradWi > Global.GradientClipValue)
                    {
                        gradWi = Global.GradientClipValue;
                    }
                    if (gradWi < -Global.GradientClipValue)
                    {
                        gradWi = -Global.GradientClipValue;
                    }
                    m.rmPropStepCache[i] = m.rmPropStepCache[i] * Global.decayRate + (1 - Global.decayRate) * gradWi * gradWi;//rmProp update routine
                    m.rmPropStepCache2[i] = m.rmPropStepCache2[i] * Global.decayRate + (1 - Global.decayRate) * gradWi;
                    m.rmPropStepCache3[i] = (double)(0.9 * m.rmPropStepCache3[i] - 1e-4 * gradWi / Math.Sqrt(m.rmPropStepCache[i] - m.rmPropStepCache2[i] * m.rmPropStepCache2[i] + Global.SmoothEpsilon));
                    m.W[i] += (double)(1.5 * m.rmPropStepCache3[i]);//- _regularization * m.W[i];//rmProp update routine
                    m.gradW[i] = 0;
                }
            }
            foreach (Matrix m in Global.GRNNLayer3.GetParameters())
            {
                for (int i = 0; i < m.W.Length; i++)
                {
                    double gradWi = m.gradW[i] / Global.nThread;//*x.Dw[i];
                    if (gradWi > Global.GradientClipValue)
                    {
                        gradWi = Global.GradientClipValue;
                    }
                    if (gradWi < -Global.GradientClipValue)
                    {
                        gradWi = -Global.GradientClipValue;
                    }
                    m.rmPropStepCache[i] = m.rmPropStepCache[i] * Global.decayRate + (1 - Global.decayRate) * gradWi * gradWi;//rmProp update routine
                    m.rmPropStepCache2[i] = m.rmPropStepCache2[i] * Global.decayRate + (1 - Global.decayRate) * gradWi;
                    m.rmPropStepCache3[i] = (double)(0.9 * m.rmPropStepCache3[i] - 1e-4 * gradWi / Math.Sqrt(m.rmPropStepCache[i] - m.rmPropStepCache2[i] * m.rmPropStepCache2[i] + Global.SmoothEpsilon));
                    m.W[i] += (double)1 * m.rmPropStepCache3[i];//- _regularization * m.W[i];//rmProp update routine
                    m.gradW[i] = 0;
                }
            }

            foreach (Matrix m in Global.GRNNLayer4.GetParameters())
            {
                for (int i = 0; i < m.W.Length; i++)
                {
                    double gradWi = m.gradW[i] / Global.nThread;//*x.Dw[i];
                    if (gradWi > Global.GradientClipValue)
                    {
                        gradWi = Global.GradientClipValue;
                    }
                    if (gradWi < -Global.GradientClipValue)
                    {
                        gradWi = -Global.GradientClipValue;
                    }
                    m.rmPropStepCache[i] = m.rmPropStepCache[i] * Global.decayRate + (1 - Global.decayRate) * gradWi * gradWi;//rmProp update routine
                    m.rmPropStepCache2[i] = m.rmPropStepCache2[i] * Global.decayRate + (1 - Global.decayRate) * gradWi;
                    m.rmPropStepCache3[i] = (double)(0.9 * m.rmPropStepCache3[i] - 1e-4 * gradWi / Math.Sqrt(m.rmPropStepCache[i] - m.rmPropStepCache2[i] * m.rmPropStepCache2[i] + Global.SmoothEpsilon));
                    m.W[i] += 1 * m.rmPropStepCache3[i];//- _regularization * m.W[i];//rmProp update routine
                    m.gradW[i] = 0;
                }
            }
            foreach (Matrix m in Global.upLSTMLayer.GetParameters())
            {
                for (int i = 0; i < m.W.Length; i++)
                {
                    double gradWi = m.gradW[i] / Global.nThread;//*x.Dw[i];
                    if (gradWi > Global.GradientClipValue)
                    {
                        gradWi = Global.GradientClipValue;
                    }
                    if (gradWi < -Global.GradientClipValue)
                    {
                        gradWi = -Global.GradientClipValue;
                    }
                    m.rmPropStepCache[i] = m.rmPropStepCache[i] * Global.decayRate + (1 - Global.decayRate) * gradWi * gradWi;//rmProp update routine
                    m.rmPropStepCache2[i] = m.rmPropStepCache2[i] * Global.decayRate + (1 - Global.decayRate) * gradWi;
                    m.rmPropStepCache3[i] = (double)(0.9 * m.rmPropStepCache3[i] - 1e-4 * gradWi / Math.Sqrt(m.rmPropStepCache[i] - m.rmPropStepCache2[i] * m.rmPropStepCache2[i] + Global.SmoothEpsilon));
                    m.W[i] += 1 * m.rmPropStepCache3[i];//- _regularization * m.W[i];//rmProp update routine
                    m.gradW[i] = 0;
                }
            }

            foreach (Matrix m in Global.upLSTMLayerr.GetParameters())
            {
                for (int i = 0; i < m.W.Length; i++)
                {
                    double gradWi = m.gradW[i] / Global.nThread;//*x.Dw[i];
                    if (gradWi > Global.GradientClipValue)
                    {
                        gradWi = Global.GradientClipValue;
                    }
                    if (gradWi < -Global.GradientClipValue)
                    {
                        gradWi = -Global.GradientClipValue;
                    }
                    m.rmPropStepCache[i] = m.rmPropStepCache[i] * Global.decayRate + (1 - Global.decayRate) * gradWi * gradWi;//rmProp update routine
                    m.rmPropStepCache2[i] = m.rmPropStepCache2[i] * Global.decayRate + (1 - Global.decayRate) * gradWi;
                    m.rmPropStepCache3[i] = (double)(0.9 * m.rmPropStepCache3[i] - 1e-4 * gradWi / Math.Sqrt(m.rmPropStepCache[i] - m.rmPropStepCache2[i] * m.rmPropStepCache2[i] + Global.SmoothEpsilon));
                    m.W[i] += 1 * m.rmPropStepCache3[i];//- _regularization * m.W[i];//rmProp update routine
                    m.gradW[i] = 0;
                }
            }


            foreach (Matrix m in Global.feedForwardLayer.GetParameters())
            {
                for (int i = 0; i < m.W.Length; i++)
                {
                    double gradWi = m.gradW[i] / Global.nThread;//*x.Dw[i];
                    if (gradWi > Global.GradientClipValue)
                    {
                        gradWi = Global.GradientClipValue;
                    }
                    if (gradWi < -Global.GradientClipValue)
                    {
                        gradWi = -Global.GradientClipValue;
                    }
                    m.rmPropStepCache[i] = m.rmPropStepCache[i] * Global.decayRate + (1 - Global.decayRate) * gradWi * gradWi;//rmProp update routine
                    m.rmPropStepCache2[i] = m.rmPropStepCache2[i] * Global.decayRate + (1 - Global.decayRate) * gradWi;
                    m.rmPropStepCache3[i] = (double)(0.9 * m.rmPropStepCache3[i] - 1e-4 * gradWi / Math.Sqrt(m.rmPropStepCache[i] - m.rmPropStepCache2[i] * m.rmPropStepCache2[i] + Global.SmoothEpsilon));
                    m.W[i] += 1 * m.rmPropStepCache3[i];//- _regularization * m.W[i];//rmProp update routine
                    m.gradW[i] = 0;
                }
            }
        }
    }
}
