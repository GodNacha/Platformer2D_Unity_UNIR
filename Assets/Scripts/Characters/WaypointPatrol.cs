using UnityEngine;
using System.Collections;

public class WaypointPatrol : MonoBehaviour
{
    [Header("Waypoints")]
    public Transform[] waypoints;

    [Header("Movement")]
    public float reachDistance = 0.2f;

    [Header("Wait Time")]
    public float minWaitTime = 1f;
    public float maxWaitTime = 2.5f;

    private int currentWaypoint;

    private bool waiting;
    public bool IsWaiting => waiting;

    public Coroutine waitCoroutine;

    public Transform CurrentTarget => waypoints[currentWaypoint];

    void Start()
    {
        if (waypoints.Length == 0)
        {
            Debug.LogWarning(gameObject.name + " has no waypoints assigned.");
            return;
        }

        SelectRandomWaypoint();
    }

    public void UpdatePatrol()
    {
        if (waypoints.Length == 0) return;

        float dist = Vector3.Distance(transform.position, CurrentTarget.position); //Distancia entre el enemigo y el waypoint actual

        if (dist <= reachDistance && waitCoroutine == null)
        {
            waitCoroutine = StartCoroutine(WaitAtWaypoint()); //Se llama a la corrutina para esperar en el waypoint actual antes de seleccionar el siguiente
        }
    }

    IEnumerator WaitAtWaypoint()
    {
        waiting = true;

        float waitTime = Random.Range(minWaitTime, maxWaitTime); // Tiempo random entre min y max

        yield return new WaitForSeconds(waitTime);

        SelectRandomWaypoint();

        waiting = false;

        waitCoroutine = null;
    }

    void SelectRandomWaypoint()
    {
        if (waypoints.Length <= 1)
            return;

        int next; //Variable "siguiente waypoint"

        do
        {
            next = Random.Range(0, waypoints.Length);
        }
        while (next == currentWaypoint); //Se repite el bucle si el nuevo waitpoint es el mismo que el anterior

        currentWaypoint = next;
    }

    public void StopWaiting() //Esta corrutina la llaman los enemigos al detectar al jugador, para así cancelar la espera en el waypoint y comenzar a moverse hacia el jugador
    {
        if (waitCoroutine != null)
        {
            StopCoroutine(waitCoroutine);

            waitCoroutine = null;
        }

        waiting = false;
    }
}