//----------------------------------------------
//            Heavy-Duty Inspector
//      Copyright © 2016 - 2017  Illogika
//----------------------------------------------
using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace HeavyDutyInspector
{
	public class InterfaceTypeRestrictionAttribute : PropertyAttribute
	{
		
		public Type interfaceType
		{
			get;
			private set;
		}

		public bool allowSceneObjects
		{
			get;
			private set;
		}

		public InterfaceTypeRestrictionAttribute(Type interfaceType, bool allowSceneObjects = true)
		{
			this.interfaceType = interfaceType;
			this.allowSceneObjects = allowSceneObjects;
		}
	}	
}
	