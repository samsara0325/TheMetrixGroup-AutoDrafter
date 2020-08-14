using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MetrixGroupPlugins.Utilities
{
   class test
   {
      public void help() {
      SqlConnection DbConn = new SqlConnection(YourConnectionString);
      SqlCommand ExecJob = new SqlCommand();
      ExecJob.CommandType = CommandType.StoredProcedure;
ExecJob.CommandText = "msdb.dbo.sp_start_job";
         ExecJob.Parameters.AddWithValue("@job_name", "YourJobName");
ExecJob.Connection = DbConn; //assign the connection to the command.
using (DbConn)
   {
    DbConn.Open();
    using (ExecJob)
    {
        ExecJob.ExecuteNonQuery();
    }
}
   }

}
