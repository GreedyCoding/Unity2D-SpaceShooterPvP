using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerProjectileController : MonoBehaviour
{
    PlayerController _playerController;
    Rigidbody2D _rigidbody;

    public float ProjectileDamage { get; private set; }

    private void Start()
    {
        GetComponents();
        SetProjectileDamage();
    }

    private void Update()
    {
        _rigidbody.velocity = transform.up * _playerController.ProjectileSpeed;
    }

    private void GetComponents()
    {
        _playerController = GameObject.Find(Constants.PLAYER_TAG).GetComponent<PlayerController>();
        _rigidbody = GetComponent<Rigidbody2D>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.gameObject.CompareTag(Constants.ENEMY_TAG))
        {
            other.GetComponent<IDamageable>().TakeDamage(ProjectileDamage);
            this.gameObject.SetActive(false);          
        }
        else if (other.gameObject.CompareTag(Constants.WALL_LEFT_TAG) || other.gameObject.CompareTag(Constants.WALL_RIGHT_TAG))
        {
            this.gameObject.SetActive(false);
        }
     }

    private void SetProjectileDamage()
    {
        switch (_playerController.CurrentGunType)
        {
            case GunTypeEnum.singleShot:
                ProjectileDamage = 5f;
                break;
            case GunTypeEnum.doubleShot:
                ProjectileDamage = 6f;
                break;
            case GunTypeEnum.tripleShot:
                ProjectileDamage = 8f;
                break;
            case GunTypeEnum.quadShot:
                ProjectileDamage = 7f;
                break;
        }
    }
}
