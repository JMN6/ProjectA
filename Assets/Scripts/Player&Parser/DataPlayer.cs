using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DataParseAndPlayer
{
    public abstract class DataPlayer<T> : MonoBehaviour where T : DataRow
    {
        [SerializeField] protected GameObject panel;

        protected List<T> _sceneInfo;
        protected int _currentRow = -1;

        protected Action _callback;

        protected virtual void Start()
        {
            Init();
        }

        protected virtual void Init() { }

        public void StartScene(in List<T> rows, Action callback = null)
        {
            _sceneInfo = rows;
            if (_sceneInfo.Count <= 0)
            {
                return;
            }

            _callback = callback;

            panel.SetActive(true);

            SetScene(0);
        }

        protected virtual void SetScene(int rowNum)
        {
            if (rowNum < 0 || rowNum >= _sceneInfo.Count)
                return;

            _currentRow = rowNum;

            SetSceneInfo(_sceneInfo[_currentRow]);

            ShowText();
        }

        protected abstract void SetSceneInfo(T info);

        protected abstract void ShowText();

        protected void StopText() => StopAllCoroutines();

        public virtual void OnShowNextScene()
        {
            StopText();

            if (++_currentRow >= _sceneInfo.Count)
            {
                OnSceneEnd();
                return;
            }
            SetScene(_currentRow);
        }

        protected virtual void OnSceneEnd()
        {
            _callback?.Invoke();
        }
    }
}
