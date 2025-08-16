using System.Collections;
using UnityEngine;

public class TrainManager : MonoBehaviour
{
    [Header("START AND END POINTS")]
    [SerializeField] private Transform startPoint;
    [SerializeField] private Transform endPoint;

    Vector3 debugCubeSize = new Vector3(1f, 3f, 2.5f);

    [Header("TIMING SETTINGS")]
    [SerializeField] private float startDelay;
    [SerializeField] private float timeBeforeRespawn; 

    private const float speed = 50f;
    private GameObject trainGo;

    [Header("VFX & AUDIO SETTINGS")]
    [SerializeField] private GameObject[] vfxPfs;
    [SerializeField] private AudioSource audioSource;


    private void Start()
    {
        Start_GetReferences();
        Start_ResetTrain();
    }

    #region START

    private void Start_GetReferences()
    {
        trainGo = transform.GetChild(0).gameObject;
        audioSource = GetComponentInChildren<AudioSource>();
        audioSource.outputAudioMixerGroup = AudioManager.Instance.sfxMixerGroup;
    }

    private void Start_ResetTrain()
    {
        for (int i = 0; i < trainGo.transform.childCount; i++)
        {
            trainGo.transform.GetChild(i).gameObject.SetActive(false);
            trainGo.transform.GetChild(i).gameObject.GetComponent<TrainWagon>().speed = speed;
            trainGo.transform.position = startPoint.position;
        }

        Invoke(nameof(SpawnTrain), startDelay);
    }

    #endregion START

    public void ResetWagon(GameObject wagon)
    {
        wagon.SetActive(false);
        wagon.transform.position = startPoint.position;
    }

    public void SpawnTrain()
    {
        StartCoroutine(CRSpawnTrain());
    }

    private IEnumerator CRSpawnTrain()
    {
        yield return new WaitForSeconds(timeBeforeRespawn);

        foreach (GameObject vfx in vfxPfs)
        {
            vfx.SetActive(false);
        }

        foreach (GameObject vfx in vfxPfs)
        {
            vfx.SetActive(true);
        }

        audioSource.clip = AudioManager.Instance.GetClip("ANT TREN");
        audioSource.Play();

        yield return new WaitForSeconds(3f);

        audioSource.clip = AudioManager.Instance.GetClip("TREN");
        audioSource.Play();

        for (int i = 0; i < trainGo.transform.childCount; i++)
        {
            trainGo.transform.GetChild(i).gameObject.SetActive(true);
            yield return new WaitForSeconds(0.098f);
        }

        yield break;
    }

    #region VISUAL DEBUG

    private void OnDrawGizmos()
    {
        if (startPoint != null && endPoint != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawCube(startPoint.position + new Vector3(0f, 1.5f, 0f), debugCubeSize);
            Gizmos.color = Color.red;
            Gizmos.DrawCube(endPoint.position + new Vector3(0f, 1.5f, 0f), debugCubeSize);
        }
    }

    #endregion VISUAL DEBUG
}