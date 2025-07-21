using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageManager : MonoBehaviour
{
    [System.Serializable]
    private class StageDatas
    {
        public Transform respawnPosition;
        public bool isFacedLeft;
        public GameObject[] ResetObjs;
        public IResetable[] ResetScripts;
    }

    [Header("Stage")]
    [SerializeField] private StageDatas[] datas;
    public int StagetCount { get => datas.Length; }

    private void Start()
    {
        Init();
    }

    private void Init()
    {
        foreach (var _ in datas)
        {
            int size = _.ResetObjs.Length;
            _.ResetScripts = new IResetable[size];
            for (int i = 0; i < size; i++)
            {
                _.ResetScripts[i] = _.ResetObjs[i].GetComponent<IResetable>();
            }
        }
    }

    public Vector3 GetRespawnPosition(int stage) => datas[stage].respawnPosition.position;
    public bool GetRespawnFaceWay(int stage) => datas[stage].isFacedLeft;

    public void ResetGame(int stage)
    {
        foreach (var _ in datas[stage].ResetScripts)
        {
            _.OnReset();
        }
    }
}
