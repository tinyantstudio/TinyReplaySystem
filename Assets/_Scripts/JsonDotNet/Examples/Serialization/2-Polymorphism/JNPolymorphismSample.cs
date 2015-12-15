//### Sample objects are in the 999-ExampleModels/JNExampleModels.cs
using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

namespace DustinHorne.Json.Examples
{
    /// <summary>
    /// This demonstrates how to use polymorphism
    /// </summary>
    public class JNPolymorphismSample
    {
        //Used for randomizing the models - Using .NET's Random implementation
        private System.Random _rnd = new System.Random();

        public void Sample()
        {
            //This example is useful if you use a lot of subclasses.  For instance, 
            //You may have a class called "Animal" that has a set of common 
            //Behaviors and properties.  You may create subclasses such as "Wolf" 
            //and "Bear" which have their own properties or behaviors.  Using the 
            //Polymorphic example, you can store them all in a generic list typed as 
            //the base class but still ensure all of the information is serialized.  
            //For this example, we include an ObjectType property that tells what the 
            //class type is.


            //Create a list to hold objects.  List is typed to the Base Type
            var objectList = new List<JNSimpleObjectModel>();

            //Add 3 Simple Object Models
            for (var i = 0; i < 3; i++)
            {
                objectList.Add(GetBaseModel());
            }

            //Now add 2 of the SubClass models
            for (var i = 0; i < 2; i++)
            {
                objectList.Add(GetSubClassModel());
            }

            //Now add three more Base models
            for (var i = 0; i < 3; i++)
            {
                objectList.Add(GetBaseModel());
            }



            //Now we can serialize our list and use the TypeNameHandling option of JSON .NET to make 
            //sure the proper type name is stored alone with the serialized data.  Formatting.None will result 
            //in the most compact serialized data.  Change it to Indented for a more readable representation
            var settings = new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.All };

            var serialized = JsonConvert.SerializeObject(objectList, Formatting.None, settings);

            //Now deserialize into a new list.  Use the sampe TypeNameHandling settings when deserializing
            var newObjectList = JsonConvert.DeserializeObject<List<JNSimpleObjectModel>>(serialized, settings);

            //Now we can loop through the objects
            for (var i = 0; i < newObjectList.Count; i++)
            {
                var obj = newObjectList[i];

                //If the object type is "SubClass" we can cast it 
                //to the SubClass type and access the subclass property
                //otherwise we'll just use the base class string value.
                if (obj.ObjectType == JNObjectType.SubClass)
                {
                    Debug.Log((obj as JNSubClassModel).SubClassStringValue);
                }
                else
                {
                    Debug.Log(obj.StringValue);
                }
            }

        }

        private JNSimpleObjectModel GetBaseModel()
        {
            var m = new JNSimpleObjectModel();
            m.IntValue = _rnd.Next();
            m.FloatValue = (float)_rnd.NextDouble();
            m.StringValue = Guid.NewGuid().ToString();
            m.IntList = new List<int> { _rnd.Next(), _rnd.Next(), _rnd.Next() };

            m.ObjectType = JNObjectType.BaseClass;

            return m;
        }

        private JNSubClassModel GetSubClassModel()
        {
            var m = new JNSubClassModel();

            m.IntValue = _rnd.Next();
            m.FloatValue = (float)_rnd.NextDouble();
            m.StringValue = Guid.NewGuid().ToString();
            m.IntList = new List<int> { _rnd.Next(), _rnd.Next(), _rnd.Next() };

            m.ObjectType = JNObjectType.SubClass;
            m.SubClassStringValue = "This is the subclass value.";

            return m;
        }
    }
}
