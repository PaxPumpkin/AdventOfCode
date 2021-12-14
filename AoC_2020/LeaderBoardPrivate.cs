using AoCLeaderBoardClasses;

namespace AoC_2020
{
    class zLeaderBoardPrivate : AoCodeModule
    {
        public zLeaderBoardPrivate()
        {
            inputFileName = @"InputFiles\" + this.GetType().Name + ".txt";
            GetInput(); 
            OutputFileReset(); 
        }
        string url = @"https://adventofcode.com/2020/leaderboard/private/view/197110.json";
        public override void DoProcess()
        {
            new LeaderBoard(url).DoProcess();
        }
    }
   
}
