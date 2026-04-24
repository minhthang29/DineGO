using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Models;

namespace DineGO_Api.Repository
{
    public interface ITableRepository
    {
        List<Table> GetTables();

        Table FindTableById(int ID);
        List<Table> FindTableByResId(int resId);

        void SaveTable(Table p);

        void UpdateTable(Table p);

        void DeleteTable(int p);
    }
}