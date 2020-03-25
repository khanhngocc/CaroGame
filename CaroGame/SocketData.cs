using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaroGame
{
    [Serializable]
    public class SocketData
    {
        private int command;

        public int Command { get => command; set => command = value; }

        private Point point;

        public Point Point { get => point; set => point = value; }
        

        public SocketData(int command,  String message , Point point)
        {
            this.command = command;
            this.point = point;
            this.messages = message;
        }
        private String messages;
        public string Messages { get => messages; set => messages = value; }
        public enum SocketCommand{
            SEND_POINT,
            NOTIFY,
            NEW_GAME,
            UNDO,
            END_GAME,
            QUIT
            
         }


    }
}
