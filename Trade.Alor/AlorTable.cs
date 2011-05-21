using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using TEClientLib;
using Trade.Core;
using Trade.Core.Logging;
using Trade.Core.Terminals;

namespace Trade.Alor
{
    /// <summary>
    /// Базовый класс для взаимодействия с таблицей COM-сервера
    /// </summary>
    public abstract class AlorTable
    {
        protected int SlotId { get; private set; }

        protected string FieldNames;

        protected readonly Dictionary<int, object[]> Data = new Dictionary<int, object[]>();
        protected int DataCount { get { lock (_locker) { return Data.Count; } } }


        private readonly bool _loadAllDataOnOpen = true;

        /// <summary>
        /// True, если таблица полностью заполнена
        /// </summary>
        private bool _filled;

        private readonly SlotTable _slotTable = new SlotTable();
        private int _tblId;

        /// <summary>
        /// Имя таблицы
        /// </summary>
        private readonly string _tableName;

        private readonly string _name;

        private readonly Dictionary<string, int> _fieldIds = new Dictionary<string, int>();

        private AutoResetEvent _aeConnect;
        private AutoResetEvent _aeDisconnect;

        private readonly object _locker = new object();

        // For orderbook table
        private bool IsOrderbook { get; set; }
        private readonly string _secBoard;
        private readonly string _secCode;

        /// <summary>
        /// Подключается к таблице на сервере.
        /// </summary>
        /// <param name="slotId">Номер слота</param>
        /// <param name="name">Имя таблицы</param>
        /// <param name="loadAllDataOnOpen">Нужно ли подписываться на событие добавление столбца</param>
        protected AlorTable(int slotId, string name, bool loadAllDataOnOpen)
        {
            SlotId = slotId;
            _tableName = name;
            _name = name + "(" + SlotId + ")";
            _loadAllDataOnOpen = loadAllDataOnOpen;
            CreateEvents();
        }

        /// <summary>
        /// Подключается к таблице ORDERBOOK на сервере.
        /// </summary>
        /// <param name="slotId">Номер слота</param>
        /// <param name="secBoard"></param>
        /// <param name="secCode"></param>
        protected AlorTable(int slotId, string secBoard, string secCode)
        {
            SlotId = slotId;
            _secBoard = secBoard;
            _secCode = secCode;
            _tableName = "ORDERBOOK";
            _name = String.Format("ORDERBOOK ['{0}','{1}']", secBoard, secCode);
            IsOrderbook = true;
            _loadAllDataOnOpen = true;
            CreateEvents();
        }

        private void CreateEvents()
        {
            _slotTable.Format += SlotTableFormat;
            _slotTable.Opened += SlotTableOpened;
            _slotTable.Closed += SlotTableClosed;
            _slotTable.ReplEnd += SlotTableReplEnd;
            if (_loadAllDataOnOpen)
                _slotTable.AddRow += SlotTableAddRow;
            _slotTable.UpdateRow += SlotTableUpdateRow;
            _slotTable.DeleteRow += SlotTableDeleteRow;
            _slotTable.Clear += SlotTableClear;
            _slotTable.Error += SlotTableError;
        }

        /// <summary>
        /// Открывает таблицу и приостанавливает выполнение потока до получения события заполнения с таблицей.
        /// </summary>
        protected void Connect()
        {
            Log.Instance.Info("Open " + _name);
            var tStart = DateTime.Now;
            try
            {
                _aeConnect = new AutoResetEvent(false);
                lock (_locker)
                {
                    if (FieldNames != null) _slotTable.FieldNames = FieldNames;
                    if (IsOrderbook)
                        _tblId = _slotTable.OpenOrderbook(SlotId, _secBoard, _secCode);
                    else
                        _tblId = _slotTable.Open(SlotId, _tableName);
                }
                _aeConnect.WaitOne(1200000, false); // Ждать до ae.Set()
                _aeConnect = null;
                if (!_filled) throw new AlorException("Превышено время ожидания подключения");
            }
            catch (COMException e)
            {
                throw new AlorException("Ошибка соединения с терминалом. " + e.Message);
            }
#if DEBUG
            var tm = DateTime.Now - tStart;
            //L.Instance.Trace("************************** open time: " + tm);
#endif
        }

        /// <summary>
        /// Закрывает таблицу.
        /// </summary>
        protected void Disconnect()
        {
            //L.Instance.Info("Close " + _name);
            _aeDisconnect = new AutoResetEvent(false);
            try
            {
                _slotTable.Close(_tblId);
            }
            catch (COMException e)
            {
                Log.Instance.Error(e.Message);
                _aeDisconnect.Set();
            }
            _aeDisconnect.WaitOne(500, false);
            _aeDisconnect = null;
        }

        protected virtual void OnRowUpdated(RowUpdateType updateType, int rowId)
        {
        }

        protected IEnumerable<int> GetRowIds()
        {
            lock (_locker)
                return new List<int>(Data.Keys);
        }

        /// <summary>
        /// Находит строку таблицы по значению поля.
        /// </summary>
        /// <param name="fieldName">Имя поля</param>
        /// <param name="fieldValue">Значение поля</param>
        /// <returns>Номер строки таблицы</returns>
        protected int GetRowId(string fieldName, object fieldValue)
        {
            int rowidFound = -1;
            lock (_locker)
            {
                foreach (var rowid in Data.Keys)
                    if (GetValue(rowid, fieldName).Equals(fieldValue))
                        rowidFound = rowid;
            }
            return rowidFound;
        }

        /// <summary>
        /// Находит строку таблицы
        /// </summary>
        /// <param name="fieldName">Имя поля</param>
        /// <param name="func">Функция сравнения значения поля</param>
        /// <returns>Номер строки таблицы</returns>
        protected int GetRowId(string fieldName, Func<object, bool> func)
        {
            lock (_locker)
                return Data.Keys.First(rowId => func(GetValue(rowId, fieldName)));
        }

        /// <summary>
        /// Находит значение поля в строке таблицы.
        /// </summary>
        /// <param name="rowId">Номер строки</param>
        /// <param name="fieldName">Имя поля</param>
        /// <returns>Значение поля</returns>
        protected object GetValue(int rowId, string fieldName)
        {
            return GetValue(rowId, _fieldIds[fieldName]);
        }

        protected object GetValue(int rowId, int fieldId)
        {
            lock (_locker)
                return Data[rowId][fieldId];
        }

        #region Events

        private void SlotTableFormat(int openId, object names, object formats)
        {
            if (names == null)
                return;

            var fields = ((object[])names).Cast<string>().ToArray();

            for (int i = 0; i < fields.Length; i++)
                if (!_fieldIds.ContainsKey(fields[i]))
                    _fieldIds.Add(fields[i], i);

        }

        private void SlotTableOpened(int openId)
        {
            //L.Instance.Info(_name + ": Opened");
        }

        private void SlotTableReplEnd(int openId)
        {
            _filled = true;
            //L.Instance.Info(_name + ": ReplEnd " + DataCount + " rows");
            if (!_loadAllDataOnOpen)
                _slotTable.AddRow += SlotTableAddRow;
            if (_aeConnect != null)
                _aeConnect.Set();
            else
            {
                lock (_locker)
                {
                    OnRowUpdated(RowUpdateType.Refreshed, 0);
                }
            }
        }

        private void SlotTableClosed(int openId)
        {
            Log.Instance.Info(_name + ": Closed");
            if (_aeDisconnect != null) _aeDisconnect.Set();
        }

        private void SlotTableAddRow(int openId, int rowid, object fields)
        {
            lock (_locker)
            {
                //L.Instance.Info(_name + ": AddRow Filled=" + Filled);
                if (Data.ContainsKey(rowid))
                    Data[rowid] = (object[])fields;
                else
                    Data.Add(rowid, (object[])fields);
            }
            if (_filled) OnRowUpdated(RowUpdateType.Added, rowid);
        }

        private void SlotTableUpdateRow(int openId, int rowid, object fields)
        {
            lock (_locker)
            {
                //L.Instance.Info(_name + ": UpdateRow Filled=" + Filled);
                if (Data.ContainsKey(rowid))
                    Data[rowid] = (object[])fields;
                else
                    Data.Add(rowid, (object[])fields);
            }
            if (_filled) OnRowUpdated(RowUpdateType.Updated, rowid);
        }

        private void SlotTableDeleteRow(int openId, int rowid)
        {
            lock (_locker)
            {
                Log.Instance.Debug(_name + ": DeleteRow " + rowid);
                Data.Remove(rowid);
            }
            OnRowUpdated(RowUpdateType.Deleted, rowid);
        }

        private void SlotTableClear(int openId)
        {
            lock (_locker)
            {
                //L.Instance.Info(_name + ": Clear (was " + DataCount + " rows)");
                Data.Clear();
                _filled = false;
            }
        }

        private void SlotTableError(int openId, int code, string description)
        {
            if (_aeConnect != null)
                _aeConnect.Set();
            Log.Instance.Error(_name + ": Error " + description);
        }

        #endregion

    }

}
