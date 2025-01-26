using DG.Tweening;
using Unity.VisualScripting;
using UnityEngine;

public class BobaStraw : MonoBehaviour
{
    public Transform _pivotPoint;
    public Transform _strawTransform;
    public Rigidbody2D _strawRigidbody;
    public StrawLump _lumpPrefab;

    public AudioSource _slurpAudio;

    public Vector2 _bounds;

    public bool IsSucking => isSucking;
    public Vector3 TipPosition => _strawTransform.position;

    public StrawPull _pull;
    public StrawSuck _suck;

    private bool isSucking = false;

    public float strawMoveForce = 1;
    private bool isInteractable = false;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        EndSuck();
    }

    // Update is called once per frame
    void Update()
    {
        if (isInteractable)
        {
            bool wasSucking = isSucking;
            isSucking = Input.GetMouseButton(0);
            if (isSucking && !wasSucking)
            {
                BeginSuck();
            }
            if (wasSucking && !isSucking)
            {
                EndSuck();
            }
        }
    }


    private void FixedUpdate()
    {
        if (isInteractable)
        {
            //fit straw to moust
            Vector3 mousePoint = Camera.main.ScreenToWorldPoint(Input.mousePosition, Camera.MonoOrStereoscopicEye.Mono);
            mousePoint.z = _pivotPoint.position.z;



            Vector3 clampedMousePoint = new Vector3(
                Mathf.Clamp(mousePoint.x, transform.position.x - _bounds.x, transform.position.x + _bounds.x),
                Mathf.Clamp(mousePoint.y, transform.position.y - _bounds.y, transform.position.y + _bounds.y),
                mousePoint.z);

            float angle = Vector3.SignedAngle(Vector3.up, _pivotPoint.position - clampedMousePoint, Vector3.forward);
            Debug.DrawLine(_pivotPoint.position, clampedMousePoint);
            Quaternion strawLook = Quaternion.LookRotation(_pivotPoint.position - clampedMousePoint, Vector3.back);

            _strawRigidbody.MovePositionAndRotation(clampedMousePoint, angle);

            //Vector2 curToTarget = (Vector2)clampedMousePoint - _strawRigidbody.position;
            //_strawRigidbody.AddForce(curToTarget.normalized *  Mathf.Min(curToTarget.magnitude, strawMoveForce));
            //_strawRigidbody.MoveRotation(angle);
            //MoveDynamic(_strawRigidbody, clampedMousePoint, Quaternion.Euler(0, 0, angle));

            //_strawTransform.position = mousePoint;
            //_strawTransform.rotation = strawLook;
        }
    }

    public void SetInteractable(bool interactable)
    {
        isInteractable = interactable;
        if (!isInteractable)
        {
            EndSuck();
        }
    }

    private void BeginSuck()
    {
        //Debug.Log("Begin Suck");
        isSucking = true;
        _pull.enabled = true;
        _suck.enabled = true;
        _slurpAudio.Play();
    }

    private void EndSuck()
    {
        //Debug.Log("End Suck");
        isSucking = false;
        _pull.enabled = false;
        _suck.enabled = false;
        _slurpAudio.Stop();
    }

    public void Suck(BobaPearl boba)
    {
        GameManager.Instance.TeaManager.BobaSucked?.Invoke(boba);
        GameObject.Destroy(boba.gameObject);
        if(AudioManager.Instance._bobaSuckSound) AudioSource.PlayClipAtPoint(AudioManager.Instance._bobaSuckSound, Camera.main.transform.position);
        SendLump();
    }

    private void SendLump()
    {
        StrawLump lump = GameObject.Instantiate(_lumpPrefab, _lumpPrefab.transform.parent);
        lump.gameObject.SetActive(true);
        lump.SetStraw(this);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(transform.position, _bounds * 2f);
    }

    /// <summary>
    /// Attempts to move a dynamic rigidbody to a specific position/rotation using ForceMode.VelocityChange. Used when we want to precisely control the movement of a rigidbody but not let it pass through static colliders.
    /// </summary>
    /// <param name="rb"></param>
    /// <param name="targetPos"></param>
    /// <param name="targetRot"></param>
    public static void MoveDynamic(Rigidbody2D rb, Vector3 targetPos, Quaternion targetRot)
    {
        float scale = rb.transform.lossyScale.x;
        Vector3 targetCenterOfMass = targetPos + targetRot * (rb.centerOfMass * scale);
        Vector3 deltaCenterOfMass = targetCenterOfMass - rb.transform.TransformPoint(rb.centerOfMass);
#if UNITY_6000_0_OR_NEWER
        Vector3 rbVelocity = rb.linearVelocity;
#else
Vector3 rbVelocity = rb.velocity;
#endif
        Vector3 correctiveVel = deltaCenterOfMass / Time.deltaTime - rbVelocity;

        rb.linearVelocity = correctiveVel;
        //rb.AddForce(correctiveVel, ForceMode2D.);

        Quaternion rbRotation = Quaternion.Euler(0, 0, rb.rotation);
        Quaternion deltaRot = (targetRot * Quaternion.Inverse(rbRotation)).normalized;

        Vector3 correctionRotAxis;
        float correctionRotAngle;
        deltaRot.ToAngleAxis(out correctionRotAngle, out correctionRotAxis);

        Vector3 correctiveAngularVel =
        correctionRotAxis * (correctionRotAngle * Mathf.Deg2Rad) /
        Time.deltaTime - new Vector3(0,0,rb.angularVelocity);

        rb.angularVelocity = correctiveAngularVel.z;
        //rb.AddTorque(correctiveAngularVel, ForceMode.VelocityChange);
    }
}
