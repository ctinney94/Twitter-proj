using UnityEngine;
using System.Collections;

public class moodSounds : MonoBehaviour {

    public ParticleSystem ps;
    public AudioSource audioSrc;
    public AudioClip[] birthSounds,deathSounds;
    int oldParticleCount;
    public float soundProbability=1;

    void Awake()
    {
        soundManager.instance.managedComponents.Add(audioSrc);
    }

	void Update ()
    {
        particleSounds();
    }

    void particleSounds()
    {
        int newCount = ps.particleCount;
        if (newCount < oldParticleCount && deathSounds.Length > 0)
        {
            if (Random.value >= 1 - soundProbability)
                audioSrc.PlayOneShot(deathSounds[Random.Range(0, deathSounds.Length - 1)]);
        }
        else if (newCount > oldParticleCount && birthSounds.Length > 0)
        {
            if (Random.value >= 1 - soundProbability)
                audioSrc.PlayOneShot(birthSounds[Random.Range(0, birthSounds.Length - 1)]);
        }
        oldParticleCount = newCount;
    }
}
