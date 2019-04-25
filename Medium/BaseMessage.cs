using System;
using System.Net.Sockets;
using System.IO;
using System.Text;
using System.Collections.Generic;


namespace MServer.Medium
{

    public class SendCode
    {
        //文件类
        public const int SEND_FILE_HEAD = 22;
        public const int SEND_FILE_DATA = 21;
        public const int SEND_FILE_END = 20;
        
        //响应类
        public const int SUCCEED = 66;      //成功
        public const int FAILURE = 65;      //失败
        public const int UNKNOWN = 64;      //未知
        public const int BEGING = 63;       //开始
        public const int CANCEL = 62;       //取消
        public const int FINISH = 61;       //完成
        public const int TERMINATION = 60;  //终止


        public static bool Equals(int code, string codestring) {

            int codeS;
            if (Int32.TryParse(codestring, out codeS))
            {
                return codeS == code;
            }
            else {
                return false;
            }
        }
        public static bool Equals(string codestring, int code)
        {

            int codeS;
            if (Int32.TryParse(codestring, out codeS))
            {
                return codeS == code;
            }
            else
            {
                return false;
            }
        }
        public static bool Equals(string codeA, string codeB)
        {
            int codea;
            int codeb;
            if (Int32.TryParse(codeA, out codea) && Int32.TryParse(codeB, out codeb))
            {
                return codea == codeb;
            }
            else
            {
                return false;
            }
        }
    }

    public class BaseMessage
    {

        public byte[] data = new byte[1024];
        public int curIndex = 0;
        
        public int RemainSize { get { return data.Length - curIndex; } }

        //只包含mess
        private byte[] getDataBuffer;
        

        /// <summary>
        /// 粘包的解析 -传输文件的最大大小为 2147483647kb
        /// </summary>
        /// <param name="dataAmount"></param>
        /// <param name="processCallBack"></param>
        /// <param name="socket"></param>
        public void ReadMessage(int dataAmount, Action<OperationRequest> processCallBack, Socket socket)
        {
            //真实的可用数据总数 count = mess
            byte sendType = data[0];
            int count = BitConverter.ToInt32(data, 1) - 4;  //=

            if (dataAmount - 5 > 0 && count > 0)
            {
                Int32 operationCode = BitConverter.ToInt32(data, 5);

                curIndex = 0;

                try
                {
                    getDataBuffer = new byte[count];    //=
                }
                catch (Exception) {
                    getDataBuffer = null;
                    Console.WriteLine("数据出错");
                }


                if (sendType == (int)SendType.String)
                {
                    string parameters = "";
                    while (curIndex < count)
                    {
                        if (curIndex == 0)
                        {
                            //数据包兼容处理
                            int getCount = count > data.Length ? data.Length - 9 : count;

                            Array.Copy(data, 9, getDataBuffer, curIndex, getCount);

                            curIndex += getCount;
                            //Console.WriteLine("ReadMessage="+curIndex + " " + count);
                        }
                        else
                        {
                            int getCount = socket.Receive(data);
                            getCount = curIndex + getCount > count ? (count - curIndex) : getCount;

                            Array.Copy(data, 0, getDataBuffer, curIndex, getCount);

                            curIndex += getCount;
                            //Console.WriteLine("ReadMessage=" + curIndex + " " + count);
                        }
                    }

                    parameters = Encoding.UTF8.GetString(getDataBuffer, 0, getDataBuffer.Length);

                    processCallBack(new OperationRequest(operationCode, parameters));
                }
                else if (sendType == (int)SendType.File)
                {
                    bool getfile = false;
                    //FileStream file = null;
                    bool getFileHead = true;

                    while (curIndex < count)
                    {
                        if (curIndex == 0 && getFileHead)
                        {

                            //数据包兼容处理
                            int getCount = count > data.Length ? data.Length - 9 : count;

                            Array.Copy(data, 9, getDataBuffer, curIndex, getCount);
                            
                            string fileInfo = Encoding.UTF8.GetString(getDataBuffer, 0, getDataBuffer.Length);
                            
                            string[] mess = fileInfo.Split('|');
                            string fileName = mess[0];
                            count = int.Parse(mess[1]);

                            processCallBack(new OperationRequest(SendCode.SEND_FILE_HEAD, fileInfo));

                            curIndex = 0;
                            getFileHead = false;
                            getfile = true;
                        }
                        else
                        {
                            int getCount = socket.Receive(data);

                            getCount = curIndex + getCount > count ? (count - curIndex) : getCount;

                            byte[] getData = new byte[getCount];
                            Array.Copy(data, 0, getData, 0, getCount);
                            processCallBack(new OperationRequest(SendCode.SEND_FILE_DATA, getData));

                            //file.Write(data, 0, getCount);

                            curIndex += getCount;
                            //Console.WriteLine(curIndex + " " + count);
                        }
                    }
                    if (getfile)
                    {
                        processCallBack(new OperationRequest(SendCode.SEND_FILE_END, SendCode.SUCCEED));
                    }
                    else
                    {
                        processCallBack(new OperationRequest(SendCode.SEND_FILE_END, SendCode.FAILURE));
                    }
                }
            }
            curIndex = 0;
        }
    }
}
