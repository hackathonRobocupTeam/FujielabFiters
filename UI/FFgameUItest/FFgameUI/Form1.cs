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
        Image finishpanel = Image.FromFile(@"C:\UIimg\finish.PNG");
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
        SoundPlayer jutsu = new SoundPlayer(@"c:\UIimg\jutsu.wav");//必殺技音読み込み
        SoundPlayer gameovergong = new SoundPlayer(@"c:\UIimg\gameover.wav");//必殺技音読み込み






        private void panel1_Paint(object sender, PaintEventArgs e) //背景画像表示
        {
            if (backgroundImage != null)
            {
                //DrawImageメソッドで画像を座標(0, 0)の位置に表示する
                e.Graphics.DrawImage(backgroundImage,
                    0, 0, backgroundImage.Width * 2 / 5, backgroundImage.Height * 2 / 5);
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

            if (AccessClass.pull("game_state") == "start")
            {
                AccessClass.push("game_state", "setup");
            }

            Update_A_HP update_a_hp = new Update_A_HP(); // Update_A_HPクラスインスタンスを作成

            update_a_hp.systemstate += new EventHandler(this.gameover);
            update_a_hp.update_A_HP += new EventHandler(this.A_HPupdate);
            update_a_hp.update_B_HP += new EventHandler(this.B_HPupdate);
            update_a_hp.drow += new EventHandler(this.drow);
            update_a_hp.update_A_MP += new EventHandler(this.A_MPupdate);
            update_a_hp.update_B_MP += new EventHandler(this.B_MPupdate);

            update_a_hp.Start(); //　Update_A_HPイベント実行


        }

        private async void A_HPupdate(object sender, System.EventArgs e) //playerAのHP,MP更新
        {
            string player_A_HP = "player_A_HP";
            progressBar1.Value = int.Parse(AccessClass.pull(player_A_HP));


        }
        private async void B_HPupdate(object sender, System.EventArgs e) //playerBのHP,MP更新
        {
            string player_B_HP = "player_B_HP";
            progressBar2.Value = int.Parse(AccessClass.pull(player_B_HP));

        }
        private async void A_MPupdate(object sender, System.EventArgs e) //playerAのMP更新
        {
            string player_A_MP = "player_A_MP";
            progressBar3.Value = int.Parse(AccessClass.pull(player_A_MP));
        }
        private async void B_MPupdate(object sender, System.EventArgs e) //playerBのMP更新
        {
            string player_B_MP = "player_B_MP";
            progressBar4.Value = int.Parse(AccessClass.pull(player_B_MP));
        }

        private void panel2_Paint(object sender, PaintEventArgs e) //player1の画像表示
        {
            if (showImage1 != null) //選択プレイヤー画像の描画
            {
                e.Graphics.DrawImage(showImage1,
                    0, 20, showImage1.Width * 2 / 3, showImage1.Height * 2 / 3);
            }
            else //デフォルト画像の描画
            {
                showImage1 = player1Image[0];
                e.Graphics.DrawImage(showImage1,
                    0, 20, showImage1.Width * 2 / 3, showImage1.Height * 2 / 3);
            }

        }

        private void panel3_Paint_1(object sender, PaintEventArgs e) //player2の画像表示
        {
            if (showImage2 != null) //選択プレイヤー画像の描画
            {
                e.Graphics.DrawImage(showImage2,
                   0, 30, showImage2.Width * 2 / 3, showImage2.Height * 2 / 3); //(描画オブジェクト, X, Y, 画像幅, 画像高さ)
            }
            else //デフォルト画像の描画
            {
                showImage2 = player2Image[0];
                e.Graphics.DrawImage(showImage2,
                   0, 30, showImage2.Width * 2 / 3, showImage2.Height * 2 / 3);
            }
        }

        public async void drow(object sender, System.EventArgs e) // キャラクター描画、音声再生の関数
        {
            string player_A_move = "player_A_move";
            string player_B_move = "player_B_move";
            string judge = "judge";
            string A_move = AccessClass.pull(player_A_move);
            string B_move = AccessClass.pull(player_B_move);
            string Judge = AccessClass.pull(judge);
            if (A_move == "jutsu" && B_move == "jutsu") //　A B jutsu
            {
                jutsu.Play();
                showImage1 = player1Image[3];
                showImage2 = player2Image[3];
                panel2.Invalidate();
                panel3.Invalidate();
                await Task.Run(() =>
                {
                    Thread.Sleep(150);
                });
                showImage1 = player1Image[0];
                showImage2 = player2Image[0];
                panel2.Invalidate();
                panel3.Invalidate();
            }
            else if (A_move == "jutsu") // A justu
            {
                jutsu.Play();
                showImage1 = player1Image[3];
                panel2.Invalidate();
                await Task.Run(() =>
                {
                    Thread.Sleep(150);
                });
                showImage2 = player2Image[4];
                panel3.Invalidate();
                await Task.Run(() =>
                {
                    Thread.Sleep(150);
                });
                showImage1 = player1Image[0];
                showImage2 = player2Image[0];
                panel2.Invalidate();
                panel3.Invalidate();
            }
            else if (B_move == "jutsu") // B jutsu
            {
                jutsu.Play();
                showImage2 = player2Image[3];
                panel3.Invalidate();
                await Task.Run(() =>
                {
                    Thread.Sleep(150);
                });
                showImage1 = player1Image[4];
                panel2.Invalidate();
                await Task.Run(() =>
                {
                    Thread.Sleep(150);
                });
                showImage1 = player1Image[0];
                showImage2 = player2Image[0];
                panel2.Invalidate();
                panel3.Invalidate();
            }
            else
            {
                if (Judge == "False") //　回避失敗
                {
                    if (A_move == "attack") // A attack
                    {
                        attack.Play();
                        showImage1 = player1Image[1];
                        panel2.Invalidate();
                        await Task.Run(() =>
                        {
                            Thread.Sleep(150);
                        });
                        showImage1 = player1Image[0];
                        panel2.Invalidate();

                        if (B_move == "normal") // B normal
                        {
                            showImage2 = player2Image[4];
                            panel3.Invalidate();
                            await Task.Run(() =>
                            {
                                Thread.Sleep(150);
                            });
                            showImage2 = player2Image[0];
                            panel3.Invalidate();
                        }
                        if (B_move == "guard") // B guard
                        {
                            guard.Play();
                            showImage2 = player2Image[2];
                            panel3.Invalidate();
                            await Task.Run(() =>
                            {
                                Thread.Sleep(150);
                            });
                            showImage2 = player2Image[0];
                            panel3.Invalidate();
                        }

                    }
                    if (B_move == "attack") // B attack
                    {
                        attack.Play();
                        showImage2 = player2Image[1];
                        panel3.Invalidate();
                        await Task.Run(() =>
                        {
                            Thread.Sleep(150);
                        });
                        showImage2 = player2Image[0];
                        panel3.Invalidate();

                        if (A_move == "guard") // A guard
                        {
                            guard.Play();
                            showImage1 = player1Image[2];
                            panel2.Invalidate();
                            await Task.Run(() =>
                            {
                                Thread.Sleep(150);
                            });
                            showImage1 = player1Image[0];
                            panel2.Invalidate();
                        }
                        if (A_move == "normal") // A normal
                        {
                            showImage1 = player1Image[4];
                            panel2.Invalidate();
                            await Task.Run(() =>
                            {
                                Thread.Sleep(150);
                            });
                            showImage1 = player1Image[0];
                            panel2.Invalidate();
                        }
                    }
                }
                else //　回避成功
                {
                    if (A_move == "attack") // A attack
                    {
                        attack.Play();
                        showImage1 = player1Image[1];
                        panel2.Invalidate();
                        await Task.Run(() =>
                        {
                            Thread.Sleep(150);
                        });
                        showImage1 = player1Image[0];
                        panel2.Invalidate();
                    }
                    if (B_move == "attack") // B attack
                    {
                        attack.Play();
                        showImage2 = player2Image[1];
                        panel3.Invalidate();
                        await Task.Run(() =>
                        {
                            Thread.Sleep(150);
                        });
                        showImage2 = player2Image[0];
                        panel3.Invalidate();
                    }
                }
            }

        }




        private async void gameover(object sender, EventArgs e)  //ゲーム終了時の描画
        {
            string game_state = "game_state";
            if (AccessClass.pull(game_state) == "game_over") // game_stateがgame_overで実行
            {

                await Task.Run(() =>
                {
                    Thread.Sleep(300);
                });
                this.panel4.Location = new Point(450, 280);
                await Task.Run(() =>
                {
                    Thread.Sleep(150);
                });
                gameovergong.Play();
                await Task.Run(() =>
                {
                    Thread.Sleep(5000);
                });
                this.panel4.Location = new Point(450, 660);


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
                panel2.Invalidate();// パネル再描画
                panel3.Invalidate();
                await Task.Run(() =>
                {
                    Thread.Sleep(150); //150ms? 待機
                });
                showImage1 = player1Image[0];
                showImage2 = player2Image[0];
                panel2.Invalidate();
                panel3.Invalidate();
            }


        }

        private async void button3_Click(object sender, EventArgs e) //ガード動作チェック
        {

            guard.Play();
            showImage1 = player1Image[2];
            panel2.Invalidate();

            await Task.Run(() =>
            {
                Thread.Sleep(150);
            });
            showImage1 = player1Image[0];
            panel2.Invalidate();
        }

        private void panel4_Paint(object sender, PaintEventArgs e)
        {
            if (finishpanel != null)
            {
                //DrawImageメソッドで画像を座標(0, 0)の位置に表示する
                e.Graphics.DrawImage(finishpanel,
                    0, 0, finishpanel.Width * 2 / 5, finishpanel.Height * 2 / 5);
            }
        }
    }
    public class Update_A_HP //player A HPのupdateイベントクラス <- ゲーム中のUI描画制御
    {
        public event EventHandler update_A_HP;
        public event EventHandler update_B_HP;
        public event EventHandler drow;
        public event EventHandler update_A_MP;
        public event EventHandler update_B_MP;
        public event EventHandler systemstate;

        public async void Start()
        {
            int n = 1;
            string UI = "UI";
            string system_update = "system_update";
            string player_A_HP = "player_A_HP";
            string player_B_HP = "player_B_HP";
            while (n == 1)
            {

                if (AccessClass.update(UI, system_update, -1)) // systemのアップデートを確認
                {
                    AccessClass.push(system_update, "False");
                    AccessClass.update(UI, system_update, -1);
                    systemstate(this, EventArgs.Empty);
                    update_A_HP(this, EventArgs.Empty);   // 更新があるとイベント発生
                    update_B_HP(this, EventArgs.Empty);
                    drow(this, EventArgs.Empty);
                    update_A_MP(this, EventArgs.Empty);
                    update_B_MP(this, EventArgs.Empty);
                }
                Application.DoEvents();

                await Task.Run(() =>
                {
                    Thread.Sleep(100);
                });
            }

        }
    }
}

