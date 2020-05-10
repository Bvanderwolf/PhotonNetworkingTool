using UnityEngine;
using UnityEngine.UI;

public class SimpleHealthSystemImplementer : MonoBehaviour
{
    [SerializeField]
    private HealthSystem healthSystem;

    [SerializeField]
    private InputField healthInput;

    [SerializeField]
    private InputField timeInput;

    [SerializeField]
    private Toggle regenToggle;

    [SerializeField]
    private Toggle currentToggle;

    [SerializeField]
    private Toggle canStackToggle;

    [SerializeField]
    private TimedHealthModifier modifier;

    [SerializeField]
    private ConditionalHealthModifier conditional;

    [SerializeField]
    private bool useTimed = true;

    [SerializeField]
    private bool useUIInputs = false;

    private void Awake()
    {
        this.healthSystem.SetCurrentToMax();
        this.healthSystem.OnRegenStop += OnRegenStopped;

        conditional.SetStopCondition(() => healthSystem.Current == 0);
    }

    private void Update()
    {
        this.healthSystem.Update();
    }

    public void OnModifyHealthButtonClick()
    {
        if (string.IsNullOrEmpty(healthInput.text) || string.IsNullOrEmpty(timeInput.text))
            return;

        int value = int.Parse(healthInput.text);
        float time = float.Parse(timeInput.text);

        if (useUIInputs)
        {
            healthSystem.AddModifier(new TimedHealthModifier("modifier", time, value, regenToggle.isOn, currentToggle.isOn, canStackToggle.isOn));
        }
        else
        {
            if (useTimed)
            {
                healthSystem.AddModifier(modifier.Clone);
            }
            else
            {
                healthSystem.AddModifier(conditional.Clone);
            }
        }
    }

    public void OnRegenStopped()
    {
        if (healthSystem.Current != 0)
        {
            healthSystem.AddModifier(conditional.Clone);
        }
    }
}