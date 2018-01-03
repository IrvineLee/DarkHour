using UnityEngine;
using System.Collections;

public class SlidePuzzleController : MonoBehaviour {
	
	public Transform emptySlot;
	public int tileID;

	static int[,] mCurrentArray;
	static int[,] mActualArray;
	static bool mIsComplete = false;
	
	const int ROW = 3;
	const int COL = 3;
	const float RECTWIDTH = 0.08f;
	const float RECTHEIGHT = 0.108f;
	
	public Rect Region;
	
	private int mFingerID = -1;
	private int mTileID = -1;	
	
	private Vector3 mStartPosition;
    private Vector3 mEndPosition;
	private float mStartTime;
	private float mTileSpeed = 7.0f;
	bool mSlideTile = false;
	bool mGetStartTime = false;
	static bool mTileInMotion = false;
	
	bool mIsSwap = false;
	
	SlidePuzzleScript mSlidePuzzleScript;
	ExamineScript mExamineScript;
	
	void Start()
	{
		mSlidePuzzleScript = GameObject.Find ("SlidePuzzle").GetComponent<SlidePuzzleScript>();
		mExamineScript = GameObject.Find ("ExaminePlane(S)").GetComponent<ExamineScript>();
		
		mCurrentArray  = new int[ROW, COL] { {8, 7, 0}, 
									      	 {4, 5, 6}, 
									      	 {1, 2, 3} };
		
		mActualArray  = new int[ROW, COL]{ {1, 2, 3}, 
									       {4, 5, 6}, 
									       {7, 8, 0} };
		
		Region.width = RECTWIDTH;
		Region.height = RECTHEIGHT;
		
		mIsComplete = false;
		mTileInMotion = false;
		mTileSpeed = 7.0f;
		mSlideTile = false;
		mGetStartTime = false;
		mTileInMotion = false;
		
		mIsSwap = false;
		
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
		if(mSlidePuzzleScript.mIsInPosition)
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
		if(mSlidePuzzleScript.mIsInPosition)
		{
			if(mFingerID != fingerID) return;
			mFingerID = -1;
			if(!mTileInMotion)
			{
				CheckIfTileCanMove();
			}
		}
	}
	
	void OnTouchDrag(int fingerID, Vector2 pos, ref bool handled)
	{
		
	}

	void Update() 
	{
		if(mSlideTile == true)
		{
			if (mGetStartTime == true)
			{
				mStartTime = Time.time;
				mGetStartTime = false;
			}

			transform.position = Vector3.Lerp(mStartPosition, mEndPosition, mTileSpeed * (Time.time - mStartTime));
			mTileInMotion = true;
			
			if(transform.position == mEndPosition) 
			{
				mSlideTile = false;
				mTileInMotion = false;
				CheckIfCompleted();
			}
		}       
    }
	
	void CheckIfTileCanMove()
	{	
		if (tileID == 1) mTileID = tileID;
		else if (tileID == 2) mTileID = tileID;
		else if (tileID == 3) mTileID = tileID;
		else if (tileID == 4) mTileID = tileID;
		else if (tileID == 5) mTileID = tileID;
		else if (tileID == 6) mTileID = tileID;
		else if (tileID == 7) mTileID = tileID;
		else if (tileID == 8) mTileID = tileID; 
		
		SwapPositionInArray(mTileID);
	}
	
	void SwapPositionIn3D(int ID)
	{
		mStartPosition = transform.position;
		mEndPosition = emptySlot.position;
	
		Vector3 temp = emptySlot.position; 
		temp.x = transform.position.x;
		temp.y = transform.position.y; 
		temp.z = transform.position.z; 
		emptySlot.position = temp;
	
		mSlideTile = true;
		mGetStartTime = true;
	}
	
	void SwapPositionInArray(int ID)
	{
		int row = 0; 
		int col = 0;
		
		// Get the clicked tile position in array
		for (int i = 0; i < ROW; i++)
		{
			for (int j = 0; j < COL; j++)
			{
				if (mCurrentArray[i,j] == ID)
				{
					row = i;
					col = j;
				}
			}		
		}

		// Swap position in array
		if (row != 0)
		{
			if (mCurrentArray[row-1, col] == 0) // Check Up
            {
                mCurrentArray[row-1, col] = ID;
				mCurrentArray[row, col] = 0;
				Region.y -= 0.105f;
				mIsSwap = true;
				Debug.Log ("swap up");
            }
		}
		if (row != (ROW - 1))
		{
			if (mCurrentArray[row+1, col] == 0) // Check Down
            {
                mCurrentArray[row+1, col] = ID;
				mCurrentArray[row, col] = 0;
				Region.y += 0.105f;
				mIsSwap = true;
				Debug.Log ("swap down");
            }
		}
		if (col != 0)
		{
			if (mCurrentArray[row, col-1] == 0) // Check Left
            {
                mCurrentArray[row, col-1] = ID;
				mCurrentArray[row, col] = 0;
				Region.x -= 0.08f;
				mIsSwap = true;
				Debug.Log ("swap left");
            }
		}
		if (col != (COL - 1))
		{
			if (mCurrentArray[row, col+1] == 0) // Check Right
        	{
				mCurrentArray[row, col+1] = ID;
				mCurrentArray[row, col] = 0;
				Region.x += 0.08f;
				mIsSwap = true;
				Debug.Log ("swap right");
        	}
		}
		
		if(mIsSwap)
		{
			SwapPositionIn3D(ID);
			mIsSwap = false;
		}
	}
	
	void CheckIfCompleted()
	{
		for (int i = 0; i < ROW; i++)
		{
			for (int j = 0; j < COL; j++)
			{
				if (mCurrentArray[i,j] == mActualArray[i,j])
				{
					mIsComplete = true;
				}
				else
				{
					mIsComplete = false;
					return;
				}
			}
		}
		
		if (mIsComplete == true)
		{
			Debug.Log ("Completed!!");
			mSlidePuzzleScript.IsEnabled = true;
			mSlidePuzzleScript.mIsFinished = true;
			mExamineScript.mTypeOfMessage = ExamineScript.TYPE_OF_MESSAGE.COMPLETED;
			mExamineScript.EnabledExamine = true;
		}
	}
	
	/*void OnGUI()
	{
		if(mSlidePuzzleScript.mIsInPosition)
		{
			GUI.Button (new Rect(Region.x * Screen.width, Region.y * Screen.height, RECTWIDTH*Screen.width, RECTHEIGHT*Screen.height), "");
		}
	}*/
}
