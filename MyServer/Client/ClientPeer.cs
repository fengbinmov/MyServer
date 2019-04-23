﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MServer
{
    public abstract class ClientPeer : PeerBase,Commponents
    {
        protected ClientPeer(InitRequest _initRequest):base(_initRequest) { }

        //组件容器 <id , 组件>
        private Dictionary<string, MCommponent> commponDict = new Dictionary<string, MCommponent>();
        public int ComCount { get { return commponDict.Count; } }

        //添加组件
        public bool AddCommponent(MCommponent mCommponent)
        {

            if (mCommponent == null || commponDict.ContainsKey(mCommponent.mName) || commponDict.Count >= 888) return false;

            commponDict.Add(mCommponent.mName, mCommponent);

            return true;
        }

        //删除组件
        public bool RemoveCommponent(string _mName)
        {
            if (commponDict.Count == 0 || !commponDict.ContainsKey(_mName)) return false;

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
