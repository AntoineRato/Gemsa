using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class sc_PlayerProperties : NetworkBehaviour
{
    [SerializeField] private float maxHealth = 100f;

    [SyncVar]
    private float currentHealth;

    [SyncVar]
    private bool _isDead = false;
    public bool isDead
    {
        get { return _isDead; }
        protected set { _isDead = value; }
    }

    private void Awake()
    {
        ResetValues();
    }

    public void ResetValues()
    {
        this.currentHealth = maxHealth;
        isDead = false;
    }

    private IEnumerator Respawn()
    {
        yield return new WaitForSeconds(GameManager.instance.minigameSettings.respawnTimer);

        ResetValues();
        Transform spawnPoint = NetworkManager.singleton.GetStartPosition();
        this.transform.position = spawnPoint.position;
        this.transform.rotation = spawnPoint.rotation;
    }

    [ClientRpc]
    public void RPCTakeDamage(float amountOfDamage)
    {
        if (!isDead)
        {
            currentHealth -= amountOfDamage;

            if (currentHealth <= 0)
            {
                Die();
            }
        }
    }

    private void Die()
    {
        isDead = true;

        StartCoroutine(Respawn());
    }
}
