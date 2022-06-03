using System;

namespace SAS.Utilities.TagSystem
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
