# Aliyun-Oss-CSharp-Helper

阿里云 OSS .Net SDK 再次封装

这里的再次封装主要是为了使用方便

<h2>函数列表</h2>
<li>bool CreateBucket</li>
<h5>创建Bucket&nbsp;&nbsp;<em>返回：</em>true-创建成功 false-创建失败</h5>
<li>CannedAccessControlList GetBucketAcl</li>
<h5>获取Bucket访问权限 返回:CannedAccessControlList枚举类型</h5>
<li>IEnumerable<Bucket> ListBuckets</li>
  <h5>列出用户所有Bucket 返回:IEnumerable<Bucket></h5>
    <li>bool IsBucketExist 判断Bucket是否存在</li>
