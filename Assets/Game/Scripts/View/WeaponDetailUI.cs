using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WeaponDetailUI : MonoBehaviour
{
    [SerializeField] private GameObject _visuals;
    [SerializeField] private TextMeshProUGUI _bulletCountText;
    [SerializeField] private Image _weaponImage;

    private void Start()
    {
        RegisterEvents();
    }

    private void OnDestroy()
    {
        UnregisterEvents();
    }

    private void RegisterEvents()
    {
        PlayerCharacter.BulletCountChanged += OnBulletCountChanged;
        PlayerCharacter.WeaponReceived += OnWeaponReceived;
        PlayerCharacter.WeaponLost += OnWeaponLost;
    }

    private void UnregisterEvents()
    {
        PlayerCharacter.BulletCountChanged -= OnBulletCountChanged;
        PlayerCharacter.WeaponReceived -= OnWeaponReceived;
        PlayerCharacter.WeaponLost -= OnWeaponLost;
    }

    private void OnWeaponReceived(WeaponType weaponType)
    {
        _visuals.SetActive(true);

        _weaponImage.sprite = WeaponIconByType.Instance.GetWeaponSprite(weaponType);
    }

    private void OnWeaponLost()
    {
        _visuals.SetActive(false);
    }

    private void OnBulletCountChanged(int bulletCount, int maxBulletCount)
    {
        _bulletCountText.text = $"{bulletCount}/{maxBulletCount}";
    }
}
