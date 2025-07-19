using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CutSceneTester : MonoBehaviour
{
    [SerializeField] private string fileName = "TempCutScene";
    [SerializeField] private CutScenePlayer player;

    private void Start()
    {

        player.StartDialog(CutSceneParser.Convert(fileName));
    }
}
