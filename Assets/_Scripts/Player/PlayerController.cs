using System.Collections;
using System.Transactions;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem.Switch;

public class PlayerController : NetworkBehaviour, IHealable
{
    [Header("Movement Handling")]
    [SerializeField] PlayerInputHandler _playerInputHandler;
    [SerializeField] Rigidbody2D _rigidBody;

    [Header("Camera")]
    [SerializeField] Camera _mainCamera;

    [Header("Damage Flash")]
    [SerializeField] SpriteRenderer _shipSpriteRenderer;
    [SerializeField] Material _damageFlashMaterial;
    [SerializeField] Material _defaultShipMaterial;
    private float _damageFlashDuration = 0.1f;

    [Header("Shield")]
    [SerializeField] SpriteRenderer _shieldSpriteRenderer;

    [Header("Audio")]
    [SerializeField] AudioSource _audioSource;
    [SerializeField] AudioClip _playerShotSound;

    [Header("Event Channels")]
    [SerializeField] GunTypeEventChannelSO _gunChangeEventChannelSO;
    [SerializeField] IntEventChannelSO _healthChangeEventChannelSO;
    [SerializeField] IntToupleEventChannelSO _bulletChangeVoidEventChannelSO;
    [SerializeField] FloatEventChannelSO _movespeedChangeEventChannelSO;

    [Header("Shot Prefabs")]
    [SerializeField] GameObject _singleShotPrefab;
    [SerializeField] GameObject _doubleShotPrefab;
    [SerializeField] GameObject _tripleShotPrefab;
    [SerializeField] GameObject _quadShotPrefab;

    //Health
    public int MaxHitPoints { get; private set; }
    public int CurrentHitPoints { get; private set; }

    //Movespeed
    public float MaxMoveSpeed { get; private set; }
    public float CurrentMoveSpeed { get; private set; }

    //Bullets
    public int MaxBullets { get; private set; }
    public int CurrentBullets { get; private set; }

    //Weapon
    public float FireRate { get; private set; }
    public float ReloadRate { get; private set; }
    public GunTypeEnum CurrentGunType { get; private set; }

    //Timers
    private float _nextTimeToReload = 0f;
    private float _nextTimeToFire = 0f;

    //Buffs
    private bool _hasShield = false;

    public TeamEnum PlayerTeamEnum = TeamEnum.blue;

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        Initialize();
    }

    private void Update()
    {
        if (!IsOwner || !Application.isFocused) return;

        HandleShoot();
        HandleReload();
    }

    private void FixedUpdate()
    {
        if (!IsOwner || !Application.isFocused) return;

        HandleMovement();
    }

    private void Initialize()
    {
        SetGunType(GunTypeEnum.tripleShot, true, false);
        SetStats();

        _mainCamera = Camera.main;

        _shipSpriteRenderer.material = _defaultShipMaterial;

        _audioSource.clip = _playerShotSound;
    }

    private void RaisePlayerStatsChangedEvents()
    {
        //Fire to initialize the object pool
        _gunChangeEventChannelSO.RaiseEvent(CurrentGunType);

        //Fire to update UI
        _healthChangeEventChannelSO.RaiseEvent(CurrentHitPoints);
        _bulletChangeVoidEventChannelSO.RaiseEvent(CurrentBullets, MaxBullets);
    }

    private void SetStats()
    {
        //Set Standard Stats
        MaxHitPoints = 3;
        MaxMoveSpeed = 7f;
        MaxBullets = 3;
        FireRate = 3;
        ReloadRate = 1.75f;

        //Set current stats
        CurrentHitPoints = MaxHitPoints;
        CurrentBullets = MaxBullets;
        CurrentMoveSpeed = MaxMoveSpeed;

        RaisePlayerStatsChangedEvents();
    }

    public void SetGunType(GunTypeEnum gunType, bool initialSet, bool sendMessage)
    {
        if(CurrentGunType == gunType && !initialSet)
        {
            IncreaseBullet(true);
            return;
        }

        CurrentGunType = gunType;
    }

    private void HandleMovement()
    {
        //Calculate the direction from player to the mouse position
        Vector2 worldMousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 lookDirection = worldMousePosition - _rigidBody.position;

        if (lookDirection.magnitude > 0.1f)
        {
            float desiredAngle = Mathf.Atan2(lookDirection.y, lookDirection.x) * Mathf.Rad2Deg - 90f;
            _rigidBody.rotation = desiredAngle;

            //Move player towards Mouse Position
            float verticalInput = _playerInputHandler.MovementInput.y;
            Vector2 movement = new Vector2(verticalInput, verticalInput);
            movement *= lookDirection;
            movement *= 1f;
            movement.Normalize();
            _rigidBody.MovePosition(_rigidBody.position + movement * CurrentMoveSpeed * Time.fixedDeltaTime);
        }
    }

    private void HandleReload()
    {
        //If the magazine is not full and its time to reload, perform reload and reset the reload timer
        if (CurrentBullets < MaxBullets && _nextTimeToReload <= Time.timeSinceLevelLoad)
        {
            _nextTimeToReload = Time.timeSinceLevelLoad + (1f / ReloadRate);
            CurrentBullets++;
            _bulletChangeVoidEventChannelSO.RaiseEvent(CurrentBullets, MaxBullets);
        }

        //If the magazine is full also reset the reload timer, so the player cant instatly reload after shooting one bullet
        if (CurrentBullets == MaxBullets)
        {
            _nextTimeToReload = Time.timeSinceLevelLoad + (1f / ReloadRate);
        }
    }

    private void HandleShoot()
    {
        if (_playerInputHandler.ShootInput && _nextTimeToFire <= Time.timeSinceLevelLoad && CurrentBullets > 0)
        {
            _nextTimeToFire = Time.timeSinceLevelLoad + (1f / FireRate);
            CurrentBullets--;
            _bulletChangeVoidEventChannelSO.RaiseEvent(CurrentBullets, MaxBullets);
            PlayShotSound();

            SpawnProjectileServerRPC();
        }
    }

    [Rpc(SendTo.Server)]
    private void SpawnProjectileServerRPC()
    {
        switch (CurrentGunType)
        {
            case GunTypeEnum.singleShot:
                ServerProjectileSpawner.Singleton.HandlePlayerProjectileInstantiation(PlayerTeamEnum, this.transform.position, this.transform.rotation, GunTypeEnum.singleShot);
                break;
            case GunTypeEnum.doubleShot:
                ServerProjectileSpawner.Singleton.HandlePlayerProjectileInstantiation(PlayerTeamEnum, this.transform.position, this.transform.rotation, GunTypeEnum.doubleShot);
                break;
            case GunTypeEnum.tripleShot:
                ServerProjectileSpawner.Singleton.HandlePlayerProjectileInstantiation(PlayerTeamEnum, this.transform.position, this.transform.rotation, GunTypeEnum.tripleShot);
                break;
            case GunTypeEnum.quadShot:
                ServerProjectileSpawner.Singleton.HandlePlayerProjectileInstantiation(PlayerTeamEnum, this.transform.position, this.transform.rotation, GunTypeEnum.quadShot);
                break;
            default:
                break;
        }
    }

    private void PlayShotSound()
    {
        _audioSource.pitch = Random.Range(0.85f, 1.15f);
        _audioSource.Play();
    }

    internal void FreezePlayer()
    {
        _rigidBody.constraints = RigidbodyConstraints2D.FreezeAll;
    }

    internal void UnfreezePlayer()
    {
        _rigidBody.constraints = RigidbodyConstraints2D.FreezeRotation;
    }

    [Rpc(SendTo.Server)]
    public void TakeDamageRPC(int damageAmount)
    {
        StartCoroutine(DamageFlash());

        if (_hasShield)
        {
            RemoveShield();
            return;
        }

        CurrentHitPoints -= damageAmount;
        _healthChangeEventChannelSO.RaiseEvent(CurrentHitPoints);
        if (CurrentHitPoints <= 0)
        {
            Destroy(this.gameObject);
        }
    }

    public void ProvideHealing(int healAmount)
    {
        CurrentHitPoints += healAmount;

        if (CurrentHitPoints > MaxHitPoints)
        {
            CurrentHitPoints = MaxHitPoints;
            AddShield();
        }

        _healthChangeEventChannelSO.RaiseEvent(CurrentHitPoints);
    }

    public void TakeDamage(int damageAmount)
    {
        TakeDamageRPC(damageAmount);
    }

    public void TakeDamage(float damageAmount)
    {
        TakeDamageRPC((int)damageAmount);
    }

    public void ProvideHealing(float healAmount)
    {
        ProvideHealing((int)healAmount);
    }

    private IEnumerator DamageFlash()
    {
        _shipSpriteRenderer.material = _damageFlashMaterial;
        yield return new WaitForSeconds(_damageFlashDuration);
        _shipSpriteRenderer.material = _defaultShipMaterial;
    }

    public void IncreaseSpeed(bool sendMessage) 
    {
        MaxMoveSpeed += 0.5f;
        CurrentMoveSpeed = MaxMoveSpeed;

        _movespeedChangeEventChannelSO.RaiseEvent(CurrentMoveSpeed);

        if (!sendMessage) return;
    }

    public void IncreaseBullet(bool sendMessage)
    {
        MaxBullets += 1;
        _bulletChangeVoidEventChannelSO.RaiseEvent(CurrentBullets, MaxBullets);

        if (!sendMessage) return;
    }

    private void UpgradeGun()
    {
        if (CurrentGunType == GunTypeEnum.quadShot) return;
        CurrentGunType++;
    }

    public void AddShield()
    {
        _hasShield = true;
        _shieldSpriteRenderer.enabled = true;
    }

    public void RemoveShield()
    {
        _hasShield = false;
        _shieldSpriteRenderer.enabled = false;
    }

}
