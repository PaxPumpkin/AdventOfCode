using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;

namespace AoC_2016
{
    class Day17_TwoStepsForward : AoCodeModule
    {
        MD5 md5 = System.Security.Cryptography.MD5.Create();
        char[] unlocked = new char[] { 'b', 'c', 'd', 'e', 'f' };
        List<string> foundPaths = new List<string>();
        List<string> activeTraversals = new List<string>();
        public Day17_TwoStepsForward()
        {
            inputFileName = @"InputFiles\" + this.GetType().Name + ".txt";
            GetInput(); 
            OutputFileReset();
        }
        public override void DoProcess()
        {

            ResetProcessTimer(true);
            string input = "ulqzkmiv";// "ihgpwlah"; // 
            input = "njfxhljp";
            int currentx = 0, currenty = 0;
            Tuple<string, bool> result = Traverse(input,currentx,currenty);
            AddResult("**********************************");
            AddResult(foundPaths.OrderBy(path=>path.Length).ToList().First());
            ResetProcessTimer(true);
            AddResult((foundPaths.OrderByDescending(path => path.Length).ToList().First().Length-input.Length).ToString());

        }
        public Tuple<string,bool> Traverse(string input, int toX, int toY)
        {
            if (toX == 3 && toY == 3) { foundPaths.Add(input); return new Tuple<string, bool>(input, true); } // we made it!!!
            if (activeTraversals.Contains(input.Substring(input.Length - 1) + toX.ToString() + toY.ToString())) return new Tuple<string, bool>(input, false);

            Tuple<string, bool> result = new Tuple<string, bool>(input, false);

            string hash = CalculateMD5Hash(input).Substring(0,4);
            List<string> possibilities = new List<string>();
            if (unlocked.Contains(hash[0]) && toY > 0) possibilities.Add("U");
            if (unlocked.Contains(hash[1]) && toY < 3) possibilities.Add("D");
            if (unlocked.Contains(hash[2]) && toX > 0) possibilities.Add("L");
            if (unlocked.Contains(hash[3]) && toX < 3) possibilities.Add("R");

            for (int x = 0; x < possibilities.Count; x++)
            {
                if (possibilities[x] == "U")
                {
                    result = Traverse(input + "U", toX, toY-1);
                }
                else if (possibilities[x] == "D")
                {
                    result = Traverse(input + "D", toX, toY+1);
                }
                else if (possibilities[x] == "L")
                {
                    result = Traverse(input + "L", toX-1, toY);
                }
                else // =="R"
                {
                    result = Traverse(input + "R", toX+1, toY);
                }
            }


            return result;
        }
        public string CalculateMD5Hash(string input)
        {
            byte[] inputBytes = System.Text.Encoding.UTF8.GetBytes(input);
            byte[] hash = md5.ComputeHash(inputBytes);
            return BitConverter.ToString(hash).Replace("-", "").ToLower();
        }
    }
}
