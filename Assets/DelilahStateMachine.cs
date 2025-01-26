using UnityEngine;

public class DelilahStateMachine : MonoBehaviour
{
    [SerializeField] private SpriteRenderer _spriteRenderer;
    [SerializeField] private Sprite[] _delilahSprites;
    [SerializeField] private BobaTeaManager _teaManager;

    [Space]
    [Header("Sprite Shake Parameters")]
    [SerializeField] private float _shakeDuration = 0.5f;
    [SerializeField] private float _shakeIntensity = 0.1f;

    private DelilahState _delilahState = DelilahState.Default;
    private float _shakeTimer = 0f;
    private Vector3 _initialSpritePosition = Vector3.zero;

    public enum DelilahState
    {
        Default,
        Drinking,
        Celebratory,
        Stunned
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        //if (_teaManager != null)
        //    _teaManager.BobaSucked += OnBobaSucked;

        _initialSpritePosition = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        /*
        if (_shakeTimer > 0f)
        {
            var offset = new Vector3((Random.insideUnitCircle * _shakeIntensity).x, 0f, 0f);
            transform.position += offset;
            _shakeTimer -= Time.deltaTime;
        }
        else if (transform.position != _initialSpritePosition)
        {
            transform.position = _initialSpritePosition;
        }
        */
    }

    /*
    private void OnBobaSucked(BobaPearl boba)
    {
        switch(boba.Type)
        {
            case BobaPearl.BobaType.GOOD:
                SetDelilahState(DelilahState.Drinking);
                break;

            case BobaPearl.BobaType.NASTY:
                SetDelilahState(DelilahState.Stunned);
                _shakeTimer = _shakeDuration;
                break;
        }
    }*/

    public void SetDelilahState(DelilahState state)
    {
        Debug.Log($"SetDelilahState {state}");
        if ((int)state < _delilahSprites.Length)
            _spriteRenderer.sprite = _delilahSprites[(int)state];

        switch (state)
        {
            case DelilahState.Stunned:
                break;
        }
    }
}
