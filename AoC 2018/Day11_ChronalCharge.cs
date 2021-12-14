using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AoC_2018
{
    class Day11_ChronalCharge : AoCodeModule
    {
        public Day11_ChronalCharge()
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
            ResetProcessTimer(true);
            long serialNumber = 5791;
            //serialNumber = 42;
            //serialNumber = 71;
            //serialNumber = 39;
            //serialNumber = 57;
            long[,] powerCells = new long[300, 300];
            // to avoid using x+1 all over the place...
            long rx, ry;
            for (long x = 0; x < 300; x++)
            {
                for (long y = 0; y < 300; y++)
                {
                    rx = x + 1; ry = y + 1;
                    long rackID = rx + 10;
                    long powerLevel = rackID * ry;
                    powerLevel += serialNumber;
                    powerLevel *= rackID;
                    powerLevel = Math.Abs(powerLevel);
                    string pl = powerLevel.ToString();
                    if (pl.Length >= 3)
                    {
                        powerLevel = long.Parse(pl.Substring(pl.Length - 3, 1));
                    }
                    else
                    {
                        powerLevel = 0;
                    }
                    powerLevel -= 5;
                    powerCells[x, y] = powerLevel;
                }
            }
            long maxPower=0, maxPowerRx=0, MaxPowerRy=0,powerTest=0, maxPowerGridSize = 0, testGridSize=0;
            for (long x = 0; x < 298; x++)
            {
                for (long y = 0; y < 298; y++)
                {
                    rx = x + 1; ry = y + 1;
                    powerTest = 0;
                    for (long i = 0; i < 3; i++) {
                        for (long j = 0; j < 3; j++)
                        {
                            powerTest += powerCells[x + i, y + j];
                        }
                    }
                    if (powerTest > maxPower)
                    {
                        maxPower = powerTest;
                        maxPowerRx = rx;
                        MaxPowerRy = ry;
                    }
                }
            }
            AddResult("Max Power " + maxPower.ToString() + " at " + maxPowerRx.ToString() + "," + MaxPowerRy.ToString());
            ResetProcessTimer(true);
            maxPower = 0; maxPowerRx = 0; MaxPowerRy = 0; powerTest = 0; maxPowerGridSize = 0; testGridSize = 0;
            FunUtilities.HackerScannerPrint hsp = new FunUtilities.HackerScannerPrint("Test at 000, 000", 'm');
            hsp.Print("Test at 000, 000", true);
            for (long x = 0; x < 300; x++)
            {
                for (long y = 0; y < 300; y++)
                {
                    hsp.Print("Test at " + x.ToString().PadLeft(3, '0') + ", " + y.ToString().PadLeft(3, '0')); // no 'm' in here, so... it's just a straight print on the same line. No random fill-in
                    rx = x + 1; ry = y + 1;
                    powerTest = 0;
                    testGridSize = Math.Min(300-x,300-y); // this is the maximum size grid only. 
                    while (testGridSize > 0)
                    {
                        for (long i = 0; i < testGridSize; i++)
                        {
                            for (long j = 0; j < testGridSize; j++)
                            {
                                powerTest += powerCells[x + i, y + j];
                            }
                        }
                        if (powerTest > maxPower)
                        {
                            maxPower = powerTest;
                            maxPowerRx = rx;
                            MaxPowerRy = ry;
                            maxPowerGridSize = testGridSize;
                        }
                        testGridSize--;
                        powerTest = 0;
                    }
                }
            }
            Console.WriteLine();
            AddResult("Max Power " + maxPower.ToString() + " at " + maxPowerRx.ToString() + "," + MaxPowerRy.ToString() + " with grid size " + maxPowerGridSize.ToString());
        }
    }
}
