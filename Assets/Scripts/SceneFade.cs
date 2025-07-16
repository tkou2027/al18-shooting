using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneFade : MonoBehaviour
{
    [SerializeField]
    string nextScene = "Stage";
    [SerializeField]
    float loadTime = 2.0f;
    [SerializeField]
    Transform[] fadeTransforms;

    AudioSource bgmAudio;
    bool loading;
    float loadTimer;
    void Start()
    {
        bgmAudio = GetComponent<AudioSource>();

        loading = false;
        loadTimer = 0f;
    }

    // Update is called once per frame
    void Update()
    {
        if (loading)
        {
            loadTimer += Time.deltaTime;
            if (loadTimer >= loadTime)
            {
                SceneManager.LoadScene(nextScene);
                return;
            }
            float t = 1.0f - loadTimer / loadTime;
            // fade bgm
            if (bgmAudio != null)
            {
                bgmAudio.volume = t * 0.2f;
            }
            // fade sprites
            foreach (Transform transform in fadeTransforms)
            {
                SpriteRenderer renderer = transform.GetComponent<SpriteRenderer>();
                renderer.color = new Color(1, 1, 1, t);
            }
            return;
        }

        if (Input.GetKeyDown(KeyCode.Return))
        {
            loading = true;
        }
    }
}
