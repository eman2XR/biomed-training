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

	[CustomPropertyDrawer(typeof(CompactViewAttribute))]
	public class CompactViewDrawer : IllogikaDrawer {
			
		CompactViewAttribute compactViewAttribute { get { return ((CompactViewAttribute)attribute); } }

		private Dictionary<string, int> arrayCount = new Dictionary<string, int>();

		public override float GetPropertyHeight (SerializedProperty prop, GUIContent label)
		{
			int multiplier = compactViewAttribute.relativeProperties.Count;

			if(compactViewAttribute.displayStyle == DisplayStyle.SideBySide)
			{
				multiplier = 1;
			}

			if(compactViewAttribute.headerStyle == HeaderStyle.VariableName)
				multiplier += 1;

			bool countArrays = false;
			if(!arrayCount.ContainsKey(prop.propertyPath))
			{
				arrayCount.Add(prop.propertyPath, 0);
				countArrays = true;
			}

			int maxSize = 0;
			foreach(string str in compactViewAttribute.relativeProperties)
			{
				if(prop.FindPropertyRelative(str).isArray && prop.FindPropertyRelative(str).propertyType != SerializedPropertyType.String)
				{
					if(countArrays)
						++arrayCount[prop.propertyPath];

					if(compactViewAttribute.displayStyle == DisplayStyle.SideBySide)
						maxSize = Mathf.Max(maxSize, prop.FindPropertyRelative(str).arraySize);
					else
						multiplier = Mathf.Max(multiplier, multiplier + prop.FindPropertyRelative(str).arraySize);
				}
			}

			if(compactViewAttribute.displayStyle == DisplayStyle.SideBySide)
				multiplier += maxSize;

			return base.GetPropertyHeight(prop, label) * multiplier + 6 + (compactViewAttribute.headerStyle == HeaderStyle.FirstElement ? 4 : 0) + multiplier - 1;
	    }

		Texture2D olPlus
		{
			get;
			set;
		}

		Texture2D olMinus
		{
			get;
			set;
		}

		public CompactViewDrawer()
		{
			olPlus = (Texture2D)EditorGUIUtility.Load(Constants.OL_PLUS_GREEN_PATH);
			olMinus = (Texture2D)EditorGUIUtility.Load(Constants.OL_MINUS_RED_PATH);
		}

		public override void OnGUI (Rect position, SerializedProperty prop, GUIContent label)
		{
			EditorGUI.BeginProperty(position, label, prop);

			if(compactViewAttribute.displayStyle == DisplayStyle.SideBySide)
			{
				if((compactViewAttribute.headerStyle == HeaderStyle.FirstElement && compactViewAttribute.relativeProperties.Count > 3) || compactViewAttribute.relativeProperties.Count > 2)
					Debug.LogError("When using the Side By Side style, CompactView can only display the first two variables, three if one of them is displayed in the header. To display more variables, use another style.");
			}

			position.width -= EditorGUI.indentLevel * 10;
			position.x += EditorGUI.indentLevel * 10;

			EditorGUI.indentLevel = 1;

			Rect backgroundRect = new Rect(position);
			backgroundRect.width -= 10;
			backgroundRect.x += 10;

			position.y += 2;
			if(compactViewAttribute.displayStyle == DisplayStyle.SideBySide)
				position.width -= 16 * arrayCount[prop.propertyPath];
			else
				position.width -= 4;

			position.width *= 0.5f;
			position.height = EditorGUIUtility.singleLineHeight;

			Rect headerRect = new Rect(backgroundRect);
			if(compactViewAttribute.headerStyle != HeaderStyle.None)
			{
				headerRect.height = EditorGUIUtility.singleLineHeight + (compactViewAttribute.headerStyle == HeaderStyle.FirstElement ?  4 : 0);
				headerRect.width *= 0.5f;

				if(compactViewAttribute.headerStyle == HeaderStyle.FirstElement)
					headerRect.y += 4;

				GUI.Box(headerRect, "", "RL Header");

				if(compactViewAttribute.headerStyle == HeaderStyle.FirstElement)
				{
					headerRect.y -= 4;
					GUI.Box(headerRect, "", "RL Header");
				}

				if(compactViewAttribute.headerStyle == HeaderStyle.VariableName)
				{
					headerRect.x += 5;
					headerRect.width -= 5;
					EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);
				}
				else
				{
					if(prop.FindPropertyRelative(compactViewAttribute.relativeProperties[0]).isArray && prop.FindPropertyRelative(compactViewAttribute.relativeProperties[0]).propertyType != SerializedPropertyType.String)
						Debug.LogError("Cannot display the first variable in a Compact View's header if it is an Array or List.");

					if(string.IsNullOrEmpty(compactViewAttribute.keywordFilePath))
						DrawDefaultGUIWithoutLabel(position, prop.FindPropertyRelative(compactViewAttribute.relativeProperties[0]));
					else
					{
						Rect keywordPos = position;
						keywordPos.x += 14;
						keywordPos.width -= 14;
						EditorGUI.BeginChangeCheck();
						string temp = EditorGUIEx.KeywordField(keywordPos, prop.FindPropertyRelative(compactViewAttribute.relativeProperties[0]).FindPropertyRelative("_key").stringValue, Resources.Load<KeywordsConfig>(compactViewAttribute.keywordFilePath), true);

						if(EditorGUI.EndChangeCheck())
							prop.FindPropertyRelative(compactViewAttribute.relativeProperties[0]).FindPropertyRelative("_key").stringValue = temp;
					}
				}
				position.y += headerRect.height;
			}
			else
			{
				headerRect.height = 0;
			}

			backgroundRect.height -= headerRect.height;
			backgroundRect.y += headerRect.height;

			GUI.Box(backgroundRect, "", "RL Background");

			int startIndex = (compactViewAttribute.headerStyle == HeaderStyle.FirstElement ? 1 : 0);
			if(compactViewAttribute.displayStyle == DisplayStyle.SideBySide)
			{
				SerializedProperty firstProp = prop.FindPropertyRelative(compactViewAttribute.relativeProperties[startIndex]);
				if(firstProp.isArray && firstProp.propertyType != SerializedPropertyType.String)
				{
					Rect firstPosition = new Rect(position);
					firstPosition.width += 8;
					Rect buttonRect = new Rect(firstPosition);
					buttonRect.x += firstPosition.width + 1;
					buttonRect.width = 16;
					buttonRect.y += 1;
					for(int i = 0; i < firstProp.arraySize; ++i)
					{
						DrawDefaultGUIWithoutLabel(firstPosition, firstProp.GetArrayElementAtIndex(i));

						if(GUI.Button(buttonRect, olMinus, "Label"))
						{
							firstProp.DeleteArrayElementAtIndex(i);
						}

						firstPosition.y += firstPosition.height + 1;
						buttonRect.y += firstPosition.height + 1;
					}

					if(GUI.Button(buttonRect, olPlus, "Label"))
					{
						firstProp.InsertArrayElementAtIndex(firstProp.arraySize);
					}

					position.x += 16;
				}
				else
				{
					if(arrayCount[prop.propertyPath] > 0)
						position.width += 8;
					else
						position.width += 4;

					DrawDefaultGUIWithoutLabel(position, firstProp);

					if(arrayCount[prop.propertyPath] > 0)
						position.width -= 8;
				}

				if(arrayCount[prop.propertyPath] > 0)
					position.x += position.width - 4;
				else
					position.x += position.width - 12;

				SerializedProperty secondProp = prop.FindPropertyRelative(compactViewAttribute.relativeProperties[startIndex + 1]);
				if(secondProp.isArray && secondProp.propertyType != SerializedPropertyType.String)
				{
					Rect secondPosition = new Rect(position);
					Rect buttonRect = new Rect(secondPosition);
					buttonRect.x += position.width + 1;
					buttonRect.width = 16;
					buttonRect.y += 1;
					for(int i = 0; i < secondProp.arraySize; ++i)
					{
						DrawDefaultGUIWithoutLabel(secondPosition, secondProp.GetArrayElementAtIndex(i));

						if(GUI.Button(buttonRect, olMinus, "Label"))
						{
							secondProp.DeleteArrayElementAtIndex(i);
						}


						secondPosition.y += secondPosition.height + 1;
						buttonRect.y += secondPosition.height + 1;
					}

					if(GUI.Button(buttonRect, olPlus, "Label"))
					{
						secondProp.InsertArrayElementAtIndex(secondProp.arraySize);
					}
				}
				else
				{
					DrawDefaultGUIWithoutLabel(position, secondProp);
				}
			}
			else
			{
				position.width *= 2;

				for(int i = startIndex; i < compactViewAttribute.relativeProperties.Count; ++i)
				{
					SerializedProperty innerProp = prop.FindPropertyRelative(compactViewAttribute.relativeProperties[i]);
					if(innerProp.isArray && innerProp.propertyType != SerializedPropertyType.String)
					{
						Rect innerPosition = new Rect(position);
						innerPosition.width -= 17;
						float totalHeight = innerPosition.height;
						Rect buttonRect = new Rect(innerPosition);
						buttonRect.x += innerPosition.width + 1;
						buttonRect.width = 16;
						buttonRect.y += 1;

						if(compactViewAttribute.displayStyle == DisplayStyle.SingleLineWithLabel)
						{
							innerPosition = EditorGUI.PrefixLabel(innerPosition, new GUIContent(ObjectNames.NicifyVariableName(innerProp.name)));
						}

						for(int j = 0; j < innerProp.arraySize; ++j)
						{
							DrawDefaultGUIWithoutLabel(innerPosition, innerProp.GetArrayElementAtIndex(j));

							if(GUI.Button(buttonRect, olMinus, "Label"))
							{
								innerProp.DeleteArrayElementAtIndex(j);
							}

							innerPosition.y += innerPosition.height + 1;
							buttonRect.y += innerPosition.height + 1;
							totalHeight += innerPosition.height + 1;
						}

						position.y += totalHeight + 1;

						if(GUI.Button(buttonRect, olPlus, "Label"))
						{
							innerProp.InsertArrayElementAtIndex(innerProp.arraySize);
						}
					}
					else
					{
						if(compactViewAttribute.displayStyle == DisplayStyle.SingleLineWithoutLabel)
							DrawDefaultGUIWithoutLabel(position, innerProp);
						else
							EditorGUI.PropertyField(position, innerProp);

						position.y += position.height + 1;
					}
				}
			}

			EditorGUI.EndProperty();
		}
	}

}
