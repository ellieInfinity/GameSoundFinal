using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class Bullet : MonoBehaviour
{
    [SerializeField] private GameObject _particle;
    
    [HideInInspector] public Vector3 _direction;
    public float _speed = 50f;

    private FMOD.Studio.EventInstance _dieSound;

    private void Awake()
    {
        _dieSound = FMODUnity.RuntimeManager.CreateInstance("event:/tear_fire");
    }

    private void Update()
    {
        transform.position += _direction * (_speed * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Wall"))
        {
            Destroy(gameObject);
        }
    }

    private void OnDestroy()
    {
        _dieSound.start();
        
        GameObject obj = Instantiate(_particle);
        obj.transform.position = transform.GetChild(0).position;
        obj.GetComponent<SpriteRenderer>().sortingOrder = GetComponentInChildren<SpriteRenderer>().sortingOrder;

        float rot = 0;
        switch (Random.Range(1, 4))
        {
            case 1:
                rot = 90;
                break;
            case 2:
                rot = 180;
                break;
            case 3:
                rot = 270;
                break;
        }
        
        obj.transform.eulerAngles = new Vector3(0, 0, rot);
        if (Random.Range(1, 100) % 2 == 0) obj.transform.localScale = new Vector3(-1, 1, 1);
    }
}
