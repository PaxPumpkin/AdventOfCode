using System;
using System.Linq;


namespace AoC_2020
{
    class Day02_PasswordPhilosophy : AoCodeModule
    {
        
        public Day02_PasswordPhilosophy()
        {
            inputFileName = @"InputFiles\" + this.GetType().Name + ".txt";
            GetInput(); 
            OutputFileReset(); 
        }
        public override void DoProcess()
        {
            // ** > Result for Day02_PasswordPhilosophy part 1:There are 600 valid passwords.(Process: 5 ms)
            // ** > Result for Day02_PasswordPhilosophy part 2:There are 245 valid passwords.(Process: 0 ms)
            //
            // code-golf result:
            // ** > Result for Day02_PasswordPhilosophy part 1:There are 600 valid passwords.(Process: 1 ms)
            // ** > Result for Day02_PasswordPhilosophy part 2:There are 245 valid passwords.(Process: 0 ms)
            // 
            // final code-golfing:
            // ** > Result for Day02_PasswordPhilosophy part 1:There are 600 valid passwords.(Process: 0 ms)
            // ** > Result for Day02_PasswordPhilosophy part 2:There are 245 valid passwords.(Process: 0 ms)
            
            

            ResetProcessTimer(true);
            AddResult("There are " + inputFile.Count(pwdLine => VoodooValidPassword(pwdLine)).ToString() + " valid passwords.");
            // part 2, XOR condition is true, add optional bool param
            ResetProcessTimer(true);
            AddResult("There are " + inputFile.Count(pwdLine => VoodooValidPassword(pwdLine,true)).ToString() + " valid passwords.");

        }
        char[] noWhammies = new char[] { ':', ' ', '-' };
        public bool VoodooValidPassword(string passwordLineToCheck,bool XOR=false)
        {
            string[] voodoo = passwordLineToCheck.Split(noWhammies,StringSplitOptions.RemoveEmptyEntries);
            return ((XOR)?
                (voodoo[3][Convert.ToInt32(voodoo[0])-1]==voodoo[2][0] ^ voodoo[3][Convert.ToInt32(voodoo[1])-1] == voodoo[2][0])
                :(voodoo[3].Count(c=>c==voodoo[2][0])>=Convert.ToInt32(voodoo[0]) && voodoo[3].Count(c=>c==voodoo[2][0])<=Convert.ToInt32(voodoo[1])));
        }
    }
}
