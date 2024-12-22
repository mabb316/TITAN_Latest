using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponIconByType : Singleton<WeaponIconByType>
{
    [SerializeField] private List<WeaponIconByTypeDict> _weaponIcons;

    public Sprite GetWeaponSprite(WeaponType weaponType)
    {
        return _weaponIcons.Find(x => x.WeaponType == weaponType).WeaponSprite;
    }
}

[System.Serializable]
internal class WeaponIconByTypeDict
{
    public WeaponType WeaponType;
    public Sprite WeaponSprite;
}
