using Nekki.Vector.Core.Game;
using Nekki.Vector.Core.Runners;
using UnityEngine;

public class TriggerController : QuadController
{
    protected override void SetSprite()
    {
        base.SetSprite();
        if (Visible && Base is TriggerRunner trigger && (trigger.IsElipsType || trigger.IsCircleType))
        {
            var spriteRender = GetComponent<SpriteRenderer>();
            var material = new Material(Shader.Find("Sprites/Circle-Colored"));
            material.SetVector("_BackgoundColor", Color);

            spriteRender.material = material;
        }
    }
}
