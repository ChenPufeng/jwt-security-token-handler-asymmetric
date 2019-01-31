using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using jwt_security_token_handler_asymmetric.Abstraction;
using jwt_security_token_handler_asymmetric.Models;
using Microsoft.AspNetCore.Mvc;

namespace jwt_security_token_handler_asymmetric.Controllers
{
  [Route("api/[controller]")]
  [ApiController]
  public class ClientsController : ControllerBase
  {
    private readonly IRepository<Client, Guid> _repository;

    public ClientsController(IRepository<Client, Guid> repository)
    {
      _repository = repository;
    }

    [HttpGet]
    public async Task<ActionResult> Get()
    {
      var clients = await _repository.GetAllAsync(null);

      if (clients == null) return NotFound();

      return Ok(clients);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult> Get(Guid id)
    {
      var client = await _repository.GetAsync(c => c.Id == id);

      if (client == null) return NotFound();

      return Ok(client);
    }

    [HttpPost]
    public async Task<ActionResult> Post([FromBody] Client client)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState.Values.SelectMany(v => v.Errors));

        await _repository.AddAsync(client);
        return Created($"api/clients/{client.Id}", client);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult> Put(Guid id, [FromBody] Client client)
    {
        var clientOld = await _repository.GetAsync(c => c.Id == id);

        if (client == null) return NotFound();

        client.Id = client.Id;

        if (!ModelState.IsValid)
            return BadRequest(ModelState.Values.SelectMany(v => v.Errors));

        await _repository.UpdateAsync(client);
        return Ok(client);
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(Guid id)
    {
        var client = await _repository.GetAsync(c => c.Id == id);

        if (client == null) return NotFound();

        await _repository.DeleteAsync(client);
        return Ok();
    }
  }
}
