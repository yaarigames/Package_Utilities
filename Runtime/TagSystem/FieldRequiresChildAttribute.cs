using System;

namespace SAS.Utilities.TagSystem
{
	[AttributeUsage(AttributeTargets.Field)]
	public class FieldRequiresChildAttribute : BaseRequiresComponent
	{
		public FieldRequiresChildAttribute(string tag = "", bool includeInactive = false)
		{
			this.includeInactive = includeInactive;
			this.tag = tag;
		}
	}
}
