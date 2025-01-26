using UnityEngine;

public class StrawLump : MonoBehaviour
{
    private BobaStraw _straw;
    public float speed;
    public float travelDistance = 10;

    public void SetStraw(BobaStraw straw)
    {
        _straw = straw;
    }

    private void Update()
    {
        if (_straw.IsSucking)
        {
            Advance();
        }
        if(transform.localPosition.y > travelDistance)
        {
            GameObject.Destroy(this.gameObject);
        }
    }

    private void Advance()
    {
        transform.localPosition += Vector3.up * Time.deltaTime * speed;
    }


}
