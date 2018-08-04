using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Aliyun.OSS;
using Aliyun.OSS.Common;
using Aliyun.OSS.Util;
using System.IO;

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
            AccessControlList acl = null;
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
        public bool SetBucketAcl(string _bucketName, CannedAccessControlList accessType)
        {
            this.bucketName = _bucketName;
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
        public bool DeleteBucket(string _bucketName)
        {
            this.bucketName = _bucketName;
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
        /// <param name="_bueketName">Bucket name</param>
        /// <returns></returns>
        public bool PutFile(string _bueketName,string filePath)
        {
            this.bucketName = _bueketName;
            return PutFile(filePath);
        }
        #endregion

        #region Download
        #endregion
    }
}
