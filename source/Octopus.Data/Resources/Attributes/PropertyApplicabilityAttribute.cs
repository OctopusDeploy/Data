using System;

namespace Octopus.Data.Resources.Attributes
{
    /// <summary>
    /// Use this attribute to specify whether a given property is applicable based on the value of another property
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public abstract class PropertyApplicabilityAttribute : Attribute
    {
        internal PropertyApplicabilityAttribute(PropertyApplicabilityMode mode, string dependsOnPropertyName, object dependsOnPropertyValue)
        {
            Mode = mode;
            DependsOnPropertyName = dependsOnPropertyName;
            DependsOnPropertyValue = dependsOnPropertyValue;
        }
        
        public PropertyApplicabilityMode Mode { get; }
        
        public string DependsOnPropertyName { get; }
        
        public object DependsOnPropertyValue { get; }
    }

    public class ApplicableWhenAnyValueAttribute : PropertyApplicabilityAttribute
    {
        public ApplicableWhenAnyValueAttribute(string dependsOnPropertyName) : 
            base(PropertyApplicabilityMode.ApplicableIfHasAnyValue, dependsOnPropertyName, null)
        {}
    }

    public class ApplicableWhenNoValueAttribute : PropertyApplicabilityAttribute
    {
        public ApplicableWhenNoValueAttribute(string dependsOnPropertyName) : 
            base(PropertyApplicabilityMode.ApplicableIfHasNoValue, dependsOnPropertyName, null)
        {}
    }

    public class ApplicableWhenSpecificValueAttribute : PropertyApplicabilityAttribute
    {
        public ApplicableWhenSpecificValueAttribute(string dependsOnPropertyName, object dependsOnPropertyValue) : 
            base(PropertyApplicabilityMode.ApplicableIfSpecificValue, dependsOnPropertyName, dependsOnPropertyValue)
        {}
    }

    public class ApplicableWhenNotSpecificValueAttribute : PropertyApplicabilityAttribute
    {
        public ApplicableWhenNotSpecificValueAttribute(string dependsOnPropertyName, object dependsOnPropertyValue) : 
            base(PropertyApplicabilityMode.ApplicableIfNotSpecificValue, dependsOnPropertyName, dependsOnPropertyValue)
        {}
    }
}