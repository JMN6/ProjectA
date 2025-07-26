using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DataParseAndPlayer;

public class CutSceneTester : MonoBehaviour
{
    [SerializeField] private string fileName = "TempCutScene";
    [SerializeField] private CutScenePlayer player;
    private static readonly string folderName = "CutScene";

    private void Start()
    {

        player.StartScene(Parser<CutSceneRow>.Convert(folderName, fileName));
    }
}
