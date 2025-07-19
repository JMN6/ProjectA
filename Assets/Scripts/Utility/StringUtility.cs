using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace StringUtility
{
    public class Typing
    {
        public static IEnumerator CoTextTyping(string _targetString, TextMeshProUGUI _text, 
            WaitForSeconds _wfTypeInterval)
        {
            var builder = new System.Text.StringBuilder();

            for (int i = 0; i < _targetString.Length; i++)
            {
                builder.Append(_targetString[i]);
                _text.text = builder.ToString();
                yield return _wfTypeInterval;
            }
        }
    }
}

