using System.Collections;
using UnityEngine;

public class VehicleMovement : MonoBehaviour
{
    public Transform[] waypoints; // Array of waypoints
    public float moveSpeed = 5f; // Speed of the vehicle
    private int currentWaypointIndex = 0; // Current waypoint index

    void Update()
    {
        MoveAlongRoad();
    }

    void MoveAlongRoad()
    {
        if (waypoints.Length == 0) return; // Exit if no waypoints

        // Move the vehicle towards the current waypoint
        Transform targetWaypoint = waypoints[currentWaypointIndex];
        transform.position = Vector3.MoveTowards(transform.position, targetWaypoint.position, moveSpeed * Time.deltaTime);
        transform.LookAt(targetWaypoint); // Rotate towards the target

        // Check if the vehicle has reached the current waypoint
        if (Vector3.Distance(transform.position, targetWaypoint.position) < 0.1f)
        {
            currentWaypointIndex = (currentWaypointIndex + 1) % waypoints.Length; // Loop back to the first waypoint
        }
    }
}

