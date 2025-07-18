using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EnemeyFSM
{
    public class EnemyPatrol : EnemyFSMState
    {
        private Vector2 startPos;
        private Vector2 endPos;
        private Vector2 dir;

        private Rigidbody2D rigid;

        private float elapsedTime = 0f;

        public override void OnEnter()
        {
            PatrolEnemy e = enemy as PatrolEnemy;
            startPos = e.StartPos.position;
            endPos = e.EndPos.position;

            dir = (endPos - startPos).normalized;

            rigid = e.Rigid;

            elapsedTime = 0f;

            rigid.MovePosition(startPos); // 초기값 세팅
        }

        public override void OnUpdate()
        {
            Vector2 newPosition = rigid.position + dir * enemy.Speed * Time.deltaTime;
            if((newPosition - endPos).sqrMagnitude <= 0.5f)
            {
                newPosition = endPos;
                Vector2 temp = endPos;
                endPos = startPos;
                startPos = temp;
                dir = (endPos - startPos).normalized;
            }

            rigid.MovePosition(newPosition);
        }

        public override void OnExit()
        {
        }
    }
}

