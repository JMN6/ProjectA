using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EnemeyFSM
{
    public class EnemyDeath : EnemyFSMState
    {
        private static readonly float animationLength = 0.75f;
        private float elaspedTime = 0f;

        public override void OnEnter()
        {
            SoundManager.Instance.PlaySFX(enemy.DeadSFX);
            bool isLeft = enemy.spriteRenderer.flipX;
            Vector2 forceVec= (Vector2.up + (isLeft ? Vector2.right : Vector2.right * -1f)).normalized;

            enemy.Rigid.AddForce(forceVec * 100f * (enemy.isParried ? 3f: 1f));
            enemy.SetAnimationTrigger(Enemy.EnemyAnimation.IsDead);
            elaspedTime = 0f;
        }

        public override void OnExit()
        {
        }

        public override void OnUpdate()
        {
            elaspedTime += Time.deltaTime;
            if(elaspedTime >= animationLength)
            {
                enemy.DestroySelf();
            }
        }
    }
}
