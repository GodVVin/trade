using System;
using System.Windows.Forms;

namespace Trade.Views
{
    public partial class FormSelectSecurity : Form
    {
        public string Selected { get; private set; }

        public FormSelectSecurity(string[] securities)
        {
            InitializeComponent();
            _lbSecurities.Items.AddRange(securities);
            _lbSecurities.SelectedIndex = 0;
        }

        private void SelectClick(object sender, EventArgs e)
        {
            Selected = _lbSecurities.SelectedItem.ToString();
            Close();
        }

        private void SecuritiesDoubleClick(object sender, EventArgs e)
        {
            Selected = _lbSecurities.SelectedItem.ToString();
            Close();
        }
    }
}
