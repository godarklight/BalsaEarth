using System;
using System.IO;
using System.IO.Compression;
using System.Reflection;
using UnityEngine;

namespace BalsaEarthCore
{
    public class Heightmap
    {
        private Stream dataStream;
        private int mapWidth;
        private int mapHeight;
        private float originX;
        private float originZ;
        public float resolution;
        private int xCenter = 7832;
        private int zCenter = 5401;

        public void Load()
        {
            if (!File.Exists("data.bin") || !File.Exists("meta.txt"))
            {
                Stream inStream = Assembly.GetExecutingAssembly().GetManifestResourceStream("BalsaEarthCore.data.zip");
                ZipArchive za = new ZipArchive(inStream, ZipArchiveMode.Read);
                foreach (ZipArchiveEntry zae in za.Entries)
                {
                    zae.ExtractToFile(zae.Name);
                }
            }
            string[] metaText = File.ReadAllLines("meta.txt");
            string[] sizeSplit = metaText[0].Split('x');
            mapWidth = int.Parse(sizeSplit[0]);
            mapHeight = int.Parse(sizeSplit[1]);
            string[] originSplit = metaText[1].Split(',');
            originX = float.Parse(sizeSplit[0]);
            originZ = float.Parse(sizeSplit[1]);
            resolution = float.Parse(metaText[2]);
            dataStream = new FileStream("data.bin", FileMode.Open);
        }

        public float[] Perlin(int xOffset, int zOffset, int pixelPerSide)
        {
            float[] retVal = new float[(pixelPerSide + 1) * (pixelPerSide + 1)];
            int index = 0;
            for (int z = 0; z <= pixelPerSide; z++)
            {
                for (int x = 0; x <= pixelPerSide; x++)
                {
                    retVal[index] = 50f * Mathf.PerlinNoise((x + xOffset) / (float)pixelPerSide, (z + zOffset) / (float)pixelPerSide);
                    index++;
                }
            }
            return retVal;
        }

        public float[] GetHeights(int xOffset, int zOffset, int pixelPerSide)
        {
            byte[] u4 = new byte[4];
            float[] retVal = new float[(pixelPerSide + 1) * (pixelPerSide + 1)];
            //Offset 0,0 to something interesting
            int xShift = xCenter + (xOffset / (int)resolution);
            int zShift = zCenter + (zOffset / (int)resolution);
            //Make sure we don't go out of bounds
            if (xShift < 0 || zShift < 0 || (xShift + pixelPerSide + 1) > mapWidth || (zShift + pixelPerSide + 1) > mapHeight)
            {
                return retVal;
            }
            int index = 0;
            for (int z = 0; z <= pixelPerSide; z++)
            {
                int readPixel = ((zShift + z) * mapWidth) + xShift;

                dataStream.Seek(4 * readPixel, SeekOrigin.Begin);
                for (int x = 0; x <= pixelPerSide; x++)
                {
                    if (z == 0 && x == 0)
                    {
                        Debug.Log($"First Read {dataStream.Position}");
                    }
                    if (z == 0 && x == pixelPerSide)
                    {
                        Debug.Log($"Last Read {dataStream.Position}");
                    }
                    dataStream.Read(u4, 0, 4);
                    float data = BitConverter.ToSingle(u4, 0);
                    if (data > 5000 || data < 1)
                    {
                        data = -100;
                    }
                    retVal[index] = data;
                    index++;
                }
            }
            return retVal;
        }
    }
}