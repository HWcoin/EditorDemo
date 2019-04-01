//
//  IOSVoiceTool.m
//  Unity-iPhone
//
//  Created by aion_mac_one on 2018/12/18.
//
#import "IOSVoiceTool.h"
#import "AudioFormatHelper.h"
/*
 rootPath : /Documents
 pcmPath  : /Library/Caches/wavaudio.pcm
 amrPath  : /Documents/UnityVoice/audio.amr
 wavPath  : /Documents/UnityVoice/amraudio.wav
 sendToUnityPath : audio.amr
 playMusic: /Documents/UnityVoice/music_name
 */

@implementation IOSVoiceTool{
    IFlySpeechRecognizer *iFlySpeechRecognizer;
    NSString *rootPath;
    NSString *pcmPath;
    NSString *amrPath;
    NSString *wavPath;
    NSString *sendToUnityPath;
    BOOL _played;
    //AVAudioPlayer *audioPlayer;
}

static IOSVoiceTool *_instance;
+(IOSVoiceTool*) Instance
{
    if (_instance == nil){
        _instance = [[IOSVoiceTool alloc] init];
    }
    return _instance;
}

-(void) StartVoice
{
    if (iFlySpeechRecognizer == nil)
    {
        [self initRecognizer];
    }
    [iFlySpeechRecognizer cancel];
    
    
    BOOL listeningResult = [iFlySpeechRecognizer startListening];
    if (listeningResult)
        NSLog(@"iFly listen begin:");
    else
        NSLog(@"iFly listen error!!!");
}

-(void) CloseVoice
{
    if ([iFlySpeechRecognizer isListening])
        [iFlySpeechRecognizer stopListening];
}
-(void) CancelVoice
{
    if ([iFlySpeechRecognizer isListening])
        [iFlySpeechRecognizer cancel];
}

-(void) DestroyVoice
{
    [iFlySpeechRecognizer cancel];
    [iFlySpeechRecognizer destroy];
}

-(void) PlayMusic:(NSString *)sound_path
{
    //sound_path : /1_2.amr
    //根路径Document/UnityVoice
    NSString *docVoicePath = [[NSHomeDirectory() stringByAppendingPathComponent:@"Documents"] stringByAppendingString:@"/UnityVoice"];
    
    //amr文件路径
    NSString *sourcePath = [docVoicePath stringByAppendingString:sound_path];
    
    //wav文件路径
    NSRange range = NSMakeRange(0, sourcePath.length-4);
    NSString *wavPath = [NSString stringWithFormat:@"%@.wav", [sourcePath substringWithRange:range]];
    
    NSLog(@"[iFly] source: %@", sourcePath);
    NSLog(@"[iFly] wavPath: %@", wavPath);
    
    NSFileManager *fileManager = [NSFileManager defaultManager];
    BOOL isDir = YES;
    BOOL isFileExist = [fileManager fileExistsAtPath:sourcePath isDirectory:&isDir];
    NSLog(@"当前播放的语音文件：%@", sourcePath);
    
    if (isFileExist && (isDir==FALSE)){//存在且不是目录
        
        BOOL wavIsDir = YES;
        BOOL wavIsExist = [fileManager fileExistsAtPath:wavPath isDirectory:&wavIsDir];
        //不存在
        if (!(wavIsExist && (wavIsDir==FALSE)))
        {
            [AudioFormatHelper amr_to_wav:sourcePath targetFormat:wavPath];
        }
        
        NSURL *url = [NSURL fileURLWithPath:wavPath];
        NSError *err = Nil;
        if (_globalPlayer != Nil)
            [_globalPlayer stop];
        _globalPlayer = [[AVAudioPlayer alloc] initWithContentsOfURL:url error:&err];
        [_globalPlayer prepareToPlay];
        _globalPlayer.volume = 1.0;
        if (err){
            NSLog(@"playVoice err:%@", err.localizedDescription);
            return;
        }
        _globalPlayer.delegate = self;
        
        [_globalPlayer play];
        _played = YES;
        
        UnitySendMessage("LayerUIMain", "PlayVoice", [sourcePath UTF8String]);
    }else{//不存在
        UnitySendMessage("LayerUIMain", "PlayVoiceError", "1");
    }
    
}

- (void)audioPlayerDidFinishPlaying:(AVAudioPlayer *)player successfully:(BOOL)flag
{
    [player stop];
}

- (void)audioPlayerBeginInterruption:(AVAudioPlayer *)player
{
    [player stop];
}

+(void) _StartVoice
{
    [[IOSVoiceTool Instance] StartVoice];
}
+(void) _CloseVoice
{
    [[IOSVoiceTool Instance] CloseVoice];
}
void _CancelVoice()
{
    [[IOSVoiceTool Instance] CancelVoice];
}
void _DestroyVoice()
{
    [[IOSVoiceTool Instance] DestroyVoice];
}
+(void) _PlayMusic:(NSString *)sound_path
{
    [[IOSVoiceTool Instance] PlayMusic:sound_path];
}
////////////////////////////////////////////////////////////////////////////////////////////callback
-(void) onBeginOfSpeech
{
    NSLog(@"iFly onBeginOfSpeech");
    //@@@do nothing, notify unity client
    UnitySendMessage("LayerUIMain", "StartTalk", "");
}

- (void)onError: (IFlySpeechError *) error{
    NSLog(@"iFly onError %@", error.errorDesc);
    [iFlySpeechRecognizer stopListening];
    //@@@do nothing, notify unity client
    UnitySendMessage("LayerUIMain", "TalkError", error.errorCode);
}

- (void)onCancel
{
    NSLog(@"iFly onCancel");
}

-(void) onResults:(NSArray *)results isLast:(BOOL)isLast
{
    NSLog(@"iFly onResults");
    if (results == NULL)
    {
        NSLog(@"iFly result is NULL");
        return;
    }
    NSMutableString * resultString = [[NSMutableString alloc] init];
    NSDictionary *dic = results[0];
    for (NSString *key in dic){
        [resultString appendFormat:@"%@",key];
    }
    NSString *resultFromJson = [ISRDataHelper stringFromJson:resultString];
    //@@@notify Unity client
    UnitySendMessage("LayerUIMain", "VoiceMessage", [resultFromJson UTF8String]);

    NSLog(@"iFly result:%@", resultFromJson);
    if (isLast)
    {
        NSThread *thread = [[NSThread alloc] initWithBlock:^{
            [[IOSVoiceTool Instance] saveAmr:pcmPath];
            long duration = [AudioFormatHelper getAmrDuration:amrPath];
            NSString *sendMsg = [[NSString alloc] initWithFormat:@"%@[duration=%ld]",sendToUnityPath, duration];
            UnitySendMessage("LayerUIMain", "EndTalk", [sendMsg UTF8String]);
        }];
        [thread start];
    }
}

- (void)onEndOfSpeech
{
    NSLog(@"iFly onEndOfSpeech");
}

-(void) onVolumeChanged:(int)volume
{
    //@@@do nothing but notify unity clietn the volume value
    NSString *volumeStr = [NSString stringWithFormat:@"%d", volume];
    UnitySendMessage("LayerUIMain", "VoiceVolume", [volumeStr UTF8String]);
}
-(void) onCompleted:(IFlySpeechError *)errorCode
{
    NSLog(@"iFly onComplete %@", errorCode.errorDesc);
}
/////////////////////////////////////////////////////////////////////////////////////////////////callback end

-(void) saveAmr:(NSString *)_pcmPath
{
    [self pcm2wav:_pcmPath];
    [self wav2amr:wavPath];
}

-(void) pcm2wav:(NSString *)_pcmPath
{
    [AudioFormatHelper pcm_to_wav:[_pcmPath UTF8String] targetFile:[wavPath UTF8String] Chanels:1 Rate:8000];
}

-(void) wav2amr:(NSString *)wavPath
{
    [AudioFormatHelper wav_to_amr:wavPath targetFormat:amrPath];
}

-(IFlySpeechRecognizer*) getIFlySpeechRecognize
{
    if (iFlySpeechRecognizer != nil)
        return iFlySpeechRecognizer;
    
    [self initRecognizer];
    
    return iFlySpeechRecognizer;
}

-(void) initFolder
{
    NSString *path = [[NSHomeDirectory() stringByAppendingPathComponent:@"Documents"] stringByAppendingString:@"/UnityVoice"];
    NSFileManager *fileManager = [NSFileManager defaultManager];
    BOOL isDir = FALSE;
    BOOL isDirExist = [fileManager fileExistsAtPath:path isDirectory:&isDir];
    if (!(isDirExist && isDir)){
        BOOL bCreateDir = [fileManager createDirectoryAtPath:path withIntermediateDirectories:YES attributes:nil error:nil];
        if (!bCreateDir){
            NSLog(@"[iFly] 重要信息，创建文件夹UnityVoice失败，语音功能可能失败！！！");
            NSLog(@"%@", path);
        }
    }
}

-(void) initRecognizer
{
    NSLog(@"[iFly init recognizer]");
    //recognition singleton without view
    if (iFlySpeechRecognizer == nil) {
        iFlySpeechRecognizer = [IFlySpeechRecognizer sharedInstance];
    }
    //instance = [IATConfig sharedInstance];
    NSString *documentPath = [NSHomeDirectory() stringByAppendingPathComponent:@"Documents"];
    NSString *libCachesPath = [[NSHomeDirectory() stringByAppendingString:@"/Library"] stringByAppendingString:@"/Caches"];
    //创建UnityVoice文件夹
    [self initFolder];
    
    sendToUnityPath = @"audio.amr";
    pcmPath = [libCachesPath stringByAppendingString:@"/wavaudio.pcm"];
    wavPath = [[documentPath stringByAppendingString:@"/UnityVoice"] stringByAppendingString:@"/amraudio.wav"];
    amrPath = [[documentPath stringByAppendingString:@"/UnityVoice"] stringByAppendingString:@"/audio.amr"];
    
    //audio_source
    [iFlySpeechRecognizer setParameter:IFLY_AUDIO_SOURCE_MIC forKey:@"audio_source"];
    //result json
    [iFlySpeechRecognizer setParameter:@"json" forKey:[IFlySpeechConstant RESULT_TYPE]];
    
    //document / UnityVoice / audio.pcm
    //[iFlySpeechRecognizer setParameter:@"" forKey:[IFlySpeechConstant PARAMS]];
    //set recognition domain
    //听写模式
    [iFlySpeechRecognizer setParameter:@"iat" forKey:[IFlySpeechConstant IFLY_DOMAIN]];
    //录音文件
    //[iFlySpeechRecognizer setParameter:@"iat.pcm" forKey:pcmPath];
    [iFlySpeechRecognizer setParameter:@"wavaudio.pcm" forKey:[IFlySpeechConstant ASR_AUDIO_PATH]];
    
    //set timeout of recording录音超时时间
    [iFlySpeechRecognizer setParameter:@"40000" forKey:[IFlySpeechConstant SPEECH_TIMEOUT]];
    //set VAD timeout of end of speech(EOS)
    //设置语音后端点:后端点静音检测时间，即用户停止说话多长时间内即认为不再输入， 自动停止录音
    [iFlySpeechRecognizer setParameter:@"1500" forKey:[IFlySpeechConstant VAD_EOS]];
    //set VAD timeout of beginning of speech(BOS)
    //设置语音前端点:静音超时时间，即用户多长时间不说话则当做超时处理
    [iFlySpeechRecognizer setParameter:@"4000" forKey:[IFlySpeechConstant VAD_BOS]];
    //set network timeout网络超时时间
    [iFlySpeechRecognizer setParameter:@"20000" forKey:[IFlySpeechConstant NET_TIMEOUT]];
    //set sample rate, 16K as a recommended option采样率
    [iFlySpeechRecognizer setParameter:@"8000" forKey:[IFlySpeechConstant SAMPLE_RATE]];
    
    //set language语言
    [iFlySpeechRecognizer setParameter:@"zh_cn" forKey:[IFlySpeechConstant LANGUAGE]];
    //set accent方言mandarin
    [iFlySpeechRecognizer setParameter:@"mandarin" forKey:[IFlySpeechConstant ACCENT]];
    
    //set whether or not to show punctuation in recognition results
    [iFlySpeechRecognizer setParameter:@"0" forKey:[IFlySpeechConstant ASR_PTT]];
    //set listener
    [iFlySpeechRecognizer setDelegate:self];
    /*
    [[NSNotificationCenter defaultCenter] addObserver:self selector:@selector(handleInterreption:) name:AVAudioSessionInterruptionNotification object:[AVAudioSession sharedInstance]];
    _played = NO;
     */
}
/*
-(void)handleInterreption:(NSNotification *)sender
{
    if (_played)
    {
        [_globalPlayer pause];
        _played = NO;
        NSLog(@"hadleInterreption: playerd = YES");
    }else{
        _played = YES;
        NSLog(@"hadleInterreption: playerd = NO");
    }
}
*/
@end


