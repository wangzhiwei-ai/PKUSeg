using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;


namespace Program
{
    public class DataSet
    {
        public int InputDimension;
        public int OutputDimension;
        public List<DataSeq> Training;//a dataSequence is a window
        public List<DataSeq> Testing;
        public static int num = 0;
        int bigramlength = 0;
        //Matrix[] wordEmbedding = new Matrix[6000];//一个单词对应一个word embedding
        List<List<int>> trainData = new List<List<int>>();
        List<List<int>> trainDatabigram1 = new List<List<int>>();
        List<List<int>> trainDatabigram2 = new List<List<int>>();
        List<List<int>> trainDatabigram3 = new List<List<int>>();
        List<List<int>> trainDatabigram4 = new List<List<int>>();
        List<List<Matrix>> trainLabel = new List<List<Matrix>>();


        List<List<int>> testData = new List<List<int>>();

        List<List<int>> testDatabigram1 = new List<List<int>>();
        List<List<int>> testDatabigram2 = new List<List<int>>();
        List<List<int>> testDatabigram3 = new List<List<int>>();
        List<List<int>> testDatabigram4 = new List<List<int>>();
        List<List<Matrix>> testLabel = new List<List<Matrix>>();


        //指定输入输出的维度
        public DataSet()
        {
            InputDimension = Global.inputDim;
            OutputDimension = Global.outputDimension;
            getwordembedding();

            if (Global.mode == "test")
            {
                preprocess.readword();
                preprocess.tonumberdatabackground(Global.readFile, "data/temp/" + "test_data.txt", "data/temp/" + "test_tag.txt", "data/temp/" + "test_raw.txt");
                //preprocess.tonumberdata(Global.readFile, "data /temp/" + "_data", "data/temp/" + "_tag", "data/temp/" + "_raw");
                readTestData("data/temp/" + "test_data.txt", "data/temp/" + "test_tag.txt");
            }
            //else if (Global.mode == "predict")
            //{
            //    preprocess.readword();
            //    preprocess.tonumberdatabackground(Global.readFile, "data/temp/" + "_data", "data/temp/" + "_tag");
            //    readTestData("data/temp/" +  "_data", "data/temp/" + "_tag");
            //}
            else if (Global.mode == "train")
            {
                preprocess.readword();
                preprocess.tonumberdata(Global.trainFile, "data/temp/" + "train_data.txt", "data/temp/" + "train_tag.txt", "data/temp/" + "train_raw.txt");
                preprocess.tonumberdata(Global.testFile, "data/temp/" + "test_data.txt", "data/temp/" + "test_tag.txt", "data/temp/" + "test_raw.txt");
                readTestData("data/temp/" + "test_data.txt", "data/temp/" + "test_tag.txt");
                readTrainData("data/temp/" + "train_data.txt", "data/temp/" + "train_tag.txt");
            }
            Training = GetTrainingData(Global.trainingScale);
            Testing = GetTestData(Global.testScale);
        }

        public void getwordembedding()
        {
            StreamReader sr = new StreamReader("data/deepNetwork/vector.txt");
            String line; int j = 0;
            while ((line = sr.ReadLine()) != null)
            {
                double[] temp = new double[100];
                string[] strs = line.Split(' ');
                for (int i = 0; i < 100; i++)
                {
                    temp[i] = (double)Convert.ToSingle(strs[i]);
                }
                Global.wordEmbedding[j] = (new Matrix(temp));
                j++;

            }
            Global.length = j;
            num = j;
            sr.Close();
        }

        public static int indexunigram(String str)
        {
            for (int i = 0; i < Global.word.Count; i++)
            {
                if (Global.word[i].Trim() == str.Trim())
                {
                    return i;
                }
            }
            return Global.word.Count-10;
        }        
    
       
        //读入训练数据
        public void readTrainData(string data, string tag)
        {
            StreamReader sr = new StreamReader(data);
            String line; int len = 0;
            while ((line = sr.ReadLine()) != null)
            {

               
                len++;
            
                List<int> temp = new List<int>();
                List<int>bigramindex1=new List<int>();
                List<int> bigramindex2 = new List<int>();
                List<int> bigramindex3 = new List<int>();
                List<int> bigramindex4 = new List<int>();
                string[] strs = line.Trim().Split(' ');
                int begin = 1955;
                int sentencebegin = 1955;
                int sentenceend = 1956; 
                String big = "";
 
                for (int i = 0; i < strs.Length; i++)
                {
                    temp.Add(Convert.ToInt32(strs[i]));

                   
                    

                }
                trainData.Add(temp);
               

            }
            sr.Close();


            StreamReader sr1 = new StreamReader(tag);
            String line1;
            while ((line1 = sr1.ReadLine()) != null)
            {
                List<Matrix> temp = new List<Matrix>();
                string[] strs = line1.Split(' ');
                for (int i = 0; i < strs.Length; i++)
                {
                    int x = Convert.ToInt32(strs[i]);
                    if (x == 1)
                    {
                        temp.Add(new Matrix(new double[] { 1, 0, 0, 0 }));
                    }
                    else if (x == 2)
                    {
                        temp.Add(new Matrix(new double[] { 0, 1, 0, 0 }));
                    }
                    else if (x == 3)
                    {
                        temp.Add(new Matrix(new double[] { 0, 0, 1, 0 }));
                    }
                    else if (x == 4)
                    {
                        temp.Add(new Matrix(new double[] { 0, 0, 0, 1 }));
                    }
                    
                }
                trainLabel.Add(temp);

            }
            sr1.Close();
        }


        public void readTestData(string path, string tag_path)
        {
            StreamReader sr = new StreamReader(path);
            String line; int len = 0;
            while ((line = sr.ReadLine()) != null)
            {


                len++;
                // if (len > 20) { break; }
                List<int> temp = new List<int>();
                List<int> bigramindex1 = new List<int>();
                List<int> bigramindex2 = new List<int>();
                List<int> bigramindex3 = new List<int>();
                List<int> bigramindex4 = new List<int>();
                string[] strs = line.Trim().Split(' ');
                int begin = 1955;
                int sentencebegin =1955;
                int sentenceend= 1956;
                String big = "";

                for (int i = 0; i < strs.Length; i++)
                {
                    temp.Add(Convert.ToInt32(strs[i]));

                 
                }
                testData.Add(temp);
                

            }
            sr.Close();

       
            StreamReader sr1 = new StreamReader(tag_path);
            String line1;
            while ((line1 = sr1.ReadLine()) != null)
            {
                List<Matrix> temp = new List<Matrix>();
                string[] strs = line1.Split(' ');
                for (int i = 0; i < strs.Length; i++)
                {
                    int x = Convert.ToInt32(strs[i]);
                    if (x == 1)
                    {
                        temp.Add(new Matrix(new double[] { 1, 0, 0, 0 }));
                    }
                    else if (x == 2)
                    {
                        temp.Add(new Matrix(new double[] { 0, 1, 0, 0 }));
                    }
                    else if (x == 3)
                    {
                        temp.Add(new Matrix(new double[] { 0, 0, 1, 0 }));
                    }
                    else if (x == 4)
                    {
                        temp.Add(new Matrix(new double[] { 0, 0, 0, 1 }));
                    }
                  
                }
                testLabel.Add(temp);

            }
            sr1.Close();
        }


   

        //将训练数据变成模型的输入
        public List<DataSeq> GetTrainingData(double scale)
        {
            List<DataSeq> result = new List<DataSeq>();
            int index = 0;
            for (int i = 0; i < (int)trainData.Count * scale; i++)
            {
                DataSeq tempSeq = new DataSeq();
                for (int j = 0; j < trainData[i].Count; j++)
                {

                        List<int> temp = new List<int>();
                        temp.Add(j - 2 >= 0 ? trainData[i][j - 2] : num - 1);
                        temp.Add(j - 1 >= 0 ? trainData[i][j - 1] : num - 1);
                        temp.Add(trainData[i][j]);
                        temp.Add(j + 1 < trainData[i].Count ? trainData[i][j + 1] : num - 1);
                        temp.Add(j + 2 < trainData[i].Count ? trainData[i][j + 2] : num - 1);
                        tempSeq.datasteps.Add(new DataStep(temp, trainLabel[i][j], trainData[i][j]));
                        index++;

                    

                }
                result.Add(tempSeq);
            }
            return result;
        }


        public List<DataSeq> GetTestData(double scale=1)
        {
            List<DataSeq> result = new List<DataSeq>();
            int index = 0;
            for (int i = 0; i < (int)testData.Count * scale; i++)
            {
                DataSeq tempSeq = new DataSeq();
                for (int j = 0; j < testData[i].Count; j++)
                {
  


                        List<int> temp = new List<int>();
                        temp.Add(j - 2 >= 0 ? testData[i][j - 2] : num - 1);
                        temp.Add(j - 1 >= 0 ? testData[i][j - 1] : num - 1);
                        temp.Add(testData[i][j]);
                        temp.Add(j + 1 < testData[i].Count ? testData[i][j + 1] : num - 1);
                        temp.Add(j + 2 < testData[i].Count ? testData[i][j + 2] : num - 1);
                        tempSeq.datasteps.Add(new DataStep(temp, testLabel[i][j], testData[i][j]));
                        index++;

                    


                    
                }
                result.Add(tempSeq);
            }
            return result;
        }
    }
}
