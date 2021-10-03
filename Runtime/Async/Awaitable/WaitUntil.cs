using System;

namespace SAS.Async
{
    public partial class Routine
    {
		/// <summary> 
		/// Routine that yields until a condition has been met.
		/// </summary>
		public static async AsyncTask WaitUntil(Func<bool> condition)
		{
			while (!condition())
				await WaitForNextFrame();
		}
	}
}
