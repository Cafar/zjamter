using UnityEngine;
using System.Collections;

public class SimpleAnimationSprite : MonoBehaviour {

	public delegate void AnimationHandler();
	public event AnimationHandler OnFinishAnimation;

	public Sprite[] sprites;
	public float framesPerSecond;
	public bool oneTime;
	private SpriteRenderer spriteRenderer;
	private bool animated;
	// Use this for initialization
	public int index;
	void Awake () 
	{
		spriteRenderer = GetComponent<Renderer>() as SpriteRenderer;
		animated = true;
	}
	
	// Update is called once per frame
	void Update () 
	{
		if(animated)
		{
			index = (int)(Time.timeSinceLevelLoad * framesPerSecond);
			index = index % sprites.Length;
			spriteRenderer.sprite = sprites[index];
			if(oneTime)
			{
				if(index >= sprites.Length-1)
				{
					animated = false;
					if(OnFinishAnimation != null)
						OnFinishAnimation();
				}
			}
		}
	}
}
