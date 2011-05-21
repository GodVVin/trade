using Trade.Core.Terminals;

namespace Trade.Alor
{
    public sealed class AlorTableDumper : AlorTable
    {
        public int Rows { get { return Data.Count; } }

        public AlorTableDumper(int slotid, string name)
            : base(slotid, name, true)
        {
            Connect();
            Disconnect();
        }

        protected override void OnRowUpdated(RowUpdateType updateType, int rowId)
        {
        }

        /// <summary>
        /// Получить значение ячейки таблицы
        /// </summary>
        /// <param name="rowid">Номер строки</param>
        /// <param name="fieldName">Имя поля</param>
        /// <returns>Значение ячейки</returns>
        public object Get(int rowid, string fieldName)
        {
            return GetValue(rowid, fieldName);
        }
    }
}
