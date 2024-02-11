namespace DataSource;

internal struct DataSourceCacheContainer
{
    public DateTime TimeStamp { get; set; }
    public DataQueryResult Result { get; set; }
    //Need to add method for getting data here. It might be needed when getting data source which needs it,
    //e.g. transactions need groups (is it the only case?).
    //It seems that dependency map needs to be defined before before any datasource is created,
    //so when document is changed - group is updated - transaction is updated. Can this result in recurrency?
    
}
