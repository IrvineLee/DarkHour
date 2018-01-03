using UnityEngine;
using System.Collections;

public class CameraRayScript : MonoBehaviour {
	
	public float RayCastDistance;
	public bool DisableRayCast = false;
	
	private Vector3 screenPos;
	
	ClickController mClickContoller;
		
	public Texture mButton;
	private float mValueX = 0.45f;
	private float mValueY = 0.45f;
	private float mValueOffSet = 0.1f;
	private bool mCanInteract = false;
	
	private int layerMask;
	
	// Use this for initialization
	void Start () 
	{	
		mClickContoller = GameObject.Find("Player").GetComponent<ClickController>();
			
		layerMask = 0x1 << LayerMask.NameToLayer("InteractiveLayer");
	}
	
	// Update is called once per frame
	void Update () 
	{
		Ray ray = Camera.main.ScreenPointToRay(new Vector3(mValueX * Screen.width, TopToBtmAxis(mValueY) * Screen.height, 0.0f));
		Ray ray2 = Camera.main.ScreenPointToRay(new Vector3((mValueX + mValueOffSet) * Screen.width, TopToBtmAxis((mValueY + mValueOffSet)) * Screen.height, 0.0f));
        RaycastHit hit ;
		
        if (Physics.Raycast (ray, out hit, RayCastDistance, layerMask) && Physics.Raycast (ray2, out hit, RayCastDistance, layerMask) && !DisableRayCast) 
		{
			mClickContoller.ClickType = ClickController.TypeOfClick.CLICK_ONCE;
			mClickContoller.Region1.x = mValueX;
			mClickContoller.Region1.y = mValueY;
			mClickContoller.Region1.width = mValueOffSet;
			mClickContoller.Region1.height = mValueOffSet;

			mCanInteract = true;
			
			if(mClickContoller.mIsComplete)
			{
				Debug.Log (hit.collider.gameObject.tag);
				if(hit.collider.gameObject.tag == "Examine")
	       		{
					hit.collider.gameObject.GetComponent<ExamineScript>().EnabledExamine = true;
				}
				else if(hit.collider.gameObject.tag == "PickUpItem")
	       		{
					hit.collider.gameObject.GetComponent<PickUpItemScript>().EnabledPickUp = true;
				}
				else if(hit.collider.gameObject.tag == "SlideDoor")
	       		{
					hit.collider.gameObject.GetComponent<SlideDoorScript>().EnabledSlideDoor = true;
				}
				else if(hit.collider.gameObject.tag == "OpenDoor")
	       		{
					hit.collider.gameObject.GetComponent<WesternDoorScript>().EnabledOpenDoor= true;
				}
				else if(hit.collider.gameObject.tag == "OpenDrawer")
	       		{
					hit.collider.gameObject.GetComponent<ChestScript>().IsEnabled = true;
				}
				else if(hit.collider.gameObject.tag == "OpenChestBox")
	       		{
					hit.collider.gameObject.GetComponent<ChestBoxScript>().IsEnabled = true;
					hit.collider.gameObject.layer = 0;
				}
				mClickContoller.mIsComplete = false;
			}
        }
		else
		{
			mClickContoller.ResetAllRectValue ();
			mCanInteract = false;
		}
	}
	
	void OnGUI()
	{
		if(mCanInteract)
		{
			GUI.Label(new Rect(mValueX * Screen.width, mValueX * Screen.height, (mValueOffSet * Screen.width), (mValueOffSet * Screen.height)), mButton);
		}
	}
	
	float TopToBtmAxis(float val)
	{
		return val = 1 - val;
	}
}
