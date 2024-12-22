using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shotgun : WeaponBase
{
    protected override void ShootAction(Vector3 directionToShoot)
    {
        base.ShootAction(directionToShoot);

        GameObject bulletObjectLeft = Instantiate(_bulletPrefab, _shootingPoint.position, Quaternion.identity);
        GameObject bulletObjectRight = Instantiate(_bulletPrefab, _shootingPoint.position, Quaternion.identity);

        Bullet bulletLeft = bulletObjectLeft.GetComponent<Bullet>();
        Bullet bulletRight = bulletObjectRight.GetComponent<Bullet>();

        SetBulletProperties(bulletLeft, directionToShoot - Vector3.right * .2f);
        SetBulletProperties(bulletRight, directionToShoot + Vector3.right * .2f);
    }
}
