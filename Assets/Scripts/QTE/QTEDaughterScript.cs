using UnityEngine;
using System.Collections;

public class QTEDaughterScript : QTEScript 
{
	AudioSource mAudioSource;
	public GUIStyle MyStyle;
	public Texture2D mButton;
	public Texture2D mSwipe;
	public Texture2D mButtonPressed;
	public Texture2D mButtonSequence1;
	public Texture2D mButtonSequence2;
	public Texture2D mButtonSequence3;
	public Texture2D mButtonSequence4;
	public static bool IsEnabled = false;
	
	public enum ActionSequence
	{
		NONE = 0,
		CRAWL,
		BRACE,
		FEND_OFF_CLICK,
		FEND_OFF_SWIPE
	}
	public static ActionSequence mActionSequence = ActionSequence.FEND_OFF_CLICK;
	
	private int mNoOfButton;
	private int mSwipeNo;
	private int mIncrementClick;
	private int mRedoActionTimes;
	private float mValueOffSet = 0.1f;
	private float mMovedDistance;
	
	private float mTime = 0.0f;
	private float mTimeInput = 3.0f;
	private float mTimeCrawling;
	private float mTimerCrawlingMax = 6.0f;
	private float mImageTime = 0.0f;
	private float mImageTimeMax = 0.5f;
	
	bool mIsCompleted = false;
	bool mSetInitialVal = true;
	bool mIsMovePlayer = false;
	bool mIsGetClickVal = true;
	bool mIsChangeAction = false;
	
	public static bool IsCrawlOnly = false;
	GameObject Player;
	
	PlayerStatusScript mPlayerStatusScript;
	TriggerScript mTriggerScript;
	ClickController mClickController;
	SwipeController mSwipeController;
	
	void Start () 
	{
		Player = GameObject.Find("Player");
		mPlayerStatusScript = Player.GetComponent<PlayerStatusScript>();
		mSwipeController = Player.GetComponent<SwipeController>();
		mClickController = Player.GetComponent<ClickController>();
		mTriggerScript = GameObject.Find ("TriggerDaughter").GetComponent<TriggerScript>();
		mAudioSource = GameObject.Find ("ScreamSound2").GetComponent<AudioSource>();
		mButton = (Texture2D)Resources.Load("btn_press");
		
		gameObjectPrefab = GameObject.FindGameObjectWithTag("Yuriko");
		AddAnimationSequence("MoveBackScript", 5.5f, false);
		
		IsEnabled = false;
		IsCrawlOnly = false;
		mActionSequence = ActionSequence.CRAWL;
		mButtonSequence1 = (Texture2D)Resources.Load("1 copy");
		mButtonSequence2 = (Texture2D)Resources.Load("2 copy");
		mButtonSequence3 = (Texture2D)Resources.Load("3 copy");
		mButtonSequence4 = (Texture2D)Resources.Load("4 copy");

		mButton = (Texture2D)Resources.Load("btn_press");
		mButtonPressed = (Texture2D)Resources.Load("gui_press");
		mSwipe = (Texture2D)Resources.Load("gui_swipeLeft");
		MyStyle = new GUIStyle();
	}
	
	void Update () 
	{
		base.Update();
		
		if(mActionSequence == ActionSequence.NONE)
		{
			mIsCompleted = true;
			SetValueToDefault();
			SetButtonToDefault();
			mPlayerStatusScript.IsPlayingAnimation = false;
			IsEnabled = false;
		}
		else if(mActionSequence == ActionSequence.CRAWL)
		{
			if(mSetInitialVal)
			{
				mClickController.NoOfClick = -1;
				mClickController.IsAnimationScene = false;
				mClickController.IsClicked = false;
				mTypeOfInteraction = QTEScript.TYPE_OF_INTERACTION.BUTTON;
				mSetInitialVal = false;
				mIsCompleted = false;
				mPlayerStatusScript.IsPlayingAnimation = false;
			}
			
			CrawlingSequence();
			
			mTimeCrawling += Time.deltaTime;
			if(mTimeCrawling > mTimerCrawlingMax && !IsCrawlOnly)
			{
				mTimeCrawling = 0;
				mSetInitialVal = true;
				mActionSequence = ActionSequence.BRACE;
			}
		}
		else if(mActionSequence == ActionSequence.BRACE)
		{
			if(mSetInitialVal)
			{
				mClickController.mClickCount = 0;
				mClickController.NoOfClick = 10 + mIncrementClick;
				mTimeInput = 3.0f;
				mTypeOfInteraction = QTEScript.TYPE_OF_INTERACTION.BUTTON;
				mSetInitialVal = false;
				mIsCompleted = false;
				GameObject.Find ("Main Camera").GetComponent<Animation>().Play("Daughter Scene 1");
				mPlayerStatusScript.IsPlayingAnimation = true;
			}
			ClickRepeatedly();
		}
		else if(mActionSequence == ActionSequence.FEND_OFF_CLICK)
		{
			if(mSetInitialVal)
			{	
				mClickController.NoOfSequence = 3;
				mTypeOfInteraction = QTEScript.TYPE_OF_INTERACTION.BUTTON;
				mTimeInput = 5.0f;
				mSetInitialVal = false;
				mIsCompleted = false;
				mPlayerStatusScript.IsPlayingAnimation = true;
			}
			ClickInSequence();
		}
		else if(mActionSequence == ActionSequence.FEND_OFF_SWIPE)
		{
			if(mSetInitialVal)
			{
				mTypeOfInteraction = QTEScript.TYPE_OF_INTERACTION.SWIPE;
				mTimeInput = 2.5f;
				mSetInitialVal = false;
				mIsCompleted = false;
				//mPlayerStatusScript.IsPlayingAnimation = true;
				if(!mSwipeController.IsHoldingDown) mSwipeController.IsDisableSwipeRegistration = false;
			}
			
			if(mSwipeNo == 0)
			{
				mSwipeController.SwipeDirection = "West";
				mSwipe = (Texture2D)Resources.Load("gui_swipeLeft");
			}
			else if(mSwipeNo == 1)
			{
				mSwipeController.SwipeDirection = "East";
				mSwipe = (Texture2D)Resources.Load("gui_swipeRight");
			}
			else if(mSwipeNo == 2) 
			{
				mSwipeController.SwipeDirection = "NorthWest";
				mSwipe = (Texture2D)Resources.Load("gui_swipeNorthWest");
			}
			else if(mSwipeNo == 3) 
			{
				mSwipeController.SwipeDirection = "SouthEast";
				mSwipe = (Texture2D)Resources.Load("gui_swipeSouthEast");
			}
			
			SwipeSequence();
		}
	}
	
	void CrawlingSequence()
	{
		mNoOfButton = 2;
		
		if(mClickController.mIsComplete)
		{
			mIsCompleted = true;
			SetButtonToDefault();
		}
		else
		{
			mClickController.ClickType = ClickController.TypeOfClick.CLICK_REPEATEDLY_IN_SEQUENCE;
			mClickController.Region1.x = 0.4f;
			mClickController.Region1.y = 0.8f;
			mClickController.Region1.width = mValueOffSet;
			mClickController.Region1.height = mValueOffSet;
			
			mClickController.Region2.x = 0.5f;
			mClickController.Region2.y = 0.8f;
			mClickController.Region2.width = mValueOffSet;
			mClickController.Region2.height = mValueOffSet;
		}
		
		if(mClickController.IsClicked && mIsGetClickVal)
		{
			mIsMovePlayer = true;
			mIsGetClickVal = false;
			mClickController.IsClicked = false;
		}
		
		if(mIsMovePlayer)
		{
			Player.transform.Translate(Vector3.forward * Time.deltaTime * 2.0f);
			mMovedDistance += Vector3.forward.z * Time.deltaTime * 2.0f;

			if(mMovedDistance >= 0.5f)
			{
				mMovedDistance = 0;
				mIsGetClickVal = true;
				mIsMovePlayer = false;
			}
			else if(mMovedDistance < 0.5f)
			{
				if(mClickController.IsClicked)
				{
					mMovedDistance = 0;
					mClickController.IsClicked = false;
				}
			}
		}
	}
	
	void ClickRepeatedly() // Brace
	{
		mNoOfButton = 1;
		
		if(!mIsCompleted)
		{
			if(mClickController.mIsComplete)
			{
				mIsCompleted = true;
				SetButtonToDefault();
			}
			mClickController.ClickType = ClickController.TypeOfClick.CLICK_REPEATEDLY;
			mClickController.Region1.x = 0.45f;
			mClickController.Region1.y = 0.45f;
			mClickController.Region1.width = mValueOffSet;
			mClickController.Region1.height = mValueOffSet;
		}
		
		mTime += Time.deltaTime;
		if(mTime > mTimeInput)
		{
			if(!mIsCompleted)
			{
				mAudioSource.Play();
				Debug.Log ("take damage");
				mIsDamaged = true;
			}
			SetValueToDefault();
			SetButtonToDefault();
			mIncrementClick += 2;
			
			if(!mIsChangeAction) mActionSequence = ActionSequence.FEND_OFF_CLICK;
			else if(mIsChangeAction) 
			{
				mIsChangeAction = false;
				mActionSequence = ActionSequence.FEND_OFF_SWIPE;
			}
		}
	}
	
	void ClickInSequence() // Fend Off by clicking
	{
		mNoOfButton = 4;
		
		if(!mIsCompleted)
		{
			if(mClickController.mIsComplete)
			{
				mIsCompleted = true;
				SetButtonToDefault();
				if(!mClickController.mIsClickedCorrectly)
				{
					mIsDamaged = true;
					mAudioSource.Play();
				}
			}
			else if(mRedoActionTimes == 0)
			{
				mClickController.ClickType = ClickController.TypeOfClick.CLICK_IN_SEQUENCE;
				
				if(mClickController.SequenceButton <= 3)
				{
					mClickController.Region4.x = 0.65f;
					mClickController.Region4.y = 0.7f;
					mClickController.Region4.width = mValueOffSet;
					mClickController.Region4.height = mValueOffSet;
				}
				
				if(mClickController.SequenceButton <= 2)
				{
					mClickController.Region3.x = 0.25f;
					mClickController.Region3.y = 0.6f;
					mClickController.Region3.width = mValueOffSet;
					mClickController.Region3.height = mValueOffSet;
				}
				
				if(mClickController.SequenceButton <= 1)
				{
					mClickController.Region2.x = 0.6f;
					mClickController.Region2.y = 0.3f;
					mClickController.Region2.width = mValueOffSet;
					mClickController.Region2.height = mValueOffSet;
				}
				
				if(mClickController.SequenceButton <= 0)
				{
					mClickController.Region1.x = 0.2f;
					mClickController.Region1.y = 0.2f;
					mClickController.Region1.width = mValueOffSet;
					mClickController.Region1.height = mValueOffSet;
				}
			}
			else if(mRedoActionTimes == 1)
			{
				if(mClickController.SequenceButton <= 3)
				{
					mClickController.Region4.x = 0.35f;
					mClickController.Region4.y = 0.15f;
					mClickController.Region4.width = mValueOffSet;
					mClickController.Region4.height = mValueOffSet;
				}
				
				if(mClickController.SequenceButton <= 2)
				{
					mClickController.Region3.x = 0.65f;
					mClickController.Region3.y = 0.75f;
					mClickController.Region3.width = mValueOffSet;
					mClickController.Region3.height = mValueOffSet;
				}
				
				if(mClickController.SequenceButton <= 1)
				{
					mClickController.Region2.x = 0.25f;
					mClickController.Region2.y = 0.65f;
					mClickController.Region2.width = mValueOffSet;
					mClickController.Region2.height = mValueOffSet;
				}
				
				if(mClickController.SequenceButton <= 0)
				{
					mClickController.ClickType = ClickController.TypeOfClick.CLICK_IN_SEQUENCE;
					mClickController.Region1.x = 0.7f;
					mClickController.Region1.y = 0.2f;
					mClickController.Region1.width = mValueOffSet;
					mClickController.Region1.height = mValueOffSet;
				}
			}
		}
		mTime += Time.deltaTime;
		if(mTime > mTimeInput)
		{
			if(!mIsCompleted)
			{
				mAudioSource.Play();
				Debug.Log ("take damage");
				mIsDamaged = true;
				mSwipeController.ResetRectValue();
			}
			SetValueToDefault();
			SetButtonToDefault();
			
			if(mRedoActionTimes < 1)
			{
				mActionSequence = ActionSequence.FEND_OFF_CLICK;
				mRedoActionTimes += 1;
				Debug.Log (mRedoActionTimes);
			}
			else if(mRedoActionTimes == 1)
			{
				mActionSequence = ActionSequence.CRAWL;
				mRedoActionTimes = 0;
				GameObject.Find ("YurikoPrefab(Clone)").transform.position = mTriggerScript.SpawnPosition.transform.position;
				AddAnimationSequence("MoveBackScript", 5.5f, false);
				//base.Update();
				//GameObject.Find ("YurikoPrefab(Clone)").GetComponent<MoveBackScript>().mMoveTimer = 0.5f;
				if(!mIsChangeAction) mIsChangeAction = true;
			}
		}
	}
	
	void SwipeSequence()
	{
		if(!mIsCompleted)
		{
			if(mSwipeController.mIsComplete)
			{
				mIsCompleted = true;
				mSwipeController.mIsComplete = false;
				if(!mSwipeController.mIsSwipedCorrectly)
				{
					mIsDamaged = true;
					mAudioSource.Play();
				}
			}
			else
			{
				mSwipeController.EnableSwipeContoller();
			}
		}
		
		mTime += Time.deltaTime;
		if(mTime > mTimeInput)
		{
			if(!mIsCompleted)
			{
				mAudioSource.Play();
				Debug.Log ("take damage");
				mIsDamaged = true;
				mSwipeController.ResetRectValue();
			}
			SetValueToDefault();
			
			if(mSwipeNo < 3)
			{
				mActionSequence = ActionSequence.FEND_OFF_SWIPE;
				mSwipeNo += 1;
				Debug.Log (mSwipeNo);
			}
			else
			{
				mActionSequence = ActionSequence.CRAWL;
				GameObject.Find ("YurikoPrefab(Clone)").transform.position = mTriggerScript.SpawnPosition.transform.position;
				AddAnimationSequence("MoveBackScript", 5.5f, false);
				//base.Update();
				//GameObject.Find ("YurikoPrefab(Clone)").GetComponent<MoveBackScript>().mMoveTimer = 10.0f;
				mSwipeNo = 0;
			}
		}
	}
	
	void SetValueToDefault()
	{
		mTime = 0.0f;
		mIsCompleted = true;
		mSetInitialVal = true;
	}
	
	void SetButtonToDefault()
	{
		mClickController.mIsComplete = false;
		mClickController.SequenceButton = 0;
		mClickController.ResetClickCount();
		mClickController.ResetAllRectValue ();
	}
	
	void OnGUI()
	{
		if(!mIsCompleted)
		{
			if(mTypeOfInteraction == QTEScript.TYPE_OF_INTERACTION.BUTTON)
			{
				if(mNoOfButton == 1)
				{
					GUI.Button(new Rect(mClickController.Region1.x * Screen.width, mClickController.Region1.y * Screen.height, (mValueOffSet * Screen.width), (mValueOffSet * Screen.height)), mButton, MyStyle);
				}
				else if(mNoOfButton == 2)
				{
					GUI.Button(new Rect(mClickController.Region1.x * Screen.width, mClickController.Region1.y * Screen.height, (mValueOffSet * Screen.width), (mValueOffSet * Screen.height)), mButton, MyStyle);
					GUI.Button(new Rect(mClickController.Region2.x * Screen.width, mClickController.Region2.y * Screen.height, (mValueOffSet * Screen.width), (mValueOffSet * Screen.height)), mButton, MyStyle);
				}
				else if(mNoOfButton == 4)
				{
					//Debug.Log (mClickController.SequenceButton);
					if(mClickController.SequenceButton <= 3) GUI.Button(new Rect(mClickController.Region4.x * Screen.width, mClickController.Region4.y * Screen.height, (mValueOffSet * Screen.width), (mValueOffSet * Screen.height)), mButtonSequence4, MyStyle);
					if(mClickController.SequenceButton <= 2) GUI.Button(new Rect(mClickController.Region3.x * Screen.width, mClickController.Region3.y * Screen.height, (mValueOffSet * Screen.width), (mValueOffSet * Screen.height)), mButtonSequence3, MyStyle);
					if(mClickController.SequenceButton <= 1) GUI.Button(new Rect(mClickController.Region2.x * Screen.width, mClickController.Region2.y * Screen.height, (mValueOffSet * Screen.width), (mValueOffSet * Screen.height)), mButtonSequence2, MyStyle);
					if(mClickController.SequenceButton <= 0) GUI.Button(new Rect(mClickController.Region1.x * Screen.width, mClickController.Region1.y * Screen.height, (mValueOffSet * Screen.width), (mValueOffSet * Screen.height)), mButtonSequence1, MyStyle);
				}
			}
			else if(mTypeOfInteraction == QTEScript.TYPE_OF_INTERACTION.SWIPE)
			{
				if(mSwipeNo == 0) GUI.Button(new Rect(0.15f * Screen.width, 0.45f * Screen.height - (0.2f * mSwipe.height/2), (0.3f * Screen.width), (0.2f * Screen.height)), mSwipe, MyStyle);
				else if(mSwipeNo == 1) GUI.Button(new Rect(0.65f * Screen.width, 0.45f * Screen.height - (0.2f * mSwipe.height/2), (0.3f * Screen.width), (0.2f * Screen.height)), mSwipe, MyStyle);
				else if(mSwipeNo == 2) GUI.Button(new Rect(0.15f * Screen.width, 0.25f * Screen.height - (0.2f * mSwipe.height/2), (0.3f * Screen.width), (0.2f * Screen.height)), mSwipe, MyStyle);
				else if(mSwipeNo == 3) GUI.Button(new Rect(0.65f * Screen.width, 0.75f * Screen.height - (0.2f * mSwipe.height/2), (0.3f * Screen.width), (0.2f * Screen.height)), mSwipe, MyStyle);
			}
		}
		
		if(mIsDamaged) // Pop-up the image
		{
			GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), mTriggerScript.ScaryImage);
			mImageTime += Time.deltaTime;
			
			if(!mHasTakenDamage)
			{
				int temp = (int)mPlayerStatusScript.mHealth;
				if(temp != 0)
				{
					temp -= 1;
					mPlayerStatusScript.mHealth = (PlayerStatusScript.HEALTH_SCRIPT)temp;
					mHasTakenDamage = true;
				}
			}
			
			if(mImageTime > mImageTimeMax)
			{
				mImageTime = 0;
				mIsDamaged = false;
				mHasTakenDamage = false;
				if(mPlayerStatusScript.mHealth == PlayerStatusScript.HEALTH_SCRIPT.DEATH)
				{
					Destroy(gameObjectPrefab.GetComponent<QTEDaughterScript>());
				}
			}
		}
		
	}
}
