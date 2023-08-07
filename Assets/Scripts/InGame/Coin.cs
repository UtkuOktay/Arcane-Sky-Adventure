using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coin : Collectable
{

    //Specifies what is to be done when the coin is collected.
    protected override void Collect(GameObject collector)
    {
        collector.GetComponent<Player>().IncrementNumberOfCoins();
    }
}
