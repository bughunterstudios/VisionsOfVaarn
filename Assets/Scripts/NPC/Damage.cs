using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Damage : MonoBehaviour
{
    public int dice;
    public int damage;
    public float force;

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<Health>())
        {
            int dmg = 0;
            if (dice == 0)
            {
                dmg = Random.Range(1, damage);
            }
            else
            {
                for (int i = 0; i < dice; i++)
                    dmg += Random.Range(1, damage);
            }
            other.GetComponent<Health>().Damage(dmg, force);
        }
    }
}
