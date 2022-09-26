using UnityEngine;
using System.Collections;

public class LifeModule : MonoBehaviour {

	public float	maxLife;

	public bool		invulnerable;

	public delegate void KilledHandler(GameObject _who);
	public event KilledHandler OnKill;

	public delegate bool PreDoDamageHandler(LifeModule _who, GameObject _bullet,Collision _collision);
	public event PreDoDamageHandler OnPreDoDamage;

	public delegate void LifeChangeHandler(LifeModule _who, float _currentLife, float _previous, float _percentage);
	public event LifeChangeHandler OnLifeChange;

	public float currentLife;

	void Awake()
	{
		SetMaxLife(maxLife);
	}

	/// <summary>
	/// Rest life! Inform if killed
	/// </summary>
	public void DoDamage(float _damageQuantity,GameObject _bullet,Collision _collision)
	{
		if(invulnerable)
			return;

		bool removeLife = true; 

		if(OnPreDoDamage != null)
			removeLife = OnPreDoDamage(this,_bullet,_collision);

		if(removeLife)
			DoDamage(_damageQuantity);
	}

	/// <summary>
	/// Does the damage.
	/// </summary>
	public void DoDamage(float _damageQuantity)
	{
		if(invulnerable)
			return;

		_damageQuantity = Mathf.Abs(_damageQuantity);

		SetLife(currentLife - _damageQuantity);
	}

	public IEnumerator DoDamage(float _damageQuantity, float _delay)
	{
		yield return new WaitForSeconds(_delay);

		DoDamage(_damageQuantity);
	}

	public void AddLife(float _quantity)
	{
		currentLife += _quantity;
	}

	public void SetMaxLife(float _maxLife)
	{
		maxLife = _maxLife;
		SetLife(maxLife);
	}

	public float GetPercentage()
	{
		return currentLife/maxLife;
	}

	public float GetCurrentLife()
	{
		return currentLife;
	}

	public void Reset()
	{
		SetMaxLife(maxLife);
	}

	public bool IsDead()
	{
		return currentLife <= 0f;
	}

	/// <summary>
	/// Use DoDamage method instead of this one. Use this only for development purposes.
	/// </summary>
	public void SetLife(float _quantity)
	{
		float previousLife = currentLife;
		currentLife = _quantity;

		if(OnLifeChange != null)
			OnLifeChange(this,currentLife, previousLife, GetPercentage());

		if(currentLife <= 0f)
		{
			currentLife = 0f;
			
			if(OnKill != null)
				OnKill(gameObject);
		}
	}
}
