using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AoC_2021
{
   class Day16_PacketDecoder : AoCodeModule
   {
      public Day16_PacketDecoder()
      {

         inputFileName = @"InputFiles\" + this.GetType().Name + ".txt";
         GetInput();
         OutputFileReset();
      }
      public override void DoProcess()
      {
         //** > Result for Day16_PacketDecoder part 1: Summation of version info is 873 (Process: 3.3481 ms)
         //** > Result for Day16_PacketDecoder part 2: Answer after processing: 402817863665 (Process: 0.0006 ms)
         ResetProcessTimer(true);
         BITSTransmission transmission = new BITSTransmission(inputFile[0]);
         transmission.Setup();
         transmission.Decode();
         AddResult("Summation of version info is " + transmission.versionSummation);
         ResetProcessTimer(true);
         AddResult("Answer after processing: " + transmission.AnswerValue);
         ResetProcessTimer(true);

      }
   }
   public class BITSTransmission
   {
      public enum operation { sum, product, min, max, literal, gt, lt, eq }
      public string binaryRepresentation = "";
      public string initialHex = "";
      public int version;
      public int typeId;
      public int lengthTypeId;


      public int recursionLevel = 0;
      public bool displayOutput = false;
      public long versionSummation = 0;
      public long RegisterValue;
      public long AnswerValue;
      public operation currentOperation;
      public bool firstSet;
      public BITSTransmission(string hexInput)
      {
         initialHex = hexInput;
         binaryRepresentation = "";
         version=0;
         typeId=0;
         lengthTypeId=0;
         recursionLevel = 0;
         versionSummation = 0;
         DebugInfo(initialHex);
      }
      public void Setup()
      {
         string hex, binaryVersion;
         int dec;
         for (int x = 0; x < initialHex.Length; x++)
         {
            hex = initialHex[x].ToString();
            dec = Convert.ToInt32(hex, 16);
            binaryVersion = Convert.ToString(dec, 2).PadLeft(4, '0');
            binaryRepresentation += binaryVersion;
         }
         DebugInfo("Remaining: " + binaryRepresentation);
         DebugInfo("Length is " + binaryRepresentation.Length + " which matches expected: " + ((initialHex.Length*4) == binaryRepresentation.Length).ToString());
         SetVersionAndType();
      }
      public int SetVersionAndType()
      {
         string versionBits = GetBits(3);
         string typeBits = GetBits(3);
         if (versionBits != "")
         {
            version = Convert.ToInt32(versionBits, 2); 
            versionSummation += version;
         }
         else
         { version = 0; }
         if (typeBits != "")
         {
            typeId = Convert.ToInt32(typeBits, 2);
         }
         else
         { typeId = 0; }
         DebugInfo("Version is " + version);
         DebugInfo("Type is " + typeId);
         return versionBits.Length + typeBits.Length;
      }
      public int Decode()
      {
         recursionLevel++;
         DebugInfo("Entering Decode.");
         int bitsProcessed = 0;
         recursionLevel++;
         switch ((operation)typeId)
         {
            case operation.literal:
               DebugInfo("Type is Literal(type 4)");
               string chunk;
               string decoded = "";
               bool endOfPacketMarked = false;
               while (!endOfPacketMarked)
               {
                  chunk = GetBits(5);
                  bitsProcessed += 5;
                  endOfPacketMarked = chunk[0] == '0';
                  decoded += chunk.Substring(1);
               }
               long literalvalue = Convert.ToInt64(decoded, 2);
               AnswerValue = literalvalue;
               
               firstSet = false;
               DebugInfo("Literal Decoded:" + literalvalue.ToString());
               break;
            default:
               AnswerValue = 0;
               currentOperation = (operation)typeId;
               firstSet = true;
               bitsProcessed += ProcessSubpackets();
               RegisterValue = AnswerValue;
               break;
         }
         recursionLevel--;
         DebugInfo("Exiting Decode");
         recursionLevel--;
         return bitsProcessed;
      }
      public int ProcessSubpackets()
      {
         int bitsProcessed = 0;
         int fieldLength = GetBits(1) == "0" ? 15 : 11; bitsProcessed += 1;
         DebugInfo("Mode is " + (fieldLength == 15 ? "Total Number Of Bits in Sub-packet(not iteration of subpackets). Should read 15 bits." : "Total number of subpackets to process. Should read 11 bits."));
         int operatorLength = Convert.ToInt32(GetBits(fieldLength), 2); bitsProcessed += fieldLength;
         DebugInfo("Field Length is " + fieldLength.ToString());
         DebugInfo((fieldLength == 15 ? "Total Number Of Bits in Sub-packet:" : "Total number of subpackets to process:") + operatorLength.ToString());
         operation thisSubpacketOp = currentOperation;
         long thisSubpacketAnswer = 0;
         bool thisFirstSet = true;
         if (fieldLength == 11)
         {
            DebugInfo("Iterating Subpackets.");
            long thisRegister = 0;
            for (int x = 0; x < operatorLength; x++)
            {
               int currentType = typeId;
               int currentVersion = version;
               bitsProcessed += SetVersionAndType();
               bitsProcessed += Decode();
               long SavedAnswer = AnswerValue;
               DebugInfo("Answer From Subpacket: " + SavedAnswer);
               DebugInfo("Current Operation is " + thisSubpacketOp);
               switch (thisSubpacketOp)
               {
                  case operation.sum:
                     thisSubpacketAnswer += SavedAnswer;
                     break;
                  case operation.product:
                     thisSubpacketAnswer = (thisFirstSet ? 1 : thisSubpacketAnswer) * SavedAnswer;
                     break;
                  case operation.min:
                     thisSubpacketAnswer = Math.Min((thisFirstSet ? long.MaxValue : thisSubpacketAnswer), SavedAnswer);
                     break;
                  case operation.max:
                     thisSubpacketAnswer = Math.Max((thisFirstSet ? long.MinValue : thisSubpacketAnswer), SavedAnswer);
                     break;
                  case operation.literal:
                     thisSubpacketAnswer = SavedAnswer;
                     break;
                  case operation.gt:
                     if (thisFirstSet)
                        thisRegister = SavedAnswer;
                     else
                        thisSubpacketAnswer = thisRegister > SavedAnswer ? 1 : 0;
                     break;
                  case operation.lt:
                     if (thisFirstSet)
                        thisRegister = SavedAnswer;
                     else
                        thisSubpacketAnswer = thisRegister < SavedAnswer ? 1 : 0;
                     break;
                  case operation.eq:
                     if (thisFirstSet)
                        thisRegister = SavedAnswer;
                     else
                        thisSubpacketAnswer = thisRegister == SavedAnswer ? 1 : 0;
                     break;

               }
               thisFirstSet = false;
               typeId = currentType;
               version = currentVersion;
            }

         }
         else
         {
            DebugInfo("Processing by bit count.");
            int operationBitCount = 0;
            int currentType = typeId;
            int currentVersion = version;
            int bitCounter;
            long thisRegister = 0;
            while (operationBitCount < operatorLength)
            {
               bitCounter = SetVersionAndType();
               bitsProcessed += bitCounter;
               operationBitCount += bitCounter;
               
               if (operationBitCount < operatorLength)
               {
                  bitCounter = Decode();
                  long SavedAnswer = AnswerValue;
                  
                  DebugInfo("Answer From Subpacket: " + SavedAnswer);
                  DebugInfo("Current Operation is " + thisSubpacketOp);
                  switch (thisSubpacketOp)
                  {
                     case operation.sum:
                        thisSubpacketAnswer += SavedAnswer;
                        break;
                     case operation.product:
                        thisSubpacketAnswer = (thisFirstSet ? 1 : thisSubpacketAnswer) * SavedAnswer;
                        break;
                     case operation.min:
                        thisSubpacketAnswer = Math.Min((thisFirstSet ? long.MaxValue : thisSubpacketAnswer), SavedAnswer);
                        break;
                     case operation.max:
                        thisSubpacketAnswer = Math.Max((thisFirstSet ? long.MinValue : thisSubpacketAnswer), SavedAnswer);
                        break;
                     case operation.literal:
                        thisSubpacketAnswer = SavedAnswer;
                        break;
                     case operation.gt:
                        if (thisFirstSet)
                           thisRegister = SavedAnswer;
                        else
                           thisSubpacketAnswer = thisRegister > SavedAnswer ? 1 : 0;
                        break;
                     case operation.lt:
                        if (thisFirstSet)
                           thisRegister = SavedAnswer;
                        else
                           thisSubpacketAnswer = thisRegister < SavedAnswer ? 1 : 0;
                        break;
                     case operation.eq:
                        if (thisFirstSet)
                           thisRegister = SavedAnswer;
                        else
                           thisSubpacketAnswer = thisRegister == SavedAnswer ? 1 : 0;
                        break;

                  }
                  thisFirstSet = false;
                  operationBitCount += bitCounter;
                  bitsProcessed += bitCounter;
               }

            }
            typeId = currentType;
            version = currentVersion;
         }
         AnswerValue = thisSubpacketAnswer;
         return bitsProcessed;
      }
      public string GetBits(int count)
      {
         if (binaryRepresentation.Length >= count)
         {
            string bits = binaryRepresentation.Substring(0, count);
            binaryRepresentation = binaryRepresentation.Substring(count);
            DebugInfo("Read " + count + " bits " + bits);
            return bits;
         }
         else if (binaryRepresentation.Length > 0)
         {
            count = binaryRepresentation.Length;
            return GetBits(count);
         }
         else
            return "";
      }
      public void DebugInfo(string text)
      {
         if (displayOutput)
         {
            string tabs = new string('\t', recursionLevel>0?recursionLevel - 1:0);
            Console.WriteLine(tabs + text);
         }
      }

   }
}
