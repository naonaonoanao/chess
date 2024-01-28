using Chess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace chess
{
    public partial class PromoteDialog : Window
    {
        private PromotionType userPromotion;
        private bool isTest;

        public PromoteDialog(bool isTest = false)
        {
            InitializeComponent();
            this.isTest = isTest;
        }

        public PromotionType getUserPromotion
        {
            get { return userPromotion; }
        }

        public void chooseRookButtonClick(object sender, EventArgs e)
        {
            userPromotion = PromotionType.ToRook;
            if (!isTest)
            {
                DialogResult = true;
            }
        }

        public void chooseBishopButtonClick(object sender, EventArgs e)
        {
            userPromotion = PromotionType.ToBishop;
            if (!isTest)
            {
                DialogResult = true;
            }
        }

        public void chooseKnightButtonClick(object sender, EventArgs e)
        {
            userPromotion = PromotionType.ToKnight;
            if (!isTest)
            {
                DialogResult = true;
            }
        }

        public void chooseQueenButtonClick(object sender, EventArgs e)
        {
            userPromotion = PromotionType.ToQueen;
            if (!isTest)
            {
                DialogResult = true;
            }
        }
    }
}
