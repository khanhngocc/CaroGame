using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CaroGame
{
    class ChessBoardManager
    {
        #region Properties
        private Panel chessBoard;

        public Panel ChessBoard
        {
            get { return chessBoard; }
            set { chessBoard = value; }

        }
        private List<Player> list_Player;
        public List<Player> List_Player { get => list_Player; set => list_Player = value; }
        private int current_player;
        public int Current_player { get => current_player; set => current_player = value; }
        private TextBox playerName;
        public TextBox PlayerName { get => playerName; set => playerName = value; }
        private PictureBox playerMark;
        public PictureBox PlayerMark { get => playerMark; set => playerMark = value; }
        

        private List<List<Button>> matrix;
        private event EventHandler<ButtonClickEvent> playerMarked;
        public event EventHandler<ButtonClickEvent> PlayerMarked
        {
            add
            {
                playerMarked += value;
            }
            remove
            {
                playerMarked -= value;
            }
        }
        private event EventHandler endGame;
        public event EventHandler EndGame
        {
            add
            {
                endGame += value;
            }
            remove
            {
                endGame -= value;
            }
        }
        private Stack<PlayInfor> playTimeLine;
        public Stack<PlayInfor> PlayTimeLine { get => playTimeLine; set => playTimeLine = value; }
        #endregion

        #region Initialize
        public ChessBoardManager(Panel chessBoard,TextBox playerName,PictureBox mark)
        {

            this.chessBoard = chessBoard;
            this.PlayerName = playerName;
            this.PlayerMark = mark;
            this.list_Player = new List<Player>() {

                new Player("PlayerX",Image.FromFile(Path.Combine (
                Path.GetDirectoryName (Assembly.GetExecutingAssembly().Location),
               "Img/x.jpg"))),
                new Player("PlayerO",Image.FromFile(Path.Combine (
                Path.GetDirectoryName (Assembly.GetExecutingAssembly().Location),
               "Img/O.png")))
            };
           
           
        }
        #endregion

        #region Methods
        public void DrawChessBoard()
        {
            chessBoard.Enabled = true;
            chessBoard.Controls.Clear();
            PlayTimeLine = new Stack<PlayInfor>();
            Current_player = 0;
            ChangePlayer();

            matrix = new List<List<Button>>();

            Button oldBtn = new Button()
            {

                Width = 0,
                Location = new Point(0, 0)

            };
            for (int i = 0; i < Cons.CHESS_BOARD_HEIGHT; i++)
            {
                matrix.Add(new List<Button>());
                for (int j = 0; j < Cons.CHESS_BOARD_WIDTH; j++)
                {
                    Button btn = new Button()
                    {

                        Width = Cons.CHESS_WIDTH,
                        Height = Cons.CHESS_HEIGHT,
                        Location = new Point(oldBtn.Location.X + oldBtn.Width, oldBtn.Location.Y),
                        BackgroundImageLayout = ImageLayout.Stretch,
                        Tag = i.ToString()
                    };
                    btn.Click += Btn_Click;
                    chessBoard.Controls.Add(btn);
                    matrix[i].Add(btn);
                    oldBtn = btn;

                }
                oldBtn.Location = new Point(0, oldBtn.Location.Y + Cons.CHESS_HEIGHT);
                oldBtn.Width = 0;
                oldBtn.Height = 0;
            }
        }
        private bool isEndgame(Button btn) {


            return isEndgameHoziontal(btn) || isEndgameVertical(btn) || isEndgamePrimary(btn) || isEndgameSub(btn);
        }
        private Point GetChessPoint(Button btn)
        {
            
            int vertical = Convert.ToInt32(btn.Tag);
            int hoziontal = matrix[vertical].IndexOf(btn);
            Point p = new Point(hoziontal,vertical);
            return p;
        }
        private bool isEndgameHoziontal(Button btn)
        {

            Point point = GetChessPoint(btn);
            int countLeft = 0;
            for (int i = point.X; i >= 0; i--)
            {
                if (matrix[point.Y][i].BackgroundImage == btn.BackgroundImage)
                {
                    countLeft++;
                }
                else
                    break;
            }
            int countRight = 0;
            for (int i = point.X + 1; i < Cons.CHESS_BOARD_WIDTH ; i++)
            {
                if (matrix[point.Y][i].BackgroundImage == btn.BackgroundImage)
                {
                    countRight++;
                }
                else
                    break;
            }
            if (countLeft + countRight == 5)
                return true;

            return false;
        }
        private bool isEndgameVertical(Button btn)
        {
            Point point = GetChessPoint(btn);
            int countTop = 0;
            for (int i = point.Y; i >= 0; i--)
            {
                if (matrix[i][point.X].BackgroundImage == btn.BackgroundImage)
                {
                    countTop++;
                }
                else
                    break;
            }
            int countBottom = 0;
            for (int i = point.Y + 1; i < Cons.CHESS_BOARD_HEIGHT; i++)
            {
                if (matrix[i][point.X].BackgroundImage == btn.BackgroundImage)
                {
                    countBottom++;
                }
                else
                    break;
            }
            if (countBottom + countTop == 5)
                return true;

            return false;

            
        }
        private bool isEndgamePrimary(Button btn)
        {
            Point point = GetChessPoint(btn);
            int countTop = 0;

            for (int i = 0;i<=point.X; i++)
            {
                if (point.X - i < 0 || point.Y - i < 0)
                    break;
                if (matrix[point.Y-i][point.X-i].BackgroundImage == btn.BackgroundImage)
                {
                    countTop++;
                }
                else
                    break;
            }
            int countBottom = 0;
            for (int i = 1; i <= Cons.CHESS_BOARD_WIDTH - point.X; i++)
            {
                if (point.Y + i >= Cons.CHESS_BOARD_HEIGHT|| point.X + i >= Cons.CHESS_BOARD_WIDTH)
                    break;
                if (matrix[point.Y + i][point.X + i].BackgroundImage == btn.BackgroundImage)
                {
                    countBottom++;
                }
                else
                    break;
            }
            if (countBottom + countTop == 5)
                return true;

           

            return false;
        }
        private bool isEndgameSub(Button btn)
        {
            Point point = GetChessPoint(btn);
            int countTop = 0;

            for (int i = 0; i <= point.X; i++)
            {
                if (point.X + i > Cons.CHESS_BOARD_WIDTH || point.Y - i < 0)
                    break;
                if (matrix[point.Y - i][point.X + i].BackgroundImage == btn.BackgroundImage)
                {
                    countTop++;
                }
                else
                    break;
            }
            int countBottom = 0;
            for (int i = 1; i <= Cons.CHESS_BOARD_WIDTH - point.X; i++)
            {
                if (point.Y + i >= Cons.CHESS_BOARD_HEIGHT || point.X - i < 0)
                    break;
                if (matrix[point.Y + i][point.X - i].BackgroundImage == btn.BackgroundImage)
                {
                    countBottom++;
                }
                else
                    break;
            }
            if (countBottom + countTop == 5)
                return true;

            return false;
        }
        private void Btn_Click(object sender, EventArgs e)
        {
            Button btn = sender as Button;
            if (btn.BackgroundImage != null)
                return;
            Mark(btn);
            PlayTimeLine.Push(new PlayInfor(GetChessPoint(btn),Current_player));

            if (Current_player == 0)
                Current_player = 1;
            else
                Current_player = 0;

            ChangePlayer();

            if (playerMarked != null)
                playerMarked(this, new ButtonClickEvent(GetChessPoint(btn)));

            if (isEndgame(btn) == true)
            {
                WinGame();
            }

        }
        public void OtherPlayerMark(Point point)
        {
            Point markPoint = point;

            Button btn = matrix[point.Y][point.X];
            if (btn.BackgroundImage != null)
                return;
           
            Mark(btn);
            PlayTimeLine.Push(new PlayInfor(GetChessPoint(btn), Current_player));

            if (Current_player == 0)
                Current_player = 1;
            else
                Current_player = 0;

            ChangePlayer();

    

            if (isEndgame(btn) == true)
            {
                WinGame();
            }

        }
        public void WinGame()
        {
            if (endGame != null)
            {
                endGame(this, new EventArgs());
            }
        }
        private void Mark(Button btn) {
            btn.BackgroundImage = list_Player[Current_player].Mark;
          
        }
        private void ChangePlayer() {

            PlayerName.Text = list_Player[Current_player].Name_player;
            PlayerMark.Image = list_Player[Current_player].Mark;

        }
      
        #endregion
    }

    public class ButtonClickEvent : EventArgs
    {
        private Point clicked;

        public Point Clicked { get => clicked; set => clicked = value; }

        public ButtonClickEvent(Point point)
        {
            this.clicked = point;
        }
    }
}
