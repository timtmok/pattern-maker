using System.Windows.Controls;
using System.Windows.Input;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PatternMaker;

namespace PatternMakerTest
{
    [TestClass]
    public class ModelTests
    {
        [TestMethod]
        public void CreateRectangleTest()
        {
            var rect = ViewModel.CreateRectangle();
            Assert.AreEqual(ViewModel.DEFAULT_FILL, rect.Fill);
            Assert.AreEqual(ViewModel.SQUARE_OPACITY, rect.Opacity);
            Assert.AreEqual(ViewModel.SQUARE_SIZE, rect.Height);
            Assert.AreEqual(ViewModel.SQUARE_SIZE, rect.Width);
            Assert.AreEqual(ViewModel.SQUARE_THICKNESS, rect.StrokeThickness);
        }

        [TestMethod]
        public void DefaultPatternTest()
        {
            MouseButtonEventHandler onRectClick = (o, args) =>
            {
            };
            MouseButtonEventHandler onRectRightClick = (o, args) =>
            {
            };

            var vm = new ViewModel(new Canvas(), onRectClick, onRectRightClick);
            vm.InitializePattern();

            Assert.AreEqual(96, vm.Row);
            Assert.AreEqual(256, vm.Col);
            Assert.AreEqual(100, vm.Zoom);
        }

        [TestMethod]
        [DeploymentItem("Data", "Data")]
        public void PatternLoadTest()
        {
            var serializer = new PatternSerializer();
            var model = serializer.Deserialize(@"Data\test1.ptn");

            Assert.AreEqual(5, model.Row);
            Assert.AreEqual(5, model.Col);
            Assert.AreEqual(200, model.Zoom);
            Assert.AreEqual(25, model.DotPattern.LongLength);
        }

        [TestMethod]
        [DeploymentItem("Data", "Data")]
        public void PatternLoadColourTest()
        {
            var serializer = new PatternSerializer();
            var model = serializer.Deserialize(@"Data\test2.ptn");

            Assert.AreEqual(1, model.Row);
            Assert.AreEqual(2, model.Col);
            Assert.AreEqual("FF00FF00", model.DotPattern[0, 0].Colour);
            Assert.AreEqual("FF0000FF", model.DotPattern[0, 1].Colour);
        }
    }
}
