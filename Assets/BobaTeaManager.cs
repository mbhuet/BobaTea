using DG.Tweening;
using System;
using System.Collections;
using UnityEngine;

public class BobaTeaManager : MonoBehaviour
{
    public BobaMix[] _bobaMixes;
    public BobaTeaCup _bobaTeaCup;
    public BobaStraw _straw;

    public Action<BobaPearl> BobaSucked;
    public Transform _offScreenTarget;
    public Transform _onScreenTarget;

    public Action OnTeaEmpty;
    public Action TeaRefilled;
    public AudioClip _emptyCupClip;

    private bool _isServingTea;

    private void Start()
    {
        Debug.Log("BobaTeaManager Start");
        _bobaTeaCup.TeaDepleted += OnTeaCompleted;
        _bobaTeaCup.gameObject.SetActive(false);
    }

    public void OnTeaCompleted()
    {
        RemoveEmptyTea();
    }

    public void BeginTeaService()
    {
        _isServingTea = true;
        ServeFreshTea();
    }

    public void EndTeaService()
    {
        _isServingTea = false;
        _straw.SetInteractable(false);
    }

    private BobaMix GetRandomMix()
    {
        return _bobaMixes[UnityEngine.Random.Range(0, _bobaMixes.Length)];
    }

    public void ServeFreshTea()
    {
        _bobaTeaCup.gameObject.SetActive(true);
        _bobaTeaCup.transform.position = _offScreenTarget.position;
        _bobaTeaCup.SpawnMix(GetRandomMix());
       StartCoroutine(ServeFreshTeaRoutine());
        _bobaTeaCup.Refill();
    }

    private IEnumerator ServeFreshTeaRoutine()
    {
        _bobaTeaCup.transform.DOMove(_onScreenTarget.position, 1).SetEase(Ease.OutBounce);
        yield return new WaitForSeconds(1);
        _straw.SetInteractable(true);
    }

    public void RemoveEmptyTea()
    {
        _straw.SetInteractable(false);
        if(_emptyCupClip != null )
        {
            AudioSource.PlayClipAtPoint(_emptyCupClip, Camera.main.transform.position);
        }
       
        StartCoroutine(RemoveEmptyTeaRoutine());
    }

    private IEnumerator RemoveEmptyTeaRoutine()
    {
        _bobaTeaCup.transform.DOShakeRotation(1, 30);
        yield return new WaitForSeconds(1);
        _bobaTeaCup.transform.DOMove(_offScreenTarget.position, 1).SetEase(Ease.OutExpo);
        yield return new WaitForSeconds(1);
        _bobaTeaCup.gameObject.SetActive(false);

        if (_isServingTea)
        {
            ServeFreshTea();
        }
    }
}
