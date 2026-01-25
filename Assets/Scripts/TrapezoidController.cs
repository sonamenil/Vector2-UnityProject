using Nekki.Vector.Core.Runners;
using UnityEngine;

public class TrapezoidController : QuadController
{
    protected override void SetSprite()
    {
        base.SetSprite();
        if (Base is TrapezoidRunner trapezoid)
        {
            var spriteRender = GetComponent<SpriteRenderer>();
            var material = new Material(Shader.Find("Custom/DiagonalClipSprite"));
            material.SetFloat("_Type", trapezoid.Type);

            spriteRender.material = material;
        }
    }
}
