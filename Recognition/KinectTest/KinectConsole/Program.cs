using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Kinect;
using System.Drawing;
using System.IO;


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

        /// <summary>深度情報→ポイントクラウドに変換するためのCoorinateMapper</summary>
        private CoordinateMapper coordinateMappper;

        private ColorImageFormat colorImageformat;

        /// <summary>
        /// Kinectのスタート
        /// </summary>
        public void START()
        {
            //kinectSensor1の値を、接続されているKinectの一番目に接続する
            this.kinectSensor1 = KinectSensor.KinectSensors[0];

            //二台目のKinectの接続
            //this.kinectSensor2 = KinectSensor.KinectSensors[1];

            //深度情報の設定。スケルトンに直結する関数なので消さないように
            this.coordinateMappper = new CoordinateMapper(this.kinectSensor1);

            //カラーイメージの有効化・大きさの定義
            this.kinectSensor1.ColorStream.Enable(ColorImageFormat.InfraredResolution640x480Fps30);
            //this.kinectSensor2.ColorStream.Enable(ColorImageFormat.InfraredResolution640x480Fps30);

            //深度イメージの有効化・大きさの定義
            this.kinectSensor1.DepthStream.Enable(DepthImageFormat.Resolution640x480Fps30);
           // this.kinectSensor2.DepthStream.Enable(DepthImageFormat.Resolution640x480Fps30);

            //骨格トラッキングの有効化・大きさの定義
            this.kinectSensor1.SkeletonStream.Enable();
            //this.kinectSensor2.SkeletonStream.Enable();

            //colorImageの領域確保
            colorImage = new ColorImagePoint[this.kinectSensor1.ColorStream.FramePixelDataLength];

            

            
            //全てのFrameのイベントハンドラ
            kinectSensor1.AllFramesReady += KinectSensor_AllFramesReady2;
           // kinectSensor2.AllFramesReady += KinectSensor_AllFramesReady2;

            //Kinectセンサーの起動
            this.kinectSensor1.Start();

            //Kinect2台目の起動
            //this.kinectSensor2.Start();
        }
        

        /// <summary>
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

                        // 深度画像の生データ（PlayerIndexなども入っている）の配列を取得
                        DepthImagePixel[] depthImagePixels = depthFrame.GetRawPixelData();

                        // depthImagePixelsと同じ数だけのポイントデータを作成する
                        if (this.skeletonPoints == null ||
                            this.skeletonPoints.Length != depthImagePixels.Length)
                        {
                            this.skeletonPoints = new SkeletonPoint[depthImagePixels.Length];

                        }

                        // 深度ピクセルを変換して skeletonPoints にする
                        this.coordinateMappper.MapDepthFrameToSkeletonFrame(
                            depthFrame.Format, depthImagePixels, skeletonPoints);


                        ///ここに各フレームに対してやりたい処理などを書いていく
                        short[] pixel=new short[depthFrame.PixelDataLength];
                        Skeleton[] skeleton = new Skeleton[skeletonFrame.SkeletonArrayLength];


                        if (skeletonFrame!=null) {                            
                            skeletonFrame.CopySkeletonDataTo(skeleton);

                            Console.WriteLine(string.Format("{0},{1},{2},{3}",
                                "left",
                                skeleton[5].Position.X,
                                skeleton[5].Position.Y,
                                skeleton[5].Position.Z));
                        }
                    }
                }
            }//using破棄

        }

        /// <summary>
        /// 全てのフレームを統括したAllFramesReadyのイベント内容
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void KinectSensor_AllFramesReady2(object sender, AllFramesReadyEventArgs e)
        {
            
            string labelingFile = Path.Combine(
              "C:/Users/jakky_2/Pictures/kobusi/player" + fn + ".txt");
            StreamWriter sw = new StreamWriter(labelingFile, append: false, encoding: Encoding.UTF8);
            int count = 0;
            int val = 0;

            //AllFrameReadyEvebtArgs内から、各要素のフレーム情報を取得することができる
            using (ImageFrame imageFrame = e.OpenColorImageFrame())
            {
                using (DepthImageFrame depthFrame = e.OpenDepthImageFrame())
                {
                    using (SkeletonFrame skeletonFrame = e.OpenSkeletonFrame())
                    {
                        // 深度画像の生データ（PlayerIndexなども入っている）の配列を取得
                        DepthImagePixel[] depthImagePixels = depthFrame.GetRawPixelData();


                        for (int u = 0; u < depthImagePixels.Length; u++)
                        {
                            if (depthImagePixels[u].PlayerIndex != 0)
                            {
                                sw.Write("1");
                                val++;
                            }
                            else
                                sw.Write("0");

                            sw.Write(",");
                        }
                            if (val != 0)
                            {
                                fn++;
                            }
                        
                        sw.Close();
                        sw.Dispose();

                        //スケルトンの処理
                        Skeleton[] skeleton = new Skeleton[skeletonFrame.SkeletonArrayLength];
                        if (skeletonFrame != null)
                        {
                            skeletonFrame.CopySkeletonDataTo(skeleton);

                            Console.WriteLine(string.Format("{0},{1},{2},{3}",
                                fn,
                                skeleton[5].Position.X,
                                skeleton[5].Position.Y,
                                skeleton[5].Position.Z));
                        }
                        
                    }
                }
            }//using破棄

        }
    }
    
}
