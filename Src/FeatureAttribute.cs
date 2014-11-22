using System;

namespace xGherkin
{
    public class FeatureAttribute : Attribute
    {
        public string Feature { get; set; }
        public string Description { get; set; }
        public FeatureAttribute(string feature, string description)
        {
            Feature = feature;
            Description = description;
        }
    }
}
