using Nekki.Vector.Core;
using Nekki.Vector.Core.Runners;
using Nekki.Vector.GUI.Scenes.Run;
using System;
using UnityEngine;

public class QuadController : MonoBehaviour
{
    public Action OnBecameVisibleEvent = delegate
    {
    };

    public Action OnBecameInvisibleEvent = delegate
    {
    };

    private bool _IsVisible;

    public bool Visible
    {
        get;
        set;
    } = false;

    private static Sprite whiteSprite;

    public QuadRunner Base
    {
        get;
        set;
    }

    public Color Color { get; set; }

    private void Awake()
    {
        if (whiteSprite == null)
        {
            whiteSprite = Resources.Load<Sprite>("v_back");
        }
    }

    private void Start()
    {
        SetSprite();
    }

    protected virtual void SetSprite()
    {
        var spriteRender = GetComponent<SpriteRenderer>();
        if (spriteRender == null)
        {
            spriteRender = gameObject.AddComponent<SpriteRenderer>();
        }
        spriteRender.sprite = whiteSprite;
        spriteRender.sortingLayerName = "Debug";
        if (Visible)
            spriteRender.color = Color;
        else
            spriteRender.color = new Color(0, 0, 0, 0);
    }

    private void Update()
    {
        if (Base != null && (transform.localScale.x != Base.Rectangle.Size.Width || transform.localScale.y != Base.Rectangle.Size.Height))
        {
            transform.localScale = new Vector3(Base.Rectangle.Size.Width, Base.Rectangle.Size.Height, 1);
        }
        if (!GetComponent<BoxCollider2D>())
        {
            gameObject.AddComponent<BoxCollider2D>();
        }
    }

    private void OnBecameVisible()
    {
        if (RunMainController.IsRunNow && RunMainController.Scene != null && !_IsVisible && Base is TriggerRunner trigger)
        {
            _IsVisible = true;
            RunMainController.Location.TriggersInViewport.Add(trigger);
            OnBecameVisibleEvent?.Invoke();
        }
    }

    private void OnBecameInvisible()
    {
        if (RunMainController.IsRunNow && RunMainController.Scene != null && _IsVisible && Base is TriggerRunner trigger)
        {
            _IsVisible = false;
            RunMainController.Location.TriggersInViewport.Remove(trigger);
            OnBecameInvisibleEvent?.Invoke();
        }
    }

    public void OnMouseDown()
    {
        if (Visible)
            DebugDataView.SetText(Base.ToString());
    }
}
