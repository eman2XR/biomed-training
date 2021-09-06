//----------------------------------------------
//            Heavy-Duty Inspector
//      Copyright © 2016 - 2017  Illogika
//----------------------------------------------
using UnityEngine;
using UnityEditor;
using System;
using System.Reflection;
using System.Collections.Generic;

namespace HeavyDutyInspector
{

	[CustomPropertyDrawer(typeof(ReadonlyPropertiesAttribute))]
	public class ReadonlyPropertiesDrawer : IllogikaDrawer {
			
		ReadonlyPropertiesAttribute readonlyPropertiesAttribute { get { return ((ReadonlyPropertiesAttribute)attribute); } }

        int nbDoubleLineProperties;
        int nbVector3s;

        public override float GetPropertyHeight (SerializedProperty prop, GUIContent label)
		{
            if (propertyInfos.Count == 0)
            {
                foreach (string propertyPath in readonlyPropertiesAttribute.Properties)
                {
                    propertyInfos.Add(prop.serializedObject.targetObject.GetType().GetProperty(propertyPath, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public));
                    propertyTypes.Add(propertyInfos[propertyInfos.Count - 1].PropertyType);
                    if (propertyTypes[propertyTypes.Count - 1] == typeof(Vector4) || propertyTypes[propertyTypes.Count - 1] == typeof(Rect))
                        ++nbDoubleLineProperties;

                    if (propertyTypes[propertyTypes.Count - 1] == typeof(Vector3))
                        ++nbVector3s;
                }
            }

	        return (readonlyPropertiesAttribute.HideVariable ? 0 : base.GetPropertyHeight(prop, label)) + EditorGUIUtility.singleLineHeight * (readonlyPropertiesAttribute.Properties.Length + nbDoubleLineProperties + (Screen.width < 335 ? nbVector3s : 0));
	    }

        List<PropertyInfo> propertyInfos = new List<PropertyInfo>();
        List<Type> propertyTypes = new List<Type>();

        public override void OnGUI (Rect position, SerializedProperty prop, GUIContent label)
		{
			EditorGUI.BeginProperty(position, label, prop);

            position.height = base.GetPropertyHeight(prop, label);
            Rect basePosition = position;

            int originalIndentLevel = EditorGUI.indentLevel;

            if (!readonlyPropertiesAttribute.HideVariable)
            {
                EditorGUI.indentLevel = 0;
                EditorGUI.PropertyField(position, prop);
            }
            else
            {
                basePosition.y -= position.height;
            }

            for(int i = 0; i < propertyInfos.Count; ++i)
            {
                basePosition.y += position.height;
                position = basePosition;

                Rect boxPos = position;
                boxPos.width = Screen.width;
                boxPos.x = 0;
                if (propertyTypes[i] == typeof(Rect) || propertyTypes[i] == typeof(Vector4) || (propertyTypes[i] == typeof(Vector3) && Screen.width < 335))
                    boxPos.height = 2 * EditorGUIUtility.singleLineHeight;
                boxPos.height += 1;
                GUI.color = Color.gray;
                EditorGUI.HelpBox(boxPos, "", MessageType.None);
                GUI.color = Color.white;

                Rect lockPosition = position;
                lockPosition.width = lockPosition.height;
                lockPosition.y += 1;
                lockPosition.x -= 13;
                GUI.Toggle(lockPosition, true, "", "IN LockButton");

                if (propertyTypes[i] != typeof(Quaternion) && propertyTypes[i] != typeof(Vector2) && propertyTypes[i] != typeof(Vector3) && propertyTypes[i] != typeof(Vector4))
                    position = EditorGUI.PrefixLabel(position, EditorGUIUtility.GetControlID(FocusType.Passive), new GUIContent(FormatLikeInspector(readonlyPropertiesAttribute.Properties[i])));

                if (propertyTypes[i] == typeof(AnimationCurve))
                    EditorGUI.CurveField(position, (AnimationCurve)propertyInfos[i].GetGetMethod().Invoke(prop.serializedObject.targetObject, null));
                else if (propertyTypes[i] == typeof(bool))
                    EditorGUI.Toggle(position, (bool)propertyInfos[i].GetGetMethod().Invoke(prop.serializedObject.targetObject, null));
                else if (propertyTypes[i] == typeof(Bounds))
                    EditorGUI.BoundsField(position, (Bounds)propertyInfos[i].GetGetMethod().Invoke(prop.serializedObject.targetObject, null));
                else if (propertyTypes[i] == typeof(Color))
                    EditorGUI.ColorField(position, (Color)propertyInfos[i].GetGetMethod().Invoke(prop.serializedObject.targetObject, null));
                else if (propertyTypes[i] == typeof(float))
                    EditorGUI.SelectableLabel(position, ((float)propertyInfos[i].GetGetMethod().Invoke(prop.serializedObject.targetObject, null)).ToString());
                else if (propertyTypes[i] == typeof(int))
                    EditorGUI.SelectableLabel(position, ((int)propertyInfos[i].GetGetMethod().Invoke(prop.serializedObject.targetObject, null)).ToString());
                else if (propertyTypes[i] == typeof(Quaternion))
                    EditorGUI.Vector3Field(position, new GUIContent(FormatLikeInspector(readonlyPropertiesAttribute.Properties[i])), ((Quaternion)propertyInfos[i].GetGetMethod().Invoke(prop.serializedObject.targetObject, null)).eulerAngles);
                else if (propertyTypes[i] == typeof(Rect))
                {
                    EditorGUI.RectField(position, (Rect)propertyInfos[i].GetGetMethod().Invoke(prop.serializedObject.targetObject, null));
                    basePosition.y += position.height;
                }
                else if (propertyTypes[i] == typeof(string))
                    EditorGUI.SelectableLabel(position, (string)propertyInfos[i].GetGetMethod().Invoke(prop.serializedObject.targetObject, null));
                else if (propertyTypes[i] == typeof(Vector2))
                    EditorGUI.Vector2Field(position, new GUIContent(FormatLikeInspector(readonlyPropertiesAttribute.Properties[i])), (Vector2)propertyInfos[i].GetGetMethod().Invoke(prop.serializedObject.targetObject, null));
                else if (propertyTypes[i] == typeof(Vector3))
                {
                    EditorGUI.Vector3Field(position, new GUIContent(FormatLikeInspector(readonlyPropertiesAttribute.Properties[i])), (Vector3)propertyInfos[i].GetGetMethod().Invoke(prop.serializedObject.targetObject, null));
                    if(Screen.width < 335)
                        basePosition.y += position.height;
                }
                else if (propertyTypes[i] == typeof(Vector4))
                {
                    EditorGUI.Vector4Field(position, FormatLikeInspector(readonlyPropertiesAttribute.Properties[i]), (Vector4)propertyInfos[i].GetGetMethod().Invoke(prop.serializedObject.targetObject, null));
                    basePosition.y += position.height;
                }
                else if (typeof(UnityEngine.Object).IsAssignableFrom(propertyTypes[i]))
                    EditorGUI.ObjectField(position, (UnityEngine.Object)propertyInfos[i].GetGetMethod().Invoke(prop.serializedObject.targetObject, null), propertyTypes[i], true);
				else if (typeof(Enum).IsAssignableFrom(propertyTypes[i]))
					EditorGUI.SelectableLabel(position, (string)propertyInfos[i].GetGetMethod().Invoke(prop.serializedObject.targetObject, null).ToString());
            }

			EditorGUI.indentLevel = originalIndentLevel;

			EditorGUI.EndProperty();
        }
	}

}
