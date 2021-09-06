//----------------------------------------------
//            Heavy-Duty Inspector
//      Copyright © 2014 - 2017  Illogika
//----------------------------------------------
using UnityEngine;

namespace HeavyDutyInspector
{

	public class ImageButtonAttribute : PropertyAttribute {

		public string imagePath
		{
			get;
			private set;
		}
		
		public string buttonFunction
		{
			get;
			private set;
		}

		public string conditionFunction
		{
			get;
			private set;
		}

		public bool hideVariable
		{
			get;
			private set;
		}
		
		/// <summary>
		/// Displays a button before the affected variable.
		/// </summary>
		/// <param name="buttonText">Text displayed on the button.</param>
		/// <param name="buttonFunction">The name of the function to be called</param>
		/// <param name="hideVariable">If set to <c>true</c> hides the variable.</param>
		public ImageButtonAttribute(string imagePath, string buttonFunction, bool hideVariable = false)
		{
			if(imagePath.ToLower().Substring(0, 7).Equals("assets/"))
				imagePath = imagePath.Substring(7);

			this.imagePath = imagePath;
			this.buttonFunction = buttonFunction;
			conditionFunction = string.Empty;
			this.hideVariable = hideVariable;
		}

		/// <summary>
		/// Displays a button before the affected variable.
		/// </summary>
		/// <param name="buttonText">Text displayed on the button.</param>
		/// <param name="buttonFunction">The name of the function to be called</param>
		/// <param name="hideVariable">If set to <c>true</c> hides the variable.</param>
		public ImageButtonAttribute(string imagePath, string buttonFunction, string conditionFunction, bool hideVariable = false)
		{
			if(imagePath.ToLower().Substring(0, 7).Equals("assets/"))
				imagePath = imagePath.Substring(7);

			this.imagePath = imagePath;
			this.buttonFunction = buttonFunction;
			this.conditionFunction = conditionFunction;
			this.hideVariable = hideVariable;
		}
	}

}
