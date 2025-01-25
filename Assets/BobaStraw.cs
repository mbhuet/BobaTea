using Unity.VisualScripting;
using UnityEngine;

public class BobaStraw : MonoBehaviour
{
    public Transform _pivotPoint;
    public Transform _strawTransform;
    public Rigidbody2D _strawRigidbody;

    public Vector2 _bounds;

    public StrawPull _pull;
    public StrawSuck _suck;

    private bool isSucking = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        EndSuck();
    }

    // Update is called once per frame
    void Update()
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
        //_strawTransform.position = mousePoint;
        //_strawTransform.rotation = strawLook;

        bool wasSucking = isSucking;
        isSucking = Input.GetMouseButton(0);
        if(isSucking && !wasSucking)
        {
            BeginSuck();
        }
        if(wasSucking && !isSucking)
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
    }

    private void EndSuck()
    {
        //Debug.Log("End Suck");
        isSucking = false;
        _pull.enabled = false;
        _suck.enabled = false;  
    }

    public void Suck(BobaPearl boba)
    {
        GameObject.Destroy(boba.gameObject);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(transform.position, _bounds * 2f);
    }
}
