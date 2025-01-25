using Unity.VisualScripting;
using UnityEngine;

public class BobaStraw : MonoBehaviour
{
    public Transform _pivotPoint;
    public Transform _strawTransform;
    public Rigidbody2D _strawRigidbody;
    public Transform _upperBound;
    public Transform _lowerBound;
    public Transform _leftBound;
    public Transform _rightBound;

    public Vector2 _bounds;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

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
        Debug.Log("angle " + angle);
        Quaternion strawLook = Quaternion.LookRotation(_pivotPoint.position - clampedMousePoint, Vector3.back);

        _strawRigidbody.MovePositionAndRotation(clampedMousePoint, angle);
        //_strawTransform.position = mousePoint;
        //_strawTransform.rotation = strawLook;


    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(transform.position, _bounds * 2f);
    }
}
