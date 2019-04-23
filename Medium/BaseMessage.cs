using System;
using System.Net.Sockets;
using System.IO;
using System.Text;


namespace MServer.Medium
{

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
        /// <param name="processCallBackS"></param>
        /// <param name="socket"></param>
        public void ReadMessage(int dataAmount, Action<OperationRequest> processCallBackS, Socket socket)
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
                            Console.WriteLine(curIndex + " " + count);
                        }
                        else
                        {
                            int getCount = socket.Receive(data);
                            getCount = curIndex + getCount > count ? (count - curIndex) : getCount;

                            Array.Copy(data, 0, getDataBuffer, curIndex, getCount);

                            curIndex += getCount;
                            Console.WriteLine(curIndex + " " + count);
                        }
                    }

                    parameters = Encoding.UTF8.GetString(getDataBuffer, 0, getDataBuffer.Length);

                    processCallBackS(new OperationRequest(operationCode, parameters));
                }
                else if (sendType == (int)SendType.File)
                {
                    FileStream file = null;
                    bool getFileHead = true;

                    while (curIndex < count)
                    {
                        if (curIndex == 0 && getFileHead)
                        {
                            //数据包兼容处理
                            int getCount = count > data.Length ? data.Length - 9 : count;

                            Array.Copy(data, 9, getDataBuffer, curIndex, getCount);

                            string[] mess = Encoding.UTF8.GetString(getDataBuffer, 0, getDataBuffer.Length).Split('|');
                            string fileName = mess[0];
                            count = int.Parse(mess[1]);

                            file = File.Create(AppDomain.CurrentDomain.RelativeSearchPath + fileName);

                            curIndex = 0;
                            getFileHead = false;
                        }
                        else
                        {
                            int getCount = socket.Receive(data);
                            getCount = curIndex + getCount > count ? (count - curIndex) : getCount;

                            file.Write(data, 0, getCount);

                            curIndex += getCount;
                            Console.WriteLine(curIndex + " " + count);
                        }
                    }
                    if (file != null)
                    {

                        file.Close();
                        Console.WriteLine("文件保存成功!!");
                    }
                }
            }
            curIndex = 0;
        }
        
    }
}
