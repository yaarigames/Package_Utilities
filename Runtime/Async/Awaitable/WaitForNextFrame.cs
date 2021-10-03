namespace SAS.Async
{
    public partial class Routine
    {
		/// <summary> 
		/// Routine the yields until the next frame's update. Current routine must be managed. 
		/// </summary>
		public static AsyncTask WaitForNextFrame()
		{
			var nextFrameRoutine = Get<AsyncTask>(true);
			nextFrameRoutine.Trace(1);
			var resumer = new LightResumer { routine = nextFrameRoutine, id = nextFrameRoutine._id };
			Current._manager.AddNextFrameResumer(ref resumer);
			return nextFrameRoutine;
		}
	}
}
