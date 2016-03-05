using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.IO;
using System.Text;

namespace FindChange_InterviewQuestion_2
{
    public class FileState
    {
        #region FIELDS
        private     string      pathToFile;             // Path to the file we want to track
        private     string      pathToStateFile;        // Path to the file that holds the state for tracked file
        private     long        fileSizeBytes;          // Size of that file in bytes
        private     int         stateResolution;        // Resolution to detect change in bytes
        #endregion


        #region CONSTRUCTOR
        public FileState(string _pathToFile, int _stateResolution)
        {
            pathToFile = _pathToFile;
            pathToStateFile = ReplaceExtension(pathToFile);

            FileInfo f = new FileInfo(pathToFile);

            fileSizeBytes = f.Length;

            stateResolution = _stateResolution;
        }
        #endregion


        #region PUBLIC_METHODS
        /// <summary>
        /// Creates a state file for the tracked file.
        /// state is basically a table with byte index in tracked file with the value in that byte
        /// if neighbouring bytes are of the same value, function goesin each direction and searches
        /// for the first non equal byte
        /// </summary>
        public void GenerateStateFile()
        {
            long        startPos            = 0;
            long        endPos              = fileSizeBytes;
            long        localStartPos;
            long        localEndPos;


            using (var fileStateStream = new StreamWriter(pathToStateFile))
            {
                using (var fileStream = new FileStream(pathToFile, FileMode.Open))
                {
                    for (long i = startPos; i <= endPos; i = i + stateResolution)
                    {
                        localStartPos = i;
                        localEndPos = i + stateResolution - 1;

                        byte[]      currReadBuffer      = new byte[localEndPos - localStartPos];

                        fileStream.Read(currReadBuffer, 0, (int)(localEndPos - localStartPos));

                        byte[]      currBufferHash      = GenerateHash(currReadBuffer);
                        string      hashAsString        = Encoding.UTF8.GetString(currBufferHash);

                        string currBufferOutputStream = localStartPos.ToString() + "," + localEndPos.ToString() + "," +
                                                    hashAsString;

                        byte[] currBufferOutputArray = Encoding.ASCII.GetBytes(currBufferOutputStream);


                        fileStateStream.WriteLine(currBufferOutputStream);
                    }
                }
            }
        }
        #endregion


        #region PRIVATE_METHODS
        private string ReplaceExtension(string filePath)
        {
            string      replacedString;
            int         dotPosition         = filePath.IndexOf('.');
            string      extension           = filePath.Substring(dotPosition+1, 3);

            replacedString = filePath.Replace(extension, "stt");

            return replacedString;
        }

        private byte[] GenerateHash(byte[] fileContentsBuffer)
        {
            byte[] hashCode;

            using (var md5 = MD5.Create())
            {
                hashCode = md5.ComputeHash(fileContentsBuffer);
            }

            return hashCode;
        }
        #endregion

    }
}
