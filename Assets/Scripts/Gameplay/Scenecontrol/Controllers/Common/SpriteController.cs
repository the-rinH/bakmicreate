using MoonSharp.Interpreter;
using UnityEngine;

namespace ArcCreate.Gameplay.Scenecontrol
{
    [MoonSharpUserData]
    public class SpriteController : Controller, IPositionController, ILayerController, ITextureController, IColorController
    {
        private static readonly int TextureModifyShaderId = Shader.PropertyToID("_Modify");
        private MaterialPropertyBlock mpb;
        [SerializeField] private SpriteRenderer spriteRenderer;

        public Vector3 DefaultTranslation { get; private set; }

        public Quaternion DefaultRotation { get; private set; }

        public Vector3 DefaultScale { get; private set; }

        public string DefaultLayer { get; private set; }

        public int DefaultSort { get; private set; }

        public float DefaultAlpha { get; private set; }

        public Color DefaultColor { get; private set; }

        public Vector2 DefaultTextureOffset { get; private set; }

        public Vector2 DefaultTextureScale { get; private set; }

        public ValueChannel TranslationX { get; set; }

        public ValueChannel TranslationY { get; set; }

        public ValueChannel TranslationZ { get; set; }

        public ValueChannel RotationX { get; set; }

        public ValueChannel RotationY { get; set; }

        public ValueChannel RotationZ { get; set; }

        public ValueChannel ScaleX { get; set; }

        public ValueChannel ScaleY { get; set; }

        public ValueChannel ScaleZ { get; set; }

        public StringChannel Layer { get; set; }

        public ValueChannel Sort { get; set; }

        public ValueChannel Alpha { get; set; }

        public ValueChannel ColorR { get; set; }

        public ValueChannel ColorG { get; set; }

        public ValueChannel ColorB { get; set; }

        public ValueChannel ColorH { get; set; }

        public ValueChannel ColorS { get; set; }

        public ValueChannel ColorV { get; set; }

        public ValueChannel ColorA { get; set; }

        public ValueChannel TextureOffsetX { get; set; }

        public ValueChannel TextureOffsetY { get; set; }

        public ValueChannel TextureScaleX { get; set; }

        public ValueChannel TextureScaleY { get; set; }

        public SpriteRenderer SpriteRenderer => spriteRenderer;

        public override void SetupDefault()
        {
            DefaultColor = spriteRenderer.color;
            DefaultTranslation = transform.localPosition;
            DefaultRotation = transform.localRotation;
            DefaultScale = transform.localScale;
            DefaultLayer = spriteRenderer.sortingLayerName;
            DefaultSort = spriteRenderer.sortingOrder;
            DefaultAlpha = spriteRenderer.color.a;

            Vector4 defaultModifyVec = spriteRenderer.material.GetVector(TextureModifyShaderId);
            DefaultTextureOffset = new Vector2(defaultModifyVec.x, defaultModifyVec.y);
            DefaultTextureScale = new Vector2(defaultModifyVec.z, defaultModifyVec.w);

            mpb = mpb ?? new MaterialPropertyBlock();
        }

        public virtual SpriteController Copy()
        {
            var c = Instantiate(gameObject, transform.parent).GetComponent<SpriteController>();
            c.spriteRenderer.material = Instantiate(c.spriteRenderer.material);
            c.IsPersistent = false;

            Controller[] children = GetChildren();
            int i = 0;
            foreach (Controller child in c.GetChildren())
            {
                child.IsPersistent = false;
                child.Start();
                child.CopyAllChannelsFrom(children[i]);
                i++;
            }

            c.Start();
            c.CopyAllChannelsFrom(this);
            Services.Scenecontrol.AddReferencedController(c);
            return c;
        }

        public virtual void UpdateColor(Color color)
        {
            spriteRenderer.color = color;
        }

        public void UpdateLayer(string layer, int sort = 0, float alpha = 255f)
        {
            spriteRenderer.sortingLayerName = layer;
            spriteRenderer.sortingOrder = sort;
        }

        public void UpdatePosition(Vector3 translation, Quaternion rotation, Vector3 scale)
        {
            transform.localScale = scale;
            transform.localRotation = rotation;
            transform.localPosition = translation;
        }

        public void UpdateTexture(Vector2 offset, Vector2 scale)
        {
            Vector4 modifyVec = new Vector4(offset.x, offset.y, scale.x, scale.y);
            spriteRenderer.GetPropertyBlock(mpb);
            mpb.SetVector(TextureModifyShaderId, modifyVec);
            spriteRenderer.SetPropertyBlock(mpb);
        }
    }
}