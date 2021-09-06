using UnityEngine;
using UnityEditor;
using System.Linq;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using HeavyDutyInspector;
using Object = UnityEngine.Object;

[CustomPropertyDrawer(typeof(LocalizationKey))]
public class LocalizationKeyDrawer : BaseKeywordDrawer
{

	public LocalizationKeyDrawer()
	{
		scriptableConfig = LocalizationKeys.Config;
		config = scriptableConfig.keyWordCategories;

		base.Init();
	}
}

public static class CreateLocalizationKeys
{
	[MenuItem("Assets/ScriptableObjects/Create New LocalizationKeys")]
	public static void CreateLocalizationKeysConfig()
	{
		KeywordsConfig config = ScriptableObject.CreateInstance<KeywordsConfig>();

		if(!System.IO.Directory.Exists(System.IO.Path.Combine(System.IO.Directory.GetCurrentDirectory(), "Assets/Resources/Config/")))
			System.IO.Directory.CreateDirectory(System.IO.Path.Combine(System.IO.Directory.GetCurrentDirectory(), "Assets/Resources/Config/"));
		
		AssetDatabase.CreateAsset(config, "Assets/Resources/Config/LocalizationKeysConfig.asset");
		AssetDatabase.SaveAssets();
		
		EditorUtility.FocusProjectWindow();
  		Selection.activeObject = config;
	}
}
