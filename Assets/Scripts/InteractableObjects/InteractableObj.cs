using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public abstract class InteractableObj: MonoBehaviour
{
    public abstract void Activate();
    public abstract void Deactivate();
}
