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

	[CustomPropertyDrawer(typeof(ChangeCheckCallbackAttribute))]
	public class ChangeCheckCallbackDrawer : IllogikaDrawer {
			
		ChangeCheckCallbackAttribute changeCheckCallbackAttribute { get { return ((ChangeCheckCallbackAttribute)attribute); } }
		
		public override float GetPropertyHeight (SerializedProperty prop, GUIContent label)
		{
	       return base.GetPropertyHeight(prop, label);
	    }

		bool waitingForCallback = false;

		public override void OnGUI (Rect position, SerializedProperty prop, GUIContent label)
		{
			EditorGUI.BeginProperty(position, label, prop);

			EditorGUI.BeginChangeCheck();

			EditorGUI.PropertyField(position, prop);

			if(EditorGUI.EndChangeCheck() && !waitingForCallback)
			{
				waitingForCallback = true;
				try{
					if(typeof(MonoBehaviour).IsAssignableFrom(prop.serializedObject.targetObject.GetType()))
						(prop.serializedObject.targetObject as MonoBehaviour).StartCoroutine(WaitForCallback(prop));
					else
						GameObject.FindObjectOfType<MonoBehaviour>().StartCoroutine(WaitForCallback(prop));
				}
				catch{
					GameObject proxyObj = GameObject.FindObjectOfType<GameObject>();
					if(proxyObj == null)
					{
						proxyObj = new GameObject("WaitForCallback", typeof(MonoBehaviour));
						proxyObj.GetComponent<MonoBehaviour>().StartCoroutine(WaitForCallback(prop, proxyObj));
					}
					else
					{
						MonoBehaviour proxy = proxyObj.AddComponent<MonoBehaviour>();
						proxy.StartCoroutine(WaitForCallback(prop, proxy));
					}
				}
			}

			EditorGUI.EndProperty();
		}

		IEnumerator WaitForCallback(SerializedProperty prop, UnityEngine.Object proxy = null)
		{
			yield return null;
			foreach(Object obj in prop.serializedObject.targetObjects)
			{
				MonoBehaviour go = obj as MonoBehaviour;
				if (go != null)
				{
					CallMethod(prop, go, changeCheckCallbackAttribute.callback);
				}
				else
				{
					ScriptableObject so = obj as ScriptableObject;
					if(so != null)
					{
						CallMethod(prop, so, changeCheckCallbackAttribute.callback);
					}
				}
			}
			waitingForCallback = false;
			if(proxy != null)
				GameObject.DestroyImmediate(proxy);
		}
	}

}
