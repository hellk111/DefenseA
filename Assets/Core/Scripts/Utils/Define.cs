using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Define 
{
    public enum CoreManagers
    {
        None = -1,
        Data,
        Input,
        END,
    }

    public static int CORE_MANAGER_COUNT = (int)CoreManagers.END;

    public enum ContentManagers
    {
        None = -1,
        Game,
        END,
    }
    public static int CONTENT_MANAGER_COUNT = (int)ContentManagers.END;


    public enum CharacterType
    {
        Player,
        Enemy,
    }

    [System.Serializable]
    public struct Range
    {
        public Vector3 center;
        public Vector3 size;
    }
}
