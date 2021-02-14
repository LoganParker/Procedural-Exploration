using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class MeshGen{
    public static MeshData GenerateTerrainMesh(float[,] heightMap, float heightMultiplier,  AnimationCurve _heightCurve, int levelOfDetail){
        AnimationCurve heightCurve = new AnimationCurve(_heightCurve.keys);
        int width = heightMap.GetLength(0);
        int height = heightMap.GetLength(1);
        //For keeping mesh centered
        float topLeftX = (width-1)/-2f;
        float topLeftZ = (height-1)/2f;

        int meshSimplificationIncrement = (levelOfDetail==0)? 1:levelOfDetail*2;
        int verticesPerLine = (width-1)/ meshSimplificationIncrement+1;

        MeshData meshData = new MeshData(verticesPerLine,verticesPerLine);
        int vertexIndex = 0;


        for(int y = 0; y < height; y += meshSimplificationIncrement){
            for(int x = 0; x < width; x += meshSimplificationIncrement){
                
                meshData.vertices[vertexIndex] = new Vector3(topLeftX + x,heightCurve.Evaluate(heightMap[x,y])*heightMultiplier, topLeftZ - y);
                meshData.uvs[vertexIndex] = new Vector2(x/(float)width, y/(float)height);

                if(x < width-1 && y < height-1 ){
                    meshData.AddTrinagle(vertexIndex,vertexIndex+ verticesPerLine+1,vertexIndex+verticesPerLine);
                    meshData.AddTrinagle(vertexIndex+verticesPerLine+1,vertexIndex,vertexIndex+1);
                }

                vertexIndex++;
            }
        }

        return meshData;

    }
}
public class MeshData{
    public Vector3[] vertices;
    public int[] triangles;
    public Vector2[] uvs;

    int traingleIndex;
    public MeshData(int meshWidth, int meshHeight){
        vertices = new Vector3[meshWidth*meshHeight];
        uvs = new Vector2[meshWidth*meshHeight];
        triangles = new int[(meshWidth-1)*(meshHeight-1)*6];
    }

    public void AddTrinagle(int a, int b, int c){
        triangles[traingleIndex] = a;
        triangles[traingleIndex+1] = b;
        triangles[traingleIndex+2] = c;
        traingleIndex+=3;
    }

    public Mesh CreateMesh(){
        Mesh mesh = new Mesh();
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.uv = uvs;
        mesh.RecalculateNormals();
        return mesh;
    }
}