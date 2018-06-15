using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace Program
{
    [Serializable]
    public class LSTMLayer
    {
        int _inputDim;
        int _hiddenDim;

        public Matrix _wix;
        public Matrix _wih;
        public Matrix _iBias;
        public Matrix _wfx;
        public Matrix _wfh;
        public Matrix _fBias;
        public Matrix _wox;
        public Matrix _woh;
        public Matrix _Bias;
        public Matrix _wcx;
        public Matrix _wch;
        public Matrix _cBias;

        ActivateFunc _activate = new ActivateFunc();



        public static void SerializeWordembedding(string path)
        {
            FileStream fs = new FileStream(path, FileMode.Create);
            BinaryFormatter bf = new BinaryFormatter();
            bf.Serialize(fs, Global.wordEmbedding);
            fs.Close();

        }

        //public static void newSaveembedding(string path)
        //{
        //    List<double[]> words = new List<double[]>();
        //    for (int i = 0; i < Global.word.Count; i++)
        //    {
        //        words.Add(Global.wordEmbedding[i].saveMatrix());
        //    }
        //    string Filepath = path;
        //    FileStream fs = new FileStream(Filepath, FileMode.Create);
        //    BinaryFormatter sl = new BinaryFormatter();
        //    sl.Serialize(fs, words);
        //    fs.Close();
        //}

        public static Matrix[] getSerializeWordembedding(string path, Matrix[] wordEmbedding)
        {
            FileStream fs = new FileStream(path, FileMode.Open);
            BinaryFormatter bf = new BinaryFormatter();
            wordEmbedding = bf.Deserialize(fs) as Matrix[];
            fs.Close();
            return wordEmbedding;
        }



        public void saveLSTM(string path)
        {
            string filePath = path;
            FileStream fs = new FileStream(filePath, FileMode.Create);
            BinaryFormatter sl = new BinaryFormatter();
            sl.Serialize(fs, this);
            fs.Close();
        }


        //public void newSaveLSTM(string path, LSTMLayer ps)
        //{

        //    Dictionary<string, double[]> saveDic = new Dictionary<string, double[]>();
        //    saveDic.Add("_wix", ps._wix.saveMatrix());
        //    saveDic.Add("_wih", ps._wih.saveMatrix());
        //    saveDic.Add("_iBias", ps._iBias.saveMatrix());
        //    saveDic.Add("_wfx", ps._wfx.saveMatrix());
        //    saveDic.Add("_wfh", ps._wfh.saveMatrix());
        //    saveDic.Add("_fBias", ps._fBias.saveMatrix());
        //    saveDic.Add("_wox", ps._wox.saveMatrix());
        //    saveDic.Add("_woh", ps._woh.saveMatrix());
        //    saveDic.Add("_Bias", ps._Bias.saveMatrix());
        //    saveDic.Add("_wcx", ps._wcx.saveMatrix());
        //    saveDic.Add("_wch", ps._wch.saveMatrix());
        //    saveDic.Add("_cBias", ps._cBias.saveMatrix());
        //    string Filepath = path;
        //    FileStream fs = new FileStream(Filepath, FileMode.Create);
        //    BinaryFormatter sl = new BinaryFormatter();
        //    sl.Serialize(fs, saveDic);
        //    fs.Close();

        //}
        //public static LSTMLayer readLSTM(string path)
        //{
        //    LSTMLayer ps = new LSTMLayer();
        //    FileStream fs = new FileStream(path, FileMode.Open);
        //    BinaryFormatter bf = new BinaryFormatter();
        //    Dictionary<string, double[]> savemodel = bf.Deserialize(fs) as Dictionary<string, double[]>;
        //    foreach(string k in savemodel.Keys)
        //    {
        //        if (k == "_wix")
        //            ps._wix.W = savemodel[k];
        //        else if (k == "_wih")
        //            ps._wih.W = savemodel[k];
        //        else if (k == "_iBias")
        //            ps._iBias.W = savemodel[k];
        //        else if (k == "_wfx")
        //            ps._wfx.W = savemodel[k];
        //        else if (k == "_wfh")
        //            ps._wfh.W = savemodel[k];
        //        else if (k == "_fBias")
        //            ps._fBias.W = savemodel[k];
        //        else if (k == "_wox")
        //            ps._wox.W = savemodel[k];
        //        else if (k == "_woh")
        //            ps._woh.W = savemodel[k];
        //        else if (k == "_Bias")
        //            ps._Bias.W = savemodel[k];
        //        else if (k == "_wcx")
        //            ps._wcx.W = savemodel[k];
        //        else if (k == "_wch")
        //            ps._wch.W = savemodel[k];
        //        else if (k == "_cBias")
        //            ps._cBias.W = savemodel[k];


        //    }
        //    return ps;
        //}
        public static LSTMLayer readLSTM(string path)
        {
            FileStream fs = new FileStream(path, FileMode.Open);
            BinaryFormatter bf = new BinaryFormatter();
            LSTMLayer ps = bf.Deserialize(fs) as LSTMLayer;
            fs.Close();
            return ps;
        }
        public LSTMLayer(double upbound = Global.upbound)
        {
            this._inputDim = Global.inputDim;
            this._hiddenDim = Global.hiddenDim;



            _wix = Matrix.newMatrix_randomorthor(_hiddenDim, _inputDim, upbound);
            _wih = Matrix.newMatrix_randomorthor(_hiddenDim, _hiddenDim, upbound);
            _iBias = new Matrix(_hiddenDim);

            _wfx = Matrix.newMatrix_randomorthor(_hiddenDim, _inputDim, upbound);
            _wfh = Matrix.newMatrix_randomorthor(_hiddenDim, _hiddenDim, upbound);
            //set forget bias to 1.0, as described here: http://jmlr.org/proceedings/papers/v37/jozefowicz15.pdf
            _fBias = Matrix.newMatrix_1(_hiddenDim, 1);

            _wox = Matrix.newMatrix_randomorthor(_hiddenDim, _inputDim, upbound);
            _woh = Matrix.newMatrix_randomorthor(_hiddenDim, _hiddenDim, upbound);
            _Bias = new Matrix(_hiddenDim);

            _wcx = Matrix.newMatrix_randomorthor(_hiddenDim, _inputDim, upbound);
            _wch = Matrix.newMatrix_randomorthor(_hiddenDim, _hiddenDim, upbound);
            _cBias = new Matrix(_hiddenDim);
        }


        public List<Matrix> activate(DataStep x, ForwdBackwdProp g)
        {
            List<int> input = x.inputs;
            List<int> bigram = x.bigram;


            Matrix final = new Matrix(Global.hiddenDim, 1);
            List<Matrix> outputs = new List<Matrix>();
            Matrix _h_tm1 = Matrix.newMatrix_0(_hiddenDim, 1);
            Matrix _s_tm1 = Matrix.newMatrix_0(_hiddenDim, 1);

            for (int i = 0; i < input.Count; i++)
            {
                //input gate
                //Matrix con = g.ConcatVectors(Global.wordEmbedding[input[i]], Global.BigramwordEmbedding[bigram[i]]);
                //Matrix inputt = g.tanhNonlin(g.Mul(_hw, con));
                Matrix inputt = Global.wordEmbedding[input[i]];
                Matrix sum0 = g.Mul(_wix, inputt);

                Matrix sum1 = g.Mul(_wih, _h_tm1);
                Matrix inputGate = g.sigNonlin(g.Add(g.Add(sum0, sum1), _iBias));

                //forget gate
                Matrix sum2 = g.Mul(_wfx, inputt);
                Matrix sum3 = g.Mul(_wfh, _h_tm1);
                Matrix forgetGate = g.sigNonlin(g.Add(g.Add(sum2, sum3), _fBias));

                //output gate
                Matrix sum4 = g.Mul(_wox, inputt);
                Matrix sum5 = g.Mul(_woh, _h_tm1);
                Matrix outputGate = g.sigNonlin(g.Add(g.Add(sum4, sum5), _Bias));

                //write operation on cells
                Matrix sum6 = g.Mul(_wcx, inputt);
                Matrix sum7 = g.Mul(_wch, _h_tm1);
                Matrix cellInput = g.tanhNonlin(g.Add(g.Add(sum6, sum7), _cBias));

                //compute new cell activation
                Matrix retainCell = g.Elmul(forgetGate, _s_tm1);
                Matrix writeCell = g.Elmul(inputGate, cellInput);
                Matrix cellAct = g.Add(retainCell, writeCell);

                //compute hidden state as gated, saturated cell activations
                Matrix output = g.Elmul(outputGate, g.tanhNonlin(cellAct));
                //if (i == 0)
                //{
                //    final = output;
                //}
                //else
                //{
                //    final = g.ConcatVectors(final, output);
                //}
                //final = g.Add(final, output);

                outputs.Add(output);
                //rollover activations for next iteration
                _h_tm1 = output;
                _s_tm1 = cellAct;
                //_h = g.Add(output, _h);
            }

            return outputs;
        }
        public List<Matrix> activate(List<Matrix> x, ForwdBackwdProp g)
        {
            // List<int> input = x.inputs;
            Matrix final = new Matrix(Global.hiddenDim, 1);
            List<Matrix> outputs = new List<Matrix>();
            Matrix _h_tm1 = Matrix.newMatrix_0(_hiddenDim, 1);
            Matrix _s_tm1 = Matrix.newMatrix_0(_hiddenDim, 1);

            for (int i = 0; i < x.Count; i++)
            {
                //input gate
                Matrix sum0 = g.Mul(_wix, x[i]);
                Matrix sum1 = g.Mul(_wih, _h_tm1);
                Matrix inputGate = g.sigNonlin(g.Add(g.Add(sum0, sum1), _iBias));

                //forget gate
                Matrix sum2 = g.Mul(_wfx, x[i]);
                Matrix sum3 = g.Mul(_wfh, _h_tm1);
                Matrix forgetGate = g.sigNonlin(g.Add(g.Add(sum2, sum3), _fBias));

                //output gate
                Matrix sum4 = g.Mul(_wox, x[i]);
                Matrix sum5 = g.Mul(_woh, _h_tm1);
                Matrix outputGate = g.sigNonlin(g.Add(g.Add(sum4, sum5), _Bias));

                //write operation on cells
                Matrix sum6 = g.Mul(_wcx, x[i]);
                Matrix sum7 = g.Mul(_wch, _h_tm1);
                Matrix cellInput = g.tanhNonlin(g.Add(g.Add(sum6, sum7), _cBias));

                //compute new cell activation
                Matrix retainCell = g.Elmul(forgetGate, _s_tm1);
                Matrix writeCell = g.Elmul(inputGate, cellInput);
                Matrix cellAct = g.Add(retainCell, writeCell);

                //compute hidden state as gated, saturated cell activations
                Matrix output = g.Elmul(outputGate, g.tanhNonlin(cellAct));
                //if (i == 0)
                //{
                //    final = output;
                //}
                //else
                //{
                //    final = g.ConcatVectors(final, output);
                //}
                //final = g.Add(final, output);

                outputs.Add(output);
                //rollover activations for next iteration
                _h_tm1 = output;
                _s_tm1 = cellAct;
                //_h = g.Add(output, _h);
            }

            return outputs;
        }

        public List<Matrix> GetParameters()
        {
            List<Matrix> result = new List<Matrix>();
            result.Add(_wix);
            result.Add(_wih);
            result.Add(_iBias);
            result.Add(_wfx);
            result.Add(_wfh);
            result.Add(_fBias);
            result.Add(_wox);
            result.Add(_woh);
            result.Add(_Bias);
            result.Add(_wcx);
            result.Add(_wch);
            result.Add(_cBias);
            return result;
        }
    }
}
