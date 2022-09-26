using UnityEngine;
using System.Collections;

public class SimpleMove : MonoBehaviour {

	public Vector3 dir;
		
	// Update is called once per frame
	void Update () 
	{
		transform.localPosition += dir*Time.deltaTime;
		if(transform.localPosition.x < -50)
		{
			transform.localPosition = new Vector2(50, transform.localPosition.y);
		}
	}
}
