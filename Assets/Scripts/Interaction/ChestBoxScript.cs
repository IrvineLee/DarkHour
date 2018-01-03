using UnityEngine;
using System.Collections;

public class ChestBoxScript : MonoBehaviour 
{
	public GameObject ChestHinge;
	public bool IsEnabled = false;

	private int mRotationAngle = 90;
	private float mRotationX;
	private float mCumulativeX;
	private float mOpenDoorSpeed = 100.0f;
	
	// Update is called once per frame
	void Update () 
	{
		if(IsEnabled)
		{
			mRotationX = Time.deltaTime * mOpenDoorSpeed;
			mCumulativeX += mRotationX;
			
			if(mCumulativeX > mRotationAngle)
			{
				mCumulativeX -= mRotationX;
				mRotationX = mRotationAngle - mCumulativeX;
				mCumulativeX = 0;
				IsEnabled = false;
			}
			ChestHinge.transform.Rotate(mRotationX, 0, 0);
		}
		
	}
}
