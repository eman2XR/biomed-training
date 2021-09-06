//----------------------------------------------
//            Heavy-Duty Inspector
//      Copyright © 2016 - 2017  Illogika
//----------------------------------------------
using UnityEngine;
using System.Linq;
using System.Collections.Generic;

namespace HeavyDutyInspector
{
	public enum HeaderStyle
	{
		None,
		VariableName,
		FirstElement
	}

	public enum DisplayStyle
	{
		SideBySide,
		SingleLineWithLabel,
		SingleLineWithoutLabel,
	}

	public class CompactViewAttribute : PropertyAttribute
	{
		
		public HeaderStyle headerStyle
		{
			get;
			private set;
		}

		public DisplayStyle displayStyle
		{
			get;
			private set;
		}

		public List<string> relativeProperties
		{
			get;
			private set;
		}

		public string keywordFilePath
		{
			get;
			private set;
		}

		/// <summary>
		/// Display the content of an object with a more compact display than Unity's default.
		/// </summary>
		/// <param name="headerStyle">What to display in the header</param>
		/// <param name="displayStyle">How to display the variables. Side by Side should only be used if you have only two properties to display, three if your first is the header.</param>
		/// <param name="relativeProperties">Relative paths to the properties you want to display.</param>
		public CompactViewAttribute(HeaderStyle headerStyle, DisplayStyle displayStyle, params string[] relativeProperties)
		{
			this.headerStyle = headerStyle;
			this.displayStyle = displayStyle;
			this.relativeProperties = relativeProperties.ToList();
		}

		/// <summary>
		/// Display the content of an object with a more compact display than Unity's default. Use this overload if your header is a keyword.
		/// </summary>
		/// <param name="keywordFilePath">Path to the keyword config file for your keyword.</param>
		/// <param name="displayStyle">How to display the variables. Side by Side should only be used if you have only two properties to display, three if your first is the header.</param>
		/// <param name="relativeProperties">Relative paths to the properties you want to display.</param>
		public CompactViewAttribute(string keywordFilePath, DisplayStyle displayStyle, params string[] relativeProperties)
		{
			this.keywordFilePath = keywordFilePath;
			headerStyle = HeaderStyle.FirstElement;
			this.displayStyle = displayStyle;
			this.relativeProperties = relativeProperties.ToList();
		}
	}
	
}
	