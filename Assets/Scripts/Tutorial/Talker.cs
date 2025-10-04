using Assets.Scripts.Managers;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Talker : MonoBehaviour
{
    [SerializeField] private GameObject speechBubble;
    [SerializeField] private Image image;
    [SerializeField] private TextMeshProUGUI dialogue;
    [SerializeField] private float talkSpeed;

    private WaitForSeconds talkTerm;
    private Coroutine coroutine;

    private void Awake()
    {
        talkTerm = new WaitForSeconds(0.2f / talkSpeed);

        speechBubble.gameObject.SetActive(false);
    }

    private void Start()
    {
        LanguageManager.Instance.SetTextFont(dialogue);
    }

    private void Update()
    {
        speechBubble.transform.rotation = Quaternion.identity;
    }

    public void Talk(string dialogue, Sprite portrait = null)
    {
        if (coroutine != null) StopCoroutine(coroutine);

        if (portrait) image.sprite = portrait;

        speechBubble.gameObject.SetActive(true);
        InputManager.Instance.SetInputEnabled(Game.Input.Maps.Player, false);

        coroutine = StartCoroutine(StringUtility.Typing.CoTextTyping(dialogue, this.dialogue, talkTerm, () => Invoke(nameof(HideBubble), 1f)));
    }

    public void HideBubble()
    {
        speechBubble.gameObject.SetActive(false);
        InputManager.Instance.SetInputEnabled(Game.Input.Maps.Player, true);
    }
}
