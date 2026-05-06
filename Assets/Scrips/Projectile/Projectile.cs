using UnityEngine;

namespace SA
{
    public class Projectile : MonoBehaviour
    {
        Rigidbody rigid;

        public float hSpeed = 5;
        public float vSpeed = 2;

        public void Init()
        {
            rigid = GetComponent<Rigidbody>();

            Vector3 targetForce = transform.forward * hSpeed;
            targetForce += transform.up * vSpeed;
            rigid.AddForce(targetForce, ForceMode.Impulse);
        }

    }
}