using UnityEngine;

public class ShellExplosion : MonoBehaviour
{
    public LayerMask m_TankMask;
    public ParticleSystem m_ExplosionParticles;
    public AudioSource m_ExplosionAudio;
    public float m_MaxDamage = 100f;
    public float m_ExplosionForce = 1000f;
    public float m_MaxLifeTime = 2f;
    public float m_ExplosionRadius = 5f;


    private void Start()
    {
        Destroy(gameObject, m_MaxLifeTime);
    }


    private void OnTriggerEnter(Collider other)
    {
        // Find all the tanks in an area around the shell and damage them.

        Collider[] colliders = Physics.OverlapSphere(transform.position, m_ExplosionRadius, m_TankMask);
        if (colliders != null)
        {
            for (int i = 0; i < colliders.Length; ++i)
            {
                Rigidbody body = colliders[i].GetComponent<Rigidbody>();
                if (!body) { continue; }

                body.AddExplosionForce(m_ExplosionForce, transform.position, m_ExplosionRadius);


                float damage = CalculateDamage(colliders[i].transform.position);
                TankHealth health = colliders[i].GetComponent<TankHealth>();
                if (!health)
                {
                    continue;
                }
                health.TakeDamage(damage);


            }
        }

        m_ExplosionParticles.transform.parent = null;
        m_ExplosionParticles.Play();
        m_ExplosionAudio.Play();

        Destroy(m_ExplosionParticles.gameObject, m_ExplosionParticles.duration);
        Destroy(gameObject);

    }


    private float CalculateDamage(Vector3 targetPosition)
    {
        // Calculate the amount of damage a target should take based on it's position.
        return Mathf.Max(0, m_MaxDamage * (m_ExplosionRadius - Vector3.Distance(transform.position, targetPosition)) / m_ExplosionRadius);
    }
}