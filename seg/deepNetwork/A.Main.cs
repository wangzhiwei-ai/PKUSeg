using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using MathNet.Numerics.Distributions;

namespace Program
{
    class Program
    {


        static void readfourword()
        {
            StreamReader sr = new StreamReader("data/deepNetwork/idoim.txt", Encoding.UTF8);
            string line;
            while ((line = sr.ReadLine()) != null)
            {
                Global.fourword.Add(line.ToString().Trim());
            }
            sr.Close();
        }


        static void readword()
        {
            StreamReader sr = new StreamReader("data/deepNetwork/word.txt", Encoding.Default);
            string line;
            while ((line = sr.ReadLine()) != null)
            {
                Global.word.Add(line.ToString());
            }
        }
       
        public static void Run()
        {
            

            System.DateTime currentTime = System.DateTime.Now;
            readword();

            
            Global.inputDim = 100;
            Global.hiddenDim = 100;
           
            readfourword();            

            Global.randn = new Normal();
            DataSet X = new DataSet();
            if (Global.isRead == 1)
            {
                string path = "model//deepNetwork";            

                Global.upLSTMLayer = LSTMLayer.readLSTM(path  + "//lstmmodel.txt");
                Global.upLSTMLayerr = LSTMLayer.readLSTM(path  + "//lstmmodelr.txt");
                Global.GRNNLayer1 = GRNNLayer.readGRNN(path  + "//grnnmodel1.txt");
                Global.GRNNLayer2 = GRNNLayer.readGRNN(path + "//grnnmodel2.txt");
                Global.GRNNLayer3 = GRNNLayer.readGRNN(path + "//grnnmodel3.txt");
                Global.GRNNLayer4 = GRNNLayer.readGRNN(path + "//grnnmodel4.txt");
                Global.feedForwardLayer = FeedForwardLayer.readFF(path  + "//feedforwardmodel.txt");
               
                Global.wordEmbedding = LSTMLayer.getSerializeWordembedding(path + "//embedding.txt", Global.wordEmbedding);

            }

            else
            {
              
                Global.GRNNLayer1 = new GRNNLayer();
                Global.GRNNLayer2 = new GRNNLayer();
                Global.GRNNLayer3 = new GRNNLayer();
                Global.GRNNLayer4 = new GRNNLayer();
                Global.upLSTMLayer = new LSTMLayer();
                Global.upLSTMLayerr = new LSTMLayer();
                Global.feedForwardLayer = new FeedForwardLayer();
            }
                                  

            Trainer.train(X);

            Global.swLog.Close();

            System.DateTime currentTime_1 = System.DateTime.Now;

            Console.WriteLine(currentTime_1 - currentTime);

            Console.Read();




        }
    }
}
