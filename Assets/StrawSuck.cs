using UnityEngine;

public class StrawSuck : MonoBehaviour
{
    public BobaStraw straw;

    private void OnTriggerStay2D(Collider2D other)
    {
        if (!enabled) return;
        if (other.attachedRigidbody)
        {
            var boba = other.attachedRigidbody.GetComponent<BobaPearl>();
            if (boba != null)
            {
                straw.Suck(boba);
            }
        }
    }
}
