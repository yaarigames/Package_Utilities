namespace SAS.Async
{
	public interface IResumerBase {}

	public interface IResumer : IResumerBase
	{
		void Resume();
		void Reset();
	}

	public interface IResumer<T> : IResumerBase
	{
		void Resume(T result);
		void Reset();
	}
}
