# JcsToolBoxLib <div id="top"/>

* 前言

>須多功能在開發過程中會一再的重複使用，因此建立一個自己的類別來使用，以加速開發過程，甚至找飯粒...範例都具有很高的參考價值。

* 歷史
>Branch:master之後因為 popupElement.cs需要windwos From的環境才能編譯，因此在2018/03/14之後開了另一個branch:noWindowVersion，之後以此版為主要維護

---
## 目錄

1. [CDbgUtil.cs](#CDbgUtil "除錯專用")
    * [jmsg](#CDbgUtil_1)
    * [jmsgt](#CDbgUtil_2)
    * [jmsgEx](#CDbgUtil_3)
    * [dInfo](#CDbgUtil_4)
    * [getDebugLink](#CDbgUtil_5)
2. [CProcEventTask.cs](#CProcEventTask "處理事件列表")
3. [CUtility.cs](#CUtility "常用<未分類>的工具")
4. [CUtilitys_ColorConvert.cs](#CUtilitys_ColorConvert "色彩的工具")
5. [CUtility_DispatcherTimer.cs](#CUtility_DispatcherTimer "定/計時功能")

---
 [top](#top)
>## <div id="CDbgUtil"/>CDbgUtil.cs
>除錯專用，產生訊息

* <div id="CDbgUtil_1"/> void jmsg(String strBugMsg, params Object[] args)
* <div id="CDbgUtil_2"/> void jmsgt(String strBugMsg, params Object[] args)
* <div id="CDbgUtil_3"/> void jmsgEx(String strBugMsg, params Object[] args)
* <div id="CDbgUtil_4"/> dinfo dInfo(int nSkeepLevel=1)
* <div id="CDbgUtil_5"/> string getDebugLink(int nFrameLayer)

 [top](#top)
>## <div id="CProcEventTask"/>CProcEventTask.cs
>處理事件列表，使用**InvokeDelegat**來處理事件代碼相對應建立的處理函式。(目前設定儲存最多100個事件)

 [top](#top)
>## <div id="CUtility"/>CUtility.cs
>常用<未分類>的工具


 [top](#top)
>## <div id="CUtility"/>CUtility.cs
>常用<未分類>的工具



 [top](#top)
>## <div id="CUtilitys_ColorConvert"/>CUtilitys_ColorConvert.cs
>色彩相關工具



 [top](#top)
>## <div id="CUtility_DispatcherTimer"/>CUtility_DispatcherTimer.cs
>定/計時功能


