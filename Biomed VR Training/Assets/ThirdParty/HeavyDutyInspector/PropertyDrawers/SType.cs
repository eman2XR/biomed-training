//----------------------------------------------
//            Heavy-Duty Inspector
//      Copyright © 2016 - 2017  Illogika
//----------------------------------------------
using UnityEngine;
using System.Reflection;

namespace HeavyDutyInspector
{
    [System.Serializable]
    public class SType
    {
        [SerializeField]
        private string typeName;

        public SType()
        {

        }

        public SType(string typeName)
        {
            this.typeName = typeName;
        }

        public static implicit operator System.Type(SType value)
        {
			if(string.IsNullOrEmpty(value.typeName))
				return null;
			else
				return Assembly.GetExecutingAssembly().GetType(value.typeName);
        }

        public static implicit operator SType(System.Type value)
        {
            return new SType(value.FullName);
        }
    }
}
