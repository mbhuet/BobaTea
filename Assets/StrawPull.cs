using UnityEngine;

public class StrawPull : MonoBehaviour
{
    public ParticleSystem _suckParticles;
    public float suckForce = 1f;
    public float suckRadius = 1;

    private void OnTriggerStay2D(Collider2D other)
    {
        if (!enabled) return;
        if (other.attachedRigidbody)
        {
            var boba = other.attachedRigidbody.GetComponent<BobaPearl>();
            if (boba != null)
            {
                Vector3 pullVector = transform.position - boba.transform.position;
                other.attachedRigidbody.AddForce(pullVector.normalized * suckForce, ForceMode2D.Force);
            }
        }
    }
    private void OnEnable()
    {
        _suckParticles.Play();
    }

    private void OnDisable()
    {
        _suckParticles.Stop();
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, suckRadius);
    }
}
