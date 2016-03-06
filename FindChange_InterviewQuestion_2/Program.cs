using System.Collections.Generic;
using System.IO;

namespace FindChange_InterviewQuestion_2
{
    class Program
    {
        static void Main(string[] args)
        {
            string pathToFile = @"C:\Temp\simulationComparison1.csv";
			List<byte[]> previousState;
			List<byte[]> currentState;

            FileState fs = new FileState(pathToFile, 4 * 1024 * 1024);

			if (File.Exists(fs.PathToStateFile))
			{
				previousState = fs.ReadStateFile();
			}
			else
			{
				previousState = null;
			}

			currentState = fs.GenerateState(false);

			fs.CompareHashLists(ref previousState, ref currentState);

        }
    }
}
