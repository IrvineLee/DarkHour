using UnityEngine;
using System.Collections;

public class FlashLightFlickerScript : MonoBehaviour
{	
	public bool IsEnabled = false;
	public float Duration = 3.0f;
	public Light light;
	public float speed;
	public float noise;
	public float minLightIntensity = 0.5f;
	public float maxLightIntensity = 1.2f;
	
	private float mTime = 0.0f;
	bool mIsFlickering = false;
	
	void Update ()
	{
		if(IsEnabled)
		{
			if(!mIsFlickering)
			{
				light.enabled = false;
				StartCoroutine(Flicker());
				mIsFlickering = true;
			}
			
			mTime += Time.deltaTime;
		}
	}
	
	IEnumerator Flicker ()
	{
		light.enabled = true;
		light.intensity = Random.Range(minLightIntensity, maxLightIntensity);
		float randNoise = Random.Range(-1,1) * Random.Range(-noise,noise);
		yield return new WaitForSeconds(speed+randNoise);
		light.enabled = false;
		yield return new WaitForSeconds(speed);
	
		if(mTime > Duration)
		{
			IsEnabled = false;
			mIsFlickering = false;
			light.enabled = true;
			light.intensity = maxLightIntensity;
		}
		else
		{
			StartCoroutine(Flicker());
		}
	}
}