using FinancesBlazor.DataAccess;

namespace FinancesBlazor.Components.Grid
{
    public class GridColumn
    {
        public string Title { get; }

        public string Data { get; }

        public DataTypes DataType { get; }

        public string NullValue { get; }

        public string? Format { get; }

        public GridColumn(string title, string data, DataTypes dataType, string nullValue = "", string? format = null)
        {
            Title = title;
            Data = data;    
            DataType = dataType;
            NullValue = nullValue;
            Format = format;
        }
    }
}
