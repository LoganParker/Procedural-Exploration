using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/*
Script to generate a noise map which is responsible for assigning 
texture according to the height of vertex

*/
public class ChunkGenerator : MonoBehaviour
{
    // Noise Variables
    [SerializeField] NoiseMap noiseMapGen;
    [SerializeField] private float mapScale;
    [SerializeField] private Wave[] waves;
    //

    // Mesh control
    [SerializeField] private MeshRenderer tileRenderer;
    [SerializeField] private MeshFilter meshFilter;
    [SerializeField] private MeshCollider meshCollider;
    [SerializeField] private float heightMult;
    [SerializeField] AnimationCurve heightCurve;
    //
    // Texture
    [SerializeField] private TerrainType[] terrains;
    //
    private void Start() {
        GenerateChunk();
    }
    void GenerateChunk(){
        
        //Calculate height/width based on vertices
        Vector3[] meshVertices = meshFilter.mesh.vertices;
        int tileHeight = (int)Mathf.Sqrt(meshVertices.Length);
        int tileWidth = tileHeight;

        // Calculate elevation offsets based on tile location
        float offsetX = -this.gameObject.transform.position.x;
        float offsetZ = -this.gameObject.transform.position.z;

        // Generate the height map
        float[,] heightMap = noiseMapGen.GenerateNoiseMap(tileHeight,tileWidth,mapScale,offsetX,offsetZ,waves,"perlin");
        
        // Build the terrain texture
        Texture2D tileTexture = BuildTexture(heightMap);
        tileRenderer.material.mainTexture = tileTexture;
        //Update Mesh
        UpdateMeshVertices(heightMap);
    }
    // Texture Control:
    private Texture2D BuildTexture(float[,] heightMap){
        int tileHeight = heightMap.GetLength(0);
        int tileWidth = heightMap.GetLength(1);

        Color[] colorMap = new Color[tileHeight*tileWidth];
        //for every coordinate in our height map,
        for(int z = 0; z < tileHeight; z++){
            for(int x = 0; x < tileWidth; x++){
                // Set color as a terrain based on the height
                int colorIndex = (z * tileWidth)+ x;
                float height = heightMap[z,x];
                TerrainType terrainType = GetTerrainType(height);
                colorMap[colorIndex] = terrainType.color;
            }
        }
        //Generate a new texture to apply to terrain
        Texture2D tileTexture = new Texture2D(tileWidth,tileHeight);
        tileTexture.wrapMode = TextureWrapMode.Clamp;
        tileTexture.SetPixels(colorMap);
        tileTexture.Apply();

        return tileTexture;
    }
    private TerrainType GetTerrainType(float height){
        foreach(TerrainType terrainType in terrains){
            if(height < terrainType.altitude){
                return terrainType;
            }
        }
        return terrains[terrains.Length -1];
    }
    //

    // Vertice Control
    private void UpdateMeshVertices(float[,] heightMap){
        
        int tileHeight = heightMap.GetLength(0);
        int tileWidth = heightMap.GetLength(1);

        Vector3[] meshVertices = meshFilter.mesh.vertices;

        //Iterate through all vertices and change height according to heightmap
        int vertexIndex = 0;
        for(int z = 0; z<tileHeight;z++){
            for(int x = 0; x<tileWidth; x++){
                float vertexHeight = heightMap[z,x];

                Vector3 vertex = meshVertices[vertexIndex];
                //Adjust vertex's Y to new height 
                meshVertices[vertexIndex] = new Vector3(vertex.x, heightCurve.Evaluate(vertexHeight)*heightMult, vertex.z);
                
                vertexIndex++;
            }
        }
        //Update the vertices in the mesh
        meshFilter.mesh.vertices = meshVertices;
        meshFilter.mesh.RecalculateBounds();
        meshFilter.mesh.RecalculateNormals();
        //Update collider
        meshCollider.sharedMesh = meshFilter.mesh;
    }
    //
}

[System.Serializable] public class TerrainType{
    public string name;
    public float altitude;
    public Color color;

}