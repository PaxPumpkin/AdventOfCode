using AoCLeaderBoardClasses;

namespace AoC_2020
{
    class zLeaderBoardGeneva : AoCodeModule
    {
        public zLeaderBoardGeneva()
        {
            inputFileName = @"InputFiles\" + this.GetType().Name + ".txt";
            GetInput(); 
            OutputFileReset(); 
        }
        string url = @"https://adventofcode.com/2020/leaderboard/private/view/100445.json";
        public override void DoProcess()
        {
            new LeaderBoard(url).DoProcess();
        }
    }
 
}
