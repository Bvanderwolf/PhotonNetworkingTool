using UnityEngine;
using UnityEngine.UI;

public class EntityInfo : MonoBehaviour
{
    [System.Serializable]
    private struct HealthSystemInfo
    {
        public GameObject parent;
        public Image healthBar;
        public Text healthText;
    }

    [SerializeField]
    private HealthSystemInfo hitPointInfo;

    [SerializeField]
    private HealthSystemInfo energyInfo;

    [SerializeField]
    private HealthSystemInfo experienceInfo;

    [SerializeField]
    private GameObject identifyIcon;

    private Vector3 heightOffset;

    private Transform entity;

    public void Activate(Transform forEntity)
    {
        entity = forEntity;
        gameObject.SetActive(true);
    }

    private void Update()
    {
        transform.position = Camera.main.WorldToScreenPoint(entity.position + heightOffset);
    }

    public void Return()
    {
        gameObject.transform.localPosition = Vector3.zero;
        gameObject.SetActive(false);
    }

    public void AddHitPointFeedback(IDamageAble damagable)
    {
        if (damagable != null)
        {
            hitPointInfo.parent.SetActive(true);
            damagable.AddDamageFeedback(hitPointInfo.healthBar, hitPointInfo.healthText);
        }
    }

    public void AddEnergyFeedback(IExhaustable exhaustable)
    {
        if (exhaustable != null)
        {
            energyInfo.parent.SetActive(true);
            exhaustable.AddEnergyFeedback(energyInfo.healthBar, energyInfo.healthText);
        }
    }

    public void AddExperienceFeedback(ILearnable learnable)
    {
        if (learnable != null)
        {
            experienceInfo.parent.SetActive(true);
            learnable.AddExperienceFeedback(experienceInfo.healthBar, experienceInfo.healthText);
        }
    }

    public void AddIdentifyAbleFeedback(IIdentifyable identifyable)
    {
        Image image = identifyIcon.GetComponent<Image>();
        image.enabled = true;
        image.sprite = identifyable.IdentifyableIcon;
        heightOffset = identifyable.ElementHeightOffset;
    }
}