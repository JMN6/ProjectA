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

        private float speed;

        public override void OnEnter()
        {
            enemy.SetAnimationTrigger(Enemy.EnemyAnimation.Patrol);

            PatrolEnemy e = enemy as PatrolEnemy;
            startPos = e.StartPos;
            endPos = e.EndPos;
            speed = e.PatrolSpeed;

            dir = (endPos - startPos).normalized;
            enemy.spriteRenderer.flipX = dir.x < 0;

            rigid = e.Rigid;

            elapsedTime = 0f;

            enemy.transform.position = startPos;

            enemy.Rigid.velocity = Vector2.zero;
        }

        public override void OnFixedUpdate()
        {
            Vector2 newPosition = rigid.position + dir * speed * 0.05f;
            if((newPosition - endPos).sqrMagnitude <= 0.5f)
            {
                newPosition = endPos;
                Vector2 temp = endPos;
                endPos = startPos;
                startPos = temp;
                dir = (endPos - startPos).normalized;

                enemy.spriteRenderer.flipX = dir.x < 0;
            }

            rigid.MovePosition(newPosition);
        }


        public override void OnTriggerEnter(Collider2D collision)
        {
            if (collision.gameObject.layer != playerLayer)
            {
                return;
            }

            enemy.Target = collision.gameObject;
            manager.ChangeState(FSMState.Chase);
        }

        public override void OnUpdate()
        { 
        }

        public override void OnExit()
        {
        }
    }
}

