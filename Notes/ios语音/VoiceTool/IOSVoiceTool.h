//
//  IOSVoiceTool.h
//  Unity-iPhone
//
//  Created by aion_mac_one on 2018/12/18.
//
#import <UIKit/UIKit.h>
#import <Foundation/Foundation.h>
#import "IFlyMSC/IFlyMSC.h"
#import "ISRDataHelper.h"
#import "IATConfig.h"

@class IFlySpeechRecognizer;

//需要实现IFlyRecognizerViewDelegate识别协议
@interface IOSVoiceTool : UIViewController<IFlySpeechRecognizerDelegate, AVAudioPlayerDelegate>


@property (nonatomic, strong) AVAudioPlayer * globalPlayer;

-(IFlySpeechRecognizer*) getIFlySpeechRecognize;
+(IOSVoiceTool*) Instance;
+(void) _StartVoice;
+(void) _CloseVoice;
+(void) _PlayMusic:(NSString *)sound_path;

-(void) initRecognizer;

@end


