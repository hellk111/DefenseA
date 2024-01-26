using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIInGame : UIBase
{
    StringBuilder _sb = new StringBuilder();
    [SerializeField]TextMeshProUGUI _waveText;
    [SerializeField]TextMeshProUGUI _characterStateText;

    [SerializeField] Image _weapon1Image;
    [SerializeField] Image _weapon2Image;
    [SerializeField] Image _weapon3Image;

    [SerializeField] Image _expMaxImage;
    [SerializeField] Image _expCurrentImage;
    [SerializeField] TextMeshProUGUI _levelText;

    Player _player;

    public override void Init()
    {
        _isInitDone = true;
    }

    private void Update()
    {
        if (!_isInitDone) return;

        if (_waveText)
            _waveText.text = $"���� ���̺� : {Managers.GetManager<GameManager>().CurrentWave}\n �� : {Managers.GetManager<GameManager>().Money}";

        _sb.Clear();
        Character character = _player.Character;
        WeaponSwaper weaponSwaper = _player.WeaponSwaper;
        if(character)
        {
            _sb.Append($"ü��: {character.MaxHp}\n");
        }
        if(weaponSwaper.CurrentWeapon)
        {
            _sb.Append($"���ݷ� : {weaponSwaper.CurrentWeapon.Damage}\n");
            _sb.Append($"�˹� : {weaponSwaper.CurrentWeapon.Power}\n");
            _sb.Append($"�Ѿ˼ӵ� : {weaponSwaper.CurrentWeapon.BulletSpeed}\n");
            _sb.Append($"�߻� ������ : {weaponSwaper.CurrentWeapon.FireDelay}\n");
            _sb.Append($"���ε�ӵ� : {weaponSwaper.CurrentWeapon.ReloadDelay}\n");
            _sb.Append($"�Ѿ� :{weaponSwaper.CurrentWeapon.CurrentAmmo}/{weaponSwaper.CurrentWeapon.MaxAmmo}\n");
        }
        if(_characterStateText)
        {
            _characterStateText.text = _sb.ToString();
        }
        if(weaponSwaper.WeaponIndex == 0 && _weapon1Image)
        {
            _weapon1Image.color = Color.green;
            _weapon2Image.color = Color.white;
            _weapon3Image.color = Color.white;
        }
        if (weaponSwaper.WeaponIndex == 1 && _weapon2Image)
        {
            _weapon1Image.color = Color.white;
            _weapon2Image.color = Color.green;
            _weapon3Image.color = Color.white;
        }
        if (weaponSwaper.WeaponIndex == 2 && _weapon3Image)
        {
            _weapon1Image.color = Color.white;
            _weapon2Image.color = Color.white;
            _weapon3Image.color = Color.green;
        }

        if(_expMaxImage && _expCurrentImage)
        {
            int maxExp = Managers.GetManager<GameManager>().MaxExp; 
            int exp = Managers.GetManager<GameManager>().Exp; 
            _expCurrentImage.rectTransform.sizeDelta
                = new Vector2(_expMaxImage.rectTransform.sizeDelta.x * exp/maxExp, _expMaxImage.rectTransform.sizeDelta.y);

            _levelText.text = Managers.GetManager<GameManager>().Level.ToString();
        }
    }

    public void SetPlayerCharacter(Player player)
    {
        _player= player;
    }

    public override void Open()
    {
        
    }
    public override void Close()
    {
        
    }
}
