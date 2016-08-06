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
            int[,] hyper = new int[480, 640];//受け取ったデータの二次元化
            int[] ln = new int[640 * 480];  //intに変換後の配列
            //int[] pos = new int [640*480];
            int[] posx = new int[640 * 480];  //プレイヤーの領域(x座標)
            int[] posy = new int[640 * 480];  //プレイヤーの領域(y座標)
            using (StreamReader r = new StreamReader("player1.txt")) //テキストファイルの読み込み
            {
                string line;    //読み込み先のstring
                char delimiter = ','; //,区切り検出のための変数
                //int m=0;
                int playercoounty;
                int playercountx;
                int poscounty = 0;
                int poscountx = 0;
                int loop = 0;
                while ((line = r.ReadLine()) != null)   //ファイルの最後まで1行ずつ読み込み
                {

                    string[] n = new string[640 * 480];   //string配列
                    n = line.Split(delimiter);          //lineからカンマ毎に配列に入れる
                    for (int x = 0; x < 640 * 480; x++)   //配列の中身全回し
                    {
                        if (n[x] != null)   //stringの配列が最後に来るまで
                        {
                            ln[x] = Convert.ToInt32(n[x]);  //int型に変換
                            //Console.Write("{0}", ln[x]);   //int型になった数字を表示

                            //1次元用のプレイヤー位置の書き出し
                            /*if (ln[x] == 1)
                            {
                                pos[m] = x;
                               // Console.Write("{0}\t", pos[m]);
                                /*using(StreamWriter w=new StreamWriter("playerposi.txt"))
                                {
                                    w.Write("{0}\t",pos[m]);
                                }
                                m++;
                                
                            }*/
                        }

                    } 
                    for (playercoounty = 0; playercoounty < 480; playercoounty++)
                    {
                        for (playercountx = 0; playercountx < 640; playercountx++)
                        {
                            hyper[playercoounty, playercountx] = ln[loop];    //二次元配列にint型にしたデータを渡す

                            if (hyper[playercoounty, playercountx] == 1)    //中身が1ならここに入る
                            {
                                posx[poscountx] = playercountx;    //x座標の値を入れる
                                posy[poscounty] = playercoounty;    //y座標の値を入れる

                                //Console.Write("posy>{0}\tposx>{1}\n",posy[poscounty],posx[poscountx]); //290,127;

                                poscountx++;    //poscoountxのカウント用
                                poscounty++;    //poscountyのカウント用


                            }
                            loop++;    //loopのカウント用
                        }
                    }

                }
            }
            int handxinit = 0;    //手のx座標
            int handyinit = 0;    //手のy座標
            int handcountx;
            int handcounty;
            int judgex = 0;
            int judgey = 0;
            int result = 0;
            for (handcountx = 0; handcountx < 640 * 480; handcountx++)
            {
                if (handxinit == posx[handcountx])   //x座標の比較
                {
                    judgex = 1; //一致したら1を返す
                }
            }
            for (handcounty = 0; handcounty < 480 * 640; handcounty++)   //y座標の比較
            {
                if (handyinit == posy[handcounty])
                {
                    judgey = 1;  //一致したら1を返す
                }
            }
            if (judgex == 1 && judgey == 1)   //両方一致してHit
            {
                result = 1;
                Console.Write(">Hit");
            }
            else
            {
                result = 0;
                Console.Write(">miss");
            }
            Console.ReadKey(); //自動で終わらないようにする 
        }
    }
}

