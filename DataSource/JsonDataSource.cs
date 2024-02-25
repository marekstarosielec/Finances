using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace DataSource;

public class JsonDataSource : IDataSource
{
    private readonly string _fileName;
    
    protected static Dictionary<string, SemaphoreSlim> _semaphores = new ();
    
    public Dictionary<string, DataColumn> Columns { get; private set; }

    public string Id => _fileName;

    private readonly DataSourceCacheStamp _cacheStamp;

    private readonly DataSourceCacheStamp? _groupStamp;
    private readonly DataQueryExecutor _dataQueryExecutor = new();

    public JsonDataSource(string path, string fileName, params DataColumn[] dataColumns)
    {
        if (string.IsNullOrWhiteSpace(fileName)) 
            throw new ArgumentException("FileName cannot be whitespace");

        
        Columns = dataColumns.ToDictionary(c => c.ColumnName, c => c);
        _fileName = Path.Combine(path, fileName);

        _cacheStamp = DataSourceCache.Instance.Register(Id, Load);

        var groupColumn = dataColumns.FirstOrDefault(GroupDataColumn.IsGroupColumn);
        if (groupColumn != null)
        {
            GroupDataSource.Create(path); //Needed to create instance of group data source
            _groupStamp = new DataSourceCacheStamp(new List<string> { GroupDataSource.Id });
        }
    }

    public Task<DataQueryResult> ExecuteQuery(DataQuery dataQuery)
        => _dataQueryExecutor.ExecuteQuery(Id, _cacheStamp, dataQuery, _groupStamp);

    public async Task Save(List<DataRow> rows)
    {
        var allDataRows = (await DataSourceCache.Instance.Get(Id, _cacheStamp)).Rows.ToList();
        foreach (DataRow row in rows)
        {
            var originalRow = allDataRows.FirstOrDefault(r => r.Id?.OriginalValue == row.Id.OriginalValue);
            if (originalRow == null)
            {
                //New row created
                var newRow = new DataRow();
                //Copy new values
                foreach (var cell in row)
                    newRow[cell.Key] = new DataValue(row[cell.Key].CurrentValue, row[cell.Key].CurrentValue);
                allDataRows.Add(newRow);
            }
            else
            {
                //Copy modified values to main storage
                foreach (var cell in row)
                {
                    if (cell.Value.CurrentValue == cell.Value.OriginalValue)
                        continue;

                    originalRow[cell.Key].CurrentValue = row[cell.Key].CurrentValue;
                }
            }
        }

        //Build structure for serialization.
        var nodes = new List<Dictionary<string, object?>>();
        foreach (var saveRow in allDataRows)
        {
            var node = new Dictionary<string, object?>();
            foreach (var column in Columns.Where(c => c.Value.CustomCreator == null)) //Do not save changes to calculated columns
            {
                node[column.Key] = saveRow[column.Key]?.CurrentValue;
            }
            nodes.Add(node);
        }

        var result = JsonSerializer.Serialize(nodes);
        var _semaphore = GetSemaphore();

        try
        {
            _semaphore.Wait();
            if (!File.Exists(_fileName))
                await File.WriteAllTextAsync(_fileName, "[]");
            await File.WriteAllTextAsync(_fileName, result);
        }
        catch (Exception ex)
        {
            //TODO: restore data?
            return;
        }
        finally
        {
            _semaphore.Release();
        }
    }

    public void RemoveCache()
    {
        DataSourceCache.Instance.Clean(Id);
    }

    private async Task<DataQueryResult> Load()
    {
        var _semaphore = GetSemaphore();

        try
        {
            var watch = System.Diagnostics.Stopwatch.StartNew();
            _semaphore.Wait();
            if (!File.Exists(_fileName))
                await File.WriteAllTextAsync(_fileName, "[]");
            var json = await File.ReadAllTextAsync(_fileName, Encoding.Latin1);
            if (string.IsNullOrWhiteSpace(json))
                throw new InvalidOperationException("Input json evaluated to null");
            Console.WriteLine($"Json file loading: {watch.ElapsedMilliseconds} ms");

            watch.Restart();
            var nodes = JsonNode.Parse(json)?.AsArray()?.Where(a => a != null)?.Select(a => a!)?.ToList();
            if (nodes == null)
                throw new InvalidOperationException("Input json evaluated to null");
            Console.WriteLine($"Json file parsing: {watch.ElapsedMilliseconds} ms");

            var dataRows = new List<DataRow>();
            foreach (var node in nodes)
            {
                var dataRow = new DataRow();
                foreach (var column in Columns)
                    try
                    {
                        if (column.Value.CustomCreator != null)
                        {
                            var result = column.Value.CustomCreator(dataRow);
                            dataRow[column.Value.ColumnName] = new DataValue(result, result);
                        }
                        else if (column.Value.ColumnDataType != ColumnDataType.Subquery) //Subqueries are not stored directly in source.
                        {
                            object? value = column.Value.ColumnDataType switch
                            {
                                ColumnDataType.Text => node[column.Key]?.GetValue<string>(),
                                ColumnDataType.Date => node[column.Key]?.GetValue<DateTime>(),
                                ColumnDataType.Precision => node[column.Key]?.GetValue<decimal>(),
                                ColumnDataType.Number => node[column.Key]?.GetValue<int>(),
                                _ => throw new Exception("Unsupported DataType")
                            };
                            dataRow[column.Value.ColumnName] = new DataValue(value, value);
                        }
                    }
                    catch (Exception ex)
                    {
                        throw new InvalidCastException($"Failed to convert {column.Key}", ex);
                    }
                dataRows.Add(dataRow);
            }

            return new DataQueryResult(Columns.Values, dataRows, dataRows.Count);
        }
        finally
        {
            _semaphore.Release();
        }
    }

    private SemaphoreSlim GetSemaphore()
    {
        if (!_semaphores.ContainsKey(_fileName))
            _semaphores[_fileName] = new SemaphoreSlim(1); 
        return _semaphores[_fileName];
    }
}