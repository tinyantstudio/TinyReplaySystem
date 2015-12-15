//### Sample objects are in the 999-ExampleModels/JNExampleModels.cs
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

namespace DustinHorne.Json.Examples
{
    /// <summary>
    /// Example of serializing and deserializing a simple object
    /// </summary>
    public class JNSimpleObjectSample
    {
        public void Sample()
        {
            //Create an object to serialize
            var original = new JNSimpleObjectModel
                {
                    IntValue = 5,
                    FloatValue = 4.98f,
                    StringValue = "Simple Object",
                    IntList = new List<int> { 4, 7, 25, 34 },
                    ObjectType = JNObjectType.BaseClass
                };

            //This string is the JSON representation of the object
            string serialized = JsonConvert.SerializeObject(original);

            //Now we can deserialize this string back into an object
            var newobject = JsonConvert.DeserializeObject<JNSimpleObjectModel>(serialized);

            Debug.Log(newobject.IntList.Count);
        }
    }
}




