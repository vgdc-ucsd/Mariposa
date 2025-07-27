using UnityEngine;

public class WaveAnimation : MonoBehaviour
{
    [Header("Frequency")]
    [SerializeField] private float baseFrequency = 1.0f;
    [SerializeField] private float frequencyVariance = 0.0f;
    private float frequency;

    [Header("Phase")]
    [SerializeField] private float basePhase = 0.0f;
    [SerializeField] private float phaseVariance = 0.0f;
    private float phase;

    [Header("Max Offset")]
    [SerializeField] private Vector2 baseMaxOffset = Vector2.one;
    [SerializeField] private Vector2 maxOffsetVariance = Vector2.zero;
    private Vector2 maxOffset;

    private Vector3 origin;

    private void Awake()
    {
        origin = transform.position;
        frequency = baseFrequency + Random.Range(-frequencyVariance, frequencyVariance);
        phase = basePhase + Random.Range(-phaseVariance, phaseVariance);
        maxOffset.x = baseMaxOffset.x + Random.Range(-maxOffsetVariance.x, maxOffsetVariance.x);
        maxOffset.y = baseMaxOffset.y + Random.Range(-maxOffsetVariance.y, maxOffsetVariance.y);
    }

    // Update is called once per frame
    void Update()
    {
        float radians = Mathf.PI * (frequency * Time.time + phase);
        float deltaX = Mathf.Sin(radians) * maxOffset.x;
        float deltaY = Mathf.Sin(radians) * Mathf.Sin(radians) * maxOffset.y;
        transform.position = origin + new Vector3(deltaX, deltaY);
    }
}
