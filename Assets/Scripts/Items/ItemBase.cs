using UnityEngine;

[RequireComponent(typeof(SpriteRenderer), typeof(Collider2D))]
public abstract class ItemBase : MonoBehaviour
{
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
    }

    public void Init(int id, Vector3 position)
    {
        _id = id;

        _sprite.sprite = GameManager.Instance.ItemsSO.GetItem(_id)?.Sprite;

        transform.position = position;
    }

    public abstract void Use();

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Use();
    }
}
