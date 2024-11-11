using System.Collections;
using System.Collections.Generic;
using CookApps.Playgrounds.Scene;
using UnityEngine;

namespace CookApps.BM.TTT.Scene
{
    [SceneDefine("Addrs/Scenes/Pinata.unity", SceneResourceType.Addressable)]
    public class Pinata : SceneBase
    {
        private readonly Vector3 _defaultGravity = new(0, -9.81f, 0);

        protected override void OnAwake()
        {
            base.OnAwake();
            if (!SceneManager.IsExist)
            {
                SceneManager.Initialize(this);
            }
        }

        public override void OnOpen()
        {
            Physics.gravity = _defaultGravity * 2;
        }

        public override void OnClose()
        {
            Physics.gravity = _defaultGravity;
        }
    }
}
