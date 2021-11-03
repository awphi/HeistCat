using UnityEngine;

public static class Utils
{
    public static Vector2 GetFacing(int f)
    {
        return f switch
        {
            0 => Vector2.up,
            1 => Vector2.right,
            2 => Vector2.down,
            3 => Vector2.left,
            _ => Vector2.zero
        };
    }
}
