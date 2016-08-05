using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Reflection;

namespace FFgameUI
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        Image currentImage = Image.FromFile(@"C:\UIimg\IMG_0159.jpg"); //背景画像読み込み
        private void panel1_Paint(object sender, PaintEventArgs e)
        {
            if (currentImage != null)
            {
                //DrawImageメソッドで画像を座標(0, 0)の位置に表示する
                e.Graphics.DrawImage(currentImage,
                    0, 0, currentImage.Width, currentImage.Height);
            }
        }


    }
}
