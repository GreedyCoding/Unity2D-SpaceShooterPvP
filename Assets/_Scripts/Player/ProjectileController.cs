using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerProjectileController : MonoBehaviour
{
    private PlayerController _playerController;
    private Rigidbody2D _rigidbody;

    public float ProjectileDamage { get; private set; }

    private void Start()
    {
        GetComponents();
        ProjectileDamage = 1f;
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
        else if (other.gameObject.CompareTag(Constants.BORDER_LEFT_TAG) || other.gameObject.CompareTag(Constants.BORDER_RIGHT_TAG) || other.gameObject.CompareTag(Constants.BORDER_TOP_TAG) || other.gameObject.CompareTag(Constants.BORDER_BOTTOM_TAG))
        {
            this.gameObject.SetActive(false);
        }
    }
}
