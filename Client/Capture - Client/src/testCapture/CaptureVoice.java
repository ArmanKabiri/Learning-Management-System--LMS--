package testCapture;

import javax.sound.sampled.AudioFormat;
import javax.sound.sampled.AudioSystem;
import javax.sound.sampled.DataLine;
import javax.sound.sampled.SourceDataLine;
import javax.sound.sampled.TargetDataLine;

public class CaptureVoice extends Thread{
	//private SourceDataLine sourceDataLine;
	//private DataLine.Info sourceDataLineInfo;
	private DataLine.Info targetDataLineInfo;
	private TargetDataLine targetDataLine;
	private byte[] lastVoiceResult=null;
	private CapturedPacket cPacket;
	
	public CaptureVoice(CapturedPacket c){
		try{
			cPacket=c;
			//sourceDataLineInfo = new DataLine.Info( SourceDataLine.class,getAudioFormat());
			targetDataLineInfo = new DataLine.Info( TargetDataLine.class,getAudioFormat());
			targetDataLine = (TargetDataLine)AudioSystem.getLine(targetDataLineInfo);
			//sourceDataLine = (SourceDataLine)AudioSystem.getLine(sourceDataLineInfo);
		}
		catch(Exception e){
			System.out.println(e.getLocalizedMessage());
		}
	}
	
	@Override
	public void run() {
		try {
			openDataLine();
			while(!Capture.shouldCapturingStop){
				//System.out.println("voice - waiting");
				cPacket.semProdVoice.acquire();
				System.out.println("voice - start recording");
				record();
				
//				new Thread(new Runnable() {
//					@Override
//					public void run() {
//						toSpeaker();
//					}
//				});
				
				cPacket.setVoice(lastVoiceResult);
				System.out.println("voice - end recording");
				cPacket.semPutToQueue.release();
			}
			closeDataLine();
		}
		catch (Exception e) {
			e.printStackTrace();
		}
	}
	
	
	public void openDataLine(){
		try{
			//sourceDataLine.open(getAudioFormat());
			targetDataLine.open( getAudioFormat() );
			targetDataLine.start();
			//sourceDataLine.start();
		}
		catch(Exception e){
			System.out.println(e.getLocalizedMessage());
		}
	}
	
	public void closeDataLine(){
		try{
			//sourceDataLine.stop();
			targetDataLine.stop();
		}
		catch(Exception e){
			System.out.println(e.getLocalizedMessage());
		}
	}
	
	private void capture_play(){
		try{
			//sourceDataLine.open(getAudioFormat());
			targetDataLine.open( getAudioFormat() );
			targetDataLine.start();
			//sourceDataLine.start();
			while(!Capture.shouldCapturingStop){
				record();
				Thread t=new Thread(new Runnable() {
					@Override
					public void run() {
						toSpeaker();
					}
				});
				t.start();
			}
			//sourceDataLine.stop();
			targetDataLine.stop();
		}
		catch(Exception e){
			System.out.println(e.getLocalizedMessage());
		}
	}
	
	private AudioFormat getAudioFormat(){
	    float sampleRate = 8000.0F;
	    //8000,11025,16000,22050,44100
	    int sampleSizeInBits = 16;
	    //8,16
	    int channels = 1;
	    //1,2
	    boolean signed = true;
	    boolean bigEndian = false;
	    return new AudioFormat( sampleRate, sampleSizeInBits, channels, signed, bigEndian );
	}
	
	public void record(){
		byte tempBuffer[]=null;
		try{
			tempBuffer = new byte[16000];
			targetDataLine.read( tempBuffer , 0 , tempBuffer.length );
		}
		catch(Exception e){
			System.out.println(e.getLocalizedMessage());	
		}
		lastVoiceResult = tempBuffer;
	}
	
	public void toSpeaker(){
	    try{
			//sourceDataLine.write( lastVoiceResult , 0, lastVoiceResult.length );
			//sourceDataLine.drain() ;
	     }
	     catch(Exception e){
	    	 	System.out.println(e.getLocalizedMessage());	
	     }
	}
}
