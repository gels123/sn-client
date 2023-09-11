using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using Skywatch.AssetManagement;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TestAddressable1 : MonoBehaviour
{
    public Button btn;
    public Button btn2;

    public Button btn3;
    public AssetReference aRef;
    // Start is called before the first frame update
    void Start()
    {
        if (btn != null)
        {
            btn.onClick.AddListener((() =>
            {
                Debug.Log("===== TestAddressableScene1 click =====");
                // 场景需要在Build Setting中设置, 默认路径为Resources/
                // SceneManager.LoadScene("TestAddressableScene2");
                
                // 同步从Resources中加载, 默认路径为Resources/
                // var prefab = Resources.Load<GameObject>("Button2");//Button2.prefab
                // var go = Instantiate(prefab,new Vector3(20,20,0), Quaternion.identity, btn.transform);
                
                // 异步从Resources中加载, 默认路径为Resources/
                // var request = Resources.LoadAsync<GameObject>("Button2");
                // Thread.Sleep(1000);
                // var prefab2 = request.asset as GameObject;
                // var go2 = Instantiate(prefab2,new Vector3(20,20,0), Quaternion.identity, btn.transform);
                
                // Addressable加载
                Debug.Log("=============sdfdf= BuildPath=" + Addressables.BuildPath);
                var opt = Addressables.LoadAssetAsync<GameObject>("Assets/Tests/TestAddressable/Button3.prefab");
                opt.Completed += delegate(AsyncOperationHandle<GameObject> handle)
                {
                    if (handle.Status == AsyncOperationStatus.Succeeded)
                    {
                        Debug.Log("=== Addressable LoadAssetAsync Success ===");
                        var go3 = Instantiate(handle.Result, new Vector3(20, 20, 0), Quaternion.identity, btn.transform);
                    }
                    else
                    {
                        Debug.Log("=== Addressable LoadAssetAsync Failed ===");
                    }
                };
                // var opt2 = Addressables.LoadSceneAsync("TestAddressableScene2");
                // opt2.Completed += delegate(AsyncOperationHandle<SceneInstance> handle)
                // {
                //     if (handle.Status == AsyncOperationStatus.Succeeded)
                //     {
                //         Debug.Log("=== Addressable LoadSceneAsync Success ===");
                //     }
                //     else
                //     {
                //         Debug.Log("=== Addressable LoadSceneAsync Failed ===");
                //     }
                // };
            }));
            btn2.onClick.AddListener(delegate
            {
                var opt = Addressables.LoadAssetAsync<GameObject>("Assets/Tests/TestAddressable/Button4.prefab");
                opt.Completed += delegate(AsyncOperationHandle<GameObject> handle)
                {
                    if (handle.Status == AsyncOperationStatus.Succeeded)
                    {
                        Debug.Log("=== Addressable LoadAssetAsync Success ===");
                        var go3 = Instantiate(handle.Result, new Vector3(-20, -20, 0), Quaternion.identity, btn2.transform);
                    }
                    else
                    {
                        Debug.Log("=== Addressable LoadAssetAsync Failed ===");
                    }
                };
            });
            btn3.onClick.AddListener((() =>
            {
                // var ok = AssetManager.LoadAssetAsync(aRef, out AsyncOperationHandle<AudioClip> handle);
                var key = "Assets/AssetsPackage/Audio/btn_click.wav";
                var ok = AssetManager.LoadAssetAsync(key, out AsyncOperationHandle<AudioClip> handle);
                if (ok)
                {
                    AudioSource au = gameObject.GetComponent<AudioSource>();
                    au.PlayOneShot(handle.Result);
                }
                else
                {
                    handle.Completed += (handle) =>
                    {
                        AudioSource au = gameObject.GetComponent<AudioSource>();
                        au.PlayOneShot(handle.Result);
                        AssetManager.Unload(key);
                    };
                }

                var handle3 = AssetManager.LoadAssetsByLabelAsync("default");
                handle3.Completed += (h =>
                {
                    if (h.Status == AsyncOperationStatus.Succeeded)
                    {
                        var list = h.Result;
                        var num = list.Count;
                    }
                });
                
                // if (AssetManager.InstantiateAsync(reference, Vector3.zero, Quaternion.identity, null, out AsyncOperationHandle<ParticleSystem> handle))
                // {
                //     //The particle system has already been loaded.
                //     Destroy(handle.Result.gameObject, 5f);
                // }
                // else
                // {
                //     //The particle system was not yet loaded.
                //     handle.Completed += op =>
                //     {
                //         Destroy(op.Result.gameObject, 5f);
                //     };
                // }
            }));
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
