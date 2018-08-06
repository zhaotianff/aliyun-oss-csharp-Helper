# Aliyun-Oss-CSharp-Helper

阿里云 OSS .Net SDK 再次封装

这里的再次封装主要是为了使用方便

<h2>函数列表
<h4>1、bool CreateBucket
<h5>创建Bucket 返回: true-创建成功 false-创建失败
<h4>2、CannedAccessControlList GetBucketAcl 获取Bucket访问权限 返回:CannedAccessControlList枚举类型
<h4>3、IEnumerable<Bucket> ListBuckets 列出用户所有Bucket 返回:IEnumerable<Bucket>
<h4>4、bool IsBucketExist 判断Bucket是否存在
