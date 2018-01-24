/* Adapted from http://www.brechtos.com/hiding-or-disabling-inspector-properties-using-propertydrawers-within-unity-5/ */

using UnityEngine;
using System;
using System.Collections;

[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property |
	AttributeTargets.Class | AttributeTargets.Struct, Inherited = true)]
public class ConditionalHideAttribute : PropertyAttribute {
	//The name of the bool field that will be in control
	public string ConditionalSourceField = "";
	//TRUE = Disables on true / FALSE = Disables on false
	public bool DisableOnTrue = false;
	//TRUE = Hide in inspector / FALSE = Disable in inspector 
	public bool HideInInspector = false;

	public ConditionalHideAttribute(string conditionalSourceField) {
		this.ConditionalSourceField = conditionalSourceField;
		this.DisableOnTrue = false;
		this.HideInInspector = false;
	}

	public ConditionalHideAttribute(string conditionalSourceField, bool disableOnTrue) {
		this.ConditionalSourceField = conditionalSourceField;
		this.DisableOnTrue = disableOnTrue;
		this.HideInInspector = false;
	}

	public ConditionalHideAttribute(string conditionalSourceField, bool disableOnTrue, bool hideInInspector) {
		this.ConditionalSourceField = conditionalSourceField;
		this.DisableOnTrue = disableOnTrue;
		this.HideInInspector = hideInInspector;
	}
}