using UnityEngine;
using System.Collections;

public class WesternDoorScript : MonoBehaviour {
	
	GameObject Player;
	PlayerController mPlayerController;
	
	public AudioClip AudioOpen;
	public AudioClip AudioClose;
	
	public Transform Door;
	public Transform LookAt;
	public bool DoorIsOpen = false;
	public bool IsLocked = false;
	
	public enum DOOR_MOVE_DIRECTION
	{
		LEFT = 0,
		RIGHT = 1
	};
	public DOOR_MOVE_DIRECTION OpenDirection;
	
	public enum WALK_THROUGH_DIRECTION
	{
		LEFT = 0,
		RIGHT = 1,
		FORWARD = 2,
		BEHIND = 3
	};
	public WALK_THROUGH_DIRECTION WalkDirection;
	
	private Vector3 mStartPosition;
    private Vector3 mEndPosition;
	private float mStartTime;

	private int mRotationAngle = 90;
	private float mRotationX;
	private float mRotationY;
	private float mCumulativeY;
	private float mOpenDoorSpeed = 40.0f;
	private float mCloseDoorSpeed = 80.0f;
	private float mWalkThroughRange = WALK_THROUGH_RANGE;
	private float mWalkThroughSpeed = 1.5f;
	
	bool mGetInitialVal = true;
	bool mAutoWalkThrough = false;
	bool mOffSetFromDoor = true;
	
	const float WALK_THROUGH_RANGE = 2.5f;
	
	[HideInInspector]
	public bool EnabledOpenDoor = false;

	void Start () 
	{
		Player = GameObject.Find ("Player");
		mPlayerController = Player.GetComponent<PlayerController>();
	}
	
	void Update () 
	{
		if(EnabledOpenDoor && !IsLocked) 
		{
			if(mOffSetFromDoor)
			{
				mPlayerController.LockControls = true;
				GetComponent<AudioSource>().PlayOneShot(AudioOpen);
				OffSetFromDoor();
			}
			
			OpenDoor();
			
			if(mAutoWalkThrough)
			{
				AutoWalkThroughDoor();
			}
		}
		else if(IsLocked)
		{
			EnabledOpenDoor = false;
		}
	}
	
	void OpenDoor()
	{
		if(DoorIsOpen && !mAutoWalkThrough)
		{
			mRotationY = Time.deltaTime * mCloseDoorSpeed;
			mCumulativeY += mRotationY;
			
			if(mCumulativeY > mRotationAngle)
			{
				mCumulativeY -= mRotationY;
				mRotationY = mRotationAngle - mCumulativeY;
				mCumulativeY = 0;
				DoorIsOpen = false;	
				mOffSetFromDoor = true;
				EnabledOpenDoor = false;
				mPlayerController.LockControls = false;
			}
			
			if(OpenDirection == DOOR_MOVE_DIRECTION.RIGHT)
			{
				mRotationY = -mRotationY;
			}
			
			Door.transform.Rotate(0, mRotationY, 0);
			
		}
		else if(!DoorIsOpen && !mAutoWalkThrough)
		{
			mRotationY = Time.deltaTime * mOpenDoorSpeed;
			mCumulativeY += mRotationY;
			
			if(mCumulativeY > mRotationAngle)
			{
				mCumulativeY -= mRotationY;
				mRotationY = mRotationAngle - mCumulativeY;
				mCumulativeY = 0;
				DoorIsOpen = true;
				mAutoWalkThrough = true;
			}
			
			if(OpenDirection == DOOR_MOVE_DIRECTION.LEFT)
			{
				mRotationY = -mRotationY;
			}
			
			Door.transform.Rotate(0, mRotationY, 0);
		}
	}
	
	void AutoWalkThroughDoor()
	{
		if (mGetInitialVal == true)
		{
			if(OpenDirection == DOOR_MOVE_DIRECTION.RIGHT)
			{
				mWalkThroughRange = -WALK_THROUGH_RANGE;
				
				if(WalkDirection == WALK_THROUGH_DIRECTION.RIGHT)
				{
					mEndPosition.x = Player.transform.position.x - mWalkThroughRange;
					mEndPosition.z = Player.transform.position.z;
				}
				else if(WalkDirection == WALK_THROUGH_DIRECTION.BEHIND)
				{
					mEndPosition.x = Player.transform.position.x;
					mEndPosition.z = Player.transform.position.z + mWalkThroughRange;
				}
			}
			else if(OpenDirection == DOOR_MOVE_DIRECTION.LEFT)
			{
				mWalkThroughRange = WALK_THROUGH_RANGE;
				
				if(WalkDirection == WALK_THROUGH_DIRECTION.LEFT)
				{
					mEndPosition.x = Player.transform.position.x - mWalkThroughRange;
					mEndPosition.z = Player.transform.position.z;
				}
				else if(WalkDirection == WALK_THROUGH_DIRECTION.FORWARD)
				{
					mEndPosition.x = Player.transform.position.x;
					mEndPosition.z = Player.transform.position.z + mWalkThroughRange;
				}
			}
			
			mStartPosition = Player.transform.position;
			mEndPosition.y = Player.transform.position.y;
			
			mStartTime = Time.time;
			mGetInitialVal = false;
		}
		
		Player.transform.position = Vector3.Lerp(mStartPosition, mEndPosition, mWalkThroughSpeed * (Time.time - mStartTime));
		
		if(Player.transform.position == mEndPosition) 
		{
			mGetInitialVal = true;
			mAutoWalkThrough = false;
			mPlayerController.LockControls = false;
			GetComponent<AudioSource>().PlayOneShot(AudioClose);
		}
	}
	
	void OffSetFromDoor()
	{
		float xtemp = 0;
		float ytemp = 0;
		float ztemp = 0;
		
		if(OpenDirection == DOOR_MOVE_DIRECTION.RIGHT)
		{
			if(WalkDirection == WALK_THROUGH_DIRECTION.RIGHT)
			{
				xtemp = LookAt.transform.position.x - 1.0f;
				ztemp = LookAt.transform.position.z;
				mRotationX = 90;
				//mRotationX = 180;
			}
			else if(WalkDirection == WALK_THROUGH_DIRECTION.BEHIND)
			{
				xtemp = LookAt.transform.position.x;
				ztemp = LookAt.transform.position.z + 1.0f;
				mRotationX = 180;
				//mRotationX = -90;
			}
		}
		else if(OpenDirection == DOOR_MOVE_DIRECTION.LEFT)
		{
			if(WalkDirection == WALK_THROUGH_DIRECTION.LEFT)
			{
				xtemp = LookAt.transform.position.x + 1.0f;
				ztemp = LookAt.transform.position.z;
				mRotationX = -90;
				//mRotationX = 0;
			}
			else if(WalkDirection == WALK_THROUGH_DIRECTION.FORWARD)
			{
				xtemp = LookAt.transform.position.x;
				ztemp = LookAt.transform.position.z - 1.0f;
				mRotationX = 0;
				//mRotationX = 90;
			}
		}
		
		ytemp = Player.transform.position.y;
		Player.transform.position = new Vector3(xtemp, ytemp, ztemp);
		
		mPlayerController.rotationX = mRotationX;
		mPlayerController.rotationY = Player.transform.rotation.y;
		mPlayerController.UseAxisControl = false;
		mOffSetFromDoor = false;
	}
}
