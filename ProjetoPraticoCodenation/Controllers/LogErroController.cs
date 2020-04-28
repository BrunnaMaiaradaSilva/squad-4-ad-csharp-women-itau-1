﻿using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ProjetoPraticoCodenation.DTOs;
using ProjetoPraticoCodenation.Services;
using System;
using System.Collections.Generic;
using ProjetoPraticoCodenation.Models;
using System.Web.Http;
using Microsoft.AspNetCore.Authorization;

namespace ProjetoPraticoCodenation.Controllers
{
    [Produces("application/json")]
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [Authorize]

    public class LogErroController : ControllerBase
    {
        private readonly ILogErroService _logErroService;
        private readonly IMapper _mapper;

        public LogErroController(ILogErroService produtoService, IMapper mapper)
        {
            _logErroService = produtoService;
            _mapper = mapper;
        }

        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<LogErroDTO> Get(int id)
        {

            var logErro = _logErroService.FindById(id);

            if (logErro != null)
            {
                var retorno = _mapper.Map<LogErroDTO>(logErro);

                return Ok(retorno);
            }
            else
                return NotFound();
        }

        // GET api/LogErro/{nivel, ambiente, teste}
        [HttpGet("BuscarNivelAmbiente/{nivel, ambiente}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<IEnumerable<LogErroDTO>> GetNivelAmbiente(string nivel, string ambiente, bool ordenarPorNivel, bool ordenarPorFrequencia)
        {
            var listaLogErro = _logErroService.LocalizarPorNivelAmbiente(nivel, ambiente, ordenarPorNivel, ordenarPorFrequencia);


            if (listaLogErro != null)
            {
                var retorno = _mapper.Map<List<LogErroDTO>>(listaLogErro);

                return Ok(retorno);
            }
            else
                return NotFound();
        }

        // GET api/LogErro/{descricao, ambiente}
        [HttpGet("BuscarDescricaoAmbiente/{Descricao, ambiente}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<IEnumerable<LogErroDTO>> GetDescricaoAmbiente(string descricao, string ambiente)
        {
            var listaLogErro = _logErroService.LocalizarPorDescricaoAmbiente(descricao, ambiente);

            if (listaLogErro != null)
            {
                var retorno = _mapper.Map<List<LogErroDTO>>(listaLogErro);

                return Ok(retorno);
            }
            else
                return NotFound();

        }

        // GET api/LogErro/{origem, ambiente, teste}
        [HttpGet("BuscarOrigemAmbiente/{origem, ambiente}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<IEnumerable<LogErroDTO>> GetOrigemAmbiente(string origem, string ambiente)
        {
            var listaLogErro = _logErroService.LocalizarPorOrigemAmbiente(origem, ambiente);

            if (listaLogErro != null)
            {
                var retorno = _mapper.Map<List<LogErroDTO>>(listaLogErro);
                return Ok(retorno);
            }
            else
                return NotFound();
        }

        [HttpPost]
        public ActionResult<LogErroDTO> Post([FromBody]LogErroDTO value)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var logErro = new LogErro()
            {
                Titulo = value.Titulo,
                Descricao = value.Descricao,
                Nivel = value.Nivel,
                UsuarioOrigem = value.UsuarioOrigem,
                Evento = value.Evento,
                Origem = value.Origem,
                Arquivado = false,
                Ambiente = value.Ambiente,
                DataCriacao = value.DataCriacao
            };

            var retornoLogErro = _logErroService.Salvar(logErro);

            var retorno = _mapper.Map<LogErroDTO>(retornoLogErro);

            return Ok(retorno);
        }
  
        [HttpPut]
        public ActionResult<LogErroDTO> Put([FromBody] LogErroDTO value)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var logErro = new LogErro()
            {
                Titulo = value.Titulo,
                Descricao = value.Descricao,
                Nivel = value.Nivel,
                UsuarioOrigem = value.UsuarioOrigem,
                Evento = value.Evento,
                Origem = value.Origem,
                Arquivado = value.Arquivado,
                Ambiente = value.Ambiente,
                DataCriacao = value.DataCriacao
            };

            var retornoLogErro = _logErroService.Salvar(logErro);
            
            var retorno = _mapper.Map<LogErroDTO>(retornoLogErro);

            return Ok(retorno);
        }

        [HttpDelete]
        public ActionResult Delete(IList<int> listaIds)
        {
            if (listaIds.Count == 0)
                return BadRequest("Nenhum item para deletar");

            foreach (int id in listaIds)
            {
                _logErroService.Remover(id);
            }

            return Ok();
        }
    }
}
