using System;

namespace SAS.TagSystem
{
	[AttributeUsage(AttributeTargets.Field)]
	public class FieldRequiresSelfAttribute : BaseRequiresComponent
	{
		public FieldRequiresSelfAttribute(string tag = "")
		{
			this.tag = tag;
		}
	}
}
