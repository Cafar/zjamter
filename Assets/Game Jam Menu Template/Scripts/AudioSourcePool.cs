using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AudioSourcePool
{
	private LinkedList< AudioSource > audioSources;
	
	public AudioSourcePool(int _maxsounds, GameObject _parent, bool createAsSeparateGameObjects)
	{
		/// <summary>
		/// List with audioSources. Ordered from more recently played to old ones.
		/// </summary>	
		audioSources = new LinkedList< AudioSource >();
	
		//Initialize pool of audiosources creating a number of audioSources in the scene
		for (int i = 0; i < _maxsounds; i++)
		{		
			GameObject aux;
			if(createAsSeparateGameObjects)
			{
				aux = new GameObject("AudioSource" + i);
				aux.transform.position = _parent.transform.position;
				aux.transform.parent = _parent.transform;
			}
			else
			{
				aux = _parent;
			}
			AudioSource source = aux.AddComponent(typeof(AudioSource)) as AudioSource;
			audioSources.AddLast(source);	
		}
	}
	
	
	/// <summary>
	/// Return a AudioSource from the pool.
	/// </summary>
	/// <returns>
	/// Return a free AudioSource if there is at least one free, or the oldest used AudioSource.
	/// </returns>
	public AudioSource getAudioSource()
	{
		AudioSource freeAudioSource = null;
		
		// Search for an available AudioSource
		foreach (AudioSource source in audioSources)
		{
			if (!source.isPlaying)
			{
				freeAudioSource = source;
				break;
			}
		}
		
		// Search for an available AudioSource wich is not looping
		foreach (AudioSource source in audioSources)
		{
			if (!source.loop)
			{
				freeAudioSource = source;
				break;
			}
		}
		
		
		// Case where all AudioSources are playing a sound.
		// We get the oldest 
		if(freeAudioSource == null)
			freeAudioSource = audioSources.First.Value;
		
		// Remove Component from its list position
		audioSources.Remove(freeAudioSource);
		
		// Insert Component at the end of the list
		audioSources.AddLast(freeAudioSource);
		
		
		return freeAudioSource;
	}
	
	/// <summary>
	/// Set sound manager play volume.
	/// </summary>
	/// <param name="newVolume">
	/// New volume between 0 and 1
	/// </param>
	public void setVolume(float newVolume)
	{
		
	}
	
	
	public void FreeResources()
	{
		foreach (AudioSource source in this.audioSources)
		{
			source.clip = null;	
		}
	}
}
