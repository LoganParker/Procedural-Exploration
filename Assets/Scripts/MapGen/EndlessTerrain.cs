using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndlessTerrain : MonoBehaviour
{
    //Viewer settings
    const float viewerMoveThreshholdForChunkUpdate = 25f;
    const float sqrViewerMoveThreshholdForChunkUpdate = viewerMoveThreshholdForChunkUpdate * viewerMoveThreshholdForChunkUpdate;
    [SerializeField]private static float maxViewDist;
    [SerializeField]private LODInfo[] detailLevels;
    public Transform viewer;
    public static Vector2 viewerPosition;
    public Vector2 viewerPositionOld;
    Dictionary<Vector2,TerrainChunk> terrainChunkDict = new Dictionary<Vector2, TerrainChunk>();
    static List<TerrainChunk> terrainChunksVisibleLastUpdate = new List<TerrainChunk>();
    // Map values
    const float mapScale = 5;
    static MapGen mapGenerator;
    int chunkSize;
    int chunksVisibleInDist;
    [SerializeField] private Material mapMaterial;


    

    void Start() {
        mapGenerator = FindObjectOfType<MapGen>();
        
        maxViewDist = detailLevels[detailLevels.Length-1].visibleDistanceThreshold;
        chunkSize = MapGen.mapChunkSize-1;
        chunksVisibleInDist = Mathf.RoundToInt(maxViewDist / chunkSize);

        UpdateVisibleChunks();
    }

    void Update(){
        viewerPosition = new Vector2(viewer.position.x, viewer.position.z) / mapScale;
        if((viewerPositionOld-viewerPosition).sqrMagnitude > sqrViewerMoveThreshholdForChunkUpdate){
            viewerPositionOld = viewerPosition;
            UpdateVisibleChunks();
        }
        
        
    }
    
    void UpdateVisibleChunks(){
        
        for(int i = 0; i<terrainChunksVisibleLastUpdate.Count; i++){
            terrainChunksVisibleLastUpdate[i].SetVisible(false);
        }
        terrainChunksVisibleLastUpdate.Clear();

        int currentChunkCoordX = Mathf.RoundToInt(viewerPosition.x / chunkSize);
        int currentChunkCoordY = Mathf.RoundToInt(viewerPosition.y / chunkSize);

        for(int yOffset = -chunksVisibleInDist; yOffset <= chunksVisibleInDist; yOffset++){
            for(int xOffset = -chunksVisibleInDist; xOffset <= chunksVisibleInDist; xOffset++){
                Vector2 viewedChunkCoord = new Vector2(currentChunkCoordX + xOffset,currentChunkCoordY+yOffset);

                if(terrainChunkDict.ContainsKey(viewedChunkCoord)){
                    terrainChunkDict[viewedChunkCoord].UpdateTerrainChunk();
                } else {
                    terrainChunkDict.Add(viewedChunkCoord, new TerrainChunk(viewedChunkCoord,chunkSize,detailLevels,transform,mapMaterial));
                }
            }
        }
    }

    public class TerrainChunk{
        
        GameObject meshObject;
        Vector2 position;
        Bounds bounds;
        MapData mapData;
        bool mapDataRecieved;

        // LOD & Rendering
        MeshRenderer meshRenderer;
        MeshFilter meshFilter;
        LODInfo[] detailLevels;
        LODMesh[] lODMeshes;
        int prevLODIndex = -1;

        public TerrainChunk(Vector2 coord, int size, LODInfo[] detailLevels,Transform parent, Material material){
            this.detailLevels = detailLevels;
            position = coord * size;
            bounds = new Bounds(position,Vector2.one * size);
            Vector3 positionV3 = new Vector3(position.x,0,position.y);


            meshObject = new GameObject("Terrain Chunk");
            meshRenderer = meshObject.AddComponent<MeshRenderer>();
            meshRenderer.material = material;
            meshFilter = meshObject.AddComponent<MeshFilter>();


            meshObject.transform.position = positionV3*mapScale;
            meshObject.transform.parent = parent;
            meshObject.transform.localScale = Vector3.one * mapScale;
            SetVisible(false);

            lODMeshes=new LODMesh[detailLevels.Length];
            for(int i =0; i < detailLevels.Length; i++ ){
                lODMeshes[i] = new LODMesh(detailLevels[i].lod, UpdateTerrainChunk);
            }

            mapGenerator.RequestMapData(position,OnMapDataRecieved);
        }

        void OnMapDataRecieved(MapData mapData){
            this.mapData = mapData;
            mapDataRecieved = true;
            
            //Draw texture to generated mesh
            Texture2D texture = TextureGen.TextureFromColorMap(mapData.colorMap,MapGen.mapChunkSize,MapGen.mapChunkSize);
            meshRenderer.material.mainTexture = texture;

            UpdateTerrainChunk();
        }

        void OnMeshDataRecieved(MeshData meshData){
            meshFilter.mesh = meshData.CreateMesh();
        }

        public void UpdateTerrainChunk(){
            if(mapDataRecieved){
                
                float viewerDistFromNearestEdge = Mathf.Sqrt(bounds.SqrDistance(viewerPosition));
                bool visible = viewerDistFromNearestEdge <= maxViewDist;
                
                SetVisible(visible);

                if(visible){
                    
                    int lodIndex = 0;
                    
                    for(int i =0;i<detailLevels.Length-1;i++){
                        if(viewerDistFromNearestEdge>detailLevels[i].visibleDistanceThreshold){
                            lodIndex= i+1;
                        }
                        else{
                            break;
                        }
                    }
                    
                    if(lodIndex != prevLODIndex){
                        LODMesh lodMesh = lODMeshes[lodIndex];
                        if(lodMesh.hasMesh){
                            prevLODIndex = lodIndex;
                            meshFilter.mesh = lodMesh.mesh;
                        }
                        else if(!lodMesh.hasRequestedMesh){
                            lodMesh.RequestMesh(mapData);
                        }
                    }

                    terrainChunksVisibleLastUpdate.Add(this);
                }
            }
        }

        public void SetVisible(bool visible){
            meshObject.SetActive(visible);
        }

        public bool IsVisible(){
            return meshObject.activeSelf;
        }
    }

    // Fetch mesh from map gen
    class LODMesh{
        public Mesh mesh;
        public bool hasRequestedMesh;
        public bool hasMesh;
        int lod;

        System.Action updateCallback;



        public LODMesh(int lod,System.Action updateCallback)
        {
            this.lod=lod;
            this.updateCallback = updateCallback;
        }
        void OnMeshDataRecieved(MeshData meshData){
            mesh = meshData.CreateMesh();
            hasMesh = true;
            updateCallback();
        }

        public void RequestMesh(MapData mapData){
            hasRequestedMesh = true;
            mapGenerator.RequestMeshData(mapData,lod,OnMeshDataRecieved);
        }
    }

    [System.Serializable]
    public struct LODInfo{
        public int lod;
        public float visibleDistanceThreshold;
    }
}
