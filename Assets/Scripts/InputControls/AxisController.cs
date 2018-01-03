using UnityEngine;
using System.Collections;

public class AxisController : MonoBehaviour {
	
	public string Name;
	public Rect Region;
	public float DragRange = 1.0f;
	public bool Locked = false;
	public Vector2 LockedStartPos;
	
	private int mFingerID = -1;
	private Vector2 mStartPos;
	private Vector2 mActiveDir;
		
	void OnTouchDown(int fingerID, Vector2 pos, ref bool handled)
	{
		if(handled || mFingerID != -1) return;
		
		//convert pixel pos to 0-1 ratio
		float x = pos.x / Screen.width;
		float y = pos.y / Screen.height;
		
		Vector2 ratioPos = new Vector2(x,y);
		if(!Region.Contains(ratioPos)) return;
		
		mFingerID = fingerID;
		
		if(Locked)
		{
			mStartPos.x = LockedStartPos.x * Screen.width;
			mStartPos.y = LockedStartPos.y * Screen.height;
		}
		else mStartPos = pos;
		
		handled = true;
		//Debug.Log ("AxisController::TouchDown() finger: " + fingerID + ", pos:" + pos);
	}
	
	void OnTouchDrag(int fingerID, Vector2 pos, ref bool handled)
	{
		if(handled || mFingerID != fingerID) return;

		//mActiveDir = pos - mStartPos;
		mActiveDir = Vector2.ClampMagnitude (pos - mStartPos, DragRange);
		//Debug.Log ("AxisController::TouchDrag(): finger: " + fingerID + ", pos:" + mActiveDir);
		handled = true;
	}
	
	void OnTouchUp(int fingerID, Vector2 pos, ref bool handled)
	{
		if(handled || mFingerID != fingerID) return;

		mFingerID = -1;
		mActiveDir = Vector2.zero;
		handled = true;
		//Debug.Log ("AxisController::TouchUp() finger: " + fingerID + ", pos:" + pos);
	}
	
	void Start()
	{
		InputManager.TouchDownEvent += OnTouchDown;
		InputManager.TouchDragEvent += OnTouchDrag;
		InputManager.TouchUpEvent += OnTouchUp;
		
		if (Screen.dpi > 0.0f) DragRange *= Screen.dpi;
		else DragRange *= 96.0f;
	}

	void OnDestroy()
	{
		InputManager.TouchDownEvent -= OnTouchDown;
		InputManager.TouchDragEvent -= OnTouchDrag;
		InputManager.TouchUpEvent -= OnTouchUp;
	}
	
	/*void OnGUI()
	{
		if(mFingerID == -1) return;

		GUI.Label (new Rect(mStartPos.x, mStartPos.y, 100, 20), "0");
		
		GUI.Label (new Rect
			(mStartPos.x + mActiveDir.x,
			mStartPos.y + mActiveDir.y, 100, 100), "0");
	}*/
	
	public Vector3 GetDirection()
	{
		return new Vector3(mActiveDir.x, 0.0f, -mActiveDir.y);
	}
	
	public void SetDirection(Vector3 direction)
	{
		mActiveDir = direction;
	}
}
