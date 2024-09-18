using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ServerSide.DBinteractions;
using ServerSide.Models;
using System;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;

namespace ServerSide.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ParkingSpotController : ControllerBase
    {
        private readonly ParkingSpotsDB _parkingSpotsDB;

        public ParkingSpotController(IConfiguration configuration)
        {
            
            _parkingSpotsDB = new ParkingSpotsDB(configuration.GetConnectionString("DefaultConnection"));
        }

        // GET: api/ParkingSpot
        [HttpGet]
        public IActionResult GetAllParkingSpots()
        {
            try
            {
                List<ParkingSpot> spots = _parkingSpotsDB.GetAllParkingSpots();
                return Ok(spots);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        // POST: api/ParkingSpot
        [HttpPost]
        public IActionResult AddParkingSpot([FromBody] ParkingSpot spot)
        {
            try
            {
                _parkingSpotsDB.AddParkingSpot(spot);
                return StatusCode(201, "Parking Spot added successfully.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        // GET: api/ParkingSpot/{lotId}
        [HttpGet("lot/{lotId}")]
        public IActionResult GetParkingSpotsByLotId(string lotId)
        {
            try
            {
                List<ParkingSpot> spots = _parkingSpotsDB.GetParkingSpotsByLotId(lotId);
                return Ok(spots);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        // PUT: api/ParkingSpot/{id}
        [HttpPut("{id}")]
        public IActionResult UpdateParkingSpot(string id, [FromBody] ParkingSpot updatedSpot)
        {
            try
            {
                bool isUpdated = _parkingSpotsDB.UpdateParkingSpot(id, updatedSpot);
                if (!isUpdated)
                    return NotFound("Parking spot not found or update failed.");

                return Ok("Parking spot updated successfully.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        // DELETE: api/ParkingSpot/{id}
        [HttpDelete("{id}")]
        public IActionResult RemoveParkingSpot(string id)
        {
            try
            {
                bool isDeleted = _parkingSpotsDB.RemoveParkingSpot(id);
                if (!isDeleted)
                    return NotFound("Parking spot not found.");

                return Ok("Parking spot removed successfully.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}
