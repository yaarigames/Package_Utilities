using System;

namespace SAS.TagSystem
{
	[AttributeUsage(AttributeTargets.Field)]
	public class FieldRequiresParentAttribute : BaseRequiresComponent
	{
		public FieldRequiresParentAttribute(string tag = "", bool includeInactive = false)
		{
			this.includeInactive = includeInactive;
			this.tag = tag;
		}
	}
}
