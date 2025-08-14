using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Item
{
    [field: SerializeField] public string Name { get; private set; }
    [field: SerializeField] public int ID { get; private set; }
    [field: SerializeField] public Sprite Sprite { get; private set; }
}

[CreateAssetMenu(fileName = "ItemsSO", menuName = "Scriptable Objects/ItemsSO")]
public class ItemsSO : ScriptableObject
{
    [SerializeField] List<Item> _items;
    Dictionary<int, Item> _itemDict;

    public Item GetItem(int id) => _itemDict != null && _itemDict.ContainsKey(id) ? _itemDict[id] : null;

    private void Awake()
    {
        _itemDict = new(_items.Count);

        foreach (var item in _items)
        {
            _itemDict.Add(item.ID, item);
        }
    }
}
