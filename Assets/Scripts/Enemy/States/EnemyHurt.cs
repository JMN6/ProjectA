using UnityEngine;
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
            SoundManager.Instance.PlaySFX(Path.ZombieSFX.Hurt, true);

            enemy.Rigid.AddForce(Vector2.up * 2f, ForceMode2D.Impulse);
            elapsedTime = 0f;
            animationTime = 0.333f;//enemy.GetCurrentAnimationLength();
            EffectManager.Instance.PlayParticle(Effects.Blood, enemy.transform.position + Vector3.up);
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