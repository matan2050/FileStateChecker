using System.Collections.Generic;

namespace FindChange_InterviewQuestion_2
{
    class Program
    {
        static void Main(string[] args)
        {
            string pathToFile = @"C:\Temp\web_sim8.log";
            FileState fs = new FileState(pathToFile, 4 * 1024 * 1024);
            fs.GenerateState(true);
            List<byte[]>  origHash = fs.ReadStateFile();

            string pathToSecondFile = @"C:\Temp\web_sim5.log";
            FileState fs2 = new FileState(pathToSecondFile, 4 * 1024 * 1024);
            fs2.GenerateState(true);
            List<byte[]>  editedHash = fs2.ReadStateFile();

        }
    }
}
