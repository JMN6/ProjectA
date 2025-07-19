using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

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
        cutScenePlayer.StartCutScene(CutSceneParser.Convert(ClearCutScene),
            () => SceneManager.LoadScene(1));
        
    }
}
