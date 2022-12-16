using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AoC_2022
{
    class Day15_BeaconExclusionZone : AoCodeModule
    {
        public Day15_BeaconExclusionZone()
        {
            inputFileName = @"InputFiles\" + this.GetType().Name + ".txt";
            GetInput();
            OutputFileReset();

        }
        public override void DoProcess()
        {
            //** > Result for Day15_BeaconExclusionZone part 1: 5240818 Spaces on line 2000000 cannot contain a beacon. (Process: 843.7166 ms)
            //** > Result for Day15_BeaconExclusionZone part 2: Distress Beacon tuning frequency is 13213086906101 (Process: 15609.8184 ms)
            ResetProcessTimer(true);
            List<BeaconSensor> sensors = new List<BeaconSensor>();
            foreach (string processingLine in inputFile)
            {
                sensors.Add(new BeaconSensor(processingLine.Trim()));
            }
            int lineNumberToCheck =  2000000; //Sample is 10, Puzzle is 2000000
            List<CoverageRange> ranges = sensors.Select(sensor => sensor.CoverageAtLineNumber(lineNumberToCheck)).Where(cr => cr != null).ToList();
            int lowestX = ranges.Min(cr => cr.FirstX), highestX = ranges.Max(cr => cr.LastX), lineLength = highestX - lowestX + 1;
            string testLine = new string('.', lineLength);
            ranges.ForEach(cr =>
                {
                    char[] testLineArray = testLine.ToArray();
                    for (int position = cr.FirstX - lowestX; position <= cr.LastX - lowestX; position++)
                    {
                        testLineArray[position] = '#';
                    }
                    testLine = new string(testLineArray);
                });

            AddResult((testLine.Count(c => c == '#') - (sensors.Any(s => s.BeaconY == lineNumberToCheck) ? 1 : 0)).ToString() + " Spaces on line " + lineNumberToCheck.ToString() + " cannot contain a beacon.");
            ResetProcessTimer(true);
            long XHole = -1;
            long YLine = -1;
            bool foundIt = false;
            List<CoverageRange> coverage = new List<CoverageRange>();

            int MaxRange = 4000000;//20; //4000000 is puzzle
            for (int y = 0; y <= MaxRange && !foundIt; y++)
            {
                YLine = y;
                List<BeaconSensor> hasCoverage = sensors.Where(s => s.YinRange(y) && s.OverlapsXRangeAtLine(0, MaxRange, y)).ToList();
                coverage.Clear();
                hasCoverage.ForEach(s =>
                {
                    coverage.Add(
                        s.CoverageAtLineNumber(y)
                    );
                });
                coverage = coverage.OrderBy(cr => cr.FirstX).ToList();
                int lastXCoverage = -1;
                coverage.ForEach(cr =>
                {
                    if (cr.FirstX > lastXCoverage + 1)
                    {
                        foundIt = true;
                        XHole = lastXCoverage + 1;
                    }
                    lastXCoverage = Math.Max(lastXCoverage, cr.LastX);
                });
            }
            long tuningFrequency = (XHole * 4000000) + YLine;
            AddResult("Distress Beacon tuning frequency is " + tuningFrequency.ToString());
            ResetProcessTimer(true);
        }
    }
    public class BeaconSensor
    {
        public int SensorX;
        public int SensorY;
        public int BeaconX;
        public int BeaconY;
        public int ManhattanDistance
        {
            get
            {
                return Math.Abs(SensorX - BeaconX) + Math.Abs(SensorY - BeaconY);
            }
        }
        public int LeftEdge
        {
            get
            {
                return BeaconX - ManhattanDistance;
            }
        }
        public int RightEdge
        {
            get
            {
                return BeaconX + ManhattanDistance;
            }
        }
        public CoverageRange CoverageAtLineNumber(int lineNumber)
        {
            CoverageRange range = null;
            int distanceToLine = Math.Abs(SensorY - lineNumber);
            if (distanceToLine <= ManhattanDistance)
            {
                int coverageWidthAtLine = ((ManhattanDistance - distanceToLine) * 2) + 1; // may be useless
                int coverageWidthBase = (ManhattanDistance - distanceToLine);
                range = new CoverageRange() { FirstX = SensorX - coverageWidthBase, LastX = SensorX + coverageWidthBase };
                if (!(range.LastX - range.FirstX + 1 == coverageWidthAtLine))
                {
                    throw new Exception("Calculation of coverage is off!");
                }
            }
            return range;
        }
        public bool YinRange(int y)
        {
            return y <= SensorY + ManhattanDistance && y >= SensorY - ManhattanDistance;
        }
        public bool OverlapsXRangeAtLine(int minX, int maxX, int lineY)
        {
            CoverageRange cr = CoverageAtLineNumber(lineY);
            bool overlaps = cr != null &&
                ((cr.FirstX <= minX && cr.LastX >= minX) ||
                (cr.FirstX <= maxX && cr.LastX >= maxX) ||
                (cr.LastX >= minX && cr.LastX <= maxX) ||
                (cr.FirstX >= minX && cr.FirstX <= maxX));
            return overlaps;
        }
        public BeaconSensor(string definitionLine)
        {
            string[] parts = definitionLine.Split(new char[] { ' ', '=', ',', ';', ':' }, StringSplitOptions.RemoveEmptyEntries);
            SensorX = int.Parse(parts[3]);
            SensorY = int.Parse(parts[5]);
            BeaconX = int.Parse(parts[11]);
            BeaconY = int.Parse(parts[13]);
        }
    }
    public class CoverageRange
    {
        public int FirstX;
        public int LastX;
        public bool Covers(int x)
        {
            return FirstX <= x && x <= LastX;
        }
    }
}
