using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class sc_PlayerManager : NetworkBehaviour
{
    [SerializeField]
    [Tooltip("Layer that player can hit by shooting.")]
    private LayerMask layerToShoot;

    [SerializeField]
    private sc_Weapon playerWeapon;

    //Player camera, to have the orientation of the crosshair
    private Transform playerCamera;
    private sc_PlayerProperties playerProperties;

    private void Awake()
    {
        playerCamera = this.GetComponent<sc_PlayerController>().playerCamera;

        if (playerCamera == null)
        {
            Debug.LogError("sc_PlayerManager : player head missing, component disabled.");
            this.enabled = false;
        }

        this.playerProperties = this.GetComponent<sc_PlayerProperties>();
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetButtonDown("Fire1"))
        {
            Shoot();
        }
    }

    [Client]
    private void Shoot()
    {
        if (!playerProperties.isDead)
        {
            RaycastHit hit;

            if (Physics.Raycast(playerCamera.position, playerCamera.forward, out hit, playerWeapon.range, layerToShoot))
            {
                if (hit.collider.tag == "Player")
                {
                    CMDPlayerShot(hit.collider.transform.root.GetComponent<NetworkIdentity>().netId, playerWeapon.damage);
                }
            }
        }
    }

    [Command]
    private void CMDPlayerShot(uint playerNetworkIDValue, float weaponDamage)
    {
        Debug.Log("Touché : " + playerNetworkIDValue);

        GameManager.GetPlayerProperties(playerNetworkIDValue).RPCTakeDamage(weaponDamage);
    }
}
