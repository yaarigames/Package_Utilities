using System;

namespace SAS.TagSystem
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
