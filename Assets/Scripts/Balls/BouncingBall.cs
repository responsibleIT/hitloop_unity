using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
[RequireComponent(typeof(Renderer))]
[RequireComponent(typeof(Rigidbody))]
public class BouncingBall : MonoBehaviour
{
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private new Renderer renderer;
    [SerializeField] private Rigidbody rigidbody;

    private BeatSetting beatSetting;
    private ClipSetting clipSetting;

    private float startY;
    private float bounceHeightY;

    private float pitch;

    internal void Initialize(int clipSettingIndex, float pitch, BeatSetting beatSetting)
    {
        this.beatSetting = beatSetting;
        this.clipSetting = GameManager.instance.clipSettings[clipSettingIndex];
        this.pitch = pitch;

        audioSource.clip = clipSetting.clip;
        audioSource.pitch = pitch;

        renderer.material.color = clipSetting.color;

        rigidbody.mass = GameManager.instance.GetBallMass(pitch);

        transform.localScale = GameManager.instance.GetBallScaleVector(pitch);
        transform.localRotation = Quaternion.identity;

        gameObject.SetActive(true);
    }

    internal void SetPosition(Vector3 spawnPosition)
    {
        transform.localPosition = spawnPosition + new Vector3(0, transform.localScale.y * 0.5f, 0);

        startY = transform.localPosition.y;
        bounceHeightY = startY + GameManager.instance.GetBounceHeight(pitch);
    }

    internal void PlayOnce()
    {
        audioSource.Play();
    }

    internal void PlayOnceIf()
    {
        if (beatSetting.beatsInOneMeasure[GameManager.instance.beat])
            audioSource.Play();
    }

    internal static void Clear(BouncingBall b) 
    {
        b.audioSource.clip = null;
        b.audioSource.pitch = 1f;
        b.renderer.material.color = Color.white;

        b.gameObject.SetActive(false);
    }

    private void Update()
    {
        if (!GameManager.instance.playing)
            return;

        // if it's my beat, bounce
        // else, wait it out
        if (beatSetting.beatsInOneMeasure[GameManager.instance.nextBeat])
        {
            transform.localPosition = new Vector3(transform.localPosition.x,
                Mathf.Lerp(startY, bounceHeightY, Mathf.Sin(GameManager.instance.GetRemappedBeatTime() * Mathf.PI)),
                transform.localPosition.z);
        }
    }
}
