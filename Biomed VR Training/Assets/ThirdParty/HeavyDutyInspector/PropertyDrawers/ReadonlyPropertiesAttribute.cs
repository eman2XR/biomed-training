//----------------------------------------------
//            Heavy-Duty Inspector
//      Copyright © 2016 - 2017  Illogika
//----------------------------------------------
using UnityEngine;

namespace HeavyDutyInspector
{

	public class ReadonlyPropertiesAttribute : PropertyAttribute {

        private string[] _properties;
        public string[] Properties
        {
            get
            {
                return _properties;
            }
        }

        private bool _hideVariable;
        public bool HideVariable
        {
            get
            {
                return _hideVariable;
            }
        }

        public ReadonlyPropertiesAttribute(bool hideVariable, params string[] properties)
		{
            _hideVariable = hideVariable;
            _properties = properties;
        }
	}
	
}
	