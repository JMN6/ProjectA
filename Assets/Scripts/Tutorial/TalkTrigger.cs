using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class TalkTrigger : MonoBehaviour
{
    [SerializeField] private string dialogue;
    private bool isExecuted = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out Talker talker))
        {
            if (!isExecuted)
            {
                isExecuted = true;
                talker.Talk(dialogue);
            }
        }
    }
}
