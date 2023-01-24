using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour
{
    public int health;
    public List<GameObject> bones;
    public float boneradius;
    public float bonelimits;
    public List<GameObject> destroyComponentsInObjects;

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Damage(int value, float force)
    {
        health -= value;
        if (health <= 0)
        {
            var components = GetComponents<Component>();
            foreach (GameObject obj in destroyComponentsInObjects)
            {
                components = obj.GetComponents<Component>();
                for (int j = 0; j < components.Length; j++)
                {
                    if (components[j].GetType() != typeof(Transform))
                        Destroy(components[j]);
                }
            }

            foreach (GameObject bone in bones)
            {
                bone.AddComponent<Rigidbody>();
                bone.GetComponent<Rigidbody>().AddForce(Random.onUnitSphere * force);
                bone.AddComponent<BoxCollider>().size = Vector3.one * boneradius;
            }

            foreach (GameObject bone in bones)
            {
                if (bone.transform.parent.GetComponent<Rigidbody>())
                {
                    HingeJoint joint = bone.AddComponent<HingeJoint>();
                    joint.connectedBody = bone.transform.parent.GetComponent<Rigidbody>();
                    joint.useLimits = true;
                    JointLimits limits = joint.limits;
                    limits.min = 0;
                    limits.bounciness = 0;
                    limits.bounceMinVelocity = 0;
                    limits.max = bonelimits;
                    joint.limits = limits;
                }
            }

            components = GetComponents<Component>();
            for (int j = 0; j < components.Length; j++)
            {
                if (components[j].GetType() != typeof(Transform))
                    Destroy(components[j]);
            }
        }
    }
}
