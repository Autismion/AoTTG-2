﻿using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Characters.Titan
{
    public class TitanDetection : MonoBehaviour
    {
        public MindlessTitan Titan;

        void Start()
        {
            if (!Titan.photonView.isMine) return;
            InvokeRepeating("CheckPlayers", 1f, 0.5f);
        }

        private List<Collider> colliders = new List<Collider>();

        private void OnTriggerEnter(Collider other)
        {
            if (!colliders.Contains(other)) { colliders.Add(other); }
        }

        private void OnTriggerExit(Collider other)
        {
            colliders.Remove(other);
        }

        protected void CheckPlayers()
        {
            if (Titan.HasTarget()) return;
            foreach (var collider in colliders)
            {
                if (collider == null) continue;
                var target = collider.transform.root.gameObject;
                if (target.GetComponent<Hero>() == null) continue;
                Vector3 targetDir = target.transform.position - transform.position;
                float angle = Vector3.Angle(targetDir, transform.forward);
                if (angle > 0 && angle < 100)
                {
                    Titan.OnTargetDetected(target);
                    break;
                }
            }
        }
    }
}
