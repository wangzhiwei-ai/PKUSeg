using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using MathNet.Numerics.Distributions;

namespace Program
{
    class Global
    {
        //public const bool forward = true;
        public static string optimizer = "rmprop";//rmprop, sgd
        public const int nThread = 8;
        public const double trainingScale = 1;
        public const double testScale = 1;
        public static string mode = "test";//  "train" or "test" or "predict";
        public static string readFile = "data/msr_test_gold.utf8";
        public static string outputFile = "data/msr_test_output.utf8";
        public static string trainFile = "data/msr_training.utf8";
        public static string testFile = "data/msr_test_gold.utf8";

        public static int inputDim = 100;
        public static int hiddenDim = 100;
        public const int bigramDim = 30;
        public const int outputDimension = 4;//B, M(middle), E, S(single)
        public const double learningRate0 = 0.0001;//0.001
        public static double decayRate = 0.95;

        public const double upbound = 0.01;
   
        public const int reportEveryNthEpoch = 1;
        public const int trainIter = 10;
        public static double SmoothEpsilon = 0.0001;
        public static double GradientClipValue = 5;
        public static double L2Reg = 0.000001; // L2 regularization strength
        public static int updatetimes = 0;
        public static int isRead = 1;        
        public static LSTMLayer upLSTMLayer;
        public static LSTMLayer upLSTMLayerr;
        public static FeedForwardLayer feedForwardLayer;
        public static GRNNLayer GRNNLayer1;
        public static GRNNLayer GRNNLayer2;
        public static GRNNLayer GRNNLayer3;
        public static GRNNLayer GRNNLayer4;
        public static StreamWriter swLog = new StreamWriter("data/temp/log.txt");
        public static List<TrainThread> threadList;
        public static Normal randn;
        public static Matrix[] wordEmbedding = new Matrix[6000];
        public static int length;      
        public static List<string> fourword = new List<string>();     
        public static List<string> word = new List<string>();
        public static List<List<string>> traingold = new List<List<string>>();
    }
}
