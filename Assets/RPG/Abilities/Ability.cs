using UnityEngine;
using System.Collections;

[System.Serializable]
public abstract class Ability : MonoBehaviour {
    public string Name;

    public abstract void Enhance();
}
