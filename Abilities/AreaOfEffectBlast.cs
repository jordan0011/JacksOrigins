using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AreaOfEffectBlast : MonoBehaviour
{
    protected PowerStats Source = null;
    protected int id = -1;
    protected bool isSumChild = false;
    protected int sumId = -1;
    public void SetPlayerMotor(PowerStats _pm, int myid)
    {
        Source = _pm;
        id = myid;
    }
    public void SumObject()
    {
        isSumChild = true;
    }
    public void SumObjectID(int mysumid)
    {
        sumId = mysumid;
    }
    public virtual void BlastDamage()
    {
        Collider[] hitEnemies1 = Physics.OverlapSphere(transform.position, transform.lossyScale.y/2);
        foreach (Collider enemy in hitEnemies1)
        {
            if (enemy.transform.GetComponent<PowerStats>() != null)
            {
                PowerStats target = enemy.transform.GetComponent<PowerStats>();
                if (target != null && Source != null && target != Source)
                {
                    target.Life(Source, -20, 0);
                }
            }
        }
    }
}
