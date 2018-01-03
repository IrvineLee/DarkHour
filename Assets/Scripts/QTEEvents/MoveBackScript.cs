using UnityEngine;
using System.Collections;

public class MoveBackScript : MonoBehaviour 
{
	public Vector3 startPosition;
	//public bool isToBeDestroyed;
	public float timer;
	
	Vector3 mPosition;
	public float mMoveSpeed = 3.2f;
	public float mMoveTimer = 3.5f;
	
	public bool mIsWalking = true;
	
	float mDistance;
	bool mGetDistance = true;
	
	GameObject Player;
	
	void Start () 
	{
		timer = 0.0f;
		//isToBeDestroyed = false;
		mPosition = gameObject.transform.localPosition;
		Player = GameObject.FindGameObjectWithTag ("Player");
		
	}
	
	void Update () 
	{
		if(mGetDistance)
		{
			mDistance = Vector3.Distance(Player.transform.position, transform.position);
			mMoveSpeed = (mDistance - 1.5f) / mMoveTimer;
			Debug.Log(mDistance);
			mGetDistance = false;
		}
		timer += Time.deltaTime;
		
		mPosition.z -=  Time.deltaTime * mMoveSpeed;
		gameObject.transform.position = mPosition;

		if(timer > mMoveTimer)
		{
			Debug.Log ("Stop Moving!");
			mGetDistance = true;
			mIsWalking = false;
			Destroy(gameObject.GetComponent<MoveBackScript>());
		}
	}

}
