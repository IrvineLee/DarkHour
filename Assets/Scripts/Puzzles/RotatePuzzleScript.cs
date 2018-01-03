using UnityEngine;
using System.Collections;

public class RotatePuzzleScript : MonoBehaviour 
{
	public bool IsEnabled = true;
	public bool GetInPosition = true;
	public Rect Region;
	public Rect TurnRemainRegion;
	
	public Transform MoveToTarget;
	public Transform LookAtTarget;
	
	GameObject Player;
	GameObject mCamera;
	
	private int mFingerID = -1;
	private Vector3 mStartPosition;
    private Vector3 mEndPosition;
	private float mStartTime;
	
	Quaternion mInitialRotation;
	
	bool mGetStartTime = true;
	public bool mIsFinished = false;
	
	[HideInInspector]
	public bool mIsInPosition = false;
	
	PlayerController mPlayerController;
	ExamineScript mExamineScript;
	CameraRayScript mCameraRayScript;
	
	void Start () 
	{
		Player = GameObject.Find ("Player");
		mCamera = GameObject.Find ("Main Camera");
		mPlayerController = Player.GetComponent<PlayerController>();
		mExamineScript = GameObject.Find ("ExaminePlane(R)").GetComponent<ExamineScript>();
		mCameraRayScript = mCamera.GetComponent<CameraRayScript>();
		
		InputManager.TouchDownEvent += OnTouchDown;
		InputManager.TouchDragEvent += OnTouchDrag;
		InputManager.TouchUpEvent += OnTouchUp;
	}
	
	void OnDestroy()
	{
		InputManager.TouchDownEvent -= OnTouchDown;
		InputManager.TouchDragEvent -= OnTouchDrag;
		InputManager.TouchUpEvent -= OnTouchUp;
	}

	
	void OnTouchDown(int fingerID, Vector2 pos,ref bool handled)
	{
		if(mIsInPosition)
		{
			if(mFingerID != -1) return;
		
			//convert pixel pos to 0-1 ratio
			float x = pos.x / Screen.width;
			float y = pos.y / Screen.height;
			
			Vector2 ratioPos = new Vector2(x,y);
			if(!Region.Contains(ratioPos)) return;
				
			mFingerID = fingerID;
		}
	}
	
	void OnTouchUp(int fingerID, Vector2 pos, ref bool handled)
	{
		if(mIsInPosition)
		{
			if(mFingerID != fingerID) return;
			
			IsEnabled = true;
			
			mExamineScript.mTypeOfMessage = ExamineScript.TYPE_OF_MESSAGE.FAILED;
			mExamineScript.EnabledExamine = true;
			RotatePuzzleController.mTurnCount = 7;
			mIsFinished = true;
			mFingerID = -1;
		}
	}
	
	void OnTouchDrag(int fingerID, Vector2 pos, ref bool handled)
	{
		
	}
	
	void Update () 
	{
		if(IsEnabled)
		{
			if(GetInPosition)
			{
				if (mGetStartTime == true)
				{
					mPlayerController.DisablePlayerController = true;
					mCameraRayScript.DisableRayCast = true;
					mStartPosition = mCamera.transform.position;
					mEndPosition = MoveToTarget.transform.position;
					mInitialRotation = mCamera.transform.rotation;
					
					mStartTime = Time.time;
					mGetStartTime = false;
				}
	
				mCamera.transform.position = Vector3.Lerp(mStartPosition, mEndPosition, 2.5f * (Time.time - mStartTime));
				mCamera.transform.rotation = Quaternion.Slerp(mCamera.transform.rotation, Quaternion.LookRotation(LookAtTarget.transform.position - mCamera.transform.position), Time.deltaTime * 5.0f);
				
				if(mCamera.transform.position == mEndPosition && Mathf.FloorToInt(Mathf.Abs(mCamera.transform.rotation.eulerAngles.y)) == Mathf.FloorToInt(Mathf.Abs(Quaternion.LookRotation(LookAtTarget.transform.position - mCamera.transform.position).eulerAngles.y))
					&& Mathf.FloorToInt(Mathf.Abs(mCamera.transform.rotation.eulerAngles.x)) == Mathf.FloorToInt(Mathf.Abs(Quaternion.LookRotation(LookAtTarget.transform.position - mCamera.transform.position).eulerAngles.x))
					&& Mathf.FloorToInt(Mathf.Abs(mCamera.transform.rotation.eulerAngles.z)) == Mathf.FloorToInt(Mathf.Abs(Quaternion.LookRotation(LookAtTarget.transform.position - mCamera.transform.position).eulerAngles.z)))
				{
					mGetStartTime = true;
					GetInPosition = false;
					mIsInPosition = true;
					IsEnabled = false;
				}
			}
			else if(!GetInPosition)
			{
				if (mGetStartTime == true)
				{
					/*if(!mIsFinished)
					{
						mCameraRayScript.DisableRayCast = false;
						mPlayerController.DisablePlayerController = false;
					}*/
					
					mStartTime = Time.time;
					mIsInPosition = false;
					mGetStartTime = false;
				}
				
				mCamera.transform.position = Vector3.Lerp(mEndPosition, mStartPosition, 2.5f * (Time.time - mStartTime));
				mCamera.transform.rotation = Quaternion.Slerp(mCamera.transform.rotation, mInitialRotation, Time.deltaTime * 10.0f);
				
				if(mCamera.transform.position == mStartPosition && Mathf.FloorToInt(Mathf.Abs(mCamera.transform.rotation.eulerAngles.y)) == Mathf.FloorToInt(Mathf.Abs(mInitialRotation.eulerAngles.y)))
				{
					//mPlayerController.DisablePlayerController = false;
					//mCameraRayScript.DisableRayCast = false;
					mCamera.transform.localRotation = Quaternion.identity;
					mIsFinished = false;
					mGetStartTime = true;
					GetInPosition = true;
					IsEnabled = false;
				}
			}
		}
	}
	
	void OnGUI () 
	{
		if(mIsInPosition)
		{
			GUI.Button (new Rect(Region.x * Screen.width, Region.y * Screen.height, Region.width*Screen.width, Region.height*Screen.height), "Exit");
			GUI.Button (new Rect(TurnRemainRegion.x * Screen.width, TurnRemainRegion.y * Screen.height, TurnRemainRegion.width*Screen.width, TurnRemainRegion.height*Screen.height), RotatePuzzleController.mTurnCount.ToString() + " turns remaining");
		}
	}
}
