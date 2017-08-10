package testCapture;

import java.awt.BorderLayout;
import java.awt.EventQueue;
import java.awt.event.ActionEvent;
import java.awt.event.ActionListener;

import javax.swing.JButton;
import javax.swing.JFrame;
import javax.swing.JPanel;
import javax.swing.JLabel;
import java.awt.Font;
import java.awt.Color;
import javax.swing.SwingConstants;
import javax.swing.UIManager;

public class MainFrame {
	boolean isCapturing=false;
	private JFrame frame;

	public static void main(String[] args) {
		EventQueue.invokeLater(new Runnable() {
			public void run() {
				try {
					MainFrame window = new MainFrame();
					window.frame.setVisible(true);
				} catch (Exception e) {
					e.printStackTrace();
				}
			}
		});
	}

	public MainFrame() {
		initialize();
	}

	private void initialize() {
		try{
			
			frame = new JFrame();
			frame.setTitle("Prof-capture");
			frame.setName("Prof-capture");
			frame.setBounds(100, 100, 300, 300);
			frame.setDefaultCloseOperation(JFrame.EXIT_ON_CLOSE);
			
			JPanel panel = new JPanel();
			frame.getContentPane().add(panel, BorderLayout.CENTER);
			final Capture capture= new Capture();
			
			final JButton btn_capture = new JButton("Start Capture");
			btn_capture.setBounds(58, 26, 200, 29);
			btn_capture.addActionListener(new ActionListener() {
				public void actionPerformed(ActionEvent e) {
					
					if(!isCapturing){
						frame.setExtendedState(JFrame.ICONIFIED);//minimize
						try {
							//capture.sleep(500);
							capture.start();
							btn_capture.setText("Stop capture");
							isCapturing=true;
							
						}
						catch(Exception x){
							System.out.println(x.getLocalizedMessage());
						}
					}
					
					else{
						isCapturing=false;
						Capture.shouldCapturingStop=true;
						capture.stop();
						btn_capture.setText("Start capture");
					}
				}
			});
			panel.setLayout(null);
			
			panel.add(btn_capture);
		}
		catch(Exception e){}
		
	}
}
