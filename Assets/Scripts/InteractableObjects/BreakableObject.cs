using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreakableObject : MonoBehaviour, IDamagalbe
{
    [SerializeField] private IActivatable[] connectedObjects = new IActivatable[0];

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
        // todo. �μ����� ����Ʈ �߰��ؾ���
        gameObject.SetActive(false);
    }
}
