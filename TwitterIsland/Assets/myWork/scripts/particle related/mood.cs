using UnityEngine;
using System.Collections;

/// <summary>
/// Manages the mood particle systems for a given island
/// </summary>

public class mood : MonoBehaviour
{
    [Range(-1, 1)]
    public float moodness = 0;

    [Range(0, 0.85f)]
    public float blackness = 0;
    public ParticleSystem rain, lightning, cloud, fireworks;
    public ParticleSystem rainbowA, rainbowB, rainbowC, rainbowD;
    public GameObject flag;
    lighting worldLight;

    void Awake()
    {
        worldLight = GameObject.Find("WorldLight").GetComponent<lighting>();
    }

    void Update()
    {
        //If the camera is looking at the island associated with these particles
        //Or any island to the left or right
        if (Camera.main != null)
        {
            int currIslnd = Camera.main.GetComponent<cameraOrbitControls>().currentIsland;
            int maxIsland = Camera.main.GetComponent<cameraOrbitControls>().islands.Count;
            if (flag.name.EndsWith(" " + currIslnd))
            {
                UpdateParticles(transform.localScale.x * 7);

                //Update shadow strength for lighting
                //Create dark shadows for sad island with clouds
                //No shadows for happy island!
                if (moodness > 0)
                    worldLight.newShadowStrength = 0;
                else
                    worldLight.newShadowStrength = Mathf.Clamp(-1 * moodness, 0, .7f);
            }
            else if (flag.name.EndsWith(" " + (currIslnd + 1)))
            {
                UpdateParticles(transform.localScale.x * 7);
            }
            else if (flag.name.EndsWith(" " + (currIslnd - 1)))
            {
                UpdateParticles(transform.localScale.x * 7);
            }
            else
            {
                //Turn everything off
                rain.enableEmission = false;
                lightning.enableEmission = false;
                cloud.enableEmission = false;
                fireworks.enableEmission = false;
                rainbowA.enableEmission = false;
                rainbowB.enableEmission = false;
                rainbowC.enableEmission = false;
                rainbowD.enableEmission = false;
            }
        }
    }
    
    //Enable/disable particle systems as needs be
    public void UpdateParticles(float scale)
    {
        //All this multiplication and division by 7 is not all pointless
        //IslandMaker.cs uses meshScale as an input for this.
        //SO DON'T FUCK IT UP.
        transform.localScale = new Vector3(scale / 7, scale / 7, scale / 7);
        if (moodness > 0)
        {
            if (flag != null)
            {
                //Pop the fireworks up the top of the island
                fireworks.transform.position = flag.transform.position;

                //Rainbows too
                rainbowA.transform.position = flag.transform.position + new Vector3(-3.75f, -.25f, -0);
                rainbowB.transform.position = flag.transform.position + new Vector3(-7.5f, -1.5f, -0);
                rainbowC.transform.position = flag.transform.position + new Vector3(-15f, -2.75f, -0);
                rainbowD.transform.position = flag.transform.position + new Vector3(-22.5f, -4, -0);
            }

            //If we're really happy, enable fireworks!
            if (moodness > 0.75f)
                fireworks.enableEmission = true;
            else
                fireworks.enableEmission = false;

            //If we're happy and we know it ENABLE APPROPRIATE PARTICLE SYSTEM
            if (moodness > 0.25f)
            {
                if (scale < 5)
                    rainbowA.enableEmission = true;
                else if (scale < 10)
                    rainbowB.enableEmission = true;
                else if (scale < 15)
                    rainbowC.enableEmission = true;
                else if (scale > 15)
                    rainbowD.enableEmission = true;
                else
                    Debug.Log(scale);
            }
            else
            {
                rainbowA.enableEmission = false;
                rainbowB.enableEmission = false;
                rainbowC.enableEmission = false;
                rainbowD.enableEmission = false;
            }

            //This is no time to be down, turn of all the clouds n that
            cloud.emissionRate = 0;
            lightning.emissionRate = 0;
            rain.emissionRate = 0;
        }
        else
        {
            //NO HAPPY PARTICLES ALLOWED.
            fireworks.enableEmission = false;
            rainbowA.enableEmission = false;
            rainbowB.enableEmission = false;
            rainbowC.enableEmission = false;
            rainbowD.enableEmission = false;
            cloud.enableEmission = true;
            lightning.enableEmission = true;
            rain.enableEmission = true;

            var temp = -1 * moodness;//turn this back into a positive so maths can happen.

            //Scale particles system depending on mood.
            //Darker clouds, more rain, clouds and lightning for the more negative tweets
            Color cloudClour = Color.Lerp(Color.white, Color.black, temp * 0.85f);
            cloud.GetComponent<Renderer>().material.SetColor("_TintColor", cloudClour);
   
            rain.emissionRate = (10 * (temp - 0.5f)) + (scale * scale * 2 * (temp - 0.5f));
            lightning.emissionRate = scale * (temp - 0.6f);
            cloud.emissionRate = scale * scale * temp;
        }
    }
    
    public void move(Vector3 newPos)
    {
        gameObject.transform.position = newPos + new Vector3(0, 17.5f, 0);
    }
    public void setMood(float mood)
    {
        moodness = mood;
    }

}
