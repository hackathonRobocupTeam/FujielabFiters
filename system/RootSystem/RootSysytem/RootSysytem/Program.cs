using System;
using System.Threading;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KinokoLib;



namespace RootSysytem
{
    class Program
    {
        private static string game_state = "game_state";
        private static string module_name = "RootSystem";

        static void Main(string[] args) {
            while (true)
            {
                state_manager();
            }
        }
        static void state_manager() {
            AccessClass.push(game_state, "start");
            // 音声認識か何かでgame_stateが変わるまで待機
            while (true) {
                Thread.Sleep(50);
                if (AccessClass.update(module_name, game_state, -1)) break;
                    }
            //  初期化関数を呼ぶ
            if(AccessClass.pull(game_state) == "setup")
            {
                __init__();
            }
            // battle時の処理
            AccessClass.push(game_state, "battle");
            event_manager();
            // ゲーム終了時の処理
            AccessClass.push(game_state, "game_over");
            // わからないけどとりあえずUIの処理が終わるまで待つ
            System.Threading.Thread.Sleep(2000);
        }
        // initialize
        static void __init__()
        {
            AccessClass.push("player_A_HP", "100");
            AccessClass.push("player_B_HP", "100");
            AccessClass.push("player_A_MP", "30");
            AccessClass.push("player_B_MP", "30");

        }
        static void event_manager()
        {
            bool game_over = false;
            string A_MOVE;
            string B_MOVE;
            // game overじゃなきゃループ
            while (!(game_over))
            {
                Thread.Sleep(100);
                if (AccessClass.update(module_name, "player_A_move", -1) || AccessClass.update(module_name, "player_B_move", -1))
                {
                    A_MOVE = AccessClass.pull("player_A_move");
                    B_MOVE = AccessClass.pull("player_B_move");
                    if (!(attack_check(A_MOVE, B_MOVE)))
                    { // attack or jutsuじゃなきゃHPは変動しない
                        break;
                    }
                    // HPが0になったらtrueを返す
                    game_over = player_manager(A_MOVE, B_MOVE);
                }
                else
                {
                    continue;
                }
                AccessClass.push("system_update", "True");
            }
        }
        static bool player_manager(string A_MOVE, string B_MOVE)
        {
            int HP_A;
            int HP_B;
 
            // 回避したか否か
            if (!(judge_check())) return false;
            string who = who_attack(A_MOVE, B_MOVE);
            if(who == "err")
            {
                Console.WriteLine("err!!\n");
                return false;
            }
            else if (who == "double")
            {
                damage("A", A_MOVE, B_MOVE);
                damage("B", A_MOVE, B_MOVE);
            }
            else
            {
                damage(who, A_MOVE, B_MOVE);
            }
            HP_A = int.Parse(AccessClass.pull("player_A_HP"));
            HP_B = int.Parse(AccessClass.pull("player_B_HP"));
 
            if (HP_A == 0 || HP_B == 0) return true;
            return false;
        }
        // ifの中で終了したいためbool型にしている。よりよい方法知ってたら教えて 
        static bool damage(string who, string A_MOVE, string B_MOVE)
        {
            int HP_A = int.Parse(AccessClass.pull("player_A_HP"));
            int HP_B = int.Parse(AccessClass.pull("player_B_HP"));
            int MP_A = int.Parse(AccessClass.pull("player_A_MP"));
            int MP_B = int.Parse(AccessClass.pull("player_B_MP"));
            Console.WriteLine("A:{0}, B:{1}", HP_A, HP_B);
            if (who == "A")
            {
                if (A_MOVE == "jutsu")
                {
                    if(MP_A > 90)
                    {
                        MP_A = 0;
                        HP_B = HP_B - 45;
                    }
                    if(HP_B < 0) HP_B = 0;
                    AccessClass.push("player_A_MP", MP_A.ToString());
                    AccessClass.push("player_B_HP", HP_B.ToString());
                    return true;
                }
                if (B_MOVE != "guard") HP_B = HP_B - 9;
                MP_A = MP_A + 10;
                MP_A = MP_A + 15;
            }
            if (who == "B")
            {
               if (B_MOVE == "jutsu") {
                   if (MP_B > 90)
                   {
                        MP_B = 0;
                        HP_A = HP_B - 45;
                    }
                    if(HP_A < 0) HP_A = 0;
                    AccessClass.push("player_B_MP", MP_B.ToString());
                    AccessClass.push("player_A_HP", HP_A.ToString());
                    return true;
                }
                if (A_MOVE != "guard") HP_A = HP_A - 9;
                MP_B = MP_B + 15;
                MP_A = MP_A + 10;
            }
            if (MP_A >= 100){
                MP_A = 100;
            }
            if (MP_B >= 100){
                MP_B = 100;
            }
            if (HP_A < 0) HP_A = 0;
            if (HP_B < 0) HP_B = 0;
            AccessClass.push("player_A_MP", MP_A.ToString());
            AccessClass.push("player_A_HP", HP_A.ToString());
            AccessClass.push("player_B_MP", MP_B.ToString());
            AccessClass.push("player_B_HP", HP_B.ToString());
            return true;
        }
        static bool judge_check()
        {
            if (AccessClass.pull("judge") == "True") return true;
            return false;
        }
        static bool attack_check(string A_MOVE, string B_MOVE)
        {
            if (A_MOVE == "jutsu") return true;
            if (B_MOVE == "jutsu") return true;
            if (A_MOVE == "attack") return true;
            if (B_MOVE == "attack") return true;
            return false;
        }
        static string who_attack(string A_MOVE, string B_MOVE)
        {
            if (A_MOVE == "attack" && B_MOVE == "attack" ) return "double";
            else if (A_MOVE == "jutsu" && B_MOVE == "jutsu" ) return "double";
            else if (A_MOVE == "jutsu") return "A";
            else if (B_MOVE == "jutsu") return "B";
            else if (A_MOVE == "attack") return "A";
            else if (B_MOVE == "attack") return "B";
            // AもBもアタックしてなかったら呼ばれるはずがない
            return "err";
 
        }
    }
}
