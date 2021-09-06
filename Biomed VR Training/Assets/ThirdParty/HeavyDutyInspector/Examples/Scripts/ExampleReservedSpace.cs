//----------------------------------------------
//            Heavy-Duty Inspector
//         Copyright © 2017  Illogika
//----------------------------------------------
using UnityEngine;
using HeavyDutyInspector;

public class ExampleReservedSpace : MonoBehaviour
{
	public float beginningOfDefaultInspector;

	[Comment("With the ReservedSpace attribute, we can add a space in the inspector and later get its Rect to use it in a custom inspector.\n\nNo need to chose between a custom inspector and a default inspector with property drawers anymore.\n\nIn this example, I used the reserved space to display a scroll view.", CommentType.Info, order = 0)]
	[ReservedSpace(304, order = 1)]
	[Comment("Be sure to also check out the editor script associated with this example for the implementation of the custom inspector used in this example.", CommentType.Warning, order = 2)]
#if UNITY_5_0
	public float aVariable;
#endif
	[Button("Ping Editor Script", "PingEditorScript", order = 3)]
	public float endOfDefaultInspector;

	public void PingEditorScript()
	{
#if UNITY_EDITOR
		foreach(string path in UnityEditor.AssetDatabase.GetAllAssetPaths())
		{
			if(path.Contains("ExampleReservedSpaceEditor"))
			{
				UnityEditor.EditorGUIUtility.PingObject(UnityEditor.AssetDatabase.LoadMainAssetAtPath(path));
			}
		}
#endif
	}
}
