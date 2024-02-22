using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(menuName = "Events/Int Touple Event Channel")]
public class IntToupleEventChannelSO : ScriptableObject
{
    public UnityAction<int, int> OnEventRaised;

    public void RaiseEvent(int value1, int value2)
    {
        if (OnEventRaised != null)
            OnEventRaised.Invoke(value1, value2);
    }
}

