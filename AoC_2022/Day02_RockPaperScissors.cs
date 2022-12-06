using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AoC_2022
{
    class Day02_RockPaperScissors : AoCodeModule
    {
        public Day02_RockPaperScissors()
        {
            inputFileName = @"InputFiles\" + this.GetType().Name + ".txt";
            GetInput(); 
            OutputFileReset();
        }
        public override void DoProcess()
        {
            //** > Result for Day02_RockPaperScissors part 1: Strategy Guide Score Result is 13009 (Process: 0.5826 ms)
            //** > Result for Day02_RockPaperScissors part 1: Revised Strategy Guide Score Result is 10398 (Process: 0.8743 ms)

            string finalResult = "Not Set";
            ResetProcessTimer(true);
            List<string[]> allMoves = new List<string[]>();
            foreach (string processingLine in inputFile)
            {
                allMoves.Add(processingLine.Trim().Split(new char[] { ' ' }));

            }
            int totalScore = 0;
            allMoves.ForEach(move => { totalScore += RockPaperScissorsEngine.Score(move[0], move[1]); });
            finalResult = "Strategy Guide Score Result is " + totalScore.ToString();
            AddResult(finalResult);

            totalScore = 0;
            allMoves.ForEach(move => { totalScore += RockPaperScissorsEngine.Score(move[0], RockPaperScissorsEngine.DetermineMoveForResult(move[0], move[1])); });
            finalResult = "Revised Strategy Guide Score Result is " + totalScore.ToString();
            AddResult(finalResult);
        }
    }
    public class RockPaperScissorsEngine
    {
        const string rock = "A", paper = "B", scissors = "C";
        static string myRock = "X", myPaper = "Y", myScissors = "Z";
        const string lose = "X", draw = "Y", win = "Z";
        static int rockScore = 1, paperScore = 2, scissorsScore = 3;
        static int winScore = 6, drawScore = 3, loseScore = 0;
        public static int BaseScore(string myThrow)
        {
            return (myThrow == myRock ? rockScore : (myThrow == myPaper ? paperScore : scissorsScore));
        }
        public static int Score(string opponentThrow, string myThrow)
        {
            int thisScore = BaseScore(myThrow);
            switch(opponentThrow)
            {
                case rock:
                    thisScore += (myThrow == myRock ? drawScore : (myThrow == myPaper ? winScore : loseScore));
                    break;
                case paper:
                    thisScore += (myThrow == myRock ? loseScore : (myThrow == myPaper ? drawScore : winScore));
                    break;
                case scissors:
                    thisScore += (myThrow == myRock ? winScore : (myThrow == myPaper ? loseScore : drawScore));
                    break;
            }
            return thisScore;
        }
        public static string DetermineMoveForResult(string opponentThrow, string requiredResult)
        {
            string requiredMove = "";
            switch(requiredResult)
            {
                case lose: 
                    requiredMove = (opponentThrow == rock ? myScissors : (opponentThrow == paper ? myRock : myPaper ));
                    break;
                case draw: 
                    requiredMove = (opponentThrow == rock ? myRock : (opponentThrow == paper ? myPaper : myScissors));
                    break;
                case win: 
                    requiredMove = (opponentThrow == rock ? myPaper : (opponentThrow == paper ? myScissors : myRock));
                    break;
            }
            return requiredMove;
        }
    }
}
