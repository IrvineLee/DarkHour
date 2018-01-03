using UnityEngine;
using System.Collections;

public class AttackScript : MonoBehaviour 
{
	public Vector3 startPosition;
	//public bool isToBeDestroyed;
	public float timer;
	
	Vector3 mPosition;
	//public float mMoveSpeed = 1.0f;
	public float mAttackTimer = 4.5f;
	
	public bool mIsWalking = true;
	
	TriggerScript mTriggerScript;
	
	void Start () 
	{
		timer = 0.0f;
		mTriggerScript = GameObject.Find ("TriggerFather").GetComponent<TriggerScript>();
		startPosition = mTriggerScript.GhostPrefab.transform.position;
		GameObject.Find (("FatherPrefab(Clone)")).active = false;
		//mTriggerScript.GhostPrefab.GetComponent<MeshRenderer>().enabled = false;
		mTriggerScript.GhostPrefab = (GameObject)Resources.Load("FatherBite");
		GameObject gameObject = (GameObject)Instantiate(mTriggerScript.GhostPrefab, startPosition, Quaternion.identity);
		Debug.Log ("=================");
		//mPosition = gameObject.transform.localPosition;
	}
	
	void Update () 
	{
		timer += Time.deltaTime;

		if(timer > mAttackTimer)
		{
			Debug.Log ("Stop Attack!");
			Destroy(gameObject.GetComponent<AttackScript>());
		}
	}

}
