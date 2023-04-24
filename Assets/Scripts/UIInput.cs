using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIInput : UIBehaviour
{
    public event System.Action<float> onValueChanged;

    [SerializeField]
    Text m_LabelText;

    [SerializeField]
    Slider m_ValueSlider;

    [SerializeField]
    InputField m_ValueInputField;

    [SerializeField]
    string m_Label;
    public string label { get => m_Label; set => Set(ref m_Label, value); }

    [SerializeField]
    float m_Value = 1;
    public float value { get => m_Value; set => Set(ref m_Value, Step(value), onValueChanged); }

    [SerializeField]
    float m_Min = 0;
    public float min { get => m_Min; set => Set(ref m_Min, value); }

    [SerializeField]
    float m_Max = 1;
    public float max { get => m_Max; set => Set(ref m_Max, value); }

    [SerializeField]
    float m_Step = 0;
    public float step { get => m_Step; set => m_Step = value; }

    [SerializeField]
    bool m_WholeNumbers = false;
    public bool wholeNumbers { get => m_ValueSlider.wholeNumbers; set => m_ValueSlider.wholeNumbers = value; }

    private void OnEnable()
    {
        m_ValueSlider.onValueChanged.AddListener(OnSliderValueChanged);
        m_ValueInputField.onValueChanged.AddListener(OnInputFieldValueChanged);
    }

    private void OnDisable()
    {
        m_ValueSlider.onValueChanged.RemoveListener(OnSliderValueChanged);
        m_ValueInputField.onValueChanged.RemoveListener(OnInputFieldValueChanged);
    }

    protected override void Render()
    {
        m_LabelText.text = m_Label;
        m_ValueSlider.minValue = m_Min;
        m_ValueSlider.maxValue = m_Max;
        m_ValueSlider.value = m_Value;
        m_ValueInputField.text = m_Value.ToString(m_Step == 1f ? "0" : "0.00");
    }

    float Step(float value)
    {
       return m_Step > 0f ? Mathf.FloorToInt(value / m_Step) * m_Step : value;
    }

    void OnSliderValueChanged(float val)
    {
        value = val;
    }

    void OnInputFieldValueChanged(string val)
    {
        if (float.TryParse(val, out var res))
        {
            value = res;
        }
    }
}
