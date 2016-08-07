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
                
            while (true)
            {

            }
        }
    }


    internal class Tracking {
        /// <summary>スケルトンポイント用のバッファ</summary>
        private SkeletonPoint[] skeletonPoints = null;

        /// <summary>スケルトン用バッファ</summary>
        private Skeleton[] skeleton;
       
        private int fn = 0;

        private KinectSensor kinectSensor1;
        private KinectSensor kinectSensor2;
        ColorImagePoint[] colorImage;
        DepthImagePixel[] depthpixel;

        DepthImageFormat depthformat;

        /// <summary>深度情報→ポイントクラウドに変換するためのCoorinateMapper</summary>
        private CoordinateMapper coordinateMappper;

        private ColorImageFormat colorImageformat;

        //プレイヤー情報/////
        private int[] player1 = new int[320 * 240];
        private int[] player2 = new int[320 * 240];

        private int kobusi1;
        private int kobusi2;


        bool guard1 = false;
        bool guard2 = false;
        //////////////




        /// <summary>
        /// Kinectのスタート
        /// </summary>
        public void START()
        {
            //kinectSensor1の値を、接続されているKinectの一番目に接続する
            this.kinectSensor1 = KinectSensor.KinectSensors[0];

            //二台目のKinectの接続
            this.kinectSensor2 = KinectSensor.KinectSensors[1];

            //深度情報の設定。スケルトンに直結する関数なので消さないように
            this.coordinateMappper = new CoordinateMapper(this.kinectSensor1);

            //カラーイメージの有効化・大きさの定義
            this.kinectSensor1.ColorStream.Enable(ColorImageFormat.InfraredResolution640x480Fps30);
            this.kinectSensor2.ColorStream.Enable(ColorImageFormat.InfraredResolution640x480Fps30);

            //深度イメージの有効化・大きさの定義
            this.kinectSensor1.DepthStream.Enable(DepthImageFormat.Resolution640x480Fps30);
            this.kinectSensor2.DepthStream.Enable(DepthImageFormat.Resolution640x480Fps30);
           

            //骨格トラッキングの有効化・大きさの定義
            this.kinectSensor1.SkeletonStream.Enable();
            this.kinectSensor2.SkeletonStream.Enable();

            //colorImageの領域確保
            colorImage = new ColorImagePoint[this.kinectSensor1.ColorStream.FramePixelDataLength];

            

            
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
            using (ImageFrame imageFrame = e.OpenColorImageFrame())
            {
                using (DepthImageFrame depthFrame = e.OpenDepthImageFrame())
                {
                    using (SkeletonFrame skeletonFrame = e.OpenSkeletonFrame())
                    {
                        //スケルトンの処理
                        Skeleton[] skeleton = new Skeleton[skeletonFrame.SkeletonArrayLength];
                        
                       
                        // 深度画像の生データ（PlayerIndexなども入っている）の配列を取得
                        DepthImagePixel[] depthImagePixels = depthFrame.GetRawPixelData();

                        if (depthFrame != null)
                        {
                            for (int u = 0; u < depthImagePixels.Length; u++)
                            {
                                if (depthImagePixels[u].PlayerIndex != 0)
                                {
                                    player1[u] = 1;
                                }
                                else
                                    player1[u] = 0;
                            }
                        }

                        //腕と頭のZ位置を比較する
                        if (skeleton != null)
                        {
                            if (skeleton[1].Position.Z >= skeleton[4].Position.Z + 50.0)
                            {
                                Point a = SkeletonPointToScreen2(skeleton[4].Position);
                                kobusi1 = a.X + (a.Y - 1) * 320;
                            }
                        }
                                                
                    }
                }
            }//using破棄

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
            using (ImageFrame imageFrame = e.OpenColorImageFrame())
            {
                using (DepthImageFrame depthFrame = e.OpenDepthImageFrame())
                {
                    using (SkeletonFrame skeletonFrame = e.OpenSkeletonFrame())
                    {

                        //スケルトンの処理
                        Skeleton[] skeleton = new Skeleton[skeletonFrame.SkeletonArrayLength];
                                           


                        // 深度画像の生データ（PlayerIndexなども入っている）の配列を取得
                        DepthImagePixel[] depthImagePixels = depthFrame.GetRawPixelData();

                        if (depthFrame != null)
                        {
                            for (int u = 0; u < depthImagePixels.Length; u++)
                            {
                                if (depthImagePixels[u].PlayerIndex != 0)
                                {
                                    player2[u] = 1;
                                }
                                else
                                    player2[u] = 0;
                            }
                        }

                        //腕と頭のZ位置を比較する
                        if (skeleton != null)
                        {
                            if (skeleton[1].Position.Z >= skeleton[4].Position.Z + 50.0)
                            {
                                Point a = SkeletonPointToScreen2(skeleton[4].Position);
                                kobusi2 = a.X + (a.Y - 1) * 320;
                            }
                        }
                        
                    }
                }
            }//using破棄

        }



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





        ///////////////     攻撃判定用       ////////////////////////////////////////////
        /// <summary>
        /// 人物位置と拳の位置、ガード状態を見て攻撃判定を行う関数
        /// </summary>
        /// <param name="player2"></param>
        /// <param name="kobusi"></param>
        /// <param name="guard"></param>
        public void AttackJudge(int[] player2, int kobusi, bool guard)
        {
            int m = 0;
            int[] pos = new int[320 * 240];  //プレイヤーの領域

            for (int x = 0; x < 320 * 240; x++)   //配列の中身全回し
            {
                if (player2[x] == 1)
                {
                        pos[m] = x;
                        //Console.Write("{0}\t", pos[m]);
                        m++;
                }
            }
            int count;
            bool result = false;
            for (count = 0; count < m; count++)
            {
                if (kobusi == pos[count])    //当たったらresultを0にする
                {
                    result = true;
                }
            }
            if (guard == true)         //ガードされたらredultを0にする
            {
                result = false;
            }
            Console.Write(">{0}", result);
            //Console.ReadKey(); //自動で終わらないようにする 
            AccessClass.push("Judge",result.ToString());
            
        }
        

    }
    
}
