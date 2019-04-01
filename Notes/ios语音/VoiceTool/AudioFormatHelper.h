#import <Foundation/Foundation.h>
#include <stdio.h>
#include <string.h>
#import "EMVoiceConverter.h"

//#define __YXXDC__PCM2Wav__
typedef unsigned long       DWORD;
typedef unsigned char       BYTE;
typedef unsigned short      WORD;

@interface AudioFormatHelper : NSObject

//int a_law_pcm_to_wav(const char *pcm_file, const char *wav);
+(int)pcm_to_wav:(const char *)src_file targetFile:(const char *)dst_file Chanels:(int) channels Rate:(int) sample_rate;
+(long) wav_to_amr:(NSString *)wav_file targetFormat:(NSString *) amr_file;
+(long) amr_to_wav:(NSString *)amr_file targetFormat:(NSString *) wav_file;
+(long) getAmrDuration:(NSString *)amr_file;
@end



 

 
 
 

 

 
