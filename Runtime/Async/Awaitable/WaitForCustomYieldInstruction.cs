using UnityEngine;

namespace SAS.Async
{
    public partial class Routine
    {
		/// <summary>
		/// Routine that yields until a CustomYieldInstruction has completed. 
		/// </summary>
		public static async AsyncTask WaitForCustomYieldInstruction(CustomYieldInstruction customYieldInstruction)
		{
			while (customYieldInstruction.keepWaiting)
				await WaitForNextFrame();
		}
	}
}
