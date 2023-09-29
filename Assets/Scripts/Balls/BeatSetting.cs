using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct BeatSetting 
{
    public bool[] beatsInOneMeasure;

    public BeatSetting(params bool[] beats) 
    {
        beatsInOneMeasure = beats;
    }
}
