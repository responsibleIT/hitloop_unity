using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BeatIndicator : MonoBehaviour
{
    [SerializeField] private Image image;
    [SerializeField] private TextMeshProUGUI text;

    private bool on;
    internal bool On { 
        get { return on; }
        set { 
            on = value;
            image.color = on ? GameManager.instance.ballMenu.beatImageOnColor 
                : GameManager.instance.ballMenu.beatImageOffColor;
        }
    }

    internal void Initialize(int index)
    {
        text.text = index.ToString();
    }

    public void ToggleOn()
    {
        On = !On;
    }

    public void SetOn(bool on)
    {
        On = on;
    }
}
