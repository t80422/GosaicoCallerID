
// The following ifdef block is the standard way of creating macros which make exporting 
// from a DLL simpler. All files within this DLL are compiled with the AD800DEVICE_EXPORTS
// symbol defined on the command line. this symbol should not be defined on any project
// that uses this DLL. This way any other project whose source files include this file see 
// AD800DEVICE_API functions as being imported from a DLL, wheras this DLL sees symbols
// defined with this macro as being exported.
#ifndef __AD800DEVICE_API_H_
#define __AD800DEVICE_API_H_

#ifdef AD800DEVICE_EXPORTS
#define AD800DEVICE_API __declspec(dllexport)
#else
#define AD800DEVICE_API __declspec(dllimport)
#endif



#ifdef  __cplusplus	
extern "C"
{
#endif
	
	

// windows��Ϣ
#define	WM_AD800MSG WM_USER + 1800

// �ص���������
typedef void (CALLBACK *AD800_EVENTCALLBACKPROC)(int iChannel, int iEvent, int iParam);
typedef void (CALLBACK *AD800_AUDIOCALLBACKPROC)(int iChannel,int AudioIndex,BYTE *pAudioBuff,int AudioLength);
typedef void (CALLBACK *AD800_FSKPROC)(int iChannel,BYTE *pFskBuff,int Length);


// �˿�״̬
enum AD800_STATUS
{
	AD800_DEVICE_CONNECTION = 0,	// �豸����״̬
 
	AD800_LINE_STATUS ,		// ����״̬
	
	AD800_LINE_VOLTAGE ,	// ���ߵ�ѹ

	AD800_LINE_CALLERID ,	// �����������

	AD800_LINE_DTMF ,		// �绰������

	AD800_REC_DATA ,		// ¼������

	AD800_PLAY_FINISHED ,	// �������

	AD800_VOICETRIGGER ,	// ��������״̬

	AD800_BUSYTONE ,		// æ��״̬

	AD800_DTMF_FINISHED ,	// DTMF�������

	// ����
	AD800_SNERROR ,			// ͨ��������Ŵ���
	AD800_VOICESN_ERROR ,	// ����������Ŵ���
	AD800_RECSTATUS ,		// ¼��״̬
 	AD800_ANALYSEDATA ,		// ��ʾ��������ݰ�,����û���������ݰ�
	AD800_PLAYSTATUS ,		// ����״̬
	AD800_PLAYACK_ERROR ,	// ����ACK��ʱ
	AD800_BUSY ,			// ռ��״̬
	AD800_DISPHONE ,		// �����Ͽ�״̬
	AD800_EPROM ,			// eeprom����
	AD800_READSN ,			// ��ȡ�豸sn���
	AD800_READTYPE ,		// ��ȡ�豸����
};

// �豸����״̬
enum AD800_CONNECTION
{
	AD800DEVICE_DISCONNECTED = 0,	// �Ͽ�
	AD800DEVICE_CONNECTED ,			// ����
};

// ����״̬
enum AD800_LINESTATUS
{
	AD800LINE_POWEROFF = 0,
	AD800LINE_HOOKOFF ,
	AD800LINE_HOOKON ,
	AD800LINE_RING ,
	AD800LINE_ANC,				// ���Է�ת(�Ʒ��ź�)
};




// ��ʼ���豸
BOOL AD800DEVICE_API _stdcall AD800_Init(void);

// �ͷ��豸
void AD800DEVICE_API _stdcall AD800_Free(void);


// �����ַ�ʽ��DLL״̬��һ����ͨ����Ϣ����һ���ǻص�����
// ���û�����Ϣ���
void AD800DEVICE_API _stdcall AD800_SetMsgHwnd(HWND hWnd);

// ���ûص�����
void AD800DEVICE_API _stdcall AD800_SetCallbackFun(AD800_EVENTCALLBACKPROC fun);
void AD800DEVICE_API _stdcall AD800_SetAudioCallbackFun(AD800_AUDIOCALLBACKPROC fun);
void AD800DEVICE_API _stdcall AD800_SetFskCallbackFun(AD800_FSKPROC fun);


// �������� - ������
BOOL AD800DEVICE_API _stdcall AD800_SendCommand(int iChannel, BYTE *pCmd, UINT iLen);
BOOL AD800DEVICE_API _stdcall AD800_ReadDeviceSN(int iDevice);
BOOL AD800DEVICE_API _stdcall AD800_ReadDeviceType(int iDevice);
DWORD AD800DEVICE_API _stdcall AD800_GetDeviceType(int iDevice);


// �õ��豸��
int AD800DEVICE_API _stdcall AD800_GetTotalDev(void);

// �õ�ͨ����
int AD800DEVICE_API _stdcall AD800_GetTotalCh(void);


// ��ȡ����״̬
int AD800DEVICE_API _stdcall AD800_GetChState(int iChannel);

// ��ȡ�������
BOOL AD800DEVICE_API _stdcall AD800_GetCallerId(int iChannel, char *pszBuff, UINT iLen);

// ��ȡ����
BOOL AD800DEVICE_API _stdcall AD800_GetDialed(int iChannel, char *pszBuff, UINT iLen);

// ��ȡ�汾��
void AD800DEVICE_API _stdcall AD800_GetVer(int iDevice, char *pszBuff, UINT iLen);

// ��ȡ�豸���
DWORD AD800DEVICE_API _stdcall AD800_GetDevSN(int iDevice);


// ����/��ȡ ������һ��ź�������(Ĭ�ϵ�ѹ��Χ�� 3 - 20v 3��20֮�����Ϊ�����������20����Ϊ�ǹһ�, ���ʱ�������500ms, �һ�300ms, ����200ms)
BOOL AD800DEVICE_API _stdcall AD800_SetHookVoltage(int iChannel, BYTE szHookOffVol, BYTE szHookOnVol);
BOOL AD800DEVICE_API _stdcall AD800_SetHookTime(int iChannel, int iHookOffTime, int iHookOnTime, int iPowerOffTime);

void AD800DEVICE_API _stdcall AD800_GetHookVoltage(int iChannel, BYTE &szHookOffVol, BYTE &szHookOnVol);
void AD800DEVICE_API _stdcall AD800_GetHookTime(int iChannel, int &iHookOffTime, int &iHookOnTime, int &iPowerOffTime);

// ����/��ȡ ����������ʱ��(Ĭ��ʱ����200ms)
BOOL AD800DEVICE_API _stdcall AD800_SetRevCIDTime(int iChannel, int iCIDTime);
void AD800DEVICE_API _stdcall AD800_GetRevCIDTime(int iChannel, int &iCIDTime);


// ����¼���ͷż�����
BOOL AD800DEVICE_API _stdcall AD800_SetRecVolume(int iChannel, int iVol);
BOOL AD800DEVICE_API _stdcall AD800_SetPlaybackVolume(int iChannel, int iVol);

// �õ���ǰ������ֵ
int AD800DEVICE_API _stdcall AD800_GetRecVolume(int iChannel);
int AD800DEVICE_API _stdcall AD800_GetPlaybackVolume(int iChannel);


// ��ʼ¼��
BOOL AD800DEVICE_API _stdcall AD800_StartFileRecFile(int iChannel, char *pszFile);
BOOL AD800DEVICE_API _stdcall AD800_StartMemRec(int iChannel, BYTE *pszBuff, UINT iLen);
int AD800DEVICE_API _stdcall AD800_GetMemRecBytes(int iChannel);

// ֹͣ¼��
int AD800DEVICE_API _stdcall AD800_StopRec(int iChannel);


// ����
BOOL AD800DEVICE_API _stdcall AD800_PlayFile(int iChannel, char *pszFile);
BOOL AD800DEVICE_API _stdcall AD800_PlayMem(int iChannel, BYTE *pszBuff, UINT iLen);

// ֹͣ����
void AD800DEVICE_API _stdcall AD800_StopPlay(int iChannel);



// ��������
BOOL AD800DEVICE_API _stdcall AD800_VoiceTrigger(int iChannel);

// ֹͣ��������
void AD800DEVICE_API _stdcall AD800_StopVoiceTrigger(int iChannel);

// ������������
BOOL AD800DEVICE_API _stdcall AD800_SetVoiceThreshold(int iChannel, int iTime, int iLevel);
BOOL AD800DEVICE_API _stdcall AD800_SetSilenceThreshold(int iChannel, int iTime, int iLevel);

void AD800DEVICE_API _stdcall AD800_GetVoiceThreshold(int iChannel, int &iTime, int &iLevel);
void AD800DEVICE_API _stdcall AD800_GetSilenceThreshold(int iChannel, int &iTime, int &iLevel);



// æ�����
BOOL AD800DEVICE_API _stdcall AD800_DetBusyTone(int iChannel);

// ֹͣæ�����
void AD800DEVICE_API _stdcall AD800_StopDetBusyTone(int iChannel);




// ����DTMF
BOOL AD800DEVICE_API _stdcall AD800_SendDTMF(int iChannel, char *pszBuff, UINT iLen);



// ��Һ͹һ�
BOOL AD800DEVICE_API _stdcall AD800_PickUp(int iChannel);
BOOL AD800DEVICE_API _stdcall AD800_HangUp(int iChannel);

// �жϺ����ӵ绰��
BOOL AD800DEVICE_API _stdcall AD800_DisconnectPhone(int iChannel);
BOOL AD800DEVICE_API _stdcall AD800_ConnectPhone(int iChannel);


// AGC����
BOOL AD800DEVICE_API _stdcall AD800_SetAGC(int iDevice, int iOnOff);



#ifdef  __cplusplus	
}
#endif




#endif