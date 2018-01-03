using UnityEngine;
using System.Collections;

public class PlayerStatusScript : MonoBehaviour 
{
	public GUIStyle MyStyle;
	
	public Rect FaceRegion;
	public Rect HealthRegion;
	public Rect OptionRegion;
	public Rect BookRegion;
	
	public Texture Face;
	public Texture Book;
	public Texture Quit;
	public Texture Credits;
	
	public Texture PuzzlePic1;
	public Texture PuzzlePicFull;
	
	public Texture2D mHealthTexture;
	public object[] mHealthTexture1;
	public object[] mHealthTexture2;
	public object[] mHealthTexture3;
	public object[] mHealthTexture4;
	public object[] mHealthTexture5;
	
	public enum HEALTH_SCRIPT
	{
		DEATH = 0,
		DISTRESS,
		TERRIFIED,
		ANXIETY,
		DREAD,
		DISTURBED
	}
	public HEALTH_SCRIPT mHealth = HEALTH_SCRIPT.DISTURBED;
	
	public AudioClip AudioHeartBeat;
	public AudioClip AudioEnd;
		
	public int Notes = 0;
	int mHealthIndex;
	float mTime;
	float mTimerMax = 8.0f;
	float mTimeHealth;
	float mTimeHealthMax = 0.08f;
	float mTimeCredit;
	float mTimerMaxCredit = 0.1f;
	float mCreditAlpha;
	
	public bool IsQTEScene = false;
	public bool IsPlayingAnimation = false;
	public bool IsDisableBook = false;
	public bool mSetIntialVal = true;
	public bool IsEnd = false;
	bool IsDisableQuit = false;
	bool IsGameOver = false;
	bool mIsStart = true;
	bool mIsDead = false;
	bool mIsHealthFine = false;
	bool mIsBook = false;
	bool mHasTriggered = false;
	bool mShowCredits = true;
	bool mReset = true;
	
	GameObject MainCamera;
	GameObject FlashLight;
	GameObject BGMSound;
	
	PlayerController mPlayerController;
	CameraRayScript mCameraRayScript;
	
	void Start () 
	{
		MainCamera = GameObject.Find ("Main Camera");
		FlashLight = GameObject.Find ("Spotlight");
		BGMSound = GameObject.Find ("BGMSound");
		
		mPlayerController = GameObject.Find ("Player").GetComponent<PlayerController>();
		mCameraRayScript = MainCamera.GetComponent<CameraRayScript>();
		
		mHealthTexture1 = Resources.LoadAll ("heart_died", typeof(Texture2D));
		mHealthTexture2 = Resources.LoadAll ("heart_01", typeof(Texture2D));
		mHealthTexture3 = Resources.LoadAll ("heart_02", typeof(Texture2D));
		mHealthTexture4 = Resources.LoadAll ("heart_03", typeof(Texture2D));
		mHealthTexture5 = Resources.LoadAll ("heart_04", typeof(Texture2D));
		
		mHealthTexture = (Texture2D)mHealthTexture5[0];
		MyStyle = new GUIStyle();
	}
	
	void OnDestroy()
	{
		IsPlayingAnimation = false;
		IsDisableBook = false;
		mSetIntialVal = true;
		IsEnd = false;
		IsGameOver = false;
		mIsStart = true;
		mIsDead = false;
		mIsHealthFine = false;
		mIsBook = false;
		mHasTriggered = false;
		BGMSound.GetComponent<AudioSource>().loop = true;
		mReset = true;
		IsQTEScene = false;
	}
	
	void Update () 
	{
		if(Notes == 2 && !mHasTriggered)
		{
			GameObject.Find ("ExaminePlane(D)").GetComponent<ExamineScript>().mTypeOfMessage = ExamineScript.TYPE_OF_MESSAGE.TRIGGER;
			mHasTriggered = true;
		}
		
		if(mIsStart)
		{
			mPlayerController.DisablePlayerController = true;
			mCameraRayScript.DisableRayCast = true;
			
			if(!MainCamera.GetComponent<Animation>().IsPlaying("WakeUp Animation"))
			{
				mPlayerController.DisablePlayerController = false;
				mCameraRayScript.DisableRayCast = false;
				mIsStart = false;
			}
		}
		
		if(mIsDead && !IsGameOver && !MainCamera.GetComponent<Animation>().IsPlaying("Die Animation(Crawl)") && !MainCamera.GetComponent<Animation>().IsPlaying("Die Animation"))
		{
			Debug.Log ("sasa");
			//MainCamera.animation["ShakeAnimation"].blendMode = AnimationBlendMode.Blend;
			MainCamera.GetComponent<Animation>().Stop ("ShakeAnimation");
			MainCamera.GetComponent<AudioSource>().Stop ();
			BGMSound.GetComponent<AudioSource>().Stop();
			playSound(AudioEnd);
			//BGMSound.audio.PlayOneShot (AudioEnd);
			IsGameOver = true;
		}
		
		if (!IsPlayingAnimation && !mIsDead && !IsEnd && !IsQTEScene)
		{
			int temp = (int)mHealth;
			if(temp < 5)
			{
				mTime += Time.deltaTime;
				if(mTime > mTimerMax)
				{
					temp += 1;
					mHealth = (PlayerStatusScript.HEALTH_SCRIPT)temp;
					mTime = 0;
					mTimerMax += 5.0f;
				}
			}
		}
		
		if(mHealth == HEALTH_SCRIPT.DEATH)
		{
			if(mReset)
			{
				mHealthIndex = 0;
				mTimeHealth = 0;
				mReset = false;
			}
			
			mTimeHealth += Time.deltaTime;
			if(mTimeHealth > mTimeHealthMax)
			{
				mHealthIndex += 1;
				if(mHealthIndex > mHealthTexture1.Length - 1)
				{
					mHealthIndex = 0;
				}
				mHealthTexture = (Texture2D)mHealthTexture1[mHealthIndex];
				mTimeHealth = 0;
			}
		}
		
		if(!mIsDead && mHealth == HEALTH_SCRIPT.DEATH)
		{
			MainCamera.GetComponent<Animation>().Stop ();
			FlashLight.GetComponent<Animation>().Stop ();
			MainCamera.GetComponent<Animation>()["ShakeAnimation"].blendMode = AnimationBlendMode.Additive;
			if(!IsEnd)
			{
				MainCamera.GetComponent<Animation>().Play("ShakeAnimation"); 
				MainCamera.GetComponent<Animation>().Play ("Die Animation(Crawl)");
			}
			else if(IsEnd)
			{
				MainCamera.GetComponent<Animation>().Play("ShakeAnimation"); 
				MainCamera.GetComponent<Animation>().Play ("Die Animation");
				FlashLight.GetComponent<Animation>().Play ("Die Animation");
			}
			mPlayerController.DisablePlayerController = true;
			mCameraRayScript.DisableRayCast = true;
			IsDisableQuit = true;
			IsDisableBook = true;
			mIsDead = true;
		}
		else if(mHealth == HEALTH_SCRIPT.ANXIETY || mHealth == HEALTH_SCRIPT.TERRIFIED || mHealth == HEALTH_SCRIPT.DISTRESS)
		{
			if(mHealth == HEALTH_SCRIPT.ANXIETY)
			{
				mTimeHealth += Time.deltaTime;
				if(mTimeHealth > mTimeHealthMax)
				{
					mHealthIndex += 1;
					if(mHealthIndex > mHealthTexture4.Length - 1)
					{
						mHealthIndex = 0;
					}
					mHealthTexture = (Texture2D)mHealthTexture4[mHealthIndex];
					mTimeHealth = 0;
				}
			}
			else if(mHealth == HEALTH_SCRIPT.TERRIFIED)
			{
				mTimeHealth += Time.deltaTime;
				if(mTimeHealth > mTimeHealthMax)
				{
					mHealthIndex += 1;
					if(mHealthIndex > mHealthTexture3.Length - 1)
					{
						mHealthIndex = 0;
					}
					mHealthTexture = (Texture2D)mHealthTexture3[mHealthIndex];
					mTimeHealth = 0;
				}
			}
			else if(mHealth == HEALTH_SCRIPT.DISTRESS)
			{
				mTimeHealth += Time.deltaTime;
				if(mTimeHealth > mTimeHealthMax)
				{
					mHealthIndex += 1;
					if(mHealthIndex > mHealthTexture2.Length - 1)
					{
						mHealthIndex = 0;
					}
					mHealthTexture = (Texture2D)mHealthTexture2[mHealthIndex];
					mTimeHealth = 0;
				}
			}
			if(mSetIntialVal)
			{
				mHealthIndex = 0;
				mTimeHealth = 0;
				MainCamera.GetComponent<AudioSource>().loop = true;
				MainCamera.GetComponent<AudioSource>().clip = AudioHeartBeat;
				MainCamera.GetComponent<AudioSource>().Play();
				MainCamera.GetComponent<Animation>()["ShakeAnimation"].layer = 1; 
				MainCamera.GetComponent<Animation>().Play("ShakeAnimation"); 
				MainCamera.GetComponent<Animation>()["ShakeAnimation"].weight = 1.0f;
				mIsHealthFine = false;
				mSetIntialVal = false;
			}
			
			if(IsPlayingAnimation)
			{
				MainCamera.GetComponent<Animation>()["ShakeAnimation"].blendMode = AnimationBlendMode.Additive;
			}
			else if (!IsPlayingAnimation)
			{
				MainCamera.GetComponent<Animation>()["ShakeAnimation"].blendMode = AnimationBlendMode.Blend;
			}
		}
		else if(mHealth == HEALTH_SCRIPT.DREAD || mHealth == HEALTH_SCRIPT.DISTURBED)
		{
			mTimeHealth += Time.deltaTime;
			if(mTimeHealth > mTimeHealthMax)
			{
				mHealthIndex += 1;
				if(mHealthIndex > mHealthTexture5.Length - 1)
				{
					mHealthIndex = 0;
				}
				mHealthTexture = (Texture2D)mHealthTexture5[mHealthIndex];
				mTimeHealth = 0;
			}
			if(!mIsHealthFine)
			{
				mHealthIndex = 0;
				mTimeHealth = 0;
				MainCamera.GetComponent<AudioSource>().loop = false;
				MainCamera.GetComponent<AudioSource>().Stop ();
				MainCamera.GetComponent<Animation>().Stop ("ShakeAnimation");
				mIsHealthFine = true;
				mSetIntialVal = true;
			}
		}
	}
	
	void OnGUI()
	{
		GUI.Button(new Rect(HealthRegion.x * Screen.width, HealthRegion.y * Screen.height, HealthRegion.width * Screen.width, HealthRegion.height * Screen.height), mHealthTexture);
		GUI.Button(new Rect(FaceRegion.x * Screen.width, FaceRegion.y * Screen.height, FaceRegion.width * Screen.width, FaceRegion.height * Screen.height),"");
		if(GUI.Button(new Rect(BookRegion.x * Screen.width, BookRegion.y * Screen.height, BookRegion.width * Screen.width, BookRegion.height * Screen.height),Book, MyStyle) && !IsDisableBook)
		{
			if(!mIsBook)
			{
				mIsBook = true;
				mPlayerController.DisablePlayerController = true;
				mCameraRayScript.DisableRayCast = true;
			}
			else if(mIsBook)
			{
				mIsBook = false;
				mPlayerController.DisablePlayerController = false;
				mCameraRayScript.DisableRayCast = false;
			}
			
		}
		else if(GUI.Button(new Rect(OptionRegion.x * Screen.width, OptionRegion.y * Screen.height, OptionRegion.width * Screen.width, OptionRegion.height * Screen.height),Quit, MyStyle) && !IsDisableQuit)
		{
			//Time.timeScale = 0;
			AutoFade.LoadLevel ("MainMenu",1,1,Color.black);
		}
		
		if(mIsBook)
		{
			if(Notes == 0) GUI.Button(new Rect(0.2f*Screen.width, 0.2f*Screen.height, 0.6f*Screen.width, 0.6f*Screen.height), "Empty");
			else if(Notes == 1)GUI.DrawTexture(new Rect(0.2f*Screen.width, 0.2f*Screen.height, 0.3f*Screen.width, 0.6f*Screen.height), PuzzlePic1);
			else if(Notes == 2)GUI.DrawTexture(new Rect(0.2f*Screen.width, 0.2f*Screen.height, 0.6f*Screen.width, 0.6f*Screen.height), PuzzlePicFull);
		}
		
		if(IsGameOver)
		{
			if(mShowCredits)
			{
				Color initialColor = GUI.color;
				initialColor.a = mCreditAlpha;
				GUI.color = initialColor;
				
				mTimeCredit += Time.deltaTime;
				if(mTimeCredit > mTimerMaxCredit)
				{
					mCreditAlpha += 0.02f;
					mTimeCredit = 0;
					if(mCreditAlpha >= 1.0f)
					{
						mCreditAlpha = 1.0f;
						mShowCredits = false;
					}
				}
			}
			else if(!mShowCredits)
			{
				if(!BGMSound.GetComponent<AudioSource>().isPlaying && GUI.Button(new Rect(0, 0, Screen.width, Screen.height), ""))
				{
					AutoFade.LoadLevel ("MainMenu",1,1,Color.black);
				}
			}
			GUI.DrawTexture (new Rect (0, 0, Screen.width, Screen.height), Credits);
			
		}
	}
	
	void playSound(AudioClip sound)
	{
		BGMSound.GetComponent<AudioSource>().clip = sound;
		BGMSound.GetComponent<AudioSource>().loop = false;
	  	BGMSound.GetComponent<AudioSource>().Play();
	}
}
