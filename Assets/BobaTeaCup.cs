using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BobaTeaCup : MonoBehaviour
{
    public BobaPearl _bobaPearlPrefab;
    public BobaPearl _nastyPearlPrefab;

    public Action TeaDepleted;
    public Transform _spawnPoint;
    public Image  _fillImage;
    public float _spawnRadius;

    public float _suckSpeed = .1f;
    public float _cupHeight = 2;

    public float Fill => _fillAmount;
    
    public BobaStraw _straw;
    private float _fillAmount = 1;
    public float _depletedFillAmount = .1f;
    public float _refillFillAmount = .9f;

    private List<BobaPearl> _spawnedPearls = new List<BobaPearl>();


    private void Update()
    {
        if (_straw.IsSucking)
        {
            if (StrawIsSubmerged())
            {
                _fillAmount -= Time.deltaTime * _suckSpeed;
                ReflectFillAmount();
                if(_fillAmount < _depletedFillAmount)
                {
                    if (TeaDepleted != null) TeaDepleted.Invoke();
                }
            }
        }
    }

    private bool StrawIsSubmerged() 
    {
        float waterLevel = transform.position.y + _cupHeight * _fillAmount;
        return _straw.TipPosition.y < waterLevel;
    }

    private void ReflectFillAmount()
    {
        _fillImage.fillAmount = _fillAmount;
    }

    public void Refill()
    {
        _fillAmount = _refillFillAmount;
        ReflectFillAmount();

    }

    public void Clean()
    {
        for (int i = _spawnedPearls.Count - 1; i >= 0; i--)
        {
            var boba = _spawnedPearls[i];
            if (boba != null)
            {
                GameObject.Destroy(boba.gameObject);
            }
        }
        _spawnedPearls.Clear();
    }


    public void SpawnMix(BobaMix mix)
    {
        Debug.Log("Spawn Mix " + mix.name);
         
        for (int i = 0; i < mix.BobaPearlCount; i++)
        {
            Vector3 pos = _spawnPoint.position + (Vector3)UnityEngine.Random.insideUnitCircle * _spawnRadius;
            _spawnedPearls.Add(GameObject.Instantiate(_bobaPearlPrefab, pos, Quaternion.identity, transform));
        }
        for (int i = 0; i < mix.NastyPearlCount; i++)
        {
            Vector3 pos = _spawnPoint.position + (Vector3)UnityEngine.Random.insideUnitCircle * _spawnRadius;
            _spawnedPearls.Add(GameObject.Instantiate(_nastyPearlPrefab, pos, Quaternion.identity, transform));
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(_spawnPoint.position, _spawnRadius);

        Gizmos.DrawLine(transform.position, transform.position + Vector3.up * _cupHeight);
    }
}
