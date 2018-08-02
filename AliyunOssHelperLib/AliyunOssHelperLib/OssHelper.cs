using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Aliyun.OSS;
using Aliyun.OSS.Common;
using Aliyun.OSS.Util;

namespace ZtiLib
{
    public class OssHelper
    {
        private string accessKeyID;
        private string accessKeySecret;
        private string endpoint;
        private string bucketName;
        private string lastError;
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
        public string LastError
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
            catch (Exception ex)
            {
                lastError = ex.Message;
                return false;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public CannedAccessControlList GetBucketAcl()
        {
            AccessControlList acl = null;
            try
            {
                acl = client.GetBucketAcl(bucketName);
            }
            catch (Exception ex)
            {
                lastError = ex.Message;
            }
            return acl.ACL;
        }


        #endregion
    }
}
