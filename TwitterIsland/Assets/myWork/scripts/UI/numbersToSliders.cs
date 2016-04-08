using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class numbersToSliders : MonoBehaviour
{
    //Not currently used
    //public Slider scaleSlider, heightSlider;
    //public InputField rtsInput;

    //The number of likes or re-tweets required for rank
    //For information on how this system was created, please consult the research report associated with this project
    int[] rankings =
        {
            0,
            3,
            6,
            10,
            19,
            35,
            65,
            120,
            222,
            412,
            761,
            1409,
            2606,
            4821,
            8920,
            16502,
            30528,
            56477,
            104482,
            193291,
            357589,
            661540,
            1223848,
            2264119,
            4188620,
        };

    int max = 0;

    //Find the rank of the given value
    public float findRank(float num, bool height)
    {
        //If this is a height value
        if (height)
            if (max != 0)//And if we want to maximum height value
                num = max;//Set height value to max

        //If value is greater than max, clamp it to max
        if (num > 4188620)
            num = 4188620;

        int rank = 0;

        //Find rank in integer form
        for (int i = 0; i < rankings.Length - 1; i++)
        {
            if (num >= rankings[i])
                rank++;
        }
        //Find decimal places for rank to add to previously obtained value, then return that value
        return rank + ((num - rankings[rank - 1]) / (rankings[rank] - rankings[rank - 1]));
    }

    public float RTs(int RTs)
    {
        //Find rank from number of re-tweets
        float j = findRank((float)(RTs), false);

        //Some number fudging to get the kind of range I want
        j /= 25;
        j *= 50;
        return j;
    }
    public float favs(int favs)
    {
        //Find rank from number of favourites
        float j = findRank((float)favs, true);
        //Some number fudging to get the kind of range I want
        j /= 25;
        j *= 1;
        return j;
    }

    //Toggle maximum height values
    public void setMax(bool b)
    {
        if (b)
            max = 4188620;
        else
            max = 0;
    }

    #region  A whole bunch of functions which are no longer used

    /*
    public void favs(string favs)
    {
        if (favs != "")
        {
            float j = findRank(float.Parse(favs),true);
            j /= 25;
            j *= heightSlider.maxValue;
            heightSlider.value = j;
        }
    }
    
    public void RTs(string RTs)
    {
        if (RTs != "")
        {
            float j = findRank(float.Parse(RTs),false);
            j /= 25;
            j *= scaleSlider.maxValue;
            scaleSlider.value = j;
            //GameObject.Find("PentTest").GetComponent<PentInfo>().UpdatePentMesh();
        }
    }

    public void scaleToRTs(float scale)
    {
        scale /= scaleSlider.maxValue;
        scale *= 25;
        var rem = scale % 1;

        if ((int)scale-1 < 0)
        {
            rtsInput.text = "0";
        } 
        else if (scale > 24)
        {
            float RTs = rankings[(int)scale - 1];
            rtsInput.text = "" + (int)RTs;
        }
        else
        {
            scale -= rem;
            float RTs = rankings[(int)scale - 1] + (rem * (rankings[(int)scale] - rankings[(int)scale - 1]));
            rtsInput.text = "" + (int)RTs;
        }
    }

    public void heightToFavs(float favs)
    {
        favs /= heightSlider.maxValue;
        favs *= 25;
        var rem = favs % 1;
        favs -= rem;

        if ((int)favs - 1 < 0)
        { 
            GetComponent<InputField>().text = "0";
        }
        else if (favs > 24)
        {
            float favser = rankings[(int)favs - 1];
            GetComponent<InputField>().text = "" + (int)favser;
        }
        else
        {
            float favser = rankings[(int)favs - 1] + (rem * (rankings[(int)favs] - rankings[(int)favs - 1]));
            GetComponent<InputField>().text = "" + (int)favser;
        }
    }*/
    #endregion
}
