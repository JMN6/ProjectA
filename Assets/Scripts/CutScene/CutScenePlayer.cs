using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using StringUtility;
using System;

public class CutScenePlayer : MonoBehaviour
{
    private static readonly string PicPath = "Images/CutScene/{0}";

    [SerializeField] private GameObject panel;
    [SerializeField] private Image bg;
    [SerializeField] private TextMeshProUGUI text;
    [SerializeField] private RectTransform textPosition;
    [SerializeField] private float textShowIntervalSpeed = 0.1f;
    private WaitForSeconds wfTextInterval = null;
    [SerializeField] private float moveToNextCutSpeed = 1.5f;
    private WaitForSeconds wfNextCut = null;


    private List<CutSceneRow> cutSceneInfo = null;
    private int currentRow = -1;

    private Action callBack = null;

    private void Start()
    {
        wfTextInterval = new WaitForSeconds(textShowIntervalSpeed);
        wfNextCut = new WaitForSeconds(moveToNextCutSpeed);
    }

    public void StartCutScene(in List<CutSceneRow> rows, Action callback = null)
    {
        cutSceneInfo = rows;
        if (cutSceneInfo.Count <= 0)
        {
            return;
        }

        this.callBack = callback;

        panel.SetActive(true);

        SetDialogView(0);
    }

    private void SetDialogView(int rowNum)
    {
        if (rowNum < 0 || rowNum >= cutSceneInfo.Count)
            return;

        currentRow = rowNum;
        CutSceneRow info = cutSceneInfo[currentRow];

        // 배경 변경
        Sprite bgSprite = Resources.Load<Sprite>(string.Format(PicPath, info.Pic));
        bg.sprite = bgSprite;

        // 텍스트 준비
        textPosition.anchoredPosition = new Vector2(info.XPos, -info.YPos);
        text.text = string.Empty;
        text.alignment = info.Align == 0 ? TextAlignmentOptions.Center :
            info.Align == -1 ? TextAlignmentOptions.Left : TextAlignmentOptions.Right;
        text.fontSize = info.FontSize;

        ShowText();
    }

    private void ShowText()
    {
        StartCoroutine(Typing.CoTextTyping(
            cutSceneInfo[currentRow].Text, text, wfTextInterval, onCutShown));
    }

    private void StopText()
    {
        StopAllCoroutines();
    }

    private void CutEnd()
    {
        callBack?.Invoke();

        InputManager.Instance.HideCursor();
    }

    private void onCutShown()
    {
        StartCoroutine(CoShowNext());
    }

    private IEnumerator CoShowNext()
    {
        yield return wfNextCut;
        if (++currentRow >= cutSceneInfo.Count)
        {
            CutEnd();
            yield break;
        }
        SetDialogView(currentRow);
    }
}
