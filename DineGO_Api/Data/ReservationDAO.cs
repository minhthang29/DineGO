using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using DineGO_Api.Data;
using Core.Models;
using DineGO_Api.Services;

public class ReservationDAO
{
    private readonly ApplicationDbContext _context;
    private readonly NotificationService _notificationService;

    public ReservationDAO(ApplicationDbContext context, NotificationService notificationService)
    {
        _context = context;
        _notificationService = notificationService;
    }

    // Get all Reservations
    public List<Reservation> GetReservations()
    {
        try
        {
            return _context.Reservations.ToList();
        }
        catch (Exception e)
        {
            throw new Exception($"Error fetching Reservations: {e.Message}");
        }
    }

    // Get reservation by ID
    public Reservation FindReservationById(int id)
    {
        try
        {
            return _context.Reservations
                            .Include(r => r.restaurant)
                            .Include(r => r.table)
                            .Include(r => r.customer)
                            .Include(r => r.payments)
                            .SingleOrDefault(x => x.reser_id == id);
        }
        catch (Exception e)
        {
            throw new Exception($"Error finding reservation: {e.Message}");
        }
    }

    // Save a new reservation
    public void SaveReservation(Reservation reservation)
    {
        try
        {
            _context.Reservations.Add(reservation);
            _context.SaveChanges();
        }
        catch (Exception e)
        {
            throw new Exception($"Error saving reservation: {e.Message}");
        }
    }

    // Update reservation details
    public void UpdateReservation(Reservation reservation)
    {
        try
        {
            _context.Entry(reservation).State = EntityState.Modified;
            _context.SaveChanges();

            string title = null;
            string content = null;

            // format thời gian theo định dạng Việt Nam
            string formattedDate = reservation.reser_date.ToString("HH:mm 'ngày' dd/MM/yyyy");

            if (reservation.reser_status == 1)
            {
                title = "Đặt bàn thành công ✅";
                content = $"Bạn đã đặt bàn thành công tại nhà hàng {reservation.restaurant.res_name} vào lúc {formattedDate}.";
            }
            else if (reservation.reser_status == 4)
            {
                title = "Không có mặt ❌";
                content = $"Bạn đã không có mặt tại nhà hàng {reservation.restaurant.res_name} vào lúc {formattedDate}.";
            }
            else if (reservation.reser_status == 2)
            {
                title = "Đặt bàn đã bị hủy ⚠️";
                content = $"Đơn đặt bàn tại {reservation.restaurant.res_name} đã bị hủy vào lúc {formattedDate} do chưa hoàn thành thanh toán.";
            }

            // chỉ thông báo nếu có cus_id
            if (reservation.cus_id.HasValue &&
                !string.IsNullOrEmpty(title) &&
                !string.IsNullOrEmpty(content))
            {
                _notificationService.NotifyCustomer(
                    reservation.cus_id.Value,
                    title,
                    content,
                    "reservation"
                );
            }
        }
        catch (Exception e)
        {
            throw new Exception($"Error updating reservation: {e.Message}", e);
        }
    }

    // Delete reservation by ID
    public void DeleteReservation(int id)
    {
        try
        {
            var reservation = _context.Reservations.SingleOrDefault(x => x.reser_id == id);
            if (reservation != null)
            {
                _context.Reservations.Remove(reservation);
                _context.SaveChanges();
            }
        }
        catch (Exception e)
        {
            throw new Exception($"Error deleting reservation: {e.Message}");
        }
    }

    public List<Reservation> GetResByCusId(int cus_id)
    {
        try
        {
            return _context.Reservations
                           .Include(r => r.restaurant).Include(r => r.table)
                           .Where(r => r.cus_id == cus_id)
                           .ToList<Reservation>();
        }
        catch (Exception e)
        {
            throw new Exception($"Error fetching Reservations: {e.Message}");
        }
    }

    public List<Reservation> GetResByResId(int res_id)
    {
        try
        {
            return _context.Reservations
                           .Include(r => r.restaurant).Include(r => r.table).Include(r => r.customer)
                           .Where(r => r.res_id == res_id)
                           .ToList<Reservation>();
        }
        catch (Exception e)
        {
            throw new Exception($"Error fetching Reservations: {e.Message}");
        }
    }
    public List<Reservation> GetReservationsByTable(int tableId)
    {
        return _context.Reservations
            .Include(r => r.customer)
            .Include(r => r.table)
            .Where(r => r.table_id == tableId
                     && !r.reser_is_deleted
                     && r.reser_status != 2)
            .OrderByDescending(r => r.reser_date) // sắp xếp mới nhất trước
            .ToList();
    }
}
