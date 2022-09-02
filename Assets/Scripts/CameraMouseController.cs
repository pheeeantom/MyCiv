using World;
using System.Collections.Generic;
using Units;
using UnityEngine;
public class CameraMouseController : MonoBehaviour
{
    [SerializeField] HexWorldGenerator _generator;
    [SerializeField] Grid _grid;
    [SerializeField] Camera _camera;
    float _maxX;
    float _maxY;
    Unit _selectedUnit;
    
    // Start is called before the first frame update
    void Start()
    {
        _maxX = 0.43f * _generator.Width - 0.215f;
        _maxY = 0.375f * _generator.Height - 0.186f;
        //Debug.Log(maxX);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.A) && transform.position.x > 0)
            transform.position += new Vector3(-0.01f, 0, 0);
        if (Input.GetKey(KeyCode.D) && transform.position.x < _maxX)
            transform.position += new Vector3(0.01f, 0, 0);
        if (Input.GetKey(KeyCode.W) && transform.position.y < _maxY)
            transform.position += new Vector3(0, 0.01f, 0);
        if (Input.GetKey(KeyCode.S) && transform.position.y > 0)
            transform.position += new Vector3(0, -0.01f, 0);
        if (Input.GetButtonDown("Fire1"))
        {
            Vector3 mousePos = new Vector3(Input.mousePosition.x, Input.mousePosition.y, -_camera.transform.position.z);
            Vector3 worldPos = _camera.ScreenToWorldPoint(mousePos);
            Vector2Int gridPos = (Vector2Int)_grid.WorldToCell(worldPos);
            Hex hex = _generator._world.GetHex(gridPos);
            
            if (hex is null)
                return;

            Unit unit = hex.Unit;
            if (!(unit is null))
            {
                _selectedUnit = unit;
                List<Hex> reachableHexes = _selectedUnit.GetReachableHexes();
                _generator._world.HighlightHexes(reachableHexes);
            }
            else if (!(_selectedUnit is null))
            {
                List<Hex> path = _selectedUnit.FindShortestPath(hex);
                _generator._world.HighlightHexes(path);
            }
        }
    }
}
