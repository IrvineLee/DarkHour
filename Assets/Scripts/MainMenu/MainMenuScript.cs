using UnityEngine;
using System.Collections;

public class MainMenuScript : MonoBehaviour {
	
	public GUIStyle MyStyle;
	public Texture mBackground;
	public Texture mButtonPlay;
	public Texture mButtonQuit;
	
	void Start () 
	{
		MyStyle = new GUIStyle();
	}
	
	void Update () 
	{
	
	}
	
	void OnGUI()
	{
		GUI.DrawTexture(new Rect(0.0f, 0.0f, Screen.width, Screen.height), mBackground);
		if(GUI.Button(new Rect(0.7f * Screen.width, 0.6f * Screen.height, 0.15f * Screen.width, 0.1f * Screen.height), mButtonPlay, MyStyle))
		{
			AutoFade.LoadLevel("MyUnity" ,1,1,Color.black);
		}
		/*if(GUI.Button(new Rect(0.7f * Screen.width, 0.73f * Screen.height, 0.15f * Screen.width, 0.08f * Screen.height), mButtonQuit, MyStyle))
		{
			Application.Quit();
		}*/
	}
}
