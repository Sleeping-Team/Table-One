using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using Random = UnityEngine.Random;

public class CustomerSpawn : SingletonNetwork<CustomerSpawn>
{
    [SerializeField] private PositionProperties[] _waitPosition;
    [SerializeField] private Customer[] _customerPrefabs;
    [SerializeField] private float _spawnDelay;

    private bool gameover = false;
    private int _customersQuantity;
    
    private void Start()
    {
        _customersQuantity = _customerPrefabs.Length;

        NetworkManager.Singleton.OnServerStarted += OnServerStarted;
    }
    
    private void OnServerStarted()
    {
        StartCoroutine(DoSpawn());
    }
    
    IEnumerator DoSpawn()
    {
        while (!gameover)
        {
            int index = PositionProperties.FindEmpty(_waitPosition);
            Debug.Log(index);
            if (index >= 0)
            {
                _waitPosition[index].SetOccupied(true);
                Customer customer = Instantiate(_customerPrefabs[Random.Range(0, _customersQuantity)], _waitPosition[index].Location);
                customer.transform.localPosition = Vector3.zero;

                customer.name = $"{customer.name}_{customer.NetworkObject.NetworkObjectId}";

                NetworkObject customerNetwork = customer.GetComponent<NetworkObject>();
                if(!customerNetwork.IsSpawned) customerNetwork.Spawn();
            }
            else
            {
                Debug.Log("Location is full");
            }

            yield return new WaitForSeconds(_spawnDelay);
        }
    }

    private void OnDestroy()
    {
        if (NetworkManager.Singleton != null)
        {
            NetworkManager.Singleton.OnServerStarted -= OnServerStarted;
        }
    }
}
