using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using StringUtility;
using DataParseAndPlayer;
using Game.Input;
using Assets.Scripts.Managers;

public class DialogPlayer : DataPlayer<DialogRow>
{
    private static readonly string FNameID = "CHARACTER_FEMALE";
    private static readonly string MNameID = "CHARACTER_MALE";

    private static readonly string FPicPath = "Images/Dialog/F{0}";
    private static readonly string MPicPath = "Images/Dialog/M{0}";

    [Header("Image")]
    [SerializeField] private Image pic1;
    [SerializeField] private Image pic2;

    [Header("Text")]
    [SerializeField] private TextMeshProUGUI text;
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private float textShowIntervalSpeed = 0.1f;
    private WaitForSeconds wfTextInterval = null;

    [Header("Btn")]
    [SerializeField] private GameObject nextBtn;

    private bool isTextOver = false;

    protected override void Init()
    {
        base.Init();

        isTextOver = false;
        wfTextInterval = new WaitForSeconds(textShowIntervalSpeed);

        LanguageManager.Instance.SetTextFont(text);
        LanguageManager.Instance.SetTextFont(nameText);
    }

    private void OnEnable()
    {
        InputManager.Instance.SetInputEnabled(Maps.UI, true);
        InputManager.Instance.AddListener(Game.Input.Actions.Skip, Game.Input.TriggerFlag.Started, OnSkip);
    }

    private void OnDisable()
    {
        InputManager.Instance.SetInputEnabled(Maps.UI, false);
        InputManager.Instance.RemoveListener(Game.Input.Actions.Skip, Game.Input.TriggerFlag.Started, OnSkip);
    }

    protected override void SetSceneInfo(DialogRow info)
    {
        // 사진 변경
        Sprite pic1Sprite = Resources.Load<Sprite>(string.Format(FPicPath, info.Pic1Number));
        Sprite pic2Sprite = Resources.Load<Sprite>(string.Format(MPicPath, info.Pic2Number));

        pic1.sprite = pic1Sprite;
        pic2.sprite = pic2Sprite;

        if (info.Talker == 0)
        {
            nameText.text = LanguageManager.Instance.GetString(FNameID);
            pic2.color = Color.gray;
            pic1.color = Color.white;
        }
        else
        {
            nameText.text = LanguageManager.Instance.GetString(MNameID);
            pic1.color = Color.gray;
            pic2.color = Color.white;
        }

        // 텍스트 준비
        text.text = string.Empty;

        nextBtn.SetActive(false);
    }

    protected override void ShowText()
    {
        string dialogueText = LanguageManager.Instance.GetString(_sceneInfo[_currentRow].TextID);
        _textShowCoroutine = StartCoroutine(Typing.CoTextTyping(
            dialogueText, text, wfTextInterval, OnTextEnd));
        isTextOver = false;
    }

    private void OnTextEnd()
    {
        nextBtn.SetActive(true);
        isTextOver = true;
    }

    protected override void OnSceneEnd()
    {
        base.OnSceneEnd();

        panel.SetActive(false);

        InputManager.Instance.HideCursor();
        InputManager.Instance.SetInputs(true);
    }

    protected void OnSkip(UnityEngine.InputSystem.InputAction.CallbackContext ctx)
    {
        if(isTextOver == false)
        {
            StopText();
            text.text = LanguageManager.Instance.GetString(_sceneInfo[_currentRow].TextID);
            OnTextEnd();
        }
        else
        {
            OnShowNextScene();
        }
    }
}
