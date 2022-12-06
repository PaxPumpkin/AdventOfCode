using AoCLeaderBoardClasses;

namespace AoC_2022
{
    class zLeaderBoardGeneva : AoCodeModule
    {
        public zLeaderBoardGeneva()
        {
            inputFileName = @"InputFiles\" + this.GetType().Name + ".txt";
            GetInput(); 
            OutputFileReset(); 
        }
        readonly string url = @"https://adventofcode.com/2022/leaderboard/private/view/100445.json";
        public override void DoProcess()
        {
            new LeaderBoard(url).DoProcess();
        }
    }
 
}
