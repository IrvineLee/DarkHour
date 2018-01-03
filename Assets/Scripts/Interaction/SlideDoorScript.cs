using UnityEngine;
using System.Collections;

public class SlideDoorScript : MonoBehaviour {
	
	public AudioClip AudioOpen;
	public AudioClip AudioClose;
	
	public Transform Door;
	public bool DoorIsOpen = false;

	public enum DOOR_MOVE_DIRECTION
	{
		LEFT = 0,
		RIGHT = 1
	};
	public DOOR_MOVE_DIRECTION OpenDirection = DOOR_MOVE_DIRECTION.RIGHT;
	
	public enum DOOR_PLACEMENT
	{
		HORIZONTAL = 0,
		VERTICAL = 1
	};
	public DOOR_PLACEMENT Placement = DOOR_PLACEMENT.HORIZONTAL;
		
	public float SlideDoorRange = 1.0f;
	public float SlideDoorSpeed = 1.5f;
	
	private Vector3 mStartPosition;
    private Vector3 mEndPosition;
	private float mStartTime;
	
	bool mGetInitialVal = true;
	
	[HideInInspector]
	public bool EnabledSlideDoor = false;
	
	void Update () 
	{
		if(EnabledSlideDoor) 
		{
			SlideDoor();
		}
	}
	
	void SlideDoor()
	{
		if (mGetInitialVal == true)
		{
			if(DoorIsOpen) 
			{
				GetComponent<AudioSource>().PlayOneShot(AudioClose);
				if(OpenDirection == DOOR_MOVE_DIRECTION.RIGHT)
				{
					SlideDoorRange = -SlideDoorRange; 
				}
				else if(OpenDirection == DOOR_MOVE_DIRECTION.LEFT)
				{
					if(SlideDoorRange < 0) SlideDoorRange = -SlideDoorRange; 
				}
			}
			else if(!DoorIsOpen) 
			{
				GetComponent<AudioSource>().PlayOneShot(AudioOpen);
				if(OpenDirection == DOOR_MOVE_DIRECTION.RIGHT)
				{
					if(SlideDoorRange < 0) SlideDoorRange = -SlideDoorRange; 
				}
				else if(OpenDirection == DOOR_MOVE_DIRECTION.LEFT)
				{
					SlideDoorRange = -SlideDoorRange; 
				}
			}
			
			if(Placement == DOOR_PLACEMENT.HORIZONTAL)
			{
				mEndPosition.x = Door.transform.position.x - SlideDoorRange;
				mEndPosition.z = Door.transform.position.z;
			}
			else if(Placement == DOOR_PLACEMENT.VERTICAL)
			{
				mEndPosition.x = Door.transform.position.x;
				mEndPosition.z = Door.transform.position.z - SlideDoorRange;
			}
			
			mStartPosition = Door.transform.position;
			mEndPosition.y = Door.transform.position.y;
			mStartTime = Time.time;
			mGetInitialVal = false;
		}

		Door.transform.position = Vector3.Lerp(mStartPosition, mEndPosition, SlideDoorSpeed * (Time.time - mStartTime));
		
		if(Door.transform.position == mEndPosition) 
		{
			EnabledSlideDoor = false;
			mGetInitialVal = true;
			
			if(DoorIsOpen) DoorIsOpen = false;
			else if(!DoorIsOpen) DoorIsOpen = true;
		}
	}
}
