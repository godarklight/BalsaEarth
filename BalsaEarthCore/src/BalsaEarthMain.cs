using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BalsaCore.Sim.FloatingOriginBehaviours;

namespace BalsaEarthCore
{
    public class BalsaEarthMain : MonoBehaviour
    {
        Heightmap heightmap;
        MapGenerator generator;
        MapLoader mapLoader;

        void Start()
        {
            heightmap = new Heightmap();
            heightmap.Load();
            generator = new MapGenerator(heightmap);
            mapLoader = new MapLoader(gameObject.transform, generator);
#if !GAME
            GameObject menuView = GameObject.Find("menuview0");
            GameObject camera = new GameObject();
            camera.name = "Main Camera";
            camera.transform.localPosition = menuView.transform.localPosition;
            camera.transform.localRotation = menuView.transform.localRotation;
            camera.AddComponent<Camera>();
#endif
        }

        void Update()
        {
            Vector3d playerPos = Vector3d.zero;
            Vector3d planePos = Vector3d.zero;
#if GAME
        if (GameLogic.CurrentScene == GameScenes.FLIGHT && GameLogic.LocalPlayer != null && GameLogic.LocalPlayer.Avatar != null && GameLogic.LocalPlayerVehicle != null && GameLogic.LocalPlayerVehicle.Physics != null)
        {
            playerPos = FloatingOrigin.GetAbsoluteWPos(GameLogic.LocalPlayer.Avatar.transform.position);
            planePos = FloatingOrigin.GetAbsoluteWPos(GameLogic.LocalPlayerVehicle.Physics.transform.position);
        }
#endif
            mapLoader.UpdateMap(playerPos, planePos);
        }
    }
}