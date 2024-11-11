using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using CookApps.Obfuscator;
using Newtonsoft.Json;
using UnityEngine;

namespace CookApps.BM.TTT.Data
{
    public interface IUserAssetChangeReceiver
    {
        void OnAssetChanged(ItemType itemType, int before, int after);
    }

    public record UserAsset
    {
        public ItemType ItemType { get; set; }
        public ObfuscatorInt Amount { get; set; }
    }

    public interface IUserAssetData : IUserDataBase
    {
        UserAsset GetAsset(ItemType itemType);
        UserAsset AddAsset(ItemType itemType, int amount);
        UserAsset SetAsset(ItemType itemType, int amount);

        void Subscribe(ItemType assetType, IUserAssetChangeReceiver receiver, bool forceFire = true);
        void Unsubscribe(IUserAssetChangeReceiver receiver);
    }

    [Serializable]
    [JsonObject(MemberSerialization.OptIn)]
    public class UserAssetData : UserDataBase, IUserAssetData
    {
        [JsonProperty("s")]
        private List<UserAsset> _userAssets = new();

        private List<IUserAssetChangeReceiver> _receiverList = new();

        [OnDeserialized]
        internal void OnDeserializedMethod(StreamingContext context)
        {
        }

        public UserAssetData()
        {
            const int defaultCoin = 1000;
            CreateNewAsset(ItemType.coin, defaultCoin);
        }

        private UserAsset CreateNewAsset(ItemType itemType, int amount = 0)
        {
            var asset = new UserAsset
            {
                ItemType = itemType,
                Amount = amount,
            };
            _userAssets.Add(asset);
            return asset;
        }

        private UserAsset GetAssetInternal(ItemType itemType)
        {
            UserAsset asset = _userAssets.Find(ua => ua.ItemType == itemType) ?? CreateNewAsset(itemType);
            return asset;
        }

        private void FireEvent(ItemType assetType, int before, int after)
        {
            foreach (IUserAssetChangeReceiver receiver in _receiverList)
            {
                receiver.OnAssetChanged(assetType, before, after);
            }
        }

        public override void ClearAllSubscribes()
        {
            _receiverList.Clear();
        }

        #region IUserAssetData

        public UserAsset GetAsset(ItemType itemType)
        {
            return GetAssetInternal(itemType);
        }

        public UserAsset AddAsset(ItemType itemType, int amount)
        {
            UserAsset asset = GetAssetInternal(itemType);
            switch (amount)
            {
                case 0:
                    return asset;
                case < 0 when asset.Amount < Mathf.Abs(amount):
                    return asset;
            }

            int before = asset.Amount;
            asset.Amount += amount;
            FireEvent(itemType, before, asset.Amount);
            return asset;
        }

        public UserAsset SetAsset(ItemType itemType, int amount)
        {
            UserAsset asset = GetAssetInternal(itemType);
            int before = asset.Amount;
            asset.Amount = amount;
            FireEvent(itemType, before, asset.Amount);
            return asset;
        }

        public void Subscribe(ItemType assetType, IUserAssetChangeReceiver receiver, bool forceFire = true)
        {
            if (!_receiverList.Contains(receiver))
            {
                _receiverList.Add(receiver);
            }

            if (!forceFire)
            {
                return;
            }

            UserAsset asset = GetAssetInternal(assetType);
            receiver.OnAssetChanged(assetType, -1, asset.Amount);
        }

        public void Unsubscribe(IUserAssetChangeReceiver receiver)
        {
            _receiverList.Remove(receiver);
        }

        #endregion
    }
}
