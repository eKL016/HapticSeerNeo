# TODO
## NeoRedisPlugin:
* �O�o��Main����
* �o��class���ӥ�abstract�A���A�X��public (O)
* SubscribeToSingleChannel ���ө�bComsumer��class�̭� (O)
* PublishToOutlet ���ө�bProducer��class�̭� (O)
* �o��Plugin�����٭n�[�J��¶ǰeMessage���Ҧ��A���OOCR�����G�A�N���ݭn��SharedMemory�h�ǰe�A������Redis���覡�ǰe�N�n�A��o�ӥ\��]�]�i�hPlugin�a�A�M�ᤣ�n��SharedMemory��Signal�ΦP�@��channel (O)
* �n�ۤv�дX��Exception��class�A���O�q���S���w��Redis��Exception 

## Comsumer:
* bufferCallback �i�ରNULL (O)
* ���n��Console.Write��Console.ReadKey�A���i��O�H����n�X�g���ɭԷ|���Ʊ�ۤv��Console�X�{�T�� (O)
* ��ڨϥήɤ��|�O��Producer�|��Comsumer���ҰʡA�ҥH���Ӥ���B�J��l�ơGctor + comsumer.Connect()�A�٭n���@��IsConnect���ܼƩάOFunction (O)
* CopyToBuffer��index�O�o�n�[�Wboundary check (O)
* CopyToBuffer��frameTick���ӬO�n�^�ǵ�bufferCallback���H
* Comsumer���@�w���DnodeSize�A���O�ù����ؤo�A�o�Ǹ�����ө�Jdatabase

## Producer:
* nodeSize��nodeCount���ө�Jdatabase�A��ComsumerŪ
* ���n��Console.Write��Console.ReadKey�A���i��O�H����n�X�g���ɭԷ|���Ʊ�ۤv��Console�X�{�T��
* �̦n��{����PID��outletName & guid�@�_�g�i�h�A�M��bdatabase register component failed���ɭԴN�i�H�^���o�ǿ��~
* WriteBuffer(T[] data) ���ӭn�g�@�� public void WriteBuffer(ref T data)������ (O)