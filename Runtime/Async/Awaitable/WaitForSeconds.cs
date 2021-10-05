using UnityEngine;

namespace SAS.Async
{
    public partial class Routine
    {
		/// <summary> 
		/// Routine that yields for a set amount of time. Uses Unity game time. 
		/// </summary>
		public static async AsyncTask WaitForSeconds(float seconds)
		{
			var endTime = Time.time + seconds;
			while (Time.time < endTime)
				await WaitForNextFrame();
		}
	}
}
