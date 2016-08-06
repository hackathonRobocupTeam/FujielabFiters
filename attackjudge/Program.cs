using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using KinokoLib;

namespace attackjudge_2
{
    class Program
    {
        static void Main(string[] args)
        {
            int m=0;
            int[] ln = new int[640 * 480];  //intに変換後の配列
            int[] pos = new int[640 * 480];  //プレイヤーの領域
            bool result = false;
            using (StreamReader r = new StreamReader("player1.txt")) //テキストファイルの読み込み
            {
                string line;    //読み込み先のstring
                const char delimiter = ','; //,区切り検出のための変数
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
                            if (ln[x] == 1)
                            {
                                pos[m] = x;
                                //Console.Write("{0}\t", pos[m]);
                                m++;
                            }
                        }

                    }
                }
                int handinit=307031;    //拳の位置
                int guard=0;            //ガード判定
                int count;
                
                for (count = 0; count < 640 * 480; count++)
                {
                    if(handinit==pos[count])    //当たったらresultを0にする
                    {
                        result = true;
                        Console.Write(">Hit\n");
                    }
                }
                if (guard == 1)         //ガードされたらredultを0にする
                {
                    result = false;
                }
                Console.Write(">{0}",result);
            } 
            AccessClass.push("Judge", result.ToString());
            Console.ReadKey(); //自動で終わらないようにする
        }
    }
}
