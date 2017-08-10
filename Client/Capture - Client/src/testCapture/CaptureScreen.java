package testCapture;

import java.awt.Dimension;
import java.awt.Graphics2D;
import java.awt.Image;
import java.awt.MouseInfo;
import java.awt.Point;
import java.awt.PointerInfo;
import java.awt.Rectangle;
import java.awt.RenderingHints;
import java.awt.Robot;
import java.awt.Toolkit;
import java.awt.Transparency;
import java.awt.image.BufferedImage;
import java.io.ByteArrayOutputStream;
import java.io.File;
import java.io.IOException;

import javax.imageio.IIOImage;
import javax.imageio.ImageIO;
import javax.imageio.ImageWriteParam;
import javax.imageio.ImageWriter;

public class CaptureScreen extends Thread{
	private Rectangle captureArea=null;
	private BufferedImage lastImgCaptured=null;
	private byte[] lastResultImg=null;
	private CapturedPacket cPacket;
	private int lastMouseLocX=0,lastMouseLocY=0;
	public CaptureScreen(CapturedPacket c){
		cPacket=c;
	}
	
	@Override
	public void run() {
		try {
			while(!Capture.shouldCapturingStop){
				//System.out.println("screen - waiting");
				cPacket.semProdImg.acquire();
				System.out.println("screen - startCapture");
				capture();
				getMouseLoc();
				cPacket.setImg(lastResultImg);
				cPacket.setMouseCordinate(lastMouseLocX, lastMouseLocY);
				System.out.println("screen - end Capture");
				cPacket.semPutToQueue.release();
			}
		}
		catch (InterruptedException e) {
			e.printStackTrace();
		}
	}
	
	private void capture(){
		BufferedImage result=null;
		byte[] resultInBytes=null;
		try {
			BufferedImage keyImg= takePicture();
			if(!are2ImagesSimilar(15, 5, keyImg,lastImgCaptured)){
				lastImgCaptured=keyImg;
				result=resizeImage(keyImg, (int)(keyImg.getWidth()*.7), (int)(keyImg.getHeight()*.7) );
				resultInBytes=compressImg(result, .3f);
			}
			
		} catch (Exception e) {
			e.printStackTrace();
		}
		if(resultInBytes==null)
			lastResultImg=null;
		else
			lastResultImg=resultInBytes.clone();
	}
	
	public void determineBounds(float ratio,int minWidth,int minHeight) throws InterruptedException{
		int width=0,height=0,x=0,y=0;
		Dimension screenSize = Toolkit.getDefaultToolkit().getScreenSize();
		int ScrWidth = (int) screenSize.getWidth();
		int scrHeight = (int) screenSize.getHeight();
		
		String dialogMessage="Select area to capture with draging mouse";
		while(width<=minWidth || height<=minHeight || x<0 || y<0 || x+width>ScrWidth || y+height>scrHeight){
			final SnipTool snip= new SnipTool(ratio,minWidth,minHeight,dialogMessage);
			Thread t= new Thread(new Runnable() {
				public void run() {
					while(snip.area == null){
						try {
							Thread.sleep(500);
						} catch (Exception e) {
							e.printStackTrace();
						}
					}
				}
			});
			
			t.start();
			t.join();
			
			captureArea=snip.area;
			height=snip.area.height;
			width=snip.area.width;
			x=snip.area.x;
			y=snip.area.y;
			
			if(width<=minWidth || height<=minHeight || x<0 || y<0 || x+width>ScrWidth || y+height>scrHeight){
				dialogMessage="Draw again. not accepted area";
				snip.lbl_Dialog.setVisible(true);
			}
		}
	}
	
	private BufferedImage takePicture() throws Exception{
		Image primaryImage = new Robot().createScreenCapture(captureArea);
		BufferedImage bufferedImg = (BufferedImage) primaryImage;	
		return bufferedImg;
	}
	
	@SuppressWarnings("unused")
	private void writeImageToFile(BufferedImage bufferedImg,String path) throws IOException{
		File file = new File(path);
		String extension = "jpg";
		ImageIO.write(bufferedImg, extension, file);
	}
	
	private byte[] compressImg(BufferedImage image,float compressionLevel) throws IOException{
		
		ByteArrayOutputStream baos = new ByteArrayOutputStream();
		ImageWriter writer = (ImageWriter) ImageIO.getImageWritersByFormatName("jpg").next();

		ImageWriteParam param = writer.getDefaultWriteParam();
		param.setCompressionMode(ImageWriteParam.MODE_EXPLICIT);
		param.setCompressionQuality(compressionLevel); // Change this, float between 0.0 and 1.0

		writer.setOutput(ImageIO.createImageOutputStream(baos));
		writer.write(null, new IIOImage(image, null, null), param);
		writer.dispose();
		BufferedImage bf=null;
		
		byte[] imageInByte;
		imageInByte = baos.toByteArray();
		baos.close();
		// convert byte array back to BufferedImage
//		InputStream in = new ByteArrayInputStream(imageInByte);
//		BufferedImage resultImg = ImageIO.read(in);
//		return resultImg;
		return imageInByte;
	}
	
	private BufferedImage resizeImage(BufferedImage img, int targetWidth, int targetHeight){
		int type = (img.getTransparency() == Transparency.OPAQUE) ? BufferedImage.TYPE_INT_RGB : BufferedImage.TYPE_INT_ARGB;
	    BufferedImage ret = img;
	    BufferedImage scratchImage = null;
	    Graphics2D g2 = null;

	    int w = img.getWidth();
	    int h = img.getHeight();

	    int prevW = w;
	    int prevH = h;

	    do {
	        if (w > targetWidth) {
	            w /= 2;
	            w = (w < targetWidth) ? targetWidth : w;
	        }

	        if (h > targetHeight) {
	            h /= 2;
	            h = (h < targetHeight) ? targetHeight : h;
	        }

	        if (scratchImage == null) {
	            scratchImage = new BufferedImage(w, h, type);
	            g2 = scratchImage.createGraphics();
	        }

	        g2.setRenderingHint(RenderingHints.KEY_INTERPOLATION,
	                RenderingHints.VALUE_INTERPOLATION_BILINEAR);
	        g2.drawImage(ret, 0, 0, w, h, 0, 0, prevW, prevH, null);

	        prevW = w;
	        prevH = h;
	        ret = scratchImage;
	    } while (w != targetWidth || h != targetHeight);

	    if (g2 != null) {
	        g2.dispose();
	    }

	    if (targetWidth != ret.getWidth() || targetHeight != ret.getHeight()) {
	        scratchImage = new BufferedImage(targetWidth, targetHeight, type);
	        g2 = scratchImage.createGraphics();
	        g2.drawImage(ret, 0, 0, null);
	        g2.dispose();
	        ret = scratchImage;
	    }
	    
	    return ret;
	}
	
	private boolean are2ImagesSimilar(int SkipPixelstep,int numAcceptedDeferences,BufferedImage... image){
		if(image[0]==null || image[1]==null)
			return false;
		else{
			int width=image[0].getWidth();
			int height=image[0].getHeight();
			int countPixelsDeferences=0;
			for (int i = 0; i < width && countPixelsDeferences<numAcceptedDeferences; i+=SkipPixelstep) {
				for (int j = 0; j < height && countPixelsDeferences<numAcceptedDeferences; j+=SkipPixelstep) {
					if(image[0].getRGB(i, j)!=image[1].getRGB(i, j)){
						countPixelsDeferences++;
					}
				}
			}
			
			return (countPixelsDeferences < numAcceptedDeferences) ? true : false;
		}
	}
	
	private void getMouseLoc(){
		PointerInfo pInfo = MouseInfo.getPointerInfo();
		Point pointer = pInfo.getLocation();
		int x = (int) pointer.getX();
		int y = (int) pointer.getY();
		if(x>=captureArea.x && y>=captureArea.y && x<= (captureArea.x + captureArea.width) && y<= (captureArea.y + captureArea.height) ){
			lastMouseLocX=x-captureArea.x;
			lastMouseLocY=y-captureArea.y;
		}
		else{
			lastMouseLocX=-1;
			lastMouseLocY=-1;
		}
		
	}
}
