using MoreMountains.Feedbacks;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Analytics;
using UnityEngine.WSA;

public class GameManager : ManagerBase
{

    // ���� ����
    public Player Player { set; get; }

    public FatherAI FatherAI;

    Character _daugther;
    public Character Daughter { get { if (Player && _daugther == null) _daugther = Player.GetComponent<Character>(); return _daugther; } }

    public DogAI DogAI { set; get; }
    Character _dog;
    public Character Dog { get { if (DogAI && _dog == null) _dog = DogAI?.GetComponent<Character>(); return _dog; } }

    Character _father;
    public Character Father { get { if (FatherAI &&_father == null) _father = FatherAI?.GetComponent<Character>(); return _father; } }


    [Header("Debug")]
    [SerializeField] bool _summonDummy;
    [SerializeField] int _dummyHp;

    [Header("���� ����")]
    [SerializeField] bool _stop;
    [field: SerializeField]public  float MapSize { set; get; }


    // �÷��� ���� ����
    [Header("����ġ")]
    [SerializeField] bool _allMaxExpFive;
    [SerializeField] int _exp;
    public int Exp {
        set
        {
            _exp = value;
            if (_exp >= MaxExp)
            {
                _exp = _exp - MaxExp;
                Level++;
                Managers.GetManager<UIManager>().GetUI<UICardSelection>().Open();
            }
        }
        get { return _exp; }
    }
    public int MaxExp
    {
        get
        {
            if (_allMaxExpFive) return 5;
            return Level * 3;
        }
    }
    public int Level { set; get; }

    // �� ��ȯ ���� ����
    [Header("�����ú���")]
    [SerializeField] bool _isEndless;
    List<GameObject> _enemySpawnList = new List<GameObject>();
    [SerializeField] List<Wave> _waveList;

    [SerializeField]int _currentWave = 0;
    public int CurrentWave => _currentWave;
    int _spawnCount;
    int _maxSpawnCount = 5;
    bool _isStartWave;
    public bool IsStartWave => _isStartWave;
    float _totalTime;
    float _genTime;

    int _nextTime;

    [Header("ī�� ������")]
    List<CardSelectionData> _remainCardSelectionList;
    Dictionary<Define.CardSelection, int> _cardSelectionCount = new Dictionary<Define.CardSelection, int>();


    Vector3 _preMousePosition;

    private int _money;
    public int Money
    {
        set
        {
            _money = value;
        }
        get
        {
            return _money;
        }
    }

    public bool IsStopWave { get; set; }

    // ���� ���ú���
    Map _map;

    public override void Init()
    {
        _remainCardSelectionList = Managers.GetManager<DataManager>().GetDataList<CardSelectionData>((d) => { return d.IsStartCard; });
        _map = new Map(60f);
        _map.SetCenterGround(GameObject.Find("Ground"));
        _map.AddBuildingPreset("Prefabs/BuildingPreset1");
        _map.AddBuildingPreset("Prefabs/BuildingPreset2");
        _map.AddBuildingPreset("Prefabs/BuildingPreset3");
        _map.AddBuildingPreset("Prefabs/BuildingPreset4");
        _map.AddMoreBackBuildingPreset("Prefabs/MoreBackBuildingPreset1");
    }

    public override void ManagerUpdate()
    {
        _totalTime += Time.deltaTime;
        HandleGround();
        if (_summonDummy)
        {
            EnemyAI enemyOrigin = Managers.GetManager<DataManager>().GetData<EnemyAI>((int)Define.EnemyName.Walker1);
            EnemyAI enemy = Managers.GetManager<ResourceManager>().Instantiate(enemyOrigin);
            enemy.transform.position = Player.transform.position + Vector3.right*3;
            Character enemyCharacter = enemy.GetComponent<Character>();
            enemyCharacter.SetHp(_dummyHp);
            _summonDummy = false;
        }


        if (_stop) return;
        if (!IsStopWave)
        {
            if (_isEndless)
            {
                EndlessWave();
            }
        }

        
        if(Player && Player.transform.position.x > MapSize)
        {
            _stop = true;

            for(int i = _enemySpawnList.Count-1; i >= 0; i--)
            {
                Managers.GetManager<ResourceManager>().Destroy(_enemySpawnList[i]);
                _enemySpawnList.RemoveAt(i);
            }
        }
    }

    void HandleGround()
    {
        if (Player == null) return;

        _map.Update(Player);
    }

    void EndlessWave()
    {
        if (_currentWave < _waveList.Count-1)
        {
            if (_totalTime > _waveList[_currentWave + 1].time)
                _currentWave++;
        }

        if (_genTime > _waveList[_currentWave].genTime)
        {
            _genTime = 0;
            if (_waveList[_currentWave].enemyList == null) return;

            EnemyAI enemyOrigin = Managers.GetManager<DataManager>().GetData<EnemyAI>((int)_waveList[_currentWave].enemyList.GetRandom());
            EnemyAI enemy = Managers.GetManager<ResourceManager>().Instantiate(enemyOrigin);
            Character enemyCharacter = enemy.GetComponent<Character>();
            enemyCharacter.SetHp((int)(enemyCharacter.MaxHp * _waveList[_currentWave].hpMultiply));

            Vector3 randomPosition = Vector3.zero;
            float distance = 50;
            if (enemyCharacter.IsEnableFly)
            {
                float angle = 0;
                if(Random.Range(0,2) == 0)
                    angle = Random.Range(30, 70);
                else
                    angle = Random.Range(110, 150);

                angle = angle * Mathf.Deg2Rad;
                randomPosition.x = Player.transform.position.x + Mathf.Cos(angle) * distance; 
                randomPosition.y = Player.transform.position.y + Mathf.Sin(angle) * distance;

            }
            else
            {
                randomPosition.x = Player.transform.position.x + (enemy.EnemyName == Define.EnemyName.Walker1 ? distance : -distance);
                randomPosition.y = _map.YPosition+1;
            }
            enemyCharacter.transform.position =  randomPosition;
            if(enemy)
                _enemySpawnList.Add(enemy.gameObject);

        }
        else
        {
            _genTime += Time.deltaTime;
        }
    }

    public CardSelectionData GetRandomCardSelectionData()
    {
        return _remainCardSelectionList.GetRandom();
    }
    public List<CardSelectionData> GetRandomCardSelectionData(int count)
    {
        return _remainCardSelectionList.GetRandom(count);
    }
    public List<CardSelectionData> GetRemainCardSelection()
    {
        return _remainCardSelectionList;
    }
    // ī�带 �����Ͽ� �ɷ�ġ �߰�
    public void SelectCardData(CardSelectionData data)
    {
        if (!_cardSelectionCount.ContainsKey(data.CardSelection))
        {
            _cardSelectionCount.Add(data.CardSelection, 0);
            _remainCardSelectionList.AddRange(data.CardListToAdd);
        }
        _cardSelectionCount[data.CardSelection]++;

        if(data.MaxUpgradeCount <= _cardSelectionCount[data.CardSelection])
        {
            _remainCardSelectionList.Remove(data);
        }

        // TODO
        // �ɷ�����

        if(data.CardSelectionType == Define.CardSelectionType.Weapon)
        {
            WeaponCardSelection weaponCardSelection = data as WeaponCardSelection;

            WeaponSwaper swaper = Player.GetComponent<WeaponSwaper>();

            swaper.ChangeNewWeapon(weaponCardSelection.WeaponSlotIndex, weaponCardSelection.WeaponName);
        }
        else
        {
            if(data.CardSelection == Define.CardSelection.�ִ�ü������)
            {
                Daughter.SetMaxHp(Daughter.MaxHp + 2);
            }
            if(data.CardSelection == Define.CardSelection.�ݵ�����)
            {
                Player.SetReboundControlPower(Player.ReboundControlPower + 10);
            }
            if(data.CardSelection == Define.CardSelection.�Ѿ˰��������)
            {
                Player.PenerstratingPower++;
            }
            if (data.CardSelection == Define.CardSelection.�������ð�����)
            {
                Player.ReduceReloadTime += 10;
            }
            if (data.CardSelection == Define.CardSelection.���Ǿ�ɷ�����)
            {
                FatherAI.IsUnlockSpear= true;
            }
            if (data.CardSelection == Define.CardSelection.��ũ���̺�ɷ�����)
            {
                FatherAI.IsUnlockShockwave = true;
            }
            if (data.CardSelection == Define.CardSelection.�溮ũ������)
            {
                Vector3 scale = Dog.transform.localScale;
                scale.x += 0.1f;
                scale.y += 0.1f;
                Dog.transform.localScale = scale;
                Dog.SetMaxHp(Dog.MaxHp + 5);
            }
        }
    }
    public int GetCardSelectionCount(Define.CardSelection cardSelection)
    {
        int count = 0;
        _cardSelectionCount.TryGetValue(cardSelection, out count);

        return count;
    }
}

[System.Serializable]
struct Wave
{
    public int time;
    public float genTime;
    public List<Define.EnemyName> enemyList;
    public float hpMultiply;
}

class Map
{
    int currentIndex = -10002;
    int moreBackBuildingIndex = -1000;
    int groundCount = 5;

    Dictionary<int, GameObject> grounds = new Dictionary<int, GameObject>();
   
    GameObject leftBuilding;
    GameObject centerBuilding;
    GameObject rightBuilding;

    GameObject moreBackLeftBuilding;
    GameObject moreBackCenterBuilding;
    GameObject moreBackRightBuilding;

    List<string> buildingPresetPathList = new List<string>();
    List<string> moreBackBuildingPresetPathList = new List<string>();

    float groundTerm;
    float yPosision = -8.91f;
    public float YPosition => yPosision;

    GameObject groundFolder;
    public Map(float groundTenm)
    {
        this.groundTerm = groundTenm;
        groundFolder = new GameObject("GroundFolder");
        groundFolder.layer = LayerMask.NameToLayer("Ground");
        groundFolder.AddComponent<Rigidbody2D>().isKinematic = true;
        groundFolder.AddComponent<CompositeCollider2D>();
    }

    public void Update(Player player)
    {
        if(player == null) return;

        Vector3 pos = player.transform.position;
        SetGround(GetIndex(pos.x));

        int mul = 4;

        int index = Mathf.RoundToInt(pos.x / (groundTerm* mul));
        if(index != moreBackBuildingIndex)
        {
            if(moreBackBuildingPresetPathList.Count > 0)
            {
                // �߰�
                Random.InitState(Mathf.RoundToInt(index / moreBackBuildingPresetPathList.Count));
                int random = (int)(Random.value * 1000);

                if (moreBackCenterBuilding)
                    Managers.GetManager<ResourceManager>().Destroy(moreBackCenterBuilding);
                Vector3 position = new Vector3(index * groundTerm * mul, 0, 0);
                position.y = -2f;
                moreBackCenterBuilding = Managers.GetManager<ResourceManager>().Instantiate(moreBackBuildingPresetPathList[(random + index) % moreBackBuildingPresetPathList.Count]);
                moreBackCenterBuilding.transform.parent = groundFolder.transform;
                // ����
                Random.InitState(Mathf.RoundToInt((index - 1) / moreBackBuildingPresetPathList.Count));
                random = (int)(Random.value * 1000);

                if (moreBackLeftBuilding)
                    Managers.GetManager<ResourceManager>().Destroy(moreBackLeftBuilding);
                position = new Vector3((index-1) * groundTerm * mul, 0, 0);
                position.y = -2f;
                moreBackLeftBuilding = Managers.GetManager<ResourceManager>().Instantiate(moreBackBuildingPresetPathList[(random + index - 1) % moreBackBuildingPresetPathList.Count]);
                moreBackLeftBuilding.transform.parent = groundFolder.transform;
                //������
                Random.InitState(Mathf.RoundToInt((index + 1) / moreBackBuildingPresetPathList.Count));
                random = (int)(Random.value * 1000);

                if (moreBackRightBuilding)
                    Managers.GetManager<ResourceManager>().Destroy(moreBackRightBuilding);
                position = new Vector3((index + 1) * groundTerm * mul , 0, 0);
                position.y = -2f;
                moreBackRightBuilding = Managers.GetManager<ResourceManager>().Instantiate(moreBackBuildingPresetPathList[(random + index + 1) % moreBackBuildingPresetPathList.Count]);
                moreBackRightBuilding.transform.parent = groundFolder.transform;
            }

            moreBackBuildingIndex = index;
        }

        float distacne = Camera.main.transform.position.x - moreBackBuildingIndex * groundTerm * mul;

        moreBackCenterBuilding.transform.position = new Vector3(moreBackBuildingIndex * groundTerm * mul + distacne/2, 0, 0);
        moreBackRightBuilding.transform.position = new Vector3(moreBackCenterBuilding.transform.position.x + groundTerm * 2, 0, 0);
        moreBackLeftBuilding.transform.position = new Vector3(moreBackCenterBuilding.transform.position.x - groundTerm * 2, 0, 0);

    }

  
    public void SetGround(int index)
    {
        if (index == currentIndex) return;

        Random.InitState(Mathf.RoundToInt(index / groundCount));
        int groundRandom = (int)(Random.value * 1000);

        List<int> indexList = grounds.Keys.ToList();

        for (int i = 0; i < groundCount; i++)
        {
            int groundIndex = index + ((i % 2) == 0 ? -1 : 1) * (Mathf.FloorToInt((i-1) / 2)+1) * (i == 0 ? 0 : 1);
            // int randomGroundTile = (groundRandom + groundIndex) % groundCount;

            if (!grounds.ContainsKey(groundIndex))
            {
                grounds.Add(groundIndex, Managers.GetManager<ResourceManager>().Instantiate("Ground"));
                grounds[groundIndex]?.transform.SetParent(groundFolder.transform);
                grounds[groundIndex].transform.position = new Vector3(groundTerm * groundIndex, YPosition, 0);
            }

            indexList.Remove(groundIndex);
        }

        for(int i = indexList.Count-1; i >= 0; i--) 
        {
            Managers.GetManager<ResourceManager>().Destroy(grounds[indexList[i]]);
            grounds.Remove(indexList[i]);
        }


        currentIndex= index;


        if (buildingPresetPathList.Count > 0)
        {

            // �߰�
            Random.InitState(Mathf.RoundToInt(index / buildingPresetPathList.Count));
            int random = (int)(Random.value * 1000);

            if (centerBuilding)
                Managers.GetManager<ResourceManager>().Destroy(centerBuilding);
            Vector3 position = grounds[currentIndex].transform.position;
            position.y = Random.Range(-1f, -4f);
            centerBuilding = Managers.GetManager<ResourceManager>().Instantiate(buildingPresetPathList[(random + index) % buildingPresetPathList.Count]);
            centerBuilding.transform.parent = groundFolder.transform;
            centerBuilding.transform.position = position;

            // ����
            Random.InitState(Mathf.RoundToInt((index-1) / buildingPresetPathList.Count));
            random = (int)(Random.value * 1000);

            if (leftBuilding)
                Managers.GetManager<ResourceManager>().Destroy(leftBuilding);
            position = grounds[currentIndex-1].transform.position;
            position.y = Random.Range(-1f, -4f);
            leftBuilding = Managers.GetManager<ResourceManager>().Instantiate(buildingPresetPathList[(random + index-1) % buildingPresetPathList.Count]);
            leftBuilding.transform.parent = groundFolder.transform;
            leftBuilding.transform.position = position;

            //������
            Random.InitState(Mathf.RoundToInt((index + 1) / buildingPresetPathList.Count));
            random = (int)(Random.value * 1000);

            if (rightBuilding)
                Managers.GetManager<ResourceManager>().Destroy(rightBuilding);
            position = grounds[currentIndex+1].transform.position;
            position.y = Random.Range(-1f, -4f);
            rightBuilding = Managers.GetManager<ResourceManager>().Instantiate(buildingPresetPathList[(random + index + 1) % buildingPresetPathList.Count]);
            rightBuilding.transform.parent = groundFolder.transform;
            rightBuilding.transform.position = position;
        }
    }

    public void SetCenterGround(GameObject ground)
    {
        grounds.Add(0,ground);

        for (int i = 1; i < groundCount; i++)
        {
            int groundIndex = ((i % 2) == 0 ? -1 : 1) * (Mathf.FloorToInt((i - 1) / 2) + 1);
            grounds.Add(groundIndex, Managers.GetManager<ResourceManager>().Instantiate("Ground"));
            grounds[groundIndex]?.transform.SetParent(groundFolder.transform);
            grounds[groundIndex].transform.position = new Vector3(groundTerm * groundIndex, YPosition, 0);
        }
    }

    public void AddBuildingPreset(string path)
    {
        buildingPresetPathList.Add(path);
    }
    public void AddMoreBackBuildingPreset(string path)
    {
        moreBackBuildingPresetPathList.Add(path);
    }

    public int GetIndex(float x)
    {
        int index = Mathf.RoundToInt(x / (groundTerm ));
        return index;
    }
}

