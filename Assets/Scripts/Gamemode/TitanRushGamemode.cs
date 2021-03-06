﻿using Assets.Scripts.Characters.Titan.Behavior;
using Assets.Scripts.Gamemode.Settings;
using Assets.Scripts.Settings;
using Assets.Scripts.UI.Elements;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Gamemode
{
    //This is the colossal gamemode, where titans "rush" towards a specified endpoint
    public class TitanRushGamemode : GamemodeBase
    {
        public sealed override GamemodeSettings Settings { get; set; }
        private RushSettings GamemodeSettings => Settings as RushSettings;

        private GameObject[] Routes { get; set; }
        private GameObject[] Spawns { get; set; }

        [UiElement("Titan frequency", "1 titan will spawn per Interval", SettingCategory.Advanced)]
        public int TitanInterval { get; set; } = 7;

        public override void OnLevelLoaded(Level level, bool isMasterClient = false)
        {
            base.OnLevelLoaded(level, isMasterClient);
            GameObject.Find("playerRespawnTrost").SetActive(false);
            Object.Destroy(GameObject.Find("playerRespawnTrost"));
            Object.Destroy(GameObject.Find("rock"));
            if (!isMasterClient) return;
            //if (IsAllPlayersDead()) return;
            PhotonNetwork.Instantiate("COLOSSAL_TITAN", (Vector3)(-Vector3.up * 10000f), Quaternion.Euler(0f, 180f, 0f), 0);
            Routes = GameObject.FindGameObjectsWithTag("route");
            GameObject[] objArray = GameObject.FindGameObjectsWithTag("titanRespawn");
            var spawns = new List<GameObject>();
            foreach (GameObject obj2 in objArray)
            {
                if (obj2.transform.parent.name == "titanRespawnCT")
                {
                    spawns.Add(obj2);
                }
            }

            Spawns = spawns.ToArray();
        }

        public override string GetGamemodeStatusTop(int time = 0, int totalRoomTime = 0)
        {
            var content = "Time : ";
            var length = time - totalRoomTime;
            return content + length.ToString() + "\nDefeat the Colossal Titan.\nPrevent abnormal titan from running to the north gate";
        }

        private ArrayList GetRoute()
        {
            GameObject route = Routes[UnityEngine.Random.Range(0, Routes.Length)];
            while (route.name != "routeCT")
            {
                route = Routes[UnityEngine.Random.Range(0, Routes.Length)];
            }

            var checkPoints = new ArrayList();
            for (int i = 1; i <= 10; i++)
            {
                checkPoints.Add(route.transform.Find("r" + i).position);
            }
            checkPoints.Add("end");

            return checkPoints;
        }

        private int nextUpdate = 1;
        public void Update()
        {
            if (Time.time < nextUpdate) return;
            nextUpdate = Mathf.FloorToInt(Time.time) + 1;

            if (nextUpdate % TitanInterval != 0) return;
            SpawnTitan();
        }

        private void SpawnTitan()
        {
            if (FengGameManagerMKII.instance.getTitans().Count >= Settings.TitanLimit) return;
            var configuration = GetTitanConfiguration();
            var route = GetRoute();
            configuration.Behaviors.Add(new RushBehavior(route));
            var spawn = Spawns[Random.Range(0, Spawns.Length)];
            FengGameManagerMKII.instance.SpawnTitan(spawn.transform.position, spawn.transform.rotation, configuration);
        }

    }
}
