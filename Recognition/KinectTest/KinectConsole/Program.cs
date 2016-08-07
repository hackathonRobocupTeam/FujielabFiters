using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Kinect;
using System.Drawing;
using System.IO;
using KinokoLib;


namespace KinectConsole
{
    public class Kinect
    {
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


    internal class Tracking
    {


        private KinectSensor kinectSensor1;
        private KinectSensor kinectSensor2;

        /// <summary>深度情報→ポイントクラウドに変換するためのCoorinateMapper</summary>
        private CoordinateMapper coordinateMappper1;
        private CoordinateMapper coordinateMappper2;

        //プレイヤー情報/////
        private int[] player1 = new int[320 * 240];
        private int[] player2 = new int[320 * 240];

        private int kobusi1;
        private int kobusi2;


        private bool guard1 = false;
        private bool guard2 = false;
        private bool attack1 = false;
        private bool attack2 = false;

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
            this.kinectSensor1.DepthStream.Enable(DepthImageFormat.Resolution320x240Fps30);
            this.kinectSensor2.DepthStream.Enable(DepthImageFormat.Resolution320x240Fps30);


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
            // using (ImageFrame imageFrame = e.OpenColorImageFrame())
            //{
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
                        PunchCheker("player1", skel);

                        //Player1の攻撃が成功したら
                        if (attack1 == true)
                        {
                                Console.WriteLine("Player1 Attacked!!");
                                AttackJudge("player1", player2, kobusi1, guard2);
                        }

                        attack1 = false;
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
            // using (ImageFrame imageFrame = e.OpenColorImageFrame())
            //{
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
                    //Console.WriteLine("Player2 Command...");

                    foreach (Skeleton skel in skeleton)
                    {
                        PunchCheker("player2", skel);

                        //Player1の攻撃が成功したら
                        if (attack2 == true)
                        {
                            Console.WriteLine("Player2 Attacked!!");
                            AttackJudge("player2", player1, kobusi2, guard1);
                        }

                        attack2 = false;
                    }
                }
                else
                {
                    //Console.WriteLine("Player2がいません");
                }

            }
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
            int[] pos = new int[320 * 240];  //プレイヤーの領域

            bool result = false;

            if (player2[kobusi] == 1)
            {
                result = true;
            }

            /*
            //int count = 0;
            //int m = 0;
            for (int x = 0; x < 320 * 240; x++)   //配列の中身全回し
            {
                if (player2[x] == 1)
                {
                        pos[m] = x;
                        //Console.Write("{0}\t", pos[m]);
                        m++;
                }
            }
            for (count = 0; count < m; count++)
            {
                if (kobusi == pos[count])    //当たったらresultを0にする
                {
                    result = true;
                }
            }
            */

            if (guard == true)         //ガードされたらredultを0にする
            {
                result = false;
            }
            Console.WriteLine(hito+"Attack >{0}", result);
            //Console.ReadKey(); //自動で終わらないようにする 
           // AccessClass.push("Judge", result.ToString());

        }








        ///////     識別用     ////////

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

            //左手の距離が頭よりも近かったら
            if (skeleton.Joints[JointType.HandLeft].Position.Z + 0.35 <= skeleton.Joints[JointType.Head].Position.Z)
            {
                Point a = SkeletonPointToScreen2(skeleton.Joints[JointType.HandLeft].Position);
                kobusi = a.X + (a.Y - 1) * 320;
                //Console.WriteLine("Kinect Tracking");

                if (player == "player1") { kobusi1 = kobusi; attack1 = true; }

                else if (player == "player2") { kobusi2 = kobusi; attack2 = true; }

                else
                    Console.WriteLine("攻撃判定においてエラーが発生しました");
            }

            //右手の距離が頭よりも近かったら
            else if (skeleton.Joints[JointType.HandRight].Position.Z + 0.35 <= skeleton.Joints[JointType.Head].Position.Z)
            {
                Point a = SkeletonPointToScreen2(skeleton.Joints[JointType.HandRight].Position);
                kobusi = a.X + (a.Y - 1) * 320;

                if (player == "player1") { kobusi1 = kobusi; attack1 = true; }

                else if (player == "player2") { kobusi2 = kobusi; attack2 = true; }

                else
                    Console.WriteLine("攻撃判定においてエラーが発生しました");
            }

            else
            {
                attack1 = false;
                attack2 = false;
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
            DepthImagePoint depthPoint = this.kinectSensor1.CoordinateMapper.MapSkeletonPointToDepthPoint(skelpoint, DepthImageFormat.Resolution320x240Fps30);
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
            DepthImagePoint depthPoint = this.kinectSensor2.CoordinateMapper.MapSkeletonPointToDepthPoint(skelpoint, DepthImageFormat.Resolution320x240Fps30);
            return new Point(depthPoint.X, depthPoint.Y);
        }

    }

}
