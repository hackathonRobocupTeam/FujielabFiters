using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
//640*480
namespace attackjudge
{
    class Program
    {
        /// <summary>
        /// めいんやで
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {
            /*int result;
            int guard;
            string[] k = new string[640*480];   
            
            int handpointx;
            int handpointy;*/
            
            using (StreamReader r = new StreamReader("player1.txt")) //テキストファイルの読み込み
            {
                string line;    //読み込み先のstring
                char delimiter=','; //,区切り検出のための変数
                while ((line = r.ReadLine()) != null)   //ファイルの最後まで1行ずつ読み込み
                {
                    int[] ln = new int[640 * 480];  //intに変換後の配列
                    string[] n = new string[640*480];   //string配列
                    n = line.Split(delimiter);          //lineからカンマ毎に配列に入れる
                    for (int x = 0; x < 640*480; x++)   //配列の中身全回し
                    {
                        if (n[x] != null)   //stringの配列が最後に来るまで
                        {
                            ln[x] = Convert.ToInt32(n[x]);  //int型に変換
                            Console.Write("{0}", ln[x]);    //int型になった数字を表示
                            
                        }
                        
                    }Console.ReadKey(); //自動で終わらないようにする
                }
            }
            
            //int[,] p1data       = new int[100, 480];//p1 position
           /* int[,] p2data       = new int[100, 100];//p2 position
            //int[,] handdata     = new int[,]{{1,2}};//hand position
            //int[,] attackdata   = new int[100, 100];//attack area
            int handpoint;


            for( handpointx=-20;  handpointx<=20;handpointx++){
                for (handpointy = -20; handpointy <= 20; handpointy++)
                {

                }
                
            }
            

            if (handdata == attackdata)
            {
                result = 1;
            }
            guard=1;
            if (guard == 1)
            {
                result = 0;
                
            }*/
        }
    }
}
