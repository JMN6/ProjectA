using UnityEngine;

namespace EnemeyFSM
{
    public class EnemyDeath : EnemyFSMState
    {
        private static readonly float animationLength = 0.75f;
        private float elaspedTime = 0f;

        public override void OnEnter()
        {
            SoundManager.Instance.PlaySFX(Path.ZombieSFX.Dead);
            bool isLeft = enemy.spriteRenderer.flipX;
            Vector2 forceVec= (Vector2.up + (isLeft ? Vector2.right : Vector2.right * -1f)).normalized;

            enemy.Rigid.AddForce(forceVec * 100f * (enemy.isParried ? 3f: 1f));
            anim.SetTrigger(Enemy.EnemyAnimation.IsDead.ToString());
            elaspedTime = 0f;
            EffectManager.Instance.PlayParticle(Effects.Blood, enemy.transform.position + Vector3.up);
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
