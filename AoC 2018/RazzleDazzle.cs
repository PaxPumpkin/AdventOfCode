using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AoC_2018
{
    class RazzleDazzle : AoCodeModule
    {
        public RazzleDazzle()
        {
            // If you always save input file in the /InputFiles/ subfolder and name it the same as the class processing it, this will work.
            // if you put it elsewhere or name it differently, just change below. 
            inputFileName = @"InputFiles\" + this.GetType().Name + ".txt";
            GetInput(); //base class method
            OutputFileReset(); // output file will be in the same location as input, with ".output.txt" appended to the name. This clears previous output from file.
            //Print("Did Something");// outputs to console immediately
            //Print("DidSomethingElse", FileOutputAlso); // both console and output file
            //Print("Another Thing", FileOutputOnly); // output file only.
        }
        public override void DoProcess()
        {
            //If Comma Delimited on a single input line
            //List<string> inputItems = inputFile[0].Split(new char[] { ',' }).ToList<string>();
            string finalResult = "Not Set";
            ResetProcessTimer(true);// true also iterates the section marker
            RazzBoard myBoard = new RazzBoard();
            foreach (string processingLine in inputFile)
            {
                for (int x = 0; x < processingLine.Length; x++)
                {
                    myBoard.sections.Add(new RazzSection() { selected = false, value = Int32.Parse(processingLine[x].ToString()) });
                }
            }
            while (myBoard.score < 100)
            {
                //myBoard.Roll();
                myBoard.SlightlyMoreFairRoll();
            }
            Console.WriteLine("It took " + myBoard.totalRolls.ToString() + " rolls to win! SUCKER! ");
            AddResult(finalResult); // includes elapsed time from last ResetProcessTimer

        }
    }
    public class RazzBoard
    {
        public List<RazzSection> sections = new List<RazzSection>();
        public long currentBet = 1;
        public int score = 0;
        public long totalBet =0;
        public long potentialPrizes = 1;
        public long totalRolls = 0;
        private Random random = new Random();
        public void Roll()
        {
            totalBet += currentBet; // get their money first! Ha!
            totalRolls++;
            sections.Where(x => x.selected == true).ToList().ForEach(x => x.selected = false);
            for (int x = 0; x < 8; x++)
            {
                int marble = random.Next(sections.Count);
                while (sections[marble].selected == true) { marble = random.Next(sections.Count); }
                // we now have an unselected section. 
                sections[marble].selected = true;
            }
            // we now have 8 selected spaces, unique. Total the current roll value.
            // note, the board we're using supposedly has a modified odds-curve versus just rolling 8 random six-digit numbers like dice. 
            int value = 0;
            sections.Where(x => x.selected == true).ToList().ForEach(x=>value += x.value);
            // and now, what happens?
            switch (value) {
                case 29:
                    potentialPrizes++;
                    currentBet *= 2;
                    Console.WriteLine("OOPS! Doubles the Bet! Extra Prize!");
                    break;
                case int n when (n >= 22 && n <= 34 && n != 29):
                    Console.WriteLine("No Score.");
                    break;
                case int n when ((n >= 18 && n <= 21) || (n >= 35 && n <= 38)):
                    Console.WriteLine("Adds a prize!");
                    potentialPrizes++;
                    break;
                case int n when (n == 8 || n == 9 || n == 47 || n == 48):
                    Console.WriteLine("WINNER!!!! 100 Points!");
                    score += 100;
                    break;
                case int n when (n == 10 || n == 12 || n == 13 || n == 43 || n == 44 || n == 46):
                    Console.WriteLine("Win! 50 Points!");
                    score += 50;
                    break;
                case int n when (n == 11 || n == 45):
                    Console.WriteLine("Win! 30 Points!");
                    score += 30;
                    break;
                case int n when (n == 14 || n == 42):
                    Console.WriteLine("Win! 20 Points!");
                    score += 20;
                    break;
                case int n when (n == 15 || n == 41):
                    Console.WriteLine("Win! 15 Points!");
                    score += 15;
                    break;
                case int n when (n == 16):
                    Console.WriteLine("Win! 10 Points!");
                    score += 10;
                    break;
                case int n when (n == 17 || n == 39 || n==40):
                    Console.WriteLine("Win! 5 Points!");
                    score += 5;
                    break;
                default:
                    Console.WriteLine("UNCOVERED CONDITION LOCATED! VALUE: " + value.ToString() + " ******************************");
                    break;
            }
            Console.WriteLine("Rolled: " + value.ToString() + "; Score: " + score.ToString() + "; Prizes: " + potentialPrizes.ToString() + "; Each Bet Now: " + currentBet.ToString() + "; Total Spent: " + totalBet.ToString());

        }
        public void SlightlyMoreFairRoll()
        {
            totalBet += currentBet; // get their money first! Ha!
            totalRolls++;
            // we now have 8 selected spaces, unique. Total the current roll value.
            // note, the board we're using supposedly has a modified odds-curve versus just rolling 8 random six-digit numbers like dice. 
            int value = 0;
            for (int x = 0; x < 8; x++)
            {
                value += random.Next(1,7); 
            }
            
            // and now, what happens?
            switch (value)
            {
                case 29:
                    potentialPrizes++;
                    currentBet *= 2;
                    Console.WriteLine("OOPS! Doubles the Bet! Extra Prize!");
                    break;
                case int n when (n >= 22 && n <= 34 && n != 29):
                    Console.WriteLine("No Score.");
                    break;
                case int n when ((n >= 18 && n <= 21) || (n >= 35 && n <= 38)):
                    Console.WriteLine("Adds a prize!");
                    potentialPrizes++;
                    break;
                case int n when (n == 8 || n == 9 || n == 47 || n == 48):
                    Console.WriteLine("WINNER!!!! 100 Points!");
                    score += 100;
                    break;
                case int n when (n == 10 || n == 12 || n == 13 || n == 43 || n == 44 || n == 46):
                    Console.WriteLine("Win! 50 Points!");
                    score += 50;
                    break;
                case int n when (n == 11 || n == 45):
                    Console.WriteLine("Win! 30 Points!");
                    score += 30;
                    break;
                case int n when (n == 14 || n == 42):
                    Console.WriteLine("Win! 20 Points!");
                    score += 20;
                    break;
                case int n when (n == 15 || n == 41):
                    Console.WriteLine("Win! 15 Points!");
                    score += 15;
                    break;
                case int n when (n == 16):
                    Console.WriteLine("Win! 10 Points!");
                    score += 10;
                    break;
                case int n when (n == 17 || n == 39 || n == 40):
                    Console.WriteLine("Win! 5 Points!");
                    score += 5;
                    break;
                default:
                    Console.WriteLine("UNCOVERED CONDITION LOCATED! VALUE: " + value.ToString() + " ******************************");
                    break;
            }
            Console.WriteLine("Rolled: " + value.ToString() + "; Score: " + score.ToString() + "; Prizes: " + potentialPrizes.ToString() + "; Each Bet Now: " + currentBet.ToString() + "; Total Spent: " + totalBet.ToString());

        }
    }
    public class RazzSection
    {
        public int value = 0;
        public bool selected = false;
    }
}
