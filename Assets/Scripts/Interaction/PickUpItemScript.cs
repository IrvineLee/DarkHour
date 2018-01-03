using UnityEngine;
using System.Collections;

public class PickUpItemScript : MonoBehaviour {
	
	public GameObject PickUpItem;
	
	[HideInInspector]
	public bool EnabledPickUp = false;
	
	PlayerStatusScript mPlayerStatusScript;
	ExamineScript mExamineScript;
	
	GameObject Key;
	
	void Start()
	{
		mExamineScript = GameObject.Find ("/Floor 1/WesternDoor3/GameObject/Plane009/Right").GetComponent<ExamineScript>();
		mPlayerStatusScript = GameObject.Find ("Player").GetComponent<PlayerStatusScript>();
		Key = GameObject.Find ("Key");
	}
	
	// Update is called once per frame
	void Update () 
	{
		if(EnabledPickUp)
		{
			if(PickUpItem == Key)
			{
				mExamineScript.mTypeOfMessage = ExamineScript.TYPE_OF_MESSAGE.TRIGGER;
				GameObject.Find ("DrawerWithKey").layer = 8;
				Key.GetComponent<AudioSource>().Play ();
				ChestScript.IsDestroyedKey = true;
			}
			if(PickUpItem == GameObject.Find ("Note1") ||PickUpItem == GameObject.Find ("Note2"))
			{
				mPlayerStatusScript.Notes += 1;
			}
			Destroy(PickUpItem);
			EnabledPickUp = false;
		}
	}
}
