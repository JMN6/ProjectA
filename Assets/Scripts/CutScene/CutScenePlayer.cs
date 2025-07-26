using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using StringUtility;
using System;
using DataParseAndPlayer;

public class CutScenePlayer : DataPlayer<CutSceneRow>
{
    private static readonly string PicPath = "Images/CutScene/{0}";

    [SerializeField] private Image bg;
    [SerializeField] private TextMeshProUGUI text;
    [SerializeField] private RectTransform textPosition;

    [SerializeField] private float textShowIntervalSpeed = 0.1f;
    private WaitForSeconds wfTextInterval = null;
    
    [SerializeField] private float moveToNextCutSpeed = 1.5f;
    private WaitForSeconds wfNextCut = null;

    protected override void Init()
    {
        wfTextInterval = new WaitForSeconds(textShowIntervalSpeed);
        wfNextCut = new WaitForSeconds(moveToNextCutSpeed);
    }

    protected override void SetSceneInfo(CutSceneRow info)
    {
        // 배경 변경
        Sprite bgSprite = Resources.Load<Sprite>(string.Format(PicPath, info.Pic));
        bg.sprite = bgSprite;

        // 텍스트 준비
        textPosition.anchoredPosition = new Vector2(info.XPos, -info.YPos);
        text.text = string.Empty;
        text.alignment = info.Align == 0 ? TextAlignmentOptions.Center :
            info.Align == -1 ? TextAlignmentOptions.Left : TextAlignmentOptions.Right;
        text.fontSize = info.FontSize;
    }

    protected override void ShowText()
    {
        StartCoroutine(Typing.CoTextTyping(
            _sceneInfo[_currentRow].Text, text, wfTextInterval, OnShowNextScene));
    }

    protected override void OnSceneEnd()
    {
        base.OnSceneEnd();

        InputManager.Instance.HideCursor();
    }

    public override void OnShowNextScene()
    {
        StartCoroutine(CoShowNext());
    }

    private IEnumerator CoShowNext()
    {
        yield return wfNextCut;
        if (++_currentRow >= _sceneInfo.Count)
        {
            OnSceneEnd();
            yield break;
        }
        SetScene(_currentRow);
    }
}
