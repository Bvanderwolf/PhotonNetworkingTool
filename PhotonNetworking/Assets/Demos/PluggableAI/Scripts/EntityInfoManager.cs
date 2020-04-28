using System.Collections.Generic;
using System.Collections;
using System.Linq;
using UnityEngine;

public class EntityInfoManager : MonoBehaviour
{
    [SerializeField]
    private GameObject entityInfoOriginal;

    private List<EntityInfo> entityInfoPool = new List<EntityInfo>();

    private void Awake()
    {
        entityInfoPool.Add(entityInfoOriginal.GetComponent<EntityInfo>());
    }

    private void Start()
    {
        foreach (Transform ai in AIService.Instance.AI)
        {
            EntityInfo info = GetPoolable(ai);
            AIStateController controller = ai.GetComponent<AIStateController>();
            info.AddHitPointFeedback(controller);
            info.AddEnergyFeedback(controller);
            info.AddExperienceFeedback(controller);
            info.AddIdentifyAbleFeedback(controller);
        }
    }

    private EntityInfo GetPoolable(Transform forEntity)
    {
        EntityInfo info = entityInfoPool.Where(e => !e.gameObject.activeInHierarchy).FirstOrDefault();
        if (info == null)
        {
            info = Instantiate(entityInfoOriginal, transform).GetComponent<EntityInfo>();
            entityInfoPool.Add(info);
        }

        info.Activate(forEntity);
        return info;
    }
}