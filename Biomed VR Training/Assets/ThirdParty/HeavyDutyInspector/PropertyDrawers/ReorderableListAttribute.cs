//----------------------------------------------
//            Heavy-Duty Inspector
//      Copyright © 2013 - 2017  Illogika
//----------------------------------------------
using UnityEngine;

namespace HeavyDutyInspector
{

	public class ReorderableListAttribute : PropertyAttribute {
		
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
		/// Adds to a list the ability to reorder its content and add or remove items from anywhere in the list.
		/// </summary>
		/// <param name="doubleComponentRefSizeInChildren">If set to <c>true</c> doubles the height of references in children serialized objects (Used if your children use NamedMonoBehaviourAttribute or ComponentSelectionAttribute)</param>
		/// <param name="useNamedMonoBehaviourDrawer">If set to <c>true</c> to display NamedMonoBehaviour with the NamedMonoBehaviour drawer. By default, all Components are displayed with the ComponentSelection drawer.</param>
		[System.Obsolete("The parameters doubleComponentRefSizeInChildren and useNamedMonoBehaviourDrawer don't do anything anymore. Use the parameterless constructor instead.")]
		public ReorderableListAttribute(bool doubleComponentRefSizeInChildren, bool useNamedMonoBehaviourDrawer)
		{
#if UNITY_EDITOR
			arrowUp = (Texture2D)UnityEditor.EditorGUIUtility.Load(Constants.ARROW_UP_PATH);
			arrowDown = (Texture2D)UnityEditor.EditorGUIUtility.Load(Constants.ARROW_DOWN_PATH);
			olPlus = (Texture2D)UnityEditor.EditorGUIUtility.Load(Constants.OL_PLUS_GREEN_PATH);
			olMinus = (Texture2D)UnityEditor.EditorGUIUtility.Load(Constants.OL_MINUS_RED_PATH);
#endif
		}

		/// <summary>
		/// Adds to a list the ability to reorder its content and add or remove items from anywhere in the list.
		/// </summary>
		/// /// <param name="useComponentSelectionDrawer">If set to <c>false</c> the Reorderable List will draw references to objects of type Component with Unity's default object selection Drawer. By default, Reorderable List will use the ComponentSelectionDrawer since you cannot apply both the ComponentSelectionAttribute and the ReorderableList attribute to a List.</param>
		public ReorderableListAttribute(bool useComponentSelectionDrawer = true)
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
