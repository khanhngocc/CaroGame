using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaroGame
{
    public class Player
    {
        private String name_player;
        public string Name_player { get => name_player; set => name_player = value; }
        private Image mark;
        public Image Mark { get => mark; set => mark = value; }
       
        public Player(String name, Image mark) {

            this.name_player = name;
            this.mark = mark;

        }
       


    }
}
