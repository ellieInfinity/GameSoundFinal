using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HorfScript : MonoBehaviour
{
    [SerializeField] private float _shakeSpeed;
    [SerializeField] private float _shakeMagnitude;
    [SerializeField] private SpriteRenderer _head;
    [SerializeField] private GameObject _intenseTrigger;

    private float _startOffset;
    private float _timer;

    private int hp = 5;
    
    private FMOD.Studio.EventInstance _dieSound;
    private FMOD.Studio.EventInstance _hurtSound;

    void Start()
    {
        _startOffset = _head.transform.localPosition.x;
        _dieSound = FMODUnity.RuntimeManager.CreateInstance("event:/enemy_die");
        _hurtSound = FMODUnity.RuntimeManager.CreateInstance("event:/enemy_hurt");
    }
    
    private void Update()
    {
        _timer += Time.deltaTime;
        Vector3 pos = _head.transform.localPosition;

        pos.x = _startOffset + (Mathf.Sin((_timer * _shakeSpeed)) * _shakeMagnitude);

        _head.transform.localPosition = pos;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Bullet"))
        {
            Destroy(other.gameObject);
            GetHurt();
        }
    }

    public void GetHurt()
    {
        hp -= 1;

        if (hp <= 0)
        {
            _dieSound.start();
            _intenseTrigger.gameObject.SetActive(false);
            Destroy(gameObject);
            return;
        }
        else
        {
            _hurtSound.start();
        }
        
        StartCoroutine(GetHurtFlash());
    }

    private IEnumerator GetHurtFlash()
    {
        float time = 0;
        float duration = 0.1f;

        while (time < duration)
        {
            time += Time.unscaledDeltaTime;
            _head.color = Color.Lerp(Color.red, Color.white, time / duration);

            yield return null;
        }
        
        _head.color = Color.white;
    }
}
