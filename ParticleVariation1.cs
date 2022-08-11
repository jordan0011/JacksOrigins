using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class ParticleVariation1 : MonoBehaviour
{
    [SerializeField]
    ParticleSystem ps1;
    [SerializeField]
    ParticleSystem ps2;
    [SerializeField]
    VisualEffect[] visualeffects;

    [SerializeField]
    private float starttimer1;
    [SerializeField]
    private float starttimer2;
    [SerializeField]
    private float starttimer3;

    [SerializeField]
    private float timer1;

    private bool start1 = true;
    private bool start2 = true;
    private bool start3 = true;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        timer1 += Time.deltaTime;

        if(timer1>= starttimer1 && start1)
        {
            ps1.gameObject.SetActive(true);

            start1 = false;
        }
        if (timer1 >= starttimer2 && start2)
        {
            ps2.gameObject.SetActive(true);

            start2 = false;
        }
        if(timer1 >= starttimer3 && start3)
        {
            for (int i = 0; i < visualeffects.Length; i++)
            {
                visualeffects[i].gameObject.SetActive(true);
            }
            start3 = false;
        }
    }
}
