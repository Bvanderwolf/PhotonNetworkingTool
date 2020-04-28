using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

[RequireComponent(typeof(NavMeshAgent))]
public class AIStateController : MonoBehaviour, IDamageAble, IExhaustable, ILearnable, IIdentifyable
{
    [Header("AI Settings")]
    [SerializeField]
    private AIState currentState;

    [Header("Identification Settings")]
    [SerializeField]
    private Sprite identifyableIcon;

    [SerializeField]
    private Vector3 IdentifyElementHeightOffset;

    [Header("Health Systems Settings")]
    [SerializeField]
    private HealthSystem hitPointSystem;

    [SerializeField]
    private HealthSystem energySystem;

    [SerializeField]
    private HealthSystem experienceSystem;

    public NavMeshAgent Agent { get; private set; }
    public Animator Animator { get; private set; }

    private AIStateData data;

    public Sprite IdentifyableIcon
    {
        get
        {
            return identifyableIcon;
        }
    }

    public Vector3 ElementHeightOffset
    {
        get
        {
            return IdentifyElementHeightOffset;
        }
    }

    private void Awake()
    {
        data = new AIStateData(this);
        Agent = GetComponent<NavMeshAgent>();
        Animator = GetComponent<Animator>();
        currentState.StartState(this);
    }

    private void Start()
    {
        AIService.Instance.Subscribe(transform);
    }

    private void OnDestroy()
    {
        AIService.Instance.UnSubscribe(transform);
    }

    public void Damage(HealthModifier modifier)
    {
        print("damaged " + this.name);
    }

    public void AddDamageFeedback(Image fillableImage, Text hitpointText = null)
    {
        hitPointSystem.AttachHealthBar(fillableImage);
        hitPointSystem.AttachHealthText(hitpointText);
    }

    public void AddEnergyFeedback(Image fillableImage, Text energyText = null)
    {
        energySystem.AttachHealthBar(fillableImage);
        energySystem.AttachHealthText(energyText);
    }

    public void AddExperienceFeedback(Image fillableImage, Text experienceText = null)
    {
        experienceSystem.AttachHealthBar(fillableImage);
        experienceSystem.AttachHealthText(experienceText);
    }

    private void Update()
    {
        if (currentState != null)
        {
            currentState.UpdateState(this);
        }
    }

    public AIDataContainer GetData(AIStateDataType _type)
    {
        return data.GetData(_type);
    }

    public void Transition(AIState nextState)
    {
        if (nextState != currentState)
        {
            OnStateEnd(currentState);
            currentState = nextState;
            OnStateStart(currentState);
        }
    }

    private void OnStateStart(AIState state)
    {
        state.StartState(this);
    }

    private void OnStateEnd(AIState state)
    {
        state.EndState(this);
    }
}