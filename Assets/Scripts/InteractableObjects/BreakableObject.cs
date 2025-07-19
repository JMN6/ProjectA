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

    private void OnEnable()
    {
        // todo. ���⿡�� �ٽ� �ʱ�ȭ ���� ���� �ؾ���
        foreach (var _ in connectedObjects)
        {
            _.Deactivate();
        }
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
        // todo. �μ����� ����Ʈ �߰��ؾ���
        gameObject.SetActive(false);
    }
}
