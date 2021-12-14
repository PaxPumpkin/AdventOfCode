using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.IO;

namespace AoC_2018
{
    class AoCodeShell
    {
        public static Assembly ass = Assembly.GetExecutingAssembly();
        public static List<Type> thingy = ass.GetTypes().Where(c => c.BaseType != null && c.BaseType.Name.Equals("AoCodeModule")).ToList();
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
                        if (moduleNumber < 1 || moduleNumber > thingy.Count)
                        {
                            Console.WriteLine("");
                            Console.WriteLine("That is not valid input. Try again, bucko.");
                            Console.WriteLine("");
                        }
                        else
                        {
                            AoCodeModule codeModule = (AoCodeModule)(Activator.CreateInstance(thingy[moduleNumber-1]));
                            codeModule.StartMainTimer();
                            codeModule.DoProcess();
                            Console.WriteLine("**************************************************************");
                            Console.WriteLine("**************************************************************");
                            codeModule.FinalOutput.ForEach(x => Console.WriteLine("** > " + x));
                            Console.WriteLine(codeModule.TotalTime);
                            if (codeModule.StatusOutput.Count > 0)
                            {
                                Console.WriteLine("**************************************************************");
                                codeModule.StatusOutput.ForEach(x => Console.WriteLine(x));
                            }
                            Console.WriteLine("**************************************************************");
                            Console.WriteLine("**************************************************************");
                        }
                    }
                    else
                    {
                        Console.WriteLine("");
                        Console.WriteLine("That is not valid input. Try again, bucko.");
                        Console.WriteLine("");
                    }
                    Console.WriteLine("Press Enter to reload module list:");
                    command = Console.ReadLine();
                }
            }

        }
        private static void OutputPrompt()
        {
            foreach (Type x in thingy)
            {
                Console.WriteLine((thingy.IndexOf(x) + 1).ToString() + " - " + x.Name);
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
        private DateTime MainTimer;
        private DateTime ProcessTimer;
        private int partIterator = 0;
        public string ProcessTime
        {
            get
            {
                return "(Process: " + ((int)(DateTime.Now - ProcessTimer).TotalMilliseconds).ToString() + " ms)";
            }
        }
        public string TotalTime
        {
            get
            {
                return "(Total: " + ((int)(DateTime.Now - MainTimer).TotalMilliseconds).ToString() + " ms)";
            }
        }
        public string Me { get { return this.GetType().Name; } }
        public AoCodeModule()
        {
        }
        public void StartMainTimer()
        {
            MainTimer = DateTime.Now;
            ResetProcessTimer(null);
        }
        public void ResetProcessTimer() { ResetProcessTimer(null); }
        public void ResetProcessTimer(bool? iteratePart)
        {
            if (iteratePart.HasValue && iteratePart.Value == true) { PartMarker(); };
            ProcessTimer = DateTime.Now;
        }
        public void PartMarker() { partIterator++; }
        public void AddResult(string result)
        {
            FinalOutput.Add("Result for " + Me + " part " + partIterator.ToString() + ":" + result + ProcessTime);
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
            catch (Exception ex) { StatusOutput.Add("************* Error happened during data read:" + ex.Message); }
        }
        public virtual void DoProcess()
        {
            FinalOutput.Add("YOU DIDN'T OVERRIDE THE DOPROCESS METHOD OR YOU CALLED THE BASE INSTANCE.");

        }
        public void Print(string line)
        {
            Print(line, null);
        }
        public void Print(string line, bool? FileOutputFlag)
        {
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
        public void OutputFileReset()
        {
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
