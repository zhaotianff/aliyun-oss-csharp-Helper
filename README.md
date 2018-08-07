# Aliyun-Oss-CSharp-Helper

阿里云 OSS .Net SDK 再次封装

这里的再次封装主要是为了使用方便，官方给的文档太分散。

<h2>函数列表</h2>
<li>bool CreateBucket()</li>
<h5>创建Bucket&nbsp;&nbsp;<em>返回：</em>true-创建成功 false-创建失败</h5>
<li>CannedAccessControlList GetBucketAcl()</li>
<h5>获取Bucket访问权限 返回:CannedAccessControlList枚举类型</h5>
<li>IEnumerable<Bucket> ListBuckets()</li>
<h5>列出用户所有Bucket 返回:IEnumerable&lt;Bucket&gt;</h5>
<li>bool IsBucketExist()</li>
<h5>判断Bucket是否存在</h5>
<li>bool IsBucketExist(string bucketName)</li>
<h5>判断指定的Bucket是否存在 参数：bucketName-指定Bucket 返回：true-存在 false-不存在</h5>
<li>bool SetBucketAcl(CannedAccessControlList accessType)</li>
<h5>设置Bucket访问权限 参数：accessType-访问权限 返回：true-设置成功 false-设置失败</h5>
<li>bool SetBucketAcl()</li>
<h5>设置指定Bucket访问权限 参数：bucketName-指定Bucket accessType-访问权限 返回：true-设置成功 false-设置失败</h5>
<li>bool DeleteBucket()</li>
<h5>删除Bucket 返回：true-删除成功 false-删除失败</h5>
<li>bool DeleteBucket(string bucketName)</li>
<h5>删除指定Bucket</h5>
===============

<br/><br/>
<h2>使用示例</h2>
<h5>上传进度事件处理函数</h5>
<pre><code>
        private static void streamProgressCallback(object sender, StreamTransferProgressArgs args)
        {
            System.Console.WriteLine("ProgressCallback - TotalBytes:{0}, TransferredBytes:{1}",
                args.TotalBytes, args.TransferredBytes);
        }
</code></pre>


