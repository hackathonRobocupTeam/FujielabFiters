using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Kinect;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.IO;
using KinokoLib;


namespace KinectConsole
{
    /// <summary>
    /// Mainのみを持つクラス
    /// </summary>
    public class Kinect
    {
        /// <summary>
        /// Mainのみを持つ関数
        /// Trackingの起動
        /// </summary>
        /// <param name="args"></param>
        public static void Main(string[] args)
        {
            Tracking tracking = new Tracking();
            tracking.START();
            Console.Write("Fight!!!");

            while (true)
            {

            }
        }
    }

    /// <summary>
    /// ゲーム用の各種データをKinectから取りこむクラス
    /// Kinect二台使用
    /// </summary>
    internal class Tracking
    {


        private KinectSensor kinectSensor1;
        private KinectSensor kinectSensor2;
        //private byte[] colorImage1;
        //private byte[] colorImage2;

        /// <summary>深度情報→ポイントクラウドに変換するためのCoorinateMapper</summary>
        private CoordinateMapper coordinateMappper1;
        private CoordinateMapper coordinateMappper2;

        //プレイヤー情報/////
        private int[] player1 = new int[640 * 480];
        private int[] player2 = new int[640 * 480];

        private int kobusi1;
        private int kobusi2;


        private bool guard1 = false;
        private bool guard2 = false;
        private bool attack1 = false;
        private bool attack2 = false;

        private bool jutsu1 = false;
        private bool jutsu2 = false;

        private bool check1 = false;
        private bool check2 = false;

        //////////////




        /// <summary>
        /// Kinectのスタート
        /// </summary>
        public void START()
        {

            Console.Write("Leady...");


            //kinectSensor1の値を、接続されているKinectの一番目に接続する
            this.kinectSensor1 = KinectSensor.KinectSensors[0];

            //二台目のKinectの接続
            this.kinectSensor2 = KinectSensor.KinectSensors[1];


            //深度情報の設定。スケルトンに直結する関数なので消さないように
            this.coordinateMappper1 = new CoordinateMapper(this.kinectSensor1);
            this.coordinateMappper2 = new CoordinateMapper(this.kinectSensor2);

            //カラーイメージの有効化・大きさの定義
            //this.kinectSensor1.ColorStream.Enable(ColorImageFormat.InfraredResolution640x480Fps30);
            //this.kinectSensor2.ColorStream.Enable(ColorImageFormat.InfraredResolution640x480Fps30);

            //深度イメージの有効化・大きさの定義
            this.kinectSensor1.DepthStream.Enable(DepthImageFormat.Resolution640x480Fps30);
            this.kinectSensor2.DepthStream.Enable(DepthImageFormat.Resolution640x480Fps30);


            //colorImage1 = new byte[this.kinectSensor1.ColorStream.FramePixelDataLength];
            //colorImage2 = new byte[this.kinectSensor2.ColorStream.FramePixelDataLength];



            //骨格トラッキングの有効化・大きさの定義
            this.kinectSensor1.SkeletonStream.Enable();
            this.kinectSensor2.SkeletonStream.Enable();

            //colorImageの領域確保
            // colorImage = new ColorImagePoint[this.kinectSensor1.ColorStream.FramePixelDataLength];




            //全てのFrameのイベントハンドラ
            kinectSensor1.AllFramesReady += KinectSensor_AllFramesReady;
            kinectSensor2.AllFramesReady += KinectSensor_AllFramesReady2;

            //Kinectセンサーの起動
            this.kinectSensor1.Start();

            //Kinect2台目の起動
            this.kinectSensor2.Start();
        }




        /// <summary>
        /// Kinect1
        /// 全てのフレームを統括したAllFramesReadyのイベント内容
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void KinectSensor_AllFramesReady(object sender, AllFramesReadyEventArgs e)
        {
            //AllFrameReadyEvebtArgs内から、各要素のフレーム情報を取得することができる
           /* using (ImageFrame imageFrame = e.OpenColorImageFrame())
            {
                Bitmap bmap = ImageToBitmap(1,colorImage1, e.OpenColorImageFrame());
                GuardGet(bmap);
            }
            */

            using (DepthImageFrame depthFrame = e.OpenDepthImageFrame())
            {
                // 深度画像の生データ（PlayerIndexなども入っている）の配列を取得
                DepthImagePixel[] depthImagePixels = depthFrame.GetRawPixelData();

                if (depthFrame != null)
                {
                    DepthGet("player1", depthImagePixels);
                }
                else
                {
                    return;
                }

                //スケルトンの処理
                Skeleton[] skeleton = new Skeleton[0];

                using (SkeletonFrame skeletonFrame = e.OpenSkeletonFrame())
                {
                    skeleton = new Skeleton[skeletonFrame.SkeletonArrayLength];
                    //スケルトン
                    skeletonFrame.CopySkeletonDataTo(skeleton);
                }


                if (check1 == true)
                {
                   // Console.WriteLine("Player1 Command...");
                    foreach (Skeleton skel in skeleton)
                    {
                        if (skel.TrackingState == SkeletonTrackingState.NotTracked)
                        {
                            continue;
                        }
                        ActionChecker1("player1", skel);

                        if(what_move1() == "attack")
                            AttackJudge("player1", player2, kobusi1, guard2);

                        #region utility
                        //AccessClass.push("player_A_move", what_move());
                        //Console.WriteLine(what_move1()); 
                        #endregion
                       
                    }
                }
                else
                {
                    //Console.WriteLine("Player1がいません");
                }
                    
            }



        }

       

        /// <summary>
        /// Kinect2
        /// 全てのフレームを統括したAllFramesReadyのイベント内容
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void KinectSensor_AllFramesReady2(object sender, AllFramesReadyEventArgs e)
        {
            //AllFrameReadyEvebtArgs内から、各要素のフレーム情報を取得することができる
            /*
            using (ImageFrame imageFrame = e.OpenColorImageFrame())
            {
                Bitmap bmap = ImageToBitmap(2,colorImage2, e.OpenColorImageFrame());
                GuardGet(bmap);
            }
            */

            using (DepthImageFrame depthFrame = e.OpenDepthImageFrame())
            {
                // 深度画像の生データ（PlayerIndexなども入っている）の配列を取得
                DepthImagePixel[] depthImagePixels = depthFrame.GetRawPixelData();

                if (depthFrame != null)
                {
                    DepthGet("player2", depthImagePixels);
                }
                else
                {
                    return;
                }

                //スケルトンの処理
                Skeleton[] skeleton = new Skeleton[0];

                using (SkeletonFrame skeletonFrame = e.OpenSkeletonFrame())
                {
                    skeleton = new Skeleton[skeletonFrame.SkeletonArrayLength];
                    //スケルトン
                    skeletonFrame.CopySkeletonDataTo(skeleton);
                }


                if (check2 == true)
                {
                    
                    // Console.WriteLine("Player1 Command...");
                    foreach (Skeleton skel in skeleton)
                    {
                        
                        if (skel.TrackingState == SkeletonTrackingState.NotTracked)
                        {
                            continue;
                        }

                        ActionChecker2("player2", skel);

                        if(what_move2() == "attack")
                            AttackJudge("player2",player1,kobusi2,guard1);


                        #region utility
                        //AccessClass.push("player_A_move", what_move());
                        //Console.WriteLine(what_move2());
                        #endregion

                    }
                }
                else
                {
                    //Console.WriteLine("Player1がいません");
                }

            }
        }

        /// <summary>
        /// Player1のやつ
        /// </summary>
        /// <returns></returns>
        public string what_move1()
        {
            if (jutsu1)
            {
                // TODO: 統合の時コメントアウト解除
                //AccessClass.push("judge", "True");
                return "jutsu";
            }
            if (attack1) return "attack";
            if (guard1) return "guard";
            return "normal";

        }

        /// <summary>
        /// Player2のやつ
        /// </summary>
        /// <returns></returns>
        public string what_move2()
        {
            if (jutsu2)
            {
                // TODO: 統合の時コメントアウト解除
                //AccessClass.push("judge", "True");
                return "jutsu";
            }
            if (attack2) return "attack";
            if (guard2) return "guard";
            return "normal";

        }



        ///////////////     攻撃判定用       //////////////

        /// <summary>
        /// 人物位置と拳の位置、ガード状態を見て攻撃判定を行う関数
        /// </summary>
        /// <param name="player2"></param>
        /// <param name="kobusi"></param>
        /// <param name="guard"></param>
        public void AttackJudge(string hito,int[] player2, int kobusi, bool guard)
        {
            int[] pos = new int[640*480];  //プレイヤーの領域
            

            bool result = false;
            /*
            if (player2.Length != 320*240 )
            {
                System.Threading.Thread.Sleep(5);
                }*/
            //Console.WriteLine("kobusi:{0},player2[kobusi]:{1}", kobusi, player2.Length);
                        
            if (player2[kobusi] == 1)
            {
                result = true;
            }
            
            Console.WriteLine(hito+"Attack >{0}", result);

            //Console.ReadKey(); //自動で終わらないようにする 

            //AccessClass.push("judge", result.ToString());

        }








        ///////     識別用     ////////

        /// <summary>
        /// 面倒くさいので指定できる関数作った
        /// </summary>
        /// <param name="player"></param>
        /// <param name="action"></param>
        /// <param name="ToF"></param>
        public void PlayerSetter(string player,string action,bool ToF)
        {
            if (player == "player1")
            {
                switch (action)
                {
                    case "attack":
                        attack1 = ToF;
                        break;

                    case "guard":
                        guard1 = ToF;
                        break;

                    case "jutsu":
                        jutsu1 = ToF;
                        break;                       
                }
            }
            else if (player == "player2")
            {
                switch (action)
                {
                    case "attack":
                        attack2 = ToF;
                        break;

                    case "guard":
                        guard2 = ToF;
                        break;

                    case "jutsu":
                        jutsu2 = ToF;
                        break;
                }
            }
        }



        /// <summary>
        /// Action認識を統一
        /// 必殺技→攻撃→防御という優先度
        /// </summary>
        /// <param name="player"></param>
        /// <param name="skel"></param>
        public void ActionChecker1(string player,Skeleton skel)
        {
            JutsuChecker(player,skel);

            if (jutsu1 != true)
            {
                PunchCheker(player, skel);

                if (jutsu1 != true)
                {
                    GuardChecker(player, skel);
                }
                else
                {
                    guard1 = false;
                }
            }
            else
            {
                attack1 = false;
                guard1 = false;
            }
        }

        /// <summary>
        /// Action認識を統一
        /// 必殺技→攻撃→防御という優先度
        /// </summary>
        /// <param name="player"></param>
        /// <param name="skel"></param>
        public void ActionChecker2(string player, Skeleton skel)
        {
            JutsuChecker(player, skel);

            if (jutsu1 != true)
            {
                PunchCheker(player, skel);

                if (jutsu1 != true)
                {
                    GuardChecker(player, skel);
                }
                else
                {
                    guard2 = false;
                }
            }
            else
            {
                attack2 = false;
                guard2 = false;
            }
        }


        /// <summary>
        /// 必殺技のコマンド
        /// 頭よりも手を上にあげる
        /// </summary>
        /// <param name="player"></param>
        /// <param name="skeleton"></param>
        public void JutsuChecker(string player,Skeleton skeleton)
        {
            //頭の上に左手
            if (skeleton.Joints[JointType.HandLeft].Position.Y > skeleton.Joints[JointType.Head].Position.Y + 0.2)
            {
                if (player == "player1") { jutsu1 = true; }

                else if (player == "player2") { jutsu2 = true; }

                Console.WriteLine(player + "の必殺技！！！！");
            }

            //頭の上に右手
            else if (skeleton.Joints[JointType.HandRight].Position.Y > skeleton.Joints[JointType.Head].Position.Y + 0.2)
            {
                if (player == "player1") { jutsu1 = true; }

                else if (player == "player2") { jutsu2 = true; }

                Console.WriteLine(player + "の必殺技！！！！");
            }

            //それ以外
            else{
                if (player == "player1") { jutsu1 = false; }

                else if (player == "player2") { jutsu2 = false; }
            }
        }

        /// <summary>
        /// ガード認識用関数
        /// 1．片腕を盾のようにかざす
        /// 2．両手を並行にして盾にする
        /// </summary>
        /// <param name="frame"></param>
        public void GuardChecker(string player,Skeleton skeleton)
        {
            //Console.WriteLine("腕：{0},頭：{1}", skeleton.Joints[JointType.HandLeft].Position.Z, skeleton.Joints[JointType.Head].Position.Z);

            //腕と肘の距離
            if (Math.Abs(skeleton.Joints[JointType.HandLeft].Position.Z - skeleton.Joints[JointType.ElbowLeft].Position.Z) <= 0.1 
                && skeleton.Joints[JointType.HandLeft].Position.Z + 0.25  <= skeleton.Joints[JointType.Head].Position.Z)
            {
                if (player == "player1") {  guard1 = true; }

                else if (player == "player2") { guard2 = true; }

                Console.WriteLine(player+"Guarded!:guard1");
            }

            //肘と腕の距離
            else if (Math.Abs(skeleton.Joints[JointType.HandLeft].Position.Z - skeleton.Joints[JointType.ElbowLeft].Position.Z) <= 0.1 
                && skeleton.Joints[JointType.HandLeft].Position.Z + 0.25 <= skeleton.Joints[JointType.Head].Position.Z)
            {
                if (player == "player1") { guard1 = true; }

                else if (player == "player2") { guard2 = true; }


                Console.WriteLine(player + "Guarded!:guard2");              
            }

            //両手の距離差が少ないとき
            else if (Math.Abs(skeleton.Joints[JointType.HandLeft].Position.Z - skeleton.Joints[JointType.HandLeft].Position.Z) <= 0.03 
                && skeleton.Joints[JointType.HandLeft].Position.Z + 0.25 <= skeleton.Joints[JointType.Head].Position.Z)
            {
                if (player == "player1") { guard1 = true; }

                else if (player == "player2") { guard2 = true; }


                Console.WriteLine(player + "Guarded!:guard3");
            }
            else
            {
                if (player == "player1") { guard1 = false; }

                else if (player == "player2") { guard2 = false; }
            }
        }




        /// <summary>
        /// 取得したDepthから人の有無を確認する関数
        /// </summary>
        /// <param name="player"></param>
        /// <param name="depthImagePixels"></param>
        public void DepthGet(string player, DepthImagePixel[] depthImagePixels)
        {

            if (player == "player1")
                check1 = false;
            else if (player == "player2")
                check2 = false;


            int[] play = new int[depthImagePixels.Length];
            for (int u = 0; u < depthImagePixels.Length; u++)
            {
                if (depthImagePixels[u].PlayerIndex != 0)
                {
                    play[u] = 1;

                    if (player == "player1")
                        check1 = true;
                    else if (player == "player2")
                        check2 = true;
                    else
                        Console.Write("おかしい");
                }


                else
                    play[u] = 0;
            }

            if (player == "player1")
                player1 = play;
            else if (player == "player2")
                player2 = play;
            else
                Console.WriteLine("人物取得においてエラーが発生しました");
        }





        /// <summary>
        /// 腕の距離と頭の距離を比較して攻撃を検出する関数
        /// stringでプレイヤーを変更可
        /// </summary>
        /// <param name="player"></param>
        /// <param name="skeleton"></param>
        public void PunchCheker(string player, Skeleton skeleton)
        {
            int kobusi = 0;
            //Console.WriteLine("腕：{0},頭：{1}", skeleton.Joints[JointType.HandLeft].Position.Z, skeleton.Joints[JointType.Head].Position.Z);

            //左手の距離が頭よりもKinectに近かったら
            if (skeleton.Joints[JointType.HandLeft].Position.Z + 0.37 <= skeleton.Joints[JointType.Head].Position.Z)
            {
                Point a = SkeletonPointToScreen2(skeleton.Joints[JointType.HandLeft].Position);
                
                Console.WriteLine(":X{0}    Y:{1}", a.X, a.Y);
                if (a.Y < 0) a.Y = 0;
                kobusi = a.X + ((a.Y) * 640);

                if (player == "player1") { kobusi1 = kobusi; attack1 = true; }

                else if (player == "player2") { kobusi2 = kobusi; attack2 = true; }

                Console.WriteLine(player + "Attaked!:Left Punching");
            }

            //右手の距離が頭よりもKinectに近かったら
            else if (skeleton.Joints[JointType.HandRight].Position.Z + 0.37 <= skeleton.Joints[JointType.Head].Position.Z)
            {
                Point a = SkeletonPointToScreen2(skeleton.Joints[JointType.HandRight].Position);
                             
                Console.WriteLine(":X{0}    Y:{1}", a.X, a.Y);
                if (a.Y < 0) a.Y = 0;
                kobusi = a.X + (a.Y) * 640;   

                if (player == "player1") { kobusi1 = kobusi; attack1 = true; }

                else if (player == "player2") { kobusi2 = kobusi; attack2 = true; }
                
                Console.WriteLine(player + "Attaked!:Right Punching");
            }

            else
            {
                if (player == "player1") { attack1 = false; }

                else if (player == "player2") { attack2 = false; }
            }        
    }

        




        /////       値変換用        //////

        /// <summary>
        /// スケルトン情報をdepthのx,y座標に置き換える関数（１台目用）
        /// </summary>
        /// <param name="skelpoint">point to map</param>
        /// <returns>mapped point</returns>
        private Point SkeletonPointToScreen(SkeletonPoint skelpoint)
        {
            // Convert point to depth space.  
            // We are not using depth directly, but we do want the points in our 640x480 output resolution.
            ColorImagePoint depthPoint = this.kinectSensor1.CoordinateMapper.MapSkeletonPointToColorPoint(skelpoint, ColorImageFormat.RawYuvResolution640x480Fps15);
            return new Point(depthPoint.X, depthPoint.Y);
        }


        /// <summary>
        /// スケルトン情報をdepthのx,y座標に置き換える関数（２台目用）
        /// </summary>
        /// <param name="skelpoint">point to map</param>
        /// <returns>mapped point</returns>
        private Point SkeletonPointToScreen2(SkeletonPoint skelpoint)
        {
            // Convert point to depth space.  
            // We are not using depth directly, but we do want the points in our 640x480 output resolution.
            ColorImagePoint depthPoint = this.kinectSensor2.CoordinateMapper.MapSkeletonPointToColorPoint(skelpoint, ColorImageFormat.RawYuvResolution640x480Fps15);
            return new Point(depthPoint.X, depthPoint.Y);
        }


        
       
    }

}
