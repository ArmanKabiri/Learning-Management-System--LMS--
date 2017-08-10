package testCapture;

import java.io.ByteArrayInputStream;
import java.io.ByteArrayOutputStream;
import java.io.IOException;
import java.io.ObjectInputStream;
import java.io.ObjectOutputStream;
import java.io.Serializable;
import java.util.concurrent.Semaphore;

import wox.serial.Easy;

public class CapturedPacket implements Cloneable , Serializable{

	private int ID=0;
    private int CourseID=10;
    private int SequenceNumber=0;
    private byte[] Img=null;
    private byte[] Voice=null;
    private int LocxMouse=0;
    private int LocyMouse=0;
    
    // We must start with Producer semaphore
    // Start with consumer semaphore unavailable.
    public Semaphore semProdImg = new Semaphore(1);
    public Semaphore semProdVoice = new Semaphore(1);
    
    public Semaphore semSendData = new Semaphore(0);
    public Semaphore semPutToQueue = new Semaphore(0);
    
    @Override
    protected CapturedPacket clone() throws CloneNotSupportedException {
    	return (CapturedPacket) super.clone();
    }
    
    public CapturedPacket(){}
    
    public void incSequenceNumber(){
    	SequenceNumber++;
    }
    
    public int getSequenceNumber(){
    	return SequenceNumber;
    }
    
    public int getID(){
    	return ID;
    }
    
    public int getCourseID(){
    	return CourseID;
    }
    
    public int getLocXMouse(){
    	return LocxMouse;
    }
    
    public int getLocYMouse(){
    	return LocyMouse;
    }
    
    public byte[] getImg(){
    	return Img;
    }
    
    public byte[] getImgWithSize(){
    	if (Img==null)
    		Img = new byte[1];
    	byte[] ImgLen = String.valueOf(Img.length+"*").getBytes();
        byte[] combined = new byte[ImgLen.length+Img.length];
    	System.arraycopy(ImgLen,0,combined,0,ImgLen.length);
    	System.arraycopy(Img,0,combined,ImgLen.length,Img .length);
    	return combined;
    }
    
    public byte[] getVoiceWithSize(){
    	if (Voice==null)
    		Voice = new byte[1];
    	byte[] VoiceLen = String.valueOf(Voice.length+"*").getBytes();
        byte[] combined = new byte[VoiceLen.length+Voice.length];
    	System.arraycopy(VoiceLen,0,combined,0,VoiceLen.length);
    	System.arraycopy(Voice,0,combined,VoiceLen.length,Voice .length);
    	return combined;
    }
    
    public byte[] getVoice(){
    	return Voice;
    }
    
    public void setMouseCordinate(int x,int y){
    	LocxMouse=x;
    	LocyMouse=y;
    }
    
    public void setImg(byte[] img){
    	if(img==null)
    		Img=null;
    	else
    		Img=img.clone();
    }
    
    public void setVoice(byte[] voice){
    	if(voice==null)
    		Voice=null;
    	else
    		Voice=voice.clone();
    }
    
    public String serialize(){
    	StringBuilder strSnapShot =new StringBuilder();
        strSnapShot.append("{\"ID\":\"").append(ID).append("\",")
            .append("\"CourseID\":\"").append(CourseID).append("\",")
            .append("\"SequenceNumber\":\"").append(SequenceNumber).append("\",")
            .append("\"Img\":\"").append( (Img==null)? "" : new String(Img) ).append("\",")
            .append("\"Voice\":\"").append( (Voice==null) ? null : new String(Voice) ).append("\",")
            .append("\"LocxMouse\":\"").append(LocxMouse).append("\",")
            .append("\"LocyMouse\":\"").append(LocyMouse).append("\"")
            .append("}");
        int a=strSnapShot.length();
        int b=strSnapShot.toString().getBytes().length;
        System.out.println("packet "+ (SequenceNumber) +" -- size: "+a);
        return strSnapShot.toString();
    }
    
    public String serialize_IntValues(){
    	StringBuilder strSnapShot =new StringBuilder();
        strSnapShot.append("{\"ID\":\"").append(ID).append("\",")
            .append("\"CourseID\":\"").append(CourseID).append("\",")
            .append("\"SequenceNumber\":\"").append(SequenceNumber).append("\",")
            .append("\"LocxMouse\":\"").append(LocxMouse).append("\",")
            .append("\"LocyMouse\":\"").append(LocyMouse).append("\"")
            .append("}");
    	StringBuilder strSnapShotFinal =new StringBuilder();
    	strSnapShotFinal.append(strSnapShot.length()).append("*").append(strSnapShot.toString());
        System.out.println("packet "+ (SequenceNumber) +" -- size: " + strSnapShot.length());
        return strSnapShotFinal.toString();
    }
    
   
    public byte[] getBytes() throws IOException{
        ByteArrayOutputStream baos = new ByteArrayOutputStream();
        ObjectOutputStream oos = new ObjectOutputStream(baos);
        oos.writeObject(this);
        byte[] buf = baos.toByteArray();
        return buf;
    }
    
    public static CapturedPacket readBytes(byte[] buf) throws Exception{
    	ObjectInputStream ois = new ObjectInputStream(new ByteArrayInputStream(buf));
		CapturedPacket c = (CapturedPacket) ois.readObject();
		ois.close();
		return c;
    }
    
    public void saveWox(){
    	Easy.save(this, "/file/a.xml");
    	CapturedPacket c=(CapturedPacket) Easy.load("/file/a.xml");
    }
}
