using UnityEngine;

public abstract class Interactable : MonoBehaviour
{
    public abstract void EnterView(GameObject viewer);

    public abstract void ExitView(GameObject viewer);

    public abstract void Interact(GameObject viewer);
} 