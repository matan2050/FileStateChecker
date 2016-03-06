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
            pathToStateFile = ReplaceExtension(pathToFile, "stt");

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
        public List<byte[]> GenerateState(bool saveToStateFile)
        {
			List<byte[]>    stateHashList       = new List<byte[]>();	// TODO - CREATE LIST OF KNOWN SIZE 
            long            startPos            = 0;
            long            endPos              = fileSizeBytes;
            long            localStartPos;
            long            localEndPos;


			var stateFileMode = FileMode.Create;
			if (File.Exists(pathToStateFile))
			{
				stateFileMode = FileMode.Open;
			}


            using (var fileStateStream = new FileStream(pathToStateFile, stateFileMode))
            {
                using (var fileStream = new FileStream(pathToFile, FileMode.Open))
                {
                    for (long i = startPos; i <= endPos; i = i + stateResolution)
                    {
                        localStartPos = i;
                        localEndPos = i + stateResolution - 1;

                        byte[]      currReadBuffer      = new byte[localEndPos - localStartPos];

                        fileStream.Read(currReadBuffer, 0, (int)(localEndPos - localStartPos));

                        stateHashList.Add(GenerateHash(currReadBuffer));
						

						if (saveToStateFile)
						{
							byte[] currHash = stateHashList[stateHashList.Count-1];
							fileStateStream.Write(currHash, 0, currHash.Length);
						}
                    }
                }
            }

			return stateHashList;
		}


		/// <summary>
		/// Reads a predefined state file and returns all hash codes for that file
		/// </summary>
		/// <returns>list of byte arrays that contains hash codes</returns>
        public List<byte[]> ReadStateFile()
        {
            List<byte[]>    stateFileHashList       = new List<byte[]>();
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
                    stateFileHashList.Add(currHash);
                }
            }
            return stateFileHashList;
        }


		//public long CompareHashLists(FileState currState)
		//{
				
		//}
        #endregion


        #region PRIVATE_METHODS

		/// <summary>
		/// Takes a path string and replaces its 3 letter extension 
		/// </summary>
		/// <param name="filePath">string contains path with a 3 letter extension</param>
		/// <returns>string contains the same path with the ext extension</returns>
        private string ReplaceExtension(string filePath, string ext)
        {
            string      replacedString;
            int         dotPosition         = filePath.IndexOf('.');
            string      extension           = filePath.Substring(dotPosition+1, 3);

            replacedString = filePath.Replace(extension, ext);

            return replacedString;
        }


		/// <summary>
		/// Generates an md5 hash code for an array of bytes
		/// </summary>
		/// <param name="fileContentsBuffer">byte array for which we will get a hash code</param>
		/// <returns>a hash code in the form of a byte array</returns>
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
