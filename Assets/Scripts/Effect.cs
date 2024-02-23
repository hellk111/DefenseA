using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class Effect : MonoBehaviour, ITypeDefine
{
    VisualEffect _visualEffect;

    [SerializeField] Define.EffectName _effectName;

    [SerializeField]float _offTime = 1f;
    float _eleasped;

    bool _isPlay;
    private void Awake()
    {
        _visualEffect = GetComponent<VisualEffect>();
        _visualEffect.Stop();
    }

    private void Update()
    {
        if (_isPlay)
        {
            _eleasped += Time.deltaTime;
        }
        if(_visualEffect && _eleasped >= _offTime)
        {
            _visualEffect.Stop();
            gameObject.SetActive(false);
            _isPlay= false;
            _eleasped=0;

            Managers.GetManager<ResourceManager>().Destroy(gameObject);
        }
    }
    public int GetEnumToInt()
    {
        return (int)_effectName;
    }

    public void SetProperty(string id, Vector3 vector)
    {
        _visualEffect.SetVector3(id, vector);
    }
    public void SetProperty(string id, float value)
    {
        _visualEffect.SetFloat(id, value);
    }
    public void SetProperty(string id, int value)
    {
        _visualEffect.SetInt(id, value);
    }

    public void Play(Vector3 position)
    {
        transform.position = position;
        gameObject.SetActive(true);
        _visualEffect.Play();
        _isPlay = true;
    }

}
