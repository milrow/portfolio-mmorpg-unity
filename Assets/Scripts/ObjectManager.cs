using System.Collections.Generic;
using Unity.Cinemachine;
using UnityEngine;

public class ObjectManager : MonoBehaviour
{
    public static ObjectManager Instance { get; private set; }

    public GameObject PlayerPrefab;
    public GameObject RemotePlayerPrefab;
    public CinemachineCamera ThirdPersonCamera;

    private uint sessionId = 0;
    Dictionary<uint, GameObject> objectPool = new Dictionary<uint, GameObject>();

    private void Awake()
    {
        Instance = this;
        DontDestroyOnLoad(gameObject);

        Application.runInBackground = true;
        // 프레임 제한을 풀거나 충분히 높게 설정 (동기화 확인용)
        Application.targetFrameRate = 60;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    public GameObject Spawn(uint sessionId, Vector3 spawnPos, bool isMyPlayer)
    {
        GameObject prefab = isMyPlayer ? PlayerPrefab : RemotePlayerPrefab;
        GameObject obj = Instantiate(prefab, spawnPos, Quaternion.identity);

        objectPool.Add(sessionId, obj);

        if(isMyPlayer )
        {
            ThirdPersonCamera.Target.TrackingTarget = obj.transform;
        }

        return obj;
    }

    public void DeSpawn(uint sessionId)
    {
        if (objectPool.TryGetValue(sessionId, out GameObject obj))
        {
            Destroy(obj);

            objectPool.Remove(sessionId);
        }
    }

    public void Add(uint sessionId, GameObject gameObject)
    {
        if(objectPool.ContainsKey(sessionId))
        {
            return;
        }
        objectPool.Add(sessionId, gameObject);
    }

    public void Remove(uint sessionId)
    {
        if(objectPool.ContainsKey(sessionId))
        {
            objectPool.Remove(sessionId);
            return;
        }
        return;
    }

    public GameObject Fine(uint sessionId)
    {
        GameObject obj;
        objectPool.TryGetValue(sessionId, out obj);
        return obj;


    }

    public void SetSessionId(uint id) { sessionId = id; }
}
