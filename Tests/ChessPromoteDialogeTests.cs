using Chess;
using chess;

namespace Tests
{
    [TestFixture, Apartment(ApartmentState.STA)]
    public class PromoteDialogTests
    {
        [Test]
        public void ChooseRookButtonClick_SetsUserPromotionToRook()
        {
            var dialog = new PromoteDialog(true);

            dialog.chooseRookButtonClick(null, null);

            Assert.AreEqual(PromotionType.ToRook, dialog.getUserPromotion);
        }

        [Test]
        public void ChooseBishopButtonClick_SetsUserPromotionToBishop()
        {
            var dialog = new PromoteDialog(true);

            dialog.chooseBishopButtonClick(null, null);

            Assert.AreEqual(PromotionType.ToBishop, dialog.getUserPromotion);
        }

        [Test]
        public void ChooseKnightButtonClick_SetsUserPromotionToKnight()
        {
            var dialog = new PromoteDialog(true);

            dialog.chooseKnightButtonClick(null, null);

            Assert.AreEqual(PromotionType.ToKnight, dialog.getUserPromotion);
        }

        [Test]
        public void ChooseQueenButtonClick_SetsUserPromotionToQueen()
        {
            var dialog = new PromoteDialog(true);

            dialog.chooseQueenButtonClick(null, null);

            Assert.AreEqual(PromotionType.ToQueen, dialog.getUserPromotion);
        }
    }
}
