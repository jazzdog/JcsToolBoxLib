#幫助開發C#時許多方便功能函式以加速開發以及除錯!

A. CDbgUtil.cs
  1. CDebug.jmsg("connect IP: {0}", clientIP);
    - 以debug message列印出格式化的訊息
  2. CDebug.jmsgEx("connect IP: {0}", clientIP);
      - 以debug message列印出格式化的訊息 針對VS環境可於output視窗產生可連回程式代碼之連結。
