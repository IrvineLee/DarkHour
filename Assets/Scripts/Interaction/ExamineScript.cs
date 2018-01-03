using UnityEngine;
using System.Collections;

public class ExamineScript : MonoBehaviour 
{
	public Rect Region;
	
	public string Message1;
	public string Message2;
	public string Message3;
	public string Message4;
	public string Message5;
	
	public string TriggerMessage1;
	public string TriggerMessage2;
	public string TriggerMessage3;
	public string TriggerMessage4;
	public string TriggerMessage5;
	
	public string CompletedMessage;
	public string FailedMessage;
	public string EndMessage;
	
	public enum TYPE_OF_MESSAGE
	{
		NORMAL = 0,
		TRIGGER,
		COMPLETED,
		FAILED,
		END
	}
	public TYPE_OF_MESSAGE mTypeOfMessage = TYPE_OF_MESSAGE.NORMAL;
	
	public enum TRIGGER_PUZZLE
	{
		NONE = 0,
		DOLL,
		SLIDE,
		ROTATE_COLOR
	}
	public TRIGGER_PUZZLE mTriggerPuzzle = TRIGGER_PUZZLE.NONE;
	
	public enum MESSAGE_DISPLAY
	{
		ONE = 0,
		TWO,
		THREE,
		FOUR,
		FIVE
	}
	
	public enum TRIGGER_MESSAGE_DISPLAY
	{
		ONE = 0,
		TWO,
		THREE,
		FOUR,
		FIVE
	}
	
	private string mMessage;
	private int NoOfMsg = 1;
	private int NoOfTrigMsg = 1;
	
	bool mIsDisplayed = false;
	
	GameObject ExamineFlaslight;
	
	CameraRayScript mCameraRayScript;
	PlayerController mPlayerController;
	SwapDollPuzzleController mSwapDollPuzzleController;
	SlidePuzzleScript mSlidePuzzleScript;
	RotatePuzzleScript mRotatePuzzleScript;
	LookAtScript mLookAtScript;
	
	[HideInInspector]
	public bool EnabledExamine = false;
	[HideInInspector]
	public MESSAGE_DISPLAY mMessageDisplay = MESSAGE_DISPLAY.ONE;
	[HideInInspector]
	public TRIGGER_MESSAGE_DISPLAY mTriggerMessageDisplay = TRIGGER_MESSAGE_DISPLAY.ONE;
	
	void Start () 
	{	
		mCameraRayScript = GameObject.Find("Main Camera").GetComponent<CameraRayScript>();
		mPlayerController = GameObject.Find("Player").GetComponent<PlayerController>();
		mLookAtScript = GameObject.Find("TriggerLookAt(F)").GetComponent<LookAtScript>();
		ExamineFlaslight = GameObject.Find ("ExaminePlane(Flashlight)");
			
		if(mTriggerPuzzle == TRIGGER_PUZZLE.DOLL)
		{
			mSwapDollPuzzleController = GameObject.Find("DollPuzzle").GetComponent<SwapDollPuzzleController>();
		}
		else if(mTriggerPuzzle == TRIGGER_PUZZLE.SLIDE)
		{
			mSlidePuzzleScript = GameObject.Find("SlidePuzzle").GetComponent<SlidePuzzleScript>();
		}
		else if(mTriggerPuzzle == TRIGGER_PUZZLE.ROTATE_COLOR)
		{
			mRotatePuzzleScript = GameObject.Find ("RotatePuzzle").GetComponent<RotatePuzzleScript>();	
		}
	}
	
	void CheckNoOfMessage()
	{
		if(Message2 != "") NoOfMsg = 2;
		if(Message3 != "") NoOfMsg = 3;
		if(Message4 != "") NoOfMsg = 4;
		if(Message5 != "") NoOfMsg = 5;
	}
	
	void CheckNoOfTriggerMessage()
	{
		if(TriggerMessage2 != "") NoOfTrigMsg = 2;
		if(TriggerMessage3 != "") NoOfTrigMsg = 3;
		if(TriggerMessage4 != "") NoOfTrigMsg = 4;
		if(TriggerMessage5 != "") NoOfTrigMsg = 5;
	}
	
	void Update () 
	{
		if(EnabledExamine) 
		{
			CheckNoOfMessage();
			CheckNoOfTriggerMessage();

			if(mTypeOfMessage == TYPE_OF_MESSAGE.NORMAL) mMessage = Message1;
			else if(mTypeOfMessage == TYPE_OF_MESSAGE.TRIGGER) mMessage = TriggerMessage1;
			else if(mTypeOfMessage == TYPE_OF_MESSAGE.COMPLETED) mMessage = CompletedMessage;
			else if(mTypeOfMessage == TYPE_OF_MESSAGE.END) mMessage = EndMessage;
			else if(mTypeOfMessage == TYPE_OF_MESSAGE.FAILED) mMessage = FailedMessage;
	
			if(Message2 != "") mMessageDisplay = MESSAGE_DISPLAY.TWO;
			if(TriggerMessage2 != "") mTriggerMessageDisplay = TRIGGER_MESSAGE_DISPLAY.TWO;
			
			mIsDisplayed = true;
			EnabledExamine = false;
			mCameraRayScript.DisableRayCast = true;
			mPlayerController.DisablePlayerController = true;
		}
	}
	
	void OnGUI()
	{
		if(mIsDisplayed)
		{
			if(mTypeOfMessage == TYPE_OF_MESSAGE.NORMAL) // Display normal message
			{
				if(GUI.Button(new Rect(Region.x * Screen.width, 
				Region.y * Screen.height, 
				Region.width * Screen.width, 
				Region.height * Screen.height), mMessage))
				{
					NoOfMsg -= 1;
					if(mMessageDisplay == MESSAGE_DISPLAY.TWO)
					{
						mMessage = Message2;
						mMessageDisplay = MESSAGE_DISPLAY.THREE;
					}
					else if(mMessageDisplay == MESSAGE_DISPLAY.THREE)
					{
						mMessage = Message3;
						mMessageDisplay = MESSAGE_DISPLAY.FOUR;
					}
					else if(mMessageDisplay == MESSAGE_DISPLAY.FOUR)
					{
						mMessage = Message4;
						mMessageDisplay = MESSAGE_DISPLAY.FIVE;
					}
					else if(mMessageDisplay == MESSAGE_DISPLAY.FIVE)
					{
						mMessage = Message5;
					}

					if(NoOfMsg == 0)
					{
						if(Message2 != "")
						{
							mMessageDisplay = MESSAGE_DISPLAY.TWO;
						}
						NoOfMsg = 1;
						mIsDisplayed = false;
						mCameraRayScript.DisableRayCast = false;
						mPlayerController.DisablePlayerController = false;
						
						if(mTriggerPuzzle == TRIGGER_PUZZLE.SLIDE)
						{
							mSlidePuzzleScript.IsEnabled = true;
							mCameraRayScript.DisableRayCast = true;
							mPlayerController.DisablePlayerController = true;
						}
						else if(mTriggerPuzzle == TRIGGER_PUZZLE.ROTATE_COLOR)
						{
							mRotatePuzzleScript.IsEnabled = true;
							mCameraRayScript.DisableRayCast = true;
							mPlayerController.DisablePlayerController = true;
						}
						
						if(gameObject == ExamineFlaslight)
						{
							if(mLookAtScript.IsTriggered == true)
							{
								Debug.Log ("wuwuwuw");
								mLookAtScript.IsEnabled = true;
							}
							else
							{
								Destroy (GameObject.Find ("TriggerLookAt(F)"));
								Destroy (GameObject.Find ("Flashlight Scene"));
								GameObject.FindGameObjectWithTag ("Spotlight").GetComponent<Light>().intensity = 1.0f;
							}
						}
					}
				}
			}
			else if(mTypeOfMessage == TYPE_OF_MESSAGE.TRIGGER) // Display triggered message
			{
				if(GUI.Button(new Rect(Region.x * Screen.width, 
				Region.y * Screen.height, 
				Region.width * Screen.width, 
				Region.height * Screen.height), mMessage))
				{
					NoOfTrigMsg -= 1;
					if(mTriggerMessageDisplay == TRIGGER_MESSAGE_DISPLAY.TWO)
					{
						mMessage = TriggerMessage2;
						mTriggerMessageDisplay = TRIGGER_MESSAGE_DISPLAY.THREE;
					}
					else if(mTriggerMessageDisplay == TRIGGER_MESSAGE_DISPLAY.THREE)
					{
						mMessage = TriggerMessage3;
						mTriggerMessageDisplay = TRIGGER_MESSAGE_DISPLAY.FOUR;
					}
					else if(mTriggerMessageDisplay == TRIGGER_MESSAGE_DISPLAY.FOUR)
					{
						mMessage = TriggerMessage4;
						mTriggerMessageDisplay = TRIGGER_MESSAGE_DISPLAY.FIVE;
					}
					else if(mTriggerMessageDisplay == TRIGGER_MESSAGE_DISPLAY.FIVE)
					{
						mMessage = TriggerMessage5;
					}

					if(NoOfTrigMsg == 0)
					{
						if(TriggerMessage2 != "")
						{
							mTriggerMessageDisplay = TRIGGER_MESSAGE_DISPLAY.TWO;
						}
						NoOfTrigMsg = 1;
						mIsDisplayed = false;
						mCameraRayScript.DisableRayCast = false;
						mPlayerController.DisablePlayerController = false;
						
						if(mTriggerPuzzle == TRIGGER_PUZZLE.DOLL)
						{
							mSwapDollPuzzleController.IsEnabled = true;
							mCameraRayScript.DisableRayCast = true;
							mPlayerController.DisablePlayerController = true;
						}
						else if(mTriggerPuzzle == TRIGGER_PUZZLE.SLIDE)
						{
							mSlidePuzzleScript.IsEnabled = true;
							mCameraRayScript.DisableRayCast = true;
							mPlayerController.DisablePlayerController = true;
						}
						else if(mTriggerPuzzle == TRIGGER_PUZZLE.ROTATE_COLOR)
						{
							mRotatePuzzleScript.IsEnabled = true;
							mCameraRayScript.DisableRayCast = true;
							mPlayerController.DisablePlayerController = true;
						}
						
						if(gameObject == GameObject.Find ("/Floor 1/WesternDoor3/GameObject/Plane009/Right"))
						{
							mTypeOfMessage = TYPE_OF_MESSAGE.END;
							GameObject.Find ("/Father Scene 1/TriggerFather").GetComponent<TriggerScript>().IsEnabled = true;
						}
					}
				}
			}
			else if(mTypeOfMessage == TYPE_OF_MESSAGE.COMPLETED) // Display completed message
			{
				if(GUI.Button(new Rect(Region.x * Screen.width, 
				Region.y * Screen.height, 
				Region.width * Screen.width, 
				Region.height * Screen.height), mMessage))
				{
					mIsDisplayed = false;
					mCameraRayScript.DisableRayCast = false;
					mPlayerController.DisablePlayerController = false;
					mTypeOfMessage = TYPE_OF_MESSAGE.END;
					
					if(mTriggerPuzzle == TRIGGER_PUZZLE.SLIDE || mTriggerPuzzle == TRIGGER_PUZZLE.ROTATE_COLOR)
					{
						gameObject.tag = "OpenChestBox";
					}
				}
			}
			else if(mTypeOfMessage == TYPE_OF_MESSAGE.END) // Display end message
			{
				if(GUI.Button(new Rect(Region.x * Screen.width, 
				Region.y * Screen.height, 
				Region.width * Screen.width, 
				Region.height * Screen.height), mMessage))
				{
					mIsDisplayed = false;
					mCameraRayScript.DisableRayCast = false;
					mPlayerController.DisablePlayerController = false;
				}
			}
			else if(mTypeOfMessage == TYPE_OF_MESSAGE.FAILED) // Display failed message
			{
				if(GUI.Button(new Rect(Region.x * Screen.width, 
				Region.y * Screen.height, 
				Region.width * Screen.width, 
				Region.height * Screen.height), mMessage))
				{
					mIsDisplayed = false;
					mCameraRayScript.DisableRayCast = false;
					mPlayerController.DisablePlayerController = false;
					mTypeOfMessage = TYPE_OF_MESSAGE.TRIGGER;
				}
			}
			
		}
	}
}
