using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainCameraController : MonoBehaviour
{
    public Transform focus;
    
    // Update is called once per frame
    void Update()
    {
        if (focus == null) return;
        var position = focus.position;
        var t = transform.parent;
        t.position = new Vector3(position.x, position.y, t.position.z);
    }

    public IEnumerator Shake(float duration, float magnitude)
    {
        var pos = transform.localPosition;
        var elapsed = 0f;

        while (elapsed < duration)
        {
            var x = Random.Range(-0.5f, 0.5f) * magnitude;
            var y = Random.Range(-0.5f, 0.5f) * magnitude;

            transform.localPosition = new Vector3(x, y, pos.z);

            elapsed += Time.deltaTime;
            
            yield return null;
        }
    }
}
