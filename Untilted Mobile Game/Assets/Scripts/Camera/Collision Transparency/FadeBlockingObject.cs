using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadeBlockingObject : MonoBehaviour
{

    [Header("FADE SETTINGS")]
    [SerializeField] private LayerMask whatIsFadeable;

    [SerializeField] private bool shadowsEnabled;

    [SerializeField] private float fadeSpeed = 4f;
    private const float fadedAlpha = 0.3f;

    private Transform followTarget;

    private List<FadeObject> blockingObjects = new List<FadeObject>();
    private Dictionary<FadeObject, Coroutine> runningCoroutines = new Dictionary<FadeObject, Coroutine>();
    RaycastHit[] hits = new RaycastHit[7];

    private void Start()
    {
        GetReferences_Start();
        GetBlockingObjects_Start();
    }

    #region START

    private void GetReferences_Start()
    {
        followTarget = GetComponent<CameraFollow>().followTarget;
    }

    private void GetBlockingObjects_Start()
    {
        StartCoroutine(CR_GetBlockingObjects());
    }

    #endregion START

    private IEnumerator CR_GetBlockingObjects()
    {
        while (true)
        {
            FadeObjectsHit();

            FadeObjectsUnHit();

            ClearHits();

            yield return null;
        }
    }

    private void FadeObjectsHit()
    {
        Vector3 dir = followTarget.position - transform.position;
        dir.Normalize();

        float distance = Vector3.Distance(followTarget.position, transform.position);

        int hitsCount = Physics.RaycastNonAlloc(transform.position, dir, hits, distance, whatIsFadeable);
        if (hitsCount > 0)
        {
            for (int i = 0; i < hitsCount; i++)
            {
                FadeObject fadingObject = GetFadeObject(hits[i]);

                if (fadingObject != null && !blockingObjects.Contains(fadingObject))
                {
                    HandleOldTransitions(fadingObject);

                    runningCoroutines.Add(fadingObject, StartCoroutine(CR_FadeObjectOut(fadingObject)));
                    blockingObjects.Add(fadingObject);
                }
            }
        }
    }

    #region FADING TRANSITIONS CRs

    private IEnumerator CR_FadeObjectOut(FadeObject fadingObject)
    {
        foreach (Material material in fadingObject.Materials)
        {
            material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
            material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusDstAlpha);
            material.SetInt("_ZWrite", 0);
            material.SetInt("_Surface", 1);

            material.renderQueue = (int)UnityEngine.Rendering.RenderQueue.Transparent;

            material.SetShaderPassEnabled("DepthOnly", false);
            material.SetShaderPassEnabled("SHADOWCASTER", shadowsEnabled);

            material.SetOverrideTag("RenderType", "Transparent");

            material.EnableKeyword("_SURFACE_TYPE_TRANSPARENT");
            material.EnableKeyword("_ALPHAPREMULTIPLY_ON");
        }

        float time = 0;
        while (fadingObject.Materials[0].color.a > fadedAlpha)
        {
            foreach (Material material in fadingObject.Materials)
            {
                if (material.HasProperty("_Color"))
                {
                    material.color = new Color(
                        material.color.r,
                        material.color.g,
                        material.color.b,
                        Mathf.Lerp(fadingObject.initialAlpha, fadedAlpha, time * fadeSpeed)
                        );
                }
            }
            time += Time.deltaTime;
            yield return null;
        }

        if (runningCoroutines.ContainsKey(fadingObject))
        {
            StopCoroutine(runningCoroutines[fadingObject]);
            runningCoroutines.Remove(fadingObject);
        }
    }

    private IEnumerator CR_FadeObjectIn(FadeObject fadingObject)
    {
        float time = 0;
        while (fadingObject.Materials[0].color.a < fadingObject.initialAlpha)
        {
            foreach (Material material in fadingObject.Materials)
            {
                if (material.HasProperty("_Color"))
                {
                    material.color = new Color(
                        material.color.r,
                        material.color.g,
                        material.color.b,
                        Mathf.Lerp(fadedAlpha, fadingObject.initialAlpha, time * fadeSpeed)
                        );
                }
            }
            time += Time.deltaTime;
            yield return null;
        }

        foreach (Material material in fadingObject.Materials)
        {
            material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
            material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.Zero);
            material.SetInt("_ZWrite", 1);
            material.SetInt("_Surface", 0);

            material.renderQueue = (int)UnityEngine.Rendering.RenderQueue.Geometry;

            material.SetShaderPassEnabled("DepthOnly", true);
            material.SetShaderPassEnabled("SHADOWCASTER", true);

            material.SetOverrideTag("RenderType", "Opaque");

            material.DisableKeyword("_SURFACE_TYPE_TRANSPARENT");
            material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
        }

        if (runningCoroutines.ContainsKey(fadingObject))
        {
            StopCoroutine(runningCoroutines[fadingObject]);
            runningCoroutines.Remove(fadingObject);
        }
    }

    #endregion FADING TRANSITIONS CRs

    #region TRANSITION HANDLERS

    private void FadeObjectsUnHit()
    {
        List<FadeObject> objectsUnHit = new List<FadeObject>(blockingObjects);

        foreach (FadeObject fadingObject in blockingObjects)
        {
            bool stillBlocking = false;

            for (int i = 0; i < hits.Length; i++)
            {
                FadeObject hitFadingObject = GetFadeObject(hits[i]);

                if (hitFadingObject != null && fadingObject == hitFadingObject)
                {
                    stillBlocking = true;
                    break;
                }
            }

            if (!stillBlocking)
            {
                HandleOldTransitions(fadingObject);

                runningCoroutines.Add(fadingObject, StartCoroutine(CR_FadeObjectIn(fadingObject)));
                objectsUnHit.Add(fadingObject);
            }

        }

        foreach (FadeObject hitObject in objectsUnHit)
        {
            blockingObjects.Remove(hitObject);
        }
    }

    private void HandleOldTransitions(FadeObject fadingObject)
    {
        if (runningCoroutines.ContainsKey(fadingObject))
        {
            if (runningCoroutines[fadingObject] != null)
            {
                StopCoroutine(runningCoroutines[fadingObject]);
            }

            runningCoroutines.Remove(fadingObject);
        }
    }

    private void ClearHits()
    {
        System.Array.Clear(hits, 0, hits.Length);
    }

    #endregion TRANSITION HANDLERS

    private FadeObject GetFadeObject(RaycastHit hit)
    {
        return hit.collider != null ? hit.collider.GetComponent<FadeObject>() : null;
    }
}