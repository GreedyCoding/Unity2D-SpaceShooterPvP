using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(menuName = "Events/GunType Event Channel")]
public class GunTypeEventChannelSO : ScriptableObject
{
    public UnityAction<GunTypeEnum> OnEventRaised;

    public void RaiseEvent(GunTypeEnum enumerator)
    {
        if (OnEventRaised != null)
            OnEventRaised.Invoke(enumerator);
    }
}

