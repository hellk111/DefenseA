using MoreMountains.Feedbacks;
using PlasticGui.Configuration;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices.WindowsRuntime;
using Unity.VisualScripting;
using UnityEditor;
using UnityEditor.PackageManager.UI;
using UnityEditor.VersionControl;
using UnityEngine;

public class HelperEditor : EditorWindow
{
    const string DAUGHTER_CARD_DATA_PATH = "Data/��ī��.csv";
    const string FATHER_CARD_DATA_PATH = "Data/�ƺ�ī��.csv";
    const string DOG_CARD_DATA_PATH = "Data/������ī��.csv";
    const string CARD_FOLDER_DATA_PATH = "Resources/Datas/Card/";
    [MenuItem("CustomWindow/HelperWindow", false, 0)]
    static void Init()
    {
        // �����Ǿ��ִ� �����츦 �����´�. ������ ���� �����Ѵ�. �̱��� �����ε��ϴ�.
        HelperEditor window = (HelperEditor)EditorWindow.GetWindow(typeof(HelperEditor));
        window.Show();
    }
    void OnGUI()
    {
        if (GUILayout.Button(new GUIContent("ī�嵥���� ����")))
        {
            CreateDaughterCardData();
            CreateFatherCardData();
            CreateDogCardData();
        }
    }
    void CreateDaughterCardData()
    {
        TextAsset textAsset = UnityEditor.AssetDatabase.LoadAssetAtPath<TextAsset>("Assets/" + DAUGHTER_CARD_DATA_PATH);
        if (textAsset == null) return;

        string[] lines = textAsset.text.Split('\n');

        string[] preHeadWords = lines[0].Split(',');

        for (int i = 1; i < lines.Length; i++)
        {
            string[] words = lines[i].Split(',');
            if (preHeadWords.Length != words.Length) continue;

            DaughterCardData data = ScriptableObject.CreateInstance<DaughterCardData>();
            data.name = words[0];
            data.CardName = GetCardName(words[0]);
            data.CardDescription = words[2];
            data.IsStartCard = words[3].Equals("1") ? true : false;

            string[] priorCards = words[4].Split("|",options:System.StringSplitOptions.RemoveEmptyEntries);
            data.PriorCards = new List<Define.CardName>();
            foreach (var priorCard in priorCards)
            {
                data.PriorCards.Add(GetCardName(priorCard));
            }
            data.MaxUpgradeCount = words[5].Equals("") ? 0 : int.Parse(words[5]);
            data.IncreaseHp = words[6].Equals("") ? 0 : int.Parse(words[6]);
            data.IncreaseRecoverHpPower = words[7].Equals("") ? 0 : float.Parse(words[7]);
            data.IncreaseDamageReducePercentage = words[8].Equals("") ? 0 : float.Parse(words[8]);
            data.UnlockLastShot = words[9].Equals("1") ? true : false;
            data.UnlockFastReload = words[10].Equals("1") ? true : false;
            data.UnlockAutoReload = words[11].Equals("1") ? true : false;
            data.UnlockExtraAmmo = words[12].Equals("1") ? true : false;
            data.DecreaseFireDelayPercentage = words[13].Equals("") ? 0 : float.Parse(words[13]);
            data.IncreaseReloadSpeedPercentage = words[14].Equals("") ? 0 : float.Parse(words[14]);
            data.IncreaseReboundControlPowerPercentage = words[15].Equals("") ? 0 : float.Parse(words[15]);
            data.IncreasePenerstratingPower = words[16].Equals("") ? 0 : int.Parse(words[16]);
            data.IncreaseAttackPoint = ParseInt(words[17]);

            AssetDatabase.CreateAsset(data, "Assets/" + CARD_FOLDER_DATA_PATH + data.name + ".asset");
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
    }
    void CreateFatherCardData()
    {
        TextAsset textAsset = UnityEditor.AssetDatabase.LoadAssetAtPath<TextAsset>("Assets/" + FATHER_CARD_DATA_PATH);
        if (textAsset == null) return;

        string[] lines = textAsset.text.Split('\n');

        string[] preHeadWords = lines[0].Split(',');

        for (int i = 1; i < lines.Length; i++)
        {
            string[] words = lines[i].Split(',');
            if (preHeadWords.Length != words.Length) continue;
            FatherCardData data = ScriptableObject.CreateInstance<FatherCardData>();
            data.name = words[0];
            data.CardName = GetCardName(words[0]);
            data.CardDescription = words[2];
            data.IsStartCard = words[3].Equals("1") ? true : false;
            string[] priorCards = words[4].Split("|", options: System.StringSplitOptions.RemoveEmptyEntries);
            data.PriorCards = new List<Define.CardName>();
            foreach (var priorCard in priorCards)
            {
                data.PriorCards.Add(GetCardName(priorCard));
            }
            data.MaxUpgradeCount = words[5].Equals("") ? 0 : int.Parse(words[5]);
            data.IncreaseHp = words[6].Equals("") ? 0 : int.Parse(words[6]);
            data.IncreaseRecoverHpPower = words[7].Equals("") ? 0 : float.Parse(words[7]);
            data.IncreaseDamageReducePercentage = words[8].Equals("") ? 0 : float.Parse(words[8]);
            data.IncreaseAttackPoint = words[9].Equals("") ? 0 : int.Parse(words[9]);
            data.IncreaseNormalAttackSpeedPercentage = words[10].Equals("") ? 0 : float.Parse(words[10]);
            data.UnlockShockwave = words[11].Equals("1") ? true : false;
            data.IncreaseShockwaveDamagePercentage = words[12].Equals("") ? 0 : float.Parse(words[12]);
            data.IncreaseShockwaveRangePercentage = words[13].Equals("") ? 0 : float.Parse(words[13]);
            data.DecreaseShockwaveCoolTimePercentage = words[14].Equals("") ? 0 : float.Parse(words[14]);
            data.IncreaseShockwaveCount = words[15].Equals("") ? 0 : int.Parse(words[15]);
            data.UnlockStempGround = words[16].Equals("1") ? true : false;
            data.IncreaseStempGroundDamagePercentage = words[17].Equals("") ? 0 : float.Parse(words[17]);
            data.IncreaseStempGroundRangePercentage = ParseFloat(words[18]);

            AssetDatabase.CreateAsset(data, "Assets/" + CARD_FOLDER_DATA_PATH + data.name + ".asset");
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
    }
    void CreateDogCardData()
    {
        TextAsset textAsset = UnityEditor.AssetDatabase.LoadAssetAtPath<TextAsset>("Assets/" + DOG_CARD_DATA_PATH);
        if (textAsset == null) return;

        string[] lines = textAsset.text.Split('\n');

        string[] preHeadWords = lines[0].Split(',');
        for (int i = 1; i < lines.Length; i++)
        {
            string[] words = lines[i].Split(',');
            if (preHeadWords.Length != words.Length) continue;

            DogCardData data = ScriptableObject.CreateInstance<DogCardData>();
            data.name = words[0];
            data.CardName = GetCardName(words[0]);
            data.CardDescription = words[2];
            data.IsStartCard = words[3].Equals("1") ? true : false;
            string[] priorCards = words[4].Split("|", options: System.StringSplitOptions.RemoveEmptyEntries);
            data.PriorCards = new List<Define.CardName>();
            foreach (var priorCard in priorCards)
            {
                data.PriorCards.Add(GetCardName(priorCard));
            }
            data.MaxUpgradeCount = words[5].Equals("") ? 0 : int.Parse(words[5]);
            data.IncreaseHp = words[6].Equals("") ? 0 : int.Parse(words[6]);
            data.IncreaseRecoverHpPower = words[7].Equals("") ? 0 : float.Parse(words[7]);
            data.IncreaseDamageReducePercentage = words[8].Equals("") ? 0 : float.Parse(words[8]);
            data.IncreaseAttackPoint = words[9].Equals("") ? 0 : int.Parse(words[9]);
            data.IncreaseReflectionDamage = words[10].Equals("") ? 0 : int.Parse(words[10]);
            data.DecreaseReviveTimePercentage = words[11].Equals("") ?0 : float.Parse(words[11]);
            data.UnlockExplosionWhenDead = words[13].Equals("1") ? true : false;
            data.IncreaseExplosionDamage = words[14].Equals("") ? 0 : int.Parse(words[14]);
            data.IncreaseExplosionRange = words[15].Equals("") ? 0 : float.Parse(words[15]);
            data.UnlockReviveWhereDaughterPosition = words[16].Equals("1") ? true : false;

            AssetDatabase.CreateAsset(data, "Assets/" + CARD_FOLDER_DATA_PATH + data.name + ".asset");
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
    }
    void CreateCardData()
    {
        for (int i = 0; i < Define.CARD_COUNT; i++)
        {
            Define.CardName cardName = (Define.CardName)i;

            Object tempObj = UnityEditor.AssetDatabase.LoadAssetAtPath<UnityEngine.Object>("Assets/" + DAUGHTER_CARD_DATA_PATH + cardName.ToString() + ".prefab");
            if (tempObj == null)
            {
                CardData tempData = ScriptableObject.CreateInstance<CardData>();
                GameObject temp = Instantiate(tempObj) as GameObject; //�ν��Ͻ� �����
                temp.name = cardName.ToString();
                UnityEditor.PrefabUtility.SaveAsPrefabAsset(temp, "Assets/" + DAUGHTER_CARD_DATA_PATH + temp.name + ".prefab", out bool isSuccess); //����

                if (temp)
                    DestroyImmediate(temp);
                if (isSuccess)
                    AssetDatabase.DeleteAsset("Assets/" + DAUGHTER_CARD_DATA_PATH + cardName.ToString() + ".prefab");
            }
        }
    }
    public static T[] GetAssetsAtPath<T>(string path) where T : Object
    {
        List<T> returnList = new List<T>();

        //get the contents of the folder's full path (excluding any meta files) sorted alphabetically
        IEnumerable<string> fullpaths = Directory.GetFiles(FullPathForRelativePath(path)).Where(x => !x.EndsWith(".meta")).OrderBy(s => s);
        //loop through the folder contents
        foreach (string fullpath in fullpaths)
        {
            //determine a path starting with Assets
            string assetPath = fullpath.Replace(Application.dataPath, "Assets");
            //load the asset at this relative path
            Object obj = AssetDatabase.LoadAssetAtPath<T>(assetPath);
            //and add it to the list if it is of type T
            if (obj is T) { returnList.Add(obj as T); }
        }

        return returnList.ToArray();
    }

    private static string FullPathForRelativePath(string path)
    {
        throw new System.NotImplementedException();
    }

    Define.CardName GetCardName(string cardName)
    {
        for (int i = 0; i < Define.CARD_COUNT; i++)
        {
            if (cardName.Equals(((Define.CardName)i).ToString()))
            {
                return (Define.CardName)i;
            }
        }
        return Define.CardName.None;
    }

    int ParseInt(string value)
    {
        int result = 0;
        int.TryParse(value, out result);
        return result;
    }

    float ParseFloat(string value)
    {
        float result = 0;
        float.TryParse(value, out result);
        return result;
    }
    bool ParseBoolean(string value)
    {
        if (value.Equals("1"))
            return true;

        return false;
    }

}
