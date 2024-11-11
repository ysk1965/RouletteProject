using System;
using System.Collections;
using System.Collections.Generic;
using CookApps.BM.TTT.InGame.Object;
using UnityEngine;
using Random = UnityEngine.Random;

namespace CookApps.BM.TTT.Pinata
{
    public class PinataObject : WorldObjectBase
    {
        [SerializeField]
        private BoxCollider _boxCollider;

        [SerializeField]
        private Rigidbody _rigidbody;

        [SerializeField]
        private float _power = 1f;

        private void AddForce(Vector3 force)
        {
            if (!_rigidbody)
                return;

            _rigidbody.AddForce(force * _power, ForceMode.Impulse);
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                AddForce(Vector3.down);
            }
            else if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                AddForce(Vector3.up);
            }
            else if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                AddForce(Vector3.left);
            }
            else if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                AddForce(Vector3.right);
            }
            else if (Input.GetKeyDown(KeyCode.Space))
            {
                Vector3 force = Random.insideUnitCircle;
                AddForce(force);
            }
        }
    }
}
