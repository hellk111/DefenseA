using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.TextCore.Text;

public class Character : MonoBehaviour
{
    Rigidbody2D _rigidBody;

    [Header("캐릭터 능력치")]
    [SerializeField] Define.CharacterType _characterType;
    public Define.CharacterType CharacterType=>_characterType;
    [SerializeField] int _maxHp;
    public int MaxHp => _maxHp;
    [SerializeField] int _hp;
    public int Hp => _hp;

    [SerializeField] float _speed;
    [SerializeField] bool _isEnableFly;
    public bool IsEnableFly => _isEnableFly;
    float _stunEleasped;
    float _stunTime;

    // 캐릭터 행동상태
    [SerializeField] bool _isTurnBodyAlongVelocity = true;
    public bool IsStun {private set; get; }
    public bool IsAttack {private set; get; }
    bool IsMove = false;

    [field:SerializeField]public bool IsEnableMove { set; get; } = true;
    Character _attackTarget;

    public Action<Character> CharacterAttack;
    public Action CharacterDead;

    [SerializeField] GaugeBar _hpBar;

    public List<Define.Passive> CharacterPassive = new List<Define.Passive>();


    Animator _animator;

    Vector3 _prePosition;
    Vector3 _mySpeed;
    public Vector3 MySpeed => _mySpeed;

    private void Awake()
    {
        _rigidBody = GetComponent<Rigidbody2D>();
        _animator = GetComponent<Animator>();
        _hp = _maxHp;
    }

    private void Update()
    {
        ControlAnimation();
        if (_hpBar)
        {
            _hpBar.SetRatio(_hp, _maxHp);
        }

        if (IsStun)
        {
            _stunEleasped += Time.deltaTime;
            if (_stunEleasped > _stunTime)
            {
                IsStun = false;
                _stunEleasped = 0;
            }
        }
        
    }
    private void FixedUpdate()
    {
        Vector3 currentPosition = transform.position;
        _mySpeed = (currentPosition - _prePosition) / Time.fixedDeltaTime;
        _prePosition = currentPosition;
    }

    void ControlAnimation()
    {
        if (!_animator) return;
        if(!IsStun)
        {
           
            
            _animator.SetFloat("WalkBlend", Mathf.Clamp(Mathf.Abs(_rigidBody.velocity.x)/_speed,0,1));
            IsMove = false;
        }
    }

    public void SetHp(int hp)
    {
        _maxHp = hp;
        _hp = hp;
    }
    
    public void SetMaxHp(int maxHp) 
    {
        _maxHp = maxHp;
    }

    public void Damage(Character attacker, int damage, float power, Vector3 direction, float stumTime = 0.1f)
    {
        Managers.GetManager<TextManager>().ShowText(transform.position + Vector3.up, damage.ToString(), 10, Color.red);

        _hp -= damage;
        _rigidBody.AddForce(direction.normalized * power, ForceMode2D.Impulse);


        if (_hp <= 0)
        {
            CharacterDead?.Invoke();
            Destroy(gameObject);
        }

        IsStun = true;
        _stunEleasped = 0;
        _stunTime = stumTime;
    }

    public void Move(Vector2 direction)
    {
        if (IsStun) return;

        // 진행 방향에 맞게 몸을 회전
        if (_isTurnBodyAlongVelocity)
        {
            if (direction.x > 0)
                transform.localScale = new Vector3(1, 1, 1);
            else if (direction.x < 0)
                transform.localScale = new Vector3(-1, 1, 1);
        }

        direction.x = Mathf.Clamp(direction.x, -1, 1);
        direction.y = Mathf.Clamp(direction.y, -1, 1);

        // 움직임 제어
        if (IsEnableMove)
        {
            IsMove = true;
            if (_isEnableFly)
                _rigidBody.velocity = new Vector2(direction.x * _speed, direction.y * _speed);
            else
                _rigidBody.velocity = new Vector2(direction.x * _speed, _rigidBody.velocity.y);
        }
    }

    public void Attack(Character character)
    {
        if (IsStun || IsAttack) return;

        IsAttack = true;
        _attackTarget = character;
        CharacterAttack?.Invoke(_attackTarget);
    }

    public void AnimatorSetBool(string name, bool value)
    {
        _animator.SetBool(name, value);
    }

}
