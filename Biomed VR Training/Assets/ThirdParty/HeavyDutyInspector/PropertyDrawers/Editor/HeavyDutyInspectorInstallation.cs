//----------------------------------------------
//            Heavy-Duty Inspector
//         Copyright © 2017  Illogika
//----------------------------------------------
using UnityEngine;
using UnityEditor;
using System.IO;
using System.Linq;
using System.Collections.Generic;

namespace HeavyDutyInspector
{
	[InitializeOnLoad]
	public static class HeavyDutyInspectorInstallation
	{
		private const string HEAVY_DUTY_INSPECTOR_DEFINE = "HEAVY_DUTY_INSPECTOR";

		static HeavyDutyInspectorInstallation()
		{
			List<string> defines = PlayerSettings.GetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup).Split(';').ToList();

			if(!defines.Contains(HEAVY_DUTY_INSPECTOR_DEFINE))
			{
				defines.Add(HEAVY_DUTY_INSPECTOR_DEFINE);
				PlayerSettings.SetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup, string.Join(";", defines.ToArray()));
			}

			if (!AssetDatabase.IsValidFolder("Assets/Editor Default Resources"))
				AssetDatabase.CreateFolder("Assets", "Editor Default Resources");

			if (!AssetDatabase.IsValidFolder("Assets/Editor Default Resources/HeavyDutyInspectorResources"))
				AssetDatabase.CreateFolder("Assets/Editor Default Resources", "HeavyDutyInspectorResources");

			InstallAsset("Assets/Illogika/HeavyDutyInspector/PropertyDrawers/Resources/-.prefab", "Assets/Editor Default Resources/HeavyDutyInspectorResources/-.prefab");
			InstallAsset("Assets/Illogika/HeavyDutyInspector/PropertyDrawers/Resources/ArrowDown.png", "Assets/Editor Default Resources/HeavyDutyInspectorResources/ArrowDown.png");
			InstallAsset("Assets/Illogika/HeavyDutyInspector/PropertyDrawers/Resources/ArrowUp.png", "Assets/Editor Default Resources/HeavyDutyInspectorResources/ArrowUp.png");
			InstallAsset("Assets/Illogika/HeavyDutyInspector/PropertyDrawers/Resources/OLCheckGreen.png", "Assets/Editor Default Resources/HeavyDutyInspectorResources/OLCheckGreen.png");
			InstallAsset("Assets/Illogika/HeavyDutyInspector/PropertyDrawers/Resources/OLMinusRed.png", "Assets/Editor Default Resources/HeavyDutyInspectorResources/OLMinusRed.png");
			InstallAsset("Assets/Illogika/HeavyDutyInspector/PropertyDrawers/Resources/OLPlusGreen.png", "Assets/Editor Default Resources/HeavyDutyInspectorResources/OLPlusGreen.png");
			InstallAsset("Assets/Illogika/HeavyDutyInspector/PropertyDrawers/Resources/OLRefresh.png", "Assets/Editor Default Resources/HeavyDutyInspectorResources/OLRefresh.png");
			InstallAsset("Assets/Illogika/HeavyDutyInspector/PropertyDrawers/Resources/OLXRed.png", "Assets/Editor Default Resources/HeavyDutyInspectorResources/OLXRed.png");
		}

		private static void InstallAsset(string oldPath, string newPath)
		{
			if (AssetDatabase.LoadMainAssetAtPath(newPath) == null)
			{
				if (AssetDatabase.IsValidFolder("Assets/AssetStoreTools"))
				{
					AssetDatabase.CopyAsset(oldPath, newPath);
				}
				else 
				{
					AssetDatabase.MoveAsset(oldPath, newPath);
				}

				string dirPath = Path.Combine(Directory.GetCurrentDirectory(), oldPath.Replace(Path.GetFileName(oldPath), ""));
				if (Directory.GetFiles(dirPath).Length == 0)
				{
					dirPath = dirPath.Substring(0, dirPath.Length - 1);
					Directory.Delete(dirPath);
					File.Delete(dirPath + ".meta");
					AssetDatabase.Refresh();
				}
			}
		}
	}
}
