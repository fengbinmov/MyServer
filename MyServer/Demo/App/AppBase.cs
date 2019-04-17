using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MServer.Demo
{
    public class AppBase
    {
        private Timer timer = null;
        public bool enable = false;
        

        public void OnEnable() {

            if (!enable) {
                enable = true;
                timer = new Timer(new TimerCallback(Enable), enable, 0,10);
                Start();
            }
        }
        private void Enable(object acit) {
            
            if (enable)
            {
                Update();
            }
            else
            {
                timer.Dispose();
                Destroy();
            }
        }
        public virtual void Start()
        {
        }
        public virtual void Update()
        {
        }
        public virtual void Destroy()
        {
        }
    }
}
