//----------------------------------------------
//            Heavy-Duty Inspector
//      Copyright © 2013 - 2014  Illogika
//----------------------------------------------

using UnityEngine;
using UnityEditor;
#if !UNITY_5_0 && !UNITY_5_1 && !UNITY_5_2
using UnityEditor.SceneManagement;
#endif
using System.IO;
using System.Reflection;
using System.Collections;

public class InstallScriptTemplates{

	[MenuItem("File/Install Script Templates")]
	public static void InstallScriptTemplatesFunc()
	{
		string unityPath = EditorApplication.applicationPath;
		string originPath = Path.Combine(Directory.GetCurrentDirectory(), "Assets/Illogika/ScriptTemplates/");

		string[] paths = Directory.GetFiles(originPath);

		if(Application.platform == RuntimePlatform.OSXEditor)
		{
			unityPath = Path.Combine(unityPath, "Contents/Resources/ScriptTemplates");
		}
		else if(Application.platform == RuntimePlatform.WindowsEditor)
		{
			unityPath = Path.Combine(unityPath.Replace(Path.GetFileName(unityPath), ""), "Data/Resources/ScriptTemplates");
		}

		foreach(string path in paths)
		{
			if(Path.GetExtension(path) == ".txt")
			{
				if(File.Exists(Path.Combine(unityPath, Path.GetFileName(path))))
				   File.Delete(Path.Combine(unityPath, Path.GetFileName(path)));

				File.Copy(path, Path.Combine(unityPath, Path.GetFileName(path)));
				Debug.Log("Copied " + path + " to " + Path.Combine(unityPath, Path.GetFileName(path)));
			}
		}

		EditorUtility.DisplayDialog("Restart Required", "Unity will shut down to complete the installation. You will need to restart it manually.", "OK");
		
#if UNITY_5_0 || UNITY_5_1 || UNITY_5_2
		EditorApplication.SaveCurrentSceneIfUserWantsTo();
#else
		EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo();
#endif
		
		EditorApplication.Exit(1);
	}
}
