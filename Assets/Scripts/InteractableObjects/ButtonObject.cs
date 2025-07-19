using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonObject : MonoBehaviour, IDamagalbe
{
    [SerializeField] private SpriteRenderer sp;
    [SerializeField] private Sprite OnSprite;
    [SerializeField] private Sprite OffSprite;
    [SerializeField] private InteractableObj[] connectedObjects = new InteractableObj[0];

    private void Start()
    {
        gameObject.layer = LayerMask.NameToLayer("InteractableObject");
    }

    private void OnEnable()
    {
        sp.sprite = OnSprite;

        foreach (var _ in connectedObjects)
        {
            _.Deactivate();
        }
    }

    public void GetDamaged(int damage)
    {
        ShowEffect();

        if (connectedObjects.Length <= 0)
        {
            return;
        }

        foreach (var _ in connectedObjects)
        {
            _.Activate();
        }
    }

    private void ShowEffect()
    {
        sp.sprite = OffSprite;
    }
}
