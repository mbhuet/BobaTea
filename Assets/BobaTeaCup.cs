using System;
using System.Collections;
using UnityEngine;

public class BobaTeaCup : MonoBehaviour
{
    public GameObject _bobaPearlPrefab;
    public Action TeaDepleted;
    public Transform _spawnPoint;
    public float _spawnRadius;

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
    }
}
