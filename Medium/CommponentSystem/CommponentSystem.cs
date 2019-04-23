using System;
using System.Collections.Generic;

namespace MServer
{
    public sealed class CommponentSystem :Commponents
    {
        private static CommponentSystem _commponentSystem;
        public static CommponentSystem CS {
            get {
                if (_commponentSystem == null)
                    _commponentSystem = new CommponentSystem();
                return _commponentSystem;
            }
        }
    
        public int MaxCount = 888;

        //组件容器 <id , 组件>
        private Dictionary<string, MCommponent> commponDict = new Dictionary<string, MCommponent>();


        //添加组件
        public bool AddCommponent(MCommponent mCommponent)
        {

            if (mCommponent == null || commponDict.ContainsKey(mCommponent.mName) || commponDict.Count >= MaxCount) return false;

            commponDict.Add(mCommponent.mName, mCommponent);

            mCommponent.OnEnable();

            return true;
        }

        //删除组件
        public bool RemoveCommponent(string _mName)
        {
            if (commponDict.Count == 0 || !commponDict.ContainsKey(_mName)) return false;

            commponDict[_mName].OnDestroy();

            commponDict.Remove(_mName);

            return true;
        }


        //获取组件
        public MCommponent GetCommponent(string _mName)
        {

            MCommponent value = null;

            if (commponDict.ContainsKey(_mName))
            {
                value = commponDict[_mName];
            }

            return value;
        }
    }
}
