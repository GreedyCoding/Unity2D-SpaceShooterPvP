using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPoolPlayerProjectiles : MonoBehaviour
{
    //Singleton
    public static ObjectPoolPlayerProjectiles SharedInstance;

    //Event Channel
    [SerializeField] GunTypeEventChannelSO _gunChangeVoidEventChannelSO;

    [SerializeField] GameObject _singleShotPrefab;
    [SerializeField] GameObject _doubleShotPrefab;
    [SerializeField] GameObject _tripleShotPrefab;
    [SerializeField] GameObject _quadShotPrefab;

    //Objects to pool
    [SerializeField] int _amountToPool;
    private GameObject _objectToPool;

    //List of pooled objects
    public List<GameObject> pooledObjects;

    void Awake()
    {
        if(SharedInstance == null)
        {
            SharedInstance = this;
        }

    }

    void Start()
    {
        InitialPoolSetup();
        _gunChangeVoidEventChannelSO.OnEventRaised += SetObjectAndAmount;
        _gunChangeVoidEventChannelSO.OnEventRaised += SetupPool;
    }

    void OnDisable()
    {
        _gunChangeVoidEventChannelSO.OnEventRaised -= SetObjectAndAmount;
        _gunChangeVoidEventChannelSO.OnEventRaised -= SetupPool;
    }

    private void InitialPoolSetup()
    {
        SetObjectAndAmount(GunTypeEnum.singleShot);
        SetupPool(GunTypeEnum.singleShot);
    }

    void SetupPool(GunTypeEnum enumerator)
    {
        if(pooledObjects.Count > 0)
        {
            foreach(GameObject pooledObject in pooledObjects)
            {
                Destroy(pooledObject);
            }
        }

        pooledObjects = new List<GameObject>();
        GameObject tempPoolItem;
        for (int i = 0; i < _amountToPool; i++)
        {
            tempPoolItem = Instantiate(_objectToPool);
            tempPoolItem.SetActive(false);
            tempPoolItem.transform.parent = this.transform;
            pooledObjects.Add(tempPoolItem);
        }
    }

    void SetObjectAndAmount(GunTypeEnum enumerator)
    {
        switch (enumerator)
        {
            case GunTypeEnum.singleShot:
                _objectToPool = _singleShotPrefab;
                break;
            case GunTypeEnum.doubleShot:
                _objectToPool = _doubleShotPrefab;
                break;
            case GunTypeEnum.tripleShot:
                _objectToPool = _tripleShotPrefab;
                break;
            case GunTypeEnum.quadShot:
                _objectToPool = _quadShotPrefab;
                break;
        }
    }

    public GameObject GetPooledObject()
    {
        for(int i = 0; i < pooledObjects.Count; i++)
        {
            if (!pooledObjects[i].activeInHierarchy)
            {
                //Reset position and rotation of the pooled object and its children
                pooledObjects[i].transform.position = Vector3.zero;
                pooledObjects[i].transform.rotation = Quaternion.identity;
                foreach(Transform child in pooledObjects[i].transform)
                {
                    child.transform.position = Vector3.zero;
                    child.transform.rotation = Quaternion.identity;
                }

                return pooledObjects[i];
            }
        }
        return null;
    }
}
