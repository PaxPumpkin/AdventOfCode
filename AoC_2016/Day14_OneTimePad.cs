using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;

namespace AoC_2016
{
    class Day14_OneTimePad : AoCodeModule
    {

        MD5 md5 = System.Security.Cryptography.MD5.Create();
        public Day14_OneTimePad()
        {
            inputFileName = @"InputFiles\" + this.GetType().Name + ".txt";
            GetInput(); //base class method
            OutputFileReset();

        }
        public override void DoProcess()
        {
            string puzzleInput = "qzyelonm";
            //string puzzleInput = "abc";
            ResetProcessTimer(true);
            List<string> foundKeys = new List<string>();
           
            int index = 0;
            string test = "";
            while (foundKeys.Count < 64)
            {
                test = CalculateMD5Hash(puzzleInput + index.ToString());
                Tuple<bool, string> testResult = ContainsTriplet(test);
                if (testResult.Item1)
                {
                    //Console.WriteLine("Found potential at " + index.ToString());
                    for (int x = 1; x <= 1000; x++)
                    {
                        if (ContainsAFiver(CalculateMD5Hash(puzzleInput + (index + x).ToString()), testResult.Item2))
                        {
                            foundKeys.Add(test);
                            //Print("Found key " + foundKeys.Count.ToString() + " at index " + index.ToString() + " at index:" + (x+index).ToString() + " key(" + test + ") contains 3 " + testResult.Item2 + " and " + CalculateMD5Hash(puzzleInput + (index + x).ToString()) + " contains 5");
                            break;
                        }
                    }
                    if (!foundKeys.Contains(test))
                    {
                        //Print("key " + test + " wasn't found to have a 5 from " + (index + 1).ToString() + " through " + (index + 1000).ToString());
                    }
                }
                index++;
            }
            index--;
            AddResult(index.ToString());
            ResetProcessTimer(true);
            Print("GO");
            foundKeys.Clear();
            index = 0; test = "";
            int hashCount = 0;
            LinkedList<string> hashes = new LinkedList<string>();
            hashCount += GetNextSetOfHashes(puzzleInput, hashCount, 12000, hashes);


            LinkedListNode<string> testNode = hashes.First;
            LinkedListNode<string> compNode;
            while (foundKeys.Count < 64)
            {
                test = testNode.Value;
                Tuple<bool, string> testResult = ContainsTriplet(test);
                if (testResult.Item1)
                {
                    //Console.WriteLine("Found potential at " + index.ToString());
                    compNode = testNode;
                    for (int x = 1; x <= 1000; x++)
                    {
                        if (compNode.Next == null) { hashCount += GetNextSetOfHashes(puzzleInput, hashCount, 1000, hashes); }
                        compNode = compNode.Next;

                        if (ContainsAFiver(compNode.Value, testResult.Item2))
                        {
                            foundKeys.Add(test);
                            //Print("Found key " + foundKeys.Count.ToString() + " at index " + index.ToString() + " at index:" + (x+index).ToString() + " key(" + test + ") contains 3 " + testResult.Item2 + " and " + CalculateMD5Hash(puzzleInput + (index + x).ToString()) + " contains 5");
                            break;
                        }
                    }
                }
                index++;
                if (testNode.Next == null) { hashCount += GetNextSetOfHashes(puzzleInput, hashCount, 1000, hashes); }
                testNode = testNode.Next;
                if (index % 500 == 0) { Print("Index " + index.ToString()); }
            }
            index--;
            AddResult(index.ToString());

        }
        public int GetNextSetOfHashes(string puzzleInput, int fromIndex, int qty, LinkedList<string> hashes)
        {
            Print("Loading " + qty.ToString() + " hashes...");
            string pi = "";
            for (int index = fromIndex; index < (fromIndex + qty); index++)
            {
                pi = CalculateMD5Hash(puzzleInput + index.ToString());
                for (int x = 0; x < 2016; x++) { pi = CalculateMD5Hash(pi); }
                hashes.AddLast(pi);
            }
            Print("...ok");
            return qty;
        }
        public Tuple<bool,string> ContainsTriplet(string test) {
            int index = 0;
            bool result = false;
            string character = "";
            int atPosition = test.Length;
            while (index < test.Length )//&& result == false)
            {
                string testString = new string(test[index], 3);
                int indexOf = test.IndexOf(testString);
                //if (test.Contains(test) )//&& !test.Contains(new string(test[index], 4)))
                if (indexOf>-1 && indexOf<atPosition)
                {
                    result = true;
                    character = test[index].ToString();
                    atPosition = indexOf;
                }
                index++;
            }

            return new Tuple<bool, string>(result, character);
        }
        public bool ContainsAFiver(string hash, string character) {
            return hash.Contains(new string(character[0], 5));
        }
        public string CalculateMD5Hash(string input)
        {
            byte[] inputBytes = System.Text.Encoding.UTF8.GetBytes(input);
            byte[] hash = md5.ComputeHash(inputBytes);
            return BitConverter.ToString(hash).Replace("-","").ToLower();
        }
    }
}
