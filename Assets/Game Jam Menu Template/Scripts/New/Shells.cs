using UnityEngine;
using System.Collections;

public class Shells : MonoBehaviour {

	private Rigidbody2D rigid;
 	
	void Start () 
	{
		rigid = GetComponent<Rigidbody2D>();

		rigid.AddForce(new Vector2(Random.Range(-.3f,.3f),Random.Range(.3f,.5f)),ForceMode2D.Impulse);
		rigid.AddTorque(.1f,ForceMode2D.Impulse);
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
