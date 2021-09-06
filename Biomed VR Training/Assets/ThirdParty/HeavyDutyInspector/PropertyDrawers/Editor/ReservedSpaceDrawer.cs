//----------------------------------------------
//
//         Copyright © 2017  Illogika
//----------------------------------------------
using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

namespace HeavyDutyInspector
{

	[CustomPropertyDrawer(typeof(ReservedSpaceAttribute))]
	public class ReservedSpaceDrawer : DecoratorDrawer {

		public static List<Rect> spaceRects;

		ReservedSpaceAttribute spaceAttribute { get { return ((ReservedSpaceAttribute)attribute); } }
		
		public ReservedSpaceDrawer()
		{
			spaceRects = null;
		}

		public override float GetHeight ()
		{
			if (spaceAttribute.space < 0)
				return base.GetHeight();
			else
				return spaceAttribute.space;
	    }
		
		public override void OnGUI (Rect position)
		{
			if(spaceRects == null)
			{
				spaceRects = new List<Rect>();
				spaceAttribute.isFirst = true;
			}

			if(spaceAttribute.isFirst)
			{
				spaceRects.Clear();
			}

			spaceRects.Add(position);
		}
	}

}
