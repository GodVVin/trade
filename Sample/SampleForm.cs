using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Trade.Alor;
using Trade.Alor.Views;
using Trade.Core;
using Trade.Core.Logging;
using Trade.Core.Stock;
using Trade.Core.Terminals;

namespace Sample
{
    public partial class SampleForm : Form
    {
        public SampleForm()
        {
            InitializeComponent();
        }

        private ITerminal _terminal;

        private readonly List<IOrderbook> _orderbooks = new List<IOrderbook>();

        #region UI events

        private void SampleFormFormClosing(object sender, FormClosingEventArgs e)
        {
            Disconnect();
        }

        private void ButtonConnectClick(object sender, EventArgs e)
        {
            Connect();
        }

        private void ButtonDisconnectClick(object sender, EventArgs e)
        {
            Disconnect();
        }

        #endregion

        #region Terminal events

        void TerminalSecuritiesUpdated(object sender, SecuritiesUpdatedEventArgs e)
        {
            var msg = string.Format("\"{0}\", \"{1}\", last: {2}, bid: {3}, ask: {4}",
                                    e.Security.ShortName, e.Security.SecCode, e.Security.LastDeal,
                                    e.Security.Bid, e.Security.Ask);
            Message(msg);
        }

        void OrderbookUpdated(object sender, OrderbookUpdatedEventArgs e)
        {
            var orderbook = sender as AlorOrderbook;
            Debug.Assert(orderbook != null);
            var rows = e.Rows;
            Message("orderbook updated " + orderbook.SecCode + " rows: " + rows.Length);
        }

        #endregion

        private void Connect()
        {
            _terminal = ConnectAlor();
            _terminal.SecuritiesUpdated += TerminalSecuritiesUpdated;
            try
            {
                _terminal.Connect();
                Message("Connected.");

                OpenOrderbooks();
            }
            catch (Exception ex)
            {
                Message("Connect error: " + ex.Message);
                MessageBox.Show("Connect error.");
            }
        }

        private ITerminal ConnectAlor()
        {
            var terminal = new AlorTerminal();
            terminal.SelectSlotDialog();
            Message("Connecting to slot " + terminal.SelectedSlot);
            return terminal;
        }

        private void Disconnect()
        {
            foreach (var orderbook in _orderbooks)
                if (orderbook != null)
                    orderbook.Close();
            _orderbooks.Clear();

            if (_terminal != null)
                _terminal.Disconnect();
            _terminal = null;
        }

        private void OpenOrderbooks()
        {
            //var names = new[] {"EuM1", "EDM1", "LKM1", "SRM1", "RIM1", "VBM1", "SiM1", "GZM1", "GDM1", "GUM1", "AUM1", "BRM1"};
            var names = new[] {"ЛУКОЙЛ", "+МосЭнерго", "РАО_ЕЭС"};
            var securities = new List<Security>(names.Length);

            foreach (var secCode in _terminal.Securities.Securities)
            {
                if (names.Contains(_terminal.Securities[secCode].ShortName))
                    securities.Add(_terminal.Securities[secCode]);
            }

            foreach (var security in securities)
            {
                Message(security.ToString());

                var orderbook = _terminal.OpenOrderbook(security);
                orderbook.Updated += OrderbookUpdated;
                _orderbooks.Add(orderbook);
            }

            foreach (var orderbook in _orderbooks)
                orderbook.Open();
        }

        private void Message(string message)
        {
            Log.Instance.Info(message);
            if (InvokeRequired)
                BeginInvoke(new Action(() => MessageUi(message)));
            else
                MessageUi(message);
        }

        private void MessageUi(string message)
        {
            _textLog.Text += message + Environment.NewLine;

            var len = _textLog.Text.Length;

            _textLog.SelectionStart = len;
            _textLog.ScrollToCaret();
        }
    }
}
