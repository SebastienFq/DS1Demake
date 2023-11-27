using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [SerializeField] private Transform _player;
    [SerializeField] private float _oobYPos = -50;
    [SerializeField] private Animation _youDiedAnimation;
    [SerializeField] private Volume _effectVolume;

    private IEnumerator deathRoutine;
    private bool isDead;

    private  static GameManager instance;

    public static GameManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = GameObject.FindObjectOfType<GameManager>();
            }

            return instance;
        }
    }

    public bool IsDead => isDead;

    private void Start()
    {
        Application.targetFrameRate = 20;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Update()
    {
        if(!isDead && _player.transform.position.y < _oobYPos)
        {
            Death();
        }
    }

    private void Death()
    {
        isDead = true;

        if (deathRoutine is not null)
            StopCoroutine(deathRoutine);
        deathRoutine = DeathRoutine();
        StartCoroutine(deathRoutine);
    }

    private IEnumerator DeathRoutine()
    {
        _youDiedAnimation.Play();

        if(_effectVolume.profile.TryGet<ColorAdjustments>(out ColorAdjustments component))
        {
            float timer = 0;
            float start = 0;
            float end = -0.8f;

            while(timer < 1)
            {
                component.saturation.value = Mathf.LerpUnclamped(start, -end, timer);
                timer += Time.deltaTime / 2f;
                yield return null;
            }

            component.saturation.value = end;
        }

        yield return new WaitForSeconds(5);

        SceneManager.LoadScene(0);
        deathRoutine = null;
    }
}
