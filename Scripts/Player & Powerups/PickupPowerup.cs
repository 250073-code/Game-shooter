using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickupPowerup : MonoBehaviour
{
    // Delegate and event to move all powerups towards the player
    public delegate void MovePowerupsTowardsPlayer();
    public static event MovePowerupsTowardsPlayer movePowerupsTowardsPlayer;
    // Delegate and event to stop moving powerups towards the player
    public delegate void DontMoveTowardsPlayer();
    public static event DontMoveTowardsPlayer dontMoveTowardsPlayer;

    // Update is called once per frame
    void Update()
    {
        // When C is pressed, trigger event to move powerups towards player
        if (Input.GetKeyDown(KeyCode.C))
        {
            if (movePowerupsTowardsPlayer != null)
            {
                movePowerupsTowardsPlayer();
            }
        }
        
        // When C is released, trigger event to stop moving powerups
        if (Input.GetKeyUp(KeyCode.C))
        {
            if (dontMoveTowardsPlayer != null)
            {
                dontMoveTowardsPlayer();
            }
        }
    }
}
