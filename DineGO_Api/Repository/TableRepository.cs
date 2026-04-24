using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DineGO_Api.Data;
using Core.Models;

namespace DineGO_Api.Repository
{
    public class TableRepository : ITableRepository
    {
        private readonly TableDAO _tableDAO;
        public TableRepository(TableDAO tableDAO)
        {
            _tableDAO = tableDAO;
        }
        public List<Table> GetTables() => _tableDAO.GetTables();
        public Table FindTableById(int Id) => _tableDAO.FindTableById(Id);
        public List<Table> FindTableByResId(int resId) => _tableDAO.FindTableByResId(resId);
        public void SaveTable(Table p) => _tableDAO.SaveTable(p);
        public void UpdateTable(Table p) => _tableDAO.UpdateTable(p);
        public void DeleteTable(int Id) => _tableDAO.DeleteTable(Id);
    }
}