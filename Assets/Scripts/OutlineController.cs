using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OutlineController : MonoBehaviour
{
    private SpriteMask _mask;
    private SpriteRenderer[] _outlines;
    
    // Start is called before the first frame update
    private void Awake()
    {
        _mask = GetComponent<SpriteMask>();
        _outlines = GetComponentsInChildren<SpriteRenderer>();
    }

    public void Target(GameObject target, float size, Color color)
    {
        var sr = target.GetComponent<SpriteRenderer>();
        
        if (sr == null)
        {
            return;
        }

        transform.position = target.transform.position;
        _mask.sprite = sr.sprite;
        
        for (var i = 0; i < _outlines.Length; i++)
        {
            _outlines[i].sprite = sr.sprite;
            _outlines[i].color = color;
            var v = Utils.GetFacing(i) * size;
            Debug.Log(v);
            _outlines[i].transform.localPosition = new Vector3(v.x, v.y, 0f);
        }
    }
}
