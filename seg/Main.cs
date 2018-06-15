using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Seg
{
    class Seg
    {
        static void Main(string[] args)
        {
            if (args.Length < 3)
            {
                Console.WriteLine("Wrong  inputs");
                Console.ReadLine();
                return;
            }
            else
            {
                if (!System.IO.File.Exists(args[1]))
                {
                    Console.WriteLine("Wrong  inputs");
                    Console.ReadLine();
                    return;
                }
                if (!System.IO.File.Exists(args[2]))
                {
                    Console.WriteLine("Wrong  inputs");
                    Console.ReadLine();
                    return;
                }
                if (args[0] == "train")
                {
                    Global.code = "train";
                    Global.trainFile = args[1];
                    Global.testFile = args[2];
                }
                else
                {
                    Global.code = "test";
                    Global.readFile = args[1];
                    Global.outputFile = args[2];
                }
            }
            if (!System.IO.File.Exists("data"))
            {
                System.IO.Directory.CreateDirectory("data");
            }
            if (!System.IO.File.Exists("data/temp"))
            {
                System.IO.Directory.CreateDirectory("data/temp");
            }

            if (!System.IO.File.Exists("data/deepNetwork"))
            {
                System.IO.Directory.CreateDirectory("data/deepNetwork");
            }

            if (!System.IO.File.Exists("model"))
            {
                System.IO.Directory.CreateDirectory("model");
            }
            if (!System.IO.File.Exists("model/deepNetwork"))
            {
                System.IO.Directory.CreateDirectory("model/deepNetwork");
            }
            if (!System.IO.File.Exists("model/crf"))
            {
                System.IO.Directory.CreateDirectory("model/crf");
            }

            if (Global.code == "soft")
            {

                if (Global.mode == "train")
                {
                    Program1.Global.runMode = "train";
                    Program1.Global.trainFile = Global.trainFile;
                    Program1.Global.testFile = Global.testFile;
                    
                }
                else
                {
                    Program1.Global.runMode = "test";
                    Program1.Global.readFile = Global.readFile;
                    Program1.Global.outputFile = Global.outputFile;
                }


                Program1.MainClass.Run();
            }
            else if (Global.code == "hard")
            {

                if (Global.mode == "train")
                {
                    Program.Global.mode = "train";
                    Program.Global.trainFile = Global.trainFile;
                    Program.Global.testFile = Global.testFile;
                    Program.Global.isRead = int.Parse(args[3]);

                }
                else
                {
                    Program.Global.mode = "test";
                    Program.Global.readFile = Global.readFile;
                    Program.Global.outputFile = Global.outputFile;
                }


                Program.Program.Run();
            }
        }
    }
}
