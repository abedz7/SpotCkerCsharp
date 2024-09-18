using Microsoft.AspNetCore.Mvc;
using ServerSide.DBinteractions;
using ServerSide.Models;
using System;
using System.Collections.Generic;

namespace ServerSide.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ParkingController : ControllerBase
    {
        private readonly ParkingDB _parkingDB;

        public ParkingController(IConfiguration configuration)
        {
            _parkingDB = new ParkingDB(configuration); 
        }

        // POST: api/Parking
        [HttpPost]
        public IActionResult AddParking([FromBody] Parking parking)
        {
            try
            {
                _parkingDB.AddParking(parking);
                return StatusCode(201, "Parking record added successfully.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        // GET: api/Parking/{id}
        [HttpGet("{id}")]
        public IActionResult GetParkingById(string id)
        {
            try
            {
                Parking parking = _parkingDB.GetParkingById(id);
                if (parking == null)
                    return NotFound("Parking record not found.");

                return Ok(parking);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        // PUT: api/Parking/{id}
        [HttpPut("{id}")]
        public IActionResult UpdateParking(string id, [FromBody] Parking updatedParking)
        {
            try
            {
                updatedParking.Id = id;
                int rowsAffected = _parkingDB.UpdateParking(updatedParking);
                if (rowsAffected == 0)
                    return NotFound("Parking record not found.");

                return Ok("Parking record updated successfully.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        // DELETE: api/Parking/{id}
        [HttpDelete("{id}")]
        public IActionResult DeleteParking(string id)
        {
            try
            {
                bool isDeleted = _parkingDB.DeleteParking(id);
                if (!isDeleted)
                    return NotFound("Parking record not found.");

                return Ok("Parking record deleted successfully.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        // GET: api/Parking/user/{userId}
        [HttpGet("user/{userId}")]
        public IActionResult GetParkingsByUserId(string userId)
        {
            try
            {
                List<Parking> parkings = _parkingDB.GetParkingsByUserId(userId);
                return Ok(parkings);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}
