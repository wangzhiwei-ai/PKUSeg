using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Seg
{
    class Global
    {


        public static string mode = "train";//  "train" or "test" or "predict";
        public static string code = "fast";


        // test mode
        public static string readFile = "data/pku_test.utf8";
        public static string outputFile = "data/pku_test_output.utf8";


        // train mode
        public static string trainFile = "data/pku_training.txt";
        public static string testFile = "data/pku_test_gold.utf8";
    }
}
