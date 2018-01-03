using UnityEngine;
using System.Collections;

public class InputManager : MonoBehaviour 
{
	public delegate void onTouchDown(int fingerID, Vector2 pos, ref bool handled);
	public delegate void onTouchDrag(int fingerID, Vector2 pos, ref bool handled);
	public delegate void onTouchUp(int fingerID, Vector2 pos, ref bool handled);
	
	public static event onTouchDown TouchDownEvent;
	public static event onTouchDrag TouchDragEvent;
	public static event onTouchUp TouchUpEvent;
	
	// The input manager singleton
	private static InputManager sSingleton;
	
	// Last mouse position for detecting drag event
	private Vector3 mLastMousePosition;
	
	/*public static InputManager singleton
	{
		get { return sSingleton; }	
	}
	
	void Awake()
	{
		sSingleton = this;	
	}*/

	void Update()
	{
#if UNITY_IPHONE || UNITY_ANDROID
		
		foreach(Touch touch in Input.touches)
		{
			bool handled = false;
			if(touch.phase == TouchPhase.Began)
			{
				Vector2 mousePos = touch.position;
				mousePos.y = Screen.height - touch.position.y;
				
				TouchDownEvent(touch.fingerId,mousePos,ref handled);
				
				mLastMousePosition = mousePos;
			}
			else if(touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled)
			{
				Vector2 mousePos = touch.position;
				mousePos.y = Screen.height - touch.position.y;
				
				TouchUpEvent(touch.fingerId,mousePos,ref handled);
			}
			else if(touch.phase == TouchPhase.Moved)
			{
				Vector2 mousePos = touch.position;
				mousePos.y = Screen.height - touch.position.y;
				
				TouchDragEvent(touch.fingerId,mousePos,ref handled);
				
				mLastMousePosition = mousePos;
			}
		}
#else
		// Mouse event
		bool handled = false;
		if (Input.GetMouseButtonDown(0))
		{
			Vector3 mousePos = Input.mousePosition;
			mousePos.y = Screen.height - mousePos.y;
			
			TouchDownEvent(0, mousePos, ref handled);
			mLastMousePosition = mousePos;
		}
		
		else if (Input.GetMouseButtonUp(0))
		{
			Vector3 mousePos = Input.mousePosition;
			mousePos.y = Screen.height - mousePos.y;
			
			TouchUpEvent(0, mousePos, ref handled);
		}
		else if (Input.GetMouseButton(0) && mLastMousePosition != Input.mousePosition)
		{
			Vector3 mousePos = Input.mousePosition;
			mousePos.y = Screen.height - mousePos.y;
			
			TouchDragEvent(0, mousePos, ref handled);
			mLastMousePosition = mousePos;
		}
#endif
	}
}
