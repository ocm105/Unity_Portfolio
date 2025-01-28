using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponTrigger : MonoBehaviour
{
    private PlayerInfo playerInfo;

    private void Awake()
    {
        playerInfo = FindObjectOfType<PlayerInfo>();
    }

    private void OnTriggerEnter(Collider other)
    {
        // if (other.CompareTag("Monster") && playerInfo._playerAniControl.playerAniState == PlayerAniState.Attack)
        // {
        //     IDamage target = other.GetComponent<IDamage>();
        //     if (target != null)
        //         target.OnDamage(playerInfo.playerData.atk);
        // }
    }
}
