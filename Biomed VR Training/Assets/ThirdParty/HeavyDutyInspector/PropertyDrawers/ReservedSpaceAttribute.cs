//----------------------------------------------
//            Heavy-Duty Inspector
//         Copyright © 2017  Illogika
//----------------------------------------------
using UnityEngine;

namespace HeavyDutyInspector
{
	public class ReservedSpaceAttribute : PropertyAttribute
	{
		
		public bool isFirst
		{
			get;
			set;
		}

		public float space
		{
			get;
			private set;
		}

		/// <summary>
		/// Add a vertical space in the inspector. Default size is one line height.
		/// In a custom inspector, you can use this to leave spaces when drawing the default inspector. You can then ask the ReservedSpaceDrawer for a list of all the Rect that these spaces used, to draw custom UI in.
		/// </summary>
		public ReservedSpaceAttribute()
		{
			space = -1;
			isFirst = false;
		}

		/// <summary>
		/// Add a vertical space in the inspector.
		/// In a custom inspector, you can use this to leave spaces when drawing the default inspector. You can then ask the ReservedSpaceDrawer for a list of all the Rect that these spaces used, to draw custom UI in.
		/// </summary>
		/// <param name="space">The amount of space to leave in pixels.</param>
		public ReservedSpaceAttribute(float space)
		{
			this.space = space;
			isFirst = false;
		}
	}	
}
	