using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.IO;

namespace BunnyHeadquarters
{
    class AoCodeShell
    {
        static Assembly ass = Assembly.GetExecutingAssembly();
        static List<Type> thingy = ass.GetTypes().Where(c => c.BaseType.Name.Equals("AoCodeModule")).ToList();
        static void Main(string[] args)
        {
            bool loopit = true;
            while (loopit)
            {
                OutputPrompt();
                string command = Console.ReadLine();
                if (command.Trim().Equals(""))
                    loopit = false;
                else
                {
                    int moduleNumber;
                    if (Int32.TryParse(command, out moduleNumber))
                    {
                        AoCodeModule codeModule = (AoCodeModule)(Activator.CreateInstance(thingy[moduleNumber]));
                        codeModule.DoProcess();
                        Console.WriteLine("**************************************************************");
                        Console.WriteLine("**************************************************************");
                        codeModule.FinalOutput.ForEach(x => Console.WriteLine("** > " + x));
                        if (codeModule.StatusOutput.Count>0)
                        {
                            Console.WriteLine("**************************************************************");
                            codeModule.StatusOutput.ForEach(x=>Console.WriteLine(x));
                        }
                        Console.WriteLine("**************************************************************");
                        Console.WriteLine("**************************************************************");
                    }
                    else
                    {
                        Console.WriteLine("");
                        Console.WriteLine("That is not valid input. Try again, bucko.");
                        Console.WriteLine("");
                    }
                    
                }
            }
            
        }
        private static void OutputPrompt()
        {
            foreach (Type x in thingy)
            {
                Console.WriteLine(thingy.IndexOf(x).ToString() + " - " + x.Name);
            }
            Console.WriteLine("Enter Module Number or nothing to exit:");
        }
    }
    class AoCodeModule
    {
        public List<string> StatusOutput = new List<string>(); // internal processing only
        protected string path = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.FullName + @"\";
        protected string inputFileName = "nothere.txt";
        protected List<string> inputFile = new List<string>();
        protected string oneLineData = "";
        protected readonly static bool FileOutputOnly = true;
        protected readonly static bool FileOutputAlso = false;
        public List<string> FinalOutput = new List<string>();
        public AoCodeModule()
        {
        }
        public virtual void GetInput()
        {
            inputFile = new List<string>();
            try
            {
                StreamReader sr = new StreamReader(path + inputFileName);
                while (!sr.EndOfStream)
                {
                    inputFile.Add(sr.ReadLine());
                }
                sr.Close();
            }
            catch (Exception ex) { StatusOutput.Add( "************* Error happened during data read:" + ex.Message); }
        }
        public virtual void DoProcess()
        {
            FinalOutput.Add("YOU DIDN'T OVERRIDE THE DOPROCESS METHOD OR YOU CALLED THE BASE INSTANCE.");

        }
        public void Print(string line) {
            Print(line, null);
        }
        public void Print(string line, bool? FileOutputFlag){
            if (!FileOutputFlag.HasValue || FileOutputFlag.Value == FileOutputAlso)
            {
                Console.WriteLine(line);
            }
            if (FileOutputFlag.HasValue)
            {
                try
                {
                    string outputfilename = path + inputFileName + ".output.txt";
                    StreamWriter sw = new StreamWriter(outputfilename, true);
                    sw.WriteLine(line);
                    sw.Close();
                }
                catch (Exception ex) { StatusOutput.Add("************* Error happened during data print to file:" + ex.Message); }
            }

        }
        public void OutputFileReset() {
            try
            {
                string outputfilename = path + inputFileName + ".output.txt";
                File.Delete(outputfilename);
                StreamWriter sw = new StreamWriter(outputfilename, true);
                sw.Close();
            }
            catch (Exception ex)
            {
                StatusOutput.Add("************* Error happened during output file reset:" + ex.Message);
            }
        }
    }

}
