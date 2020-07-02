using HelperTools;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace HeightWeightSECA_v1
{
    public enum ServiceMethodType
    {
        GET, POST, PUT, DELETE
    }

    public class ApiHelper
    {
        /// <summary>
        /// 調用 GET Service 使用 XML 格式
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="serviceURL"></param>
        /// <param name="authorization"></param>
        /// <returns></returns>
        public static T GetInvokeByXML<T>(string serviceURL, string authorization)
        {
            string responseXML = CallServiceByXML(serviceURL, ServiceMethodType.GET, string.Empty, authorization);
            return SerializerHelper.XmlDeserializeEntity<T>(responseXML);
        }        

        /// <summary>
        /// 調用 Service 使用 XML 格式
        /// ***Modify on 20170803 by Arvin for MKTPO1225  DockerCreateItem Phase III
        /// </summary>
        private static string CallServiceByXML(string serviceURL, ServiceMethodType methodType, string requestXML, string authorization)
        {
            string responseXML = CallService(serviceURL, "application/xml", "application/xml", methodType, requestXML, authorization);
            return responseXML;
        }        

        private static string CallService(string serviceURL, string contentType, string accept, ServiceMethodType methodType, string requestString, string authorization)
        {
            string responseString = String.Empty;//***Add on 20170414 by Arvin for MKTPO1225
            StringBuilder logMsg = new StringBuilder();
            HttpWebRequest request = null;

            try
            {
                logMsg.AppendLine();
                logMsg.AppendLine($"[CallService] URL:{serviceURL} ({methodType.ToString()})");
                if (!String.IsNullOrEmpty(authorization))
                    logMsg.AppendLine($"[Authorization]{authorization}");
                logMsg.AppendLine($"[requestMsg]\r\n{requestString}");

                request = (HttpWebRequest)WebRequest.Create(serviceURL);
                request.Method = methodType.ToString();
                request.Accept = accept;
                request.ContentType = contentType;
                request.Proxy = null;

                if (!string.IsNullOrWhiteSpace(authorization) && authorization != "&")
                    request.Headers["Authorization"] = authorization;

                Stream dataStream;

                if (!string.IsNullOrWhiteSpace(requestString))
                {
                    byte[] byteArray = Encoding.UTF8.GetBytes(requestString);

                    using (dataStream = request.GetRequestStreamAsync().Result)
                    {
                        dataStream.Write(byteArray, 0, byteArray.Length);
                    }
                }

                using (WebResponse response = request.GetResponseAsync().Result)
                {
                    using (dataStream = response.GetResponseStream())
                    {
                        using (StreamReader reader = new StreamReader(dataStream))
                        {
                            if (accept.ToLower() == "application/xml")
                            {
                                XmlDocument doc = new XmlDocument();
                                doc.LoadXml(reader.ReadToEnd());
                                responseString = doc.InnerXml;//***Add on 20170414 by Arvin for MKTPO1225  DockerCreateItem Phase II
                            }
                            else //"application/json"
                                responseString = reader.ReadToEnd();

                            logMsg.AppendLine($"[response]\r\n{responseString}");
                            return responseString;
                        }
                    }
                }
            }
            catch (System.Exception ex)
            {
                if (request != null) request.Abort();

                throw ex;
            }
            finally
            {
                //LogHelper.WriteProcessLog(logMsg.ToString());
            }
        }

        public static string CallServiceByJSON(string serviceURL, ServiceMethodType methodType, string requestJSON, string authorization)
        {
            string responseJson = CallService(serviceURL, "application/json", "application/json", methodType, requestJSON, authorization);
            return responseJson;
        }
    }




}
