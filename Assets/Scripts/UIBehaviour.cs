using UnityEngine;

public class UIBehaviour : MonoBehaviour
{
    bool m_IsDirty;

    protected virtual void LateUpdate()
    {
        CheckDirty();
    }

#if UNITY_EDITOR
    // インスペクタで値を変えた時用
    protected virtual void OnValidate()
    {
        m_IsDirty = true;
        CheckDirty();
    }
#endif

    protected virtual void Render()
    {
    }

    protected void Set<T>(ref T prop, T value, System.Action<T> callback = null)
    {
        if (!value.Equals(prop))
        {
            prop = value;
            m_IsDirty = true;
            callback?.Invoke(value);

#if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                CheckDirty();
            }
#endif
        }
    }

    protected void CheckDirty()
    {
        if (m_IsDirty)
        {
            Render();
            m_IsDirty = false;
        }
    }
}
