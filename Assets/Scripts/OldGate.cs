using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OldGate : MonoBehaviour 
{
	[SerializeField] public InputField input;

	public bool IsSet
	{
		get { int i; return int.TryParse(input.text, out i); }
	}

	public int GetDiff()
	{
		return int.Parse(input.text);
	}
}
