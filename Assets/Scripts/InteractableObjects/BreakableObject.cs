using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreakableObject : MonoBehaviour, IDamagalbe
{
    [SerializeField] private InteractableObj[] connectedObjects = new InteractableObj[0];

    private void Start()
    {
        gameObject.layer = LayerMask.NameToLayer("InteractableObject");
    }

    public void GetDamaged(int damage)
    {
        ShowEffect();

        if(connectedObjects.Length <= 0 )
        {
            return;
        }

        foreach(var _ in connectedObjects)
        {
            _.Activate();
        }
    }

    private void ShowEffect()
    {
        // todo. 부셔지는 이펙트 추가해야함
        gameObject.SetActive(false);
    }
}
