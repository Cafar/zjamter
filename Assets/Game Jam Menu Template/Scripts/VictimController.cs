using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class VictimController : MonoBehaviour {

	public List<AudioClip> gritos;
	public LayerMask ignoreLayer;
	private float time;
	// Use this for initialization
	void Start () 
	{
		time = 0;
	}
	
	// Update is called once per frame
	void Update () 
	{
	}
	void OnTriggerEnter2D(Collider2D col)
	{
		if(col.transform.tag == "Enemy")
		{
			SoundManager.Instance.Play2DSound(gritos[Random.Range(0,gritos.Count)]);
		}
	}
}
