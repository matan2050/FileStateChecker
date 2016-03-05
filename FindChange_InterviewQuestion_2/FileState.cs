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
        /// state is a key value pair containing each interval in the desired resolution
        /// and the appropriate md5 hash for that interval
        /// </summary>
        public void GenerateStateFile()
        {
            long            startPos            = 0;
            long            endPos              = fileSizeBytes;
            long            localStartPos;
            long            localEndPos;

            using (var fileStateStream = new FileStream(pathToStateFile, FileMode.Create))
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

                        fileStateStream.Write(currBufferHash, 0, currBufferHash.Length);
                    }
                }
            }
        }

        public List<byte[]> ReadStateFile()
        {
            List<byte[]>    stateFileHash       = new List<byte[]>();
            long            currByteIndex       = 0;
            byte[]          currHash;
            FileInfo        stateFileInf        = new FileInfo(pathToStateFile);

            using (var fileStream = new FileStream(pathToStateFile, FileMode.Open))
            {
                for (currByteIndex = 0; currByteIndex < stateFileInf.Length; currByteIndex += 128)
                {
                    fileStream.Position = currByteIndex;
                    currHash = new byte[128];
                    fileStream.Read(currHash, 0, 128);          // TODO: CHECK IF CHANGE 128 FROM HARD CODED
                    stateFileHash.Add(currHash);
                }
            }
            return stateFileHash;
        }

        public List<byte[]> FindCurrentState()
        {
            List<byte[]>    currStateHash       = new List<byte[]>();
            long            startPos            = 0;
            long            endPos              = fileSizeBytes;

            long            localStartPos;
            long            localEndPos;


            // Checking if a state file exist - if not, first run, everything is different
            if (!File.Exists(pathToStateFile))
            {
                for (long i = startPos; i <= endPos; i = i + stateResolution)
                {
                    Console.WriteLine("{0} - {1}: Change", i, i+stateResolution);
                }
            }


            using (var fileStream = new FileStream(pathToFile, FileMode.Open))
            {
                for (long i = startPos; i <= endPos; i = i + stateResolution)
                {
                    localStartPos = i;
                    localEndPos = i + stateResolution - 1;

                    byte[]      currReadBuffer      = new byte[localEndPos - localStartPos];

                    fileStream.Read(currReadBuffer, 0, (int)(localEndPos - localStartPos));

                    byte[]      currBufferHash      = GenerateHash(currReadBuffer);
                    currStateHash.Add(currBufferHash);
                }
            }

            return currStateHash;
        }
        //public bool IsEqual(FileState )
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
