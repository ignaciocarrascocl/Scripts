using UnityEngine;

public class NoteController : MonoBehaviour
{
    private float noteSpeed;
    private bool isHit = false;

    [Header("Feedback Spawn Offset")]
    public Vector3 feedbackOffset = Vector3.zero;

    public void Initialize(float speed)
    {
        noteSpeed = speed;
    }

    void Update()
    {
        transform.position += Vector3.back * noteSpeed * Time.deltaTime;
    }

    void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Stopper") && !isHit)
        {
            ScoreManager.instance.RegisterMiss(transform.position + feedbackOffset);
            DestroyNote();
        }
    }

    public void OnHit()
    {
        if(!isHit)
        {
            isHit = true;
            float accuracy = Mathf.Abs(transform.position.z - ScoreManager.instance.playerTransform.position.z);
            string hitType;

            if (accuracy < ScoreManager.instance.greatRange)
            {
                hitType = "great";
            }
            else if (accuracy < ScoreManager.instance.goodRange)
            {
                hitType = "good";
            }
            else if (accuracy < ScoreManager.instance.mehRange)
            {
                hitType = "meh";
            }
            else
            {
                hitType = "meh"; // Treat anything beyond mehRange as meh
            }

            ScoreManager.instance.RegisterHit(hitType, transform.position + feedbackOffset, accuracy);
            DestroyNote();
        }
    }

    void DestroyNote()
    {
        Destroy(gameObject);
    }
}
