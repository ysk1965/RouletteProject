using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CookApps.BM.TTT.Data
{
    public interface IUserDataBase
    {
        void Initialize();
        void ClearAllSubscribes();
    }

    public abstract class UserDataBase : IUserDataBase
    {
        public virtual void Initialize()
        {
        }

        public virtual void ClearAllSubscribes()
        {
        }
    }
}
