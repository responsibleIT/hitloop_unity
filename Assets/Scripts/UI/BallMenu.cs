using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BallMenu : MonoBehaviour
{
    // ui references
    [SerializeField] private TMP_Dropdown clipDropdown;
    [SerializeField] private Slider pitchSlider;
    [SerializeField] private RectTransform previewRect;
    [SerializeField] private RawImage previewImage;
    [SerializeField] private Transform beatButtonParent;
    [SerializeField] private GameObject beatButtonPrefab;
    private BeatIndicator[] beatButtons;

    // settings
    [SerializeField] internal Color beatImageOffColor;
    [SerializeField] internal Color beatImageOnColor;

    private BouncingBall previewBall; 
    private GameObject previewBallObject;
    private BeatSetting beatSetting;

    private bool initialized = false;

    internal void Initialize()
    {
        List<string> options = new List<string>();
        for (int i = 0; i < GameManager.instance.clipSettings.Length; i++)
            options.Add(GameManager.instance.clipSettings[i].clip.name);
        clipDropdown.ClearOptions();
        clipDropdown.AddOptions(options);

        pitchSlider.minValue = GameManager.instance.minPitch;
        pitchSlider.maxValue = GameManager.instance.maxPitch;

        beatButtons = new BeatIndicator[GameManager.instance.beatsPerTime];
        for (int i = 0; i < beatButtons.Length; i++)
        {
            beatButtons[i] = GameObject.Instantiate(beatButtonPrefab, beatButtonParent).GetComponent<BeatIndicator>();
            beatButtons[i].Initialize(i + 1);
            int index = i;
            beatButtons[i].GetComponent<Button>().onClick.AddListener(delegate { ChangeBeat(index); });
            beatButtons[i].SetOn(false);
        }
            
        beatButtons[0].On = true;

        bool[] beats = new bool[GameManager.instance.beatsPerTime];
        for (int i = 0; i < beats.Length; i++)
            beats[i] = beatButtons[i].On;
        beatSetting = new BeatSetting(beats);

        previewBallObject = GameObject.Instantiate(GameManager.instance.ballPrefab);
        previewBall = previewBallObject.GetComponent<BouncingBall>();
        previewBall.Initialize(0, 1, beatSetting);
        previewBall.transform.SetParent(GameManager.instance.previewBallParent);

        initialized = true;

        Refresh();
        Clear();
    }

    internal void Open()
    {
        Refresh();

        previewBall.Initialize(clipDropdown.value, pitchSlider.value, beatSetting);
        previewBall.transform.localPosition = new Vector3(0, 0, 0);
        GameManager.instance.previewCamera.Activate(previewBall.transform);
        previewBall.transform.localPosition = new Vector3(0, 4.5f, 0);

        gameObject.SetActive(true);
    }

    internal void Close()
    {
        Clear();
        gameObject.SetActive(false);
    }

    private void Clear()
    {
        clipDropdown.value = 0;
        pitchSlider.value = 1;

        for (int i = 0; i < beatButtons.Length; i++)
            beatButtons[i].SetOn(false);
        beatButtons[0].On = true;

        bool[] beats = new bool[GameManager.instance.beatsPerTime];
        for (int i = 0; i < beats.Length; i++)
            beats[i] = beatButtons[i].On;
        beatSetting = new BeatSetting(beats);

        GameManager.instance.previewCamera.Deactivate();
        previewBallObject.SetActive(false);
    }

    private void Refresh()
    {
        if (!initialized)
            return;

        GameManager.instance.previewCamera.widthRT = Mathf.RoundToInt(Mathf.Abs(previewRect.rect.max.x - previewRect.rect.min.x) * GameManager.instance.canvasRect.localScale.x);
        GameManager.instance.previewCamera.heightRT = Mathf.RoundToInt(Mathf.Abs(previewRect.rect.max.y - previewRect.rect.min.y) * GameManager.instance.canvasRect.localScale.x);
        GameManager.instance.previewCamera.RefreshRenderTexture(true);
        previewImage.texture = GameManager.instance.previewCamera.renderTexture;
    }

    private void OnRectTransformDimensionsChange()
    {
        Refresh();
    }

    #region INTERACTIONS
    public void ChangeTrack()
    {
        OnChange();
    }

    public void ChangePitch()
    {
        OnChange();
    }

    public void ChangeBeat(int i)
    {
        beatSetting.beatsInOneMeasure[i] = beatButtons[i].On;
    }

    public void OnChange()
    {
        previewBall.Initialize(clipDropdown.value, pitchSlider.value, beatSetting);
    }

    public void PlaySound()
    {
        previewBall.PlayOnce();
    }

    public void ClickCancel()
    {
        Close();
    }

    public void ClickCreate()
    {
        GameManager.instance.CreateBall(clipDropdown.value, pitchSlider.value, beatSetting);
        Close();
    }
    #endregion
}
