using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Enemy;

public class AnimationPlayer : MonoBehaviour
{
    [SerializeField] private Animator anim;

    public void SetTrigger(string _trigger)
    {
        anim.SetTrigger(_trigger.ToString());
    }

    public void SetTrigger(int _triggerHash)
    {
        anim.SetTrigger(_triggerHash);
    }

    public void SetIntegar(string name, int value)
    {
        anim.SetInteger(name, value);
    }

    public void SetIntegar(int hash, int value)
    {
        anim.SetInteger(hash, value);
    }

    public void SetFloat(string name, float value)
    {
        anim.SetFloat(name, value);
    }
    public void SetFloat(int hash, float value)
    {
        anim.SetFloat(hash, value);
    }

    public void SetBool(string name, bool value)
    {
        anim.SetBool(name, value);
    }

    public void SetBool(int hash, bool value)
    {
        anim.SetBool(hash, value);
    }

    public float GetCurrentAnimationLength()
    {
        return anim.GetCurrentAnimatorStateInfo(0).length;
    }
}
