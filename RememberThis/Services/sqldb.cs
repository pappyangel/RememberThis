using System.Collections.Generic;
using System.Data;
using Microsoft.Data.SqlClient;
using System.Text.Json;
using System.Threading.Tasks;
using SharedModels;
using Microsoft.Extensions.Configuration;

namespace RememberThis.Services;

public class SqlDb
{
    private readonly ILogger<SqlDb> _logger;
    private readonly IConfiguration _configuration;

    private List<rtItem> sqlrtItems = new();
    private string tblName = "Items";
    private string viewName = "vw_Items";    
    
    //This line is brioken.  Needs to be a valid select selecing the columns
    private string selectClause = String.Empty;

    public SqlDb(ILogger<SqlDb> logger, IConfiguration configuration)    
    {
        _logger = logger;
        _configuration = configuration;

    }

    private string GetSQLCnStr()
    {
        var builder = new SqlConnectionStringBuilder(
            _configuration["ConnectionStrings:defaultSQLConnection"]);

        // var keyVaultSecretLookup = _configuration["AzureKeyVaultSecret:defaultSecret"];
        builder.Password = _configuration.GetValue<string>("SQLPW");
        
        return builder.ConnectionString;
    }


    public async Task<int> ExecuteQueryAsync(string qry, string UserObjectId)
    {
        int queryReturnCode = 1;        
        
            
        using SqlConnection SQLCn = new SqlConnection(GetSQLCnStr());        
        using SqlCommand command = new SqlCommand(qry, SQLCn);
        command.CommandType = CommandType.StoredProcedure;
        command.Parameters.Add("@selecteduser", SqlDbType.VarChar, 100).Value = UserObjectId;

        await SQLCn.OpenAsync();
        SqlDataReader dataReader = await command.ExecuteReaderAsync();

        while (dataReader.Read())
        {
            sqlrtItems.Add(new rtItem()
            {
                rtId = dataReader.GetInt32(0),
                rtUserObjectId = dataReader.GetString(1),
                rtDescription = dataReader.GetString(2),
                rtLocation = dataReader.GetString(3),
                rtDateTime = dataReader.GetDateTime(4),
                rtImagePath = dataReader.GetString(5)
            });
        }

        // Tim beleives these are superfluous
        dataReader.Close();
        command.Dispose();
        SQLCn.Close();

        return queryReturnCode;

    }
    public async Task<List<rtItem>> GetrtItemsById(int id, string UserObjectId)
    {
        string qryId = selectClause + $"from {viewName} where Id = {id} order by Id";

        await ExecuteQueryAsync(qryId, UserObjectId);

        return sqlrtItems;

    } // end get by Id

    public async Task<List<rtItem>> GetAllItemsbyUser(string UserObjectId)
    {
        // define variables          
        string spGetAllItemsbyUser = "usp_GetAllItemsbyUser";

        await ExecuteQueryAsync(spGetAllItemsbyUser, UserObjectId);

        return sqlrtItems;

    }
    private async Task<int> CRUDAsync(string sqlStatetment, rtItem rtItem)
    {
        int rowsAffected = 0;

        using SqlConnection SQLCn = new SqlConnection(GetSQLCnStr());

        using SqlCommand crudCommand = new SqlCommand(sqlStatetment, SQLCn);
        crudCommand.CommandType = CommandType.Text;

        bool IgnoreCase = true;
        if (sqlStatetment.StartsWith("D",IgnoreCase, null) | sqlStatetment.StartsWith("U",IgnoreCase, null))
            crudCommand.Parameters.Add("@ItemId", SqlDbType.Int).Value = rtItem.rtId;
        
        if (sqlStatetment.StartsWith("I",IgnoreCase, null) | sqlStatetment.StartsWith("U",IgnoreCase, null))
        {
            crudCommand.Parameters.Add("@rtUserObjectId", SqlDbType.VarChar, 100).Value = rtItem.rtUserObjectId;
            crudCommand.Parameters.Add("@rtDescription", SqlDbType.VarChar, 255).Value = rtItem.rtDescription;
            crudCommand.Parameters.Add("@rtLocation", SqlDbType.VarChar, 100).Value = rtItem.rtLocation;
            crudCommand.Parameters.Add("@rtDateTime", SqlDbType.DateTime).Value = rtItem.rtDateTime;
            crudCommand.Parameters.Add("@rtImagePath", SqlDbType.VarChar, 255).Value = rtItem.rtImagePath;
        }            

        try
        {
            await SQLCn.OpenAsync();
            rowsAffected = await crudCommand.ExecuteNonQueryAsync();
        }
        catch (Exception Ex)
        {
            string methodReturnValue = Ex.Message;
            rowsAffected = -1;
            // throw;
        }    

        return rowsAffected;

    }
    public async Task<int> DeleteItem(rtItem deleteItem)
    {
        int crudResult = 0;
        string sql = $"Delete from {tblName} where Id = @ItemId";           

        crudResult = await CRUDAsync(sql, deleteItem);

        return crudResult;
    }

    public async Task<int> UpdatertItembyId(rtItem rtItem)
    {
        int crudResult;
        string sql = $"Update t Set t.UserObjectId = '{rtItem.rtUserObjectId}', t.Description = {rtItem.rtDescription}, t.Location = {rtItem.rtLocation}, t.Dt = {rtItem.rtDateTime},  t.ImagePath = '{rtItem.rtImagePath}'"
         + $" From {tblName} t where t.id = {rtItem.rtId}";

        crudResult = await CRUDAsync(sql, rtItem);

        return crudResult;
    }

    
    public async Task<int> InsertrtItem(rtItem rtItem)    
    {
        int crudResult;

        string sql = $"Insert into {tblName} (UserObjectId, Description, Location, Dt, ImagePath) values (@rtUserObjectId, @rtDescription, @rtLocation, @rtDateTime, @rtImagePath)";

        crudResult = await CRUDAsync(sql, rtItem);

        return crudResult;

    }
    public List<rtItem> GetAllrtItemsJSON()
    {
        // local var rtItems
        string tblName = "rtItems";
        List<rtItem> _rtItems = new();

        using SqlConnection _pluralCn = new SqlConnection(GetSQLCnStr());

        
        _pluralCn.Open();

        string qry = $"Select * from {tblName} order by Id for json auto";
        var sqlCommand = new SqlCommand(qry, _pluralCn);

        //List<rtItem> sqlrtItems = sqlCommand.ExecuteReader
        object jsonObject = sqlCommand.ExecuteScalar();

        _rtItems = JsonSerializer.Deserialize<List<rtItem>>(jsonObject.ToString()!)!;
        return _rtItems;


        // to do  action results




    }

}  // end of class
// end of ns