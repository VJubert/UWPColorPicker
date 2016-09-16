using System.Diagnostics;
using Windows.UI;

namespace UWPColorPickerTesting
{
    public class TestingVM
    {
        private Color _testColor;

        public Color TestColor
        {
            get { return _testColor; }
            set
            {
                _testColor = value;
                Debug.WriteLine(value);
            }
        }

        public TestingVM()
        {
            TestColor = Colors.Yellow;
        }
    }
}
