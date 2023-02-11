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
        if (transform.IsChildOf(other.transform))
            return;
        var health = other.GetComponent<Health>();
        var player_health = other.GetComponent<PlayerHealth>();
        if (health != null || player_health != null)
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
            if (health != null)
                health.Damage(dmg, force);
            else if (player_health != null)
                player_health.Damage(dmg, transform);
        }
    }
}
