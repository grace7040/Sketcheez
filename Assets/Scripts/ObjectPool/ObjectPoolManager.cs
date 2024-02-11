using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class ObjectPoolManager : Singleton<ObjectPoolManager>
{
    [System.Serializable]
    private class ObjectInfo
    {
        // ������Ʈ �̸�
        public string objectName;
        // ������Ʈ Ǯ���� ������ ������Ʈ
        public GameObject perfab;
        // ��� �̸� ���� �س�������
        public int count;
    }

    string currentColorName = null;
    // ������ƮǮ �Ŵ��� �غ� �Ϸ�ǥ��
    public bool IsReady { get; private set; }

    [SerializeField]
    private ObjectInfo[] objectInfos = null;

    // ������ ������Ʈ�� key�������� ���� ����
    private string objectName;

    // ������ƮǮ���� ������ ��ųʸ�
    private Dictionary<string, IObjectPool<GameObject>> ojbectPoolDic = new Dictionary<string, IObjectPool<GameObject>>();

    // ������ƮǮ���� ������Ʈ�� ���� �����Ҷ� ����� ��ųʸ�
    private Dictionary<string, GameObject> goDic = new Dictionary<string, GameObject>();

    private static ObjectPoolManager instance = null;
    public static ObjectPoolManager Instance
    {
        get
        {
            if (null == instance)
            {
                return null;
            }
            return instance;
        }
    }


    private void Start()
    {
        if (null == instance)
        {
            instance = this;
        }
        else
        {
            Destroy(this.gameObject);
        }
        Init();
    }

    private void Init()
    {
        IsReady = false;

        for (int idx = 0; idx < objectInfos.Length; idx++)
        {
            IObjectPool<GameObject> pool = new ObjectPool<GameObject>(CreatePooledItem, OnTakeFromPool, OnReturnedToPool,
            OnDestroyPoolObject, true, objectInfos[idx].count, objectInfos[idx].count);

            if (goDic.ContainsKey(objectInfos[idx].objectName))
            {
                Debug.Log($"{objectInfos[idx].objectName} �̹� ��ϵ� ������Ʈ�Դϴ�.");
                return;
            }

            goDic.Add(objectInfos[idx].objectName, objectInfos[idx].perfab);
            ojbectPoolDic.Add(objectInfos[idx].objectName, pool);

            // �̸� ������Ʈ ���� �س���
            for (int i = 0; i < objectInfos[idx].count; i++)
            {
                objectName = objectInfos[idx].objectName;
                CreatePooledItem().GetComponent<PoolAble>();
                PoolAble poolAbleGo = CreatePooledItem().GetComponent<PoolAble>();
                poolAbleGo.Pool.Release(poolAbleGo.gameObject);
            }
        }

        Debug.Log("������ƮǮ�� �غ� �Ϸ�");
        IsReady = true;
        ObjectManager.Instance.ObjectPoolManager_Ready = true;
    }

    // ����
    private GameObject CreatePooledItem()
    {
        GameObject poolGo = Instantiate(goDic[objectName], this.transform.position, Quaternion.identity);
        //Debug.Log($"����: {poolGo.name}");
        poolGo.GetComponent<PoolAble>().Pool = ojbectPoolDic[objectName];
        poolGo.transform.SetParent(this.transform);
        return poolGo;
    }

    // �뿩
    private void OnTakeFromPool(GameObject poolGo)
    {
        //Debug.Log($"�뿩: {poolGo.name}");
        poolGo.SetActive(true);
    }

    // ��ȯ
    private void OnReturnedToPool(GameObject poolGo)
    {
        poolGo.SetActive(false);
        //Debug.Log($"��ȯ: {poolGo.name}");
    }

    // ����
    private void OnDestroyPoolObject(GameObject poolGo)
    {
        Destroy(poolGo);
    }

    public GameObject GetGo(string goName)
    {
        objectName = goName;

        if (goDic.ContainsKey(goName) == false)
        {
            Debug.Log($"<{goName}> �� ������ƮǮ�� ��ϵ��� ���� ������Ʈ�Դϴ�.");
            return null;
        }
        //Debug.Log(goName);
        return ojbectPoolDic[goName].Get();
    }
    

    public GameObject GetGo()
    {
        objectName = currentColorName;
        if (goDic.ContainsKey(currentColorName) == false)
        {
            Debug.Log($"<{currentColorName}> �� ������ƮǮ�� ��ϵ��� ���� ������Ʈ�Դϴ�.");
            return null;
        }
        //Debug.Log(currentColorName);
        return ojbectPoolDic[currentColorName].Get();
    }

    public GameObject GetGo(Colors color)
    {
        string name = ColorsToBloodname(color);
        return ojbectPoolDic[name].Get();
    }

    public void SetColorName(Colors color)
    {
        switch (color)
        {
            case Colors.def:
                currentColorName = "DefaultBlood";
                Debug.Log("Blood Effect ���� �����ϼ���.");
                break;
            case Colors.red:
                currentColorName = "RedBlood";
                break;
            case Colors.yellow:
                currentColorName = "YellowBlood";
                break;
            case Colors.blue:
                currentColorName = "BlueBlood";
                break;
            case Colors.orange:
                currentColorName = "OrangeBlood";
                break;
            case Colors.green:
                currentColorName = "GreenBlood";
                break;
            case Colors.purple:
                currentColorName = "PurpleBlood";
                break;
            case Colors.black:
                currentColorName = "BlackBlood";
                break;
        }
    }

    public string ColorsToBloodname(Colors color)
    {
        string colorName = "";

        switch (color)
        {
            case Colors.def:
                colorName = "DefaultBlood";
                break;
            case Colors.red:
                colorName = "RedBlood";
                break;
            case Colors.yellow:
                colorName = "YellowBlood";
                break;
            case Colors.blue:
                colorName = "BlueBlood";
                break;
            case Colors.orange:
                colorName = "OrangeBlood";
                break;
            case Colors.green:
                colorName = "GreenBlood";
                break;
            case Colors.purple:
                colorName = "PurpleBlood";
                break;
            case Colors.black:
                colorName = "BlackBlood";
                break;
        }

        return colorName;
    }
}