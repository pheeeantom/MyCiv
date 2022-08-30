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
            transform.position = transform.position + new Vector3(-0.01f, 0, 0);
        if (Input.GetKey(KeyCode.D) && transform.position.x < maxX)
            transform.position = transform.position + new Vector3(0.01f, 0, 0);
        if (Input.GetKey(KeyCode.W) && transform.position.y < maxY)
            transform.position = transform.position + new Vector3(0, 0.01f, 0);
        if (Input.GetKey(KeyCode.S) && transform.position.y > 0)
            transform.position = transform.position + new Vector3(0, -0.01f, 0);
        if (Input.GetButtonDown("Fire1"))
        {
            Vector3 mousePos = Input.mousePosition;
            float screenToCameraDistance = Camera.nearClipPlane;
            Vector3 mousePosNearClipPlane = new Vector3(mousePos.x, mousePos.y, screenToCameraDistance);
            Vector3 worldPointPos = Camera.ScreenToWorldPoint(mousePosNearClipPlane);
            Debug.Log(worldPointPos);
            Vector3Int gridPos = Grid.LocalToCell(worldPointPos);
            Debug.Log(gridPos);
            /*foreach (Unit unit in Unit.units)
            {
                Grid.GetCellCenterWorld((Vector3Int)unit.pos);
                Debug.Log()
            }*/

        }
        /*if (Input.GetButtonDown("Fire1"))
        {
            Vector3 mousePos = Input.mousePosition;
            foreach (Unit unit in Unit.units)
            {
                Vector3Int position = Vector3Int.FloorToInt(HandleUtility.GUIPointToWorldRay(Event.current.mousePosition).origin);
                Vector3Int cellPos = Grid.WorldToCell(position);

                Debug.Log(unit.pos + ", " + cellPos);
                if (Vector3.Distance(mousePos, new Vector3(unit.pos.x, unit.pos.y, 0)) < 1)
                {
                    Debug.Log(100);
                    Hex hex = Generator._world.GetHex(unit.pos);
                    int cost = unit.movement;
                    List<Hex> movementField = (List<Hex>)Lib.GetUniques<Hex>(unit.GetMovementField(cost, hex));
                    Generator._world.HighlightHexes(movementField);
                }
            }
        }*/
            /*Event e = Event.current;
            if (e != null)
            {
                if (Event.current.type == EventType.MouseDown)
                {
                    Vector3Int position = Vector3Int.FloorToInt(HandleUtility.GUIPointToWorldRay(Event.current.mousePosition).origin);
                    Vector3Int cellPos = Grid.WorldToCell(position);

                    Debug.Log(cellPos);
                }
            }*/
    }
}
