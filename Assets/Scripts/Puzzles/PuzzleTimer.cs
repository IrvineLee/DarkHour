using UnityEngine;
using System.Collections;

public class PuzzleTimer : MonoBehaviour 
{
	public bool IsEnabled = false;
	public float CountdownTimer = 45.0f;
	public Rect TimerDisplay;
	public Rect Region;
	
	private float mTime;
	
	[HideInInspector]
	public bool TimeIsUp = false;
	
	SwapDollPuzzleController mSwapDollPuzzleController;
	
	void Start()
	{
	 	mTime = CountdownTimer;
		mSwapDollPuzzleController = GameObject.Find ("DollPuzzle").GetComponent<SwapDollPuzzleController>();
	}
	
	void Update () 
	{
		if(IsEnabled)
		{
			if (mTime >= 0 && !mSwapDollPuzzleController.mIsComplete) 
			{
				mTime -= Time.deltaTime;
				if(mTime <= 0)
				{
					Debug.Log ("Time's Up");
					mTime = CountdownTimer;
					IsEnabled = false;
					TimeIsUp = true;
				}
			}
		}
	}
	
	void OnGUI()
	{
		if(IsEnabled)
		{
			if(GUI.Button (new Rect(Region.x * Screen.width, Region.y * Screen.height, Region.width*Screen.width, Region.height*Screen.height), "Exit"))
			{
				mTime = 0;
			}
			
			GUI.Button (new Rect(TimerDisplay.x * Screen.width, 
				TimerDisplay.y * Screen.height, 
				TimerDisplay.width * Screen.width, 
				TimerDisplay.height * Screen.height), 
				"Time: " + Mathf.RoundToInt(mTime).ToString());
		}
	}
}
