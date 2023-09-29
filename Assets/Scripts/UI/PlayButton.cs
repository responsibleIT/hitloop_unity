using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayButton : MonoBehaviour
{
    [SerializeField] private GameObject iconOn;
    [SerializeField] private GameObject iconOff;

    private bool on;
    private bool On { get { return on; } set { if (on != value) iconOn.SetActive(value); iconOff.SetActive(!value); on = value; } }

    public void OnClick()
    {
        On = !On;
        GameManager.instance.Play(On);
    }

    private void Update()
    {
        // keep in synch w/ GM
        // in case someone else changed the play status
        if (GameManager.instance.playing != On)
            On = !On;
    }
}
