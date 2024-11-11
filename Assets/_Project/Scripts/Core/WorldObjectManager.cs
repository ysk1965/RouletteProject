using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using CookApps.BM.TTT.Data;
using CookApps.BM.TTT.InGame.Object;
using UnityEngine;

public class WorldObjectManager
{
    private int _mapId;
    private Func<List<UpgradableObjectBase>> _upgradableObjectFactory;
    private Dictionary<UpgradeObjectType, UpgradableObjectBase> _upgradableObjectList = new();

    public WorldObjectManager(int mapId, Func<List<UpgradableObjectBase>> upgradableObjectFactory)
    {
        _mapId = mapId;
        _upgradableObjectFactory = upgradableObjectFactory;
    }

    public void Initialize()
    {
        Dictionary<UpgradeObjectType, List<SpecUpgradeObject>> specList = SpecDataManager.Instance
            .GetUpgradeObjectList(_mapId)
            .GroupBy(s => s.object_id)
            .ToDictionary(d => d.Key, d => d.ToList());

        List<UpgradableObjectBase> list = _upgradableObjectFactory?.Invoke();
        if (list != null)
        {
            foreach (UpgradableObjectBase upgradableObjectBase in list)
            {
                UpgradeObjectType type = upgradableObjectBase.UpgradeObjectType;
                _upgradableObjectList.Add(type, upgradableObjectBase);

                if (specList.TryGetValue(type, out List<SpecUpgradeObject> spec))
                {
                    upgradableObjectBase.Initialize(spec);
                }
            }
        }
    }

    public void SetFocusObject(UpgradeObjectType type)
    {
        foreach (KeyValuePair<UpgradeObjectType, UpgradableObjectBase> pair in _upgradableObjectList)
        {
            pair.Value.SetActiveVirtualCamera(pair.Key == type);
        }
    }
}
