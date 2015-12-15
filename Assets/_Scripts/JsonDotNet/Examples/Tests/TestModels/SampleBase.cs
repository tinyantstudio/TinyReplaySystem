using UnityEngine;

namespace Assets.DustinHorne.JsonDotNetUnity.TestCases.TestModels
{
	public class SampleBase
	{
		public string TextValue { get; set; }
		public int NumberValue { get; set; }
		public Vector3 VectorValue { get; set; }

        public virtual void UdpateCoolThing()
        {
            Debug.Log(" this is base class for update cool thing.");
        }
	}
}
