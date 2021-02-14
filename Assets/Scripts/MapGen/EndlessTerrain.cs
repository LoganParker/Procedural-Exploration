using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndlessTerrain : MonoBehaviour
{
    //Viewer settings
    [SerializeField]public const float maxViewDist = 450;
    public Transform viewer;
    public static Vector2 viewerPosition;
    Dictionary<Vector2,TerrainChunk> terrainChunkDict = new Dictionary<Vector2, TerrainChunk>();
    List<TerrainChunk> terrainChunksVisibleToLastUpdate = new List<TerrainChunk>();
    // Map values
    static MapGen mapGenerator;
    int chunkSize;
    int chunksVisibleInDist;
    [SerializeField] private Material mapMaterial;


    

    void Start() {
        mapGenerator = FindObjectOfType<MapGen>();
        chunkSize = MapGen.mapChunkSize-1;
        chunksVisibleInDist = Mathf.RoundToInt(maxViewDist / chunkSize);
    }

    void Update(){
        viewerPosition = new Vector2(viewer.position.x, viewer.position.z);
        UpdateVisibleChunks();
    }
    
    void UpdateVisibleChunks(){
        
        for(int i = 0; i<terrainChunksVisibleToLastUpdate.Count; i++){
            terrainChunksVisibleToLastUpdate[i].SetVisible(false);
        }
        terrainChunksVisibleToLastUpdate.Clear();

        int currentChunkCoordX = Mathf.RoundToInt(viewerPosition.x / chunkSize);
        int currentChunkCoordY = Mathf.RoundToInt(viewerPosition.y / chunkSize);

        for(int yOffset = -chunksVisibleInDist; yOffset <= chunksVisibleInDist; yOffset++){
            for(int xOffset = -chunksVisibleInDist; xOffset <= chunksVisibleInDist; xOffset++){
                Vector2 viewedChunkCoord = new Vector2(currentChunkCoordX + xOffset,currentChunkCoordY+yOffset);

                if(terrainChunkDict.ContainsKey(viewedChunkCoord)){
                    terrainChunkDict[viewedChunkCoord].UpdateTerrainChunk();
                    if(terrainChunkDict[viewedChunkCoord].IsVisible()){
                        terrainChunksVisibleToLastUpdate.Add(terrainChunkDict[viewedChunkCoord]);
                    }
                } else {
                    terrainChunkDict.Add(viewedChunkCoord, new TerrainChunk(viewedChunkCoord,chunkSize,transform,mapMaterial));
                }
            }
        }
    }

    public class TerrainChunk{
        
        GameObject meshObject;
        Vector2 position;
        Bounds bounds;

        MapData mapData;

        MeshRenderer meshRenderer;
        MeshFilter meshFilter;
        
        public TerrainChunk(Vector2 coord, int size, Transform parent, Material material){
            position = coord * size;
            bounds = new Bounds(position,Vector2.one * size);
            Vector3 positionV3 = new Vector3(position.x,0,position.y);


            meshObject = new GameObject("Terrain Chunk");
            meshRenderer = meshObject.AddComponent<MeshRenderer>();
            meshRenderer.material = material;
            meshFilter = meshObject.AddComponent<MeshFilter>();


            meshObject.transform.position = positionV3;
            meshObject.transform.parent = parent;
            SetVisible(false);

            mapGenerator.RequestMapData(OnMapDataRecieved);
        }

        void OnMapDataRecieved(MapData mapData){
            mapGenerator.RequestMeshData(mapData,OnMeshDataRecieved);
        }

        void OnMeshDataRecieved(MeshData meshData){
            meshFilter.mesh = meshData.CreateMesh();
        }

        public void UpdateTerrainChunk(){
            float viewerDistFromNearestEdge = Mathf.Sqrt(bounds.SqrDistance(viewerPosition));
            bool visible = viewerDistFromNearestEdge <= maxViewDist;
            SetVisible(visible);
        }

        public void SetVisible(bool visible){
            meshObject.SetActive(visible);
        }

        public bool IsVisible(){
            return meshObject.activeSelf;
        }
    }
}
