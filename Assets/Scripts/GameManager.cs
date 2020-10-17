using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [Header("Mover Settings")]
    public GameObject moverPrefab;
    public List<Vector3> waypoints = new List<Vector3>();
    public bool followCamera;
    public float wayPointThreshold = 0.5f;

    [Header("Mine Settings")]
    [Range(5f, 25f)]
    public float mineSpawnRange = 10f;
    public int mineSpawnCount = 100;
    public GameObject minePrefab;
    public Transform unusedMineObject;

    [Header("Effects Settings")]
    public GameObject collisionEffectPrefab;
    public Transform unusedEffects;


    [Header("Sound Settings")]
    public AudioSource soundSource;
    public AudioClip beepSound;
    public AudioClip explosionSound;

    List<Transform> mines = new List<Transform>();

    GameObject mover;

    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < mineSpawnCount; i++)
        {
            GameObject mine = Instantiate(minePrefab, unusedMineObject);
            mines.Add(mine.transform);
            mine.SetActive(false);
        }
        for (int i = 0; i < mineSpawnCount; i++)
        {
            GameObject effect = Instantiate(collisionEffectPrefab, unusedEffects);
            effect.SetActive(false);
        }
        ReloadMines();

        mover = Instantiate(moverPrefab, transform);
        mover.GetComponent<MoverScript>().SetWaypoints(waypoints, wayPointThreshold);
    }

    public IgnitionScript BeginEffect(Vector3 position, Vector3 lookAt)
    {
        IgnitionScript effectObject = unusedEffects.GetComponentInChildren<IgnitionScript>(true);
        effectObject.transform.SetParent(transform);
        effectObject.transform.position = new Vector3(position.x, position.y, position.z);
        effectObject.transform.up = new Vector3(lookAt.x, lookAt.y, lookAt.z);
        effectObject.gameObject.SetActive(true);
        effectObject.GetComponent<IgnitionScript>().FadeIn();
        return effectObject;
    }

    public void ReturnEffectToPool(GameObject effectToStore)
    {
        effectToStore.transform.SetParent(unusedEffects);

        effectToStore.gameObject.SetActive(false);
        effectToStore.transform.localScale = Vector3.one;
        effectToStore.transform.position = Vector3.zero;
    }


    void OnDrawGizmosSelected()
    {
        // Draw a yellow sphere at the transform's position
        Gizmos.color = Color.green;
        foreach(Vector3 waypoint in waypoints)
        {
            Gizmos.DrawSphere(waypoint, 0.5f);
        }
    }

    void CheckCamera()
    {
        Transform cameraTransform = Camera.main.transform;
        if (followCamera && cameraTransform.parent != mover.transform)
        {
            cameraTransform.SetParent(mover.transform);
            cameraTransform.localPosition = new Vector3(0f , 0.5f, -2.5f);
            cameraTransform.localEulerAngles = Vector3.zero;
        } else if(!followCamera && cameraTransform.parent != transform)
        {
            cameraTransform.SetParent(transform);
            cameraTransform.position = Vector3.zero;
        } else if(!followCamera)
        {
            cameraTransform.LookAt(mover.transform);
        }
    }

    // Update is called once per frame
    void Update()
    {
        CheckCamera();
        TakeInput();
    }

    void TakeInput()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            ReloadMines();
        }
    }

    void ReloadMines()
    {
        // Spawn Extra mines
        if(mineSpawnCount > mines.Count)
        {
            for(int i = 0; i < mineSpawnCount - mines.Count; i++)
            {
                GameObject mine = Instantiate(minePrefab, unusedMineObject);
                mines.Add(mine.transform);
                mine.SetActive(false);
            }

        }
        // Relocate
        foreach (Transform mine in mines)
        {
            mine.gameObject.SetActive(false);
            mine.position = new Vector3(Random.Range(-mineSpawnRange, mineSpawnRange), Random.Range(-mineSpawnRange, mineSpawnRange), Random.Range(-mineSpawnRange, mineSpawnRange));
            mine.gameObject.SetActive(true);
        }
    }

    public void PlayerCollision(GameObject collidedMine)
    {
        BeginEffect(collidedMine.transform.position, mover.transform.position);
        collidedMine.SetActive(false);
        
        soundSource.PlayOneShot(explosionSound);

    }

    public void WaypointReached()
    {
        soundSource.PlayOneShot(beepSound);
    }
}
