using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
            TestColor = Color.FromArgb(255, 255, 255, 255);
        }
    }
}
