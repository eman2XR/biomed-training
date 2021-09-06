//----------------------------------------------
//            Heavy-Duty Inspector
//      Copyright © 2013 - 2017  Illogika
//----------------------------------------------

using UnityEngine;

namespace HeavyDutyInspector
{

	public class ButtonAttribute : PropertyAttribute {
		
		public string buttonText
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
		public ButtonAttribute(string buttonText, string buttonFunction, bool hideVariable = false)
		{
			this.buttonText = buttonText;
			this.buttonFunction = buttonFunction;
			conditionFunction = string.Empty;
			this.hideVariable = hideVariable;
		}

		/// <summary>
		/// Displays a button before the affected variable.
		/// </summary>
		/// <param name="buttonText">Text displayed on the button.</param>
		/// <param name="buttonFunction">The name of the function to be called</param>
		/// <param name="conditionFunction">The name of a function that takes no parameter and returns a boolean. The button will be grayed out and will not work if the function returns false.</param>
		/// <param name="hideVariable">If set to <c>true</c> hides the variable.</param>
		public ButtonAttribute(string buttonText, string buttonFunction, string conditionFunction, bool hideVariable = false)
		{
			this.buttonText = buttonText;
			this.buttonFunction = buttonFunction;
			this.conditionFunction = conditionFunction;
			this.hideVariable = hideVariable;
		}
	}

}
