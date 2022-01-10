using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BalsaEarthCore
{
    public struct MapPos
    {
        public int xPos;
        public int zPos;

        public MapPos(int xPos, int zPos)
        {
            this.xPos = xPos;
            this.zPos = zPos;
        }

        public override bool Equals(object testObject)
        {
            if (!(testObject is MapPos))
            {
                return false;
            }
            MapPos testMapPos = (MapPos)testObject;
            if (testMapPos.xPos != xPos)
            {
                return false;
            }
            if (testMapPos.zPos != zPos)
            {
                return false;
            }
            return true;
        }

        public override int GetHashCode()
        {
            //Lower 2 bytes are probably enough for this test, better to mix the bytes in fully though
            return (xPos << 0x16) | (zPos & 0xFF);
        }
    }
}