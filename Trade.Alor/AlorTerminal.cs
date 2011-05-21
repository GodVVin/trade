using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using TEClientLib;
using Trade.Alor.Views;
using Trade.Core;
using Trade.Core.Logging;
using Trade.Core.Orders;
using Trade.Core.Stock;
using Trade.Core.Terminals;

namespace Trade.Alor
{
    public struct SlotInfo
    {
        public int Id { get; set; }
        public string Caption { get; set; }
        public string Server { get; set; }
        public string Login { get; set; }
        public string State { get; set; }
        public string BrokerRef { get; set; }
        public bool Ready { get; set; }
        public override string ToString()
        {
            return String.Format("{0}\t{1}\t{2}\t{3}",
                Id, State, Server, Login);
        }
    }

    public class AlorTerminal : ITerminal
    {
        public TerminalType Type { get { return TerminalType.AlorTrade; } }

        public bool Reconnect { get; set; }

        public string Server { get; set; }
        public string Login { get; set; }
        public string Password { private get; set; }

        public int SelectedSlot
        {
            get { return _selectedSlot; }
            set
            {
                _selectedSlot = value; SelectedSlotChanged();
                _slot = _sf.GetSlot(_selectedSlot) as Slot;
            }
        }

        public ISecurities Securities { get; private set; }
        public IOrders Orders { get; private set; }
        public ITrades Trades { get; private set; }
        public OrdersManager OrdersManager { get; private set; }

        public event EventHandler<EventArgs> Connected;
        public event EventHandler<EventArgs> Disconnected;
        public event EventHandler<EventArgs> Error;
        public event EventHandler<SecuritiesUpdatedEventArgs> SecuritiesUpdated;
        public event EventHandler<OrdersUpdatedEventArgs> OrdersUpdated;
        public event EventHandler<TradesUpdatedEventArgs> TradesUpdated;

        public AlorTerminal()
        {
            Reconnect = true;
            try
            {
                Init();
            }
            catch (COMException)
            {
                throw new TradeException();
            }
        }

        private void Init()
        {
            _sf = new SlotFace();
            _sf.Disconnected += OnDisconnected;
            _sf.Synchronized += OnSynchronized;
            _sf.Error += OnError;

            _slots = new SlotInfo[_sf.MaxSlotID + 1];
            for (int i = 0; i <= _sf.MaxSlotID; i++)
                _slots[i] = new SlotInfo
                {
                    Id = i,
                    Caption = _sf.Caption[i],
                    Server = _sf.Server[i],
                    Login = _sf.Login[i],
                    State = _sf.State[i],
                    BrokerRef = _sf.BrokerRef[i],
                    Ready = _sf.State[i] == "Готово"
                };
            _selectedSlot = -1;
        }

        public bool ConnectServer()
        {
            _sf.Open(SelectedSlot);
            string msg;
            var res = _sf.Connect(SelectedSlot, Server, Login, Password, out msg);
            Log.Instance.Debug("Connect result {0} ({1})", res, msg);
            if (res == (int) SFE.SFE_OK)
                return true;

            return false;
        }

        private void OnDisconnected(int openid, int slotid, int resultcode, string resultmsg)
        {
            Log.Instance.Debug("OnDisconnected slot: {0}, result: {1} ({2})", slotid, resultcode, resultmsg);
            if (Disconnected != null)
                Disconnected(this, EventArgs.Empty);
        }

        private void OnSynchronized(int openid, int slotid)
        {
            Log.Instance.Debug("OnSynchronized slot: {0}", slotid);
            if (Connected != null)
                Connected(this, EventArgs.Empty);
        }

        private void OnError(int openid, int slotid, int code, string description)
        {
            Log.Instance.Debug("OnError slot: {0}, code: {1} ({2})", slotid, code, description);
            if (Error != null)
                Error(this, EventArgs.Empty);
        }

        /// <summary>
        /// Подключает необходимые таблицы, заполняет их и инициализирует обработчики событий обновления таблиц.
        /// </summary>
        public void Connect()
        {
            Log.Instance.Info("AlorTerminal Connecting to slot " + SelectedSlot);
            if (_slot == null)
                throw new AlorException("Не выбран слот");

            if (_slot.GetCurrentState() == (int)SS.SS_DISCONNECTED)
                throw new AlorException("Слот не подключен");

            _slotOpenId = _sf.Open(SelectedSlot);

            _trdacc = new AlorTableDumper(SelectedSlot, "TRDACC");
            if (_trdacc.Rows > 0)
                _account = (string) _trdacc.Get(0, "Account");
            _brokerRef = _slots[SelectedSlot].BrokerRef;

            Securities = new AlorSecurities(SelectedSlot);
            Securities.UpdatedEvent += SecuritiesUpdatedEvent;
            Securities.Open();
        }

        /// <summary>
        /// Закрывает таблицы.
        /// </summary>
        public void Disconnect()
        {
            if (Orders != null) Orders.Close();
            if (Trades != null) Trades.Close();
            if (Securities != null) Securities.Close();
            try
            {
                _sf.Close(_slotOpenId);
            }
            catch (COMException)
            {
                throw new TradeException();
            }
        }

        public void OpenOrders()
        {
            Orders = new AlorOrders(SelectedSlot, _account, _brokerRef);
            Orders.Updated += OrdersUpdatedEvent;
            Orders.Open();
            Trades = new AlorTrades(SelectedSlot);
            Trades.Updated += TradesUpdatedEvent;
            Trades.Open();
            OrdersManager = new OrdersManager(Orders, Trades, Securities);
        }

        public IOrderbook OpenOrderbook(Security security)
        {
            return new AlorOrderbook(SelectedSlot, security.SecBoard, security.SecCode);
        }

        public AlorOrderbook OpenOrderBook(string secBoard, string secCode)
        {
            var orderBook = new AlorOrderbook(SelectedSlot, secBoard, secCode);
            orderBook.Open();
            return orderBook;
        }


        public IEnumerable<SlotInfo> GetSlots() { return new List<SlotInfo>(_slots); }

        public void SelectSlotDialog()
        {
            using (var form = new FormSlotSelect(this))
                form.ShowDialog();
        }

        private SlotFace _sf;
        private Slot _slot;
        private int _slotOpenId;

        private SlotInfo[] _slots;
        private int _selectedSlot;

        private AlorTableDumper _trdacc;

        private string _account;
        private string _brokerRef;

        private void SelectedSlotChanged()
        {
            Login = _slots[_selectedSlot].Login;
            Server = _slots[_selectedSlot].Server;
        }

        private void SecuritiesUpdatedEvent(object sender, SecuritiesUpdatedEventArgs e)
        {
            if (SecuritiesUpdated != null) SecuritiesUpdated(this, e);
        }

        private void OrdersUpdatedEvent(object sender, OrdersUpdatedEventArgs e)
        {
            if (OrdersUpdated != null) OrdersUpdated(this, e);
        }

        void TradesUpdatedEvent(object sender, TradesUpdatedEventArgs e)
        {
            if (TradesUpdated != null) TradesUpdated(this, e);
        }
    }
}
