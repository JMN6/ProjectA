using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CutSceneTrigger : MonoBehaviour, IInteractable
{
    [SerializeField] private CutScenePlayer player;
    [SerializeField] private string fileName = "TempCutScene 1";

    private List<CutSceneRow> info;

    private LayerMask playerMask;

    private GameObject playerObj;
    private Player playerScript;

    private bool isUsed = false;

    private void Start()
    {
        info = CutSceneParser.Convert(fileName);
        playerMask = LayerMask.NameToLayer("Player");

        isUsed = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (isUsed == true)
            return;

        if (collision.gameObject.TryGetComponent<Player>(out playerScript) == true)
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

        player.StartCutScene(info, () =>
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
