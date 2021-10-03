using System;

namespace SAS.Async
{
#if DEBUG
	public class RoutineException : Exception
	{
		public RoutineException(string message, Exception innerException) : base(message, innerException) { }
	}
#endif

	public partial class Routine
	{
#if DEBUG
		protected System.Diagnostics.StackFrame _stackFrame = null; //Track where the routine was created for debugging
		/// <summary> 
		/// The current async/await stack trace for this routine 
		/// </summary>
		internal string StackTrace
		{
			get
			{
				string formatFrame(System.Diagnostics.StackFrame frame)
				{
					if (frame == null)
						return "(unknown) at unknown:0:0";

					var filePath = frame.GetFileName().Replace("\\", "/");
					var assetsIndex = filePath.IndexOf("/Assets/");
					if (assetsIndex >= 0)
						filePath = filePath.Substring(assetsIndex + 1);

					return string.Format(
						"{0}.{1} (at <a href=\"{2}\" line=\"{3}\">{2}:{3}:{4}</a>)",
						frame.GetMethod().DeclaringType,
						frame.GetMethod().Name,
						filePath,
						frame.GetFileLineNumber(),
						frame.GetFileColumnNumber()
					);
				}
				var routine = this;
				var stackTrace = formatFrame(routine._stackFrame);
				while (routine._parent != null)
				{
					routine = routine._parent;
					stackTrace += "\n" + formatFrame(routine._stackFrame);
				}
				return stackTrace;
			}
		}
#endif
		/// <summary> Internal use only. Store the current stack frame for debugging. </summary>
		[System.Diagnostics.Conditional("DEBUG")]
		internal void Trace(int frame)
		{
#if DEBUG
			if (TracingEnabled)
				_stackFrame = new System.Diagnostics.StackFrame(frame + 1, true);
#endif
		}
	}
}
