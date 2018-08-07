using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Aliyun.OSS;
using Aliyun.OSS.Common;
using Aliyun.OSS.Util;
using System.IO;
using System.Threading;

namespace ZtiLib
{
    public class OssHelper
    {
        private string accessKeyID;
        private string accessKeySecret;
        private string endpoint;
        private string bucketName;
        private OssException lastError;
        private bool isCNameFlag = false;

        private OssClient client;
        AutoResetEvent resetEvent;

        /// <summary>
        /// AccessKeyID
        /// </summary>
        public string AccessKeyID
        {
            get
            {
                return accessKeyID;
            }

            set
            {
                accessKeyID = value;
            }
        }

        /// <summary>
        /// AccessKeySecret
        /// </summary>
        public string AccessKeySecret
        {
            get
            {
                return accessKeySecret;
            }

            set
            {
                accessKeySecret = value;
            }
        }

        /// <summary>
        /// Endpoint URL
        /// </summary>
        public string Endpoint
        {
            get
            {
                return endpoint;
            }

            set
            {
                endpoint = value;
            }
        }

        /// <summary>
        /// Get Last Error
        /// </summary>
        public OssException LastError
        {
            get
            {
                return lastError;
            }
            set
            {
                lastError = value;
            }
        }

        /// <summary>
        /// BucketName
        /// </summary>
        public string BucketName
        {
            get
            {
                return bucketName;
            }

            set
            {
                bucketName = value;
            }
        }

        #region Initialization

        public OssHelper()
        {

        }

        /// <summary>
        /// Instantiate OssHelper Object
        /// </summary>
        /// <param name="_accessKeyID">AccessKeyID</param>
        /// <param name="_accessKeySecret">AccessKeySecret</param>
        /// <param name="_endpoint">Endpoint</param>
        /// <param name="_bucketName">BucketName</param>
        /// <param name="isCName">CName Flag  true-CName false-OSS Domain Name</param>
        /// <remarks>使用CNAME时，无法使用ListBuckets接口。</remarks>
        public OssHelper(string accessKeyID, string accessKeySecret, string endpoint, string bucketName, bool isCName)
        {
            this.accessKeyID = accessKeyID;
            this.accessKeySecret = accessKeySecret;
            this.endpoint = endpoint;
            this.bucketName = bucketName;

            ClientConfiguration conf = new ClientConfiguration();
            conf.IsCname = isCName;
            isCNameFlag = isCName;

            client = new OssClient(endpoint, accessKeyID, accessKeySecret, conf);
            resetEvent= new AutoResetEvent(false);
        }

        #endregion

        #region Bucket

        /// <summary>
        /// CreateBucket
        /// </summary>
        /// <param name="bucketName">BucketName</param>
        /// <returns>result</returns>
        public bool CreateBucket(string bucketName)
        {
            this.bucketName = bucketName;
            return CreateBucket();
        }

        /// <summary>
        /// CreateBucket
        /// </summary>
        /// <param name="bucketName">BucketName</param>
        /// <returns>result</returns>
        public bool CreateBucket()
        {
            try
            {
                var bucket = client.CreateBucket(bucketName);
                return true;
            }
            catch (OssException ex)
            {
                lastError = ex;
                return false;
            }
        }

        /// <summary>
        /// Get Bucket Acl
        /// </summary>
        /// <returns></returns>
        public CannedAccessControlList GetBucketAcl()
        {
            AccessControlList acl = new AccessControlList();
            try
            {
                acl = client.GetBucketAcl(bucketName);
            }
            catch (OssException ex)
            {
                lastError = ex;
            }
            return acl.ACL;
        }

        /// <summary>
        /// List All Buckets
        /// </summary>
        public IEnumerable<Bucket> ListBuckets()
        {
            try
            {
                var buckets = client.ListBuckets();
                return buckets;
            }
            catch (OssException ex)
            {
                lastError = ex;
                return null;
            }
        }

        /// <summary>
        /// Is Bucket Exist
        /// </summary>
        public bool IsBucketExist()
        {
            bool result = false;
            try
            {
                result = client.DoesBucketExist(bucketName);
            }
            catch (OssException ex)
            {
                lastError = ex;
            }

            return result;
        }

        /// <summary>
        /// Is Bucket Exist
        /// </summary>
        /// <param name="_bucketName">Bucket name</param>
        public bool IsBucketExist(string _bucketName)
        {
            this.bucketName = _bucketName;
            return IsBucketExist();
        }

        /// <summary>
        /// Set Bucket Acl
        /// </summary>
        public bool SetBucketAcl(CannedAccessControlList accessType)
        {
            try
            {
                client.SetBucketAcl(bucketName, accessType);
                return true;
            }
            catch (OssException ex)
            {
                lastError = ex;
                return false;
            }
        }

        /// <summary>
        /// Set Bucket Acl
        /// </summary>
        public bool SetBucketAcl(string bucketName, CannedAccessControlList accessType)
        {
            this.bucketName = bucketName;
            return SetBucketAcl(accessType);
        }

        /// <summary>
        /// Delete Bucket
        /// </summary>
        public bool DeleteBucket()
        {
            try
            {
                client.DeleteBucket(bucketName);
                return true;
            }
            catch (OssException ex)
            {
                lastError = ex;
                return false;
            }
        }

        /// <summary>
        /// Delete Bucket
        /// </summary>
        public bool DeleteBucket(string bucketName)
        {
            this.bucketName = bucketName;
            return DeleteBucket();
        }

        #endregion

        #region Upload

        /// <summary>
        /// Upload string object to bucket
        /// </summary>
        /// <param name="str">string object</param>
        /// <param name="name">designate object name</param>
        /// <returns></returns>
        public bool PutString(string str,string name)
        {
            try
            {            
                byte[] binaryData = Encoding.ASCII.GetBytes(str);
                MemoryStream requestContent = new MemoryStream(binaryData);
                client.PutObject(bucketName, name, requestContent);
                return true;
            }
            catch (OssException ex)
            {
                lastError = ex;
                return false;
            }
        }

        /// <summary>
        /// Upload string object to bucket
        /// </summary>
        ///<param name="_bucketName">Bucket name</param>
        ///<param name="str">string object</param>
        ///<param name="name">designate object name</param>
        /// <returns></returns>
        public bool PutString(string _bucketName,string str,string name)
        {
            this.bucketName = _bucketName;
            return PutString(str,name);
        }


        /// <summary>
        /// Upload file object to bucket
        /// </summary>
        /// <param name="filePath">File to upload</param>
        /// <returns></returns>
        public bool PutFile(string filePath)
        {
            try
            {             
                client.PutObject(bucketName, System.IO.Path.GetFileName(filePath), filePath);
                return true;
            }
            catch (OssException ex)
            {
                lastError = ex;
                return false;
            }
        }

        /// <summary>
        /// Upload file object to bucket
        /// </summary>
        /// <param name="filePath">File to upload</param>
        /// <param name="objectKey">Object key in oss</param>
        /// <returns></returns>
        public bool PutFile(string filePath,string objectKey)
        {
            try
            {
                client.PutObject(bucketName, objectKey, filePath);
                return true;
            }
            catch (OssException ex)
            {
                lastError = ex;
                return false;
            }
        }

        /// <summary>
        /// Upload file object to bucket
        /// </summary>
        /// <param name="filePath">File to upload</param>
        /// <param name="_bueketName">Bucket name</param>
        /// <param name="objectKey">Object key in oss</param>
        /// <returns></returns>
        public bool PutFile(string bueketName, string filePath,string objectKey)
        {
            this.bucketName = bueketName;
            return PutFile(filePath,objectKey);
        }

        /// <summary>
        /// Put File With Md5 Check
        /// </summary>
        /// <param name="filePath">File to upload</param>
        /// <param name="objectKey">Object key in oss</param>
        /// <param name="md5">md5 code</param>
        /// <returns></returns>
        public bool PutFileWithMd5(string filePath, string objectKey,string md5)
        {
            try
            {
                using (FileStream fs = File.Open(filePath, FileMode.Open))
                {
                    md5 = OssUtils.ComputeContentMd5(fs, fs.Length);
                }
                ObjectMetadata objectMeta = new ObjectMetadata
                {
                    ContentMd5 = md5
                };
                client.PutObject(bucketName, objectKey, filePath, objectMeta);
                return true;
            }
            catch (OssException ex)
            {
                lastError = ex;
                return false;
            }
        }

        /// <summary>
        /// Put File With Md5 Check
        /// </summary>
        /// <param name="bucketName">Bucket name</param>
        /// <param name="filePath">File to upload</param>
        /// <param name="objectKey">Object key in oss</param>
        /// <param name="md5">md5 code</param>
        /// <returns></returns>
        public bool PutFileWithMd5(string bucketName,string filePath, string objectKey, string md5)
        {
            this.bucketName = bucketName;
            return PutFileWithMd5(filePath, objectKey, md5);
        }

        //TODO
        //PutFileWithHeader()

        /// <summary>
        /// Pub File Async
        /// </summary>
        /// <param name="bucketName">Bucket Name</param>
        public void PutFileAsync(string bucketName,string filePath,string objcetKey)
        {
            try
            {
                resetEvent.Reset();

                using (var fs = File.Open(filePath, FileMode.Open))
                {
                    string result = "Notice user: put object finish";
                    ObjectMetadata metadata = new ObjectMetadata();
                    client.BeginPutObject(bucketName, objcetKey, fs, metadata, PutFileCallback, result.ToCharArray());
                    resetEvent.WaitOne();
                }
            }
            catch (OssException ex)
            {
                lastError = ex;
            }
        }

        /// <summary>
        /// Put File Async Callback 
        /// </summary>
        /// <param name="ar">IAsyncResult</param>
        private void PutFileCallback(IAsyncResult ar)
        {
            try
            {
                client.EndPutObject(ar);              
            }
            catch (OssException ex)
            {
                throw ex;
            }
            finally
            {
                resetEvent.Set();
            }
        }
        #endregion

        #region Download

        #endregion

        #region Object Management
        /// <summary>
        /// Delete object
        /// </summary>
        /// <param name="key">object name</param>
        /// <remarks>删除文件夹需要在后面加个"/"</remarks>
        public bool DeleteObject(string name)
        {
            try
            {
                client.DeleteObject(bucketName, name);
                return true;
            }
            catch (OssException ex)
            {
                lastError = ex;
                return false;
            }
        }

        /// <summary>
        /// Delete object
        /// </summary>
        /// <param name="bucketName">Bucket name</param>
        /// <param name="key">object name</param>
        public bool DeleteObject(string bucketName, string name)
        {
            this.bucketName = bucketName;
            return DeleteObject(name);
        }
        #endregion
    }
}
