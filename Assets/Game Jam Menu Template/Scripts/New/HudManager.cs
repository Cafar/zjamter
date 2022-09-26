using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using DG.Tweening;


public class HudManager : MonoBehaviour {
	


	public HudPlayer player1, player2;

	public static HudManager instance;



	void Awake () 
	{
		instance = this;

	}


	public void ChangeWeapon(int weaponNum)
	{

		SetSquare();
	}

	private void SetSquare()
	{
//		square.transform.SetParent(weaponsHud[weaponSelected].transform);
	}

	void Update () 
	{
	
	}


}
