//----------------------------------------------
//            Heavy-Duty Inspector
//      Copyright © 2013 - 2017  Illogika
//----------------------------------------------
using UnityEngine;
using System.Collections;
using HeavyDutyInspector;

public class ExampleAssetPath : MonoBehaviour {

	[Comment("Don't let typing errors slow you down. Slide an asset in what looks like a reference and store its path as a string instead. The path is also convieniently displayed under the reference in a selectable Label to allow easy Copy/Pasting.", CommentType.Info, 0)]
	[Comment("Paths can be saved relative to the Asset folder.", CommentType.None, 1)]
	// Get a path relative to the Asset folder
	[AssetPath(typeof(Texture2D), PathOptions.RelativeToAssets)]
	public string illogikaLogoPath;

	[Comment("Relative to the last \"Resources\" folder in the file's hierarchy.", CommentType.None)]
	// Get a path relative a folder named Resource, with no file extension, to use with Resource.Load
	[AssetPath(typeof(Texture2D), PathOptions.RelativeToResources)]
	public string heavyDutyInspectorLogoPath;

	[Comment("Or you can keep just the file's name. Filename Only no longer restores the reference to the original object as this was causing too much lag in big projects.", CommentType.None, 1)]
	// Get the filename only (with the extension)
	[AssetPath(typeof(TextAsset), PathOptions.FilenameOnly)]
	public string scriptTemplatePath;
}
