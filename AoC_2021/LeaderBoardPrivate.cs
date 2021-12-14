using AoCLeaderBoardClasses;

namespace AoC_2021
{
    class zLeaderBoardPrivate : AoCodeModule
    {
        public zLeaderBoardPrivate()
        {
            inputFileName = @"InputFiles\" + this.GetType().Name + ".txt";
            GetInput(); 
            OutputFileReset(); 
        }
        readonly string url = @"https://adventofcode.com/2021/leaderboard/private/view/197110.json";
        public override void DoProcess()
        {
            new LeaderBoard(url).DoProcess();
        }
    }
   
}
