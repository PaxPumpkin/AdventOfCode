using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AoC_2021
{
   class Day22_ReactorReboot : AoCodeModule
   {
      public Day22_ReactorReboot()
      {
         inputFileName = @"InputFiles\" + this.GetType().Name + ".txt";
         GetInput();
         OutputFileReset();
      }
      public override void DoProcess()
      {

       
         ResetProcessTimer(true);
         Reactor.Reset();
         foreach (string processingLine in inputFile)
         {
            Reactor.ProcessRebootStep(processingLine, Reactor.Stage.Initialize);
         }
         AddResult("Initialize -- Count of ON cubes: " + Reactor.ReactorCubes.Count().ToString());
         
         ResetProcessTimer(true);
         Reactor.Reset();
         foreach (string processingLine in inputFile)
         {
            Reactor.ProcessRebootStep(processingLine, Reactor.Stage.Reboot);
         }
         AddResult("Reboot -- Count of ON cubes: " + Reactor.ReactorCubes.Count().ToString());

      }
   }
   public class Reactor
   {
      public static HashSet<(long, long, long)> ReactorCubes = new HashSet<(long, long, long)>();
      public enum Stage { Initialize, Reboot }
      public Reactor()
      {

      }
      public static void Reset()
      {
         ReactorCubes = new HashSet<(long, long, long)>();
      }
      public static void ProcessRebootStep(string instruction, Stage stage)
      {
         string[] instructions = instruction.Split(new char[] { ' ', ',' }, StringSplitOptions.RemoveEmptyEntries);
         bool Add = instructions[0] == "on";
         (long start, long stop) XRange = ParseRange(instructions[1]);
         (long start, long stop) YRange = ParseRange(instructions[2]);
         (long start, long stop) ZRange = ParseRange(instructions[3]);
         //Console.WriteLine("Turn Cubes " + (Add?"ON":"OFF"));
         if (stage==Stage.Reboot || (XRange.start >= -50 && XRange.stop <= 50 && YRange.start >= -50 && YRange.stop <= 50 && ZRange.start >= -50 && ZRange.stop <= 50))
            for (long x = XRange.start; x <= XRange.stop; x++)
               for (long y = YRange.start; y <= YRange.stop; y++)
                  for (long z = ZRange.start; z <= ZRange.stop; z++)
                  {
                     //Console.WriteLine(x + "," + y + "," + z);
                     (long, long, long) CubeKey = (x, y, z);
                     bool cubeIsOn = ReactorCubes.Contains(CubeKey);
                     if (cubeIsOn && !Add) // turn off/remove from hashset
                        ReactorCubes.Remove(CubeKey);
                     else if (!cubeIsOn && Add)
                        ReactorCubes.Add(CubeKey);
                  }

      }
      private static (long, long) ParseRange(string rangeSet)
      {
         (long start, long stop) resultRange;
         string[] parts = rangeSet.Split(new char[] { '=', '.' }, StringSplitOptions.RemoveEmptyEntries);
         resultRange.start = long.Parse(parts[1]);
         resultRange.stop = long.Parse(parts[2]);
         return resultRange;
      }
   }
}
