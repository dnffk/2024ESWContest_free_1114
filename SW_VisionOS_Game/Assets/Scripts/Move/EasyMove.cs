using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EasyMove : IGhostMove
{
    public void Move(NavGhost navGhost)
    {
        navGhost.MoveObjectTowardsPlayer();
        //navGhost.CheckIfInView();
    }
}
