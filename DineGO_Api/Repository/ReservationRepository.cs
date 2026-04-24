using System;
using System.Collections.Generic;
using System.Linq;
using DineGO_Api.Data;
using Core.Models;

namespace DineGO_Api.Repository
{
    public class ReservationRepository : IReservationRepository
    {
        private readonly ReservationDAO _reservationDAO;
        public ReservationRepository(ReservationDAO reservationDAO)
        {
            _reservationDAO = reservationDAO;
        }
        public List<Reservation> GetReservations() => _reservationDAO.GetReservations();
        public Reservation FindReservationById(int id) => _reservationDAO.FindReservationById(id);
        public void SaveReservation(Reservation r) => _reservationDAO.SaveReservation(r);
        public void UpdateReservation(Reservation r) => _reservationDAO.UpdateReservation(r);
        public void DeleteReservation(int id) => _reservationDAO.DeleteReservation(id);

        // public List<Reservation> GetResByCusId(int id)
        // {
        //     return GetReservations().Where(p => p.cus_id == id).ToList();
        // }

        public List<Reservation> GetResByResId(int id) => _reservationDAO.GetResByResId(id);
        public List<Reservation> GetResByCusId(int id) => _reservationDAO.GetResByCusId(id);

        public List<Reservation> GetReservationsByTable(int tableId) => _reservationDAO.GetReservationsByTable(tableId);
    }
}