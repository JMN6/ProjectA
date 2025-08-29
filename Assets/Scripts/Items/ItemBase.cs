using System.Collections;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer), typeof(Collider2D))]
public abstract class ItemBase : MonoBehaviour
{
    [SerializeField] private float floatDistance = 0.3f;
    [SerializeField] private float floatTime = 1.3f;
    [SerializeField] private float floatSpeed = 2f;

    protected int _id;
    protected SpriteRenderer _sprite;
    protected Collider2D _collider;

    protected virtual void Awake()
    {
        _sprite = GetComponent<SpriteRenderer>();
        _collider = GetComponent<Collider2D>();
    }

    private void Start()
    {
        _collider.excludeLayers = ~LayerMask.GetMask("Player");

        StartCoroutine(CoFloatItem(
            transform.position + transform.up * floatDistance * 0.5f,
            transform.position - transform.up * floatDistance * 0.5f
            ));
    }

    public void Init(int id, Vector3 position)
    {
        _id = id;

        _sprite.sprite = GameManager.Instance.ItemsSO.GetItem(_id)?.Sprite;

        transform.position = position;
    }

    private IEnumerator CoFloatItem(Vector3 startPos, Vector3 endPos)
    {
        float elapsedTime = 0f;

        while(elapsedTime < floatTime)
        {
            elapsedTime += Time.deltaTime;
            transform.position = Vector3.Lerp(transform.position, endPos, Time.deltaTime * floatSpeed);
            //transform.position = Vector3.Lerp(startPos, endPos, elapsedTime / floatTime);
            yield return null;
        }

        StartCoroutine(CoFloatItem(endPos, startPos));
    }

    public abstract void Use();

    private void OnTriggerEnter2D(Collider2D collision)
    {
        AnalyticsSystem.Instance.OnPlayerGotItem(_id);
        Use();

        Destroy(gameObject);
    }
}
