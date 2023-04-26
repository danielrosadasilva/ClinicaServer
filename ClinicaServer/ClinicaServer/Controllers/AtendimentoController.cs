using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ClinicaServer.Models;
using ClinicaServer.Repositories;
using System.Linq.Expressions;
using ClinicaServer.Dto;
using System.Data.Common;

namespace ClinicaServer.Controllers
{
    [Consumes("application/json")]
    [Produces("application/json")]
    [Route("api/[controller]/")]
    [ApiController]
    public class AtendimentoController : ControllerBase
    {
        private IRepositoryWrapper _repository;
        public AtendimentoController(IRepositoryWrapper repository)
        {
            _repository = repository;
        }

        [HttpGet]
        public List<Atendimento> GetAtendimentos()
        {
            //return _repository.Atendimento.FindAll().OrderByDescending(x => x.Datahoraatendimento).ToList<Atendimento>();
            List<Atendimento> lista= _repository.Atendimento.FindByConditionAndInclude(x=>x.Id==x.Id, "Examecovid").OrderByDescending(x => x.Datahoraatendimento).ToList<Atendimento>();
            //Console.WriteLine("id paciente="+lista.First().Paciente.Id);
            return lista;
        }

        [HttpGet("{id}")]
        public List<Atendimento> Get(uint id)
        {
            return _repository.Atendimento.FindByCondition(x => x.Id == id).ToList<Atendimento>();
        }

        [HttpPost]
        public bool CreateOrUpdate([Bind("Id,Pacienteid,Datahoraatendimento,Concluido")] Atendimento atendimento)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    if (atendimento.Id > 0)
                    {
                        return _repository.Atendimento.update(atendimento) > 0;

                    }
                    else
                    {
                        return _repository.Atendimento.create(atendimento) > 0;
                        
                    }
                }
            }
            catch (Exception ex)
            {

            }
            Response.StatusCode = 400;
            return false;
        }


        [HttpDelete]
        public bool Delete(uint id)
        {
            if (_repository.Atendimento == null)
            {
                Response.StatusCode = 400;
                return false;
            }
            var atendimento = _repository.Atendimento.FindByCondition(x=>x.Id==id).First<Atendimento>();
            if (atendimento != null)
            {
                _repository.Atendimento.delete(atendimento);
                int linhasafetadas = _repository.save();
                if (linhasafetadas > 0)
                {
                    return true;
                }
            }
            Response.StatusCode = 400;
            return false;

        }


        [HttpGet("relatorio")]
        public List<Relatorio> GetReportData()
        {
            try
            {
                List<Relatorio> listarelatorio = new List<Relatorio>();
                _repository.getContext().Database.OpenConnection();
                DbConnection connect = _repository.getContext().Database.GetDbConnection();

                var command = connect.CreateCommand();
                command.CommandText = "select YEAR(a.datahoraatendimento) as ano,MONTH(a.datahoraatendimento) as mes, count(a.id) as totalatendimentos, count(case when ec.febre+ec.coriza+ec.narizentupido+ec.cansaco+ec.tosse+ec.dordecabeca+ec.doresnocorpo+ec.malestargeral+ec.dordegarganta+ec.dificuldadederespirar+ec.faltadepaladar+ec.faltadeolfato+ec.dificuldadedelocomocao+ec.diarreia*100/14>5 then 1 end)-count(case when ec.febre+ec.coriza+ec.narizentupido+ec.cansaco+ec.tosse+ec.dordecabeca+ec.doresnocorpo+ec.malestargeral+ec.dordegarganta+ec.dificuldadederespirar+ec.faltadepaladar+ec.faltadeolfato+ec.dificuldadedelocomocao+ec.diarreia*100/14>8 then 1 end) as totalpossivelmenteinfectados, count(case when ec.febre+ec.coriza+ec.narizentupido+ec.cansaco+ec.tosse+ec.dordecabeca+ec.doresnocorpo+ec.malestargeral+ec.dordegarganta+ec.dificuldadederespirar+ec.faltadepaladar+ec.faltadeolfato+ec.dificuldadedelocomocao+ec.diarreia*100/14>8 then 1 end) as totalpotencialmenteinfectados from atendimento a, examecovid ec where ec.atendimentoid=a.id group by ano,mes order by ano desc, mes desc limit 0,6";
                DbDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    Relatorio relatorio = new Relatorio();
                    relatorio.ano = reader.GetInt32(0);
                    relatorio.mes = reader.GetInt32(1);
                    relatorio.totalatendimentos = reader.GetInt64(2);
                    relatorio.totalpossivelmenteinfectados = reader.GetInt64(3);
                    relatorio.totalpotencialmenteinfectados = reader.GetInt64(4);
                    listarelatorio.Add(relatorio);

                }
                _repository.getContext().Database.CloseConnection();


                return listarelatorio;


            }
            catch (Exception e)
            {
                return null;
            }


            //return _repository.Paciente.FindAll().ToList<object>();
        }


    }
}
