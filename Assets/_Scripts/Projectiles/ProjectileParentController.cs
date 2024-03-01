using Unity.Netcode;
using Unity.VisualScripting;
using UnityEditor.MPE;
using UnityEngine;

public class ProjectileParentController : NetworkBehaviour
{
    public GameObject ProjectilePrefab;

    private NetworkObject _networkObject;

    private void OnEnable()
    {
        _networkObject = GetComponent<NetworkObject>();

    }
    void FixedUpdate()
    {
        DisableIfChildrenAreDead();
    }

    private void DisableIfChildrenAreDead()
    {
        bool anyChildrenActive = false;
        foreach (Transform child in transform)
        {
            if (child.gameObject.activeSelf == true)
            {
                anyChildrenActive = true;
            }
        }
        if (!anyChildrenActive)
        {
            DespawnAndReturnNetworkObject();
        }
    }

    private void DespawnAndReturnNetworkObject()
    {
        if (!IsServer) return;

        this.gameObject.SetActive(false);
        _networkObject.Despawn(false);
        NetworkObjectPool.Singleton.ReturnNetworkObject(_networkObject, ProjectilePrefab);
    }
}
