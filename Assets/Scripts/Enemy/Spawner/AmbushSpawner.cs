using UnityEngine;

public class AmbushSpawner : BreakableObject
{
    [Header("Status")]
    [SerializeField] private int maxHealth;
    [SerializeField] private int health;

    [Header("Spawn")]
    [SerializeField] [Range(0f, 100f)] private float _spawnChance;
    [SerializeField] private Enemy[] _zombies;

    protected override void Awake()
    {
        health = maxHealth;
        base.Awake();
        foreach (Enemy enemy in _zombies)
        {
            enemy.gameObject.SetActive(false);
        }
    }

    public override void GetDamaged(int damage, GameObject damageSource)
    {
        health = Mathf.Clamp(health - 1, 0, maxHealth);
        if (health > 0)
        {
            return;
        }

        base.GetDamaged(damage, damageSource);

        float c = Random.Range(0f, 100f);
        if(c > _spawnChance)
        {
            return;
        }

        foreach (Enemy enemy in _zombies)
        {
            enemy.gameObject.SetActive(true);
            enemy.Rigid.AddForce(Vector3.up * 1000f);
        }
    }

    public override void OnReset()
    {
        base.OnReset();

        health = maxHealth;

        foreach (Enemy enemy in _zombies)
        {
            enemy.OnReset();
            enemy.gameObject.SetActive(false);
        }
    }
}
