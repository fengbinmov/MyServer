using System;
using System.Collections.Generic;
using System.Text;

namespace MServer.Medium
{

    public sealed class OperationRequest
    {
        Int32 operationCode;                     //操作代码
        string parameters;                      //数据信息字典

        public Int32 OperationCode { get { return operationCode; } set { operationCode = value; } }
        public string Parameters { get { return parameters; } set { parameters = value; } }


        public OperationRequest() { }
        public OperationRequest(Int32 _operationCode, string _parameters)
        {
            operationCode = _operationCode;
            parameters = _parameters;
        }
        
        
        public byte[] ToBytes()
        {
            byte[] requestBytes = BitConverter.GetBytes(operationCode);
            byte[] msgDataBytes = Encoding.UTF8.GetBytes(parameters);
            byte[] dataAmount = BitConverter.GetBytes(requestBytes.Length + msgDataBytes.Length);
            
            byte[] packBytes = new byte[requestBytes.Length + msgDataBytes.Length + dataAmount.Length];
            dataAmount.CopyTo(packBytes, 0);
            requestBytes.CopyTo(packBytes, dataAmount.Length);
            msgDataBytes.CopyTo(packBytes, dataAmount.Length + requestBytes.Length);

            return packBytes;
        }
    }
}
