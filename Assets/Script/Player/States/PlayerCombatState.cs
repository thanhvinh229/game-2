using UnityEngine;

public class PlayerCombatState : PlayerMoveState
{
 private bool sheathWeapon;
    public PlayerCombatState(PlayerController player) : base(player) { }

    public override void Enter()
    {
        base.Enter();
        sheathWeapon = false;
    }   

    
        }
    
 
   
 
    
 
 

