using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DineGO_Api.Data;
using Core.Models;

namespace DineGO_Api.Repository
{
    public class TableAreaRepository : ITableAreaRepository
    {
        private readonly TableAreaDAO _tableareaDAO;
        public TableAreaRepository(TableAreaDAO tableareaDAO)
        {
            _tableareaDAO = tableareaDAO;
        }
        public List<TableArea> GetTableAreas() => _tableareaDAO.GetTableAreas();
        public TableArea FindTableAreaById(int Id) => _tableareaDAO.FindTableAreaById(Id);
        public List<TableArea> FindTableAreaByResId(int resId) => _tableareaDAO.FindTableAreaByResId(resId);
        public void SaveTableArea(TableArea p) => _tableareaDAO.SaveTableArea(p);
        public void UpdateTableArea(TableArea p) => _tableareaDAO.UpdateTableArea(p);
        public void DeleteTableArea(int Id) => _tableareaDAO.DeleteTableArea(Id);
    }
}