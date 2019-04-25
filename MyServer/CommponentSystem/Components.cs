using System;
using System.Collections.Generic;
using System.Text;

namespace MServer
{
    public interface Components
    {
        T AddComponent<T>() where T: MComponent, new();
        T AddComponent<T>(T mComponent) where T : MComponent, new();
        void RemoveComponent<T>();
        T PopComponent<T>() where T : MComponent;
        T GetComponent<T>() where T : MComponent;
    }
}
