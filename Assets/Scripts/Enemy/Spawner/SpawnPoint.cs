using UnityEngine;

public class SpawnPoint : MonoBehaviour, IResetable
{
    [SerializeField] private GameObject _hint;
    [SerializeField] private Enemy[] _zombie;

    public void Start()
    {
        _hint.SetActive(true);

        foreach (Enemy enemy in _zombie)
        {
            enemy.gameObject.SetActive(false);
        }
    }

    public void Spawn()
    {
        // todo. 여기에 생성 연출 추가
        _hint.SetActive(false);

        foreach (var zombie in _zombie)
        {
            zombie.gameObject.SetActive(true);
            // todo. 여기 좀비 생성 시 필요한 애니메이션 실행
            //zombie.StartFSM();
        }
    }

    public void OnReset()
    {
        _hint.SetActive(true);

        foreach (var zombie in _zombie)
        {
            zombie.OnReset();
            zombie.gameObject.SetActive(false);
        }
    }
}
