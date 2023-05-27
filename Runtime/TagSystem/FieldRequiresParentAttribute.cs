using System;

namespace SAS.Utilities.TagSystem
{
	[AttributeUsage(AttributeTargets.Field)]
	public class FieldRequiresParentAttribute : BaseRequiresComponent
	{
		public FieldRequiresParentAttribute(Tag tag = Tag.None, bool includeInactive = false)
		{
			this.includeInactive = includeInactive;
			this.tag = tag;
		}
	}
}
