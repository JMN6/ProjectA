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

        private float elapsedTime = 0f;

        private float speed;

        PatrolEnemy e;

        public override void init(Enemy _enemy, EnemyFSMManager _manager)
        {
            base.init(_enemy, _manager);

            e = _enemy as PatrolEnemy;
        }

        public override void OnEnter()
        {
            anim.SetTrigger(Enemy.EnemyAnimation.Patrol.ToString());

            startPos = e.StartPos;
            endPos = e.EndPos;
            speed = e.PatrolSpeed;

            dir = (endPos - startPos).normalized;
            enemy.spriteRenderer.flipX = dir.x < 0;

            elapsedTime = 0f;

            enemy.transform.position = startPos;

            enemy.Rigid.linearVelocity = Vector2.zero;
        }

        public override void OnFixedUpdate()
        {
            Vector2 newPosition = move.Rigid.position + dir * speed * 0.05f;
            if((newPosition - endPos).sqrMagnitude <= 0.5f)
            {
                newPosition = endPos;
                Vector2 temp = endPos;
                endPos = startPos;
                startPos = temp;
                dir = (endPos - startPos).normalized;

                enemy.spriteRenderer.flipX = dir.x < 0;
            }

            move.SetVelocity(dir.x * enemy.Speed, move.Rigid.linearVelocityY);
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

