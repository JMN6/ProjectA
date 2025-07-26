using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using DataParseAndPlayer;

public class GameStarter : MonoBehaviour
{
    [SerializeField] private CutScenePlayer cutScenePlayer;
    [SerializeField] private string ClearCutScene = "TempCutScene";
    private void Awake()
    {
        InputManager.Instance.ShowCursor();
    }

    public void GameStart()
    {
        cutScenePlayer.StartScene(Parser<CutSceneRow>.Convert("CutScene", ClearCutScene),
            () => SceneManager.LoadScene(1));
        
    }
}
