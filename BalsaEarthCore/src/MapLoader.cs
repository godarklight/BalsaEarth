using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BalsaCore;

namespace BalsaEarthCore
{
    public class MapLoader
    {
        MapGenerator generator;
        List<GameObject> gos;
        GameObject ocean;
        HashSet<MapPos> loadedTiles;
        HashSet<MapPos> keepTiles;
        Transform mainTransfrom;
        const int MAX_TILES_PER_FRAME = 1;
        const int DISTANCE = 10;
        public const int TILE_SIZE = 500;
        bool firstLoad = true;

        public MapLoader(Transform mainTransform, MapGenerator generator)
        {
            this.generator = generator;
            gos = new List<GameObject>();
            loadedTiles = new HashSet<MapPos>();
            keepTiles = new HashSet<MapPos>();
            this.mainTransfrom = mainTransform;
            GenerateOcean(mainTransform);
        }

        private void GenerateOcean(Transform mainTransform)
        {
            Mesh mesh = new Mesh();
            mesh.name = "Ocean Mesh";
            Vector3[] verts = new Vector3[] { new Vector3(-10000f, 0, -10000f), new Vector3(10000f, 0, -10000f), new Vector3(-10000f, 0, 10000f), new Vector3(10000f, 0, 10000f) };
            int[] tris = new int[] { 0, 2, 1, 1, 2, 3 };
            mesh.SetVertices(verts);
            mesh.SetTriangles(tris, 0);
            mesh.RecalculateNormals();
            Material oceanMat = new Material(Shader.Find("Standard"));
            oceanMat.name = "Ocean Material";
            oceanMat.color = Color.blue;
            ocean = new GameObject();
            ocean.name = "Ocean";
            ocean.transform.parent = mainTransform;
            MeshFilter mf = ocean.AddComponent<MeshFilter>();
            mf.mesh = mesh;
            MeshRenderer mr = ocean.AddComponent<MeshRenderer>();
            mr.material = oceanMat;
        }

        public void UpdateMap(Vector3d playerPosition, Vector3d planePosition)
        {
            keepTiles.Clear();
            //Load map around player
            for (int z = -DISTANCE; z < DISTANCE; z++)
            {
                for (int x = -DISTANCE; x < DISTANCE; x++)
                {
                    int ourX = ((int)playerPosition.x / TILE_SIZE) * TILE_SIZE;
                    int ourZ = ((int)playerPosition.z / TILE_SIZE) * TILE_SIZE;
                    MapPos mp = new MapPos(ourX + x * TILE_SIZE, ourZ + z * TILE_SIZE);
                    keepTiles.Add(mp);
                }
            }

            //Load map around plane
            for (int z = -DISTANCE; z < DISTANCE; z++)
            {
                for (int x = -DISTANCE; x < DISTANCE; x++)
                {
                    int ourX = ((int)planePosition.x / TILE_SIZE) * TILE_SIZE;
                    int ourZ = ((int)planePosition.z / TILE_SIZE) * TILE_SIZE;
                    MapPos mp = new MapPos(ourX + x * TILE_SIZE, ourZ + z * TILE_SIZE);
                    if (!keepTiles.Contains(mp))
                    {
                        keepTiles.Add(mp);
                    }
                }
            }

            //Remove the rest
            foreach (GameObject go in gos)
            {
                int ourX = (int)go.transform.localPosition.x;
                int ourZ = (int)go.transform.localPosition.z;
                MapPos mp = new MapPos(ourX, ourZ);
                if (!keepTiles.Contains(mp))
                {
                    loadedTiles.Remove(mp);
                    List<GameObject> newGos = new List<GameObject>(gos);
                    newGos.Remove(go);
                    gos = newGos;
                    go.DestroyGameObject();
                }
            }

            //Load missing tiles
            int genTiles = 0;
            foreach (MapPos mp in keepTiles)
            {
                if (!loadedTiles.Contains(mp))
                {
                    loadedTiles.Add(mp);
                    GameObject newTile = generator.GenerateTile(mp.xPos, mp.zPos, mainTransfrom);
                    List<GameObject> newGos = new List<GameObject>(gos);
                    newGos.Add(newTile);
                    gos = newGos;
                    genTiles++;
                    if (MAX_TILES_PER_FRAME > 0 && genTiles > MAX_TILES_PER_FRAME && !firstLoad)
                    {
                        return;
                    }
                }
            }
            firstLoad = false;
        }
    }
}