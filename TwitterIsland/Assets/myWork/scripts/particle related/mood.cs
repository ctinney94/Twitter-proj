using UnityEngine;
using System.Collections;

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
        if (Camera.main != null)
        {
            int currIslnd = Camera.main.GetComponent<cameraOrbitControls>().currentIsland;
            int maxIsland = Camera.main.GetComponent<cameraOrbitControls>().islands.Count;
            if (flag.name.EndsWith(" " + currIslnd))
            {
                UpdateParticles(transform.localScale.x * 7);

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


    public void UpdateParticles(float scale)
    {
        transform.localScale = new Vector3(scale / 7, scale / 7, scale / 7);
        if (moodness > 0)
        {
            if (flag != null)
            {
                fireworks.transform.position = flag.transform.position;

                rainbowA.transform.position = flag.transform.position + new Vector3(-3.75f, -0, -0);
                rainbowB.transform.position = flag.transform.position + new Vector3(-7.5f, -0, -0);
                rainbowC.transform.position = flag.transform.position + new Vector3(-15f, -.5f, -0);
                rainbowD.transform.position = flag.transform.position + new Vector3(-22.5f, -1, -0);
            }

            if (moodness > 0.8f)
                fireworks.enableEmission = true;
            else
                fireworks.enableEmission = false;

            if (moodness > 0.4f)
            {
                if (scale < 4)
                    rainbowA.enableEmission = true;
                else if (scale < 8)
                    rainbowB.enableEmission = true;
                else if (scale < 12)
                    rainbowC.enableEmission = true;
                else if (scale < 16)
                    rainbowD.enableEmission = true;
            }
            else
            {
                rainbowA.enableEmission = false;
                rainbowB.enableEmission = false;
                rainbowC.enableEmission = false;
                rainbowD.enableEmission = false;
            }

            cloud.emissionRate = 0;
            lightning.emissionRate = 0;
            rain.emissionRate = 0;
        }
        else
        {
            fireworks.enableEmission = false;
            rainbowA.enableEmission = false;
            rainbowB.enableEmission = false;
            rainbowC.enableEmission = false;
            rainbowD.enableEmission = false;
            cloud.enableEmission = true;
            lightning.enableEmission = true;
            rain.enableEmission = true;

            var temp = -1 * moodness;//turn this back into a positive so maths can happen.

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
