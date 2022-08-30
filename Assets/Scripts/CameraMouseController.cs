using Assets.Scripts;
using Assets.Scripts.Map;
using Assets.Scripts.Units;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class CameraMouseController : MonoBehaviour
{
    [SerializeField] HexWorldGenerator Generator;
    [SerializeField] Grid Grid;
    [SerializeField] Camera Camera;
    float maxX;
    float maxY;
    // Start is called before the first frame update
    void Start()
    {
        maxX = 0.43f * Generator.Width - 0.215f;
        maxY = 0.375f * Generator.Height - 0.186f;
        //Debug.Log(maxX);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.A) && transform.position.x > 0)
            transform.position += new Vector3(-0.01f, 0, 0);
        if (Input.GetKey(KeyCode.D) && transform.position.x < maxX)
            transform.position += new Vector3(0.01f, 0, 0);
        if (Input.GetKey(KeyCode.W) && transform.position.y < maxY)
            transform.position += new Vector3(0, 0.01f, 0);
        if (Input.GetKey(KeyCode.S) && transform.position.y > 0)
            transform.position += new Vector3(0, -0.01f, 0);
        if (Input.GetButtonDown("Fire1"))
        {
            Vector3 mousePos = new Vector3(Input.mousePosition.x, Input.mousePosition.y, -Camera.transform.position.z);
            Vector3 worldPos = Camera.ScreenToWorldPoint(mousePos);
            Vector3 gridPos = Grid.WorldToCell(worldPos);
            Debug.Log("Grid: " + gridPos);

        }
    }
}
