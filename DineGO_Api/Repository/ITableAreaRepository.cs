using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Models;

namespace DineGO_Api.Repository
{
    public interface ITableAreaRepository
    {
        List<TableArea> GetTableAreas();

        TableArea FindTableAreaById(int ID);
        List<TableArea> FindTableAreaByResId(int resId);

        void SaveTableArea(TableArea p);

        void UpdateTableArea(TableArea p);

        void DeleteTableArea(int p);
    }
}