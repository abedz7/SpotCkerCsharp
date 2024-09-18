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
    public class ParkingLotController : ControllerBase
    {
        private readonly ParkingLotsDB _parkingLotsDB;

        public ParkingLotController(IConfiguration configuration)
        {
            
            _parkingLotsDB = new ParkingLotsDB(configuration.GetConnectionString("DefaultConnection"));
        }

        // GET: api/ParkingLot
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public ActionResult<ParkingLot[]> Get()
        {
            try
            {
                List<ParkingLot> parkingLots = _parkingLotsDB.GetAllParkingLots();
                return new JsonResult(parkingLots);
            }
            catch (Exception e)
            {
                return StatusCode(StatusCodes.Status400BadRequest, e.Message);
            }
        }

        // GET: api/ParkingLot/{id}
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ParkingLot))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult Get(string id)
        {
            try
            {
                ParkingLot parkingLot = _parkingLotsDB.GetParkingLotById(id);
                if (parkingLot == null)
                    return NotFound($"Parking lot with id: {id} wasn't found.");

                return Ok(parkingLot);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        // POST: api/ParkingLot
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(ParkingLot))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult Post([FromBody] ParkingLot value)
        {
            try
            {
                if (value == null)
                    return BadRequest("Parking lot is null.");

                value.Id = Guid.NewGuid().ToString();  
                string newId = _parkingLotsDB.AddParkingLot(value);

                return CreatedAtAction(nameof(Get), new { id = newId }, value);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        // PUT: api/ParkingLot/{id}
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult Put(string id, [FromBody] ParkingLot value)
        {
            try
            {
                if (value == null || value.Id != id)
                    return BadRequest("Invalid parking lot data.");

                int rowsAffected = _parkingLotsDB.UpdateParkingLot(value);
                if (rowsAffected == 0)
                    return NotFound($"Parking lot with id: {id} wasn't found, can't update.");

                return NoContent();
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        // DELETE: api/ParkingLot/{id}
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public IActionResult Delete(string id)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(id))
                    return BadRequest("Invalid parking lot ID.");

                bool isDeleted = _parkingLotsDB.DeleteParkingLot(id);
                if (!isDeleted)
                    return NotFound($"Parking lot with id: {id} wasn't found, can't delete.");

                return NoContent();
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        // GET: api/ParkingLot/name/{name}
        [HttpGet("name/{name}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ParkingLot))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult GetByName(string name)
        {
            try
            {
                ParkingLot parkingLot = _parkingLotsDB.GetParkingLotByName(name);
                if (parkingLot == null)
                    return NotFound($"Parking lot with name: {name} wasn't found.");

                return Ok(parkingLot);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
    }
}
