using UnityEngine;
using System.Collections;

public class ChestScript : MonoBehaviour 
{
	public bool IsEnabled = false;
	public bool IsOpened = false;
	public static bool IsDestroyedKey = false;
	
	private float mSpeed = 0.25f;
	private float mKeySpeedModifier = 3.0f; 
	private float mMoveDistance;
	private float mMoveDistanceMax = 0.15f;
	private float mCumulativeDistance;
	
	GameObject Key;
	GameObject DrawerWithKey;
	
	// Use this for initialization
	void Start () 
	{
		Key = GameObject.Find ("Key");
		DrawerWithKey = GameObject.Find ("DrawerWithKey");
		IsDestroyedKey = false;
	}

	// Update is called once per frame
	void Update () 
	{
		if(IsEnabled)
		{
			if(!IsOpened) // Open drawer
			{
				Vector3 temp = gameObject.transform.localPosition;
				mMoveDistance = mSpeed * Time.deltaTime;
				mCumulativeDistance += mMoveDistance;
				temp.x += mMoveDistance;
				gameObject.transform.localPosition = temp;
				
				Debug.Log (IsDestroyedKey);
				if(!IsDestroyedKey && gameObject == DrawerWithKey)
				{
					Debug.Log ("DSDSDS");
					Vector3 temp2 = Key.transform.localPosition;
					temp2.x += mMoveDistance;
					Key.transform.localPosition = temp2;
				}
				
				if(mCumulativeDistance > mMoveDistanceMax)
				{
					if(!IsDestroyedKey && gameObject == DrawerWithKey)
					{
						gameObject.layer = 0;
					}
					temp.x -= mCumulativeDistance;
					temp.x += mMoveDistance;
					mCumulativeDistance = 0;
					IsOpened = true;
					IsEnabled = false;
				}
			}
			else if(IsOpened) // Close drawer
			{
				Vector3 temp = gameObject.transform.localPosition;
				mMoveDistance = mSpeed * Time.deltaTime;
				mCumulativeDistance += mMoveDistance;
				temp.x -= mMoveDistance;
				gameObject.transform.localPosition = temp;
				
				if(!IsDestroyedKey && gameObject == DrawerWithKey)
				{
					Vector3 temp2 = Key.transform.localPosition;
					temp2.x -= mMoveDistance;
					Key.transform.localPosition = temp2;
				}
				
				if(mCumulativeDistance > mMoveDistanceMax)
				{
					temp.x += mCumulativeDistance;
					temp.x -= mMoveDistance;
					mCumulativeDistance = 0;
					IsOpened = false;
					IsEnabled = false;
				}
			}
		}
		
	}
}
