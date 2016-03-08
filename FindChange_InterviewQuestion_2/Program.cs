using System.Collections.Generic;
using System.IO;
using System;

namespace FindChange_InterviewQuestion_2
{
    class Program
    {
        static void Main(string[] args)
        {

            // Input check
            if (args.Length == 0)
            { 
                Console.WriteLine("Missing file path argument");
                Console.ReadLine();
                return;
            }
            else
            {
                if (!File.Exists(args[0]))
                {
                    Console.WriteLine("Invalid path, file missing");
                    return;
                }
            }


            string          pathToFile      = args[0];
			List<byte[]>    previousState   = null;
			List<byte[]>    currentState    = null;
            int             changePosition;

            FileState fs = new FileState(pathToFile, 4 * 1024 * 1024);

			if (File.Exists(fs.PathToStateFile))
			{
				previousState = fs.ReadStateFile();
			}

			currentState = fs.GenerateState(false);

            if (previousState != null)
            {
                changePosition = fs.CompareHashLists(ref previousState, ref currentState);

                if (changePosition == -1)
                {
                    Console.WriteLine("File is identical to previous version");
                }
                else
                {
                    long[] range = fs.HashPositionToRange(changePosition);
                    Console.WriteLine("File changed in between {0} and {1} Bytes", range[0], range[1]);
                }
            }
            else
            {
                Console.WriteLine("All {0} MB ranges have changed", fs.StateResolution/(1024*1024));
            }
            fs.WriteStateFile(currentState);

        }
    }
}
