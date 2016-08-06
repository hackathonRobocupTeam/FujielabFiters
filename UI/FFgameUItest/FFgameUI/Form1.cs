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
using System.Threading;
using System.Media;

namespace FFgameUI
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }


        Image backgroundImage = Image.FromFile(@"C:\UIimg\IMG_0159.jpg"); //背景画像読み込み
 
        private void panel1_Paint(object sender, PaintEventArgs e) //背景画像表示
        {
            if (backgroundImage != null)
            {
                //DrawImageメソッドで画像を座標(0, 0)の位置に表示する
                e.Graphics.DrawImage(backgroundImage,
                    0, 0, backgroundImage.Width, backgroundImage.Height);
            }
        }

        private async void button1_Click(object sender, EventArgs e) //スタートボタンで初期化
        {
            SoundPlayer gong = new SoundPlayer(@"C:\UIimg\gong.wav");
            progressBar1.Value = 100; //player1 HP
            progressBar2.Value = 100; //player2 HP
            progressBar3.Value = 0; //player1 MP
            progressBar4.Value = 0; //player2 MP
            button1.Text = "Ready...";
            await Task.Run(() =>
            {
                Thread.Sleep(2000);
            });
            button1.Text = "Fight!!!";
            gong.Play();


            
        }


    }
}
