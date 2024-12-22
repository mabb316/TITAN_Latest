using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PiercingBullet : Bullet
{
    protected override int Damage => 3;
    protected override void OnCollidedWithSomething(bool destroyFlag)
    {
        // do nothing, this is a piercing bullet
    }
}
