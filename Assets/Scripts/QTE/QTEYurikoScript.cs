using UnityEngine;
using System.Collections;

public class QTEYurikoScript : QTEScript 
{
	MoveScript mMoveScript;

	void Start () 
	{
		gameObjectPrefab = GameObject.FindGameObjectWithTag("Yuriko");
		AddAnimationSequence("MoveScript", 0, false);
	}
	
	void Update () 
	{
		base.Update();
	}
}
