using System.Collections.Generic;
using UnityEngine;

public class SceneWeaponsController : Singleton<SceneWeaponsController>
{
    private List<WeaponBase> _availableWeapons = new List<WeaponBase>();

    private void Start()
    {
        WeaponBase.WeaponStatusChanged += OnWeaponAvailabilityChanged;
    }

    private void OnDisable()
    {
        WeaponBase.WeaponStatusChanged -= OnWeaponAvailabilityChanged;
    }

    private void OnWeaponAvailabilityChanged(bool isAvailable, WeaponBase weapon)
    {
        if(isAvailable)
        {
            if(!_availableWeapons.Contains(weapon))
            {
                _availableWeapons.Add(weapon);
            }
        }
        else
        {
            if(_availableWeapons.Contains(weapon))
            {
                _availableWeapons.Remove(weapon);
            }
        }
    }

    public Transform GetNearestFreeWeaponTransform(Vector3 position)
    {
        WeaponBase nearestWeapon = null;
        float minDistance = float.MaxValue;
        List<WeaponBase> weaponsToRemove = new List<WeaponBase>();

        foreach (var weapon in _availableWeapons)
        {
            if(weapon == null)
            {
                weaponsToRemove.Add(weapon);
                continue;
            }
            if (weapon.IsFree)
            {
                float distance = Vector3.Distance(weapon.transform.position, position);
                if (distance < minDistance)
                {
                    minDistance = distance;
                    nearestWeapon = weapon;
                }
            }
        }

        ClearWeaponsListFromElements(weaponsToRemove);

        return nearestWeapon?.transform;
    }

    private void ClearWeaponsListFromElements(List<WeaponBase> weaponsToRemove)
    {
        for(int i = 0; i < weaponsToRemove.Count; i++)
        {
            _availableWeapons.Remove(weaponsToRemove[i]);
        }
    }
}