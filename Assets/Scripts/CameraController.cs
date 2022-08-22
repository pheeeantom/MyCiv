using UnityEngine;

public class CameraController : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.A) && transform.position.x > 0)
            transform.position = transform.position + new Vector3(-0.01f, 0, 0);
        if (Input.GetKey(KeyCode.D) && transform.position.x < 8)
            transform.position = transform.position + new Vector3(0.01f, 0, 0);
        if (Input.GetKey(KeyCode.W) && transform.position.y < 0)
            transform.position = transform.position + new Vector3(0, 0.01f, 0);
        if (Input.GetKey(KeyCode.S) && transform.position.y > -7)
            transform.position = transform.position + new Vector3(0, -0.01f, 0);
    }
}
