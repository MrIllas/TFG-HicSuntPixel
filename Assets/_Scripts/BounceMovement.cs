using UnityEngine;

public class BounceMovement : MonoBehaviour
{
    public float circularRadius = 0.0f;
    public float circularSpeed = 0.2f;
    private float circAngle = 0.0f;
    private const float TAU = Mathf.PI * 2;


    private float aux;

    // Start is called before the first frame update
    void Start()
    {
        aux = transform.localPosition.y;
    }

    // Update is called once per frame
    void Update()
    {
        TestMode();
    }

    private void TestMode()
    {
        if (circularRadius <= 0) return;
        circAngle -= TAU * circularSpeed * Time.deltaTime;


        

        Vector3 newPosition = new Vector3(transform.localPosition.x,
                                            aux + Mathf.Sin(circAngle) * circularRadius,
                                            transform.localPosition.z);
        transform.localPosition = newPosition;
    }
}
