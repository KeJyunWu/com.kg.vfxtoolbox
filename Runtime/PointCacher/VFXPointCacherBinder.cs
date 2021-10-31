using System.Collections.Generic;
using UnityEngine.VFX;
using UltraCombos.VFXToolBox;

namespace UnityEngine.VFX.Utility
{
    [AddComponentMenu("VFX/Property Binders/Point Cacher Binder")]
    [VFXBinder("Ultra VFX Tools/Point Cacher Binder")]
    class VFXPointCacherBinder : VFXBinderBase
    {
        public PointCacher PointCacher;
        [VFXPropertyBinding("UnityEngine.Texture2D"), UnityEngine.Serialization.FormerlySerializedAs("PositionMapParameter")]
        public ExposedProperty PositionMapProperty = "PositionMap";
        [VFXPropertyBinding("UnityEngine.Texture2D"), UnityEngine.Serialization.FormerlySerializedAs("NormalMapParameter")]
        public ExposedProperty NormalMapProperty = "NormalMap";
        [VFXPropertyBinding("UnityEngine.Texture2D"), UnityEngine.Serialization.FormerlySerializedAs("VelocityMapParameter")]
        public ExposedProperty VelocityMapProperty = "VelocityMap";
        [VFXPropertyBinding("UnityEngine.Texture2D"), UnityEngine.Serialization.FormerlySerializedAs("UVMapParameter")]
        public ExposedProperty UVMapProperty = "UVMap";

        protected override void OnEnable()
        {
            base.OnEnable();
        }

        public override bool IsValid(VisualEffect component)
        {
            return PointCacher != null &&
                component.HasTexture(PositionMapProperty) &&
                component.HasTexture(NormalMapProperty) &&
                component.HasTexture(VelocityMapProperty) &&
                component.HasTexture(UVMapProperty);
        }

        public override void UpdateBinding(VisualEffect component)
        {
            if (Application.isPlaying)
            {
                component.SetTexture(PositionMapProperty, PointCacher.PositionMap);
                component.SetTexture(NormalMapProperty, PointCacher.NormalMap);
                component.SetTexture(VelocityMapProperty, PointCacher.VelocityMap);
                component.SetTexture(UVMapProperty, PointCacher.UVMap);
            }
        }

        public override string ToString()
        {
            return string.Format("PointCacher is null");
        }
    }
}

