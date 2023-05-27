using System;

namespace SAS.Utilities.TagSystem
{
	[AttributeUsage(AttributeTargets.Field)]
	public class FieldRequiresSelfAttribute : BaseRequiresComponent
	{
		public FieldRequiresSelfAttribute(Tag tag = Tag.None)
		{
			this.tag = tag;
		}
	}
}
