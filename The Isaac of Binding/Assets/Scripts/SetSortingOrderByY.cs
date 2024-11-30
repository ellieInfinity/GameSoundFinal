using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SetSortingOrderByY : MonoBehaviour
{
    [SerializeField] private float _offset;
    [SerializeField] private SpriteRenderer[] _sprites;

    private void Awake()
    {
        _sprites = GetComponentsInChildren<SpriteRenderer>(true).ToArray();
    }

    private void LateUpdate()
    {
        foreach (SpriteRenderer spr in _sprites)
        {
            spr.sortingOrder = (int) ((transform.position.y * 100) + _offset) * -1;
        }
    }
}
