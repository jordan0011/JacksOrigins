using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class VisualEffectVariations : MonoBehaviour
{
    [SerializeField]
    private VisualEffect[] visualeffect;

    [SerializeField]
    private int visualid;
    [SerializeField]
    private float starttime0 = 0f;
    [SerializeField]
    private float starttime1 = 0.2f;
    [SerializeField]
    private float starttime;

    private bool start = false;

    [SerializeField]
    private int howmuchiuse = 5;
    // Start is called before the first frame update
    void Start()
    {
        for(int i=0; i< visualeffect.Length; i++)
        {
            visualeffect[i].playRate = 1.80f;
        }

        if(visualid == 0)
        {
            starttime = starttime0;
        }
        else if(visualid == 1)
        {
            starttime = starttime1;
        }
    }

    private void Update()
    {
        if (starttime > 0)
        {
            starttime -= Time.deltaTime;
        }
        else if(!start)
        {
            for(int i=0; i<howmuchiuse; i++)
            {
                visualeffect[i].gameObject.SetActive(true);
            }
            start = true;
        }
    }
}
