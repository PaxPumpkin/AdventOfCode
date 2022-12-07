using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AoC_2022
{
    class Day07_NoSpaceLeftOnDevice : AoCodeModule
    {
        public Day07_NoSpaceLeftOnDevice()
        {

            inputFileName = @"InputFiles\" + this.GetType().Name + "Sample.txt";
            GetInput();
            OutputFileReset();
        }
        public override void DoProcess()
        {
            //** > Result for Day07_NoSpaceLeftOnDevice part 1: Sum of Directory Sizes that are at most 100000 is 1427048 (Process: 0.6041 ms)
            //** > Result for Day07_NoSpaceLeftOnDevice part 2: Smallest directory to delete that will free up enough space is:  snqcgbs size 2940614 (Process: 0.4482 ms)

            ResetProcessTimer(true);// remove system core instantiation in debug mode overhead time. 
            AOCDirectory root, currentDirectory;
            root = currentDirectory = new AOCDirectory(null, "/");
            string[] terminal, splitStrings = new string[] { " " };
            foreach (string processingLine in inputFile)
            {
                if (!processingLine.Equals(string.Empty)) //final line
                {
                    terminal = processingLine.Split(splitStrings, StringSplitOptions.RemoveEmptyEntries);
                    // removed all exception throwing for weird or unexpected conditions. None occurred, so drop it. Assume input is OK.
                    if (terminal[0] == "$") // line showing a command was issued. Only "cd" counts, not "ls"
                    {
                        if (terminal[1] == "cd")
                        {
                            // if explicit root name, go there. else, if ".." go up one, otherwise, get the dir so named. 
                            currentDirectory = (terminal[2] == "/") ? root : ((terminal[2] == "..") ? currentDirectory.ParentDirectory : currentDirectory.ChildDirectories.FirstOrDefault(dir => dir.Name.Equals(terminal[2])));
                        }
                    }
                    else// gotta be a ls output
                    {
                        if (long.TryParse(terminal[0], out long size))
                            currentDirectory.FileList.Add(new AOCFile(currentDirectory, terminal[1], size));
                        else // gotta be terminal[0] == "dir"
                            currentDirectory.ChildDirectories.Add(new AOCDirectory(currentDirectory, terminal[1]));
                    }
                }
            }
            if (inputFileName.Contains("Sample")) foreach (string lsLine in root.RecursiveLs) Print(lsLine, FileOutputAlso); //output to check against sample data.

            AddResult("Sum of Directory Sizes that are at most 100000 is " + root.ChildDirectoriesRecursive.Where(cdr => cdr.DirectorySize <= 100000).Sum(cdr => cdr.DirectorySize).ToString());
            ResetProcessTimer(true);
            long MaxDiskSize = 70_000_000;
            long UnusedSpace = MaxDiskSize - root.DirectorySize;
            long SpaceRequired = 30_000_000 - UnusedSpace;

            AOCDirectory smallestToDelete = root.ChildDirectoriesRecursive.Where(cdr => cdr.DirectorySize >= SpaceRequired).OrderBy(cdr => cdr.DirectorySize).FirstOrDefault() ?? new AOCDirectory(null, "Poop!");
            AddResult("Smallest directory to delete that will free up enough space is:  " + smallestToDelete.Name + " size " + smallestToDelete.DirectorySize.ToString());
        }
    }
    public class AOCFilesSystemObject
    {
        public AOCDirectory ParentDirectory;
        public string Name;
    }
    public class AOCDirectory : AOCFilesSystemObject
    {
        public List<AOCDirectory> ChildDirectories;
        public List<AOCFile> FileList;
        public List<string> RecursiveLs // for fun output and sample checking only. Not needed for puzzle. 
        {
            get
            {
                List<string> ls = new List<string>();
                ls.Add("- " + Name + " (dir, Size = " + DirectorySize.ToString() + ")");
                ChildDirectories.ForEach(cd =>
                {
                    cd.RecursiveLs.ForEach(cdrls => ls.Add("  " + cdrls));
                });
                FileList.ForEach(f => ls.Add("  - " + f.Name + " (file, size=" + f.Size.ToString() + ")"));
                return ls;
            }
        }
        public long DirectorySize
        {
            get
            {
                return FileList.Sum(files => files.Size) + ChildDirectories.Sum(children => children.DirectorySize);
            }
        }
        public List<AOCDirectory> ChildDirectoriesRecursive
        {
            get
            {
                List<AOCDirectory> cdr = ChildDirectories.ToList();
                ChildDirectories.ForEach(cd => cdr.AddRange(cd.ChildDirectoriesRecursive));
                return cdr;
            }
        }
        public AOCDirectory(AOCDirectory parent, string name)
        {
            ParentDirectory = parent;
            Name = name;
            ChildDirectories = new List<AOCDirectory>();
            FileList = new List<AOCFile>();
        }
    }
    public class AOCFile : AOCFilesSystemObject
    {
        public long Size;
        public AOCFile(AOCDirectory parent, string name, long size)
        {
            ParentDirectory = parent; Name = name; Size = size;
        }
    }
}
