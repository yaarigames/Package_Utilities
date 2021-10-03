using System;

namespace SAS.TagSystem
{
	[AttributeUsage(AttributeTargets.Field)]
	public class AddComponentAttribute : BaseRequiresComponent
	{
		public AddComponentAttribute(string tag = "")
		{
			this.tag = tag;
		}
	}
}
