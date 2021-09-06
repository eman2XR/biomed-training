using UnityEngine;
using System.Linq;
using System.Collections;
using HeavyDutyInspector;
using Object = UnityEngine.Object;

[System.Serializable]
public class LocalizationKey : Keyword {

	public LocalizationKey() : base()
	{
	}

	private LocalizationKey(string key) : base(key)
	{
	}

	public static implicit operator string (LocalizationKey word)
	{
		return word._key.Split('/').Last();
	}

	public static implicit operator LocalizationKey (string key)
	{
		return new LocalizationKey (key);
	}
}

public class LocalizationKeys : System.Object
{

	private static KeywordsConfig _config;
	public static KeywordsConfig Config
	{
		get
		{
			if (_config == null)
			{
				_config = Resources.Load("Config/LocalizationKeysConfig") as KeywordsConfig;
			}
			return _config;
		}
	}
}
