using Unity.Netcode;
using Unity.VisualScripting;
using UnityEditor.MPE;
using UnityEngine;

public class ProjectileParentController : NetworkBehaviour
{
    public GameObject ProjectilePrefab;

    private NetworkObject _networkObject;

    void Update()
    {
        _networkObject = GetComponent<NetworkObject>();
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
            _networkObject.Despawn(false);
            NetworkObjectPool.Singleton.ReturnNetworkObject(_networkObject, ProjectilePrefab);
            this.gameObject.SetActive(false);
        }
    }
}
