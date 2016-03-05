using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace FindChange_InterviewQuestion_2
{
    public class FileState
    {
        #region FIELDS
        private     string      pathToFile;
        private     long        fileSizeBytes;
        private     int         stateResolution;
        #endregion


        #region CONSTRUCTOR
        public FileState(string _pathToFile, int _stateResolution)
        {
            pathToFile = _pathToFile;

            FileInfo f = new FileInfo(pathToFile);

            fileSizeBytes = f.Length;

            stateResolution = _stateResolution;
        }
        #endregion


        #region PUBLIC_METHODS
        public void FreezeState()
        {

        }
        #endregion


        #region PRIVATE_METHODS

        #endregion
    }
}
