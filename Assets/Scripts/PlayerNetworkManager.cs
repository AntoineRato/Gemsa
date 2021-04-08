using UnityEngine;
using Mirror;

public class PlayerNetworkManager : NetworkBehaviour
{
    [SerializeField] Behaviour[] componentsToDestroy;
    [SerializeField] GameObject[] gameObjectsToDestroy;

    private Camera menuCamera;

    private void Awake()
    {
        menuCamera = Camera.main;
    }

    private void Start()
    {
        if (!isLocalPlayer)
        {
            DestroyComponents();
        }
        else
        {
            if(menuCamera != null)
            {
                menuCamera.gameObject.SetActive(false);
            }
        }
    }

    public override void OnStartClient()
    {
        base.OnStartClient();

        GameManager.RegisterPlayer(this.GetComponent<NetworkIdentity>().netId, this.GetComponent<PlayerProperties>());
    }

    private void DestroyComponents()
    {
        for (int i = 0; i < componentsToDestroy.Length; i++)
        {
            Destroy(componentsToDestroy[i]);
        }

        for (int i = 0; i < gameObjectsToDestroy.Length; i++)
        {
            Destroy(gameObjectsToDestroy[i]);
        }
    }

    private void OnDisable()
    {
        if(menuCamera != null)
        {
            menuCamera.gameObject.SetActive(true);
        }

        GameManager.UnregisterPlayer(this.GetComponent<NetworkIdentity>().netId);
    }
}
