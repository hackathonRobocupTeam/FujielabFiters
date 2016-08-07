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
using KinokoLib;
using System.Globalization;

namespace FFgameUI
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

            SetStyle(ControlStyles.SupportsTransparentBackColor, true);
            BackColor = Color.Transparent;　//panelの背景透明化
        }
        /// <summary>
        /// 画像素材の読み込み,各ファイルのパスを自分の環境に合わせて
        /// </summary>
        Image backgroundImage = Image.FromFile(@"C:\UIimg\background2.JPG"); //背景画像読み込み
        Image[] player1Image = new Image[] { //player1の素材
        Image.FromFile(@"C:\UIimg\man.png"), //デフォルト画像
        Image.FromFile(@"C:\UIimg\man14.png"),  //攻撃時
        Image.FromFile(@"C:\UIimg\man11.png"),   //ガード時
        Image.FromFile(@"C:\UIimg\man13.png"),  //必殺技
        Image.FromFile(@"C:\UIimg\man22.png")};  //ダメージ
    Image showImage1;
        Image[] player2Image = new Image[] { //player2の素材
        Image.FromFile(@"C:\UIimg\2man.png"), //デフォルト画像
        Image.FromFile(@"C:\UIimg\2man14.png"),  //攻撃時
        Image.FromFile(@"C:\UIimg\2man11.png"),   //ガード時
        Image.FromFile(@"C:\UIimg\2man13.png"), //必殺技
        Image.FromFile(@"C:\UIimg\2man22.png")};  //ダメージ
    Image showImage2;
        SoundPlayer gong = new SoundPlayer(@"C:\UIimg\gong.wav"); //ゴング音の読み込み
        SoundPlayer attack = new SoundPlayer(@"c:\UIimg\attack.wav");//アタック音声読み込み
        SoundPlayer guard = new SoundPlayer(@"c:\UIimg\guard.wav");//ガード音読み込み


        private void panel1_Paint(object sender, PaintEventArgs e) //背景画像表示
        {
            if (backgroundImage != null)
            {
                //DrawImageメソッドで画像を座標(0, 0)の位置に表示する
                e.Graphics.DrawImage(backgroundImage,
                    0, 0, backgroundImage.Width*2/5, backgroundImage.Height*2/5);
            }
        }

        protected override void OnPaintBackground(PaintEventArgs e) //背景の初期化を無効化
        {
        }

        private async void button1_Click(object sender, EventArgs e) //スタートボタンで初期化
        {
            
            progressBar1.Value = 100; //player1 HP
            progressBar2.Value = 100; //player2 HP
            progressBar3.Value = 0; //player1 MP
            progressBar4.Value = 0; //player2 MP
            button1.Text = "Ready..."; //ボタン表示切替
            await Task.Run(() =>
            {
                Thread.Sleep(2000); //2000ms?待機
            });
            button1.Text = "Fight!!!";
            gong.Play(); //ゴング音再生
            await Task.Run(() =>
            {
                Thread.Sleep(1000);
            });
            button1.Text = "START";

            Update_A_HP update_a_hp = new Update_A_HP();

            update_a_hp.update_A_HP += new EventHandler(this.A_HPupdate);
            update_a_hp.update_B_HP += new EventHandler(this.B_HPupdate);
            update_a_hp.Start();


        }

        private async void A_HPupdate(object sender, System.EventArgs e) //playerAのHP,MP更新
        {
            drow();
            string player_A_HP = "player_A_HP";
            progressBar1.Value = int.Parse(AccessClass.pull(player_A_HP));
            
            
        }
        private async void B_HPupdate(object sender, System.EventArgs e) //playerBのHP,MP更新
        {
            drow();
            string player_B_HP = "player_B_HP";
            progressBar2.Value = int.Parse(AccessClass.pull(player_B_HP));
            
        }

        private void panel2_Paint(object sender, PaintEventArgs e) //player1の画像表示
        {
            if(showImage1 != null) //選択プレイヤー画像の描画
            {
                e.Graphics.DrawImage(showImage1,
                    0, 20, showImage1.Width*2/3, showImage1.Height*2/3);
            }
            else //デフォルト画像の描画
            {
                showImage1 = player1Image[0];
                e.Graphics.DrawImage(showImage1,
                    0, 20, showImage1.Width*2/3, showImage1.Height*2/3);
            }
         
        }

        private void panel3_Paint_1(object sender, PaintEventArgs e) //player2の画像表示
        {
            if (showImage2 != null) //選択プレイヤー画像の描画
            {
                e.Graphics.DrawImage(showImage2,
                   0, 30, showImage2.Width *2/ 3, showImage2.Height *2/ 3); //(描画オブジェクト, X, Y, 画像幅, 画像高さ)
            }
            else //デフォルト画像の描画
            {
                showImage2 = player2Image[0];
                e.Graphics.DrawImage(showImage2,
                   0, 30, showImage2.Width *2/ 3, showImage2.Height *2/ 3);
            }
        }

       public async void drow()
        {
            string player_A_move = "player_A_move";
            string player_B_move = "player_B_move";
            if (AccessClass.pull(player_A_move) == "attack") // A アタック時の描画
            {
                attack.Play();
                showImage1 = player1Image[1];
                showImage2 = player2Image[4];
                panel1.Invalidate();
                await Task.Run(() =>
                {
                    Thread.Sleep(150);
                });
                showImage1 = player1Image[0];
                showImage2 = player2Image[0];
                panel1.Invalidate(); 
            }
            if(AccessClass.pull(player_B_move)== "attack")
            {
                attack.Play();
                showImage1 = player1Image[4];
                showImage2 = player2Image[1];
                panel1.Invalidate();
                await Task.Run(() =>
                {
                    Thread.Sleep(150);
                });
                showImage1 = player1Image[0];
                showImage2 = player2Image[0];
                panel1.Invalidate();
            }
            if(AccessClass.pull(player_A_move)=="attack" && AccessClass.pull(player_B_move) == "attack")
            {
                attack.Play();
                showImage1 = player1Image[1];
                showImage2 = player2Image[1];
                panel1.Invalidate();
                await Task.Run(() =>
                {
                    Thread.Sleep(150);
                });
                showImage1 = player1Image[0];
                showImage2 = player2Image[0];
                panel1.Invalidate();
            }
        }


        private async void button2_Click(object sender, EventArgs e) //アタック動作のチェック
        {
            if (progressBar2.Value != 0)
            {
                
                attack.Play();
                progressBar2.Value -= 10;//プログレスバー値入力
                showImage1 = player1Image[1];//プレイヤー１画像切り替え
                showImage2 = player2Image[4];//プレイヤー２画像切り替え
                panel1.Invalidate();// パネル再描画
                await Task.Run(() =>　
                {
                    Thread.Sleep(150); //150ms? 待機
                });
                showImage1 = player1Image[0];
                showImage2 = player2Image[0];
                panel1.Invalidate();
            }
            

        }

        private async void button3_Click(object sender, EventArgs e) //ガード動作チェック
        {
                
                guard.Play();
                showImage1 = player1Image[2];
                panel1.Invalidate();
                
                await Task.Run(() =>
                {
                    Thread.Sleep(150);
                });
                showImage1 = player1Image[0];
                panel1.Invalidate();
        }
    }
    public class Update_A_HP //player A HPのupdateイベントクラス
    {
        public event EventHandler update_A_HP;
        public event EventHandler update_B_HP;

        public async void Start()
        {
            int n = 1;
            string UI = "UI";
            string system_update = "system_update";
            string player_A_HP = "player_A_HP";
            string player_B_HP = "player_B_HP";
            while (n == 1)
            {
                if(AccessClass.update(UI, system_update, -1)) // systemのアップデートを確認
                {
                    AccessClass.push(system_update, "False");
                    AccessClass.update(UI, system_update, -1);
                    update_A_HP(this, EventArgs.Empty);   // 更新があるとイベント発生
                    update_B_HP(this, EventArgs.Empty);  
                }
                Application.DoEvents();
            }
        }
    }
}
