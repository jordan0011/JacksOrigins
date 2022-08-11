using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleSystemVariations : MonoBehaviour
{
    [SerializeField]
    private GameObject me;
    [SerializeField]
    private ParticleSystem ps;

    [SerializeField]
    private int parsysid = 0;

    // Start is called before the first frame update
    void Start()
    {
        ps.Play();
    }

}
