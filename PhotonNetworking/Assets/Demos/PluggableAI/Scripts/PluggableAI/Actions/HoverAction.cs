using System;
using UnityEngine;

[CreateAssetMenu(menuName = "PluggableAI/Actions/Hover")]
public class HoverAction : AIAction
{
    [SerializeField]
    private float m_HoverSpeed;

    private Func<float, float, float, float> m_Sine = (a, f, t) => a * Mathf.Sin(f * t);
    private const float m_SineAmplitude = 0.15f;

    public override void Act(AIDataContainer data)
    {
        var controller = data.Controller;
        var position = controller.transform.position;
        var hoverHeight = controller.SpawnHeight + m_Sine(m_SineAmplitude, m_HoverSpeed, controller.TimeSinceAwakening);
        controller.transform.position = new Vector3(position.x, hoverHeight, position.z);
    }
}