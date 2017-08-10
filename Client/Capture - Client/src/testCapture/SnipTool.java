package testCapture;
import java.awt.BorderLayout;
import java.awt.Color;
import java.awt.EventQueue;
import java.awt.Font;
import java.awt.Graphics;
import java.awt.Graphics2D;
import java.awt.GraphicsDevice;
import java.awt.GraphicsEnvironment;
import java.awt.Point;
import java.awt.Rectangle;
import java.awt.Robot;
import java.awt.event.MouseAdapter;
import java.awt.event.MouseEvent;
import java.awt.geom.Area;

import javax.swing.JFrame;
import javax.swing.JLabel;
import javax.swing.JPanel;
import javax.swing.SwingConstants;
import javax.swing.SwingUtilities;
import javax.swing.UIManager;
import javax.swing.UnsupportedLookAndFeelException;

public class SnipTool {
	public Rectangle area=null;
	private float ratioY_over_X=.75f; 	//ex: .75 : 800:600
	private int minAllowedHeight,minAllowedWidth;
	private boolean shouldDrawArea=true;
	private JFrame jf=null;
	public JLabel lbl_Dialog;
	
    public SnipTool(float ratio,int minWidth,int minHeight,final String dialogMessage) {
    	ratioY_over_X=ratio;
    	minAllowedHeight=minHeight;
    	minAllowedWidth=minWidth;
    	
        EventQueue.invokeLater(new Runnable() {
            @Override
            public void run() {
                try {
                    UIManager.setLookAndFeel(UIManager.getSystemLookAndFeelClassName());
                } catch (ClassNotFoundException | InstantiationException | IllegalAccessException | UnsupportedLookAndFeelException ex) {}

                JFrame frame = new JFrame("SnipFrame");
                jf=frame;
                frame.setUndecorated(true);
                frame.setBackground(new Color(60, 60, 70,50));
                frame.setDefaultCloseOperation(JFrame.EXIT_ON_CLOSE);
                frame.setLayout(new BorderLayout());
                CapturePane cPane=new CapturePane();
                
                Rectangle bounds = getVirtualBounds();
                
                lbl_Dialog = new JLabel(dialogMessage);
                lbl_Dialog.setForeground(Color.cyan);
                lbl_Dialog.setFont(new Font("Lao Sangam MN", Font.BOLD, 30));
                lbl_Dialog.setBounds( (int)(bounds.width*(.1)) , (int)(bounds.height*(.7)) , (int)(bounds.width*(.8)), (int)(bounds.height*(.2)) );
                lbl_Dialog.setHorizontalAlignment(SwingConstants.CENTER);
    			frame.add(lbl_Dialog);
    			
                frame.add(cPane);
                frame.setLocation(bounds.getLocation());
                frame.setSize(bounds.getSize());
                frame.setAlwaysOnTop(true);
                frame.setVisible(true);
            }
        });
    }
    
    public class CapturePane extends JPanel {

        private Rectangle selectionBounds;
        private Point clickPoint;

        public CapturePane(){
            setOpaque(false);
            MouseAdapter mouseHandler = new MouseAdapter() {
                @Override
                public void mouseClicked(MouseEvent e) {
                    if (SwingUtilities.isLeftMouseButton(e) && e.getClickCount() == 2) {
                        //System.exit(0);
                    }
                }

                @Override
                public void mousePressed(MouseEvent e) {
                    clickPoint = e.getPoint();
                    lbl_Dialog.setVisible(false);
                    //selectionBounds = null;
                }

                @Override
                public void mouseReleased(MouseEvent e) {
                	if(selectionBounds.width>minAllowedWidth && selectionBounds.height>minAllowedHeight && shouldDrawArea==true){
                		shouldDrawArea=false;
                		lbl_Dialog.setText("Now you can relocate selected area");
                		lbl_Dialog.setVisible(true);
                	}
                	else{
                		lbl_Dialog.setVisible(false);
	                    clickPoint = null;
	                    area=new Rectangle(selectionBounds);
	                    jf.setVisible(false);
                	}
                }

                @Override
                public void mouseDragged(MouseEvent e) {
                	try{
                		if(shouldDrawArea==true){
		                    Point dragPoint = e.getPoint();
		                    if(dragPoint.x<clickPoint.x || dragPoint.y<clickPoint.y)
		                    	new Robot().mouseMove(clickPoint.x, clickPoint.y );
		                    else
		                    	new Robot().mouseMove(dragPoint.x, (int)(clickPoint.y + (dragPoint.x - clickPoint.x)*(ratioY_over_X)) );
		                    selectionBounds = new Rectangle(clickPoint.x, clickPoint.y, dragPoint.x - clickPoint.x, dragPoint.y - clickPoint.y);
		                    repaint();
                		}
                		else{
                			Point dragPoint = e.getPoint();
                			int changeX=dragPoint.x-clickPoint.x;
                			int changeY=dragPoint.y-clickPoint.y;
                			selectionBounds = new Rectangle(selectionBounds.x+changeX , selectionBounds.y+changeY , selectionBounds.width , selectionBounds.height);
		                    clickPoint.x=dragPoint.x;
		                    clickPoint.y=dragPoint.y;
                			repaint();
                		}
                	}
                	catch(Exception x){
                		x.printStackTrace();
                		System.out.println(x.getMessage());
                	}
                }
            };

            addMouseListener(mouseHandler);
            addMouseMotionListener(mouseHandler);   
        }

        @Override
        protected void paintComponent(Graphics g) {
            super.paintComponent(g);
            Graphics2D g2d = (Graphics2D) g.create();
            g2d.setColor(new Color(30, 30, 30,220));
      
            Area fill = new Area(new Rectangle(new Point(0, 0), getSize()));
            if (selectionBounds != null) {
                fill.subtract(new Area(selectionBounds));
            }
            g2d.fill(fill);
            if (selectionBounds != null) {
                g2d.setColor(Color.BLUE);
                g2d.draw(selectionBounds);
            }
            g2d.dispose();
        }
    }

    public static Rectangle getVirtualBounds() {
        Rectangle bounds = new Rectangle(0, 0, 0, 0);

        GraphicsEnvironment ge = GraphicsEnvironment.getLocalGraphicsEnvironment();
        GraphicsDevice lstGDs[] = ge.getScreenDevices();
        for (GraphicsDevice gd : lstGDs) {
            bounds.add(gd.getDefaultConfiguration().getBounds());
        }
        return bounds;
    }
}