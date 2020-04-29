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

    [Header("Veterancy System Settings")]
    [SerializeField]
    private VeterancySystem veterancySystem;

    public NavMeshAgent Agent { get; private set; }
    public Animator Animator { get; private set; }

    public AIStateData Data;

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
        Data = new AIStateData();
        Agent = GetComponent<NavMeshAgent>();
        Animator = GetComponent<Animator>();

        hitPointSystem.SetCurrentToMax();
        experienceSystem.OnReachedMax += OnLevelUp;
        energySystem.SetCurrentToMax();
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
        hitPointSystem.AddModifier(modifier);
    }

    public void Learn(int value)
    {
        experienceSystem.AddModifier(new TimedHealthModifier(0, value, true, true));
    }

    public void OnLevelUp()
    {
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

        hitPointSystem.Update();
        energySystem.Update();
        experienceSystem.Update();
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