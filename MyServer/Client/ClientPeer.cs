using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MServer
{
    public class ClientPeer : PeerBase,Components
    {
        public ClientPeer(InitRequest _initRequest):base(_initRequest) { }

        //组件容器 <id , 组件>
        private Dictionary<Type, MComponent> componDict = new Dictionary<Type, MComponent>();
        public int ComCount { get { return componDict.Count; } }

        //添加组件
        public T AddComponent<T>() where T : MComponent ,new()
        {
            return AddComponent<T>(new T());
        }
        public T AddComponent<T>(T mComponent) where T : MComponent, new()
        {
            if (mComponent == null) return null;

            if (componDict.ContainsKey(typeof(T)))
            {
                return componDict[typeof(T)] as T;
            }

            componDict.Add(typeof(T), mComponent);

            mComponent.OnEnable();

            return mComponent;
        }

        //删除组件
        public void RemoveComponent<T>()
        {
            if (componDict.Count == 0 || !componDict.ContainsKey(typeof(T))) return;

            componDict[typeof(T)].OnDestroy();

            componDict.Remove(typeof(T));
        }
        //弹出组件
        public T PopComponent<T>() where T : MComponent
        {

            MComponent value = null;

            if (componDict.ContainsKey(typeof(T)))
            {
                value = componDict[typeof(T)];
                componDict[typeof(T)].DisEnable();
                componDict.Remove(typeof(T));
            }

            return value as T;
        }

        //获取组件
        public T GetComponent<T> () where T: MComponent
        {

            MComponent value = null;

            if (componDict.ContainsKey(typeof(T)))
            {
                value = componDict[typeof(T)];
            }

            return value as T;
        }

        public override void OnDisconnect()
        {
            foreach (Type item in componDict.Keys)
            {
                componDict[item].OnDestroy();
            }
            componDict.Clear();
        }
    }
}
