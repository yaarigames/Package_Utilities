using System;

namespace SAS.Utilities.TagSystem
{
	[AttributeUsage(AttributeTargets.Field)]
	public class FieldRequiresChildAttribute : BaseRequiresComponent
	{
		public FieldRequiresChildAttribute(Tag tag = Tag.None, bool includeInactive = false)
		{
			this.includeInactive = includeInactive;
			this.tag = tag;
		}
	}
}
