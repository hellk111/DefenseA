using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using System.Linq;
using System.Xml.XPath;
using TMPro;
using UnityEngine;

public class ResourceManager : ManagerBase
{
    Dictionary<string, List<Poolable>> _pool = new Dictionary<string, List<Poolable>>();
    public override void Init()
    {
    }

    public override void ManagerUpdate()
    {
        GameObject gp;
    }

    public GameObject Instantiate(string name)
    {
        GameObject result = null;

        GameObject origin = Resources.Load<GameObject>(name);

        if (origin == null)
        {
            Debug.LogWarning($"{name}�� �ش��ϴ� ���ҽ��� �����ϴ�.");
            return null;
        }
        Poolable pool = null;
        // Ǯ�� ������ ������Ʈ��� Ǯ���� ������Ʈ�� ã���ϴ�.
        if ((pool = origin.GetComponent<Poolable>()) != null)
        {
            if (_pool[name] != null)
            {
                foreach (var p in _pool[name])
                {
                    if (!p.IsUsed)
                    {
                        result = p.gameObject;
                        p.IsUsed = true;
                        break;
                    }
                }

            }
        }

        // Ǯ�� ������ ������Ʈ�� ���ٸ� ���� ������ݴϴ�.
        if (result == null)
        {
            result = Object.Instantiate(origin);
            // Ǯ�� �����ϴٸ� Ǯ�����ݴϴ�.
            if (pool != null)
            {
                if (_pool[name] == null)
                    _pool[name] = new List<Poolable>();

                _pool[name].Add(result.GetComponent<Poolable>());
                _pool[name][_pool[name].Count - 1].IsUsed = true;
            }
        }

        result.gameObject.SetActive(true);

        return result;
    }

    public T Instantiate<T>(string name) where T : MonoBehaviour
    {
        T result = null;

        T origin = Resources.Load<T>(name);

        if (origin == null)
        {
            Debug.LogWarning($"{name}�� �ش��ϴ� ���ҽ��� �����ϴ�.");
            return null;
        }
        Poolable pool = null;
        // Ǯ�� ������ ������Ʈ��� Ǯ���� ������Ʈ�� ã���ϴ�.
        if ((pool = origin.GetComponent<Poolable>()) != null )
        {
            if(_pool[name] != null)
            {
                foreach (var p in _pool[name])
                {
                    if (!p.IsUsed)
                    {
                        result = p.GetComponent<T>();
                        p.IsUsed = true;
                        break;
                    }
                }

            }
        }

        // Ǯ�� ������ ������Ʈ�� ���ٸ� ���� ������ݴϴ�.
        if (result == null)
        {
            result = Object.Instantiate<T>(origin);
            // Ǯ�� �����ϴٸ� Ǯ�����ݴϴ�.
            if (pool != null)
            {
                if (_pool[name] == null)
                    _pool[name] = new List<Poolable>();

                _pool[name].Add(result.GetComponent<Poolable>());
                _pool[name][_pool[name].Count - 1].IsUsed = true;
            }
        }
        result.gameObject.SetActive(true);

        return result;
    }
    public new T Instantiate<T>(T origin) where T : MonoBehaviour
    {
        T result = null;

        if (origin == null)
        {
            Debug.LogWarning($"{name}�� �ش��ϴ� ���ҽ��� �����ϴ�.");
            return null;
        }
        Poolable pool = null;
        // Ǯ�� ������ ������Ʈ��� Ǯ���� ������Ʈ�� ã���ϴ�.
        if ((pool = origin.GetComponent<Poolable>()) != null)
        {
            if (_pool[name] != null)
            {
                foreach (var p in _pool[name])
                {
                    if (!p.IsUsed)
                    {
                        result = p.GetComponent<T>();
                        p.IsUsed = true;
                        break;
                    }
                }

            }
        }

        // Ǯ�� ������ ������Ʈ�� ���ٸ� ���� ������ݴϴ�.
        if (result == null)
        {
            result = Object.Instantiate<T>(origin);
            // Ǯ�� �����ϴٸ� Ǯ�����ݴϴ�.
            if (pool != null)
            {
                if (_pool[name] == null)
                    _pool[name] = new List<Poolable>();

                _pool[name].Add(result.GetComponent<Poolable>());
                _pool[name][_pool[name].Count - 1].IsUsed = true;
            }
        }
        result.gameObject.SetActive(true);

        return result;
    }
    public void Destroy(GameObject gameObject)
    {
        Poolable pool = null;
        if(pool= gameObject.GetComponent<Poolable>())
        {
            pool.IsUsed = false;
            gameObject.SetActive(false);
        }
        else
        {
            Object.Destroy(gameObject);
        }
    }
}
