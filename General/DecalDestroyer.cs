using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DecalDestroyer : MonoBehaviour
{
    [SerializeField]
    private float _time;

    private float time0 = 0;
    private void Update()
    {
        if (time0 < _time)
        {
            time0 += Time.deltaTime;
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
