using UnityEngine;
using System.Collections;

public class TriggerFlickerScript : MonoBehaviour {
	
	public GameObject Flashlight;
	
	FlashLightFlickerScript mFlashLightFlickerScript;
	
	// Use this for initialization
	void Start () 
	{
		mFlashLightFlickerScript = Flashlight.GetComponent<FlashLightFlickerScript>();
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	void OnTriggerEnter(Collider other)
	{
		mFlashLightFlickerScript.IsEnabled = true;
		Destroy (gameObject);
    }
}
