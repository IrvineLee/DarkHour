using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour {
	
	AxisController AxisControllerLeft;
	AxisController AxisControllerRight;
	CharacterController charController;
	
	public enum RotationAxes { MouseXAndY = 0, MouseX = 1, MouseY = 2 }
	public RotationAxes axes = RotationAxes.MouseXAndY;
	public float movingSpeedModifier = 500.0f;
	public float sensitivityX = 1.0F;
	public float sensitivityY = 1.0F;

	public float minimumX = -360F;
	public float maximumX = 360F;
	
	public float minimumY = -60F;
	public float maximumY = 60F;
	
	public float rotationX = 0F;
	public float rotationY = 0F;
	
	public float otherRotationX = 0.0f;
	public float otherRotationY = 0.0f;
	
	Quaternion originalRotation;
	Quaternion xQuaternion;
	Quaternion yQuaternion;
	
	public Texture mButtonLeft;
	public Texture mButtonRight;
	public Texture mBloodSplatter;
	
	public bool LockControls = false;
	public bool UseAxisControl = true;
	
	public bool DisablePlayerController = false;
	// Use this for initialization
	void Start () 
	{
		charController = GetComponent<CharacterController>();
		AxisController[] tempAxisController = gameObject.GetComponents<AxisController>();
		
		for (int i = 0; i < tempAxisController.Length; i++)
		{
			if (tempAxisController[i].Name == "Left")
			{
				AxisControllerLeft = tempAxisController[i];
			}
			if (tempAxisController[i].Name == "Right")
			{
				AxisControllerRight = tempAxisController[i];
			}
		}
		
		/*if (!AxisControllerLeft)
		{
			Debug.LogError("Object do not have AxisControllerLeft");	
		}
		if (!AxisControllerRight)
		{
			Debug.LogError("Object do not have AxisControllerRight");	
		}*/
		
		// Make the rigid body not change rotation
    	if (GetComponent<Rigidbody>()) GetComponent<Rigidbody>().freezeRotation = true;
   		originalRotation = transform.localRotation;
	}
	
	// Update is called once per frame
	void Update ()
	{
		if (Input.GetKey(KeyCode.W))
		{	
			charController.SimpleMove(transform.TransformDirection(Vector3.forward) * 5);	
		}
		
		if (Input.GetKey(KeyCode.S))
		{
			charController.SimpleMove(transform.TransformDirection(Vector3.back) * 5);	
		}
		
		if (Input.GetKey(KeyCode.A))
		{
			charController.SimpleMove(transform.TransformDirection(Vector3.left) * 5);	
		}
		
		if (Input.GetKey(KeyCode.D))
		{
			charController.SimpleMove(transform.TransformDirection(Vector3.right) * 5);	
		}
		
		if(!DisablePlayerController)
		{
			if(UseAxisControl && !LockControls)
			{
				charController.SimpleMove(transform.TransformDirection(AxisControllerLeft.GetDirection()) * movingSpeedModifier);
				
				if (axes == RotationAxes.MouseXAndY)
		    	{
					rotationX += (AxisControllerRight.GetDirection().x * sensitivityX); //+ otherRotationX;
			        rotationY += (AxisControllerRight.GetDirection().z * sensitivityY); //+ otherRotationY;
			        rotationX = ClampAngle (rotationX, minimumX, maximumX);
			        rotationY = ClampAngle (rotationY, minimumY, maximumY);
					//Debug.Log (rotationX);
			        xQuaternion = Quaternion.AngleAxis (rotationX, Vector3.up);
					yQuaternion = Quaternion.AngleAxis (rotationY, Vector3.left);
					transform.rotation = originalRotation * xQuaternion * yQuaternion;
					
					//otherRotationX = 0;
					//otherRotationY = 0;
					//Debug.Log("AxisControllerRight.GetDirection().x : " + AxisControllerRight.GetDirection().x + " " + AxisControllerRight.GetDirection().y);
					//Debug.Log("=======================Rotation : " + rotationX + " " + rotationY);
				}
			}
			else
			{
				xQuaternion = Quaternion.AngleAxis (rotationX, Vector3.up);
				yQuaternion = Quaternion.AngleAxis (rotationY, Vector3.left);
				transform.rotation = originalRotation * xQuaternion * yQuaternion;
				UseAxisControl = true;
				//Debug.Log("##########Rotation : " + rotationX + " " + rotationY);
			}
		}
		/*else
		{
	        xQuaternion = Quaternion.AngleAxis (rotationX, Vector3.up);
			yQuaternion = Quaternion.AngleAxis (rotationY, Vector3.left);
			originalRotation = transform.localRotation * xQuaternion * yQuaternion;
		}*/
	}
	
	void OnGUI()
	{
		if(!LockControls && !DisablePlayerController)
		{
			GUI.Label(new Rect((AxisControllerLeft.Region.x + 0.04f) * Screen.width, 
				(AxisControllerLeft.Region.y + 0.04f) * Screen.height, 
				AxisControllerLeft.Region.width * Screen.width, 
				AxisControllerLeft.Region.height * Screen.height), mButtonLeft);
			
			/*GUI.Button(new Rect((AxisControllerRight.Region.x) * Screen.width, 
				(AxisControllerRight.Region.y) * Screen.height, 
				AxisControllerRight.Region.width * Screen.width, 
				AxisControllerRight.Region.height * Screen.height), mButtonRight);*/
			
			GUI.Label(new Rect((AxisControllerRight.Region.x + 0.04f) * Screen.width, 
				(AxisControllerRight.Region.y + 0.04f) * Screen.height, 
				AxisControllerRight.Region.width * Screen.width, 
				AxisControllerRight.Region.height * Screen.height), mButtonRight);
			
			//GUI.Label(new Rect(0, 0, Screen.width, Screen.height), mBloodSplatter);
		}
	}
	
	public static float ClampAngle (float angle, float min, float max)
	{
	    if (angle < -360F)
	        angle += 360F;
	    if (angle > 360F)
	        angle -= 360F;
	    return Mathf.Clamp (angle, min, max);
	}
}
