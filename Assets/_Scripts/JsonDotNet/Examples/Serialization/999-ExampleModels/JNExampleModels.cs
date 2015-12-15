using System;
using System.Collections.Generic;

namespace DustinHorne.Json.Examples
{
    public enum JNObjectType
    {
        BaseClass = 0,
        SubClass = 1
    }

    /// <summary>
    /// Simple model used in the basic serialization sample
    /// </summary>
    public class JNSimpleObjectModel
    {
        public int IntValue { get; set; }
        public float FloatValue { get; set; }
        public string StringValue { get; set; }
        public List<int> IntList { get; set; }
        public JNObjectType ObjectType { get; set; }
    }

    /// <summary>
    /// Subclass model used in the PolyMorphism sample.
    /// </summary>
    public class JNSubClassModel : JNSimpleObjectModel
    {
        public string SubClassStringValue { get; set; }
    }
}
