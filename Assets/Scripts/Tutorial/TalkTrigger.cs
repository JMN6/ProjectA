using Assets.Scripts.Managers;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class TalkTrigger : MonoBehaviour
{
    //[SerializeField, TextArea] private string _dialogue;
    [SerializeField] private string _dialogueTextId;
    [SerializeField] private Sprite _portrait;
    private bool _isExecuted = false;
    

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (_isExecuted) return;

        if (!collision.TryGetComponent(out Talker talker))
        {
            talker = collision.GetComponentInParent<Talker>();
        }

        if (talker == null) return;

        _isExecuted = true;
        string dialogue = LanguageManager.Instance.GetString(_dialogueTextId);
        talker.Talk(dialogue, _portrait);
    }
}
