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
        /// state is a set of hash codes that relates to each 'stateResolution'
        /// range in the file, outputted one after the other
        /// </summary>
        public List<byte[]> GenerateState(bool saveToStateFile)
        {
			List<byte[]>    stateHashList       = new List<byte[]>();	// TODO - CREATE LIST OF KNOWN SIZE 
            long            startPos            = 0;
            long            endPos              = fileSizeBytes;
            long            localStartPos;
            long            localEndPos;


            using (var fileStateStream = new FileStream(pathToStateFile, FileMode.OpenOrCreate))
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

            if (!saveToStateFile)
            {
                File.Delete(pathToStateFile);
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
                for (currByteIndex = 0; currByteIndex < stateFileInf.Length; currByteIndex += 16)
                {
                    fileStream.Position = currByteIndex;
                    currHash = new byte[128];
                    fileStream.Read(currHash, 0, 128);          // TODO: CHECK IF CHANGE 128 FROM HARD CODED
                    stateFileHashList.Add(currHash);
                }
            }
            return stateFileHashList;
        }


        /// <summary>
        /// Outputs a list of hash codes to the path predefined
        /// to the state file
        /// </summary>
        /// <param name="stateFileHashList">List of byte arrays (hash codes)</param>
        public void WriteStateFile(List<byte[]> stateFileHashList)
        {
            using (var fileStream = new FileStream(pathToStateFile, FileMode.OpenOrCreate))
            {
                foreach (var currRangeHash in stateFileHashList)
                {
                    fileStream.Write(currRangeHash, 0, currRangeHash.Length);
                }
            }
        }


        /// <summary>
        /// Compares two hash lists and return first different hash position in list
        /// </summary>
        /// <param name="hashSet1">First set of hash codes (list)</param>
        /// <param name="hashSet2">Second set of hash codes (list)</param>
        /// <returns>Index of the first different hash code</returns>
		public int CompareHashLists(ref List<byte[]> hashSet1, ref List<byte[]> hashSet2)
		{
			int		hashSet1Size        = hashSet1.Count;
			int		hashSet2Size        = hashSet2.Count;
			int		numHashesToCheck    = Math.Min(hashSet1Size, hashSet2Size);

			int    firstDiffPosition = -1;

			for (int i = 0; i < numHashesToCheck; i++)
			{
				byte[]	currHash1Element	= hashSet1[i];
				byte[]	currHash2Element	= hashSet2[i];

				int		currHashDiff;
                bool    firstDiffFound  = false;

				for (int j = 0; j < 16; j++)
				{
					currHashDiff = (int)(currHash1Element[j] - currHash2Element[j]);

					if (currHashDiff != 0)
					{
						firstDiffPosition = i;
                        firstDiffFound = true;
						break;
					}
				}

                if (firstDiffFound)
                {
                    break;
                }
			}

			return firstDiffPosition;
		}


        public long[] HashPositionToRange(int hashPosition)
        {
            long[]      range           = new long[2];
            int         rangesCounter   = 0;

            for (int i = 0; i < fileSizeBytes; i += stateResolution)
            {
                if (rangesCounter == hashPosition)
                {
                    range[0] = i;
                    range[1] = i + stateResolution;
                }
                rangesCounter += 1;
            }

            return range;
        }
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


		#region PROPERTIES
		public string PathToStateFile
		{
			get
			{
				return pathToStateFile;
			}
			set
			{
				pathToStateFile = value;
			}
		}

        public int StateResolution
        {
            get
            {
                return stateResolution;
            }
            set
            {
                stateResolution = value;
            }
        }
		#endregion
	}
}
