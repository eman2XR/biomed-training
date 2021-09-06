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

	[CustomPropertyDrawer(typeof(SType))]
	public class STypeDrawer : IllogikaDrawer
    {		
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

            float tempWidth = position.width / 3;
            position.width -= tempWidth;

            EditorGUI.SelectableLabel(position, prop.FindPropertyRelative("typeName").stringValue);

            position.x += position.width;
            position.width = tempWidth;

            EditorGUI.BeginChangeCheck();

            MonoScript obj = EditorGUI.ObjectField(position, null, typeof(MonoScript), false) as MonoScript;

            if(EditorGUI.EndChangeCheck())
            {
                if(obj == null)
                    prop.FindPropertyRelative("typeName").stringValue = string.Empty;
                else
                    prop.FindPropertyRelative("typeName").stringValue = obj.GetClass().FullName;
            }

			EditorGUI.indentLevel = originalIndentLevel;

			EditorGUI.EndProperty();
		}
	}

}
