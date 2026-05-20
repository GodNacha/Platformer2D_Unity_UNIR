using UnityEngine;
using UnityEngine.UI;

public class ProgressBar : MonoBehaviour
{
    [Header ("References")]
    public Transform player;
    public Transform startPoint;
    public Transform endPoint;

    public Slider progressBar;

    private Vector3 pathDirection;
    private float pathLength;

    void Start()
    {
        pathDirection = (endPoint.position - startPoint.position).normalized;
        pathLength = Vector3.Distance(startPoint.position, endPoint.position);
    }

    void Update()
    {
        Vector3 playerOffset = player.position - startPoint.position;

        float distanceAlongPath = Vector3.Dot(playerOffset, pathDirection);

        float progress = Mathf.Clamp01(distanceAlongPath / pathLength);

        progressBar.value = progress;
    }
}