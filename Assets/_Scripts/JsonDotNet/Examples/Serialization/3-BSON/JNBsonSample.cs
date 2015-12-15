//### Sample objects are in the 999-ExampleModels/JNExampleModels.cs
using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Bson; // <-- for the BSON Support
using UnityEngine;
using System.IO;

namespace DustinHorne.Json.Examples
{
    /// <summary>
    /// Example of serializing and deserializing using 
    /// BSON (Binary formatting)
    /// </summary>
    public class JNBsonSample
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

            //Placeholder to hold the serialized data so we can deserialize it later
            byte[] serializedData = new byte[]{}; 

            //Create a memory stream to hold the serialized bytes
            using(var stream  = new MemoryStream())
            {
                using (BsonWriter writer = new BsonWriter(stream))
                {
                    JsonSerializer serializer = new JsonSerializer();
                    serializer.Serialize(writer, original);
                }

                //Read the stream to a byte array.  We could 
                //just as easily output it to a file
                serializedData = stream.ToArray();

                //You could write the raw bytes to a file, here we're converting 
                //them to a base-64 string and writing out to the debug log
                var serialized = Convert.ToBase64String(serializedData);
                Debug.Log(serialized);
            }

            //Placeholder for our deserialized object so it's available outside 
            //of the using block

            JNSimpleObjectModel newObject;

            //Now that we have a byte array of our serialized data, let's Deserialize it.
            using (var stream = new MemoryStream(serializedData))
            {
                using (BsonReader reader = new BsonReader(stream))
                {
                    //If you're deserializing a collection, the following option 
                    //must be set to instruct the reader that the root object 
                    //is actually an array / collection type.
                    //
                    //reader.ReadRootValueAsArray = true;

                    JsonSerializer serializer = new JsonSerializer();
                    newObject = serializer.Deserialize<JNSimpleObjectModel>(reader);
                }
            }

            if (newObject != null)
            {
                Debug.Log(newObject.StringValue);
            }
            


            

            


            //Debug.Log(newobject.IntList.Count);
        }
    }
}




