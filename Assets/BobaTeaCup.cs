using System;
using System.Collections;
using UnityEngine;

public class BobaTeaCup : MonoBehaviour
{
    public GameObject _bobaPearlPrefab;
    public Action TeaDepleted;
    public Transform _spawnPoint;
    public float _spawnRadius;

    public float _suckSpeed = .1f;
    public float _cupHeight = 2;

    public float Fill => _fillAmount;
    
    public BobaStraw _straw;
    private float _fillAmount = 1;


    private void Update()
    {
        if (_straw.IsSucking)
        {
            if (StrawIsSubmerged())
            {
                _fillAmount -= Time.deltaTime * _suckSpeed;
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

    }

    public void SpawnMix(BobaMix mix)
    {
        Debug.Log("Spawn Mix " + mix.name);
         
        for (int i = 0; i < mix.BobaPearlCount; i++)
        {
            Vector3 pos = _spawnPoint.position + (Vector3)UnityEngine.Random.insideUnitCircle * _spawnRadius;
            GameObject.Instantiate(_bobaPearlPrefab, pos, Quaternion.identity, transform);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(_spawnPoint.position, _spawnRadius);

        Gizmos.DrawLine(transform.position, transform.position + Vector3.up * _cupHeight);
    }
}
