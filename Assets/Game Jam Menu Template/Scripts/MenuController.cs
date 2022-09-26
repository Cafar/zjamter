using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MenuController : MonoBehaviour {

	public Text text1,text2,text3,text4;
	private int num;

	void Awake () 
	{
		num = 0;
	}
	
	// Update is called once per frame
	void Update () 
	{
		if(Input.GetButtonDown ("StartPlayer1"))
		{
			Gameover();
		}
		if(Input.GetButtonDown ("StartPlayer2"))
		{
			Gameover();
		}
		if(Input.GetButtonDown ("StartPlayer3"))
		{
			Gameover();
		}
	}
	void Gameover()
	{
		Debug.Log(num);
		if(num == 0)
		{
			text1.gameObject.SetActive(false);
			text2.gameObject.SetActive(true);

		}
		if(num == 1)
		{
			text2.gameObject.SetActive(false);
			text3.gameObject.SetActive(true);

		}
		if(num == 2)
		{
			text3.gameObject.SetActive(false);
			text4.gameObject.SetActive(true);

		}
		if(num == 3)
		{
			SceneManager.LoadScene("Main");
		}
		num++;
	}
}
