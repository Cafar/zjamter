using UnityEngine;
using System.Collections;
using DG.Tweening;
using System.Collections.Generic;

[System.Serializable]
public struct WaveInfo
{
	public int count;
	public float timeToSpawn;
	public float enemyVelocityIncrease;
}

public class SpawnerEnemies : MonoBehaviour {

//	public delegate void HandleWaveClear();
//	public static event HandleWaveClear OnWaveClear;

	public static SpawnerEnemies instance;

	public GameObject[] spawners;
	public GameObject[] enemies;
	public GameObject boss;

	private Transform enemy;
	private int lastSpawnZone;

	private int totalZombiesOnScreen;

	public List<WaveInfo> m_waves;

	private Coroutine co;

	private int totalEnemiesSpawned;

	void Awake()
	{
		instance = this;
	}
		
	public void ResetLevel()
	{
		GameManager.instance.m_currentWaveIndex = 0;
	}

	public void NextWave()
	{
		totalEnemiesSpawned = 0;

		if(CurrentWaveIndex == m_waves.Count)
		{
			boss.transform.DOMove(Vector3.zero,5);
			boss.GetComponent<SpriteRenderer>().sortingLayerName = "Foreground";
		}
		else
		{
			co = StartCoroutine("Spawn");
		}

		lastSpawnZone = 10;//10 no es ningún spawn
	}

	public WaveInfo CurrentWave
	{
		get
		{
			return m_waves[GameManager.instance.m_currentWaveIndex - 1];
		}
	}

	public int CurrentWaveIndex
	{
		get
		{
			return GameManager.instance.m_currentWaveIndex - 1;
		}
	}

	public void StopCo()
	{
		StopCoroutine(co);
	}

	IEnumerator Spawn()
	{
		int spawnNum = Random.Range(0,spawners.Length);
		while(spawnNum == lastSpawnZone)
			spawnNum = Random.Range(0,spawners.Length);

		lastSpawnZone = spawnNum;

		Vector2 spawnPosition = spawners[spawnNum].transform.position;
		Quaternion spawnRotation = Quaternion.identity;
		enemy = Instantiate ( enemies[Random.Range(0,enemies.Length)]).GetComponent<Transform>();
		enemy.GetComponent<EnemyController>().velocity *= CurrentWave.enemyVelocityIncrease;

		enemy.transform.position = spawners[spawnNum].transform.position;
		if(spawners[spawnNum].transform.localRotation.y == 0)
		{
			enemy.transform.localScale = new Vector2(-1,1);
			enemy.GetComponent<EnemyController>().velocity = -enemy.GetComponent<EnemyController>().velocity;
		}

		totalEnemiesSpawned++;

		yield return new WaitForSeconds(CurrentWave.timeToSpawn);

		//Wave Clear
		if(CurrentWave.count == totalEnemiesSpawned)
		{
			StopCoroutine(co);
		}
		else
		{
			co = StartCoroutine("Spawn");
		}

	}


}
