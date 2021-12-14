using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AoC_2018
{
    class Day14_ChocolateCharts : AoCodeModule
    {
        public Day14_ChocolateCharts()
        {
            inputFileName = @"InputFiles\" + this.GetType().Name + ".txt";
            GetInput(); 
            OutputFileReset();
        }
        public override void DoProcess()
        {
            //string finalResult = "Not Set";
            ResetProcessTimer(true);
            int input =  540561; // how many recipes to test
            input += 10; // we need ten more AFTER the input.
            StringBuilder recipeholder = new StringBuilder(input + 2); // max size of the string we'll build; each iteration could potentially add 1-2 chars.
            int elf1pointer = 0, elf2pointer = 1, elf1value = 0,elf2value=0 ;
            recipeholder.Append("37");
            while (recipeholder.Length < input)
            {
                elf1value = int.Parse(recipeholder[elf1pointer].ToString());
                elf2value = int.Parse(recipeholder[elf2pointer].ToString());
                recipeholder.Append((elf1value+elf2value).ToString());
                elf1pointer = (elf1pointer + 1 + elf1value) % recipeholder.Length;
                elf2pointer = (elf2pointer + 1 + elf2value) % recipeholder.Length;
            }
            string recipeList = recipeholder.ToString();
            string last10 = recipeList.Substring(input - 10, 10);
            AddResult("Score of last ten: " + last10.ToString());
            ResetProcessTimer(true);
            string lookingFor = (input - 10).ToString();
            //lookingFor = "59414";
            bool found = recipeList.IndexOf(lookingFor)>-1;
            LinkedList<string> last10New = new LinkedList<string>();
            for (int x = recipeList.Length-1; x >= recipeList.Length -6; x--)
            {
                last10New.AddFirst(recipeList[x].ToString());
            }
            string summat = "";
            while (!found)
            {
                elf1value = int.Parse(recipeholder[elf1pointer].ToString());
                elf2value = int.Parse(recipeholder[elf2pointer].ToString());
                summat= (elf1value + elf2value).ToString();
                recipeholder.Append(summat);
                elf1pointer = (elf1pointer + 1 + elf1value) % recipeholder.Length;
                elf2pointer = (elf2pointer + 1 + elf2value) % recipeholder.Length;
                last10New.AddLast(summat[0].ToString());
                last10New.RemoveFirst();
                if (summat.Length > 1) { last10New.AddLast(summat[1].ToString());  }
                found = GotIt2(lookingFor, last10New);
                if (summat.Length > 1)
                {
                    last10New.RemoveFirst();
                }
            }
            AddResult("Recipes to the left: " + recipeholder.ToString().IndexOf(lookingFor));
        }
        public bool GotIt(string lf, LinkedList<string> last10) {
            LinkedListNode<string> c = last10.First;
            bool failure = false;
            int pointer = 0;
            while (c != null && !failure && pointer<lf.Length)
            {
                if (!c.Value.Equals(lf[pointer].ToString())) failure = true;
                pointer++;
                c = c.Next;
            }
            return !failure;
        }
        public bool GotIt2(string lf, LinkedList<string> last10)
        {
            LinkedListNode<string> c = last10.First;
            //bool failure = false;
            string test = "";
            while (c != null )
            {
                test += c.Value;
                c = c.Next;
            }
            return test.IndexOf(lf) >= 0;
        }
    }
}
