using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DataParseAndPlayer;

[System.Serializable]
public class DialogRow : DataRow
{
    public int Pic1Number;
    public int Pic2Number;
    public string Text;
    public int Talker;

    public override void SetValue(string[] datas)
    {
        base.SetValue(datas);
        Pic1Number = int.Parse(datas[1]);
        Pic2Number = int.Parse(datas[2]);
        Text = datas[3];
        Talker = int.Parse(datas[4]);
    }
}
