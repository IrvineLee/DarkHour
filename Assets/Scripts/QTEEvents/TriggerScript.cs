using UnityEngine;
using System.Collections;

public class TriggerScript : MonoBehaviour 
{	
	GameObject Player;
	GameObject MainCamera;
	GameObject FlashLight;
	
	public AudioClip AudioScene;
	
	public bool IsEnabled = false;
	
	public enum QTE_TYPE
	{
		CRAWL_TURN = 0,
		STAND,
		QTE_FATHER,
		QTE_DAUGTHER,
		QTE_YURIKO
	}
	
	public QTE_TYPE QTEType;
	
	public bool isTriggered;
	
	public GameObject GhostPrefab;
	public GameObject SpawnPosition;

	public Transform LookAtTarget;
	public Transform MoveToTarget;
	
	public Texture ScaryImage;
	
	private Vector3 mStartPosition;
    private Vector3 mEndPosition;
	private float mStartTime;
	private float mRotateSpeed = 2.5f;
	
	private Vector3 mEndCameraPos;
	private Vector3 mEndFlashlightPos;
	Quaternion mEndCameraRotation;
	
	bool mGetStartTime = true;
	bool mGetInPosition = true;
	bool mIsStopAnimation = false;
	
	float mTime;
	float mTimerMax = 22.0f;
	
	float mTime2;
	float mTimerMax2 = 1.0f;
	
	bool mHasPlayedAudio = false;
	
	PlayerStatusScript mPlayerStatusScript;
	PlayerController mPlayerController;
	CameraRayScript mCameraRayScript;
	CrawlScript mCrawlScript;
	AxisController mAxisControllerRight;
	ExamineScript mExamineScript;

	void Start () 
	{
		gameObject.GetComponent<MeshRenderer>().enabled = false;
		isTriggered = false;
		
		Player = GameObject.Find("Player");
		MainCamera = GameObject.Find ("Main Camera");
		FlashLight = GameObject.Find ("Spotlight");
		
		mPlayerStatusScript = Player.GetComponent<PlayerStatusScript>();
		mPlayerController = Player.GetComponent<PlayerController>();
		mCameraRayScript = MainCamera.GetComponent<CameraRayScript>();
		mCrawlScript = Player.GetComponent<CrawlScript>();
		mExamineScript = GameObject.Find ("CrawlExamine").GetComponent<ExamineScript>();
		
		AxisController[] axisControllerList = Player.GetComponents<AxisController>();
		for(int i=0; i<axisControllerList.GetLength(0); i++)
		{
			if(axisControllerList[i].Name == "Right")
			{
				mAxisControllerRight = axisControllerList[i];
				break;
			}
		}
	}

	void Update () 
	{
		if(isTriggered)
		{
			if(QTEType == QTE_TYPE.STAND)
			{
				mPlayerController.rotationX = 180;
				mCrawlScript.EnableCrawl = true;
				mCrawlScript.IsCrawl = false;
				QTEDaughterScript.mActionSequence = QTEDaughterScript.ActionSequence.NONE;
				mPlayerController.DisablePlayerController = false;
				mCameraRayScript.DisableRayCast = false;
				isTriggered = false;
				
				Destroy(GameObject.Find ("TriggerDaughter").GetComponent<QTEDaughterScript>());
				Destroy(GameObject.Find ("YurikoPrefab(Clone)"));
				
				GameObject.Find ("TriggerStand").GetComponent<Collider>().enabled = false;
				GameObject.Find ("CrawlExamine").GetComponent<Collider>().enabled = true;
				mPlayerStatusScript.IsDisableBook = false;
				mPlayerStatusScript.IsQTEScene = false;
			}
			else if(QTEType == QTE_TYPE.CRAWL_TURN)
			{
				if(mGetInPosition)
				{
					QTEDaughterScript.IsCrawlOnly = true;
					GetInPosition();
					if(!mGetInPosition) isTriggered = false;
				}
			}
			else if(QTEType == QTE_TYPE.QTE_DAUGTHER)
			{
				if(mGetInPosition)
				{
					GetInPosition();
					if(!mGetInPosition) isTriggered = false;
				}
			}
			else if(QTEType == QTE_TYPE.QTE_FATHER)
			{
				if(mGetInPosition)
				{
					GetInPosition();
				}
				
				mTime += Time.deltaTime;
				if(mTime > mTimerMax)
				{
					MainCamera.GetComponent<Animation>().Stop ("ShakeAnimation");
					MainCamera.GetComponent<Animation>()["ShakeAnimation"].blendMode = AnimationBlendMode.Blend;
					mPlayerStatusScript.IsPlayingAnimation = false;
					mTime = 0;
				}
				
				if (!MainCamera.GetComponent<Animation>().IsPlaying("Father Scene 1") || mPlayerStatusScript.mHealth == PlayerStatusScript.HEALTH_SCRIPT.DEATH) // Teleport Player collider to main camera
				{
					mPlayerStatusScript.IsPlayingAnimation = false;

					mEndCameraPos = MainCamera.transform.position;
					mEndCameraRotation = MainCamera.transform.rotation;
					
					Player.transform.position = mEndCameraPos;
					Vector3 tempPosition = Player.transform.position;
					tempPosition.y = 0;
					Player.transform.position = tempPosition;
					
					mPlayerController.rotationX = mEndCameraRotation.eulerAngles.y;
					mPlayerController.rotationY = mEndCameraRotation.eulerAngles.x;
					
					MainCamera.transform.localPosition = new Vector3(0.0f, 0.9f, 0.0f);
					MainCamera.transform.localRotation = Quaternion.identity;
					FlashLight.transform.localPosition = new Vector3(-0.34f, 0.72f, 0.0f);
					FlashLight.transform.localRotation = Quaternion.identity;
					FlashLight.transform.Rotate(0, 5, 0);
					
					mPlayerController.DisablePlayerController = false;
					mCameraRayScript.DisableRayCast = false;
					
					if(mPlayerStatusScript.mHealth != PlayerStatusScript.HEALTH_SCRIPT.DEATH)
					{
						mPlayerStatusScript.mSetIntialVal = true;
						mCrawlScript.EnableCrawl = true;
						mCrawlScript.IsCrawl = true;
					}
					
					isTriggered = false;
					
					Destroy(GetComponent<QTEFatherScript>());
					
					Destroy(GameObject.FindGameObjectWithTag("Father"));
					Destroy(GameObject.Find ("GameQTE"));
					GameObject.Find ("/Father Scene 1/TriggerFather").GetComponent<TriggerScript>().IsEnabled = false;
					GameObject.Find("/Floor 1/WesternDoor1/GameObject/Plane009/Left").tag = "OpenDoor";
					GameObject.Find("/Floor 1/WesternDoor1/GameObject/Plane009/Left").GetComponent<WesternDoorScript>().IsLocked = false;
					GameObject.Find ("CrawlExamine").GetComponent<Collider>().enabled = true;
					mExamineScript.mTypeOfMessage = ExamineScript.TYPE_OF_MESSAGE.TRIGGER;
					mPlayerStatusScript.IsDisableBook = false;
					mPlayerStatusScript.IsQTEScene = false;
				}
			}
			else if(QTEType == QTE_TYPE.QTE_YURIKO)
			{
				if(mGetInPosition)
				{
					GetInPosition();
				}
				mPlayerStatusScript.mHealth = PlayerStatusScript.HEALTH_SCRIPT.DEATH;
				mTime2 += Time.deltaTime;
				if(!mHasPlayedAudio && mTime2 > mTimerMax2)
				{
					GetComponent<AudioSource>().PlayOneShot(AudioScene);
					mHasPlayedAudio = true;
					mTime2 = 0;
				}
			}
		}
	}
	
	void OnTriggerEnter(Collider other)
	{
		if(IsEnabled && !isTriggered)
		{
	        if(other.gameObject.tag == "Player")
			{
				mPlayerStatusScript.IsDisableBook = true;
				mPlayerStatusScript.IsQTEScene = true;
				CreateQTE();
			}
		}
    }
	
	void GetInPosition()
	{
		if (mGetStartTime == true)
		{
			mStartPosition = Player.transform.position;
			mEndPosition = MoveToTarget.transform.position;
			Vector3 temp = mEndPosition;
			temp.y = mStartPosition.y;
			mEndPosition = temp;
			mStartTime = Time.time;
			mGetStartTime = false;
		}

		Player.transform.position = Vector3.Lerp(mStartPosition, mEndPosition, 2.5f * (Time.time - mStartTime));
		
		mPlayerController.DisablePlayerController = true;
		Player.transform.rotation = Quaternion.Slerp(Player.transform.rotation, Quaternion.LookRotation(LookAtTarget.transform.position - Player.transform.position), Time.deltaTime * mRotateSpeed);
		
		if(Player.transform.position == mEndPosition && Mathf.FloorToInt(Mathf.Abs(Player.transform.rotation.eulerAngles.y)) == Mathf.FloorToInt(Mathf.Abs(Quaternion.LookRotation(LookAtTarget.transform.position - Player.transform.position).eulerAngles.y))
			&& Mathf.FloorToInt(Mathf.Abs(Player.transform.rotation.eulerAngles.x)) == Mathf.FloorToInt(Mathf.Abs(Quaternion.LookRotation(LookAtTarget.transform.position - Player.transform.position).eulerAngles.x))
			&& Mathf.FloorToInt(Mathf.Abs(Player.transform.rotation.eulerAngles.z)) == Mathf.FloorToInt(Mathf.Abs(Quaternion.LookRotation(LookAtTarget.transform.position - Player.transform.position).eulerAngles.z)))
		{
			if(QTEType == QTE_TYPE.QTE_DAUGTHER) // Offset nicely
			{
				Quaternion temp = Player.transform.rotation;
				temp.x = 0;
				temp.y = 0;
				temp.z = 0;
				Player.transform.rotation = temp;
			}
			mGetInPosition = false;
		}
	}
	
	void CreateQTE()
	{
		if(QTEType == QTE_TYPE.CRAWL_TURN)
		{
			isTriggered = true;
			mGetInPosition = true;
		}
		else if(QTEType == QTE_TYPE.STAND)
		{
			isTriggered = true;
		}
		else if(QTEType == QTE_TYPE.QTE_FATHER)
		{
			GameObject gameObject = (GameObject)Instantiate(GhostPrefab, SpawnPosition.transform.position, Quaternion.identity); //Quaternion.AngleAxis(270.0f, new Vector3(1.0f,0.0f,0.0f)) * Quaternion.AngleAxis(270.0f, new Vector3(0.0f,0.0f,1.0f)));
			GameObject gameQTE = new GameObject("GameQTE");
			gameObject.transform.parent = gameQTE.transform;
			gameQTE.AddComponent<QTEFatherScript>();

			isTriggered = true;
			mGetInPosition = true;
			mCameraRayScript.DisableRayCast = true;
		    MainCamera.GetComponent<Animation>().Play("Father Scene 1");
			FlashLight.GetComponent<Animation>().Play("Father Scene 1");
			mPlayerStatusScript.IsPlayingAnimation = true;
			
			/*MainCamera.animation["ShakeAnimation"].layer = 1; 
			MainCamera.animation["ShakeAnimation"].blendMode = AnimationBlendMode.Additive;
			MainCamera.animation.Play("ShakeAnimation"); 
			MainCamera.animation["ShakeAnimation"].weight = 1.0f;*/
			
			/*MainCamera.animation["Father Scene 1"].layer = 0; 
			MainCamera.animation.Play("Father Scene 1"); 
			MainCamera.animation["Father Scene 1"].weight=1.0f;*/
		}
		else if(QTEType == QTE_TYPE.QTE_DAUGTHER)
		{
			GameObject gameObject = (GameObject)Instantiate(GhostPrefab, SpawnPosition.transform.position, /*Quaternion.identity*/ Quaternion.AngleAxis(270.0f, new Vector3(1.0f,0.0f,0.0f)) * Quaternion.AngleAxis(90.0f, new Vector3(0.0f,0.0f,1.0f)));
			gameObject.AddComponent<QTEDaughterScript>();

			isTriggered = true;
			mGetInPosition = true;
			mCameraRayScript.DisableRayCast = true;
		}
		else if(QTEType == QTE_TYPE.QTE_YURIKO)
		{
			GameObject gameObject = (GameObject)Instantiate(GhostPrefab, SpawnPosition.transform.position, /*Quaternion.identity*/ Quaternion.AngleAxis(270.0f, new Vector3(1.0f,0.0f,0.0f)) * Quaternion.AngleAxis(270.0f, new Vector3(0.0f,0.0f,1.0f)));
			gameObject.AddComponent<QTEYurikoScript>();
			mPlayerStatusScript.IsPlayingAnimation = true;
			
			isTriggered = true;
			mGetInPosition = true;
			mCameraRayScript.DisableRayCast = true;
			mPlayerStatusScript.IsEnd = true;
		}
	}
}
