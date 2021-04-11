using UnityEngine;
using Mirror;

public class sc_PlayerNetworkManager : NetworkBehaviour
{
    [SerializeField] Behaviour[] componentsToDestroy;
    [SerializeField] GameObject[] gameObjectsToDestroy;

    [Tooltip("Layer name for other online players")]
    [SerializeField] private const string onlinePlayerLayerName = "OnlinePlayer";
    [Tooltip("Layer name for other local players")]
    [SerializeField] private const string localPlayerLayerName = "LocalPlayer";
    [Tooltip("Layer name for object that will not be display locally")]
    [SerializeField] private const string hideVisualLayerName = "HideVisual";
    [Tooltip("Object reference for visual skin that will not be display locally")]
    [SerializeField] private Transform[] listObjectHideVisual;

    private Camera menuCamera;
    private sc_SimpleCrosshair crosshair;

    private void Awake()
    {
        menuCamera = Camera.main;
        crosshair = this.GetComponent<sc_SimpleCrosshair>();
    }

    private void Start()
    {
        if (!isLocalPlayer)
        {
            DestroyComponentsAndGameObjects();
        }
        else
        {
            if(menuCamera != null)
            {
                menuCamera.gameObject.SetActive(false);
            }

            crosshair.GenerateCrosshair();
        }

        AssignLayers();
    }

    public override void OnStartClient()
    {
        base.OnStartClient();

        GameManager.RegisterPlayer(this.GetComponent<NetworkIdentity>().netId, this.GetComponent<sc_PlayerProperties>());
    }

    private void SetLayerNoDraw()
    {
        for (int i = 0; i < listObjectHideVisual.Length; i++)
        {
            foreach (MeshRenderer childRenderer in listObjectHideVisual[i].GetComponentsInChildren<MeshRenderer>())
            {
                childRenderer.gameObject.layer = LayerMask.NameToLayer(hideVisualLayerName);
            }

            foreach (SkinnedMeshRenderer childRenderer in listObjectHideVisual[i].GetComponentsInChildren<SkinnedMeshRenderer>())
            {
                childRenderer.gameObject.layer = LayerMask.NameToLayer(hideVisualLayerName);
            }
        }
    }

    private void DestroyComponentsAndGameObjects()
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

    private void AssignLayers()
    {
        foreach (Transform child in this.gameObject.GetComponentsInChildren<Transform>())
        {
            if (!isLocalPlayer)
            {
                child.gameObject.layer = LayerMask.NameToLayer(onlinePlayerLayerName);
            }
            else
            {
                child.gameObject.layer = LayerMask.NameToLayer(localPlayerLayerName);
            }
        }

        if(isLocalPlayer)
        {
            SetLayerNoDraw();
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
