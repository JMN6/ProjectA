using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogTrigger : MonoBehaviour
{
    [SerializeField] private DialogPlayer player;
    [SerializeField] private string dialogFileName = "TempDialog";
    private List<DialogRow> info;

    private LayerMask playerMask;

    private void Start()
    {
        info = DialogParser.Convert(dialogFileName);
        playerMask = LayerMask.NameToLayer("Player");
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer != playerMask)
            return;

        InputManager.Instance.ShowCursor();
        InputManager.Instance.SetInputs(false);

        player.StartDialog(info);
    }
}
