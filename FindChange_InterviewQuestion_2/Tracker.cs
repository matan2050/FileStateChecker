using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace FindChange_InterviewQuestion_2
{
	public class Tracker
	{
		#region FIELDS
		private     string      filePath;
		private     bool        fileExists		= true;
		#endregion


		#region CONSTRUCTOR
		public Tracker(string _filePath)
		{
			filePath = _filePath;

			if (!File.Exists(filePath))
			{
				fileExists = false;
			}
		}
		#endregion


		#region METHODS

		#endregion
	}
}
