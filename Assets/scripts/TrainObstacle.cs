using UnityEngine;
using System.Collections;

/// <summary>
/// Controls a train obstacle that moves across the track at intervals.
/// </summary>
public class TrainObstacle : MonoBehaviour
{
    public Transform startPoint;
    public Transform endPoint;
    public float speed = 25f;
    public float interval = 45f;
    
    private bool isMoving = false;

    void Start()
    {
        StartCoroutine(TrainRoutine());
    }

    private IEnumerator TrainRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(interval);
            yield return StartCoroutine(MoveTrain());
        }
    }

    private IEnumerator MoveTrain()
    {
        isMoving = true;
        transform.position = startPoint.position;
        Debug.Log("[Chaos] Train approaching crossing!");

        // Trigger train horn if we have a clip
        if (CinematicAudioManager.Instance != null)
            CinematicAudioManager.Instance.PlayCinematicSound("TrainHorn", 1.0f);

        while (Vector3.Distance(transform.position, endPoint.position) > 0.1f)
        {
            transform.position = Vector3.MoveTowards(transform.position, endPoint.position, speed * Time.deltaTime);
            yield return null;
        }

        isMoving = false;
        Debug.Log("[Chaos] Train passed.");
    }
}
