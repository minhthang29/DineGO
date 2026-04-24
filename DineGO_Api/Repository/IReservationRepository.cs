using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Models;

public interface IReservationRepository
{
    List<Reservation> GetReservations();

    Reservation FindReservationById(int ID);
    void SaveReservation(Reservation reservation);

    void UpdateReservation(Reservation reservation);

    void DeleteReservation(int reservationId);

    List<Reservation> GetResByResId(int id);
    List<Reservation> GetResByCusId(int id); 
    List<Reservation> GetReservationsByTable(int tableId);
}

