using UnityEngine;
using UnityEngine.Events;
using Sirenix.OdinInspector;
using System.Linq;

namespace UltraCombos.VFXToolBox
{
    [ExecuteInEditMode]
    public class GradientTexture : GTBase
    {
        [System.Serializable]
        public class MaterialApply
        {
            public Material m_material;
            public string m_propertyName;
        }

        [SerializeField] Gradient m_gradient;
        public Gradient Gradient { get { return m_gradient; } set { m_gradient = value; } }

        [SerializeField, ReadOnly]
        Texture2D m_result;
        public Texture2D Result { get { return m_result; } }

        [TitleGroup("Material Apply")]
        [SerializeField] MaterialApply[] m_materials;
        public MaterialApply[] Materials { get { return m_materials; } set { m_materials = value; } }

        [Space]
        [TitleGroup("Event")]
        public UnityEvent<Texture2D> OnEvent = new UnityEvent<Texture2D>();

        void InternalUpdate()
        {
            UpdateTexture(ref m_result, m_gradient);
            OnEvent?.Invoke(m_result);

            var _filter =
            from _source in m_materials
            where _source.m_material != null && _source.m_material.HasProperty(_source.m_propertyName)
            select _source;

            foreach(MaterialApply _apply in _filter)
            {
                _apply.m_material.SetTexture(_apply.m_propertyName, m_result);
            }
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