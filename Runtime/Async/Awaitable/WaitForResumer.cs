namespace SAS.Async
{
    public partial class Routine
    {
		/// <summary>
		/// Routine that yields until a resumer is resumed.
		/// Useful for using resumers in WaitForAll/WaitForAny.
		/// </summary>
		public static async AsyncTask WaitForResumer(IResumer resumer)
		{
			await resumer;
		}

		/// <summary>
		/// Routine that yields until a resumer is resumed.
		/// Useful for using resumers in WaitForAll/WaitForAny.
		/// </summary>
		public static async AsyncTask<T> WaitForResumer<T>(IResumer<T> resumer)
		{
			return await resumer;
		}
	}
}
