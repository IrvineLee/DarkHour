using UnityEngine;
using System.Collections;

public class RotatePuzzleController : MonoBehaviour {
	
	public Transform CenterTile;
	public int tileID;

	//const float RECTWIDTH = 0.082f;
	//const float RECTHEIGHT = 0.108f;
	const int COL = 5;
	const int MAXTURN = 7;
	
	public Rect Region;
	public float DragRange = 1.0f;
	public bool Locked = false;
	public Vector2 LockedStartPos;
	
	private int mFingerID = -1;
	private Vector2 mStartPos;
	private Vector2 mActiveDir;
	private string DragDirection;

	private float mRotationY;
	private float mCumulativeY;
	private float mRotationAngle = 90.0f;
	
	static bool[] mHasRotateToDefault;
	static bool mAllHasRotated = false;
	bool mRotate = false;
	static bool mTileInMotion = false;
	static bool mIsComplete = false;
	public static int mTurnCount = MAXTURN;
	
	static int[] mCurrentArray;
	static int[] mActualArray;
	
	RotatePuzzleScript mRotatePuzzleScript;
	ExamineScript mExamineScript;
	
	void Start()
	{
		mRotatePuzzleScript = GameObject.Find ("RotatePuzzle").GetComponent<RotatePuzzleScript>();
		mExamineScript = GameObject.Find ("ExaminePlane(R)").GetComponent<ExamineScript>();
		
		//Region.width = RECTWIDTH;
		//Region.height = RECTHEIGHT;
		
		mHasRotateToDefault = new bool[COL] {false,false,false,false,false};
		mCurrentArray  = new int[COL] {0, 0, 0, 0, 0};
		mActualArray = new int[COL] {3, 1, 1, 2, 7};
		
		mTurnCount = MAXTURN;
		mAllHasRotated = false;
		mRotate = false;
		mTileInMotion = false;
		mIsComplete = false;
		
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

	void Update() 
	{
		if(mRotate)
		{
			if(tileID == 3)
			{
				Debug.Log ("================");
			}
			mTileInMotion = true;
			mRotationY = Time.deltaTime * 150;
			mCumulativeY += mRotationY;
			//Debug.Log (mCumulativeY);
			
			if(mCumulativeY > mRotationAngle)
			{
				mCumulativeY -= mRotationY;
				mRotationY = mRotationAngle - mCumulativeY;
				//mCumulativeY += mRotationY;
				//Debug.Log (mCumulativeY);
				mRotate = false;
				mCumulativeY = 0;
				mTileInMotion = false;
				CheckIfComepleted();
			}
			transform.Rotate(0, -mRotationY, 0);
			CenterTile.Rotate(0, mRotationY, 0);
		}
		
		if(mRotatePuzzleScript.mIsFinished)
		{
			ResetToDefault();
		}
    }
	
	void ResetToDefault()
	{
		for(int i = 0; i < COL; i++)
		{
			if(mHasRotateToDefault[i]) mAllHasRotated = true;
			else 
			{
				mAllHasRotated = false;
				break;
			}
		}
		
		if(mAllHasRotated)
		{
			mHasRotateToDefault[0] = false;
			mHasRotateToDefault[1] = false;
			mHasRotateToDefault[2] = false;
			mHasRotateToDefault[3] = false;
			mHasRotateToDefault[4] = false;
			mAllHasRotated = false;
			mRotatePuzzleScript.mIsFinished = false;
		}
		
		if(tileID == 1 && !mHasRotateToDefault[0])
		{
			GameObject.Find("TopLeftTile").transform.Rotate(0, mCurrentArray[0] * mRotationAngle, 0);
			mCurrentArray[0] = 0;
			mRotate = false;
			mHasRotateToDefault[0] = true;
		}
		if(tileID == 2 && !mHasRotateToDefault[1])
		{
			GameObject.Find("TopRightTile").transform.Rotate(0, mCurrentArray[1] * mRotationAngle, 0);
			mCurrentArray[1] = 0;
			mRotate = false;
			mHasRotateToDefault[1] = true;
		}
		if(tileID == 3 && !mHasRotateToDefault[2])
		{
			GameObject.Find("BtmLeftTile").transform.Rotate(0, mCurrentArray[2] * mRotationAngle, 0);
			mCurrentArray[2] = 0;
			mRotate = false;
			mHasRotateToDefault[2] = true;
		}
		if(tileID == 4 && !mHasRotateToDefault[3])
		{
			GameObject.Find("BtmRightTile").transform.Rotate(0, mCurrentArray[3] * mRotationAngle, 0);
			mCurrentArray[3] = 0;
			mRotate = false;
			mHasRotateToDefault[3] = true;
		}
		if(tileID == 5 && !mHasRotateToDefault[4])
		{
			Debug.Log ("dfgdfg"+mCurrentArray[4]);
			GameObject.Find("CenterTile").transform.Rotate(0, -(mCurrentArray[4] * mRotationAngle), 0);
			mCurrentArray[4] = 0;
			mRotate = false;
			mHasRotateToDefault[4] = true;
		}
	}
	
	void OnTouchDown(int fingerID, Vector2 pos, ref bool handled)
	{
		if(mRotatePuzzleScript.mIsInPosition)
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
	}
	
	void OnTouchDrag(int fingerID, Vector2 pos, ref bool handled)
	{
	}
	
	void OnTouchUp(int fingerID, Vector2 pos, ref bool handled)
	{		
		if(mRotatePuzzleScript.mIsInPosition)
		{
			if(mFingerID != fingerID) return;
			mFingerID = -1;
			mActiveDir = Vector2.zero;
			if(!mTileInMotion)
			{
				mRotate = true;
				
			}
		}
	}
	
	void CheckIfComepleted()
	{
		if (tileID == 1) mCurrentArray[0] += 1;
		else if (tileID == 2) mCurrentArray[1] += 1;
		else if (tileID == 3) mCurrentArray[2] += 1;
		else if (tileID == 4) mCurrentArray[3] += 1;
		mCurrentArray[4] += 1;
		mTurnCount -= 1;
		Debug.Log ("Turn:" + mTurnCount);
		Debug.Log ("array 0  "+mCurrentArray[4]);
		for(int i = 0; i < COL; i++)
		{
			if(mCurrentArray[i] == mActualArray[i]) mIsComplete = true;
			else 
			{
				mIsComplete = false;
				break;
			}
		}
		
		if(mIsComplete)
		{
			mRotatePuzzleScript.IsEnabled = true;
			mExamineScript.mTypeOfMessage = ExamineScript.TYPE_OF_MESSAGE.COMPLETED;
			mExamineScript.EnabledExamine = true;
		}
		else if(mTurnCount == 0) 
		{
			mTurnCount = MAXTURN;
			mRotatePuzzleScript.IsEnabled = true;
			mExamineScript.mTypeOfMessage = ExamineScript.TYPE_OF_MESSAGE.FAILED;
			mExamineScript.EnabledExamine = true;
			mRotatePuzzleScript.mIsFinished = true;
			ResetToDefault ();
		}
	}
	
	/*void OnGUI()
	{
		if(mRotatePuzzleScript.mIsInPosition)
		{
			GUI.Button (new Rect(Region.x * Screen.width, Region.y * Screen.height, Region.width*Screen.width, Region.height*Screen.height), "Start");
		}
	}*/
}
