//----------------------------------------------
//            Heavy-Duty Inspector
//      Copyright © 2016 - 2017  Illogika
//----------------------------------------------
using UnityEngine;
using System;
using System.Reflection;
using System.Collections.Generic;

namespace HeavyDutyInspector
{
	public enum DefaultTypeValue
	{
		Null,
		BaseClass,
	}


	public class TypeInspectorAttribute : PropertyAttribute {

        public List<Type> allTypes
        {
            get;
            private set;
        }

        public List<string> allNames
        {
            get;
            private set;
        }

		public DefaultTypeValue defaultTypeValue
		{
			get;
			private set;
		}

		public Type baseType
		{
			get;
			private set;
		}

		public Assembly ExecutingAssembly
		{
			get
			{
				return Assembly.GetExecutingAssembly();
			}
		}

        public TypeInspectorAttribute(Type typeRestriction, DefaultTypeValue defaultTypeValue = DefaultTypeValue.Null, bool excludeAbstract = false, string excludePredicate = "", params string[] exclude)
		{
            allTypes = new List<Type>();
            allNames = new List<string>();
			this.defaultTypeValue = defaultTypeValue;
			baseType = typeRestriction;

            foreach(Type type in typeRestriction.Assembly.GetTypes())
            {
				if(excludeAbstract && type.IsAbstract)
					continue;

				if(Contains(exclude, type.Name))
					continue;

				if(!string.IsNullOrEmpty(excludePredicate) && NameContainsRecursively(type, excludePredicate))
					continue;

                if(typeRestriction.IsAssignableFrom(type))
                {
					allTypes.Add(type);
                    allNames.Add(type.Name);
                }
            }

			if(defaultTypeValue == DefaultTypeValue.Null)
			{
				allTypes.Insert(0, null);
				allNames.Insert(0, "None");
			}
		}

		private bool NameContainsRecursively(Type type, string name)
		{
			if(type.Name.Contains(name))
				return true;

			Type[] interfaces = type.GetInterfaces();
			for(int i = interfaces.Length - 1; i >= 0; --i)
			{
				if(interfaces[i].Name.Contains(name))
					return true;
			}

			if(type.BaseType == null)
				return false;
			else
				return NameContainsRecursively(type.BaseType, name);
		}

		private bool Contains<T>(IList<T> list, params T[] items)
		{
			foreach(T item in items)
			{
				if(list.Contains(item))
					return true;
			}
			return false;
		}
	}
	
}
	