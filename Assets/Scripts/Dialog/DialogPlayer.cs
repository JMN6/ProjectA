using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DialogPlayer : MonoBehaviour
{
    private static readonly string FName = "나연";
    private static readonly string MName = "용복";

    private static readonly string FPicPath = "Images/Dialog/F{0}";
    private static readonly string MPicPath = "Images/Dialog/M{0}";

    [SerializeField] private GameObject panel;
    [SerializeField] private Image pic1;
    [SerializeField] private Image pic2;
    [SerializeField] private TextMeshProUGUI text;
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private float textShowIntervalSpeed = 0.1f;
    private WaitForSeconds wfTextInterval = null;

    private List<DialogRow> dialogInfo = null;
    private int currentRow = -1;

    private void Start()
    {
        wfTextInterval = new WaitForSeconds(textShowIntervalSpeed);
    }

    public void StartDialog(in List<DialogRow> rows)
    {
        dialogInfo = rows;
        if(dialogInfo.Count <= 0)
        {
            return;
        }

        panel.SetActive(true);

        SetDialogView(0);
    }

    private void SetDialogView(int rowNum)
    {
        if (rowNum < 0 || rowNum >= dialogInfo.Count)
            return;

        currentRow = rowNum;
        DialogRow info = dialogInfo[currentRow];

        // 사진 변경
        Sprite pic1Sprite = Resources.Load<Sprite>(string.Format(FPicPath, info.Pic1Number));
        Sprite pic2Sprite = Resources.Load<Sprite>(string.Format(MPicPath, info.Pic2Number));

        pic1.sprite = pic1Sprite;
        pic2.sprite = pic2Sprite;

        if(info.Talker == 0)
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

        ShowText();
    }

    private void ShowText()
    {
        StartCoroutine(CoShowText());
    }

    private IEnumerator CoShowText()
    {
        string newText = string.Empty;
        string targetText = dialogInfo[currentRow].Text;
        foreach(var _ in targetText)
        {
            newText += _;
            text.text = newText;

            yield return wfTextInterval;
        }
    }

    private void StopText()
    {
        StopAllCoroutines();
    }

    private void DialogEnd()
    {
        panel.SetActive(false);
    }

    public void OnNextBtnClick()
    {
        StopText();

        if(++currentRow >= dialogInfo.Count)
        {
            DialogEnd();
            return;
        }
        SetDialogView(currentRow);
    }

}
