using UnityEngine;

/// <summary>
/// Script to be attached to any gameobject that needs to have its collider automatically scaled with
/// sprite size.
/// </summary>
[ExecuteInEditMode]
[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(Collider2D))]
public class ValidTest : MonoBehaviour
{
    private SpriteRenderer _renderer;
    private BoxCollider2D _collider;

    private void Start()
    {
        _renderer = GetComponent<SpriteRenderer>();
        _collider = GetComponent<BoxCollider2D>();
    }

    private void Update()
    {
        _collider.size = new Vector2(Mathf.Abs(_renderer.size.x), Mathf.Abs(_renderer.size.y));
    }
}