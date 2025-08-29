using UnityEngine;

public class HeartUI : MonoBehaviour
{
    private static readonly int _HurtAnim = Animator.StringToHash("Hurt");
    private static readonly int _RestoreAnim = Animator.StringToHash("Restore");

    [SerializeField] private Animator _animator;
    private bool isHurt = false;

    public void SetHeart(bool _isHurt)
    {
        if (isHurt == _isHurt)
            return;

        if(_isHurt)
        {
            _animator?.SetTrigger(_HurtAnim);
        }
        else
        {
            _animator?.SetTrigger(_RestoreAnim);
        }

        isHurt = _isHurt;
    }
}
