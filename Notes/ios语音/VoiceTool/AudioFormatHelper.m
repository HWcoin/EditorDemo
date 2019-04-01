
#import "AudioFormatHelper.h"

typedef  struct  {
    char         fccID[4];
    int32_t      dwSize;
    int16_t      wFormatTag;
    int16_t      wChannels;
    int32_t      dwSamplesPerSec;
    int32_t      dwAvgBytesPerSec;
    int16_t      wBlockAlign;
    int16_t      uiBitsPerSample;
    
}FMT;

typedef  struct  {
    char fccID[4];
    int32_t dwSize;
    
}DATA;

//wav头的结构如下所示：
typedef  struct  {
    char        fccID[4];
    int32_t      dwSize;
    char        fccType[4];
} HEADER;

@implementation AudioFormatHelper

+(int)pcm_to_wav:(const char *)src_file targetFile:(const char *)dst_file Chanels:(int) channels Rate:(int) sample_rate
{
    int bits = 16;
    //以下是为了建立.wav头而准备的变量
    HEADER  pcmHEADER;
    FMT  pcmFMT;
    DATA  pcmDATA;
    unsigned  short  m_pcmData;
    
    FILE  *fp,*fpCpy;
    if((fp=fopen(src_file,  "rb"))  ==  NULL) //读取文件
    {
        printf("open pcm file %s error\n", src_file);
        return -1;
    }
    
    if((fpCpy=fopen(dst_file,  "wb+"))  ==  NULL) //为转换建立一个新文件
    {
        printf("create wav file error\n");
        return -1;
    }
    
    //以下是创建wav头的HEADER;但.dwsize未定，因为不知道Data的长度。
    strncpy(pcmHEADER.fccID,"RIFF",4);
    strncpy(pcmHEADER.fccType,"WAVE",4);
    fseek(fpCpy,sizeof(HEADER),1); //跳过HEADER的长度，以便下面继续写入wav文件的数据;
    //以上是创建wav头的HEADER;
    
    if(ferror(fpCpy))
    {
        printf("error\n");
    }
    
    //以下是创建wav头的FMT;
    pcmFMT.dwSamplesPerSec=sample_rate;
    pcmFMT.dwAvgBytesPerSec=pcmFMT.dwSamplesPerSec*sizeof(m_pcmData);
    pcmFMT.uiBitsPerSample=bits;
    strncpy(pcmFMT.fccID,"fmt  ", 4);
    pcmFMT.dwSize=16;
    pcmFMT.wBlockAlign=2;
    pcmFMT.wChannels=channels;
    pcmFMT.wFormatTag=1;
    //以上是创建wav头的FMT;
    
    fwrite(&pcmFMT,sizeof(FMT),1,fpCpy); //将FMT写入.wav文件;
    
    //以下是创建wav头的DATA;  但由于DATA.dwsize未知所以不能写入.wav文件
    strncpy(pcmDATA.fccID,"data", 4);
    pcmDATA.dwSize=0; //给pcmDATA.dwsize  0以便于下面给它赋值
    fseek(fpCpy,sizeof(DATA),1); //跳过DATA的长度，以便以后再写入wav头的DATA;
    fread(&m_pcmData,sizeof(int16_t),1,fp); //从.pcm中读入数据
    
    while(!feof(fp)) //在.pcm文件结束前将他的数据转化并赋给.wav;
    {
        pcmDATA.dwSize+=2; //计算数据的长度；每读入一个数据，长度就加一；
        fwrite(&m_pcmData,sizeof(int16_t),1,fpCpy); //将数据写入.wav文件;
        fread(&m_pcmData,sizeof(int16_t),1,fp); //从.pcm中读入数据
    }
    
    fclose(fp); //关闭文件
    
    pcmHEADER.dwSize = 0;  //根据pcmDATA.dwsize得出pcmHEADER.dwsize的值
    rewind(fpCpy); //将fpCpy变为.wav的头，以便于写入HEADER和DATA;
    fwrite(&pcmHEADER,sizeof(HEADER),1,fpCpy); //写入HEADER
    fseek(fpCpy,sizeof(FMT),1); //跳过FMT,因为FMT已经写入
    fwrite(&pcmDATA,sizeof(DATA),1,fpCpy);  //写入DATA;
    fclose(fpCpy);  //关闭文件
    return 0;
    
}

+(long) wav_to_amr:(NSString *)wav_file targetFormat:(NSString *) amr_file
{
    long length = [EMVoiceConverter wavToAmr:wav_file amrSavePath:amr_file];
    return length;
}

+(long) amr_to_wav:(NSString *)amr_file targetFormat:(NSString *)wav_file
{
    long length = [EMVoiceConverter amrToWav:amr_file wavSavePath:wav_file];
    return length;
}

//计算方式跟Adroid一样。
+(long) getAmrDuration:(NSString *)amr_file
{
    long duration = -1;
    int packedSize[] = {12, 13, 15, 17, 19, 20, 26, 31, 5, 0, 0, 0, 0, 0, 0, 0};
    const char * amr_path = [amr_file UTF8String];
    FILE *famr = fopen(amr_path, "rb");
    if (famr == NULL)
        return duration;
    
    fseek(famr, 0L, SEEK_END);
    long length = ftell(famr);
    fseek(famr, 0L, SEEK_SET);
    
    int pos = 6;
    int frameCount = 0;
    int packedPos = -1;
    Byte datas[1];
    while (pos < length) {
        
        fseek(famr, 0, pos);
        if (fread(datas, 1, 1, famr) != 1){
            duration = length > 0 ? ((length-6)/650) : 0;
            break;
        }
        packedPos = (datas[0] >> 3) & 0x0f;
        pos += packedSize[packedPos] + 1;
        frameCount++;
    }
    duration += frameCount * 20;
    fclose(famr);
    return duration;
}
@end
