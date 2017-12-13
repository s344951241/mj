
#import <AVFoundation/AVFoundation.h>
#import "VoiceConverter.h"

@interface SyUnionSDKFun : NSObject<UIApplicationDelegate>
+(id)Instance;
@property(nonatomic)    BOOL    isCanceled;
@property(nonatomic, retain)    NSString*   unityMsgReceiver;
@property(nonatomic)    BOOL    isShowLog;

@property(nonatomic, retain)    AVAudioRecorder*   recorder;
@property(nonatomic, retain)    AVAudioPlayer*   player;
@property(nonatomic, retain)    NSString*   pcm2AmrPath;
@property(nonatomic, retain)    NSString*   recordPath;
@property(nonatomic, retain)    NSString*   pcmPath;
@property(nonatomic, retain)    NSString*   amr2WavPath;
@property(nonatomic, retain)    NSString*   voicePath;

@end




