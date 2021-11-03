using System;
using UnityEngine;

public class OutlineManager : MonoBehaviour
{
    public GameObject outlinePrefab;
    private static GameObject _outlinePrefabStatic;

    private void Awake()
    {
        _outlinePrefabStatic = outlinePrefab;
    }
    
    public static void Outline(GameObject target, float size = 0.05f)
    {
        Outline(target, size, Color.white);
    }

    public static void Outline(GameObject target, float size, Color color)
    {
        Remove(target);

        var o = Instantiate(_outlinePrefabStatic, target.transform).GetComponent<OutlineController>();
        o.name = "Outline";
        o.Target(target, size, color);
    }

    public static void Remove(GameObject target)
    {
        var c = target.transform.Find("Outline");
        if (c != null)
        {
            Destroy(c.gameObject);
        }
    }
}
