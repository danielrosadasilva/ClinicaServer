using ClinicaServer.Models;
using ClinicaServer.Repositories;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ClinicaServer.Controllers
{
    [Produces("application/json")]
    [Consumes("application/json")]
    [Route("api/[controller]/")]
    [ApiController]
    public class PacienteController : ControllerBase
    {
        private IRepositoryWrapper _repository;
        public PacienteController(IRepositoryWrapper repository)
        {
            _repository=repository;
        }

        [HttpGet]
        public List<Paciente> GetAll()
        {
            return _repository.Paciente.FindAll().ToList();
        }

        [HttpGet("{id}")]
        public List<Paciente> GetById(uint id)
        {
            return _repository.Paciente.FindByCondition(x=>x.Id==id).ToList<Paciente>();
        }

        [HttpGet("cpf={cpf}")]
        public List<Paciente> GetByCpf(ulong cpf)
        {
            return _repository.Paciente.FindByCondition(x => x.Cpf == cpf).ToList<Paciente>();
        }



        [HttpGet("{pacienteid}/Atendimento/")]
        public List<Atendimento> GetAtendimentosByPaciente(uint pacienteid)
        {
            return _repository.Atendimento.FindByCondition(x => x.Pacienteid == pacienteid).OrderByDescending(x=>x.Datahoraatendimento).ToList<Atendimento>();
        }

        [HttpGet("{pacienteid}/Atendimento/{atendimentoid}")]
        public List<Atendimento> GetAtendimentoByPaciente(uint pacienteid, uint atendimentoid)
        {
            return _repository.Atendimento.FindByCondition(x => x.Pacienteid == pacienteid && x.Id==atendimentoid).ToList<Atendimento>();
        }

        [HttpGet("{pacienteid}/Atendimento/{atendimentoid}/Examegeral/")]
        public List<Examegeral> GetExamegeralByAtendimentoAndPaciente(uint pacienteid, uint atendimentoid)
        {
            return _repository.Examegeral.FindByCondition(x => x.Atendimentoid ==atendimentoid && x.Atendimento.Pacienteid == pacienteid).ToList<Examegeral>();
        }

        [HttpGet("{pacienteid}/Atendimento/{atendimentoid}/Examecovid/")]
        public List<Examecovid> GetExamecovidByAtendimentoAndPaciente(uint pacienteid, uint atendimentoid)
        {
            return _repository.Examecovid.FindByCondition(x => x.Atendimentoid == atendimentoid && x.Atendimento.Pacienteid == pacienteid).ToList<Examecovid>();
        }

        [HttpGet("{pacienteid}/Atendimento/{atendimentoid}/Examegeral/{examegeralid}")]
        public List<Examegeral> GetExamegeralByAtendimentoAndPacienteAndExamegeral(uint pacienteid, uint atendimentoid, uint examegeralid)
        {
            return _repository.Examegeral.FindByCondition(x => x.Atendimentoid == atendimentoid && x.Atendimento.Pacienteid == pacienteid && x.Id == examegeralid).ToList<Examegeral>();
        }

        [HttpGet("{pacienteid}/Atendimento/{atendimentoid}/Examecovid/{examecovidid}")]
        public List<Examecovid> GetExamecovidByAtendimentoAndPaciente(uint pacienteid, uint atendimentoid, uint examecovidid)
        {
            return _repository.Examecovid.FindByCondition(x => x.Atendimentoid == atendimentoid && x.Atendimento.Pacienteid == pacienteid && x.Id == examecovidid).ToList<Examecovid>();
        }


        [HttpPost]
        public bool CreateOrUpdate([FromBody] Paciente paciente)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    if (paciente.Id > 0)
                    {
                        return _repository.Paciente.update(paciente)>0;

                    }
                    else
                    {
                        return _repository.Paciente.create(paciente)>0;
                    }
                }
            }
            catch (Exception ex)
            {

            }
            Response.StatusCode = 400;
            return false;
        }


        [HttpPost("{pacienteid}/Atendimento/")]
        //public bool CreateAtendimentoByFuncionario([Bind("datahoraatendimento,concluido")] Atendimento atendimento, uint pacienteid)
        public bool CreateAtendimentoByFuncionario(uint pacienteid)
        {
            
            
            try{
                Atendimento atendimento = new Atendimento();
                atendimento.Concluido = false;
                atendimento.Datahoraatendimento = DateTime.Now;
                atendimento.Pacienteid = pacienteid;

                Examegeral examegeral = new Examegeral();
                examegeral.Pressaosistolica = 0;
                examegeral.Pressaodiastolica = 0;
                examegeral.Pulsacao = 0;
                examegeral.Respiracao = 0;
                examegeral.Temperatura = 0;
                examegeral.Concluido = false;
                ICollection<Examegeral> examegerals = new HashSet<Examegeral>();
                examegerals.Add(examegeral);
                atendimento.Examegeral = examegerals;

                Examecovid examecovid = new Examecovid();
                examecovid.Cansaco = false;
                examecovid.Coriza = false;
                examecovid.Diarreia = false;
                examecovid.Dificuldadederespirar = false;
                examecovid.Dificuldadedelocomocao = false;
                examecovid.Dordecabeca = false;
                examecovid.Dordegarganta = false;
                examecovid.Doresnocorpo = false;
                examecovid.Faltadeolfato = false;
                examecovid.Faltadepaladar = false;
                examecovid.Febre = false;
                examecovid.Malestargeral = false;
                examecovid.Narizentupido = false;
                examecovid.Tosse = false;
                examecovid.Concluido = false;
                ICollection<Examecovid> examescovid = new HashSet<Examecovid>();
                examescovid.Add(examecovid);
                atendimento.Examecovid = examescovid;



                return _repository.Atendimento.create(atendimento)>0;
            }   
            catch(Exception e)
            {

            }
            Response.StatusCode = 400;
            return false;
        }


        [HttpPost("{pacienteid}/Atendimento/{atendimentoid}/Finalizar")]
        public bool UpdateAtendimentoByFuncionarioAndAtendimento(uint pacienteid, uint atendimentoid)
        {
            
            try
            {
                Atendimento atendimento = _repository.Atendimento.FindByCondition(x => x.Id == atendimentoid && x.Pacienteid == pacienteid && x.Concluido == false).ToList<Atendimento>().ElementAt<Atendimento>(0);
                atendimento.Concluido = true;
                return _repository.Atendimento.update(atendimento) > 0;
            }
            catch (Exception e)
            {

            }
            Response.StatusCode = 400;
            return false;
        }



        [HttpPost("{pacienteid}/Atendimento/{atendimentoid}/Examegeral/")]
        public bool CreateExamegeralByFuncionarioAndAtendimento([Bind("Pressaosistolica,Pressaodiastolica,Pulsacao,Respiracao,Temperatura")] Examegeral examegeral, uint pacienteid, uint atendimentoid)
        {
            examegeral.Concluido = true;
            examegeral.Atendimentoid=atendimentoid;
            try
            {
                return _repository.Examegeral.create(examegeral) > 0;
            }
            catch (Exception e)
            {

            }
            Response.StatusCode = 400;
            return false;
        }




        [HttpPost("{pacienteid}/Atendimento/{atendimentoid}/Examecovid/")]
        public bool CreateExamecovidByFuncionarioAndAtendimento([FromBody] Examecovid examecovid, uint pacienteid, uint atendimentoid)
        {
            examecovid.Concluido = true;
            examecovid.Atendimentoid = atendimentoid;
            try
            {
                return _repository.Examecovid.create(examecovid) > 0;
            }
            catch (Exception e)
            {

            }
            Response.StatusCode = 400;
            return false;
        }


        [HttpPost("{pacienteid}/Atendimento/{atendimentoid}/Examegeral/{examegeralid}")]
        public bool UpdateExamegeralByFuncionarioAndAtendimentoAndExamegeral([Bind("Pressaosistolica,Pressaodiastolica,Pulsacao,Respiracao,Temperatura")] Examegeral examegeral, uint pacienteid, uint atendimentoid, uint examegeralid)
        {


            try
            {
                //Examegeral examegeraldb = _repository.Examegeral.FindByCondition(x => x.Id == examegeralid && x.Atendimentoid == atendimentoid && x.Atendimento.Pacienteid == pacienteid && x.Concluido == false).ToList<Examegeral>().ElementAt<Examegeral>(0);
                examegeral.Id = examegeralid;
                examegeral.Atendimentoid =atendimentoid;
                examegeral.Concluido = true;

                //adicionar condicionais de sintomas para possivelmente gerar o formulário vazio para a pessoa realizar o examecovid
                //if sintomas > 40%, gerar formulario abaixo para salvar no banco de dados
                
                Boolean result= _repository.Examegeral.update(examegeral) > 0;
                Console.WriteLine(examegeral.Id+"-"+examegeral.Atendimentoid+"-"+examegeral.Concluido+"-"+examegeral.Pressaodiastolica + "-" + examegeral.Pressaosistolica + "-" + examegeral.Respiracao + "-" + examegeral.Pulsacao + "-" + examegeral.Temperatura);
                return result;
            }
            catch (Exception e)
            {

            }
            Response.StatusCode = 400;
            return false;
        }




        [HttpPost("{pacienteid}/Atendimento/{atendimentoid}/Examecovid/{examecovidid}")]
        public bool UpdateExamecovidByFuncionarioAndAtendimento([FromBody] Examecovid examecovid, uint pacienteid, uint atendimentoid, uint examecovidid)
        {

            try
            {
                //Examecovid examecoviddb = _repository.Examecovid.FindByCondition(x => x.Id == examecovidid && x.Atendimentoid == atendimentoid && x.Atendimento.Pacienteid == pacienteid && x.Concluido == false).ToList<Examecovid>().ElementAt<Examecovid>(0);
                examecovid.Id = examecovidid;
                examecovid.Atendimentoid = atendimentoid;
                examecovid.Concluido = true;
                return _repository.Examecovid.update(examecovid) > 0;
            }
            catch (Exception e)
            {

            }
            Response.StatusCode = 400;
            return false;
        }




    }
}
