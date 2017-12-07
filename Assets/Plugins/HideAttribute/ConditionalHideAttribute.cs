using UnityEngine;
using System;

[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property |
	AttributeTargets.Class | AttributeTargets.Struct, Inherited = true)]
public class ConditionalHideAttribute : PropertyAttribute {
	//The name of the bool field that will be in control
	public string ConditionalSourceField = "";
	//TRUE = Hide in inspector / FALSE = Disable in inspector 
	public bool HideInInspector = false;
	public int targetValue;
    public bool isInt;
    
    /// <summary>
    /// Hide the Attribute if the specified bool is true.
    /// </summary>
    /// <param name="conditionalSourceField"> The boolean property deciding whether to show the attribute or not. </param>
    /// <param name="hideInInspector"> Hide in inspector ? If false, simply disable. </param>
	public ConditionalHideAttribute(string conditionalSourceField, bool hideInInspector = true) {
		this.ConditionalSourceField = conditionalSourceField;
		this.HideInInspector = hideInInspector;
        isInt = false;
    }

    /// <summary>
    /// Hide the Attribute if the specified int is equal to targetValue.
    /// </summary>
    /// <param name="conditionalSourceField"> The integer property deciding whether to show the attribute or not. </param>
    /// <param name="targetValue"> The value the integer property must be equal to to show the attribute. </param>
    /// <param name="hideInInspector"> Hide in inspector ? If false, simply disable. </param>
    public ConditionalHideAttribute(string conditionalSourceField, int targetValue, bool hideInInspector = true) {
		this.ConditionalSourceField = conditionalSourceField;
		this.HideInInspector = hideInInspector;
		this.targetValue = targetValue;
        isInt = true;
	}
}