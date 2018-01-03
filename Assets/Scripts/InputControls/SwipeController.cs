using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SwipeEffect
{
	public Vector2 mSwipePos;
	public float mSwipeTimer;
}

public class SwipeController : MonoBehaviour {
	
	public string Name;
	public Rect Region;
	public float DragRange = 1.0f;
	public bool Locked = false;
	public Vector2 LockedStartPos;
	public string SwipeDirection;
	
	private string mDirection;
	private int mFingerID = -1;
	private Vector2 mStartPos;
	private Vector2 mActiveDir;
	
	public Texture SwipeTexture;
	
	private List<SwipeEffect> mSwipeEffectList = new List<SwipeEffect>();
	
	[HideInInspector]
	public bool mIsComplete = false;
	public bool mIsSwipedCorrectly = false;
	public bool IsHoldingDown = false;
	public bool IsDisableSwipeRegistration = false;
	
	void Start()
	{
		InputManager.TouchDownEvent += OnTouchDown;
		InputManager.TouchDragEvent += OnTouchDrag;
		InputManager.TouchUpEvent += OnTouchUp;
		
		/*if (Screen.dpi > 0.0f) DragRange *= Screen.dpi;
		else DragRange *= 96.0f;*/
	}

	void OnDestroy()
	{
		InputManager.TouchDownEvent -= OnTouchDown;
		InputManager.TouchDragEvent -= OnTouchDrag;
		InputManager.TouchUpEvent -= OnTouchUp;
	}

	void OnTouchDown(int fingerID, Vector2 pos, ref bool handled)
	{
		if(mFingerID != -1) return;
		
		//convert pixel pos to 0-1 ratio
		float x = pos.x / Screen.width;
		float y = pos.y / Screen.height;
		
		Vector2 ratioPos = new Vector2(x,y);
		if(!Region.Contains(ratioPos)) return;
			
		mFingerID = fingerID;
		
		if(Locked)
		{
			mStartPos.x = LockedStartPos.x * Screen.width;
			mStartPos.y = LockedStartPos.y * Screen.height;
		}
		else mStartPos = pos;
	}
	
	void OnTouchDrag(int fingerID, Vector2 pos, ref bool handled)
	{
		if(mFingerID != fingerID) return;
		
		IsHoldingDown = true;
		
		SwipeEffect tempSwipeEffect = new SwipeEffect();
		tempSwipeEffect.mSwipePos = pos;
		tempSwipeEffect.mSwipeTimer = 0.0f;
		mSwipeEffectList.Add(tempSwipeEffect);
		
		for(int i=0; i<mSwipeEffectList.Count; i++)
		{
			mSwipeEffectList[i].mSwipeTimer += Time.deltaTime;
			if(mSwipeEffectList[i].mSwipeTimer > 0.1f)
			{
				mSwipeEffectList.Remove(mSwipeEffectList[i]);
				i--;
			}
		}
		
		//mActiveDir = pos - mStartPos;
		mActiveDir = Vector2.ClampMagnitude (pos - mStartPos, DragRange);
		//Debug.Log ("SwipeController::TouchDrag(): finger: " + fingerID + ", pos:" + mActiveDir);
		
		Vector2 dirUnit = mActiveDir.normalized;	
		float angle = Mathf.Asin(dirUnit.y);
		angle *= Mathf.Rad2Deg;

		if(mActiveDir.y < 0) // TOP
		{		
			if(mActiveDir.x < 0) // LEFT
			{
				if(angle > -22.5f) mDirection = "West";
				else if(angle < -22.5f && angle > -67.5f) mDirection = "NorthWest";
				else if(angle < -67.5f && angle > -90) mDirection = "North";
			}			
			else if(mActiveDir.x > 0) // RIGHT
			{
				if(angle < -67.5f && angle > -90) mDirection = "North";
				else if(angle < -22.5f && angle > -67.5) mDirection = "NorthEast";
				else if(angle > -22.5f) mDirection = "East";
			}
		}
		else if(mActiveDir.y > 0) // BOTTOM
		{	
			if(mActiveDir.x < 0) // LEFT
			{
				if(angle < 22.5f) mDirection = "West";
				else if(angle > 22.5f && angle < 67.5f) mDirection = "SouthWest";
				else if(angle > 67.5f && angle < 90) mDirection = "South";
			}
			else if(mActiveDir.x > 0) // RIGHT
			{
				if(angle > 67.5f && angle < 90) mDirection = "South";
				else if(angle > 22.5f && angle < 67.5) mDirection = "SouthEast";
				else if(angle < 22.5f) mDirection = "East";
			}
		}
		
		if(IsHoldingDown && !IsDisableSwipeRegistration)
		{
			if(SwipeDirection != mDirection)
			{
				if (Vector3.Distance(mStartPos, pos) >= Screen.width/6)
				{
					mIsComplete = true;
					mIsSwipedCorrectly = false;
					ResetRectValue();
					Debug.Log ("Failed");
					IsDisableSwipeRegistration = true;
				}
			}
			else
			{
				if (Vector3.Distance(mStartPos, pos) >= Screen.width/3)
				{
					if(SwipeDirection == mDirection)
					{
						mIsComplete = true;
						mIsSwipedCorrectly = true;
						ResetRectValue();
						Debug.Log ("Completed");
						IsDisableSwipeRegistration = true;
					}
					Debug.Log (mDirection);
					Debug.Log (Vector3.Distance(mStartPos, pos));
				}
			}
		}
		
	}
	
	void OnTouchUp(int fingerID, Vector2 pos, ref bool handled)
	{		
		if(mFingerID != fingerID) return;
		
		mFingerID = -1;
		mActiveDir = Vector2.zero;
		IsHoldingDown = false;
		IsDisableSwipeRegistration = false;
		mSwipeEffectList.Clear();
	}
	
	void OnGUI()
	{
		if(mFingerID == -1) return;
		
		GUI.Label (new Rect(mStartPos.x, mStartPos.y, 100, 20), "Start");
		
		GUI.Label (new Rect
			(mStartPos.x + mActiveDir.x,
			mStartPos.y + mActiveDir.y, 100, 100), "Active");
		
		for(int i=0; i<mSwipeEffectList.Count; i++)
		{
			GUI.Label (new Rect(mSwipeEffectList[i].mSwipePos.x, mSwipeEffectList[i].mSwipePos.y, SwipeTexture.width, SwipeTexture.height), SwipeTexture);
		}
	}
	
	public void EnableSwipeContoller()
	{
		Region.x = 0;
		Region.y = 0;
		Region.width = 1;
		Region.height = 1;
	}
	
	public void ResetRectValue()
	{
		Region.x = 0;
		Region.y = 0;
		Region.width = 0;
		Region.height = 0;
	}
	
	public Vector3 GetDirection()
	{
		return new Vector3(mActiveDir.x, 0.0f, -mActiveDir.y);
	}
}
