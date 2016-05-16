using UnityEngine;
using System.Collections;

public class soundManager : MonoBehaviour {

    #region Singleton
    private static soundManager m_instance;
    public static soundManager instance
    {
        get { return m_instance; }
    }
    #endregion
    AudioSource audioComponent;

    public void playSound(AudioClip sound)
    {
        audioComponent.PlayOneShot(sound);
    }

    void Start ()
    {
        audioComponent = GetComponent<AudioSource>();
        m_instance = this;
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
