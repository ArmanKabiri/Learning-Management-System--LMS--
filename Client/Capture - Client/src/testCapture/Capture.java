package testCapture;

public class Capture extends Thread {
	static public boolean shouldCapturingStop=false;
	
	@Override
	public void run() {
		startCapture();
	}
	
	public void startCapture(){
		try{
			CapturedPacket cPacket=new CapturedPacket();
			CaptureVoice captureVoice=new CaptureVoice(cPacket);
			CaptureScreen captureScreen=new CaptureScreen(cPacket);
			NetworkTool netTool=new NetworkTool(cPacket);
			
			captureScreen.determineBounds(.75f,10,10);
			captureScreen.start();
			captureVoice.start();
			netTool.start();
		}
		catch(Exception x){
			System.out.println(x.getLocalizedMessage());
		}
	}
}
