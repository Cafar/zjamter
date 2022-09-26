using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class SimpleAnimationImage : MonoBehaviour {

	public Sprite[] sprites;
	public float framesPerSecond;
	private Image spriteRenderer;
	
	// Use this for initialization
	void Start () {
		spriteRenderer = GetComponent<Image>() as Image;
	}
	
	// Update is called once per frame
	void Update () {
		int index = (int)(Time.timeSinceLevelLoad * framesPerSecond);
		index = index % sprites.Length;
		spriteRenderer.sprite = sprites[index];
	}
}
