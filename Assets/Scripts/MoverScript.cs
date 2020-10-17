using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoverScript : MonoBehaviour
{
    public float rotateSpeed = 0.25f;
    public float moveSpeed = 0.5f;

    List<Vector3> movePoints;
    float waypointThreshold;
    int pointIndex;

    GameManager gameManager;

    // Start is called before the first frame update
    void Start()
    {
        gameManager = FindObjectOfType<GameManager>(); 
    }

    // Update is called once per frame
    void Update()
    {
        CheckWaypoint();
        MoveToWaypoint();
    }

    void CheckWaypoint()
    {
        if (pointIndex > movePoints.Count)
        {
            pointIndex = 0;
        }

        if ((movePoints[pointIndex] - transform.position).magnitude < waypointThreshold)
        {
            gameManager.WaypointReached(pointIndex);

            pointIndex++;
            if (pointIndex >= movePoints.Count)
            {
                pointIndex = 0;
            }
        }
    }

    void MoveToWaypoint()
    {
        // Look towards waypoint
        Vector3 dir = movePoints[pointIndex] - transform.position;
        Quaternion rot = Quaternion.LookRotation(dir);
        // slerp to the desired rotation over time
        transform.rotation = Quaternion.Slerp(transform.rotation, rot, rotateSpeed * Time.deltaTime);

        // Move towards waypoint
        transform.position = Vector3.MoveTowards(transform.position, movePoints[pointIndex], moveSpeed * Time.deltaTime);
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.tag == "Mine")
        {
            gameManager.PlayerCollision(other.gameObject);
        }
    }

    public void SetWaypoints(List<Vector3> waypoints, float threshold)
    {
        waypointThreshold = threshold;
        movePoints = waypoints;
    }
}
