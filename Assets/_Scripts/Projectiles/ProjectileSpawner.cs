using System;
using Unity.Netcode;
using UnityEngine;

public class ProjectileSpawner : NetworkBehaviour
{
    public static ProjectileSpawner Singleton;

    [SerializeField] GameObject _singleShotPrefab;
    [SerializeField] GameObject _doubleShotPrefab;
    [SerializeField] GameObject _tripleShotPrefab;
    [SerializeField] GameObject _quadShotPrefab;

    private GameObject _currentShotPrefab;


    private void Awake()
    {
        if (Singleton != null && Singleton != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Singleton = this;
        }
    }

    /// <summary>   
    /// This function gets different player projectiles from the network object pool, sets their position and rotation and then spawns the projectile at the players position. 
    /// </summary>
    /// <executionorder>
    /// Handling of the children of the different projectiles needs to be done first,
    /// Otherwise the rotation and position of the player will missalign with the projectiles
    /// </executionorder>
    public void HandlePlayerProjectileInstantiation(TeamEnum playerTeamEnum, Vector3 playerPosition, Quaternion playerRotation, GunTypeEnum playerGunType)
    {
        //Set the prefab thats going to be instantiated
        switch (playerGunType)
        {
            case GunTypeEnum.singleShot:
                _currentShotPrefab = _singleShotPrefab;
                break;
            case GunTypeEnum.doubleShot:
                _currentShotPrefab = _doubleShotPrefab;
                break;
            case GunTypeEnum.tripleShot:
                _currentShotPrefab = _tripleShotPrefab;
                break;
            case GunTypeEnum.quadShot:
                _currentShotPrefab = _quadShotPrefab;
                break;
            default:
                _currentShotPrefab = _singleShotPrefab;
                break;
        }

        //Get the projectile gameobject from the pool
        GameObject playerProjectileGO = NetworkObjectPool.Singleton.GetNetworkObject(_currentShotPrefab, Vector2.zero, Quaternion.Euler(Vector3.zero)).gameObject;

        //Get the ProjectileParentController and set the prefab to enable returning the object to the network pool
        if (playerProjectileGO.TryGetComponent(out ProjectileParentController projectileParentController))
        {
            projectileParentController.ProjectilePrefab = _currentShotPrefab;
        }

        foreach (Transform child in playerProjectileGO.transform)
        {
            child.GetComponent<ProjectileController>().OwnerTeamEnum = playerTeamEnum;
        }

        //Variables for the spacing and angle offset of the different shot types
        float shotSpacing = 0.3f;
        float doubleShotMaxOffsetLeft = -0.15f;
        float quadShotMaxOffsetLeft = -0.45f;

        float tripleShotAngleOffset = 12f;
        float tripleShotMaxAngleOffsetRight = 12f;

        float quadShotAngleOffset = 5f;
        float quadShotMaxAngleOffsetRight = 7.5f;

        //Set the spacing and angles for the different projectiles
        if (playerGunType == GunTypeEnum.singleShot)
        {
            foreach (Transform child in playerProjectileGO.transform)
            {
                child.gameObject.transform.position = Vector2.zero;
                child.gameObject.SetActive(true);
            }
        }
        else if (playerGunType == GunTypeEnum.doubleShot)
        {
            foreach (Transform child in playerProjectileGO.transform)
            {
                child.gameObject.transform.position = new Vector2(doubleShotMaxOffsetLeft, 0f);
                child.gameObject.SetActive(true);
                doubleShotMaxOffsetLeft += shotSpacing;
            }
        }
        else if (playerGunType == GunTypeEnum.tripleShot)
        {
            foreach (Transform child in playerProjectileGO.transform)
            {
                child.gameObject.transform.position = Vector2.zero;
                child.gameObject.transform.rotation = Quaternion.Euler(0f, 0f, tripleShotMaxAngleOffsetRight);
                child.gameObject.SetActive(true);
                tripleShotMaxAngleOffsetRight -= tripleShotAngleOffset;
            }
        }
        else if (playerGunType == GunTypeEnum.quadShot)
        {
            foreach (Transform child in playerProjectileGO.transform)
            {
                child.gameObject.transform.position = new Vector2(quadShotMaxOffsetLeft, 0f);
                child.gameObject.transform.rotation = Quaternion.Euler(0f, 0f, quadShotMaxAngleOffsetRight);
                child.gameObject.SetActive(true);
                quadShotMaxOffsetLeft += shotSpacing;
                quadShotMaxAngleOffsetRight -= quadShotAngleOffset;

            }
        }

        //Position the projectile at the playersposition
        playerProjectileGO.transform.position = playerPosition;
        playerProjectileGO.transform.rotation = playerRotation;

        //Spawn the object
        playerProjectileGO.GetComponent<NetworkObject>().Spawn();
    }
}
