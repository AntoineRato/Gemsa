using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class sc_PlayerManager : NetworkBehaviour
{
    [SerializeField]
    [Tooltip("Player head, to have the orientation of the crosshair.")]
    private Transform playerHead;
    [SerializeField]
    [Tooltip("Layer that player can hit by shooting.")]
    private LayerMask layerToShoot;

    [SerializeField]
    private sc_Weapon playerWeapon;

    private sc_PlayerProperties playerProperties;

    private void Awake()
    {
        if (playerHead == null)
        {
            Debug.LogError("sc_PlayerManager : player head missing, component disabled.");
            this.enabled = false;
        }

        this.playerProperties = this.GetComponent<sc_PlayerProperties>();
    }

    // Start is called before the first frame update
    void Start()
    {
        
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

            if (Physics.Raycast(playerHead.position, playerHead.forward, out hit, playerWeapon.range, layerToShoot))
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
