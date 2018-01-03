using UnityEngine;
using System.Collections;

public class MoveScript : MonoBehaviour 
{
	public Vector3 startPosition;
	//public bool isToBeDestroyed;
	public float timer;
	
	Vector3 mPosition;
	public float mMoveSpeed = 1.0f;
	public float mMoveTimer = 14.0f;
	
	public bool mIsWalking = true;
	
	void Start () 
	{
		timer = 0.0f;
		//isToBeDestroyed = false;
		mPosition = gameObject.transform.localPosition;
	}
	
	void Update () 
	{
		timer += Time.deltaTime;
		
		mPosition.z +=  Time.deltaTime * mMoveSpeed;
		gameObject.transform.position = mPosition;

		if(timer > mMoveTimer)
		{
			Debug.Log ("Stop Moving!");
			mIsWalking = false;
			Destroy(gameObject.GetComponent<MoveScript>());
		}
	}

}
