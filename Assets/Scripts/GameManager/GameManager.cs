using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [Serializable] private enum SpawnMethod { Line, Circle }

    [Header("Scene References")]
    [SerializeField] internal Transform ballParent;
    [SerializeField] internal Transform previewBallParent;
    [SerializeField] internal CameraController mainCamera;
    [SerializeField] internal PreviewCamera previewCamera;

    [Header("UI References")]
    [SerializeField] internal Canvas canvas;
    [SerializeField] internal RectTransform canvasRect;
    [SerializeField] internal BallMenu ballMenu;
    [SerializeField] internal Button openBallMenuButton;
    [SerializeField] private Transform beatIndicatorsParent;
    [SerializeField] private GameObject beatIndicatorPrefab;
    private BeatIndicator[] beatIndicators;

    [Header("Ball Settings")]
    [SerializeField] internal float minBallMass = 1f;
    [SerializeField] internal float maxBallMass = 10f;
    [SerializeField] internal float minBallScale = 0.5f;
    [SerializeField] internal float maxBallScale = 10f;
    [SerializeField] internal float minPitch = -3f;
    [SerializeField] internal float maxPitch = 3f;
    [SerializeField] internal float minBounceY = 5f;
    [SerializeField] internal float maxBounceY = 20f;

    [Header("Sound Settings")]
    [SerializeField] internal int bpm = 120;
    [SerializeField] internal int beatsPerTime = 4;
    [SerializeField] internal ClipSetting[] clipSettings;

    [Header("Spawn Settings")]
    [SerializeField] private SpawnMethod spawnMethod;
    [SerializeField] private int maxBalls;
    [Header("Spawn in Line Settings")]
    [SerializeField] private Vector3 startSpawnPositionLine;
    [SerializeField] private float spawnSpacingLine;
    [Header("Spawn in Circle Settings")]
    [SerializeField] private Vector3 centerSpawnPositionCircle;
    [SerializeField] private float spawnSpacingCircle;
    [SerializeField] private float spawnRadiusCircle;

    // bouncing ball prefab
    [SerializeField] internal GameObject ballPrefab;

    // bouncing balls
    internal ObjectPool<BouncingBall> ballPool;

    // keep track of playing & beats
    internal bool playing;

    // time beats
    private float beatTime;
    private float beatHalfTime;
    private float beatTimer;

    // counts amount of beats passed total
    private int beatAmount;
    internal int beat { get { return beatAmount % beatsPerTime; } }
    internal int nextBeat { get { if (beat + 1 >= beatsPerTime) return 0; else return beat + 1; } }

    // singleton 
    internal static GameManager instance;

    // constants 
    private const float SECONDS_IN_MINUTE = 60f;

    private void Awake()
    {
        // lazy init for singleton
        if (instance != null)
            Destroy(instance.gameObject);
        instance = this;

        // inits 
        ballPool = new ObjectPool<BouncingBall>(ballPrefab, null, BouncingBall.Clear, ballParent);
        previewCamera.Initialize();
        ballMenu.Initialize();
        mainCamera.Initialize(Vector3.zero);

        beatIndicators = new BeatIndicator[beatsPerTime];
        for (int i = 0; i < beatIndicators.Length; i++)
        {
            beatIndicators[i] = GameObject.Instantiate(beatIndicatorPrefab, beatIndicatorsParent).GetComponent<BeatIndicator>();
            beatIndicators[i].Initialize(i + 1);
            beatIndicators[i].SetOn(false);
        }

        // calculate beats
        beatTime = SECONDS_IN_MINUTE / bpm;
        beatHalfTime = beatTime * 0.5f;
        beatAmount = 0;

        // highlight current beat indicator
        for (int i = 0; i < beatIndicators.Length; i++)
        {
            beatIndicators[i].SetOn(beat == i);
        }
    }

    private void Update()
    {
        if (!playing)
            return;

        // increase timer
        beatTimer += Time.deltaTime;

        // if one beat has passed
        if(beatTimer >= beatTime)
        {
            beatAmount++;
            beatTimer -= beatTime;

            // highlight current beat indicator
            for (int i = 0; i < beatIndicators.Length; i++)
            {
                beatIndicators[i].SetOn(beat == i);
            }

            ballPool.usedObjects.ForEach(b => b.PlayOnceIf());
        }
    }

    internal void Play(bool play)
    {
        playing = play;
    }

    public void OpenBallMenu()
    {
        if (ballPool.usedObjects.Count >= maxBalls)
            return;

        ballMenu.Open();
        Play(false);
    }

    internal void CreateBall(int clipSettingIndex, float pitch, BeatSetting beatSetting)
    {
        BouncingBall b = ballPool.Get();
        b.Initialize(clipSettingIndex, pitch, beatSetting);

        switch (spawnMethod)
        {
            case SpawnMethod.Circle:
                Vector3 pos = centerSpawnPositionCircle;
                pos.x += (spawnRadiusCircle * Mathf.Cos(((ballPool.usedObjects.Count - 1) * spawnSpacingCircle + 180) / (180 / Mathf.PI)));
                pos.z += (spawnRadiusCircle * Mathf.Sin(((ballPool.usedObjects.Count - 1) * spawnSpacingCircle + 180) / (180 / Mathf.PI)));
                b.SetPosition(pos);
                break;
            case SpawnMethod.Line:
                b.SetPosition(startSpawnPositionLine + new Vector3((ballPool.usedObjects.Count - 1) * spawnSpacingLine, 0, 0));
                break;
        }

        openBallMenuButton.interactable = ballPool.usedObjects.Count < maxBalls;
    }

    internal float GetBallScaleScalar(float pitch)
    {
        return Mathf.Lerp(maxBallScale, minBallScale, RemapPitch(pitch));
    }

    internal Vector3 GetBallScaleVector(float pitch)
    {
        float scaleScalar = GetBallScaleScalar(pitch);
        return new Vector3(scaleScalar, scaleScalar, scaleScalar);
    }

    internal float GetBallMass(float pitch)
    {
        return Mathf.Lerp(maxBallMass, minBallMass, RemapPitch(pitch));
    }

    internal float GetBounceHeight(float pitch)
    {
        return Mathf.Lerp(maxBounceY, minBounceY, RemapPitch(pitch));
    }

    internal float RemapPitch(float pitch)
    {
        return (pitch - minPitch) / (maxPitch - minPitch);
    }

    internal float GetRemappedBeatTime()
    {
        return beatTimer / beatTime;
    }

    internal float GetRemappedBeatHalfTime()
    {
        return beatTimer * 0.5f / beatHalfTime;
    }

    internal bool PassedBeatHalfTime()
    {
        return beatTimer > beatHalfTime;
    }
}
