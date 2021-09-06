//----------------------------------------------
//            Heavy-Duty Inspector
//      Copyright © 2014 - 2017  Illogika
//----------------------------------------------
using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

namespace HeavyDutyInspector
{

	[CustomPropertyDrawer(typeof(ComplexHeaderAttribute))]
	public class ComplexHeaderDrawer : DecoratorDrawer {
			
		ComplexHeaderAttribute complexHeaderAttribute { get { return ((ComplexHeaderAttribute)attribute); } }
		
		public override float GetHeight ()
		{
	       return base.GetHeight();
	    }
		
		public override void OnGUI (Rect position)
		{
			int originalIndentLevel = EditorGUI.indentLevel;
			EditorGUI.indentLevel = 0;

			Rect background = position;

			background.x = 0;
			background.width += 24;

			GUIStyle headerStyle = GUI.skin.label;
			headerStyle.fontStyle = FontStyle.Bold;

			float textWidth = headerStyle.CalcSize(new GUIContent(complexHeaderAttribute.text)).x;

			Color temp = GUI.color;
			if(complexHeaderAttribute.style == Style.Box)
			{
				GUI.color = complexHeaderAttribute.backgroundColor;
				EditorGUI.HelpBox(background, "", MessageType.None);
			}
			else if(complexHeaderAttribute.style == Style.Line)
			{
				if(complexHeaderAttribute.textAlignment == Alignment.Left)
				{
					GUI.color = complexHeaderAttribute.backgroundColor;
					background.y += background.height / 2;
					background.height = 1;
					background.width = 12;
					GUI.Box(background, "");
					background.x += textWidth + background.width + 5;
					background.width = Screen.width - background.x;
					GUI.Box(background, "");
				}
				else if(complexHeaderAttribute.textAlignment == Alignment.Center)
				{
					GUI.color = complexHeaderAttribute.backgroundColor;
					background.y += background.height / 2;
					background.height = 1;
					background.width /= 2;
					background.width = Mathf.Max(background.width - textWidth / 2, 0);
					GUI.Box(background, "");
					background.x += textWidth + background.width + 5;
					GUI.Box(background, "");
				}
				else if(complexHeaderAttribute.textAlignment == Alignment.Right)
				{
					GUI.color = complexHeaderAttribute.backgroundColor;
					background.y += background.height / 2;
					background.height = 1;
					background.width -= textWidth + 12;
					GUI.Box(background, "");
				}
			}

			GUI.color = complexHeaderAttribute.textColor;
			if(complexHeaderAttribute.textAlignment == Alignment.Left)
			{
				EditorGUI.LabelField(position, complexHeaderAttribute.text, headerStyle);
			}
			else if(complexHeaderAttribute.textAlignment == Alignment.Center)
			{
				position.x += Mathf.Max((position.width - textWidth)/2, 0);
				position.width = Mathf.Max(position.width, textWidth);
				EditorGUI.LabelField(position, complexHeaderAttribute.text, headerStyle);
			}
			else if(complexHeaderAttribute.textAlignment == Alignment.Right)
			{
				position.x += Mathf.Max(position.width - textWidth, 0);
				position.width = Mathf.Max(position.width, textWidth);
				EditorGUI.LabelField(position, complexHeaderAttribute.text, headerStyle);
			}

			GUI.color = temp;

			EditorGUI.indentLevel = originalIndentLevel;
		}
	}

}
