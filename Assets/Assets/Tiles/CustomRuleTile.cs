using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;
using System.Linq;

[CreateAssetMenu]
public class CustomRuleTile : RuleTile<CustomRuleTile.Neighbor> {
    public bool alwaysConnect;
    public TileBase[] tilesToConnect;
    public bool checkSelf;

    public class Neighbor : RuleTile.TilingRule.Neighbor
    {
        public const int This = 1;
        public const int NotThis = 2;
        public const int Any = 3;
        public const int Specific = 4;
        public const int Nothing = 5;
    }

    public override bool RuleMatch(int neighbor, TileBase tile) {
        switch (neighbor) {
            case Neighbor.This: return CheckThis(tile);
            case Neighbor.NotThis: return CheckNotThis(tile);
            case Neighbor.Any: return CheckAny(tile);
            case Neighbor.Specific: return CheckSpecific(tile);
            case Neighbor.Nothing: return CheckNothing(tile);
        }
        return base.RuleMatch(neighbor, tile);
    }

    private bool CheckThis(TileBase tile)
    {
        if (alwaysConnect) return tile == this;
        return tilesToConnect.Contains(tile) || tile == this;
    }
    private bool CheckNotThis(TileBase tile)
    {
        return tile != this;
    }
    private bool CheckAny(TileBase tile)
    {
        if (checkSelf) return tile != null;
        return tile != null && tile != this;
    }
    private bool CheckSpecific(TileBase tile)
    {
        return tilesToConnect.Contains(tile);
    }
    private bool CheckNothing(TileBase tile)
    {
        return tile == null;
    }
    
}