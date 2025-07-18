using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogTrigger : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // todo. 다이얼로그 출력하기
        Debug.Log("Dialog Text");
    }
}
