
#import "SySDK.h"

@interface SyUnionSDKFun()

@end

@implementation SyUnionSDKFun

@synthesize recorder, player, pcm2AmrPath, recordPath, pcmPath, amr2WavPath, voicePath;

+(id)Instance
{
    static SyUnionSDKFun *s_instance = nil;
    if(nil == s_instance)
    {
        @synchronized(self)
        {
            if(nil == s_instance)
            {
                s_instance = [[self alloc] init];
            }
        }
    }

    return s_instance;
}


-(void)sendMessage:(NSString *)messageName
            param:(NSString *)param
{
    //if(_isShowLog)
    NSLog(@"##Receiver=%@   message=%@  param=%@", _unityMsgReceiver, messageName, param);

    UnitySendMessage([_unityMsgReceiver UTF8String], [messageName UTF8String], [param UTF8String]);
}

-(void)UnityDebugLog:(NSString *)log
{
    [self sendMessage:@"AddLog" param:log];
}


-(void)InitSySDK:(NSString*)reciever isShowLog:(NSString*)showlog 
{
    NSLog(@"##IOS InitSySDK() Recv=%@ showlog=%@", reciever, showlog);
    if([showlog isEqualToString:@"1"])
    {
        _isShowLog = true;
    }

    _unityMsgReceiver = [[NSString alloc]initWithString:reciever];

    NSFileManager *fm = [NSFileManager defaultManager];
    NSString *rootFolderPath = [NSHomeDirectory() stringByAppendingPathComponent:@"Documents/csmj"];
    recordPath = [[NSString alloc]initWithString:[rootFolderPath stringByAppendingPathComponent:@"UnityVoice/record.wav"]];
    voicePath = [[NSString alloc]initWithString:[rootFolderPath stringByAppendingPathComponent:@"UnityVoice"]];
    amr2WavPath = [[NSString alloc]initWithString:[rootFolderPath stringByAppendingPathComponent:@"UnityVoice/amr2Wav.wav"]];
    pcm2AmrPath = [[NSString alloc]initWithString:[rootFolderPath stringByAppendingPathComponent:@"out/wavaudio.amr"]];
    BOOL isDirectory = FALSE;
    BOOL isExist = [fm fileExistsAtPath:voicePath isDirectory:&isDirectory];
    if(!(isExist && isDirectory))
    {
        BOOL isCreated = [fm createDirectoryAtPath:voicePath withIntermediateDirectories:YES attributes:nil error:nil];
        if(!isCreated)
        {
            NSLog(@"##VoicePath create failed");
        }
    }
}


-(void)StartRecord:(NSString *)amrPath
{
    self.pcm2AmrPath = [amrPath stringByAppendingPathComponent:@"wavaudio.amr"];
    NSLog(@"StartRecord armPath =%@", pcm2AmrPath);
    [[AVAudioSession sharedInstance] setCategory: AVAudioSessionCategoryPlayAndRecord error:nil];
    [[AVAudioSession sharedInstance] setActive:YES error:nil];
    if(recorder)
    {
        [recorder stop];
    }
    else
    {
        NSDictionary *recordSetting = @{
                                            AVFormatIDKey           : @(kAudioFormatLinearPCM),
                                            AVSampleRateKey         : @(8000.f),
                                            AVNumberOfChannelsKey   : @(1),
                                            AVLinearPCMBitDepthKey  : @(16),
                                            AVLinearPCMIsNonInterleaved : @NO,
                                            AVLinearPCMIsFloatKey   : @NO,
                                            AVLinearPCMIsBigEndianKey   : @NO
                                        };
        self.recorder = [[AVAudioRecorder alloc]initWithURL:[NSURL URLWithString:recordPath]
                                                    settings:recordSetting
                                                      error:nil];
    }


    recorder.delegate = self;
    recorder.meteringEnabled = YES;

    NSError *error = nil;

    if([recorder prepareToRecord] == YES)
    {
        [self UnityDebugLog:@"StartRecord true"];
        [recorder record];
    }
    else
    {
        int errorCode = CFSwapInt32HostToBig([error code]);
        NSLog(@"error:%@[%4.4s])", [error localizedDescription], (char*)&errorCode);
        [self UnityDebugLog:@"StartRecord false"];

    }
}

-(void)StopRecord
{
    [[AVAudioSession sharedInstance] setCategory: AVAudioSessionCategoryPlayback error:nil];
    [[AVAudioSession sharedInstance] setActive:YES error:nil];
    if(self.recorder)
    {
        [self UnityDebugLog:@"StopRecord"];
        [self.recorder stop];
      //  self.recorder = nil;
    }
}
//recorder的回掉函数
-(void)audioRecorderDidFinishRecording:(AVAudioRecorder *) aRecorder successfully:(BOOL)flag
{
    //NSLog(@"audioRecorderDidFinishRecording:successfully=%@", flag);
    if(flag)
    {
        [self pcm2Amr];
        [self UnityDebugLog:@"audioRecorderDidFinishRecording true"];
        [self sendMessage:@"IOS_RecordEnd" param:pcm2AmrPath];
        
    }
    else
    {
        [self UnityDebugLog:@"audioRecorderDidFinishRecording false"];
    }
}

-(void)pcm2Amr
{
    [VoiceConverter wavToAmr:recordPath amrSavePath:pcm2AmrPath];
}

-(void)playSound:(NSString*)amrPath
{
    if(amrPath.length == 0)
    {
        return;
    }

    if(player)
    {
        [player stop];
    }else
    {
        player = [AVAudioPlayer alloc];
    }
    NSLog(@"playSound = %@", amrPath);

    [VoiceConverter amrToWav:amrPath wavSavePath:amr2WavPath];

    NSString *urlStr = amr2WavPath;
    urlStr = [urlStr stringByAddingPercentEscapesUsingEncoding:NSUTF8StringEncoding];
    NSURL *url = [NSURL URLWithString:urlStr];
    [player initWithContentsOfURL:url error:nil];
    self.player.delegate = self;
    [player play];


}


-(void)stopSound
{
    if(player)
    {
        [player stop];
    }
}

-(void)audioPlayerDidFinishPlaying:(AVAudioPlayer *)player successfully:(BOOL) flag
{
    NSLog(@"play finished");
    if(player)
    {
        [player stop];
    }

    [self sendMessage:@"IOS_PlaySound" param:@"0"];
}

-(void)audioPlayerDecodeErrorDidOccur:(AVAudioPlayer *)player error:(NSError *)error
{
    NSLog(@"play error1");
    [self sendMessage:@"IOS_PlaySound" param:@"1"];  
}

-(void)audioPlayerBeginInterruption:(AVAudioPlayer *)player
{
    NSLog(@"play error2");
    [self sendMessage:@"IOS_PlaySound" param:@"2"];
}

-(void)audioPlayerEndInterruption:(AVAudioPlayer *)player withOptions:(NSUInteger)flags
{
    NSLog(@"play error3");
    [self sendMessage:@"IOS_PlaySound" param:@"3"];
    if(flags == AVAudioSessionInterruptionFlags_ShouldResume && player != NULL)
    {
        [player play];
    }

}

@end


#if defined(__cplusplus)
extern "C"{
#endif    
    NSString* CharToNsstring(char *str)
    {
        if(str == NULL)
        {
            return @"";
        }

        NSString* nstring = [NSString stringWithCString:str encoding:NSUTF8StringEncoding];
        return nstring;
    }
#if defined(__cplusplus)
}
#endif




#if defined(__cplusplus)
extern "C"{
#endif    
    bool canShowLog = false;
    void OCinitsySDK(char* reciever, char* showlog)
    {
        NSString*   NS_receiver = CharToNsstring(reciever);
        NSString*   NS_showlog = CharToNsstring(showlog);

        NSLog(@"###IOS OCinitsySDK() reciever=%@ NS_showlog=%@", NS_receiver, NS_showlog);
        [[SyUnionSDKFun Instance] InitSySDK:NS_receiver isShowLog:NS_showlog];

    }

    void OCsendMsg(char* msg)
    {
        NSString *ns_msg = CharToNsstring(msg);
        [[SyUnionSDKFun Instance] UnityDebugLog:ns_msg];
    }

    void OCstartRecord(char *amrPath)
    {
        NSString *ns_amrPath = CharToNsstring(amrPath);
        [[SyUnionSDKFun Instance] StartRecord:ns_amrPath];
    }

    void OCstopRecord()
    {
        [[SyUnionSDKFun Instance] StopRecord];
    }   

    void OCplaySound(char *amrPath)
    {
        NSString *ns_amrPath = CharToNsstring(amrPath);
        [[SyUnionSDKFun Instance] playSound:ns_amrPath];
    }

    void OC_CallIOS(char* funName, char* strParam[])
    {
        NSString*   NS_funName = CharToNsstring(funName);
        NSString*   NS_strParam = @"";
        NSArray*    arry = NULL;
        int charLeng = 0;
        if([NS_funName isEqualToString:@"OCinitsySDK"])
        {
            NSString*   nsShowLog = CharToNsstring(strParam[1]);
            if([nsShowLog isEqualToString:@"1"])
            {
                canShowLog = true;
            }
        }

        if(strParam != NULL)
        {
            charLeng = sizeof(strParam)/sizeof(strParam[0]);
            for(int i = 0; i < charLeng; i++)
            {
                if(i == charLeng - 1)
                {
                    NS_strParam = [NS_strParam stringByAppendingString:CharToNsstring(strParam[i])];
                }else
                {
                    NS_strParam = [NS_strParam stringByAppendingFormat:@"%@,%@", NS_strParam, CharToNsstring(strParam[i])];
                }

            }

            if(canShowLog)
                NSLog(@"##IOS OCCallIosFun() funName=%@ strParam=%@", NS_funName, NS_strParam);
        }else
        {
            if(canShowLog)
                NSLog(@"##IOS OCCallIosFun() funName=%@", NS_funName);
        }

        if([NS_funName isEqualToString:@"OCinitsySDK"])
        {
            OCinitsySDK(strParam[0], strParam[1]);
        }else if([NS_funName isEqualToString:@"OCsendMsg"])
        {
            OCsendMsg(strParam[0]);
        }else if([NS_funName isEqualToString:@"OCstartRecord"])
        {
            OCstartRecord(strParam[0]);
        }else if([NS_funName isEqualToString:@"OCstopRecord"])
        {
            OCstopRecord();
        }else if([NS_funName isEqualToString:@"OCplaySound"])
        {
            OCplaySound(strParam[0]);
        }



    }


#if defined(__cplusplus)
}
#endif















