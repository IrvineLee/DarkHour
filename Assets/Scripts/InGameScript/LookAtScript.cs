using UnityEngine;
using System.Collections;

public class LookAtScript : MonoBehaviour 
{
	public bool IsEnabled = true;
	public bool IsTriggered = false;
	public bool IsLookAt = true;
	public Rect Region;
	
	public Transform LookAtTarget;
	
	GameObject mCamera;
	
	bool mIsInPosition = false;
	
	CameraRayScript mCameraRayScript;
	PlayerController mPlayerController;
	
	void Start () 
	{
		mCamera = GameObject.Find ("Main Camera");
		mCameraRayScript = mCamera.GetComponent<CameraRayScript>();
		mPlayerController = GameObject.Find ("Player").GetComponent<PlayerController>();;
	}
	
	void Update () 
	{
		if(IsEnabled)
		{
			if(IsLookAt)
			{
				IsTriggered = true;
				mPlayerController.DisablePlayerController = true;
				mCamera.transform.rotation = Quaternion.Slerp(mCamera.transform.rotation, Quaternion.LookRotation(LookAtTarget.transform.position - mCamera.transform.position), Time.deltaTime * 2.5f);
				
				if(Mathf.FloorToInt(Mathf.Abs(mCamera.transform.rotation.eulerAngles.x)) == Mathf.FloorToInt(Mathf.Abs(Quaternion.LookRotation(LookAtTarget.transform.position - mCamera.transform.position).eulerAngles.x))
					&& Mathf.FloorToInt(Mathf.Abs(mCamera.transform.rotation.eulerAngles.y)) == Mathf.FloorToInt(Mathf.Abs(Quaternion.LookRotation(LookAtTarget.transform.position - mCamera.transform.position).eulerAngles.y)))
				{
					mCameraRayScript.RayCastDistance = 2.0f;
					IsLookAt = false;
					mIsInPosition = true;
					IsEnabled = false;
				}
			}
			else if(!IsLookAt)
			{
				
				IsTriggered = false;
				mPlayerController.DisablePlayerController = false;
				mCamera.transform.rotation = Quaternion.Slerp(mCamera.transform.rotation, Quaternion.identity, Time.deltaTime * 5.0f);
				
				if(mCamera.transform.rotation == Quaternion.identity)
				{
					mCamera.transform.localRotation = Quaternion.identity;
					mCameraRayScript.RayCastDistance = 1.2f;
					Destroy (GameObject.Find ("TriggerLookAt(F)"));
					Destroy (GameObject.Find ("Flashlight Scene"));
					GameObject.FindGameObjectWithTag ("Spotlight").GetComponent<Light>().intensity = 1.0f;
					IsLookAt = true;
					mIsInPosition = false;
					IsEnabled = false;
					Destroy (gameObject);
				}
			}
		}
	}
	
	void OnTriggerEnter(Collider other)
	{
		IsEnabled = true;
    }
	
	/*void OnGUI () 
	{
		if(mIsInPosition)
		{
			if(GUI.Button (new Rect(Region.x * Screen.width, Region.y * Screen.height, Region.width*Screen.width, Region.height*Screen.height), "Pick Up"))
			{
				IsEnabled = true;
				mIsInPosition = false;
			}
		}
	}*/
}
