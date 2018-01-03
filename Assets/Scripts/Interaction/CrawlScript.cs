using UnityEngine;
using System.Collections;

public class CrawlScript : MonoBehaviour 
{
	public bool EnableCrawl = false;
	public bool IsCrawl = false;
	public float CrawlRadius = 0.3f;
	public float CrawlHeight = 0.5f;
	public float CameraHeight = 0.3f;
	public float FlashlightHeight = 0.04f;
	
	const float WALK_RADIUS = 0.5f;
	const float WALK_HEIGHT = 2.0f;
	const float WALK_CAMERA_HEIGHT = 0.9f;
	const float Flashlight_HEIGHT = 0.72f;
	
	CharacterController charController;
	GameObject CameraController;
	GameObject Player;
	GameObject Flashlight;
	
	// Use this for initialization
	void Start () 
	{
		Player = GameObject.Find("Player");
		charController = Player.GetComponent<CharacterController>();
		CameraController = GameObject.Find("Main Camera");
		Flashlight = GameObject.Find ("Spotlight");
	}
	
	// Update is called once per frame
	void Update () 
	{
		if(EnableCrawl)
		{
			if(IsCrawl)
			{
				charController.radius = CrawlRadius;
				charController.height = CrawlHeight;
				
				Vector3 tempPlayerPosition = Player.transform.position;
				tempPlayerPosition.y = -0.48f;
				Player.transform.position = tempPlayerPosition;
				
				Vector3 temp = CameraController.transform.localPosition;
				temp.y = CameraHeight; 
				CameraController.transform.localPosition = temp;

				Vector3 tempFlashlightPosition = Flashlight.transform.localPosition;
				tempFlashlightPosition.y = FlashlightHeight;
				Flashlight.transform.localPosition = tempFlashlightPosition;
			}
			else if(!IsCrawl)
			{
				charController.radius = WALK_RADIUS;
				charController.height = WALK_HEIGHT;
				
				Vector3 tempPlayerPosition = Player.transform.position;
				tempPlayerPosition.y = 0.0f;
				Player.transform.position = tempPlayerPosition;
				
				Vector3 temp = CameraController.transform.localPosition;
				temp.y = WALK_CAMERA_HEIGHT; 
				CameraController.transform.localPosition = temp;

				Vector3 tempFlashlightPosition = Flashlight.transform.localPosition;
				tempFlashlightPosition.y = Flashlight_HEIGHT;
				Flashlight.transform.localPosition = tempFlashlightPosition;
			}
			EnableCrawl = false;
		}
	}
}
