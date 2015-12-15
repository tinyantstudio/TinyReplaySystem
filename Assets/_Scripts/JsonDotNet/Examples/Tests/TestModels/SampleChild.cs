using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.DustinHorne.JsonDotNetUnity.TestCases.TestModels
{
	public class SampleChildB : SampleBase
	{
		public List<SimpleClassObject> ObjectList { get; set; }

        public Dictionary<string, int> ObjectDictionary { get; set; }
        public Color curColor { get; set; }
        public override void UdpateCoolThing()
        {
            Debug.Log(" this is SampleChildB for update cool thing my color:" + this.curColor);
            /*for (int i = 0; i < ObjectList.Count;i++ )
            {
                Debug.Log("number value is " + ObjectList[i].NumberValue);
            }

            foreach (KeyValuePair<string, int> obj in ObjectDictionary)
                Debug.Log(" object dic is " + obj.Value);*/
        }
	}

    public class SampleChildA : SampleBase
    {
        public float rotation = 10.0f;

        public override void UdpateCoolThing()
        {
            Debug.Log(" this is SampleChildA for update cool thing my color:" + this.rotation);
        }
        
        public void UpdateChildAState()
        {
            Debug.Log(" this is UpdateChildAState ");
        }
    }
}