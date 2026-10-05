using UnityEngine;

public class ActivityZone : MonoBehaviour
{
    public enum ZoneType
    {
        Desk,
        Kitchen,
        TV,
        Sofa,
        CoffeeMachine,
        Radio
    }

    public ZoneType zoneType;
}
