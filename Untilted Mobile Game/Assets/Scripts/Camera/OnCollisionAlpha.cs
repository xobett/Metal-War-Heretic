using System.Collections.Generic;
using UnityEngine;

public class OnCollisionAlpha : MonoBehaviour
{
    public LayerMask treeLayer;      // Layer for the trees
    public Material fadeMaterial;    // Reference to the custom transparent material (set in the Inspector)
    public float fadeThreshold = 5f; // Adjustable fade threshold (set in the Inspector)
    public float radialDetectionRadius = 5f; // Radial radius for detecting trees around the camera

    public Transform target;

    // Dictionary to store original materials of faded trees
    private Dictionary<Renderer, Material> fadedTrees = new Dictionary<Renderer, Material>();

    void Start()
    {
        if (target == null)
        {
            Debug.LogError("Player not found! Ensure the player GameObject is assigned to the correct layer.");
            return;
        }

        // Ensure fadeMaterial is assigned via the Inspector
        if (fadeMaterial == null)
        {
            Debug.LogError("Fade material is not assigned! Please assign a transparent material in the Inspector.");
            return;
        }
    }

    void Update()
    {
        if (target == null) return;

        Vector3 cameraPosition = Camera.main.transform.position;
        Vector3 direction = (target.position - cameraPosition).normalized;

        HashSet<Renderer> currentHitTrees = new HashSet<Renderer>();

        // Cast a ray to detect trees blocking the player in the camera's forward direction
        if (Physics.Raycast(cameraPosition, direction, out RaycastHit hit, Mathf.Infinity, treeLayer))
        {
            Renderer treeRenderer = hit.collider.GetComponent<Renderer>();

            if (treeRenderer != null)
            {
                float distanceToCamera = Vector3.Distance(cameraPosition, treeRenderer.transform.position);

                // If the tree is within the fade threshold, fade it
                if (distanceToCamera < fadeThreshold || IsCameraInsideTree(cameraPosition, treeRenderer))
                {
                    currentHitTrees.Add(treeRenderer);

                    // If the tree hasn't been faded yet, fade it
                    if (!fadedTrees.ContainsKey(treeRenderer))
                    {
                        // Store the original material and assign the fade material
                        fadedTrees[treeRenderer] = treeRenderer.material;
                        treeRenderer.material = fadeMaterial;
                    }

                    // Directly set the opacity to 0.2 (no gradual fading)
                    Color color = treeRenderer.material.color;
                    color.a = 0.2f;
                    treeRenderer.material.color = color;
                }
            }
        }

        // Cast additional rays in a radial pattern for overhead detection
        DetectTreesInRadialPattern(cameraPosition, currentHitTrees);

        // Additional check to fade tree based on mesh renderer visibility
        CheckForTreeBehindCamera(currentHitTrees);

        // Restore trees that are no longer hit
        RestoreUnhitTrees(currentHitTrees);
    }

    // Cast multiple rays in all directions around the camera to detect trees
    void DetectTreesInRadialPattern(Vector3 cameraPosition, HashSet<Renderer> currentHitTrees)
    {
        Collider[] hitColliders = Physics.OverlapSphere(cameraPosition, radialDetectionRadius, treeLayer);

        foreach (var collider in hitColliders)
        {
            Renderer treeRenderer = collider.GetComponent<Renderer>();
            if (treeRenderer != null && !currentHitTrees.Contains(treeRenderer))
            {
                // If the tree hasn't been faded yet, fade it
                if (!fadedTrees.ContainsKey(treeRenderer))
                {
                    // Store the original material and assign the fade material
                    fadedTrees[treeRenderer] = treeRenderer.material;
                    treeRenderer.material = fadeMaterial;
                }

                // Directly set the opacity to 0.2 (no gradual fading)
                Color color = treeRenderer.material.color;
                color.a = 0.2f;
                treeRenderer.material.color = color;

                currentHitTrees.Add(treeRenderer);
            }
        }
    }

    // Check if the camera is inside the tree mesh
    bool IsCameraInsideTree(Vector3 cameraPosition, Renderer treeRenderer)
    {
        // Assuming the tree has a Collider component (e.g., BoxCollider, MeshCollider)
        Collider treeCollider = treeRenderer.GetComponent<Collider>();

        if (treeCollider != null)
        {
            return treeCollider.bounds.Contains(cameraPosition);
        }

        return false;
    }

    void CheckForTreeBehindCamera(HashSet<Renderer> currentHitTrees)
    {
        // Perform a second check to fade trees based on their mesh renderer
        foreach (var treeRenderer in fadedTrees.Keys)
        {
            if (!currentHitTrees.Contains(treeRenderer))
            {
                Vector3 treePosition = treeRenderer.transform.position;
                Vector3 cameraPosition = Camera.main.transform.position;
                Vector3 directionToTree = (treePosition - cameraPosition).normalized;

                // Perform a raycast from the camera to the tree mesh renderer
                RaycastHit hit;
                if (Physics.Raycast(cameraPosition, directionToTree, out hit, Mathf.Infinity, treeLayer))
                {
                    if (hit.collider.GetComponent<Renderer>() == treeRenderer)
                    {
                        // Tree is between camera and player, so fade it
                        treeRenderer.material.color = new Color(treeRenderer.material.color.r, treeRenderer.material.color.g, treeRenderer.material.color.b, 0.2f);
                    }
                }
            }
        }
    }

    void RestoreUnhitTrees(HashSet<Renderer> currentHitTrees)
    {
        List<Renderer> treesToRestore = new List<Renderer>();

        // Check all trees in the fadedTrees dictionary
        foreach (var treeRenderer in fadedTrees.Keys)
        {
            if (!currentHitTrees.Contains(treeRenderer))
            {
                // Immediately restore the original material
                treeRenderer.material = fadedTrees[treeRenderer];
                treesToRestore.Add(treeRenderer);
            }
        }

        // Remove restored trees from the fadedTrees dictionary
        foreach (var treeRenderer in treesToRestore)
        {
            fadedTrees.Remove(treeRenderer);
        }
    }
}
