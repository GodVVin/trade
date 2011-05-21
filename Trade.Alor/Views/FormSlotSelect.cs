using System;
using System.Windows.Forms;

namespace Trade.Alor.Views
{
    public partial class FormSlotSelect : Form
    {
        public FormSlotSelect(AlorTerminal terminal)
        {
            _terminal = terminal;
            InitializeComponent();
        }

        private readonly AlorTerminal _terminal;

        private void FormSlotSelectLoad(object sender, EventArgs e)
        {
            var slots = _terminal.GetSlots();
            foreach (var slot in slots)
            {
                _slots.Items.Add(String.Format("{0} {1} | {2}", slot.Ready ? ">" : "  ", slot.Id, slot.Login));
            }
            _slots.SelectedIndex = 0;
        }

        private void ButtonConnectClick(object sender, EventArgs e)
        {
            _terminal.SelectedSlot = _slots.SelectedIndex;
            Close();
        }
    }
}
