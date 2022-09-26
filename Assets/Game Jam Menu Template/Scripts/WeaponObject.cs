using UnityEngine;
using System.Collections;

[System.Serializable]
public class WeaponObject : ScriptableObject {

	public string weaponName = "Weapon Name Here";
	public int cost = 50;
	public string description;

	public float maxBullets = 0;
	public float currentBullets = 0;
	public float fireRate = .5f;
	public int damage = 10;
	public bool automatic = false;
	public bool infiniteBullets = false;

}