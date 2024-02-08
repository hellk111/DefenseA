using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceManager : ManagerBase
{
    Dictionary<string, List<Poolable>> _pool = new Dictionary<string, List<Poolable>>();
    public override void Init()
    {
    }

    public override void ManagerUpdate()
    {
    }

    public GameObject Instantiate(string name)
    {
        GameObject result = null;

        GameObject origin = Resources.Load<GameObject>(name);

        if (origin == null)
        {
            Debug.LogWarning($"{name}에 해당하는 리소스가 없습니다.");
            return null;
        }
        Poolable pool = null;
        // 풀링 가능한 오브젝트라면 풀링된 오브젝트를 찾습니다.
        if ((pool = origin.GetComponent<Poolable>()) != null)
        {
            if (!_pool.ContainsKey(origin.name))
                _pool.Add(origin.name, new List<Poolable>());
            if (_pool[origin.name] != null)
            {
                foreach (var p in _pool[origin.name])
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

        // 풀링 가능한 오브젝트가 없다면 새로 만들어줍니다.
        if (result == null)
        {
            result = Object.Instantiate(origin);
            // 풀링 가능하다면 풀링해줍니다.
            if (pool != null)
            {
                if (_pool[origin.name] == null)
                    _pool[origin.name] = new List<Poolable>();

                _pool[origin.name].Add(result.GetComponent<Poolable>());
                _pool[origin.name][_pool[origin.name].Count - 1].IsUsed = true;
            }
        }

        result.gameObject.SetActive(true);

        return result;
    }

    public T Instantiate<T>(string path) where T : MonoBehaviour
    {
        T result = null;

        T origin = Resources.Load<T>(path);

        if (origin == null)
        {
            Debug.LogWarning($"{path}에 해당하는 리소스가 없습니다.");
            return null;
        }
        Poolable pool = null;
        // 풀링 가능한 오브젝트라면 풀링된 오브젝트를 찾습니다.
        if ((pool = origin.GetComponent<Poolable>()) != null )
        {
            if (!_pool.ContainsKey(origin.name))
                _pool.Add(origin.name, new List<Poolable>());
            if(_pool[origin.name] != null)
            {
                foreach (var p in _pool[origin.name])
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

        // 풀링 가능한 오브젝트가 없다면 새로 만들어줍니다.
        if (result == null)
        {
            result = Object.Instantiate<T>(origin);
            // 풀링 가능하다면 풀링해줍니다.
            if (pool != null)
            {
                if (_pool[origin.name] == null)
                    _pool[origin.name] = new List<Poolable>();

                _pool[origin.name].Add(result.GetComponent<Poolable>());
                _pool[origin.name][_pool[origin.name].Count - 1].IsUsed = true;
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
            Debug.LogWarning($"해당하는 리소스가 없습니다.");
            return null;
        }
        Poolable pool = null;
        // 풀링 가능한 오브젝트라면 풀링된 오브젝트를 찾습니다.
        if ((pool = origin.GetComponent<Poolable>()) != null)
        {
            if (!_pool.ContainsKey(origin.name))
                _pool.Add(origin.name, new List<Poolable>());
            if (_pool[origin.name] != null)
            {
                foreach (var p in _pool[origin.name])
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

        // 풀링 가능한 오브젝트가 없다면 새로 만들어줍니다.
        if (result == null)
        {
            result = Object.Instantiate<T>(origin);
            // 풀링 가능하다면 풀링해줍니다.
            if (pool != null)
            {
                if (_pool[origin.name] == null)
                    _pool[origin.name] = new List<Poolable>();

                _pool[origin.name].Add(result.GetComponent<Poolable>());
                _pool[origin.name][_pool[origin.name].Count - 1].IsUsed = true;
            }
        }
        result.gameObject.SetActive(true);

        return result;
    }
    public void Destroy(GameObject gameObject)
    {
        Poolable pool = null;
        if (gameObject)
        {
            if (pool = gameObject.GetComponent<Poolable>())
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
}
