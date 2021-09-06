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

	[CustomPropertyDrawer(typeof(EditableCommentAttribute))]
	public class EditableCommentDrawer : IllogikaDrawer {
			
		EditableCommentAttribute editableCommentAttribute { get { return ((EditableCommentAttribute)attribute); } }

		private bool inEditMode;

		public override float GetPropertyHeight (SerializedProperty prop, GUIContent label)
		{
			if(string.IsNullOrEmpty(prop.stringValue))
				prop.stringValue = "N		";
			else if(prop.stringValue.Split('	').Length == 1)
				prop.stringValue = "N		" + prop.stringValue;
			else if(prop.stringValue.Split('	').Length == 2)
				prop.stringValue = "N	" + prop.stringValue;

			if(inEditMode)
			{
				return base.GetPropertyHeight(prop, label) + GetTextAreaHeight(prop.stringValue.Split('	')[2]);
			}
			else
			{
				return base.GetPropertyHeight(prop, label) + (GetMessageType(prop.stringValue.Split('	')[0]) != MessageType.None ? Mathf.Max(GetCommentHeight(prop.stringValue.Split('	')[2], GetMessageType(prop.stringValue.Split('	')[0])), GetCommentHeight("\n\n ", GetMessageType(prop.stringValue.Split('	')[0]))) : GetCommentHeight(prop.stringValue.Split('	')[2], GetMessageType(prop.stringValue.Split('	')[0])));
			}
	    }

		public float GetTextAreaHeight(string comment)
		{
			GUIStyle style = GUI.skin.textArea;
			return Mathf.Max(style.CalcHeight(new GUIContent(comment), Screen.width - 35), EditorGUIUtility.singleLineHeight);
		}

		private MessageType GetMessageType(string type)
		{
			if(type == "N")
				return MessageType.None;
			if(type == "I")
				return MessageType.Info;
			if(type == "W")
				return MessageType.Warning;
			if(type == "E")
				return MessageType.Error;

			return MessageType.None;
		}

		private int GetMessageTypeInt(string type)
		{
			if(type == "N")
				return 0;
			if(type == "I")
				return 1;
			if(type == "W")
				return 2;
			if(type == "E")
				return 3;

			return 0;
		}

		string[] messageTypesStrings =
		{
			"None",
			"Info",
			"Warning",
			"Error"
		};

		public override void OnGUI (Rect position, SerializedProperty prop, GUIContent label)
		{
			EditorGUI.BeginProperty(position, label, prop);

			if(prop.propertyType != SerializedPropertyType.String)
			{
				WrongVariableTypeWarning("The EditableComment", "strings");
				return;
			}

			string[] split = prop.stringValue.Split('	');

			int originalIndentLevel = EditorGUI.indentLevel;
			EditorGUI.indentLevel = 0;

			if(!inEditMode)
			{
				// Name
				Rect backgroundRect = position;

				backgroundRect.height = EditorGUIUtility.singleLineHeight;

				GUIStyle headerStyle = GUI.skin.label;
				headerStyle.fontStyle = FontStyle.Bold;

				float textWidth = headerStyle.CalcSize(new GUIContent(split[1])).x;

				Color temp = GUI.color;

				GUI.color = editableCommentAttribute.headerColor;
				EditorGUI.HelpBox(backgroundRect, "", MessageType.None);
				GUI.color = temp;

				Rect nameRect = position;
				nameRect.height = EditorGUIUtility.singleLineHeight + 1;

				nameRect.x += Mathf.Max((nameRect.width - textWidth) / 2, 0);
				nameRect.width = Mathf.Max(nameRect.width, textWidth);
				EditorGUI.LabelField(nameRect, split[1], headerStyle);

				// Edit Button
				Rect buttonRect = position;
				buttonRect.height = EditorGUIUtility.singleLineHeight;
				buttonRect.x += (buttonRect.width - buttonRect.height);
				buttonRect.width = buttonRect.height;
				if(GUI.Button(buttonRect, inEditMode ? editableCommentAttribute.olCheckGreen : editableCommentAttribute.olRefresh, "Label"))
				{
					inEditMode = !inEditMode;
				}

				// Comment
				MessageType messageType = GetMessageType(split[0]);
				position.height -= EditorGUIUtility.singleLineHeight;
				position.y += EditorGUIUtility.singleLineHeight;

				EditorGUI.HelpBox(position, split[2], messageType);
			}
			else
			{
				Rect buttonRect = position;
				buttonRect.height = EditorGUIUtility.singleLineHeight;
				buttonRect.x += (buttonRect.width - buttonRect.height);
				buttonRect.width = buttonRect.height;

				Rect messageTypeRect = position;
				messageTypeRect.height = EditorGUIUtility.singleLineHeight;
				messageTypeRect.width = messageTypeRect.width / 4;
				messageTypeRect.width = Mathf.Max(messageTypeRect.width, 100);

				Rect nameRect = position;
				nameRect.height = EditorGUIUtility.singleLineHeight;
				nameRect.width -= (buttonRect.width + messageTypeRect.width + 2);

				messageTypeRect.x += (nameRect.width + 1);

				position.height -= EditorGUIUtility.singleLineHeight;
				position.y += EditorGUIUtility.singleLineHeight;

				// Name
				EditorGUI.BeginChangeCheck();
				string name = EditorGUI.TextField(nameRect, split[1]);
				if(EditorGUI.EndChangeCheck())
				{
					split[1] = name;
				}

				// Message Type
				EditorGUI.BeginChangeCheck();

				int type = EditorGUI.Popup(messageTypeRect, GetMessageTypeInt(split[0]), messageTypesStrings);
				if(EditorGUI.EndChangeCheck())
				{
					switch(type)
					{
						case 0:
							split[0] = "N";
							break;
						case 1:
							split[0] = "I";
							break;
						case 2:
							split[0] = "W";
							break;
						case 3:
							split[0] = "E";
							break;
					}
				}

				if(GUI.Button(buttonRect, inEditMode ? editableCommentAttribute.olCheckGreen : editableCommentAttribute.olRefresh, "Label"))
				{
					inEditMode = !inEditMode;
				}

				EditorGUI.BeginChangeCheck();
				EditorStyles.textField.wordWrap = true;
				string comment = EditorGUI.TextArea(position, split[2]);
				EditorStyles.textField.wordWrap = false;

				if(EditorGUI.EndChangeCheck())
				{
					split[2] = comment.Replace("	", "    ");
				}

				prop.stringValue = split[0] + "	" + split[1] + "	" + split[2];
			}

			EditorGUI.indentLevel = originalIndentLevel;

			EditorGUI.EndProperty();
		}
	}

}
