
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
	
	

// windows消息
#define	WM_AD800MSG WM_USER + 1800

// 回调函数定义
typedef void (CALLBACK *AD800_EVENTCALLBACKPROC)(int iChannel, int iEvent, int iParam);
typedef void (CALLBACK *AD800_AUDIOCALLBACKPROC)(int iChannel,int AudioIndex,BYTE *pAudioBuff,int AudioLength);
typedef void (CALLBACK *AD800_FSKPROC)(int iChannel,BYTE *pFskBuff,int Length);


// 端口状态
enum AD800_STATUS
{
	AD800_DEVICE_CONNECTION = 0,	// 设备连接状态
 
	AD800_LINE_STATUS ,		// 外线状态
	
	AD800_LINE_VOLTAGE ,	// 外线电压

	AD800_LINE_CALLERID ,	// 外线来电号码

	AD800_LINE_DTMF ,		// 电话机拨号

	AD800_REC_DATA ,		// 录音数据

	AD800_PLAY_FINISHED ,	// 放音完成

	AD800_VOICETRIGGER ,	// 语音触发状态

	AD800_BUSYTONE ,		// 忙音状态

	AD800_DTMF_FINISHED ,	// DTMF发送完成

	// 测试
	AD800_SNERROR ,			// 通信数据序号错误
	AD800_VOICESN_ERROR ,	// 语音数据序号错误
	AD800_RECSTATUS ,		// 录音状态
 	AD800_ANALYSEDATA ,		// 显示缓存的数据包,就是没分析的数据包
	AD800_PLAYSTATUS ,		// 放音状态
	AD800_PLAYACK_ERROR ,	// 放音ACK超时
	AD800_BUSY ,			// 占线状态
	AD800_DISPHONE ,		// 话机断开状态
	AD800_EPROM ,			// eeprom操作
	AD800_READSN ,			// 读取设备sn完成
	AD800_READTYPE ,		// 读取设备类型
};

// 设备连接状态
enum AD800_CONNECTION
{
	AD800DEVICE_DISCONNECTED = 0,	// 断开
	AD800DEVICE_CONNECTED ,			// 连上
};

// 外线状态
enum AD800_LINESTATUS
{
	AD800LINE_POWEROFF = 0,
	AD800LINE_HOOKOFF ,
	AD800LINE_HOOKON ,
	AD800LINE_RING ,
	AD800LINE_ANC,				// 极性反转(计费信号)
};




// 初始化设备
BOOL AD800DEVICE_API _stdcall AD800_Init(void);

// 释放设备
void AD800DEVICE_API _stdcall AD800_Free(void);


// 有两种方式收DLL状态，一种是通过消息，别一种是回调函数
// 设置回送消息句柄
void AD800DEVICE_API _stdcall AD800_SetMsgHwnd(HWND hWnd);

// 设置回调函数
void AD800DEVICE_API _stdcall AD800_SetCallbackFun(AD800_EVENTCALLBACKPROC fun);
void AD800DEVICE_API _stdcall AD800_SetAudioCallbackFun(AD800_AUDIOCALLBACKPROC fun);
void AD800DEVICE_API _stdcall AD800_SetFskCallbackFun(AD800_FSKPROC fun);


// 发送命令 - 测试用
BOOL AD800DEVICE_API _stdcall AD800_SendCommand(int iChannel, BYTE *pCmd, UINT iLen);
BOOL AD800DEVICE_API _stdcall AD800_ReadDeviceSN(int iDevice);
BOOL AD800DEVICE_API _stdcall AD800_ReadDeviceType(int iDevice);
DWORD AD800DEVICE_API _stdcall AD800_GetDeviceType(int iDevice);


// 得到设备数
int AD800DEVICE_API _stdcall AD800_GetTotalDev(void);

// 得到通道数
int AD800DEVICE_API _stdcall AD800_GetTotalCh(void);


// 读取外线状态
int AD800DEVICE_API _stdcall AD800_GetChState(int iChannel);

// 读取来电号码
BOOL AD800DEVICE_API _stdcall AD800_GetCallerId(int iChannel, char *pszBuff, UINT iLen);

// 读取拨号
BOOL AD800DEVICE_API _stdcall AD800_GetDialed(int iChannel, char *pszBuff, UINT iLen);

// 读取版本号
void AD800DEVICE_API _stdcall AD800_GetVer(int iDevice, char *pszBuff, UINT iLen);

// 读取设备序号
DWORD AD800DEVICE_API _stdcall AD800_GetDevSN(int iDevice);


// 设置/读取 提机，挂机信号侦测参数(默认电压范围是 3 - 20v 3到20之间就认为是提机，大于20就认为是挂机, 侦测时间是提机500ms, 挂机300ms, 掉电200ms)
BOOL AD800DEVICE_API _stdcall AD800_SetHookVoltage(int iChannel, BYTE szHookOffVol, BYTE szHookOnVol);
BOOL AD800DEVICE_API _stdcall AD800_SetHookTime(int iChannel, int iHookOffTime, int iHookOnTime, int iPowerOffTime);

void AD800DEVICE_API _stdcall AD800_GetHookVoltage(int iChannel, BYTE &szHookOffVol, BYTE &szHookOnVol);
void AD800DEVICE_API _stdcall AD800_GetHookTime(int iChannel, int &iHookOffTime, int &iHookOnTime, int &iPowerOffTime);

// 设置/读取 来电号码结束时间(默认时间是200ms)
BOOL AD800DEVICE_API _stdcall AD800_SetRevCIDTime(int iChannel, int iCIDTime);
void AD800DEVICE_API _stdcall AD800_GetRevCIDTime(int iChannel, int &iCIDTime);


// 设置录音和放间音量
BOOL AD800DEVICE_API _stdcall AD800_SetRecVolume(int iChannel, int iVol);
BOOL AD800DEVICE_API _stdcall AD800_SetPlaybackVolume(int iChannel, int iVol);

// 得到当前的音量值
int AD800DEVICE_API _stdcall AD800_GetRecVolume(int iChannel);
int AD800DEVICE_API _stdcall AD800_GetPlaybackVolume(int iChannel);


// 开始录音
BOOL AD800DEVICE_API _stdcall AD800_StartFileRecFile(int iChannel, char *pszFile);
BOOL AD800DEVICE_API _stdcall AD800_StartMemRec(int iChannel, BYTE *pszBuff, UINT iLen);
int AD800DEVICE_API _stdcall AD800_GetMemRecBytes(int iChannel);

// 停止录音
int AD800DEVICE_API _stdcall AD800_StopRec(int iChannel);


// 放音
BOOL AD800DEVICE_API _stdcall AD800_PlayFile(int iChannel, char *pszFile);
BOOL AD800DEVICE_API _stdcall AD800_PlayMem(int iChannel, BYTE *pszBuff, UINT iLen);

// 停止放音
void AD800DEVICE_API _stdcall AD800_StopPlay(int iChannel);



// 语音触发
BOOL AD800DEVICE_API _stdcall AD800_VoiceTrigger(int iChannel);

// 停止语音触发
void AD800DEVICE_API _stdcall AD800_StopVoiceTrigger(int iChannel);

// 语音触发参数
BOOL AD800DEVICE_API _stdcall AD800_SetVoiceThreshold(int iChannel, int iTime, int iLevel);
BOOL AD800DEVICE_API _stdcall AD800_SetSilenceThreshold(int iChannel, int iTime, int iLevel);

void AD800DEVICE_API _stdcall AD800_GetVoiceThreshold(int iChannel, int &iTime, int &iLevel);
void AD800DEVICE_API _stdcall AD800_GetSilenceThreshold(int iChannel, int &iTime, int &iLevel);



// 忙音侦测
BOOL AD800DEVICE_API _stdcall AD800_DetBusyTone(int iChannel);

// 停止忙音侦测
void AD800DEVICE_API _stdcall AD800_StopDetBusyTone(int iChannel);




// 发送DTMF
BOOL AD800DEVICE_API _stdcall AD800_SendDTMF(int iChannel, char *pszBuff, UINT iLen);



// 提挂和挂机
BOOL AD800DEVICE_API _stdcall AD800_PickUp(int iChannel);
BOOL AD800DEVICE_API _stdcall AD800_HangUp(int iChannel);

// 切断和连接电话机
BOOL AD800DEVICE_API _stdcall AD800_DisconnectPhone(int iChannel);
BOOL AD800DEVICE_API _stdcall AD800_ConnectPhone(int iChannel);


// AGC控制
BOOL AD800DEVICE_API _stdcall AD800_SetAGC(int iDevice, int iOnOff);



#ifdef  __cplusplus	
}
#endif




#endif