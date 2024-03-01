using System.Runtime.CompilerServices;
using Unity.Netcode;
using UnityEngine;

public class ProjectileController : NetworkBehaviour
{
    private Rigidbody2D _rigidbody;

    public float ProjectileDamage { get; private set; }
    public float ProjectileSpeed { get; private set; }

    public TeamEnum OwnerTeamEnum;


    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        GetComponents();
        ProjectileDamage = 1f;
        ProjectileSpeed = 20f;
    }

    private void FixedUpdate()
    {
        _rigidbody.velocity = transform.up * ProjectileSpeed;
    }

    private void GetComponents()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!IsServer) return;

        if (other.gameObject.CompareTag(Constants.PLAYER_TAG))
        {
            TeamEnum hitTeamEnum = other.GetComponent<PlayerController>().PlayerTeamEnum;
            if (OwnerTeamEnum == hitTeamEnum) return;

            other.GetComponent<IDamageable>().TakeDamage(ProjectileDamage);
            this.gameObject.SetActive(false);
        }
        else if(other.gameObject.CompareTag(Constants.BORDER_LEFT_TAG) || other.gameObject.CompareTag(Constants.BORDER_RIGHT_TAG) || other.gameObject.CompareTag(Constants.BORDER_TOP_TAG) || other.gameObject.CompareTag(Constants.BORDER_BOTTOM_TAG))
        {
            this.gameObject.SetActive(false);
        }
    }
}
