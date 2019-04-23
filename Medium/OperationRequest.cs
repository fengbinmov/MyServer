﻿using System;
using System.Collections.Generic;
using System.Text;

namespace MServer.Medium
{
    public enum SendType
    {
        String = 0,
        File = 1
    }

    public sealed class OperationRequest
    {
        SendType sendType;                          //发送文件类型 0 字符串 | 1 文件
        Int32 operationCode;                    //操作代码
        string parameters;                      //数据信息字典


        public Int32 OperationCode { get { return operationCode; } set { operationCode = value; } }
        public string Parameters { get { return parameters; } set { parameters = value; } }
        


        public OperationRequest() { }

        //send Text
        public OperationRequest(Int32 _operationCode, string _parameters, SendType _sendType = SendType.String)
        {
            operationCode = _operationCode;
            parameters = _parameters;
            sendType = _sendType;
        }


        public byte[] ToBytes()
        {
            byte[] requestBytes = BitConverter.GetBytes(operationCode);
            byte[] msgDataBytes = Encoding.UTF8.GetBytes(parameters);
            byte[] dataAmount = BitConverter.GetBytes(requestBytes.Length + msgDataBytes.Length);
            Console.WriteLine(msgDataBytes.Length);

            byte[] packBytes = new byte[1 + requestBytes.Length + msgDataBytes.Length + dataAmount.Length];
            packBytes[0] = (byte)sendType;
            dataAmount.CopyTo(packBytes, 1);
            requestBytes.CopyTo(packBytes, dataAmount.Length + 1);
            msgDataBytes.CopyTo(packBytes, dataAmount.Length + requestBytes.Length + 1);

            return packBytes;
        }
    }
}
