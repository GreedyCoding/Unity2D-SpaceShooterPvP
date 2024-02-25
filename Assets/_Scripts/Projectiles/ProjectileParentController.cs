using Unity.Netcode;
using UnityEngine;

public class ProjectileParentController : NetworkBehaviour
{
    // Update is called once per frame
    void Update()
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
            this.gameObject.SetActive(false);
        }
    }
}
