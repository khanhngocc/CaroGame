using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CaroGame
{
    public partial class Form1 : Form
    {
        #region Properties
        ChessBoardManager chessBoard;
        SocketManager socket;
        #endregion
        public Form1()
        {
            InitializeComponent();
            Control.CheckForIllegalCrossThreadCalls = false;
            chessBoard = new ChessBoardManager(pnlChessBoard, txtBox_playername, pB_mark);
            chessBoard.EndGame += chessBoard_EndGame;
            chessBoard.PlayerMarked += chessBoard_PlayerMarked;

            progressBar.Step = Cons.COOL_DOWN_STEP;
            progressBar.Maximum = Cons.COOL_DOWN_TIME;
            progressBar.Value = 0;
            timer.Interval = Cons.COOL_DOWN_INTERVAL;
            socket = new SocketManager();
            NewGame();

        }
        #region Method
        void NewGame()
        {
            progressBar.Value = 0;
            timer.Stop();
        
            chessBoard.DrawChessBoard();
            
        }
        
        void Quit()
        {
            Application.Exit();
        }
        void WinGame() {
            MessageBox.Show("Win Game!!!");

        }

        void EndGame()
        {
            timer.Stop();
            pnlChessBoard.Enabled = false;
           
            MessageBox.Show("Finished!!!");
        }
        private void chessBoard_PlayerMarked(object sender, ButtonClickEvent e)
        {
            timer.Start();
            pnlChessBoard.Enabled = false;
            progressBar.Value = 0;
            socket.Send(new SocketData((int) SocketData.SocketCommand.SEND_POINT,"" ,e.Clicked));
         
            Listen();
        }

        private void chessBoard_EndGame(object sender, EventArgs e)
        {
            EndGame();
        }

        private void timer_Tick(object sender, EventArgs e)
        {
            progressBar.PerformStep();

            if (progressBar.Value >= progressBar.Maximum)
            {
                
               
                EndGame();
            }
        }

        private void newGameToolStripMenuItem_Click(object sender, EventArgs e)
        {
            NewGame();
            socket.Send(new SocketData((int)SocketData.SocketCommand.NEW_GAME, "", new Point()));
            pnlChessBoard.Enabled = true;
        }

        private void undoToolStripMenuItem_Click(object sender, EventArgs e)
        {
          //  UnDo();
        }

        private void quitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Quit();
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (MessageBox.Show("Do you want to exit?", "Notification", MessageBoxButtons.OKCancel) != System.Windows.Forms.DialogResult.OK)
                      e.Cancel = true;
            else
            {
                try {
                    socket.Send(new SocketData((int)SocketData.SocketCommand.QUIT, "", new Point()));
                }
                catch
                {

                }
            }
               
        }

        private void button1_Click(object sender, EventArgs e)
        {
            socket.IP = txtIP.Text;

            if (!socket.ConnectServer())
            {
                socket.isServer = true;
                pnlChessBoard.Enabled = true;
                socket.CreateServer();
                
            }
            else
            {
                socket.isServer = false;
                pnlChessBoard.Enabled = false;
                Listen();
               
            }

        }

        private void Form1_Shown(object sender, EventArgs e)
        {
            txtIP.Text = socket.GetLocalIPv4(NetworkInterfaceType.Wireless80211);

            if (string.IsNullOrEmpty(txtIP.Text))
            {
                txtIP.Text = socket.GetLocalIPv4(NetworkInterfaceType.Ethernet);
            }
        }

        public void Listen()
        {
            
                Thread listenThread = new Thread(() => {
                    try
                    {
                        SocketData data = (SocketData)socket.Receive();
                        ProcessData(data);
                    }
                    catch
                    {

                    }
                    
                });

                listenThread.IsBackground = true;
                listenThread.Start();
            
            
        



        }

        private void ProcessData(SocketData data)
        {
            switch (data.Command)
            {
                case (int)SocketData.SocketCommand.NOTIFY:
                    MessageBox.Show(data.Messages);
                    break;
                case (int)SocketData.SocketCommand.SEND_POINT:
                    this.Invoke(
                        
                        (MethodInvoker)(() =>

                        {
                            progressBar.Value = 0;
                            pnlChessBoard.Enabled = true;
                            timer.Start();
                            chessBoard.OtherPlayerMark(data.Point);
                           
                            
                           
                        }
                        
                        ));
                  
                    break;
                case (int)SocketData.SocketCommand.UNDO:
                    
                    break;
                case (int)SocketData.SocketCommand.NEW_GAME:
                    this.Invoke(

                        (MethodInvoker)(() =>

                        {
                            NewGame();
                            pnlChessBoard.Enabled = false;
                        }

                        ));
                  
                    break;
                case (int)SocketData.SocketCommand.END_GAME:

                    break;
                case (int)SocketData.SocketCommand.QUIT:
                    timer.Stop();
                    MessageBox.Show("Opponent has been gone");
                    break;
                default:
                    break;

            }
            Listen();
        }
        #endregion


    }
}
