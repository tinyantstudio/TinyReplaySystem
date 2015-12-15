using System;
using System.Collections.Generic;
using System.Text;
using SampleClassLibrary;
using UnityEngine;
using System.Collections;
using Newtonsoft.Json;
using Assets.DustinHorne.JsonDotNetUnity.TestCases;
using Assets.DustinHorne.JsonDotNetUnity.TestCases.TestModels;

public class JsonTestScript
{
    private TextMesh _text;
    private const string BAD_RESULT_MESSAGE = "Incorrect Deserialized Result";

    public JsonTestScript(TextMesh text)
    {
        _text = text;
    }

    /// <summary>
    /// Simple Vector3 Serialization
    /// </summary>
    public void SerializeVector3()
    {
        LogStart("Vector3 Serialization");
        try
        {

            var v = new Vector3(2, 4, 6);
            var serialized = JsonConvert.SerializeObject(v);
            LogSerialized(serialized);
            var v2 = JsonConvert.DeserializeObject<Vector3>(serialized);

            LogResult("4", v2.y);

            if (v2.y != v.y)
            {
                DisplayFail("Vector3 Serialization", BAD_RESULT_MESSAGE);
            }

            DisplaySuccess("Vector3 Serialization");
        }
        catch (Exception ex)
        {
            DisplayFail("Vector3 Serialization", ex.Message);
        }

        LogEnd(1);
    }

    /// <summary>
    /// List<T> serialization
    /// </summary>
    public void GenericListSerialization()
    {
        LogStart("List<T> Serialization");

        try
        {
            var objList = new List<SimpleClassObject>();

            for (var i = 0; i < 4; i++)
            {
                objList.Add(TestCaseUtils.GetSimpleClassObject());
            }

            var serialized = JsonConvert.SerializeObject(objList);
            LogSerialized(serialized);

            var newList = JsonConvert.DeserializeObject<List<SimpleClassObject>>(serialized);

            LogResult(objList.Count.ToString(), newList.Count);
            LogResult(objList[2].TextValue, newList[2].TextValue);

            if ((objList.Count != newList.Count) || (objList[3].TextValue != newList[3].TextValue))
            {
                DisplayFail("List<T> Serialization", BAD_RESULT_MESSAGE);
                Debug.LogError("Deserialized List<T> has incorrect count or wrong item value");
            }
            else
            {
                DisplaySuccess("List<T> Serialization");
            }
        }
        catch (Exception ex)
        {
            DisplayFail("List<T> Serialization", ex.Message);
            throw;
        }

        LogEnd(2);
    }

    /// <summary>
    /// Polymorphism
    /// </summary>
    public void PolymorphicSerialization()
    {
        LogStart("Polymorphic Serialization");

        try
        {
            var list = new List<SampleBase>();

            for (var i = 0; i < 4; i++)
            {
                SampleChildB newChild = TestCaseUtils.GetSampleChid();
                list.Add(newChild);
                // Debug.Log("origin color is " + newChild.curColor);
            }

            // add child A
            for (var i = 0; i < 3; i++)
            {
                SampleChildA newChild = TestCaseUtils.GetSampleChidA();
                list.Add(newChild);
            }


            for (int i = 0; i < list.Count; i++)
            {
                list[i].UdpateCoolThing();
            }

            var serialized = JsonConvert.SerializeObject(list, Formatting.None, new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.All });
            LogSerialized(serialized);

            List<SampleBase> newList = JsonConvert.DeserializeObject<List<SampleBase>>(serialized, new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.All });


            LogResult(list[2].TextValue, newList[2].TextValue);

            for (int i = 0; i < newList.Count; i++)
            {
                SampleBase objA = (newList[i]);
                objA.UdpateCoolThing();
            }

            if (list[2].TextValue != newList[2].TextValue)
            {
                DisplayFail("Polymorphic Serialization", BAD_RESULT_MESSAGE);
            }
            else
            {
                DisplaySuccess("Polymorphic Serialization");
            }



            Dictionary<int, SampleBase> testForDic = new Dictionary<int, SampleBase>();

            for (var i = 0; i < 4; i++)
            {
                SampleChildB newChild = TestCaseUtils.GetSampleChid();
                testForDic.Add(i, newChild);
            }

            // add child A
            for (var i = 6; i < 10; i++)
            {
                SampleChildA newChild = TestCaseUtils.GetSampleChidA();
                testForDic.Add(i, newChild);
            }
            serialized = JsonConvert.SerializeObject(testForDic, Formatting.None, new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.Objects });
            LogSerialized(serialized);

            Dictionary<int, SampleBase> newDic = JsonConvert.DeserializeObject<Dictionary<int, SampleBase>>(serialized, new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.Objects });

            foreach (KeyValuePair<int, SampleBase> data in newDic)
            {
                if (data.Value.GetType() == typeof(SampleChildA))
                {
                    SampleChildA childAObject = data.Value as SampleChildA;
                    childAObject.UpdateChildAState();
                    Debug.Log("this is sampleChildA class ");
                }
                else if (data.Value.GetType() == typeof(SampleChildB))
                    Debug.Log("this is sampleChildB class ");
                Debug.Log(" this item type is " + data.GetType());
                data.Value.UdpateCoolThing();
            }
        }
        catch (Exception ex)
        {
            DisplayFail("Polymorphic Serialization", ex.Message);
            throw;
        }


        LogEnd(3);
    }

    /// <summary>
    /// Dictionary Serialization
    /// </summary>
    public void DictionarySerialization()
    {
        LogStart("Dictionary & Other DLL");

        try
        {
            var o = new SampleExternalClass { SampleString = Guid.NewGuid().ToString() };

            o.SampleDictionary.Add(1, "A");
            o.SampleDictionary.Add(2, "B");
            o.SampleDictionary.Add(3, "C");
            o.SampleDictionary.Add(4, "D");

            var serialized = JsonConvert.SerializeObject(o);
            LogSerialized(serialized);

            var newObj = JsonConvert.DeserializeObject<SampleExternalClass>(serialized);

            LogResult(o.SampleString, newObj.SampleString);
            LogResult(o.SampleDictionary.Count.ToString(), newObj.SampleDictionary.Count);

            var keys = new StringBuilder(4);
            var vals = new StringBuilder(4);

            foreach (var kvp in o.SampleDictionary)
            {
                keys.Append(kvp.Key.ToString());
                vals.Append(kvp.Value);
            }

            LogResult("1234", keys.ToString());
            LogResult("ABCD", vals.ToString());

            if ((o.SampleString != newObj.SampleString) || (o.SampleDictionary.Count != newObj.SampleDictionary.Count) ||
                (keys.ToString() != "1234") || (vals.ToString() != "ABCD"))
            {
                DisplayFail("Dictionary & Other DLL", BAD_RESULT_MESSAGE);
            }
            else
            {
                DisplaySuccess("Dictionary & Other DLL");
            }
        }
        catch (Exception ex)
        {
            DisplayFail("Dictionary & Other DLL", ex.Message);
            throw;
        }

    }

    /// <summary>
    /// Serialize a dictionary with an object as the value
    /// </summary>
    public void DictionaryObjectValueSerialization()
    {
        LogStart("Dictionary (Object Value)");

        try
        {
            var dict = new Dictionary<int, SampleBase>();

            for (var i = 0; i < 4; i++)
            {
                dict.Add(i, TestCaseUtils.GetSampleBase());
            }

            var serialized = JsonConvert.SerializeObject(dict);
            LogSerialized(serialized);

            var newDict = JsonConvert.DeserializeObject<Dictionary<int, SampleBase>>(serialized);

            LogResult(dict[1].TextValue, newDict[1].TextValue);

            if (dict[1].TextValue != newDict[1].TextValue)
            {
                DisplayFail("Dictionary (Object Value)", BAD_RESULT_MESSAGE);
            }
            else
            {
                DisplaySuccess("Dictionary (Object Value)");
            }
        }
        catch (Exception ex)
        {
            DisplayFail("Dictionary (Object Value)", ex.Message);
            throw;
        }
    }

    /// <summary>
    /// Serialize a dictionary with an object as the key
    /// </summary>
    public void DictionaryObjectKeySerialization()
    {
        LogStart("Dictionary (Object As Key)");

        try
        {
            var dict = new Dictionary<SampleBase, int>();

            for (var i = 0; i < 4; i++)
            {
                dict.Add(TestCaseUtils.GetSampleBase(), i);
            }

            var serialized = JsonConvert.SerializeObject(dict);
            LogSerialized(serialized);
            _text.text = serialized;
            var newDict = JsonConvert.DeserializeObject<Dictionary<SampleBase, int>>(serialized);


            var oldKeys = new List<SampleBase>();
            var newKeys = new List<SampleBase>();

            foreach (var k in dict.Keys)
            {
                oldKeys.Add(k);
            }

            foreach (var k in newDict.Keys)
            {
                newKeys.Add(k);
            }

            LogResult(oldKeys[1].TextValue, newKeys[1].TextValue);

            if (oldKeys[1].TextValue != newKeys[1].TextValue)
            {
                DisplayFail("Dictionary (Object As Key)", BAD_RESULT_MESSAGE);
            }
            else
            {
                DisplaySuccess("Dictionary (Object As Key)");
            }
        }
        catch (Exception ex)
        {
            DisplayFail("Dictionary (Object As Key)", ex.Message);
            throw;
        }
    }


    #region Private Helper Methods

    private void DisplaySuccess(string testName)
    {
        _text.text = testName + "\r\nSuccessful";
    }

    private void DisplayFail(string testName, string reason)
    {
        try
        {
            _text.text = testName + "\r\nFailed :( \r\n" + reason ?? string.Empty;
        }
        catch
        {
            Debug.Log("%%%%%%%%%%%" + testName);
        }

    }

    private void LogStart(string testName)
    {
        Log(string.Empty);
        Log(string.Format("======= SERIALIZATION TEST: {0} ==========", testName));
    }

    private void LogEnd(int testNum)
    {
        //Log(string.Format("====== SERIALIZATION TEST #{0} COMPLETE", testNum));
    }

    private void Log(object message)
    {
        Debug.Log(message);
    }

    private void LogSerialized(string message)
    {

        Debug.Log(string.Format("#### Serialized Object: {0}", message));
    }

    private void LogResult(string shouldEqual, object actual)
    {
        Log("--------------------");
        Log(string.Format("*** Original Test value: {0}", shouldEqual));
        Log(string.Format("*** Deserialized Test Value: {0}", actual));
        Log("--------------------");
    }
    #endregion
}
