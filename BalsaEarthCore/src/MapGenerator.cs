using System;
using System.IO;
using System.IO.Compression;
using System.Reflection;
using UnityEngine;

namespace BalsaEarthCore
{
    public class MapGenerator
    {
        Heightmap heightmap;
        int pixelsPerSide;
        public MapGenerator(Heightmap heightmap)
        {
            this.heightmap = heightmap;
            pixelsPerSide = MapLoader.TILE_SIZE / (int)heightmap.resolution;
        }

        public GameObject GenerateTile(int xOffset, int zOffset, Transform mainTransform)
        {
            GameObject retVal = new GameObject();
            retVal.name = $"Tile ({xOffset}, {zOffset})";
            retVal.layer = (int)BalsaLayers.scenery;
            Mesh mesh = new Mesh();
            mesh.name = $"Mesh ({xOffset}, {zOffset})";
            retVal.transform.parent = mainTransform;
            retVal.transform.localPosition = new Vector3(xOffset, 0, zOffset);
            Vector3[] verts = new Vector3[(pixelsPerSide + 1) * (pixelsPerSide + 1)];
            Vector2[] uvs = new Vector2[verts.Length];
            float[] heightData = heightmap.GetHeights(xOffset, zOffset, pixelsPerSide);
            int[] tris = new int[pixelsPerSide * pixelsPerSide * 6];
            for (int z = 0; z <= pixelsPerSide; z++)
            {
                float zscale = (z - (pixelsPerSide / 2)) * heightmap.resolution;
                float zuv = (z / (float)pixelsPerSide);
                for (int x = 0; x <= pixelsPerSide; x++)
                {
                    float xscale = (x - (pixelsPerSide / 2)) * heightmap.resolution;
                    float xuv = (x / (float)pixelsPerSide);
                    int vertexIndex = z * (pixelsPerSide + 1) + x;
                    verts[vertexIndex] = new Vector3(xscale, heightData[vertexIndex], zscale);
                    uvs[vertexIndex] = new Vector2(xuv, zuv);
                    if (x < pixelsPerSide && z < pixelsPerSide)
                    {
                        int triIndex = 6 * (pixelsPerSide * z + x);
                        //BL
                        tris[triIndex] = vertexIndex;
                        //TL
                        tris[triIndex + 1] = vertexIndex + pixelsPerSide + 1;
                        //BR
                        tris[triIndex + 2] = vertexIndex + 1;
                        //BR
                        tris[triIndex + 3] = vertexIndex + 1;
                        //TL
                        tris[triIndex + 4] = vertexIndex + pixelsPerSide + 1;
                        //TR
                        tris[triIndex + 5] = vertexIndex + pixelsPerSide + 2;
                    }
                }
            }
            mesh.SetVertices(verts);
            mesh.SetTriangles(tris, 0);
            mesh.SetUVs(0, uvs);
            mesh.Optimize();
            mesh.RecalculateNormals();
            MeshFilter mf = retVal.AddComponent<MeshFilter>();
            mf.mesh = mesh;
            MeshRenderer mr = retVal.AddComponent<MeshRenderer>();
            mr.material = GenerateMaterial(xOffset, zOffset);
            MeshCollider mc = retVal.AddComponent<MeshCollider>();
            return retVal;
        }

        //Texture function
        private Material GenerateMaterial(int xOffset, int zOffset)
        {
            Material retVal = new Material(Shader.Find("Standard"));
            retVal.name = $"Material ({xOffset}, {zOffset})";
            retVal.color = new Color(0f, 0.5f, 0f, 1f);
            return retVal;
        }
    }
}