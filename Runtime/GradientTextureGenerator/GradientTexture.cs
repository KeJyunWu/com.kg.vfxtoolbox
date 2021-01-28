using UnityEngine;
using UnityEngine.Events;
using Sirenix.OdinInspector;

namespace UltraCombos.VFXToolBox
{
    [ExecuteInEditMode]
    public class GradientTexture : GTBase
    {
        [SerializeField] Gradient m_gradient;
        public Gradient Gradient { get { return m_gradient; } set { m_gradient = value; } }

        [SerializeField, ReadOnly]
        Texture2D m_result;
        public Texture2D Result { get { return m_result; } }

        public UnityEvent<Texture2D> OnEvent = new UnityEvent<Texture2D>();

        void InternalUpdate()
        {
            UpdateTexture(ref m_result, m_gradient);
            OnEvent?.Invoke(m_result);
        }

        private void OnValidate()
        {
            InternalUpdate();
        }

        void Update()
        {
            InternalUpdate();
        }
    }
}