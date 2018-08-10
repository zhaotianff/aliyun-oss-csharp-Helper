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
            AccessControlList acl = null;
            CannedAccessControlList accessType = CannedAccessControlList.Default;

            try
            {
                acl = client.GetBucketAcl(bucketName);
                accessType = acl.ACL;
            }
            catch (OssException ex)
            {
                lastError = ex;
            }
            return accessType;
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
        public bool PutString(string bucketName,string str,string name)
        {
            this.bucketName = bucketName;
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
        /// Upload file object to bucket with progress
        /// </summary>
        /// <param name="filePath">File to upload</param>
        /// <param name="objcetKey">Object key in oss</param>
        /// <param name="progressCallback">Progress event handler</param>
        public void PutFileProgress(string filePath,string objectKey,EventHandler<StreamTransferProgressArgs> progressCallback)
        {
            try
            {
                using (var fs = File.Open(filePath, FileMode.Open))
                {
                    var putObjectRequest = new PutObjectRequest(bucketName, objectKey, fs);
                    putObjectRequest.StreamTransferProgress += progressCallback;
                    client.PutObject(putObjectRequest);
                }                
            }
            catch (OssException ex)
            {
                lastError = ex;
            }
        }

        /// <summary>
        /// Upload file object to bucket with progress
        /// </summary>
        /// <param name="bucketName">Bucket name</param>
        /// <param name="filePath">File to upload</param>
        /// <param name="objcetKey">Object key in oss</param>
        /// <param name="progressCallback">Progress event handler</param>
        public void PutFileProgress(string bucketName, string filePath, string objcetKey, EventHandler<StreamTransferProgressArgs> progressCallback)
        {
            this.bucketName = bucketName;
            PutFileProgress(filePath, objcetKey, progressCallback);
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
        public void PutFileAsync(string bucketName,string filePath,string objcetKey,Action act)
        {
            try
            {
                resetEvent.Reset();

                using (var fs = File.Open(filePath, FileMode.Open))
                {
                    string result = "Notice user: put object finish";
                    ObjectMetadata metadata = new ObjectMetadata();
                    client.BeginPutObject(bucketName, objcetKey, fs, metadata, PutFileAsyncCallback, result.ToCharArray());
                    resetEvent.WaitOne();
                    
                    if (act != null)
                        act();
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
        private void PutFileAsyncCallback(IAsyncResult ar)
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

        /// <summary>
        /// Put File With Call Back
        /// </summary>      
        /// <param name="filePath">File to upload</param>
        /// <param name="objectKey">Object key in oss</param>
        /// <returns></returns>
        public void PutObjectCallback(string filePath, string objectKey,string callbackUrl,string callbackBody)
        {         
            try
            {
                var metadata = BuildCallbackMetadata(callbackUrl, callbackBody);
                using (var fs = File.Open(filePath, FileMode.Open))
                {
                    var putObjectRequest = new PutObjectRequest(bucketName, objectKey, fs, metadata);
                    var result = client.PutObject(putObjectRequest);                   
                }               
            }
            catch (OssException ex)
            {
                lastError = ex;
            }

        }

        /// <summary>
        /// Build Callback Metadata
        /// </summary>
        /// <param name="callbackUrl"></param>
        /// <param name="callbackBody"></param>
        /// <returns></returns>
        private static ObjectMetadata BuildCallbackMetadata(string callbackUrl, string callbackBody)
        {
            string callbackHeaderBuilder = new CallbackHeaderBuilder(callbackUrl, callbackBody).Build();
            string CallbackVariableHeaderBuilder = new CallbackVariableHeaderBuilder().
                AddCallbackVariable("x:var1", "x:value1").AddCallbackVariable("x:var2", "x:value2").Build();

            var metadata = new ObjectMetadata();
            metadata.AddHeader(HttpHeaders.Callback, callbackHeaderBuilder);
            metadata.AddHeader(HttpHeaders.CallbackVar, CallbackVariableHeaderBuilder);
            return metadata;
        }


        //TODO
        //AppendObject

        /// <summary>
        /// Put File Multi Part
        /// </summary>
        public void PutFileMultipart(string filePath, string objectKey, int partSize)
        {
            try
            {
                var initMultiPartResult = InitiateMultipartUpload(filePath, objectKey);
                var requestId = initMultiPartResult.RequestId;

                if (initMultiPartResult.HttpStatusCode == System.Net.HttpStatusCode.OK)
                {
                    List<PartETag> list = UploadParts(filePath, objectKey, partSize, requestId);
                    var result = CompleteUploadPart(objectKey, requestId, list);
                }
            }
            catch (OssException ex)
            {
                lastError = ex;
            }
        }


        public InitiateMultipartUploadResult InitiateMultipartUpload(string filePath,string objectKey)
        {
            var request = new InitiateMultipartUploadRequest(bucketName, objectKey);
            var result = client.InitiateMultipartUpload(request);
            return result;
        }

        public List<PartETag> UploadParts(string filePath,string objectKey,int partSize,string uploadId)
        {
            var fi = new FileInfo(filePath);
            var fileSize = fi.Length;
            var partCount = fileSize / partSize;
            if (fileSize % partSize != 0)
            {
                partCount++;
            }

            var partETags = new List<PartETag>();
            using (var fs = File.Open(filePath, FileMode.Open))
            {
                for (var i = 0; i < partCount; i++)
                {
                    var skipBytes = (long)partSize * i;
                    fs.Seek(skipBytes, 0);
                    var size = (partSize < fileSize - skipBytes) ? partSize : (fileSize - skipBytes);
                    var request = new UploadPartRequest(bucketName, objectKey, uploadId)
                    {
                        InputStream = fs,
                        PartSize = size,
                        PartNumber = i + 1
                    };

                    var result = client.UploadPart(request);

                    partETags.Add(result.PartETag);                
                }
            }
            return partETags;
        }

        public CompleteMultipartUploadResult CompleteUploadPart(string objectKey, string uploadId, List<PartETag> partETags)
        {
            CompleteMultipartUploadResult uploadResult = new CompleteMultipartUploadResult();
            try
            {
                var completeMultipartUploadRequest = new CompleteMultipartUploadRequest(bucketName, objectKey, uploadId);
                foreach (var partETag in partETags)
                {
                    completeMultipartUploadRequest.PartETags.Add(partETag);
                }
                uploadResult = client.CompleteMultipartUpload(completeMultipartUploadRequest);               
            }
            catch (OssException ex)
            {
                lastError = ex;
            }

            return uploadResult;
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

        /// <summary>
        /// List objcets
        /// </summary>
        /// <returns>OssObjectSummary Collection</returns>
        public IEnumerable<OssObjectSummary> ListObjects()
        {
            IEnumerable<OssObjectSummary> summaryList = new List<OssObjectSummary>();
            try
            {             
                var listObjectsRequest = new ListObjectsRequest(bucketName);
                var result = client.ListObjects(listObjectsRequest);
                summaryList = result.ObjectSummaries;
            }
            catch (OssException ex)
            {
                lastError = ex;
            }
            return summaryList;
        }

        public IEnumerable<OssObjectSummary> ListObjects(string bucketName)
        {
            this.bucketName = bucketName;
            return ListObjects();
        }
        #endregion
    }
}
