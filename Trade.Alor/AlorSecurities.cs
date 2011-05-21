using System;
using System.Collections.Generic;
using System.Linq;
using Trade.Core;
using Trade.Core.Stock;
using Trade.Core.Terminals;

namespace Trade.Alor
{
    public sealed class AlorSecurities : AlorTable, ISecurities
    {
        public AlorSecurities(int slotId)
            : base(slotId, "SECURITIES", true)
        {
        }

        #region Private properties

        private enum FieldIndex { ID = 0, SecBoard, SecCode, ShortName, MinStep, StepPrice, Bid, Offer, BidDepth, OfferDepth, Last, OptionType, Strike, FutCode, Volatility };

        private readonly object _rowsLock = new object();

        private Dictionary<int, Security> _rows;

        #endregion

        #region Члены ISecurities

        public event EventHandler<SecuritiesUpdatedEventArgs> UpdatedEvent;

        public void Open()
        {
            FieldNames = "ID,SecBoard,SecCode,ShortName,MinStep,StepPrice,Bid,Offer,BidDepth,OfferDepth,Last,OptionType,Strike,FutCode,Volatility";
            Connect();
            InitRows();
        }

        public void Close()
        {
            lock (_rowsLock)
                _rows = null;
            Disconnect();
        }

        public int Count
        {
            get { lock (_rowsLock) return _rows.Count; }
        }

        public Security this[string secCode]
        {
            get { lock (_rowsLock) { return _rows[FindRow(secCode)]; } }
        }

        public string[] Securities
        {
            get { return _rows.Values.Select(s => s.SecCode).ToArray(); }
        }

        #endregion

        protected override void OnRowUpdated(RowUpdateType updateType, int rowId)
        {
            //WriteLine("Securities updated " + Rows[LastUpdatedRow] + " " + Rows.Count);
            if (_rows == null) return;

            if (updateType == RowUpdateType.Updated)
            {
                //lock (_rowsLock)
                    SetRow(rowId);
            }
            else
                throw new InvalidOperationException("Added or deleted row in SECUTIRIES");
            var security = _rows[rowId];
            if (UpdatedEvent != null)
                UpdatedEvent(this, new SecuritiesUpdatedEventArgs(security.Updated, security));
        }

        private void InitRows()
        {
            lock (_rowsLock)
            {
                _rows = new Dictionary<int, Security>();
                var ids = GetRowIds();
                foreach (int rowid in ids)
                {
                    _rows.Add(rowid, new Security());
                    SetRow(rowid);
                }
            }
        }

        private void SetRow(int rowid)
        {
            if (_rows == null)
                return;
            if (!_rows.ContainsKey(rowid))
                throw new ArgumentException("Rows doesn't contain key " + rowid);
            _rows[rowid].SecBoard = (string) GetValue(rowid, (int) FieldIndex.SecBoard);
            _rows[rowid].SecCode = (string) GetValue(rowid, (int) FieldIndex.SecCode);
            _rows[rowid].ShortName = (string) GetValue(rowid, (int) FieldIndex.ShortName);

            _rows[rowid].MinStep = (double) GetValue(rowid, (int) FieldIndex.MinStep);
            _rows[rowid].StepPrice = (double) GetValue(rowid, (int) FieldIndex.StepPrice);

            _rows[rowid].Bid = (double) GetValue(rowid, (int) FieldIndex.Bid);
            _rows[rowid].Ask = (double) GetValue(rowid, (int) FieldIndex.Offer);
            _rows[rowid].BidDepth = (int) GetValue(rowid, (int) FieldIndex.BidDepth);
            _rows[rowid].AskDepth = (int) GetValue(rowid, (int) FieldIndex.OfferDepth);
            _rows[rowid].LastDeal = (double) GetValue(rowid, (int) FieldIndex.Last);

            _rows[rowid].OptionType = (string) GetValue(rowid, (int) FieldIndex.OptionType);
            _rows[rowid].Strike = (double) GetValue(rowid, (int) FieldIndex.Strike);
            _rows[rowid].FutCode = (string) GetValue(rowid, (int) FieldIndex.FutCode);
            _rows[rowid].Volatility = (double) GetValue(rowid, (int) FieldIndex.Volatility);

            _rows[rowid].Updated = DateTime.Now; // TODO get update time from terminal
        }

        private int FindRow(string secCode)
        {
            return GetRowId("SecCode", secCode);
            //return GetRowId("SecCode", code => code.ToString().Trim() == secCode.Trim());
        }
    }
}
