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

    public enum EnemyName
    {
        None = -1,
        Walker1,
        Walker2,
        Filier1,
        Filier2,
        FlyingFireEnemy,
        OctoHead,
        END,
    }

    public static int ENEMY_COUNT = (int)EnemyName.END;
    public enum EffectName
    {
        None,
        Damage,
        Effect,
        Flare,
    }
    public enum FatherSkill
    {
        FindingRange = 0,
        NormalAttackRange,
        PenerstrateRange,
        SpearAttackRange,
        END
    }
    public static int FatherSkillCount = (int)FatherSkill.END;

    public enum CardSelectionType
    {
        Status,
        Weapon
    }
    public enum CardSelection
    {
        None = - 1,
        �ݵ�����,
        ü��ȸ��,
        �ִ�ü������,
        AK47,
        Mk2,
        ����跰,
        ��������,
        ������,
        ����Ʈ�̱�,
        �����,
        ���Ǿ�ɷ�����,
        �йи�����ݷ�����,
        �溮�ִ�ü������,
        �溮ũ������,
        �溮�ӵ�����,
        �Ѿ˰��������,
        �Ѿ˵���������,
        ��ũ���̺�ɷ�����,    
        �������ð�����,
    }

    public enum WeaponName
    {
        None = -1,
        AK47,
        Mk2,
        DoubleBarreledShotgun,
        PolicePistal,
        DesertEagle,
        Revolver,
        Bat,
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
        public FigureType figureType;
    }

    public enum FigureType
    {
        Box,
        Circle,
        Raycast
    }
}