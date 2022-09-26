using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;


public class HudPlayer : MonoBehaviour {
	
	public PlayerController player;

	public int playerNum;

	public Text text;

	public GridLayoutGroup grid;

	public Image squareSelection;

	private int score;

	private List<HudWeapon> hudWeapons;

	private Animator textAnimator;

	void Start () 
	{
		player.OnSwitchWeapon 	+= HandleOnSwitchWeapon;	
		player.OnShoot 			+= HandleOnShoot;
		player.OnKillEnemy		+= HandleOnKillEnemy;
		player.OnPlayerInstance	+= HandleOnPlayerInstance;


		textAnimator = text.GetComponent<Animator>();
		hudWeapons = new List<HudWeapon>();

		for(int i = 0; i<player.weapon.Length; i++)
		{
			HudWeapon hw = Instantiate(Resources.Load("HUD/" + player.weapon[i].weaponName, typeof(HudWeapon)) as HudWeapon);

			hudWeapons.Add(hw);
			hudWeapons[i].gameObject.SetActive(false);
			hudWeapons[i].transform.SetParent(grid.transform);
			hudWeapons[i].transform.localScale = Vector3.one;
		}

		textAnimator.SetBool("Blink",true);
	}

	void OnDisable()
	{
		player.OnSwitchWeapon 	-= HandleOnSwitchWeapon;	
		player.OnShoot 			-= HandleOnShoot;
		player.OnKillEnemy		-= HandleOnKillEnemy;
		player.OnPlayerInstance	-= HandleOnPlayerInstance;
	}

	private void HandleOnPlayerInstance()
	{
		textAnimator.SetBool("Blink",false);
		squareSelection.gameObject.SetActive(true);
		text.text = "Player"+player.playerNumber+ ": " + score.ToString("D4");

		for(int i = 0; i<player.weapon.Length; i++)
		{
			hudWeapons[i].gameObject.SetActive(true);
			hudWeapons[i].bullets.text = player.weapon[i].currentBullets.ToString();
		}
		SwitchWeapon();

	}

	private void HandleOnSwitchWeapon()
	{
		SwitchWeapon();
	}

	private void HandleOnShoot()
	{
		float bullets = player.weapon[player.currentWeapon].currentBullets;
		hudWeapons[player.currentWeapon].bullets.text = bullets.ToString();
	}

	private void HandleOnKillEnemy(int _score)
	{
		score += _score;
		text.text = "Player"+player.playerNumber+ ": " + score.ToString("D4");
	}

	public void SwitchWeapon()
	{
		SetSquare();
	}

	private void SetSquare()
	{
		squareSelection.transform.SetParent(hudWeapons[player.currentWeapon].transform);
		squareSelection.transform.localPosition = Vector3.zero;
	}
}
