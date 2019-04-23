using System;
using System.Collections.Generic;
using System.Text;

namespace MServer
{
    public class MCommponent
    {
        public MCommponent() {

            mhashID = GetHashCode();
        }

        private int mhashID;

        public string mName;
        public string mTag;
        public bool avtiveSelf {  get; private set; }


        public virtual void OnEnable() {
            if (avtiveSelf) return;
            avtiveSelf = true;
        }

        public virtual void OnDestroy() {
            avtiveSelf = false;
        }
    }
}
