//----------------------------------------------
//            Heavy-Duty Inspector
//      Copyright © 2016 - 2017  Illogika
//----------------------------------------------
using UnityEngine;

namespace HeavyDutyInspector
{

	public class EditableCommentAttribute : PropertyAttribute {

		public Color headerColor
		{
			get;
			private set;
		}

		public Texture2D olRefresh
		{
			get;
			private set;
		}

		public Texture2D olCheckGreen
		{
			get;
			private set;
		}

		public EditableCommentAttribute()
		{
			headerColor = Color.black;
#if UNITY_EDITOR
			olRefresh = (Texture2D)UnityEditor.EditorGUIUtility.Load(Constants.OL_REFRESH_PATH);
			olCheckGreen = (Texture2D)UnityEditor.EditorGUIUtility.Load(Constants.OL_CHECK_GREEN_PATH);
#endif
		}

		public EditableCommentAttribute(ColorEnum headerColor)
		{
			this.headerColor = ColorEx.GetColorByEnum(headerColor);
#if UNITY_EDITOR
			olRefresh = (Texture2D)UnityEditor.EditorGUIUtility.Load(Constants.OL_REFRESH_PATH);
			olCheckGreen = (Texture2D)UnityEditor.EditorGUIUtility.Load(Constants.OL_CHECK_GREEN_PATH);
#endif
		}

		public EditableCommentAttribute(float r, float g, float b)
		{
			headerColor = new Color(r, g, b);
#if UNITY_EDITOR
			olRefresh = (Texture2D)UnityEditor.EditorGUIUtility.Load(Constants.OL_REFRESH_PATH);
			olCheckGreen = (Texture2D)UnityEditor.EditorGUIUtility.Load(Constants.OL_CHECK_GREEN_PATH);
#endif
		}
	}
	
}
	