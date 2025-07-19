using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogTrigger : MonoBehaviour
{
    [SerializeField] private DialogPlayer dialogPlayer;
    [SerializeField] private string dialogFileName = "TempDialog";
    private List<DialogRow> info;

    private LayerMask playerMask;

    private GameObject playerObj;
    private Player playerScript;

    private bool isUsed = false;

    private void Start()
    {
        info = DialogParser.Convert(dialogFileName);
        playerMask = LayerMask.NameToLayer("Player");

        isUsed = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (isUsed == true)
            return;

        if(collision.gameObject.TryGetComponent<Player>(out playerScript) == true)
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

        dialogPlayer.StartDialog(info);

        isUsed = true;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (isUsed == true)
            return;

        if(playerObj == collision.gameObject)
        {
            playerScript.CanInteract(false);
            playerObj = null;
            playerScript = null;
        }
    }
}
