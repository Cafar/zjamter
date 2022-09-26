using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//Me creo un componente que sea singleton que sea MusicManager 


/// <summary>
/// Utility class to play sounds from anywhere. All game sounds must be played from this class for volume normalization and performance.
/// A pool of AudioSources is created on Awake of this Manager. When you play a 2D Sound, a free AudioSouce is taken from the pool if there is at least an
/// AudioSource that is not playing a clip, but there is a max number of sounds that can be played at the same time equal to the number of AudioSources of the pool.
/// 
/// To be sure no sound is stopped, you can use 3D sound methods, that allow you to pass by parameter your custom AudioSource.
/// </summary>
/// <author> Gonzalo De Santos </author>
/// <refactoring> Adrian Mesa </refactoring>
public class MusicManager : MonoBehaviour
{
	public float volume = 1f;
	
	public float defaultVolume = 1; 
	
	public float pitch  = 1f;
	
	public bool repeat = true;
	
	/// <summary>
	/// Max audio sources setted to 10 => 10 sounds at most playing at the same time. Change it if needed.
	/// </summary>
	private const int MAX_AUDIO_SOURCES = 1;
	
	private AudioSourcePool audioSourcePool;
	
	/// <summary>
	/// Store all audiosources of the scene tdue to performance improvements
	/// </summary>
	private List<AudioSource> cachedAudioSources = new List<AudioSource>();
	
	#region Singleton
	
	private static MusicManager instance;
	
	public static MusicManager Instance
	{
		get
		{
			if (instance == null)
			{
				Object soundManagerPrefab = Resources.Load("MusicManager");
				
				if(soundManagerPrefab == null)
				{
					Debug.LogError("MusicManager prefab must exist in Resources folder");
					return null;
				}
				else
				{
					instance 		= (Instantiate(soundManagerPrefab) as GameObject).GetComponent<MusicManager>();
					instance.name	= "MusicManager";
				}
			}
			
			return instance;
		}
	}
	
	public void OnApplicationQuit ()
	{
		instance = null;
	}
	
	#endregion
	
	void Awake()
	{
		instance = this;
		audioSourcePool = new AudioSourcePool(MAX_AUDIO_SOURCES, gameObject, true);
		DontDestroyOnLoad(this.gameObject);
		PrepareAudioSources();
		
		
		if(PlayerPrefs.HasKey("MusicManagerVolume"))
		{
			SetVolume(PlayerPrefs.GetFloat("MusicManagerVolume"));
		}
		else
			SetVolume(defaultVolume);
	}
	
	/// <summary>
	/// Search all scene AudioSources and cache them to improve searchs performance.
	/// </summary>
	public void PrepareAudioSources()
	{
		cachedAudioSources.Clear();
		
		// Search for active audiosources in the scene
		foreach (Object source in GameObject.FindObjectsOfType(typeof(AudioSource)))
		{
			if(source.name != "MusicManager")
			{
				(source as AudioSource).volume *= volume;
				cachedAudioSources.Add(source as AudioSource);
			}
		}
	}
	
	#region 2D methods
	
	/// <summary>
	/// Play a sound in a free AudioSource. Other older sounds can be stopped using this method.
	/// </summary>
	/// <param name="clip">
	/// Clip to play.
	/// </param>
	public void Play2DSound(AudioClip _clip)
	{
		if(!gameObject.activeSelf)
			return;
		
		PlaySound(_clip, 1f, 0f, 0f, 0f, null, Vector3.zero);
	}
	
	/// <summary>
	/// Play a sound in a free AudioSource. Other older sounds can be stopped using this method.
	/// </summary>
	/// <param name="clip">
	/// Clip to play.
	/// </param>
	/// <param name="pitch">
	/// Clip pitch.
	/// </param>
	/// <param name="delay">
	/// Delay before play the sound in seconds.
	/// </param>
	public void Play2DSound(AudioClip _clip, float _pitch, float _delay, float _volume = 1.0f)
	{
		if(!gameObject.activeSelf)
			return;
		
		if(_delay > 0f)
		{
			StartCoroutine(PlaySoundCoroutine(_clip, _pitch, _delay, 0f, 0f, null, Vector3.zero,_volume));
		}
		else
		{
			PlaySound(_clip, _pitch,0f, 0f, 0f, null, Vector3.zero,_volume);
		}
	}
	
	#endregion
	
	#region 3D methods
	
	/// <summary>
	/// Play a sound in a free AudioSource. 
	/// </summary>
	/// <param name="clip">
	/// Clip to play.
	/// </param>
	/// <param name ="source">
	/// Source where play the sound. Use one if you want to play a localized sound in 3D space or you want to be sure no sound stop playing.
	/// </param>
	public void Play3DSound(AudioClip _clip, AudioSource _source)
	{
		if (_source == null)
			Debug.LogError("An AudioSource is needed if you want to play a sound localized in 3D space or have a own AudioSource to avoid sound overlapping");
		
		if(!gameObject.activeSelf)
			return;
		
		PlaySound(_clip, _source.pitch, 0f, _source.maxDistance, _source.minDistance, _source, Vector3.zero);
	}
	
	/// <summary>
	/// Play a sound in a free AudioSource. Other older sounds can be stopped using this method.
	/// </summary>
	/// <param name="clip">
	/// Clip to play.
	/// </param>
	/// <param name="pitch">
	/// Clip pitch.
	/// </param>
	/// <param name="delay">
	/// Delay before play the sound in seconds.
	/// </param>
	/// <param name ="source">
	/// Source where play the sound. Use one if you want to play a localized sound in 3D space or you want to be sure no sound stop playing.
	/// </param>
	public void Play3DSound(AudioClip _clip, float _delay, AudioSource _source)
	{
		if (_source == null)
			Debug.LogError("An AudioSource is needed if you want to play a sound localized in 3D space or have a own AudioSource to avoid sound overlapping");
		
		if(!gameObject.activeSelf)
			return;
		
		if(_delay > 0f)
			StartCoroutine(PlaySoundCoroutine(_clip, _source.pitch, _delay, _source.maxDistance, _source.minDistance, _source, Vector3.zero));
		else
			PlaySound(_clip, _source.pitch, 0f, _source.maxDistance, _source.minDistance, _source, Vector3.zero);
	}
	
	public void Play3DSound(AudioClip _clip, float _maxVolumeDistance, float _minVolumeDistance, Vector3 _location)
	{	
		if(!gameObject.activeSelf)
			return;
		
		PlaySound(_clip, 1f, 0f, _maxVolumeDistance, _minVolumeDistance, null, _location);
	}
	
	public void Play3DSound(AudioClip _clip, float _delay, float _maxVolumeDistance, float _minVolumeDistance, Vector3 _location)
	{	
		if(!gameObject.activeSelf)
			return;
		
		if(_delay > 0f)
			StartCoroutine(PlaySoundCoroutine(_clip, 1f, _delay, _maxVolumeDistance, _minVolumeDistance, null, _location));
		else
			PlaySoundCoroutine(_clip, 1f, 0f, _maxVolumeDistance, _minVolumeDistance, null, _location);
	}
	
	#endregion
	
	/// <summary>
	/// Stop current fades and set the volume and pitch
	/// </summary>
	public void ForceSet(float _volume,float _pitch)
	{
		//iTween.Stop(gameObject);
		SetVolume(_volume);
		SetPitch(_pitch);
	}
	
	/// <summary>
	/// Set sound manager play volume.
	/// </summary>
	/// <param name="newVolume">
	/// New volume between 0 and 1
	/// </param>
	public void SetVolume(float _volume)
	{
		volume = _volume;
		
		if(cachedAudioSources.Count == 0)
			Debug.LogError("You must call PrepareAudioSources int he Awake or Start of your script before work with MusicManager");
		
		for (int i = 0; i < cachedAudioSources.Count; i++)
		{
			AudioSource source = cachedAudioSources[i];
			
			if(source == null)
			{
				cachedAudioSources.Remove(source);
				i--;
			}
			else
			{
				if(_volume == 0)
					source.Stop();
				else
				{
					if(!source.isPlaying && source.playOnAwake && source.gameObject.activeSelf)
					{
						source.Play();
					}
					
					source.volume = _volume;
				}
			}
		}
	}
	
	/// <summary>
	/// Change volume with a time.
	/// </summary>
	/// <param name="newVolume">
	/// New volume between 0 and 1
	/// </param>
	public void SetVolume(float _volume, float _time)
	{
		//iTween.ValueTo(gameObject,iTween.Hash("time",_time,"onupdate","SetVolume","from",volume,"to",_volume,"ignoretimescale",true));
	}
	
	public void SetPitch(float _pitch)
	{
		pitch = _pitch;
		
		if(cachedAudioSources.Count == 0)
			Debug.LogError("You must call PrepareAudioSources int he Awake or Start of your script before work with MusicManager");
		
		// Search for active audiosources in the scene
		for (int i = 0; i < cachedAudioSources.Count; i++)
		{
			AudioSource source = cachedAudioSources[i];
			
			if(source == null)
			{
				cachedAudioSources.Remove(source);
				i--;
			}
			else
			{
				source.pitch = pitch;
			}
		}
	}
	
	/// <summary>
	/// Change volume pitch a time.
	/// </summary>
	/// <param name="newVolume">
	/// New pitch between 0 and 1
	/// </param>
	public void SetPitch(float _from,float _to, float _time, float _delay = 0f, bool _ignoreTimeScale = false)
	{
		//iTween.ValueTo(gameObject,iTween.Hash("time",_time,"onupdate","SetPitch","from",_from,"to",_to,"delay",_delay,"ignoretimescale",_ignoreTimeScale));
	}
	
	public void FreeResources()
	{
		audioSourcePool.FreeResources();
	}
	
	public void SaveVolume(float _newVolume)
	{
		PlayerPrefs.SetFloat("MusicManagerVolume",_newVolume);	
		SetVolume(_newVolume);
	}
	
	#region MAIN METHOD
	
	/// <summary>
	/// Play a sound. If audioSource is null, we get the next AudioSource from the pool.
	/// </summary>
	private IEnumerator PlaySoundCoroutine(AudioClip _clip, float _pitch, float _delay, float _maxVolumeDistance, float _minVolumeDistance, AudioSource _audioSource, Vector3 _location, float _volume = 1.0f)
	{
		if(_clip == null)
			yield break;
		
		yield return new WaitForSeconds(_delay);
		
		PlaySound(_clip,_pitch,_delay,_maxVolumeDistance,_minVolumeDistance,_audioSource,_location,_volume);
	}
	
	private void PlaySound(AudioClip _clip, float _pitch, float _delay, float _maxVolumeDistance, float _minVolumeDistance, AudioSource _audioSource, Vector3 _location, float _volume = 1.0f)
	{
		if(_audioSource == null)
		{
			_audioSource = audioSourcePool.getAudioSource();
			
			_audioSource.playOnAwake 	= false;
			_audioSource.loop 			= repeat;
			_audioSource.rolloffMode 	= AudioRolloffMode.Linear;
			_audioSource.volume 		= _volume;
		}
		else
		{
			if(!cachedAudioSources.Contains(_audioSource))
				cachedAudioSources.Add(_audioSource);
		}
		
		if(_audioSource!=null)
		{
			_audioSource.enabled = true;
			
			if(_location != Vector3.zero)
			{
				_audioSource.transform.position = _location;
			}
			
			if(_audioSource.isPlaying)
				_audioSource.Stop();
			
			_audioSource.clip  			= _clip;
			_audioSource.pitch 			= _pitch * pitch;
			_audioSource.minDistance 	= _minVolumeDistance;
			_audioSource.maxDistance 	= _maxVolumeDistance;
			_audioSource.volume 		= _volume;
			
			if(_audioSource.gameObject.activeSelf)
				_audioSource.Play();
			
			//_audioSource.Play((ulong)(44100 / _clip.frequency * _clip.frequency * _delay));	
		}
	}
	
	//	void ChangeVolume(float _newVolume)
	//	{
	//		SetVolume(_newVolume);
	//	}
	
	#endregion
}

