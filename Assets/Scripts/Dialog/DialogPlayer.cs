using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using StringUtility;
using DataParseAndPlayer;

public class DialogPlayer : DataPlayer<DialogRow>
{
    private static readonly string FName = "엘리";
    private static readonly string MName = "데이브";

    private static readonly string FPicPath = "Images/Dialog/F{0}";
    private static readonly string MPicPath = "Images/Dialog/M{0}";

    [SerializeField] private Image pic1;
    [SerializeField] private Image pic2;
    [SerializeField] private TextMeshProUGUI text;
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private float textShowIntervalSpeed = 0.1f;
    private WaitForSeconds wfTextInterval = null;

    protected override void Init()
    {
        wfTextInterval = new WaitForSeconds(textShowIntervalSpeed);
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
            nameText.text = FName;
            pic2.color = Color.gray;
            pic1.color = Color.white;
        }
        else
        {
            nameText.text = MName;
            pic1.color = Color.gray;
            pic2.color = Color.white;
        }

        // 텍스트 준비
        text.text = string.Empty;
    }

    protected override void ShowText()
    {
        StartCoroutine(Typing.CoTextTyping(
            _sceneInfo[_currentRow].Text, text, wfTextInterval));
    }

    protected override void OnSceneEnd()
    {
        base.OnSceneEnd();

        panel.SetActive(false);

        InputManager.Instance.HideCursor();
        InputManager.Instance.SetInputs(true);
    }
}
