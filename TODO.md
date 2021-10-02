# TODO
## NeoRedisPlugin:
* 記得把Main拿掉
* 這個class應該用abstract，不適合用public (O)
* SubscribeToSingleChannel 應該放在Comsumer的class裡面 (O)
* PublishToOutlet 應該放在Producer的class裡面 (O)
* 這個Plugin應該還要加入單純傳送Message的模式，像是OCR的結果，就不需要用SharedMemory去傳送，直接用Redis的方式傳送就好，把這個功能也包進去Plugin吧，然後不要跟SharedMemory的Signal用同一條channel (O)
* 要自己創幾個Exception的class，像是電腦沒有安裝Redis的Exception 

## Comsumer:
* bufferCallback 可能為NULL (O)
* 不要用Console.Write跟Console.ReadKey，有可能別人之後要擴寫的時候會不希望自己的Console出現訊息 (O)
* 實際使用時不會保證Producer會比Comsumer早啟動，所以應該分兩步驟初始化：ctor + comsumer.Connect()，還要有一個IsConnect的變數或是Function (O)
* CopyToBuffer的index記得要加上boundary check (O)
* CopyToBuffer的frameTick應該是要回傳給bufferCallback的？
* Comsumer不一定知道nodeSize，像是螢幕的尺寸，這些資料應該放入database

## Producer:
* nodeSize跟nodeCount應該放入database，讓Comsumer讀
* 不要用Console.Write跟Console.ReadKey，有可能別人之後要擴寫的時候會不希望自己的Console出現訊息
* 最好把程式的PID跟outletName & guid一起寫進去，然後在database register component failed的時候就可以回報這些錯誤
* WriteBuffer(T[] data) 應該要寫一個 public void WriteBuffer(ref T data)的版本 (O)