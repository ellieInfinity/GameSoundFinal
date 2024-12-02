using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Isaac : MonoBehaviour
{
    [SerializeField] private float _topSpeed;
    [SerializeField] private float _accelSpeed;
    [SerializeField] private float _deccelSpeed;
    [Space]
    [SerializeField] private float _fireRate;
    [SerializeField] private GameObject _bulletPrefab;
    [SerializeField] private GameObject _intenseTrigger;
    
    private BoxCollider2D _collider;
    private Rigidbody2D _rb;
    private Animator _anim;

    [SerializeField] private Vector3 _vel;
    private float _fireTimer;
    private Vector2 _currentFireDir = new Vector2();
    private bool _hasFired;
    private bool _leftEye;
    private bool _scrollingRooms;
    
    private FMOD.Studio.EventInstance _footSound;
    private FMOD.Studio.EventInstance _woosh;
    private FMOD.Studio.EventInstance _wallHit;
    private FMOD.Studio.EventInstance _spawnTear;

    private void Awake()
    {
        Application.targetFrameRate = -1;
        _collider = GetComponent<BoxCollider2D>();
        _rb = GetComponent<Rigidbody2D>();
        _anim = GetComponent<Animator>();
        
        _footSound = FMODUnity.RuntimeManager.CreateInstance("event:/foot");
        _woosh = FMODUnity.RuntimeManager.CreateInstance("event:/woosh");
        _wallHit = FMODUnity.RuntimeManager.CreateInstance("event:/wallHit");
        _spawnTear = FMODUnity.RuntimeManager.CreateInstance("event:/tear_spawn");
    }

    public void PlayFootSound()
    {
        _footSound.start();
    }
    public void PlayWoosh()
    {
        _woosh.start();
    }
    public void PlayWallHit()
    {
        _wallHit.start();
    }

    private void Update()
    {
        if (_scrollingRooms)
        {
            _hasFired = false;
            return;
        }
        
        HandleMovement();
        HandleTears();
        HandleAnimation();

        _hasFired = false;
    }

    private void HandleMovement()
    {
        float x = 0;
        float y = 0;

        if (Input.GetKey(KeyCode.A)) x -= 1;
        if (Input.GetKey(KeyCode.D)) x += 1;
        
        if (Input.GetKey(KeyCode.S)) y -= 1;
        if (Input.GetKey(KeyCode.W)) y += 1;
        
        Vector2 moveVector = new Vector2(x, y).normalized;

        if (moveVector.x != 0)
        {
            float tempTopSpeed = _topSpeed * (Mathf.Abs(moveVector.x));

            if (_vel.x > tempTopSpeed)
            {
                _vel.x = Mathf.Max(_vel.x - (_deccelSpeed * Time.deltaTime), tempTopSpeed);
            } 
            else if (_vel.x < -tempTopSpeed)
            {
                _vel.x = Mathf.Min(_vel.x + (_deccelSpeed * Time.deltaTime), -tempTopSpeed);
            }
            else
            {
                float mult = 1;

                if ((moveVector.x > 0 && _vel.x < 0) || moveVector.x < 0 && _vel.x > 0) mult = 3;
                
                _vel.x += _accelSpeed * mult * Mathf.Round(moveVector.x) * Time.deltaTime;
                _vel.x = Mathf.Clamp(_vel.x, -tempTopSpeed, tempTopSpeed);
            }
        }
        else
        {
            if (_vel.x > 0.1)
                _vel.x = Mathf.Max(_vel.x - _deccelSpeed * Time.deltaTime, 0);
            else if (_vel.x < -0.1)
                _vel.x = Mathf.Min(_vel.x + _deccelSpeed * Time.deltaTime, 0);
            else 
                _vel.x = 0;
        }

        if (moveVector.y != 0)
        {
            float tempTopSpeed = _topSpeed * (Mathf.Abs(moveVector.y));
            
            if (_vel.y > tempTopSpeed)
            {
                _vel.y = Mathf.Max(_vel.y - (_deccelSpeed * Time.deltaTime), tempTopSpeed);
            } 
            else if (_vel.y < -tempTopSpeed)
            {
                _vel.y = Mathf.Min(_vel.y + (_deccelSpeed * Time.deltaTime), -tempTopSpeed);
            }
            else
            {
                float mult = 1;

                if ((moveVector.y > 0 && _vel.y < 0) || moveVector.y < 0 && _vel.y > 0) mult = 3;
                
                _vel.y += _accelSpeed * mult * Mathf.Round(moveVector.y) * Time.deltaTime;
                _vel.y = Mathf.Clamp(_vel.y, -tempTopSpeed, tempTopSpeed);
            }
        }
        else
        {
            if (_vel.y > 0.1)
                _vel.y = Mathf.Max(_vel.y - _deccelSpeed * Time.deltaTime, 0);
            else if (_vel.y < -0.1)
                _vel.y = Mathf.Min(_vel.y + _deccelSpeed * Time.deltaTime, 0);
            else 
                _vel.y = 0;
        }

        _rb.velocity = _vel;
    }

    private void HandleTears()
    {
        bool[][] fireVector = new bool[2][];
        fireVector[0] = new bool[2];
        fireVector[1] = new bool[2];
        
        if (Input.GetKey(KeyCode.LeftArrow)) fireVector[0][0] = true;
        if (Input.GetKey(KeyCode.RightArrow)) fireVector[0][1] = true;
        
        if (Input.GetKey(KeyCode.DownArrow)) fireVector[1][0] = true;
        if (Input.GetKey(KeyCode.UpArrow)) fireVector[1][1] = true;

        if (_currentFireDir.x < 0)
        {
            if (Input.GetKeyDown(KeyCode.RightArrow)) _currentFireDir.x = 1;
            else if (!fireVector[0][0]) _currentFireDir.x = 0;
        } 
        else if (_currentFireDir.x > 0)
        {
            if (Input.GetKeyDown(KeyCode.LeftArrow)) _currentFireDir.x = -1;
            else if (!fireVector[0][1]) _currentFireDir.x = 0;
        }
        else
        {
            if (fireVector[0][0]) _currentFireDir.x = -1;
            else if (fireVector[0][1]) _currentFireDir.x = 1;
        }
            
        if (_currentFireDir.y < 0)
        {
            if (Input.GetKeyDown(KeyCode.UpArrow)) _currentFireDir.y = 1;
            else if (!fireVector[1][0]) _currentFireDir.y = 0;
        } 
        else if (_currentFireDir.y > 0)
        {
            if (Input.GetKeyDown(KeyCode.DownArrow)) _currentFireDir.y = -1;
            else if (!fireVector[1][1]) _currentFireDir.y = 0;
        }
        else
        {
            if (fireVector[1][0]) _currentFireDir.y = -1;
            else if (fireVector[1][1]) _currentFireDir.y = 1;
        }
        
        _fireTimer -= Time.deltaTime;

        if (_currentFireDir != Vector2.zero)
        {
            if (_fireTimer <= 0)
            {
                _hasFired = true;
                _fireTimer = _fireRate;
                Vector2 dir = _currentFireDir;
                if (dir.y != 0) dir.x = 0;

                _spawnTear.start();
                Bullet obj = Instantiate(_bulletPrefab).GetComponent<Bullet>();
                Vector3 pos = transform.position;
                int offset = _leftEye ? -3 : 3;

                if (dir.y != 0)
                {
                    pos.x += offset;
                }
                else
                {
                    pos.y += offset;
                }

                obj.transform.position = pos;
                _leftEye = !_leftEye;

                if (dir.y != 0)
                {
                    dir.x = _vel.x / obj._speed / 2;
                }
                else if (dir.x != 0)
                {
                    dir.y = _vel.y / obj._speed / 2;
                }
                
                obj._direction = dir;
            }
        }
    }

    private void HandleAnimation()
    {
        if (_vel.magnitude < 0.1f) _anim.SetBool("Moving", false);
        else _anim.SetBool("Moving", true);
        
        _anim.SetFloat("xMovement", _vel.x);
        _anim.SetFloat("yMovement", _vel.y);
        
        _anim.SetFloat("xFire", _currentFireDir.x);
        _anim.SetFloat("yFire", _currentFireDir.y);

        if (_currentFireDir.magnitude > 0.1)
        {
            _anim.SetLayerWeight(1, 1);

            if (_hasFired) _anim.Play("Fire", 1, 0);
        }
        else
        {
            _anim.SetLayerWeight(1, 0);
        }
    }

    private IEnumerator RoomTransition(Transform target)
    {
        _scrollingRooms = true;
        _intenseTrigger.gameObject.SetActive(true);
        Time.timeScale = 0;

        GameObject cam = Camera.main.gameObject;

        Vector3 startPos = cam.transform.position;
        Vector3 endPos = target.parent.parent.position;
        endPos.z = startPos.z;
    
        yield return new WaitForSecondsRealtime(0.1f);
        transform.position = target.position;

        float time = 0;
        float duration = 0.2f;

        while (time < duration)
        {
            time += Time.unscaledDeltaTime;
            cam.transform.position = Vector3.Slerp(startPos, endPos, time / duration);

            yield return null;
        }

        cam.transform.position = endPos;
        Time.timeScale = 1;
        _scrollingRooms = false;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("a");
        if (other.CompareTag("Door"))
        {
            _vel = Vector3.zero;
            PlayWoosh();
            StartCoroutine(RoomTransition(other.GetComponent<DoorController>().LinkedDoor));
        }
        if (other.gameObject.tag.Equals("Wall"))
        {
            Debug.Log("aa");
            PlayWallHit();
            Debug.Log("a");
        }
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.tag.Equals("Wall"))
        {
            Debug.Log("aa");
            PlayWallHit();
            Debug.Log("a");
        }
    }
}
