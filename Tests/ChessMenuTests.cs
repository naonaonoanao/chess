using System.Windows.Controls;
using uwp;
using System.Windows.Media;

namespace Tests
{
    [TestFixture, Apartment(ApartmentState.STA)]
    public class ChessMenuTests
    {
        private MenuView menuView;

        [SetUp]
        public void Setup()
        {
            menuView = new MenuView();
        }

        [Test]
        public void Difficulty_SelectionChanged_SetIsMinimaxToTrue_WhenItemSelectedIsHard()
        {
            var comboBox = new ComboBox();
            var comboBoxItem = new ComboBoxItem();
            comboBoxItem.Content = "Сложный";
            comboBox.Items.Add(comboBoxItem);

            menuView.Difficulty_SelectionChanged(comboBox, null);

            Assert.IsTrue(menuView.isMinimax);
        }


        [Test]
        public void Button_MouseEnter_ChangesBorderBrush_ToYellow()
        {
            var button = new Button();

            menuView.Button_MouseEnter(button, null);

            Assert.AreEqual(Colors.Yellow, (button.BorderBrush as SolidColorBrush).Color);
        }

        [Test]
        public void Button_MouseLeave_ResetsBorderBrush_ToTransparent()
        {
            var button = new Button();
            button.BorderBrush = Brushes.Yellow;

            menuView.Button_MouseLeave(button, null);

            Assert.AreEqual(Colors.Transparent, (button.BorderBrush as SolidColorBrush).Color);
        }

        [Test]
        public void ToggleHighlight_HighlightsButton_WhenCalled()
        {
            var button = new Button();

            menuView.ToggleHighlight(button);

            Assert.IsTrue(menuView.isButtonHighlighted);
            Assert.AreEqual(Colors.Yellow, (button.BorderBrush as SolidColorBrush).Color) ;
        }

        [Test]
        public void ToggleHighlight_UnhighlightsButton_WhenCalledTwice()
        {
            var button = new Button();

            menuView.ToggleHighlight(button);
            menuView.ToggleHighlight(button);

            Assert.IsFalse(menuView.isButtonHighlighted);
            Assert.AreEqual(Colors.Transparent, (button.BorderBrush as SolidColorBrush).Color);
        }

        [Test]
        public void ResetColorSelection_ResetsButtonHighlight()
        {
            menuView.isButtonHighlighted = true;
            menuView.lastClickedButton = new Button();

            menuView.ResetColorSelection();

            Assert.IsFalse(menuView.isButtonHighlighted);
            Assert.IsNull(menuView.lastClickedButton);
        }
    }
}
