using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    [SerializeField] private int mapWidthInTiles,mapDepthInTiles;
    [SerializeField] private GameObject tilePrefab;

    void Start()
    {
        GenerateMap();
    }

    void GenerateMap()
    {
        // Tile dimensions from prefab
        Vector3 tileSize = tilePrefab.GetComponent<MeshRenderer>().bounds.size;
        int tileWidth = (int)tileSize.x;
        int tileHeight = (int)tileSize.z;

        // for each tile requested, generate the tile
        for(int tileX = 0; tileX < mapWidthInTiles; tileX++){
            for(int tileZ = 0; tileZ < mapDepthInTiles; tileZ++){
                    //Get tile position based on x and z index
                    Vector3 tilePos = new Vector3(
                        this.gameObject.transform.position.x + tileX * tileWidth,
                        this.gameObject.transform.position.y,
                        this.gameObject.transform.position.z + tileZ * tileHeight);
                    GameObject tile = Instantiate(tilePrefab, tilePos, Quaternion.identity) as GameObject;
            }
        }


    }
}
