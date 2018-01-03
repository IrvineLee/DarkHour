using UnityEngine;
using System.Collections;

public class ClickController : MonoBehaviour {
	
	public string Name;
	
	public enum TypeOfClick
	{
		NONE = 0,
		CLICK_ONCE = 1,
		CLICK_REPEATEDLY = 2,
		CLICK_IN_SEQUENCE = 3,
		CLICK_REPEATEDLY_IN_SEQUENCE = 4
	}
	public TypeOfClick ClickType;	
	
	public int NoOfClick = 1;
	
	public Rect Region1;
	public Rect Region2;
	public Rect Region3;
	public Rect Region4;
	public Rect Region5;

	private int mFingerID = -1;
	//private Vector2 mClickPos;
	
	public int NoOfSequence;
	private bool[] mClickSequenceArray;
	private bool mSequenceIsComplete = false;
	
	[HideInInspector]
	public int mClickCount = 0;
	[HideInInspector]
	public bool mIsComplete = false;
	[HideInInspector]
	public bool IsClicked = false;
	[HideInInspector]
	public bool IsAnimationScene = false;
	[HideInInspector]
	public int SequenceButton;
	
	public bool mIsClickedCorrectly = false;
	
	void Start()
	{
		InputManager.TouchDownEvent += OnTouchDown;
		InputManager.TouchUpEvent += OnTouchUp;
		
		mClickSequenceArray = new bool[4]{false, false, false, false};
		//CheckNumberOfSequence();
	}

	void OnDestroy()
	{
		InputManager.TouchDownEvent -= OnTouchDown;
		InputManager.TouchUpEvent -= OnTouchUp;
	}

	void Update()
	{
		if (ClickType == TypeOfClick.CLICK_REPEATEDLY_IN_SEQUENCE && !IsAnimationScene)
		{
			if((Input.GetKeyDown(KeyCode.Z)) && mClickSequenceArray[0] == false)
			{
				mClickCount += 1;
				
				if(!IsClicked)
				{
					IsClicked = true;
					//Debug.Log (mClickCount);
					mClickSequenceArray[0] = true;
					mClickSequenceArray[1] = true;
				}
				
				CheckIfCompleted();
			}
			else if((Input.GetKeyDown(KeyCode.C)) && mClickSequenceArray[1] == true)
			{
				mClickCount += 1;
				if(!IsClicked)
				{
					IsClicked = true;
					//Debug.Log (mClickCount);
					mClickSequenceArray[0] = false;
					mClickSequenceArray[1] = false;
				}
				CheckIfCompleted();
			}
		}
	}
	
	void OnTouchDown(int fingerID, Vector2 pos, ref bool handled)
	{
		if(mFingerID != -1) return;
		
		//convert pixel pos to 0-1 ratio
		float x = pos.x / Screen.width;
		float y = pos.y / Screen.height;

		Vector2 ratioPos = new Vector2(x,y);
		if(!Region1.Contains(ratioPos) && !Region2.Contains(ratioPos)
		&& !Region3.Contains(ratioPos) && !Region4.Contains(ratioPos)
		&& !Region5.Contains(ratioPos)) 
		{
			return;
		}
			
		mFingerID = fingerID;
		//mClickPos = pos;
	}
	
	void OnTouchUp(int fingerID, Vector2 pos, ref bool handled)
	{
		if (mFingerID != fingerID) return;
		mFingerID = -1;
		
		if (ClickType == TypeOfClick.CLICK_ONCE)
		{
			mClickCount += 1;
			Debug.Log ("Click Once: " + mClickCount);
			
			ClickType = TypeOfClick.NONE;
			Region1 = ResetRectValue(Region1);
			mClickCount = 0;
			mIsComplete = true;
		}
		else if (ClickType == TypeOfClick.CLICK_REPEATEDLY)
		{
			mClickCount += 1;

			Debug.Log (mClickCount);
			if (mClickCount == NoOfClick)
			{
				Debug.Log ("Completed");
				ClickType = TypeOfClick.NONE;
				Region1 = ResetRectValue(Region1);
				mClickCount = 0;
				mIsComplete = true;
			}
		}
		else if (ClickType == TypeOfClick.CLICK_IN_SEQUENCE)
		{
			float x = pos.x / Screen.width;
			float y = pos.y / Screen.height;
			
			Vector2 ratioPos = new Vector2(x,y);
			if(Region1.Contains(ratioPos))
			{
				Debug.Log ("Click Region 1!!");
				Region1 = ResetRectValue(Region1);
				SequenceButton += 1;
				mClickSequenceArray[0] = true;
			}
			else if(Region2.Contains(ratioPos) && mClickSequenceArray[0] == true)
			{
				Debug.Log ("Click Region 2!!");
				Region2 = ResetRectValue(Region2);
				SequenceButton += 1;
				CheckSequenceIfCompleted();
				mClickSequenceArray[1] = true;
			}
			else if(Region3.Contains(ratioPos) && mClickSequenceArray[1] == true)
			{
				Debug.Log ("Click Region 3!!");
				Region3 = ResetRectValue(Region3);
				SequenceButton += 1;
				CheckSequenceIfCompleted();
				mClickSequenceArray[2] = true;
			}
			else if(Region4.Contains(ratioPos) && mClickSequenceArray[2] == true)
			{
				Debug.Log ("Click Region 4!!");
				Region4 = ResetRectValue(Region4);
				SequenceButton += 1;
				CheckSequenceIfCompleted();
				mClickSequenceArray[3] = true;
			}
			else if(Region5.Contains(ratioPos) && mClickSequenceArray[3] == true)
			{
				Debug.Log ("Click Region 5!!");
				Region5 = ResetRectValue(Region5);
				SequenceButton += 1;
				CheckSequenceIfCompleted();
			}
			else
			{
				Debug.Log ("Failed!!");
				mIsClickedCorrectly = false;
				mIsComplete = true;
				SequenceButton = 0;
				ResetAllRectValue();
			}
		}
		else if (ClickType == TypeOfClick.CLICK_REPEATEDLY_IN_SEQUENCE)
		{
			float x = pos.x / Screen.width;
			float y = pos.y / Screen.height;
			
			Vector2 ratioPos = new Vector2(x,y);
			
			if(Region1.Contains(ratioPos) && mClickSequenceArray[0] == false)
			{
				mClickCount += 1;
				
				if(IsAnimationScene || !IsClicked)
				{
					IsClicked = true;
					Debug.Log (mClickCount);
					mClickSequenceArray[0] = true;
					mClickSequenceArray[1] = true;
				}
				
				CheckIfCompleted();
			}
			else if(Region2.Contains(ratioPos) && mClickSequenceArray[1] == true)
			{
				mClickCount += 1;
				if(IsAnimationScene || !IsClicked)
				{
					IsClicked = true;
					Debug.Log (mClickCount);
					mClickSequenceArray[0] = false;
					mClickSequenceArray[1] = false;
				}
				CheckIfCompleted();
			}
		}
	}
	
	void CheckNumberOfSequence()
	{
		if(Region2.height != 0 || Region2.width != 0) NoOfSequence += 1;
		if(Region3.height != 0 || Region3.width != 0) NoOfSequence += 1;
		if(Region4.height != 0 || Region4.width != 0) NoOfSequence += 1;
		if(Region5.height != 0 || Region5.width != 0) NoOfSequence += 1;
	}
	
	void CheckIfCompleted()
	{
		if(mClickCount == NoOfClick) 
		{
			Debug.Log ("Complete!!");
			ResetEverything();
		}
	}
	
	void CheckSequenceIfCompleted()
	{
		for(int i = 0; i < NoOfSequence; i++)
		{
			if(mClickSequenceArray[i] == true)
			{
				mSequenceIsComplete = true;
			}
			else mSequenceIsComplete = false;
		}
		
		if(mSequenceIsComplete)
		{
			Debug.Log ("Sequence Complete!!");
			mIsClickedCorrectly = true;
			ResetEverything();
		}
		else Debug.Log ("Still have more to click!");
	}
	
	void ResetClickSequenceArray()
	{
		for(int i = 0; i < 4; i++)
		{
			mClickSequenceArray[i] = false;
		}
	}
	
	void ResetEverything()
	{
		ClickType = TypeOfClick.NONE;
		mIsComplete = true;
		ResetAllRectValue();
		ResetClickSequenceArray();
		mClickCount = 0;
		SequenceButton = 0;
	}
	
	Rect ResetRectValue(Rect Region)
	{
		Region.x = 0;
		Region.y = 0;
		Region.width = 0;
		Region.height = 0;
		return Region;
	}
	
	public void ResetAllRectValue()
	{
		Region1 = ResetRectValue(Region1);
		Region2 = ResetRectValue(Region2);
		Region3 = ResetRectValue(Region3);
		Region4 = ResetRectValue(Region4);
		Region5 = ResetRectValue(Region5);
	}
	
	public void ResetClickCount()
	{
		mClickCount = 0;
	}
	
	void OnGUI()
	{
		if(mFingerID == -1) return;
	}
	
	/*public Vector3 GetPosition()
	{
		return new Vector3(mClickPos.x, 0.0f, mClickPos.y);
	}*/
}
