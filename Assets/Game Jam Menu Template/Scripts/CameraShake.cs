using UnityEngine;
using System.Collections;


public class CameraShake : MonoBehaviour
{
	// Transform of the camera to shake. Grabs the gameObject's transform
	// if null.
	public Transform camTransform;
	
	// How long the object should shake for.
	public float shake = 0f;
	
	// Amplitude of the shake. A larger value shakes the camera harder.
	public float shakeAmount = 0.7f;
	public float decreaseFactor = 1.0f;
	
	Vector3 originalPos;
	
	void Awake()
	{
		if (camTransform == null)
		{
			camTransform = GetComponent(typeof(Transform)) as Transform;
		}
	}
	
	void OnEnable()
	{
		originalPos = camTransform.position;
	}
	
	void Update()
	{
//		if (shake > 0)
//		{
//			camTransform.localPosition = Vector3.Lerp(originalPos, originalPos + Vector3.up * 2, Time.deltaTime);
//			
//			shake -= Time.deltaTime * decreaseFactor;
//		}
//		else
//		{
//			shake = 0f;
//			enabled = false;
//			camTransform.localPosition = originalPos;
//		}
		Debug.Log(originalPos +Vector3.up * shakeAmount);
		camTransform.position = Vector3.Lerp(originalPos, originalPos + Vector3.up * shakeAmount, Time.deltaTime);
	}
}
