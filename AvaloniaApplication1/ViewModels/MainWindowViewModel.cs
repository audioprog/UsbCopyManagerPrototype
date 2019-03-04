using Avalonia.Controls;
using AvaloniaApplication1.Models;
using MySql.Data.MySqlClient;
using NReco.Data;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;

namespace AvaloniaApplication1.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        public MainWindowViewModel()
        {
            List<QField> fields = new List<QField>();
            foreach (string field in Settings.DivineService.Fields)
            {
                fields.Add(new QField(field));
            }

            ViewItemConverter.ServiceSettings = Settings.DivineService;

            try
            {
                DateTime date = new DateTime(DateTime.Now.Year, DateTime.Now.Month - 1, 1);
                DateTime nextMonth = date.AddMonths(1);
                Query query = new Query(
                    new QTable(Settings.DivineService.TableName, null), (QField)"Datum" >= (QConst)date & (QField)"Datum" < (QConst)nextMonth).Select(fields.ToArray());
                DivineServices = DbAdapter.Select(query).ToDataTable();
            }
            catch (Exception e)
            {
                Errors.Add(e.ToString());
            }

            DriveInfo[] drives = DriveInfo.GetDrives();
            foreach (DriveInfo drive in drives)
            {
                if (!drive.IsReady || drive.DriveType != DriveType.Removable)
                {
                    continue;
                }

                string name = drive.VolumeLabel;
                if (string.IsNullOrEmpty(name))
                {
                    name = drive.Name;
                }

                Drives.Add(name);
            }
        }

        public ViewMainHeader MainHeader { get; } = new ViewMainHeader();

        public Settings Settings { get; } = new Settings();


        private List<string> tables = DbAdapter.Select(new Query(new QTable("INFORMATION_SCHEMA.tables", null),
                                (QField)"TABLE_SCHEMA" != (QConst)"INFORMATION_SCHEMA" &
                                (QField)"TABLE_SCHEMA" != (QConst)"MYSQL" &
                                (QField)"TABLE_SCHEMA" != (QConst)"PERFORMANCE_SCHEMA"
                                ).Select("TABLE_NAME")).ToList<string>();

        public List<string> Tables
        {
            get { return tables; }
            set { this.RaiseAndSetIfChanged(ref tables, value); }
        }


        private static DbDataAdapter dbAdapter = null;
        public static DbDataAdapter DbAdapter
        {
            get
            {
                if (dbAdapter == null)
                {
                    string sqlDbPath = "Server=dietpi;User ID=audio;Password=1udio;Database=audio;";

                    DbFactory dbFactory = new DbFactory(MySqlClientFactory.Instance)
                    {
                        LastInsertIdSelectText = "SELECT last_insert_rowid()"
                    };
                    System.Data.IDbConnection dbConnection = dbFactory.CreateConnection();
                    dbConnection.ConnectionString = sqlDbPath;

                    DbCommandBuilder dbCmdBuilder = new DbCommandBuilder(dbFactory);
                    dbAdapter = new DbDataAdapter(dbConnection, dbCmdBuilder);
                }
                return dbAdapter;
            }
        }


        private DataTable divineServices = null;

        public DataTable DivineServices
        {
            get => divineServices;
            set => this.RaiseAndSetIfChanged(ref divineServices, value);
        }


        public ObservableCollection<string> Errors { get; } = new ObservableCollection<string>();

        public ObservableCollection<string> Drives { get; set; } = new ObservableCollection<string>();
    }
}
