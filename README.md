# Msmq
远程msmq访问
<a href="https://msdn.microsoft.com/en-us/library/windows/desktop/ms699854(v=vs.85).aspx">从远程队列中读取消息（不在一个域内/工作组）</a>

<br><br>
运行非受信任域中Windows Server系列成员的MSMQ 3.0应用程序将使用安全远程读取API。默认情况下，托管包含要读取的消息的队列的MSMQ 3.0服务器要求其他域计算机发出读取请求以建立加密通道，但不能在不可信域之间建立此类通道。因此，将拒绝来自不可信计算机的远程读取请求。若要修改此默认行为并允许消息队列服务器接受来自未建立加密通道的域计算机的请求，请添加HKEY_LOCAL_MACHINE \ SOFTWARE \ Microsoft \ MSMQ \ Parameters \ Security \ NewRemoteReadServerAllowNoneSecurityClient注册表项（DWORD）并将其设置为1。
<br><br>
运行在工作组模式下安装了MSMQ 3.0的Windows Server系列成员的计算机上运行的应用程序将使用安全的远程读取API。工作组计算机无法建立用于远程读取的加密通道，并且默认情况下，托管包含要读取的消息的队列的消息队列服务器接受来自非加密通道上的工作组计算机的请求。若要修改此默认行为，以便MSMQ 3.0服务器拒绝来自工作组计算机的请求，请添加HKEY_LOCAL_MACHINE \ SOFTWARE \ Microsoft \ MSMQ \ Parameters \ Security \ NewRemoteReadServerDenyWorkgroupClient注册表项（DWORD）并将其设置为1。

