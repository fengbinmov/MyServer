using System;
using System.Collections.Generic;
using System.Text;

namespace MServer
{
    public interface Commponents
    {
        bool AddCommponent(MCommponent mCommponent);
        bool RemoveCommponent(string _mName);
        MCommponent GetCommponent(string _mName);
    }
}
