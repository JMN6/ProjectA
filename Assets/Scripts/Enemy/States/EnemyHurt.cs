using EnemeyFSM;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using static Enemy;

namespace EnemeyFSM
{
    public class EnemyHurt : EnemyFSMState
    {
        private float animationTime = 0f;
        private float elapsedTime = 0f;

        public override void OnEnter()
        {
            enemy.SetAnimationTrigger(EnemyAnimation.Hurt);
            if (Random.value > 0.5f) SoundManager.Instance.PlayOneShot(enemy.HurtSFX);
            else SoundManager.Instance.PlayOneShot(enemy.HurtSFX2);

            enemy.Rigid.AddForce(Vector2.up * 2f, ForceMode2D.Impulse);
            elapsedTime = 0f;
            animationTime = 0.333f;//enemy.GetCurrentAnimationLength();
            EffectManager.Instance.PlayParticle(0, enemy.transform.position + Vector3.up);
        }

        public override void OnExit() { }

        public override void OnUpdate()
        {
            elapsedTime += Time.deltaTime;
            if (elapsedTime > animationTime)
            {
                manager.ChangeToPrevState();
            }
        }
    }

}