using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FFgameUI
{
    public partial class Form2 : Form
    {
        public Form2()
        {
            InitializeComponent();
            SetStyle(ControlStyles.SupportsTransparentBackColor, true);
            BackColor = Color.Transparent;　//panelの背景透明化
            pictureBox1.Image = Image.FromFile(@"C:\UIimg\startbackground.gif");//背景画像読み込み)
        }
        
    }
}
