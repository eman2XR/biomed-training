//----------------------------------------------
//            Heavy-Duty Inspector
//      Copyright © 2013 - 2017  Illogika
//----------------------------------------------
using UnityEngine;

namespace HeavyDutyInspector
{

	public class ReorderableArrayAttribute : PropertyAttribute {
		
		public Texture2D arrowUp
		{
			get;
			private set;
		}

		public Texture2D arrowDown
		{
			get;
			private set;
		}

        public Texture2D olPlus
        {
            get;
            private set;
        }

        public Texture2D olMinus
        {
            get;
            private set;
        }

		public bool useDefaultComponentDrawer
		{
			get;
			private set;
		}

		/// <summary>
		/// Adds to an array the ability to reorder its content and add or remove items from anywhere in the array.
		/// </summary>
		/// /// <param name="useComponentSelectionDrawer">If set to <c>false</c> the Reorderable Array will draw references to objects of type Component with Unity's default object selection Drawer. By default, Reorderable Array will use the ComponentSelectionDrawer since you cannot apply both the ComponentSelectionAttribute and the ReorderableArray attribute to a List.</param>
		public ReorderableArrayAttribute(bool useComponentSelectionDrawer = true)
		{
#if UNITY_EDITOR
			arrowUp = (Texture2D)UnityEditor.EditorGUIUtility.Load(Constants.ARROW_UP_PATH);
			arrowDown = (Texture2D)UnityEditor.EditorGUIUtility.Load(Constants.ARROW_DOWN_PATH);
			olPlus = (Texture2D)UnityEditor.EditorGUIUtility.Load(Constants.OL_PLUS_GREEN_PATH);
			olMinus = (Texture2D)UnityEditor.EditorGUIUtility.Load(Constants.OL_MINUS_RED_PATH);
#endif
			useDefaultComponentDrawer = !useComponentSelectionDrawer;

		}
	}

}
