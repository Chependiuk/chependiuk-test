using UnityEngine;
using System.Collections.Generic;

public class CameraFollow : MonoBehaviour
{
    [Header("Camera Settings")]
    public Transform target;
    public Vector3 offset = new Vector3(0f, 2f, -5f);
    public float smoothSpeed = 5f;

    [Header("Occlusion Settings")]
    public LayerMask occlusionMask;
    public float checkRadius = 0.5f;
    public float fadeAlpha = 0.3f;

    private Dictionary<GameObject, OcclusionInfo> hiddenObjects = new Dictionary<GameObject, OcclusionInfo>();
    private List<GameObject> objectsToRestore = new List<GameObject>();

    void LateUpdate()
    {
        if (target == null) return;

        // Camera movement
        Vector3 desiredPosition = target.position + offset;
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);
        transform.position = smoothedPosition;
        transform.LookAt(target);

        // Occlusion handling
        HandleOcclusion();
    }

    void HandleOcclusion()
    {
        // Restore objects that are no longer occluding
        RestoreObjects();

        // Find new occluding objects
        RaycastHit[] hits = Physics.SphereCastAll(
            transform.position,
            checkRadius,
            (target.position - transform.position).normalized,
            Vector3.Distance(transform.position, target.position),
            occlusionMask
        );

        // Hide new occluding objects
        foreach (RaycastHit hit in hits)
        {
            if (hit.collider.gameObject != target.gameObject && !hiddenObjects.ContainsKey(hit.collider.gameObject))
            {
                HideObject(hit.collider.gameObject);
            }
        }
    }

    void HideObject(GameObject obj)
    {
        // Skip if already hidden or doesn't have renderer
        Renderer renderer = obj.GetComponent<Renderer>();
        if (renderer == null) return;

        // Store original state
        OcclusionInfo info = new OcclusionInfo
        {
            renderer = renderer,
            collider = obj.GetComponent<Collider>(),
            materials = new Material[renderer.materials.Length],
            originalAlpha = new float[renderer.materials.Length]
        };

        // Copy materials and store original alpha
        for (int i = 0; i < renderer.materials.Length; i++)
        {
            info.materials[i] = new Material(renderer.materials[i]);
            info.originalAlpha[i] = renderer.materials[i].color.a;

            // Apply fade
            Color color = renderer.materials[i].color;
            color.a = fadeAlpha;
            info.materials[i].color = color;

            // Enable transparency
            info.materials[i].SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
            info.materials[i].SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
            info.materials[i].EnableKeyword("_ALPHABLEND_ON");
            info.materials[i].renderQueue = 3000;
        }

        // Apply new materials
        renderer.materials = info.materials;

        // Optionally disable collider
        if (info.collider != null)
            info.collider.enabled = false;

        hiddenObjects.Add(obj, info);
    }

    void RestoreObjects()
    {
        objectsToRestore.Clear();

        // Find objects to restore
        foreach (var pair in hiddenObjects)
        {
            if (!IsOccluding(pair.Key))
            {
                objectsToRestore.Add(pair.Key);
            }
        }

        // Restore found objects
        foreach (GameObject obj in objectsToRestore)
        {
            OcclusionInfo info = hiddenObjects[obj];

            // Restore original materials
            if (info.renderer != null)
            {
                Material[] originalMats = new Material[info.materials.Length];
                for (int i = 0; i < originalMats.Length; i++)
                {
                    Color color = info.materials[i].color;
                    color.a = info.originalAlpha[i];
                    info.materials[i].color = color;
                }
                info.renderer.materials = info.materials;
            }

            // Restore collider
            if (info.collider != null)
                info.collider.enabled = true;

            hiddenObjects.Remove(obj);
        }
    }

    bool IsOccluding(GameObject obj)
    {
        return Physics.Linecast(transform.position, target.position, out RaycastHit hit, occlusionMask) &&
               hit.collider.gameObject == obj;
    }

    private class OcclusionInfo
    {
        public Renderer renderer;
        public Collider collider;
        public Material[] materials;
        public float[] originalAlpha;
    }
}