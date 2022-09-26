using UnityEngine;
using System.Collections;

public class WaveMove : MonoBehaviour {
	
	public float MoveSpeed = 5.0f;
	
	public float frequency = 20.0f;  // Speed of sine movement
	public float magnitude = 0.5f;   // Size of sine movement
	private Vector3 axis;
	
	private Vector3 pos;
	
	void Start () {
		pos = transform.position;
		axis = transform.up;  // May or may not be the axis you want
		
	}
	
	void Update () {
		pos += Random.insideUnitSphere * Time.deltaTime * MoveSpeed;
		transform.position = pos + axis * Mathf.Sin (Time.time * frequency) * magnitude;
		
	}
}
