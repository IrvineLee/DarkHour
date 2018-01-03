using UnityEngine;
using System.Collections;

public class QTEFatherScript : QTEScript 
{
	AudioSource mAudioSource;
	//public Texture mScaryImage;
	//GUIStyle mButton;
	public GUIStyle MyStyle;
	public Texture2D mButton;
	public Texture2D mButtonPressed;
	public Texture2D mSwipe;
	
	public enum ActionSequence
	{
		NONE = 0,
		STEP_BACK_SLOWLY,
		TURN_AROUND,
		RUN_AWAY,
		PUSH_BACK
	}
	public ActionSequence mActionSequence = ActionSequence.STEP_BACK_SLOWLY;
	public ActionSequence mNextActionSequence;
	
	private float mTime = 0.0f;
	private float mTimeIdle = 0.0f;
	private float mTimeInput = 3.0f;
	private float mImageTime = 0.0f;
	private float mImageTimeMax = 0.5f;
	
	private float mValueX = 0.45f;
	private float mValueY = 0.45f;
	private float mValueOffSet = 0.1f;
	
	bool mIsCompleted = false;
	bool mSetInitialVal = true;
	bool mIsDamaged = false;
	
	PlayerStatusScript mPlayerStatusScript;
	TriggerScript mTriggerScript;
	MoveScript mMoveScript;
	ClickController mClickController;
	SwipeController mSwipeController;
	
	void Start () 
	{	
		gameObjectPrefab = GameObject.FindGameObjectWithTag("Father");
		mPlayerStatusScript = GameObject.Find("Player").GetComponent<PlayerStatusScript>();
		mClickController = GameObject.Find("Player").GetComponent<ClickController>();
		mSwipeController = GameObject.Find("Player").GetComponent<SwipeController>();
		mTriggerScript = GameObject.Find ("TriggerFather").GetComponent<TriggerScript>();
		AddAnimationSequence("MoveScript", mTimeIdle, false);
		mAudioSource = GameObject.Find ("ScreamSound").GetComponent<AudioSource>();
		
		
		mButton = (Texture2D)Resources.Load("btn_press");
		mButtonPressed = (Texture2D)Resources.Load("gui_press");
		mSwipe = (Texture2D)Resources.Load("gui_swipeRight");
		
		mTime = 0.0f;
		mTimeIdle = 0.0f;
		mTimeInput = 3.0f;
		mImageTime = 0.0f;
		mImageTimeMax = 0.5f;
		
		mValueX = 0.45f;
		mValueY = 0.45f;
		mValueOffSet = 0.1f;
		
		mIsCompleted = false;
		mSetInitialVal = true;
		mIsDamaged = false;
		//AddAnimationSequence("AttackScript", 2.0f, false);
		MyStyle = new GUIStyle();
	}
	
	protected override void Update () 
	{
		base.Update();
		
		mTime += Time.deltaTime;
		if(mActionSequence == ActionSequence.STEP_BACK_SLOWLY)
		{
			if(mSetInitialVal)
			{
				mTypeOfInteraction = QTEScript.TYPE_OF_INTERACTION.BUTTON;
				mSetInitialVal = false;
				mTimeInput = 6.5f;
				mClickController.NoOfClick = 8;
				mClickController.IsAnimationScene = true;
				mNextActionSequence = ActionSequence.TURN_AROUND;
			}
			RunningSequence();
		}
		else if(mActionSequence == ActionSequence.TURN_AROUND)
		{
			if(mSetInitialVal)
			{
				mIsCompleted = false;
				mTypeOfInteraction = QTEScript.TYPE_OF_INTERACTION.SWIPE;
				mSetInitialVal = false;
				mTimeInput = 2.0f;
				mSwipeController.SwipeDirection = "East";
				mNextActionSequence = ActionSequence.RUN_AWAY;
			}
			SwipeSequence();
		}
		else if(mActionSequence == ActionSequence.RUN_AWAY)
		{
			if(mSetInitialVal)
			{
				gameObjectPrefab.GetComponent<MoveScript>().mMoveSpeed = 2.4f;
				mIsCompleted = false;
				mTypeOfInteraction = QTEScript.TYPE_OF_INTERACTION.BUTTON;
				mSetInitialVal = false;
				mTimeInput = 4.5f;
				mClickController.NoOfClick = 10;
				mNextActionSequence = ActionSequence.PUSH_BACK;
			}
			RunningSequence();
		}
		else if(mActionSequence == ActionSequence.PUSH_BACK)
		{
			if(mSetInitialVal)
			{
				//Debug.Log ("SDSDSDS");
				//AddAnimationSequence("AttackScript", mTimeIdle, false);
				
				Vector3 temp = gameObjectPrefab.transform.position;
				Destroy (gameObjectPrefab);
				GameObject gameObject = (GameObject)Instantiate(Resources.Load ("FatherBite"), temp, Quaternion.identity);
				mIsCompleted = false;
				mTypeOfInteraction = QTEScript.TYPE_OF_INTERACTION.BUTTON;
				mSetInitialVal = false;
				mTimeInput = 4.5f;
				mClickController.NoOfClick = 7;
				mNextActionSequence = ActionSequence.NONE;
			}
			ClickRepeatedlySequence();
		}
	}
	
	void RunningSequence()
	{
		mNoOfButton = 2;
		
		if(!mIsCompleted)
		{
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
		}
		
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
			mActionSequence = mNextActionSequence;
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
					mAudioSource.GetComponent<AudioSource>().Play();
					mIsDamaged = true;
				}
			}
			else
			{
				mSwipeController.EnableSwipeContoller();
			}
		}
		
		if(mTime > mTimeInput)
		{
			if(!mIsCompleted)
			{
				mAudioSource.GetComponent<AudioSource>().Play();
				Debug.Log ("take damage");
				mIsDamaged = true;
				mSwipeController.ResetRectValue();
			}
			SetValueToDefault();
			mActionSequence = mNextActionSequence;
		}
	}
	
	void ClickRepeatedlySequence()
	{
		mNoOfButton = 1;
		
		if(!mIsCompleted)
		{
			if(mClickController.mIsComplete)
			{
				mIsCompleted = true;
				mClickController.mIsComplete = false;
			}
			mClickController.ClickType = ClickController.TypeOfClick.CLICK_REPEATEDLY;
			mClickController.Region1.x = 0.45f;
			mClickController.Region1.y = 0.45f;
			mClickController.Region1.width = mValueOffSet;
			mClickController.Region1.height = mValueOffSet;
		}
		
		if(mTime > mTimeInput)
		{
			if(!mIsCompleted)
			{
				mAudioSource.GetComponent<AudioSource>().Play();
				Debug.Log ("take damage");
				mIsDamaged = true;
			}
			SetValueToDefault();
			mActionSequence = mNextActionSequence;
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
			}
			else if(mTypeOfInteraction == QTEScript.TYPE_OF_INTERACTION.SWIPE)
			{
				GUI.Button(new Rect(0.65f * Screen.width, 0.45f * Screen.height - (0.2f * mSwipe.height/2), (Screen.width), (0.2f * Screen.height)), mSwipe, MyStyle);
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
					Destroy(GameObject.FindGameObjectWithTag("Father").GetComponent<QTEFatherScript>());
				}
			}
		}
	}
}
