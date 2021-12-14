using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BunnyHeadquarters
{
    class ElectromagneticMoat : AoCodeModule
    {
        public ElectromagneticMoat()
        {
            inputFileName = "electromagneticmoat.txt";
            base.GetInput();
        }

        public override void DoProcess()
        {
            inputFile.ForEach(line => { string[] ports = line.Split(new char[] { '/' }); new BridgePiece(Int32.Parse(ports[0]),  Int32.Parse(ports[1])); });
            List<int> strengths = new List<int>();
            BridgePiece.AllPieces.Where(piece => (piece.connectorA == 0 || piece.connectorB == 0)).ToList().ForEach(starter => 
            {
                strengths.Add(BridgePiece.BridgeStrength(starter,0));
            });
            BridgePiece.AllPieces.Where(piece => (piece.connectorA == 0 || piece.connectorB == 0)).ToList().ForEach(starter =>
            {
                BridgePiece.bridgeLength = 0;
                BridgePiece.MinMaxBridgeStrength(starter, 0,0);
            });
            FinalOutput.Add("Strongest Bridge: " + strengths.Max().ToString());
            FinalOutput.Add("Longest Bridge was " + BridgePiece.maxBridgeLength + " pieces of strength " + BridgePiece.maxBridgeStrength);
        }
    }
    class BridgePiece
    {
        public int connectorA;
        public int connectorB;
        public bool inUse = false;
        public static int bridgeLength=0;
        public static bool foundAMaxLength = false;
        public static int maxBridgeLength=0;
        public static int maxBridgeStrength=0;
        public int strength { get { return connectorA + connectorB; } }
        public static List<BridgePiece> AllPieces = new List<BridgePiece>();
        public BridgePiece(int a, int b)
        {
            this.connectorA = a; this.connectorB = b;
            AllPieces.Add(this);
        }
        public static int BridgeStrength(BridgePiece from,int usedPort)
        {
            int strength = from.strength;
            from.inUse = true;
            int maxFound = 0;
            
            int connectionPort = (from.connectorA == usedPort) ? from.connectorB : from.connectorA;
            List<BridgePiece> possibleConnections = BridgePiece.AllPieces.Where(piece => (!piece.inUse && (piece.connectorA == connectionPort || piece.connectorB == connectionPort))).ToList();
            possibleConnections.ForEach(piece =>
            {
                maxFound = Math.Max(maxFound, BridgePiece.BridgeStrength(piece, connectionPort));
            });

            
            strength += maxFound;
            from.inUse = false;
            
            
            return strength;
        }
        public static void MinMaxBridgeStrength(BridgePiece from, int usedPort, int strengthFrom)
        {
            bridgeLength++;
            from.inUse = true;
            strengthFrom += from.strength;
            int connectionPort = (from.connectorA == usedPort) ? from.connectorB : from.connectorA;
            List<BridgePiece> possibleConnections = BridgePiece.AllPieces.Where(piece => (!piece.inUse && (piece.connectorA == connectionPort || piece.connectorB == connectionPort))).ToList();
            possibleConnections.ForEach(piece =>
            {
                BridgePiece.MinMaxBridgeStrength(piece, connectionPort,strengthFrom);
            });
            if (possibleConnections.Count == 0)
            {
                //Console.WriteLine("We've built a bridge to completion at length:" + bridgeLength);
                if (bridgeLength >= maxBridgeLength)
                {
                    Console.WriteLine("It either matches or exceeds old max length. Old:" + maxBridgeLength + ", this one: " + bridgeLength);
                    maxBridgeLength = bridgeLength;
                    maxBridgeStrength = Math.Max(maxBridgeStrength, strengthFrom);
                }
            }
            bridgeLength--;
            from.inUse = false;
        }
    }
}
