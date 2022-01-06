using UnityEngine;

public abstract class Interactable : MonoBehaviour
{
    public abstract void EnterView(ViewConeController viewer);

    public abstract void ExitView(ViewConeController viewer);

    public abstract void Interact(ViewConeController viewer);
} 