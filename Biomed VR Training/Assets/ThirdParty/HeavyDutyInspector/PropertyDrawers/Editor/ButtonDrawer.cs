//----------------------------------------------
//            Heavy-Duty Inspector
//      Copyright © 2013 - 2017  Illogika
//----------------------------------------------

using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Reflection;

namespace HeavyDutyInspector
{

	[CustomPropertyDrawer(typeof(ButtonAttribute))]
	public class ButtonDrawer : IllogikaDrawer {
				
		ButtonAttribute buttonAttribute { get { return ((ButtonAttribute)attribute); } }
		
		bool ShowVariable(SerializedProperty prop)
		{
			bool showVariable = !buttonAttribute.hideVariable;				
			return showVariable;
		}
		
		bool isInteractable(SerializedProperty prop)
		{
			bool interactable = string.IsNullOrEmpty(buttonAttribute.conditionFunction);

			if(!interactable)
			{
				MonoBehaviour go = prop.serializedObject.targetObject as MonoBehaviour;
				if(go != null)
				{
					try
					{
						interactable = (bool)CallMethod(prop, go, buttonAttribute.conditionFunction);
					}
					catch { }
				}
				else
				{
					ScriptableObject so = prop.serializedObject.targetObject as ScriptableObject;
					if(so != null)
					{
						try
						{
							interactable = (bool)CallMethod(prop, so, buttonAttribute.conditionFunction);
						}
						catch { }
					}
				}
			}
			return interactable;
		}
			
		public override float GetPropertyHeight (SerializedProperty prop, GUIContent label)
		{
			float baseHeight = base.GetPropertyHeight(prop, label);
			return ShowVariable(prop) ? baseHeight * 2 : baseHeight;
	    }

		public override void OnGUI (Rect position, SerializedProperty prop, GUIContent label)
		{
			EditorGUI.BeginProperty(position, label, prop);

			bool showVariable = ShowVariable(prop);
				
			if (showVariable)
				position.height /= 2;

			bool interactable = isInteractable(prop);
			Color baseColor = GUI.color;

			if(!interactable)
			{
				GUI.color = Color.gray;
			}

			if(GUI.Button(EditorGUI.IndentedRect(position), buttonAttribute.buttonText) && interactable)
			{
				foreach(Object obj in prop.serializedObject.targetObjects)
				{
					MonoBehaviour go = obj as MonoBehaviour;
					if (go != null)
					{
						CallMethod(prop, go, buttonAttribute.buttonFunction);
					}
					else
					{
						ScriptableObject so = obj as ScriptableObject;
						if(so != null)
						{
							CallMethod(prop, so, buttonAttribute.buttonFunction);
						}
					}
				}
			}

			GUI.color = baseColor;
				
			if (showVariable)
				position.y += position.height;
				
			if(showVariable)
			{
				EditorGUI.PropertyField(position, prop);	
			}

			EditorGUI.EndProperty();
			
		}
	}

}
