//----------------------------------------------
//            Heavy-Duty Inspector
//      Copyright © 2016 - 2017  Illogika
//----------------------------------------------
using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

namespace HeavyDutyInspector
{

	[CustomPropertyDrawer(typeof(TypeInspectorAttribute))]
	public class TypeInspectorDrawer : IllogikaDrawer {
			
		TypeInspectorAttribute typeInspectorAttribute { get { return ((TypeInspectorAttribute)attribute); } }
		
		public override float GetPropertyHeight (SerializedProperty prop, GUIContent label)
		{
	       return base.GetPropertyHeight(prop, label);
	    }
		
		public override void OnGUI (Rect position, SerializedProperty prop, GUIContent label)
		{
			EditorGUI.BeginProperty(position, label, prop);

			position = EditorGUI.PrefixLabel(position, EditorGUIUtility.GetControlID(FocusType.Passive), label);

			int originalIndentLevel = EditorGUI.indentLevel;
			EditorGUI.indentLevel = 0;

			string typename = prop.FindPropertyRelative("typeName").stringValue;

			System.Type temp = null;		
			if(!string.IsNullOrEmpty(typename))
				temp = typeInspectorAttribute.ExecutingAssembly.GetType(typename);

			if(temp == null && typeInspectorAttribute.defaultTypeValue == DefaultTypeValue.BaseClass)
				temp = typeInspectorAttribute.baseType;

            EditorGUI.BeginChangeCheck();
            temp = typeInspectorAttribute.allTypes[EditorGUI.Popup(position, typeInspectorAttribute.allTypes.IndexOf(temp), typeInspectorAttribute.allNames.ToArray())];

            if(EditorGUI.EndChangeCheck())
            {
				prop.FindPropertyRelative("typeName").stringValue = temp.FullName;
			}

			EditorGUI.indentLevel = originalIndentLevel;

			EditorGUI.EndProperty();
		}
	}

}
