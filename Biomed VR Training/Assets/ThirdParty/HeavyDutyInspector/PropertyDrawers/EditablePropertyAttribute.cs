//----------------------------------------------
//            Heavy-Duty Inspector
//      Copyright © 2015 - 2017  Illogika
//----------------------------------------------
using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace HeavyDutyInspector
{

	public class EditablePropertyAttribute : PropertyAttribute {

		public string accessorName
		{
			get;
			private set;
		}

		public EditablePropertyAttribute()
		{
			accessorName = String.Empty;
		}

		public EditablePropertyAttribute(string accessorName)
		{
			this.accessorName = accessorName;
		}
	}
	
}
	