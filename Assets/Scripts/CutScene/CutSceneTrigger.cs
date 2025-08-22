using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DataParseAndPlayer;

public class CutSceneTrigger : MonoBehaviour, IInteractable
{
    [SerializeField] private CutScenePlayer player;
    [SerializeField] private string fileName = "TempCutScene 1";
    private static readonly string folderName = "CutScene";

    private List<CutSceneRow> info;

    private LayerMask playerMask;

    private GameObject playerObj;
    private Player playerScript;

    private bool isUsed = false;

    private void Start()
    {
        info = DataParseAndPlayer.Parser<CutSceneRow>.Convert(folderName, fileName);
        playerMask = LayerMask.NameToLayer("Player");

        isUsed = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (isUsed == true)
            return;

        if (collision.attachedRigidbody.TryGetComponent<Player>(out playerScript) == true)
        {
            playerScript.CanInteract(true, this);

            playerObj = collision.gameObject;
        }
    }

    public void Interact()
    {
        GameManager.Instance.StageClear();

        InputManager.Instance.ShowCursor();
        InputManager.Instance.SetInputs(false);

        player.StartScene(info, () =>
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene(0);
        });

        isUsed = true;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (isUsed == true)
            return;

        if (playerObj == collision.gameObject)
        {
            playerScript.CanInteract(false);
            playerObj = null;
            playerScript = null;
        }
    }
}
