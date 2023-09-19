using System;
using System.Collections;
using UnityEngine;
using System.Threading;
using System.Threading.Tasks;

namespace Tests.TestAsync
{
    public class ResourceAsyncOperation : IEnumerator, IDisposable //需继承IEnumerator
    {
        private bool flag = false;
        public ResourceAsyncOperation()
        {
            Debug.Log("f ResourceAsyncOperation new");
            Task.Run((() =>
            {
                Debug.Log("f ResourceAsyncOperation begin");
                // Thread.Sleep(2000);// 导致阻塞Update
                Task.Delay(2000).Wait(); // 不阻塞Update
                flag = true;
                Debug.Log("f ResourceAsyncOperation end");
            }));
        }
        public object Current
        {
            get { return null; }
        }
        public bool isDone
        {
            get
            {
                Debug.Log("f ResourceAsyncOperation call isDone");
                return IsDone();
            }
        }
        public bool MoveNext() //  实现yield return rao等待的关键, 这个非true时会一直卡着等待, 不断检测此函数
        {
            Debug.Log("f ResourceAsyncOperation call MoveNext");
            return !IsDone();
        }
        public void Reset()
        {
        }
        public bool IsDone()
        {
            return flag;
        }
        public void Dispose()
        {
        }
    }
    public class TestAsync : MonoBehaviour
    {
        private int Running = 0;

        private IEnumerator Start()
        {
            Running = 1;
            yield return f3();
            // yield return f4();
            ResourceAsyncOperation rao = new ResourceAsyncOperation();
            yield return rao;
            f5();
        }
        void Update()
        {
            if (Input.GetKeyDown(KeyCode.A))
            {
                Debug.Log("==KeyDown A==");
                if (Running == 0)
                {
                    Running = 1;
                    // f1异步执行, f2同步执行, f2会在f1结束前执行
                    // Task task = Task.Run(f1);
                    // f2();
                    // f1 f2都同步执行, 比如f执行完再f2
                    // f1(); //等于Task.Run(f1).Wait();
                    // f2();
                    
                }
            }
            if (Running > 0)
            {
                Running++;
                Debug.Log("== Update Running==" + Running);
                if (Running > 500)
                {
                    Running = 0;
                }
            }
        }
        async void f1()
        {
            Debug.Log("==f1==");
            // Thread.Sleep(200);// 同步延迟, 阻塞线程,  不能取消, 消耗更少资源
            // Task.Delay(200);  // 异步延迟, 不阻塞线程, 可以取消, 消耗更多资源, 创建一个新的计时任务(类比加载资源任务)
            Task.Delay(500).Wait(); // 等待异步延迟结束
            Debug.Log("==f1 end==");
        }
        void f2()
        {
            Debug.Log("==f2==");
        }
        IEnumerator f3()
        {
            Debug.Log("==f3==");
            yield break;
        }
        IEnumerator f4()
        {
            Debug.Log("==f4 begin==");
            Task task = Task.Run((() =>
            {
                Debug.Log("==f4 run begin==");
                Task.Delay(2000).Wait();
                Debug.Log("==f4 run done==");
            }));
            // 以下task.Wait()会阻塞Update
            // task.Wait();
            Debug.Log("==f4 end==");
            yield break;
        }
        int f5()
        {
            Debug.Log("==f5==");
            return 200;
        }
    }
}