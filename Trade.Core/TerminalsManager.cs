using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using AlorLib;
using AlorLib.Views;
using Intraday.Properties;
using OECLib;
using OECLib.Views;
using Trade;
using Trade.Core;
using Trade.Core.Terminals;

namespace Intraday
{
    public class TerminalsManager
    {
        public Form OwnerForm { get; set; }

        public ConfigIntraday Config { get; set; }

        public ITerminal CreateTerminal(TerminalType type)
        {
            switch (type)
            {
                case TerminalType.AlorTrade:
                    return new AlorTerminal();
                case TerminalType.Oec:
                    return new OecTerminal();
            }
            throw new InvalidOperationException();
        }

        public void InitTerminal(ITerminal terminal)
        {
            switch (terminal.Type)
            {
                case TerminalType.AlorTrade:
                    InitAlorTerminal(terminal as AlorTerminal, -1);
                    break;
                case TerminalType.Oec:
                    InitOecTerminal(terminal as OecTerminal);
                    break;
            }
        }

        private void InitAlorTerminal(AlorTerminal terminal, int slotId)
        {
            if (slotId == -1)
                terminal.SelectedSlot = SelectSlot(terminal.GetSlots());
            else
                terminal.SelectedSlot = slotId;
        }

        private void InitOecTerminal(OecTerminal terminal)
        {
            var f = new LoginForm();
            f.Owner = OwnerForm;
            f.Login = Config.OecLogin;
            f.Password = Config.OecPassword;
            f.ShowDialog();
            var username = f.Login;
            var password = f.Password;
            Config.OecLogin = username;
            Config.OecPassword = password;
            ConfigIntraday.Save("config.xml", Config); // TODO filename
            terminal.Login = username;
            terminal.Password = password;
            //terminal.Host = "127.0.0.1";
        }

        private int SelectSlot(IEnumerable<SlotInfo> slots)
        {
            int slotId;
            using (var f = new FormSlotSelect(slots))
            {
                f.Owner = OwnerForm;
                f.ShowDialog();
                slotId = (int)f.Tag;
            }
            return slotId;
        }
    }
}
