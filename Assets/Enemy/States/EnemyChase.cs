using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EnemeyFSM
{
    public class EnemyChase : EnemyFSMState
    {
        private Rigidbody2D rigid;
        private Transform targetTrasnform;

        private float sqrAttackRange;

        private int targetDirection;

        public override void OnEnter()
        {
            rigid = enemy.Rigid;
            targetTrasnform = enemy.Target.transform;

            sqrAttackRange = enemy.AttackRange * enemy.AttackRange;
            targetDirection = 0;
        }

        public override void OnUpdate()
        {
            ChaseTarget();

            if(isTargetCloseEnough())
            {
                manager.ChangeState(FSMState.Attack);
            }
        }

        private void ChaseTarget()
        {
            float dir = rigid.position.x - targetTrasnform.position.x;
            int newDir = Convert.ToInt32(Mathf.Sign(dir)) * -1;

            // 스프라이트 방향 바꾸기
            if(targetDirection != newDir && targetDirection != 0)
            {
                targetDirection = newDir;

                enemy.spriteRenderer.flipX = newDir == 0;
            }

            Vector2 newPosition = rigid.position + 
                Vector2.right * newDir * enemy.Speed * Time.deltaTime;
            rigid.MovePosition(newPosition);
        }

        private bool isTargetCloseEnough()
        {
            return (rigid.position - (Vector2)targetTrasnform.position).sqrMagnitude <
                sqrAttackRange;
        }

        public override void OnExit()
        {
            sqrAttackRange = 0f;
            targetDirection = 0;
        }
    }
}

