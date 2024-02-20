using System.Collections;
using UnityEditor.Rendering;
using UnityEngine;

public class Weapon : MonoBehaviour, ITypeDefine
{
    protected Character _character;
    protected Player _player;
    protected AudioSource _audioSource;
    protected Character Character
    {
        get { if (_character == null) _character = GetComponentInParent<Character>(); return _character; }
    }
    protected Player Player
    {
        get { if(_player == null) _player = GetComponentInParent<Player>(); return _player; }
    }


    [SerializeField]protected Define.WeaponName _weaponName;
    public Define.WeaponName WeaponName =>_weaponName;

    [SerializeField][Range(0,1)] float _audioLength;
    Coroutine _audioCoroutine;

    [SerializeField]protected bool _isRaycast;
    [SerializeField] protected bool _isAuto;
    public bool IsAuto => _isAuto;

    [SerializeField] protected float _knockBackPower = 1;
    public float KnockBackPower => _knockBackPower;
    [SerializeField] protected float _bulletSpeed = 15;
    public float BulletSpeed => _bulletSpeed;

    [SerializeField] protected int _damage = 1;
    public int Damage => (_damage + (_player ? _player.IncreasedDamage : 0));

    [SerializeField] int _penerstratingPower;
    public int PenerstratingPower => (_penerstratingPower+ (_player? _player.IncreasedPenerstratingPower:0));

    [SerializeField] protected int _maxAmmo;
    public int MaxAmmo => _maxAmmo;
    [SerializeField]protected int _currentAmmo;
    public int CurrentAmmo => _currentAmmo;

    [SerializeField] protected bool _isAllReloadAmmo;
    public bool IsAllReloadAmmo => _isAllReloadAmmo;
    [SerializeField] protected float _fireDelay;
    public float FireDelay => _fireDelay + (_player? -(_player.DecreasedFireRatePercent/100f) *_fireDelay:0);
    [SerializeField] protected float _reloadDelay;
    public float ReloadDelay => _reloadDelay - (_player? (_player.IncreasedReloadSpeedPercent /100f)* _reloadDelay:0);

    [SerializeField] protected GameObject _firePosition;
    public GameObject FirePosition => _firePosition;

    [SerializeField]protected float _rebound;
    public float Rebound => _rebound;

    protected float _fireElapsed;

    protected float _reloadElapsed;
    public float ReloadElapsed => _reloadElapsed;

    protected bool _isReload;
    public bool IsReload => _isReload;

    [SerializeField] protected GaugeBar _reloadGauge;

    [SerializeField] protected Define.EffectName _hitEffect;

    bool _fastReloadFailed;

    public GameObject HandlePosition;

    public void Init(Character character)
    {
        _audioSource = GetComponent<AudioSource>();
        _currentAmmo = _maxAmmo;
        _character = character;
        _reloadGauge = _character.transform.Find("GagueBar").GetComponent<GaugeBar>();
        HandlePosition = transform.Find("HandlePosition").gameObject;
        
        if(_reloadGauge)
            _reloadGauge.gameObject.SetActive(false);

    }
    public virtual void Fire(Character fireCharacter)
    {
        if (_currentAmmo <= 0)
        {
            Reload();
            return;
        }

        if (_fireElapsed < FireDelay) return;

        // ������ ���̶�� �������� ����ϴ�.
        if (_isReload)
        {
            CancelReload();
        }


        _currentAmmo--;
        _fireElapsed = 0;

        if (_audioCoroutine != null)
            StopCoroutine(_audioCoroutine);
        _audioCoroutine = StartCoroutine(CorPlayAudio());

        float angle = FirePosition.transform.rotation.eulerAngles.z;
        angle = angle * Mathf.Deg2Rad;
        Vector3 direction = new Vector3(Mathf.Cos(angle) * transform.lossyScale.x/ Mathf.Abs(transform.lossyScale.x), Mathf.Sin(angle) * transform.lossyScale.x / Mathf.Abs(transform.lossyScale.x), 0);
        direction = direction.normalized;
        Effect fireFlareOrigin = Managers.GetManager<DataManager>().GetData<Effect>((int)Define.EffectName.FireFlare);
        Effect fireFlare = Managers.GetManager<ResourceManager>().Instantiate(fireFlareOrigin);
        fireFlare.Play(_firePosition.transform.position);
        fireFlare.transform.rotation = _firePosition.transform.rotation;

        if (!_isRaycast)
        {
            GameObject go = Managers.GetManager<ResourceManager>().Instantiate("Prefabs/Projectile");
            go.transform.position = _firePosition.transform.position;
            Projectile projectile = go.GetComponent<Projectile>();
            int damage = Damage;

            // �÷��̾ ��Ʈ�� �ɷ��� �ִٸ� ������ 3��
            if (Player.IsHaveLastShot && _currentAmmo == 0)
                damage = Damage * 3;
            projectile.Init(KnockBackPower, BulletSpeed, damage,Define.CharacterType.Enemy,PenerstratingPower);
            projectile.Fire(fireCharacter, direction.normalized);
        }
        else
        {
            RaycastHit2D hit = Physics2D.Raycast(_firePosition.transform.position, direction.normalized,Mathf.Infinity,LayerMask.GetMask("Character") | LayerMask.GetMask("Ground"));

            if (hit.collider != null)
            {
                if (_hitEffect != Define.EffectName.None)
                {
                    Effect effect = Managers.GetManager<EffectManager>().GetEffect(_hitEffect);
                    effect.SetProperty("Direction", direction);
                    effect.Play(hit.point);
                }

                Character character = hit.collider.gameObject.GetComponent<Character>();

                if (character != null)
                {
                    character.Damage(Character, _damage, _knockBackPower, direction);
                }

            }
        }

        Player?.Rebound(_rebound);
    }
    public void Update()
    {
        if (_fireElapsed < FireDelay)
            _fireElapsed += Time.deltaTime;

        if (_isReload && _reloadElapsed < ReloadDelay)
        {
            _reloadElapsed += Time.deltaTime;
            if(_reloadGauge)
                _reloadGauge.SetRatio(_reloadElapsed, ReloadDelay);
        }
        else if(_isReload && _reloadElapsed >= ReloadDelay)
        {
            CompleteReload();
        }
    }

    public void Reload()
    {
        if (_maxAmmo <= _currentAmmo) return;
        if (_isReload) return;

        _fastReloadFailed = false;
        _isReload = true;
        if (_reloadGauge)
        {
            if (_player)
            {
                if (_player.IsHaveFastReload)
                {
                    _reloadGauge.Point(0.7f, 0.9f);
                }
                else
                {
                    _reloadGauge.DisablePoint();
                }
            }
            _reloadGauge.SetRatio(0, 1);
            _reloadGauge.gameObject.SetActive(true);
        }
    }

    public void FastReload()
    {
        if (_fastReloadFailed) return;

        float ratio = ReloadElapsed / ReloadDelay;
        if (ratio > 0.7f && ratio < 0.9f)
        {
            CompleteReload();
        }
        else
        {
            _fastReloadFailed = true;
        }
    }
    public void CompleteReload()
    {
        if (_isAllReloadAmmo)
        {
            _isReload = false;
            if (_player.IsHaveExtraAmmo)
                _currentAmmo = Mathf.CeilToInt(_maxAmmo * 1.5f);
            else
                _currentAmmo = _maxAmmo;
            _reloadElapsed = 0;
            if (_reloadGauge)
                _reloadGauge.gameObject.SetActive(false);
        }
        else
        {
            _currentAmmo++;
            if (_currentAmmo >= _maxAmmo)
            {
                _currentAmmo = _maxAmmo;
                _isReload = false;
                if (_reloadGauge)
                    _reloadGauge.gameObject.SetActive(false);
            }
            _reloadElapsed = 0;
        }
    }

    public void CancelReload()
    {
        _isReload = false;
        _reloadGauge.gameObject.SetActive(false);
        _reloadGauge.SetRatio(0, 1);
        _reloadElapsed = 0;
    }

    public void SetPower(float power)
    {
        _knockBackPower = power;
    }

    public void SetDamage(int damage)
    {
        _damage = damage;
    }

    public void IncreaseMaxAmmo(int count)
    {
        _maxAmmo += count;
    }

    public void DecreaseReloadDelay(float delay)
    {
        _reloadDelay -= delay;
    }

    public int GetEnumToInt()
    {
        return (int)WeaponName;
    }

    IEnumerator CorPlayAudio()
    {
        if (_audioSource) { 
        _audioSource.Stop();
        _audioSource.Play();
        yield return new WaitForSeconds(_audioLength);
        _audioSource.Stop();
            }
    }
}
