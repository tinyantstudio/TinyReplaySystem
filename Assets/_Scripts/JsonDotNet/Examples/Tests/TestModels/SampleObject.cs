using UnityEngine;
using System.Collections;
using System;

public class SampleObject
{
	public string StringProperty { get; set; }
	public float FloatProperty { get; set; }
	public Guid GuidProperty { get; set; }
	public Vector3 VectorProperty { get; set; }
}




public class Dustin
{
	public string Name { get; private set; }
	public int Age { get; private set; }
	public string Url { get; private set; }

	public Dustin()
	{
		Name = "Dustin Horne";
		Age = 34;
		Url = "http://www.dustinhorne.com";
	}
}