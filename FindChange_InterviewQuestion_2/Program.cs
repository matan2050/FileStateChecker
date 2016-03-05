using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FindChange_InterviewQuestion_2
{
    class Program
    {
        static void Main(string[] args)
        {
            string pathToFile = @"C:\Users\User\Desktop\InterviewQuestions\classifierHOG.mat";
            FileState fs = new FileState(pathToFile, 4 * 1024 * 1024);
            fs.GenerateStateFile();

            string pathToSecondFile = @"C:\Users\User\Desktop\InterviewQuestions\classifierHOG - Copy.mat";
            FileState fs2 = new FileState(pathToSecondFile, 4 * 1024 * 1024);
            fs2.GenerateStateFile();
        }
    }
}
