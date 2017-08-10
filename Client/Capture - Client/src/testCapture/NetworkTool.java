package testCapture;

import java.io.BufferedReader;
import java.io.IOException;
import java.io.InputStream;
import java.io.InputStreamReader;
import java.io.OutputStreamWriter;
import java.net.DatagramPacket;
import java.net.DatagramSocket;
import java.net.HttpURLConnection;
import java.net.InetAddress;
import java.net.URL;
import java.util.ArrayList;
import java.util.LinkedList;
import java.util.List;
import java.util.Queue;

import org.apache.http.HttpEntity;
import org.apache.http.HttpResponse;
import org.apache.http.NameValuePair;
import org.apache.http.client.HttpClient;
import org.apache.http.client.entity.UrlEncodedFormEntity;
import org.apache.http.client.methods.HttpGet;
import org.apache.http.client.methods.HttpPost;
import org.apache.http.entity.mime.content.ByteArrayBody;
import org.apache.http.impl.client.DefaultHttpClient;
import org.apache.http.message.BasicNameValuePair;
import org.apache.http.params.BasicHttpParams;
import org.apache.http.params.HttpConnectionParams;
import org.apache.http.params.HttpParams;
import org.apache.http.protocol.HTTP;
import org.apache.http.util.ByteArrayBuffer;

public class NetworkTool extends Thread{
	public CapturedPacket cPacket=null;
	public final String webServiceUrl="localhost:65435/Service_LMS.asmx/ServerReceiveData";
	public Queue<CapturedPacket> packetQueue;
	
	@Override
	public void run() {
		try{
			new Thread(new Runnable() {
				public void run() {
					uploadToServer();
				}
			}).start();
			
			while(!Capture.shouldCapturingStop){
				cPacket.semPutToQueue.acquire();
				cPacket.semPutToQueue.acquire();
				cPacket.incSequenceNumber();
				System.out.println("net - data added to Queue "+"sequnce:"+cPacket.getSequenceNumber());
				packetQueue.add(cPacket.clone());
				cPacket.semSendData.release();
				cPacket.semProdImg.release();
				cPacket.semProdVoice.release();
			}
		}
		catch (Exception e) {
			e.printStackTrace();
		}
	}
	
	public NetworkTool(CapturedPacket c){
		packetQueue=new LinkedList<CapturedPacket>();
		cPacket=c;
	}
	
	@SuppressWarnings({ "deprecation", "resource", "unused" })
	private InputStream openHttpConnectionPOST(NameValuePair...nameValuePairs) throws Exception{
		InputStream in=null;
		HttpParams httpParams=new BasicHttpParams();
		HttpConnectionParams.setConnectionTimeout(httpParams, 10000);
		HttpConnectionParams.setSoTimeout(httpParams, 10000);
		HttpClient httpclient = new DefaultHttpClient(httpParams);
		HttpPost httppost = new HttpPost(webServiceUrl);
		
		List<NameValuePair> params = new ArrayList<NameValuePair>(nameValuePairs.length);
		for( int i=0;i< nameValuePairs.length;i++){
			params.add(nameValuePairs[i]);
		}
		
		
		byte[] b=new byte[500000];
		int a;
		UrlEncodedFormEntity urlEncoded;
		
		urlEncoded=new UrlEncodedFormEntity(params,HTTP.ASCII);
		a=urlEncoded.getContent().read(b);
		
		System.out.println("final size: "+a);
		
		httppost.setEntity(urlEncoded);
		
		
		HttpResponse response = httpclient.execute(httppost);
		HttpEntity entity = response.getEntity();

		if (entity != null) {
		    in = entity.getContent();
		}
		else{
			throw new IOException("openHttpConnectionPost : entity was Null");
		}
		return in;
	}
	
	private void openHttpConnectionPOST2(String s) throws Exception{
		String data ="snapShot = "+s;

	    URL url = new URL(webServiceUrl);
	    HttpURLConnection conn = (HttpURLConnection) url.openConnection();
	    conn.setRequestMethod("POST");
	    conn.setDoOutput(true);
	    OutputStreamWriter wr = new OutputStreamWriter(conn.getOutputStream());
	    wr.write(data);
	    wr.flush();

	    BufferedReader rd = new BufferedReader(new InputStreamReader(conn.getInputStream()));
	    String line;
	    while ((line = rd.readLine()) != null) {
	      System.out.println(line);
	    }
	    wr.close();
	    rd.close();
	}
		
	@SuppressWarnings({ "deprecation", "resource", "unused" })
	private InputStream openHttpConnectionGET(String s) throws IOException{
		InputStream in=null;
		try{
			HttpParams httpParams=new BasicHttpParams();
			HttpConnectionParams.setConnectionTimeout(httpParams, 15000);
			HttpConnectionParams.setSoTimeout(httpParams, 15000);
			HttpClient httpclient = new DefaultHttpClient(httpParams);
			HttpGet httpGet = new HttpGet(webServiceUrl+s);
			HttpResponse response = httpclient.execute(httpGet);
			HttpEntity entity = response.getEntity();
			if (entity != null) {
			    in = entity.getContent();
			}
			else{
				throw new IOException("openHttpConnectionGet : entity was Null");
			}
		}
		catch (Exception e) {
			System.out.println(e.getMessage());
			e.printStackTrace();
		}
		return in;
	}
	
	private void sendThruUDP(byte packet[]) throws Exception{
		DatagramSocket socket = new DatagramSocket();
		DatagramPacket datagramPacket = new DatagramPacket( packet, packet.length );
		InetAddress remote_addr = InetAddress.getByName("localhost");           
		datagramPacket.setAddress( remote_addr );
		datagramPacket.setPort(8888);
		socket.send( datagramPacket );
		socket.close();
    }
	
	private void sendThruUDP2(CapturedPacket packet) throws Exception{
		DatagramSocket socket = new DatagramSocket();
		InetAddress remote_addr = InetAddress.getByName("localhost");
		DatagramPacket datagramPacket = new DatagramPacket( new byte[1] , 1);
		datagramPacket.setAddress( remote_addr );
		datagramPacket.setPort(8888);
		byte[] dataPacketSpec=null;
		byte[] dataImg=null;
		byte[] dataVoice=null;
		
		int packetSize=0;
		
		dataPacketSpec=packet.serialize_IntValues().getBytes();
	//	datagramPacket.setData(dataPacketSpec, 0, dataPacketSpec.length);
		packetSize+=dataPacketSpec.length;
       // socket.send( datagramPacket );
        
		dataImg=packet.getImgWithSize();
        if(dataImg==null)
        	dataImg=new byte[1];
   //     datagramPacket.setData(dataImg, 0, dataImg.length);
        packetSize+=dataImg.length;
        //socket.send( datagramPacket );
        
        dataVoice=packet.getVoiceWithSize();
        if(dataVoice==null)
        	dataVoice=new byte[1];  
        packetSize+=dataVoice.length;
        byte[] combined = new byte[packetSize];
        System.arraycopy(dataPacketSpec,0,combined,0,dataPacketSpec.length);
        System.arraycopy(dataImg,0,combined,dataPacketSpec.length,dataImg.length);
        System.arraycopy(dataVoice,0,combined,dataPacketSpec.length+dataImg.length,dataVoice.length);
        datagramPacket.setData(combined, 0, combined.length);
      
    
        socket.send( datagramPacket );
        
        System.out.println("final packet " + packet.getSequenceNumber() + " size is : "+packetSize);
		socket.close();
    }
	
	private void uploadToServer(){
		while(!Capture.shouldCapturingStop || !packetQueue.isEmpty()){
			try{
				cPacket.semSendData.acquire();
				CapturedPacket cpToSend=packetQueue.poll();
				
				System.out.println("net - uploading data "+"sequnce:"+cpToSend.getSequenceNumber());
				//NameValuePair valuePair=new BasicNameValuePair("packet", cpToSend.serialize());
				//InputStream in=openHttpConnectionPOST(valuePair);
				sendThruUDP2(cpToSend);
				
				System.out.println("net - packet uploaded "+"sequnce:"+cpToSend.getSequenceNumber());
			}
			catch (Exception e) {
				System.out.println(e.getMessage());
				e.printStackTrace();
			}
		}
	}
}
