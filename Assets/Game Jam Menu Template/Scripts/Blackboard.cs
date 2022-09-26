using UnityEngine;
using System.Collections;

public class Blackboard : MonoBehaviour {
	
	public AudioClip MusicGame;

	public int zombiesKilled;

	public bool spawnWaves;

	private static Blackboard instance;
	public static Blackboard Instance 
	{
		get
		{
			if(instance == null)
			{
				string resourcesPrefabPath = "Blackboard";
				// Search in resources folder for this GameObject
				Blackboard managerPrefab = Resources.Load<Blackboard>(resourcesPrefabPath);
				
				if(managerPrefab == null)
				{
					Debug.LogError("[ERROR] Prefab "+resourcesPrefabPath+" not found in Resources directory");
					return null;
				}
				
				Instance = Instantiate(managerPrefab) as Blackboard;
			}
			
			return instance;
		}
		
		private set{
			instance = value;
		}
	}

	void Awake()
	{
		MusicManager.Instance.Play2DSound(MusicGame);

		Screen.SetResolution(720,480,false);
	}

	void Update()
	{
		if(Input.GetKeyDown(KeyCode.Escape))
		{
			Application.Quit();
		}
	}
}
