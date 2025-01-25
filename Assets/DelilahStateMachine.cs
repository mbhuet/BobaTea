using UnityEngine;

public class DelilahStateMachine : MonoBehaviour
{
    [SerializeField] private SpriteRenderer _spriteRenderer;
    [SerializeField] private Sprite[] _delilahSprites;
    [SerializeField] private BobaTeaManager _teaManager;

    private DelilahState _delilahState = DelilahState.Default;

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
        if (_teaManager != null)
            _teaManager.BobaSucked += OnBobaSucked;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnBobaSucked(BobaPearl boba)
    {
        switch(boba.Type)
        {
            case BobaPearl.BobaType.GOOD:
                SetDelilahState(DelilahState.Drinking);
                break;

            case BobaPearl.BobaType.NASTY:
                SetDelilahState(DelilahState.Stunned);
                break;
        }
    }

    public void SetDelilahState(DelilahState state)
    {
        if ((int)state < _delilahSprites.Length)
            _spriteRenderer.sprite = _delilahSprites[(int)state];
    }
}
