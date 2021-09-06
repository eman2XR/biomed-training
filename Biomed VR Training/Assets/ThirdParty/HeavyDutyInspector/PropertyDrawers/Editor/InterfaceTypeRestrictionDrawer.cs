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

	[CustomPropertyDrawer(typeof(InterfaceTypeRestrictionAttribute))]
	public class InterfaceTypeRestrictionDrawer : IllogikaDrawer {
			
		InterfaceTypeRestrictionAttribute interfaceTypeRestrictionAttribute { get { return ((InterfaceTypeRestrictionAttribute)attribute); } }
		
		public override float GetPropertyHeight (SerializedProperty prop, GUIContent label)
		{
	       return base.GetPropertyHeight(prop, label);
	    }
		
		public override void OnGUI (Rect position, SerializedProperty prop, GUIContent label)
		{
			EditorGUI.BeginProperty(position, label, prop);

			if(prop.propertyType != SerializedPropertyType.ObjectReference)
			{
				WrongVariableTypeWarning("InterfaceTypeRestriction", "object references");
				return;
			}

			position = EditorGUI.PrefixLabel(position, EditorGUIUtility.GetControlID(FocusType.Passive), label);

			int originalIndentLevel = EditorGUI.indentLevel;
			EditorGUI.indentLevel = 0;

			EditorGUI.BeginChangeCheck();
			GUI.color = Color.clear;
			Object tempGO = EditorGUI.ObjectField(position, prop.objectReferenceValue, typeof(GameObject), interfaceTypeRestrictionAttribute.allowSceneObjects);
			GUI.color = Color.white;

			if(EditorGUI.EndChangeCheck())
			{
				if(tempGO == null)
					prop.objectReferenceValue = null;
				else
					prop.objectReferenceValue = (tempGO as GameObject).GetComponent(interfaceTypeRestrictionAttribute.interfaceType);
			}

			EditorGUI.BeginChangeCheck();
			Object temp = EditorGUI.ObjectField(position, prop.objectReferenceValue, interfaceTypeRestrictionAttribute.interfaceType, interfaceTypeRestrictionAttribute.allowSceneObjects);

			if(EditorGUI.EndChangeCheck())
			{
				prop.objectReferenceValue = temp;
			}

			EditorGUI.indentLevel = originalIndentLevel;

			EditorGUI.EndProperty();
		}
	}

}
