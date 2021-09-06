//----------------------------------------------
//            Heavy-Duty Inspector
//      Copyright © 2014 - 2017  Illogika
//----------------------------------------------
using UnityEngine;

namespace HeavyDutyInspector
{

	public class HideVariableAttribute : PropertyAttribute {

		/// <summary>
		/// Works like HideInInspector but doesn't prevent DecoratorDrawers from being displayed
		/// </summary>
		public HideVariableAttribute()
		{
			
		}
	}
	
}
	