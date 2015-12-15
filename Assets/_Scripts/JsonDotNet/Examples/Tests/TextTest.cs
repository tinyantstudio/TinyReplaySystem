using System;
using System.Runtime.InteropServices;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TextTest : MonoBehaviour
{
    public GameObject TextObject;
    private TextMesh _statusText;
    private DateTime _refTime;
    private int _testNum = 5;
    private JsonTestScript _tester;
    private bool _complete;

	// Use this for initialization
	void Start ()
	{
        //Set the test starting point
        _testNum = 0;
	    _statusText = TextObject.GetComponent<TextMesh>();
        _statusText.text = "-- SERIALIZATION TESTS -- \r\n Tests are run with \r\n a three second delay \r\n Starting in 10 seconds.";
        _tester = new JsonTestScript(_statusText);
        _refTime = DateTime.Now.AddSeconds(7);
        
	}
	
	// Update is called once per frame
	void Update () 
    {	
	    if (!_complete && (DateTime.Now - _refTime).TotalSeconds >= 3)
	    {
            // _testNum++;
            // RunNextTest();
	        // _refTime = DateTime.Now;
            _complete = true;
            _tester.PolymorphicSerialization();
	    }
	}

    private void RunNextTest()
    {
        switch (_testNum)
        {
            case 1:
                _tester.SerializeVector3();
                break;
            case 2:
                _tester.GenericListSerialization();
                break;
            case 3:
                _tester.PolymorphicSerialization();
                break;
            case 4:
                _tester.DictionarySerialization();
                break;
            case 5:
                _tester.DictionaryObjectValueSerialization();
                break;
            default:
                _complete = true;
                _statusText.text = "Tests Complete\r\nSee Console for Log";
                break;
        }
    }
}
