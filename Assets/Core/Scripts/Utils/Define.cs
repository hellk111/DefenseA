using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using UnityEngine;

public static class Define 
{
    public enum CoreManagers
    {
        None = -1,
        Data,
        Input,
        Resource,
        END,
    }

    public static int CORE_MANAGER_COUNT = (int)CoreManagers.END;

    public enum ContentManagers
    {
        None = -1,
        Game,
        UI,
        Effect,
        Text,
        END,
    }
    public static int CONTENT_MANAGER_COUNT = (int)ContentManagers.END;


    public enum CharacterType
    {
        Player,
        Enemy,
        Building
    }
    public enum EffectName
    {
        None,
        Damage,
    }

    public enum CardSelection
    {
        None = - 1,
        WeaponDamageUp,
        AmmoUp,
        ReloadSpeedUp,
        RecoverHp,
        UnlockSpear,
    }

    public enum WeaponName
    {
        None = -1,
        Shotgun,
        Handgun,
        Minigun,
        Revolver1,
        Revolver2,
        END
    }
    public enum Passive
    {
        None = 1,
        ź��Һ񹫽�,
        ����������,
    }

    [System.Serializable]
    public struct Range
    {
        public Vector3 center;
        public Vector3 size;
    }
}
