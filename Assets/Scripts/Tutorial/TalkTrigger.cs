using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class TalkTrigger : MonoBehaviour
{
    [SerializeField] private string dialogue;
    [SerializeField] private Sprite Portrait;
    private bool isExecuted = false;
    

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (isExecuted) return;

        if (!collision.TryGetComponent(out Talker talker))
        {
            talker = collision.GetComponentInParent<Talker>();
        }

        if (talker == null) return;

        isExecuted = true;
        talker.Talk(dialogue, Portrait);
    }
}
