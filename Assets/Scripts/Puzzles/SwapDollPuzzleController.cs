using UnityEngine;
using System.Collections;

public class SwapDollPuzzleController : MonoBehaviour {
	
	public bool IsEnabled = false;
	
	public Transform emptySlot;
	public Transform MoveToTarget;
	public Transform LookAtTarget;
	
	public Rect Region;
	public Rect Region2;
	public Rect Region3;
	public Rect Region4;
	public Rect Region5;
	public Rect Region6;
	public Rect Region7;
	public Rect Region8;
	public Rect Region9;
	public Rect Region10;
	public Rect Region11;
	
	private int[,] mOriginalArray;
	private int[,] mCurrentArray;
	private int[,] mActualArray;
	private int[,] mActualArray2;
	private int[,] mActualArray3;
	private int[,] mActualArray4;
	
	GameObject mClickedObject;
	GameObject Player;
		
	private int ID = -1;
	private Rect mClickedRegion;
	private Rect[] mDefaultRegion = new Rect[RECTMAX];
	private Vector3[] mDefaultPosition = new Vector3[RECTMAX+1];
	
	private Vector3 mStartPosition;
    private Vector3 mEndPosition;
	private float mStartTime;
	
	Quaternion mInitialRotation;
	Vector3 mEndRotation;
	
	bool mGetStartTime = true;
	bool mGetInPosition = true;
	bool mReturnToDefault = false;
	bool mIsReset = false;
	
	private int mFingerID = -1;
	Vector2 ratioPos;
	
	const int ROW = 4;
	const int COL = 4;
	const float RECTWIDTH = 0.13f;
	const float RECTHEIGHT = 0.175f;
	const int RECTMAX = 11;
	
	[HideInInspector]
	public bool mIsComplete = false;
	
	PlayerController mPlayerController;
	PlayerStatusScript mPlayerStatusScript;
	ExamineScript mExamineScript;
	PuzzleTimer mPuzzleTimer;
	
	void Start()
	{
		Player = GameObject.Find ("Player");
		mPlayerStatusScript = Player.GetComponent<PlayerStatusScript>();
		mExamineScript = GameObject.Find ("ExaminePlane(D)").GetComponent<ExamineScript>();
		mPuzzleTimer = GameObject.Find("DollPuzzle").GetComponent<PuzzleTimer>();
		
		mPlayerController = Player.GetComponent<PlayerController>();
			
		mDefaultRegion[0] = Region;
		mDefaultRegion[1] = Region2;
		mDefaultRegion[2] = Region3;
		mDefaultRegion[3] = Region4;
		mDefaultRegion[4] = Region5;
		mDefaultRegion[5] = Region6;
		mDefaultRegion[6] = Region7;
		mDefaultRegion[7] = Region8;
		mDefaultRegion[8] = Region9;
		mDefaultRegion[9] = Region10;
		mDefaultRegion[10] = Region11;
		
		mDefaultPosition[0] = GameObject.Find("Cube 1").transform.position;
		mDefaultPosition[1] = GameObject.Find("Cube 2").transform.position;
		mDefaultPosition[2] = GameObject.Find("Cube 3").transform.position;
		mDefaultPosition[3] = GameObject.Find("Cube 4").transform.position;
		mDefaultPosition[4] = GameObject.Find("Cube 5").transform.position;
		mDefaultPosition[5] = GameObject.Find("Cube 6").transform.position;
		mDefaultPosition[6] = GameObject.Find("Cube 7").transform.position;
		mDefaultPosition[7] = GameObject.Find("Cube 8").transform.position;
		mDefaultPosition[8] = GameObject.Find("Cube 9").transform.position;
		mDefaultPosition[9] = GameObject.Find("Cube 10").transform.position;
		mDefaultPosition[10] = GameObject.Find("Cube 11").transform.position;
		mDefaultPosition[11] = GameObject.Find("Empty Tile").transform.position;
		
		mOriginalArray  = new int[ROW, COL] { {-1, 7, 6, -1}, 
									      	 {-1, 3, 10, -1}, 
									      	 {11, 4, 2, 0},
		 									 {1, 9, 5, 8}};
		
		mCurrentArray  = new int[ROW, COL] { {-1, 7, 6, -1}, 
									      	 {-1, 3, 10, -1}, 
									      	 {11, 4, 2, 0},
		 									 {1, 9, 5, 8}};
		
		mActualArray  = new int[ROW - 2, COL] { {-1, 10, 11, -1}, 
									      	    {-1, 8, 9, -1}};
		
		mActualArray2  = new int[ROW - 2, COL] { {-1, 10, 11, -1}, 
									      	     {-1, 9, 8, -1}};
		
		mActualArray3  = new int[ROW - 2, COL] { {-1, 11, 10, -1}, 
									      	     {-1, 8, 9, -1}};
		
		mActualArray4  = new int[ROW - 2, COL] { {-1, 11, 10, -1}, 
									      	     {-1, 9, 8, -1}};
		
		InputManager.TouchDownEvent += OnTouchDown;
		InputManager.TouchUpEvent += OnTouchUp;
	}
	
	
	void OnDestroy()
	{
		InputManager.TouchDownEvent -= OnTouchDown;
		InputManager.TouchUpEvent -= OnTouchUp;
		
		mGetStartTime = true;
		mGetInPosition = true;
		mReturnToDefault = false;
		mIsReset = false;
		
		mFingerID = -1;

		mIsComplete = false;
	}

	
	void Update()
	{
		if(IsEnabled)
		{
			if(mGetInPosition)
			{
				if (mGetStartTime == true)
				{
					mStartPosition = Player.transform.position;
					mEndPosition = MoveToTarget.transform.position;
					mInitialRotation = Player.transform.rotation;
					
					mStartTime = Time.time;
					mGetStartTime = false;
				}
	
				Player.transform.position = Vector3.Lerp(mStartPosition, mEndPosition, 2.5f * (Time.time - mStartTime));
				Player.transform.rotation = Quaternion.Slerp(Player.transform.rotation, Quaternion.LookRotation(LookAtTarget.transform.position - Player.transform.position), Time.deltaTime * 10.0f);

				if(Player.transform.position == mEndPosition && Mathf.FloorToInt(Mathf.Abs(Player.transform.rotation.eulerAngles.y)) == Mathf.FloorToInt(Mathf.Abs(Quaternion.LookRotation(LookAtTarget.transform.position - Player.transform.position).eulerAngles.y)))
					//&& Mathf.FloorToInt(Mathf.Abs(Player.transform.rotation.eulerAngles.x)) == Mathf.FloorToInt(Mathf.Abs(Quaternion.LookRotation(LookAtTarget.transform.position - Player.transform.position).eulerAngles.x))
					//&& Mathf.FloorToInt(Mathf.Abs(Player.transform.rotation.eulerAngles.z)) == Mathf.FloorToInt(Mathf.Abs(Quaternion.LookRotation(LookAtTarget.transform.position - Player.transform.position).eulerAngles.z)))
				{
					mPlayerStatusScript.IsDisableBook = true;
					mPuzzleTimer.IsEnabled = true;
					mGetStartTime = true;
					mGetInPosition = false;
					
				}
			}
		}
		else if(!IsEnabled && mReturnToDefault)
		{
			if(mGetInPosition)
			{
				mPuzzleTimer.IsEnabled = false;
				
				if (mGetStartTime == true)
				{
					mStartTime = Time.time;
					mGetStartTime = false;
				}
				Player.transform.position = Vector3.Lerp(mEndPosition, mStartPosition, 2.5f * (Time.time - mStartTime));
				Player.transform.rotation = Quaternion.Slerp(Player.transform.rotation, mInitialRotation, Time.deltaTime * 2.5f);
				if(Player.transform.position == mStartPosition & Mathf.FloorToInt(Mathf.Abs(Player.transform.rotation.eulerAngles.y)) == Mathf.FloorToInt(Mathf.Abs(mInitialRotation.eulerAngles.y))
					&& Mathf.FloorToInt(Mathf.Abs(Player.transform.rotation.eulerAngles.x)) == Mathf.FloorToInt(Mathf.Abs(mInitialRotation.eulerAngles.x))
					&& Mathf.FloorToInt(Mathf.Abs(Player.transform.rotation.eulerAngles.z)) == Mathf.FloorToInt(Mathf.Abs(mInitialRotation.eulerAngles.z)))
				{
					mPlayerStatusScript.IsDisableBook = false;
					mGetStartTime = true;
					mReturnToDefault = false;
				}
			}
		}
		
		if(mPuzzleTimer.TimeIsUp)
		{
			IsEnabled = false;
			mIsReset = true;
			mGetInPosition = true;
			mReturnToDefault = true;
			mExamineScript.mTypeOfMessage = ExamineScript.TYPE_OF_MESSAGE.FAILED;
			mExamineScript.EnabledExamine = true;
			ResetToDefault();
		}
	}
	
	void OnTouchDown(int fingerID, Vector2 pos, ref bool handled)
	{
		if(IsEnabled)
		{
			if(mFingerID != -1) return;
		
			//convert pixel pos to 0-1 ratio
			float x = pos.x / Screen.width;
			float y = pos.y / Screen.height;
			
			ratioPos = new Vector2(x,y);
			if(!Region.Contains(ratioPos) && !Region2.Contains(ratioPos) && !Region3.Contains(ratioPos) 
				&& !Region4.Contains(ratioPos) && !Region5.Contains(ratioPos) && !Region6.Contains(ratioPos)
				&& !Region7.Contains(ratioPos) && !Region8.Contains(ratioPos) && !Region9.Contains(ratioPos)
				&& !Region10.Contains(ratioPos) && !Region11.Contains(ratioPos)) 
			{
				Debug.Log ("DFDFFD");
				return;
			}
				
			mFingerID = fingerID;
		}
	}

	void OnTouchUp(int fingerID, Vector2 pos, ref bool handled)
	{		
		if(IsEnabled)
		{
			if(mFingerID != fingerID) return;

			mFingerID = -1;
			CheckIfTileCanMove();
		}
	}

	void CheckIfTileCanMove()
	{	
		if (Region.Contains(ratioPos)) ID = 1;
		else if (Region2.Contains(ratioPos)) ID = 2;
		else if (Region3.Contains(ratioPos)) ID = 3;
		else if (Region4.Contains(ratioPos)) ID = 4;
		else if (Region5.Contains(ratioPos)) ID = 5;
		else if (Region6.Contains(ratioPos)) ID = 6;
		else if (Region7.Contains(ratioPos)) ID = 7;
		else if (Region8.Contains(ratioPos)) ID = 8; 
		else if (Region9.Contains(ratioPos)) ID = 9; 
		else if (Region10.Contains(ratioPos)) ID = 10; 
		else if (Region11.Contains(ratioPos)) ID = 11; 

		SwapPositionInArray();
		CheckIfCompleted();
	}
	
	void SwapPositionInArray()
	{
		int row = 0; 
		int col = 0;
		bool isSwap = false;
		
		FindClickedRegionGUI();
		
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
				mClickedRegion.y -= 0.215f;
				isSwap = true;
				//Debug.Log ("swap up");
            }
		}
		if (row != (ROW - 1))
		{
			if (mCurrentArray[row+1, col] == 0) // Check Down
            {
                mCurrentArray[row+1, col] = ID;
				mCurrentArray[row, col] = 0;
				mClickedRegion.y += 0.215f;
				isSwap = true;
				//Debug.Log ("swap down");
            }
		}
		if (col != 0)
		{
			if (mCurrentArray[row, col-1] == 0) // Check Left
            {
                mCurrentArray[row, col-1] = ID;
				mCurrentArray[row, col] = 0;
				mClickedRegion.x -= 0.16f;
				isSwap = true;
				//Debug.Log ("swap left");
            }
		}
		if (col != (COL - 1))
		{
			if (mCurrentArray[row, col+1] == 0) // Check Right
        	{
				mCurrentArray[row, col+1] = ID;
				mCurrentArray[row, col] = 0;
				mClickedRegion.x += 0.16f;
				isSwap = true;
				//Debug.Log ("swap right");
        	}
		}
		
		SwapClickedRegionGUI();
		
		if(isSwap == true)
		{
			SwapPositionIn3D();
			isSwap = false;
		}
	}
	
	void SwapPositionIn3D()
	{
		float tempX, tempY, tempZ;
		
		FindClickedObject();
		
		tempX = mClickedObject.transform.position.x;
		tempY = mClickedObject.transform.position.y;
		tempZ = mClickedObject.transform.position.z;
		mClickedObject.transform.position = emptySlot.position;
		
		Vector3 endPosition = emptySlot.position; 
		endPosition.x = tempX;
		endPosition.y = tempY; 
		endPosition.z = tempZ; 
		emptySlot.position = endPosition;
	}
	
	void FindClickedRegionGUI()
	{
		if(ID == 1) mClickedRegion = Region;
		else if(ID == 2) mClickedRegion = Region2;
		else if(ID == 3) mClickedRegion = Region3;
		else if(ID == 4) mClickedRegion = Region4;
		else if(ID == 5) mClickedRegion = Region5;
		else if(ID == 6) mClickedRegion = Region6;
		else if(ID == 7) mClickedRegion = Region7;
		else if(ID == 8) mClickedRegion = Region8;
		else if(ID == 9) mClickedRegion = Region9;
		else if(ID == 10) mClickedRegion = Region10;
		else if(ID == 11) mClickedRegion = Region11;
	}
	
	void SwapClickedRegionGUI()
	{
		if(ID == 1) Region = mClickedRegion;
		else if(ID == 2) Region2 = mClickedRegion;
		else if(ID == 3) Region3 = mClickedRegion;
		else if(ID == 4) Region4 = mClickedRegion;
		else if(ID == 5) Region5 = mClickedRegion;
		else if(ID == 6) Region6 = mClickedRegion;
		else if(ID == 7) Region7 = mClickedRegion;
		else if(ID == 8) Region8 = mClickedRegion;
		else if(ID == 9) Region9 = mClickedRegion;
		else if(ID == 10) Region10 = mClickedRegion;
		else if(ID == 11) Region11 = mClickedRegion;
	}
	
	void FindClickedObject()
	{
		if(ID == 1) mClickedObject = GameObject.Find("Cube 1");
		else if(ID == 2) mClickedObject = GameObject.Find("Cube 2");
		else if(ID == 3) mClickedObject = GameObject.Find("Cube 3");
		else if(ID == 4) mClickedObject = GameObject.Find("Cube 4");
		else if(ID == 5) mClickedObject = GameObject.Find("Cube 5");
		else if(ID == 6) mClickedObject = GameObject.Find("Cube 6");
		else if(ID == 7) mClickedObject = GameObject.Find("Cube 7");
		else if(ID == 8) mClickedObject = GameObject.Find("Cube 8");
		else if(ID == 9) mClickedObject = GameObject.Find("Cube 9");
		else if(ID == 10) mClickedObject = GameObject.Find("Cube 10");
		else if(ID == 11) mClickedObject = GameObject.Find("Cube 11");
	}
	
	void CheckIfCompleted()
	{
		for (int i = 0; i < ROW - 2; i++)
		{
			for (int j = 0; j < COL; j++)
			{
				if (mCurrentArray[i,j] == mActualArray[i,j])
				{
					mIsComplete = true;
				}
				else if (mCurrentArray[i,j] == mActualArray2[i,j])
				{
					mIsComplete = true;
				}
				else if (mCurrentArray[i,j] == mActualArray3[i,j])
				{
					mIsComplete = true;
				}
				else if (mCurrentArray[i,j] == mActualArray4[i,j])
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
			GameObject.Find ("TriggerYuriko").GetComponent<TriggerScript>().IsEnabled = true;
			
			mIsComplete = false;
			IsEnabled = false;
			mGetInPosition = true;
			mReturnToDefault = true;
			mPuzzleTimer.IsEnabled = false;
			mExamineScript.mTypeOfMessage = ExamineScript.TYPE_OF_MESSAGE.COMPLETED;
			mExamineScript.EnabledExamine = true;
		}
	}
	
	void ResetToDefault()
	{
		if(mIsReset)
		{
			Debug.Log ("Reset");
			DefaultRegion();
			DefaultPosition();
			//mCurrentArray = mOriginalArray;
			mIsReset = false;
			mPuzzleTimer.TimeIsUp = false;
			
			for(int i = 0; i < ROW; i++)
			{
				for(int j = 0; j < COL; j++)
				{
					mCurrentArray[i,j] = mOriginalArray[i,j];
				}
			}
		}
	}
	
	void DefaultRegion()
	{
		Region = mDefaultRegion[0];
		Region2 = mDefaultRegion[1];
		Region3 = mDefaultRegion[2];
		Region4 = mDefaultRegion[3];
		Region5 = mDefaultRegion[4];
		Region6 = mDefaultRegion[5];
		Region7 = mDefaultRegion[6];
		Region8 = mDefaultRegion[7];
		Region9 = mDefaultRegion[8];
		Region10 = mDefaultRegion[9];
		Region11 = mDefaultRegion[10];
	}
	
	void DefaultPosition()
	{
		GameObject.Find("Cube 1").transform.position = mDefaultPosition[0];
		GameObject.Find("Cube 2").transform.position = mDefaultPosition[1];
		GameObject.Find("Cube 3").transform.position = mDefaultPosition[2];
		GameObject.Find("Cube 4").transform.position = mDefaultPosition[3];
		GameObject.Find("Cube 5").transform.position = mDefaultPosition[4];
		GameObject.Find("Cube 6").transform.position = mDefaultPosition[5];
		GameObject.Find("Cube 7").transform.position = mDefaultPosition[6];
		GameObject.Find("Cube 8").transform.position = mDefaultPosition[7];
		GameObject.Find("Cube 9").transform.position = mDefaultPosition[8];
		GameObject.Find("Cube 10").transform.position = mDefaultPosition[9];
		GameObject.Find("Cube 11").transform.position = mDefaultPosition[10];
		GameObject.Find("Empty Tile").transform.position = mDefaultPosition[11];
	}
	
	void OnGUI()
	{
		if(IsEnabled)
		{
			GUI.Button (new Rect(Region.x * Screen.width, Region.y * Screen.height, RECTWIDTH*Screen.width, RECTHEIGHT*Screen.height), "Start");
			GUI.Button (new Rect(Region2.x * Screen.width, Region2.y * Screen.height, RECTWIDTH*Screen.width, RECTHEIGHT*Screen.height), "Start");
			GUI.Button (new Rect(Region3.x * Screen.width, Region3.y * Screen.height, RECTWIDTH*Screen.width, RECTHEIGHT*Screen.height), "Start");
			GUI.Button (new Rect(Region4.x * Screen.width, Region4.y * Screen.height, RECTWIDTH*Screen.width, RECTHEIGHT*Screen.height), "Start");
			GUI.Button (new Rect(Region5.x * Screen.width, Region5.y * Screen.height, RECTWIDTH*Screen.width, RECTHEIGHT*Screen.height), "Start");
			GUI.Button (new Rect(Region6.x * Screen.width, Region6.y * Screen.height, RECTWIDTH*Screen.width, RECTHEIGHT*Screen.height), "Start");
			GUI.Button (new Rect(Region7.x * Screen.width, Region7.y * Screen.height, RECTWIDTH*Screen.width, RECTHEIGHT*Screen.height), "Start");
			GUI.Button (new Rect(Region8.x * Screen.width, Region8.y * Screen.height, RECTWIDTH*Screen.width, RECTHEIGHT*Screen.height), "Start");
			GUI.Button (new Rect(Region9.x * Screen.width, Region9.y * Screen.height, RECTWIDTH*Screen.width, RECTHEIGHT*Screen.height), "Start");
			GUI.Button (new Rect(Region10.x * Screen.width, Region10.y * Screen.height, RECTWIDTH*Screen.width, RECTHEIGHT*Screen.height), "Start");
			GUI.Button (new Rect(Region11.x * Screen.width, Region11.y * Screen.height, RECTWIDTH*Screen.width, RECTHEIGHT*Screen.height), "Start");
		}
	}
}
