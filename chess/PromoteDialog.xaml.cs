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
    /// <summary>
    /// Логика взаимодействия для PromoteDialog.xaml
    /// </summary>
    public partial class PromoteDialog : Window
    {
        private PromotionType userPromotion;

        public PromoteDialog()
        {
            InitializeComponent();
        }

        public PromotionType getUserPromotion
        {
            get { return userPromotion; }
        }

        private void chooseRookButtonClick(object sender, EventArgs e)
        {
            userPromotion = PromotionType.ToRook;
            DialogResult = true;
        }

        private void chooseBishopButtonClick(object sender, EventArgs e)
        {
            userPromotion = PromotionType.ToBishop;
            DialogResult = true;
        }

        private void chooseKnightButtonClick(object sender, EventArgs e)
        {
            userPromotion = PromotionType.ToKnight;
            DialogResult = true;
        }

        private void chooseQueenButtonClick(object sender, EventArgs e)
        {
            userPromotion = PromotionType.ToQueen;
            DialogResult = true;
        }
    }
}
