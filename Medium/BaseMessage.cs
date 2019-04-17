using System;
using System.Text;

namespace MServer.Medium
{
    public class BaseMessage
    {
        public byte[] data = new byte[1024];
        public int curIndex = 0;
        
        public int RemainSize { get { return data.Length - curIndex; } }
    
    
        /// <summary>
        /// 粘包的解析
        /// </summary>
        public void ReadMessage(int dataAmount,Action<OperationRequest> processCallBackS){

            curIndex += dataAmount;
            int count = BitConverter.ToInt32(data, 0);

            while (true) {

                if (curIndex - 4 > 0)
                {
                    Int32 operationCode = BitConverter.ToInt32(data, 4);
                    string parameters = Encoding.UTF8.GetString(data, 8, count - 4);
                    
                    processCallBackS(new OperationRequest(operationCode,parameters));

                    Array.Copy(data, count + 4, data, 0, curIndex - count - 4);
                    curIndex -= (count + 4);
                }
                else {
                    curIndex = 0;
                    break;
                }
            }
        }
    }
}
