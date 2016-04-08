using UnityEngine;
using System.Collections;

public class poof : MonoBehaviour
{
    ParticleSystem ps;

    void Start()
    {
        ps = GetComponent<ParticleSystem>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!ps.IsAlive())
        {
            //If particle system has finished playing and is longer alive, destroy this game object.
            Destroy(gameObject);
        }
    }
}
