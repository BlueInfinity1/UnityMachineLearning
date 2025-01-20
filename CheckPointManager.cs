using UnityEngine;

public class CheckPointManager : MonoBehaviour
{
    public static CheckPointManager Instance { get; private set; }

    public int totalCheckPoints { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    void Start()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            var child = transform.GetChild(i);
            var checkPoint = child.GetComponent<CheckPoint>();
            if (checkPoint != null)
            {
                checkPoint.checkPointIndex = i;
            }
        }

        totalCheckPoints = transform.childCount;
    }
}
