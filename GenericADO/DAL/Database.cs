using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;

namespace GenericADO.DAL
{

    public abstract class Database
    {
        protected DbConnection _connection = null;
        protected DbCommand _command = null;

        public abstract void SetDbConnection();
        public abstract void SetBasicDbCommand();

        private void SetParameters(DbCommand commands)
        {
            
            _command.CommandText = commands.CommandText;

            foreach (DbParameter command in commands.Parameters)
            {
                DbParameter param = _command.CreateParameter();

                param.DbType = command.DbType;
                param.Size = command.Value != null ? command.Value.ToString().Length : 0;
                param.Precision = command.Precision;
                param.Scale = command.Scale;

                if (command.Value == null)
                    param.Direction = ParameterDirection.Output;
                else
                {
                    param.Value = command.Value;  
                }
                param.ParameterName = command.ParameterName;

                _command.Parameters.Add(param);
            }
        }

        public List<object> OExecProc(DbCommand commands)
        {

            List<object> objs = new List<object>();
            try
            {
                if (commands != null)
                {
                    SetParameters(commands);

                    OpenConnection();
                    _command.ExecuteNonQuery();

                    foreach (DbParameter parameter in _command.Parameters)
                    {
                        if (parameter.Direction == ParameterDirection.Output)
                        {
                            objs.Add(parameter.Value);
                        }
                    }

                    CloseConnection();
                }
            }
            catch (Exception ex)
            {
                CloseConnection();
                //Tratar melhor essa mensagem
                throw new Exception(ex.Message);

            }

            return objs;
        }

        public void IExecNonQueryProc(DbCommand commands)
        {
            List<object> objs = new List<object>();
            try
            {
                if (commands != null)
                {
                    SetParameters(commands);

                    OpenConnection();
                    _command.ExecuteNonQuery();
                    CloseConnection();
                }
            }
            catch (Exception ex)
            {
                CloseConnection();
                throw ex;
            }
        }

        public List<Dictionary<string, object>> IOExecProc(DbCommand commands)
        {
            List<Dictionary<string, object>> lDict = new List<Dictionary<string, object>>();

            try
            {
                using (_connection)
                {
                    OpenConnection();
                    using (commands)
                    {
                        SetParameters(commands);
                        DbDataReader reader = _command.ExecuteReader();

                        while (reader.Read())
                        {
                            Dictionary<string, object> line = new Dictionary<string, object>();

                            for (int x = 0; x < reader.FieldCount; x++)
                            {
                                if (!reader.IsDBNull(x))
                                    line.Add(reader.GetName(x), reader.GetValue(x).ToString());
                                else
                                    line.Add(reader.GetName(x), 0);
                            }
                            lDict.Add(line);
                        }
                        reader.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                CloseConnection();
                throw ex;
            }

            return lDict;
        }

        private void OpenConnection()
        {
            // _connection.ConnectionString = connectionString;
            _connection.Open();
        }

        private void CloseConnection()
        {
            _connection.Close();
            _connection.Dispose();
            _command.Dispose();
        }
    }


}
