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

	public class ComponentFieldRestrictionAttribute : PropertyAttribute
	{

		public Type componentType
		{
			get;
			private set;
		}

		public Type endMemberType
		{
			get;
			private set;
		}

		public ComponentFieldRestrictionAttribute(Type endMemberType)
		{
			this.endMemberType = endMemberType;
		}

		public ComponentFieldRestrictionAttribute(Type endMemberType, Type componentType)
		{
			this.componentType = componentType;
			this.endMemberType = endMemberType;
		}
	}
}
	