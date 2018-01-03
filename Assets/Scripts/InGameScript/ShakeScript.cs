using UnityEngine;
using System.Collections;

public class ShakeScript : MonoBehaviour {

	// Use this for initialization
	void Start () 
	{
	 	gameObject.GetComponent<Animation>()["ShakeAnimation"].wrapMode = WrapMode.Loop;
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
