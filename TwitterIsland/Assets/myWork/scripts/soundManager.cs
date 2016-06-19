using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class soundManager : MonoBehaviour {

    #region Singleton
    private static soundManager m_instance;
    public static soundManager instance
    {
        get { return m_instance; }
    }
    #endregion
    AudioSource audioComponent;
    public List<AudioSource> managedComponents = new List<AudioSource>();
    public float maxVolume=1;

    public void playSound(AudioClip sound)
    {
        audioComponent.PlayOneShot(sound);
    }

    void Update()
    {
        if (m_instance == null)
            m_instance = this;
    }

    void Start ()
    {
        audioComponent = GetComponent<AudioSource>();
        m_instance = this;
	}

    public void setNewVolume(float newVol)
    {
        maxVolume = newVol;
        for (int i=0; i < managedComponents.Count;i++)
        { 
            managedComponents[i].volume = newVol;
        }
        audioComponent.volume = newVol;
    }
}
