using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MediumMove : IGhostMove
{
    private System.Action<NavGhost> selectedMove;

    public MediumMove()
    {
        System.Action<NavGhost>[] moves = new System.Action<NavGhost>[]
        {
            MoveTowardsPlayer,
            CheckIfInView,
            UpdateVisibilityAndMove
        };
        selectedMove = moves[Random.Range(0, moves.Length)];
    }

    public void Move(NavGhost navGhost)
    {
        selectedMove(navGhost);
    }

    private void MoveTowardsPlayer(NavGhost navGhost)
    {
        navGhost.MoveObjectTowardsPlayer();
    }

    private void CheckIfInView(NavGhost navGhost)
    {
        navGhost.CheckIfInView();
    }

    public void UpdateVisibilityAndMove(NavGhost navGhost)
    {
        navGhost.UpdateVisibility();
        navGhost.GhostMovement();
    }
}
