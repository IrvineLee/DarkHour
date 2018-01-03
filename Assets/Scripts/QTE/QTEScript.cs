using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class QTEScript : MonoBehaviour 
{
	public class AnimationSequence
	{
		public string scriptName;
		public MonoBehaviour script;
		public float time;
		public bool isAnAttack;
	}
	
	public enum TYPE_OF_INTERACTION
	{
		NONE = 0,
		BUTTON,
		SWIPE
	}
	public TYPE_OF_INTERACTION mTypeOfInteraction = TYPE_OF_INTERACTION.NONE;
	
	public float timer;
	public List<AnimationSequence> mAnimationSequenceList = new List<AnimationSequence>();
	public GameObject gameObjectPrefab;
	
	[HideInInspector]
	public int mNoOfButton;
	[HideInInspector]
	public bool mIsDamaged = false;
	[HideInInspector]
	public bool mHasTakenDamage = false;
	
	void Start () 
	{
		timer = 0.0f;
	}
	
	protected virtual void Update () 
	{
		for(int i = 0; i < mAnimationSequenceList.Count; i++)
		{
			timer += Time.deltaTime;
			if(timer > mAnimationSequenceList[i].time)
			{
				UnityEngineInternal.APIUpdaterRuntimeServices.AddComponent(gameObjectPrefab, "Assets/Scripts/QTE/QTEScript.cs (46,5)", mAnimationSequenceList[i].scriptName);
				mAnimationSequenceList[i].script = (MonoBehaviour)gameObjectPrefab.GetComponent(mAnimationSequenceList[i].scriptName);
				mAnimationSequenceList.RemoveAt(i);
				i--;				
				timer = 0;
			}
		}
	}
	
	protected void AddAnimationSequence(string scriptName, float time, bool isAnAttack)
	{
		AnimationSequence tempAnimationSequence = new AnimationSequence();
		
		tempAnimationSequence.scriptName = scriptName;
		tempAnimationSequence.time = time;
		tempAnimationSequence.isAnAttack = isAnAttack;
		
		mAnimationSequenceList.Add(tempAnimationSequence);
	}
}
