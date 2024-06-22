using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldUtilityManager : MonoBehaviour
{
    public static WorldUtilityManager instance;

    [Header("Layers")]
    [SerializeField] LayerMask characterLayers;
    [SerializeField] LayerMask enviornmentalLayers;
    [SerializeField] LayerMask wallsLayerMask; // For the see Through

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public LayerMask GetCharacterLayers()
    {
        return characterLayers;
    }

    public LayerMask GetEnviornemntalLayers()
    {
        return enviornmentalLayers;
    }

    public LayerMask GetWallsLayerMask() 
    {
        return wallsLayerMask;
    }

    public bool IsThisTargetMyEnemy(Faction attacking, Faction targeting)
    {
        //if (attacking == targeting) return false;

        switch (attacking)
        {
            case Faction.Faction01:
                {
                    switch (targeting)
                    {
                        case Faction.Faction01: return false;
                        case Faction.Faction02: return true;
                        default: return false;
                    }
                }
            case Faction.Faction02:
                {
                    switch (targeting)
                    {
                        case Faction.Faction01: return true;
                        case Faction.Faction02: return false;
                        default: return false;
                    }
                }
        }

        return false;
    }
}
