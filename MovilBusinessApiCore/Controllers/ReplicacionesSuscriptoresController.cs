using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using MovilBusinessApiCore.Models;
using MovilBusinessApiCore.Models.Internal.Structs;
using MovilBusinessApiCore.Utils;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace MovilBusinessApiCore.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class ReplicacionesSuscriptoresController : ControllerBase
    {
        private readonly MBContext _context;

        public ReplicacionesSuscriptoresController(MBContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return Ok();
        }


        [HttpPost]
        [Route("api/ReplicacionesSuscriptores/VerificarSuscriptor")]
        public async Task<IActionResult> VerificarSuscriptor([FromBody]UsuarioArgs args)
        {
            var writeLog = false;

            try
            {

                if (string.IsNullOrEmpty(args.RepCodigo))
                {
                    return BadRequest("El codigo de usuario no puede estar vacio!");
                }

                if (string.IsNullOrEmpty(args.RepClave))
                {
                    return BadRequest("La contraseña de usuario no puede estar vacia!");
                }

                if (string.IsNullOrEmpty(args.Suscriptor))
                {
                    return BadRequest("El key del suscriptor no puede estar vacio!");
                }

                using (MBContext db = new MBContext())
                {
                    writeLog = db.Parametros.Where(x => x.ParReferencia.Trim().ToUpper() == "WRITELOG" && x.ParValor.Trim() == "1").FirstOrDefault() != null;

                    if (!UsuarioSistema.Exists(args.RepCodigo, args.RepClave, db))
                    {
                        return BadRequest("Usuario o contraseña invalidos");
                    }

                    var suscriptor = db.ReplicacionesSuscriptores.Where(x => x.UsuInicioSesion == args.RepCodigo && x.RepSuscriptor.ToString().Substring(0, 8) == args.Suscriptor).FirstOrDefault();

                    if (suscriptor == null)
                    {
                        return BadRequest("Key invalido!");
                    }

                    if (suscriptor.resEstado != 1)
                    {
                        return BadRequest("No disponible para carga inicial");
                    }

                    if (suscriptor.RsuTipo == 1)
                    {
                        VerificarLicencia(db);
                    }

                    SqlConnection conn = (SqlConnection)(db.Database.GetDbConnection());
                    SqlCommand cmd = new SqlCommand("[sp_MDSOFTGeneraCargaInicialSuscriptoresCambios] @RepID, @RepSuscriptor", conn)
                    {
                        CommandTimeout = 300
                    };

                    if (conn.State != ConnectionState.Open)
                        conn.Open();

                    cmd.Parameters.AddRange(
                         new SqlParameter[] {
                         new SqlParameter() { ParameterName = "@RepID", SqlDbType = System.Data.SqlDbType.Int, Value = suscriptor.RepID },
                         new SqlParameter() { ParameterName = "@RepSuscriptor", SqlDbType = System.Data.SqlDbType.VarChar, Size = int.MaxValue, Value = suscriptor.RepSuscriptor.ToString()}}
                         );
                    suscriptor.resEstado = 3;

                    await cmd.ExecuteNonQueryAsync();

                    var query = string.Format("SELECT COUNT(RepID) AS Value FROM [ReplicacionesSuscriptoresCambios{0}] with(nolock)", suscriptor.RepSuscriptor.ToString());

                    var currentTransaction = db.Database.CurrentTransaction;
                    cmd.Transaction = currentTransaction != null ? (SqlTransaction)currentTransaction.GetDbTransaction() : cmd.Transaction;

                    SqlDataAdapter adapt = new SqlDataAdapter(cmd);
                    DataTable result = new DataTable();

                    adapt.Fill(result);

                    if (currentTransaction == null)
                        conn.Close();

                    //var rr = (ReplicacionesSuscriptoresCambiosCount)result1.Rows.AsQueryable();

                    int cambios = (int)result.Rows[0].ItemArray.GetValue(0);

                    return Ok(new VerificarSuscriptorResult() { RepSuscriptor = suscriptor.RepSuscriptor.ToString(), CantidadCambios = cambios });
                }           
            }
            catch (Exception e)
            {
                string msgError = e.Message;

                if (writeLog)
                {
                    if (e.InnerException != null)
                    {
                        msgError += Environment.NewLine + "InnerException: " + e.InnerException.Message;
                    }

                    Functions.WriteLog(args.RepCodigo, $"VerificarSuscriptor() - Error validando suscriptor en MovilBusiness. Usuario: {args.RepCodigo}, Key del suscriptor: {args.Suscriptor}, Mensaje: {msgError}");
                }
                return BadRequest(e.Message);
            }
        }

        private void VerificarLicencia(MBContext _context)
        {
            var par = _context.Parametros.Where(x => x.ParReferencia.Trim().ToUpper() == "LICENCIA").FirstOrDefault();

            if (par == null)
            {
                throw new Exception("No dispone de licencias disponibles para realizar la carga inicial");
            }

           string parvalor = par.ParValor;

            if (parvalor.Length != 36 || !parvalor.Contains("-"))
            {
                throw new Exception("Licencia mal configurada, comuniquese con MDSOFT");
            }

            var value = parvalor.Substring(4, 1) + parvalor.Substring(10, 1) + parvalor.Substring(20, 1) + parvalor.Substring(31, 1);

            int.TryParse(value, out int licencias);

            var cantidadSuscriptores = _context.ReplicacionesSuscriptores.Count(x => x.RsuTipo == 1);

            if (licencias < cantidadSuscriptores)
            {
                throw new Exception("No dispone de licencias disponibles para realizar la carga inicial");
            }
        }

        [HttpPost]
        [Route("api/ReplicacionesSuscriptores/VerificarSuscriptorLegacy")]
        public async Task<IActionResult> VerificarSuscriptorLegacy([FromBody]UsuarioArgs args)
        {
            var writeLog = false;

            try
            {
                if (string.IsNullOrEmpty(args.RepCodigo))
                {
                    return BadRequest("El codigo de usuario no puede estar vacio!");
                }

                if (string.IsNullOrEmpty(args.RepClave))
                {
                    return BadRequest("La contraseña de usuario no puede estar vacia!");
                }

                if (string.IsNullOrEmpty(args.Suscriptor))
                {
                    return BadRequest("El key del suscriptor no puede estar vacio!");
                }

                //using (MBContext db = new MBContext())
                //{
                    writeLog = _context.Parametros.Where(x => x.ParReferencia.Trim().ToUpper() == "WRITELOG" && x.ParValor.Trim() == "1").FirstOrDefault() != null;

                    if (!UsuarioSistema.Exists(args.RepCodigo, args.RepClave, _context))
                    {
                        return BadRequest("Usuario o contraseña invalidos");
                    }

                    var suscriptor = _context.ReplicacionesSuscriptores.Where(x => x.UsuInicioSesion == args.RepCodigo && x.RepSuscriptor.ToString().Substring(0, 8) == args.Suscriptor).FirstOrDefault();

                    if (suscriptor == null)
                    {
                        return BadRequest("Key invalido!");
                    }

                    if (suscriptor.resEstado != 1)
                    {
                        return BadRequest("No disponible para carga inicial");
                    }

                    if (suscriptor.RsuTipo == 1)
                    {
                        VerificarLicencia(_context);
                    }

                    SqlConnection conn = (SqlConnection)(_context.Database.GetDbConnection());
                    SqlCommand cmd = new SqlCommand("[sp_MDSOFTGeneraCargaInicialSuscriptoresCambios] @RepID, @RepSuscriptor", conn)
                    {
                        CommandTimeout = 300
                    };

                    if (conn.State != ConnectionState.Open)
                        conn.Open();

                    cmd.Parameters.AddRange(
                         new SqlParameter[] {
                         new SqlParameter() { ParameterName = "@RepID", SqlDbType = System.Data.SqlDbType.Int, Value = suscriptor.RepID },
                         new SqlParameter() { ParameterName = "@RepSuscriptor", SqlDbType = System.Data.SqlDbType.VarChar, Size = int.MaxValue, Value = suscriptor.RepSuscriptor.ToString()}}
                         );
                    suscriptor.resEstado = 3;

                    await cmd.ExecuteNonQueryAsync();


                    var query = string.Format($"SELECT COUNT(RepID) AS Value FROM [ReplicacionesSuscriptoresCambios] with(nolock) where RepSuscriptor = '{suscriptor.RepSuscriptor}'");                    

                    var currentTransaction = _context.Database.CurrentTransaction;
                    cmd.Transaction = currentTransaction != null ? (SqlTransaction)currentTransaction.GetDbTransaction() : cmd.Transaction;

                    SqlDataAdapter adapt = new SqlDataAdapter(cmd);
                    DataTable result = new DataTable();

                    adapt.Fill(result);

                    if (currentTransaction == null)
                        conn.Close();

                    int cambios = (int)result.Rows[0].ItemArray.GetValue(0);

                    return Ok(new VerificarSuscriptorResult() { RepSuscriptor = suscriptor.RepSuscriptor.ToString(), CantidadCambios = cambios });
               // }

            }
            catch (Exception e)
            {
                string msgError = e.Message;

                if (writeLog)
                {
                    if (e.InnerException != null)
                    {
                        msgError += Environment.NewLine + "InnerException: " + e.InnerException.Message;
                    }

                    Functions.WriteLog(args.RepCodigo, $"VerificarSuscriptor() - Error validando suscriptor en MovilBusiness. Usuario: {args.RepCodigo}, Key del suscriptor: {args.Suscriptor}, Mensaje: {msgError}");
                }

                return BadRequest(e.Message);
            }
        }

        [HttpPost]
        [Route("api/ReplicacionesSuscriptores/ReplicacionesTablasLeer")]
        public IActionResult ReplicacionesTablasLeer([FromBody] ReplicacionesTablasLeerArgs args)
        {
            var writeLog = false;

            try
            {

                using (MBContext db = new MBContext())
                {
                    writeLog = db.Parametros.Where(x => x.ParReferencia.Trim().ToUpper() == "WRITELOG" && x.ParValor.Trim() == "1").FirstOrDefault() != null;

                    if (!UsuarioSistema.Exists(args.RepCodigo, args.RepClave, db))
                    {
                        if (writeLog)
                        {
                            Functions.WriteLog(args.RepCodigo, $"ReplicacionesTablasLeer(). No existe este usuario en los usuarios del sistema., UsuInicioSesion: {args.RepCodigo}.");
                        }

                        return BadRequest("Este usuario no existe en los usuarios del sistema");
                    }

                    var replicacion = db.Replicaciones.Where(x => x.RepNombre == args.RepNombre).FirstOrDefault();

                    if (replicacion == null)
                    {
                        return BadRequest("La Replicacion solicitada no existe");
                    }

                    var list = db.ReplicacionesTablas.Where(x => x.RepID == replicacion.RepID).ToList();

                    return Ok(list);

                }

            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpPost]
        [Route("api/ReplicacionesSuscriptores/ReplicacionesTablasLeerNew")]
        public IActionResult ReplicacionesTablasLeerNew([FromBody] ReplicacionesTablasLeerArgs args)
        {
            var writeLog = false;

            try
            {

                using (MBContext db = new MBContext())
                {
                    writeLog = db.Parametros.Where(x => x.ParReferencia.Trim().ToUpper() == "WRITELOG" && x.ParValor.Trim() == "1").FirstOrDefault() != null;

                    if (!UsuarioSistema.Exists(args.RepCodigo, args.RepClave, db))
                    {
                        if (writeLog)
                        {
                            Functions.WriteLog(args.RepCodigo, $"ReplicacionesTablasLeer(). No existe este usuario en los usuarios del sistema., UsuInicioSesion: {args.RepCodigo}.");
                        }

                        return BadRequest("Este usuario no existe en los usuarios del sistema");
                    }

                    var suscriptor = db.ReplicacionesSuscriptores.Where(x => x.RepSuscriptor.ToString().Trim().ToUpper() == args.RepSuscriptor.Trim().ToUpper()).FirstOrDefault();

                    if (suscriptor == null)
                    {
                        return BadRequest("La Replicacion solicitada no existe");
                    }

                    var list = db.ReplicacionesTablas.Where(x => x.RepID == suscriptor.RepID).ToList();

                    return Ok(list);

                }

            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }


        [HttpPost]
        [Route("api/ReplicacionesSuscriptores/ExecuteQuery")]
        public IActionResult ExecuteQuery([FromBody] ExecuteQueryArgs args)
        {
            if (args == null || args.Values == null || args.User == null)
            {
                return Ok();
            }

            bool writeLog = false;

            try
            {
                using (MBContext db = new MBContext())
                {
                    writeLog = db.Parametros.Where(x => x.ParReferencia.Trim().ToUpper() == "WRITELOG" && x.ParValor.Trim() == "1").FirstOrDefault() != null;
                    var noValidarSuscriptor = writeLog;

                    if (writeLog)
                    {
                        Functions.WriteLog(args.User.RepCodigo, $"ExecuteQuery() - Iniciado. RepCodigo: {args.User.RepCodigo}, Script:  {JsonConvert.SerializeObject(args.Values)}" +
                        $"ConFecha: {DateTime.Now.ToString("dd/MM/yyyy")}");
                    }

                    if (!UsuarioSistema.Exists(args.User.RepCodigo, args.User.RepClave, db))
                    {
                        if (writeLog)
                        {
                            Functions.WriteLog(args.User.RepCodigo, $"ExecuteQuery(). No existe este usuario en los usuarios del sistema., UsuInicioSesion: '{args.User.RepCodigo}'. UsuClave: '{args.User.RepClave}'");
                        }
                        return BadRequest("No existe este usuario en los usuarios del sistema.");
                    }

                    var existsUser = true;

                    if (!noValidarSuscriptor)
                    {
                        var suscriptorn = db.ReplicacionesSuscriptores.Where(x => x.UsuInicioSesion.Trim() == args.User.RepCodigo.Trim() && x.RepSuscriptor.ToString() == args.User.Suscriptor).FirstOrDefault();                        

                        if (suscriptorn != null)
                        {
                            if (writeLog)
                            {
                                Functions.WriteLog(args.User.RepCodigo, $"ExecuteQuery(). No existe el suscriptor solicitado en la tabla de ReplicacionesSuscriptores, RepSuscriptor: {args.User.Suscriptor}.");
                            }

                            return BadRequest("No existe el suscriptor solicitado");
                        }
                       
                        if (suscriptorn.resEstado == 0)
                        {
                            return BadRequest("Suscriptor Bloqueado.");
                        }
                    }

                    string CurrentTable = "", CurrentScript = "";

                    try
                    {
                        try
                        {
                            SqlConnection conn = (SqlConnection)(db.Database.GetDbConnection());
                            SqlCommand cmd = new SqlCommand("[sp_MDSOFTGeneraCargaInicialSuscriptoresCambios] @RepID, @RepSuscriptor", conn)
                            {
                                CommandTimeout = 300
                            };
                            conn.Open();
                            foreach (ExecuteQueryValues value in args.Values)
                            {
                                cmd.CommandText = "[sp_MDSOFTGeneraCargaInicialSuscriptoresCambios] @RepID, @RepSuscriptor";
                                cmd.Parameters.AddWithValue("@query", value.Query);
                                cmd.ExecuteNonQuery();
                            }
                            conn.Close();
                        }
                        catch (Exception e)
                        {
                            throw e;
                        }
                    }
                    catch (Exception e)
                    {
                        string msgError = e.Message;

                        if (writeLog)
                        {
                            if (e.InnerException != null)
                            {
                                msgError += Environment.NewLine + "InnerException: " + e.InnerException.Message;
                            }

                            Functions.WriteLog(args.User.RepCodigo, $"ExecuteQuery(). Error: { msgError}, CurrentScript: {CurrentScript}, QueryList:  { JsonConvert.SerializeObject(args.Values) }");
                        }

                        if (e is SqlException)
                        {
                            if (!existsUser)
                            {
                                if (writeLog)
                                {
                                    Functions.WriteLog(args.User.RepCodigo, $"ExecuteQuery(). No existe este usuario en los usuarios del sistema., UsuInicioSesion: {args.User.RepCodigo}. UsuClave: {args.User.RepClave}");
                                }

                                return BadRequest(e.Message);
                            }

                            try
                            {
                                var conflicto = new ReplicacionesSuscriptoresConflictos
                                {
                                    RepID = !noValidarSuscriptor ? 1 : 0,
                                    RscTabla = CurrentTable,
                                    RscScript = CurrentScript,
                                    UsuInicioSesion = args.User.RepCodigo,
                                    RscFechaActualizacion = DateTime.Now
                                };

                                Guid.TryParse(args.User.Suscriptor, out Guid guid);
                                conflicto.RepSuscriptor = guid;
                                db.ReplicacionesSuscriptoresConflictos.Add(conflicto);
                                db.SaveChanges();

                            }
                            catch (Exception ex)
                            {
                                Console.Write(ex.Message);
                            }
                        }

                        throw e;
                    }
                }

                return Ok();

            }
            catch (Exception e)
            {
                string msgError = e.Message;

                if (writeLog)
                {
                    if (e.InnerException != null)
                    {
                        msgError += Environment.NewLine + "InnerException: " + e.InnerException.Message;
                    }

                    Functions.WriteLog(args.User.RepCodigo, $"ExecuteQuery(). Error: { msgError}, QueryList:  { JsonConvert.SerializeObject(args.Values) }");
                }

                return BadRequest(e.Message);
            }
        }

        [HttpPost]
        [Route("api/ReplicacionesSuscriptores/SuscriptoresCambiosLeer")]
        public async Task<IActionResult> SuscriptoresCambiosLeer([FromBody] ReplicacionesSuscriptoresCambiosLeerArgs args)
        {
            var writeLog = false;

            try
            {
                using (MBContext db = new MBContext())
                {
                    writeLog = db.Parametros.Where(x => x.ParReferencia.Trim().ToUpper() == "WRITELOG" && x.ParValor.Trim() == "1").FirstOrDefault() != null;

                    if (writeLog)
                    {
                        Functions.WriteLog(args.User.RepCodigo, $"SuscriptoresCambiosLeer() - Iniciado. RepSuscriptor: {args.User.Suscriptor}");
                    }

                    if (!UsuarioSistema.Exists(args.User.RepCodigo, args.User.RepClave, db))
                    {
                        if (writeLog)
                        {
                            Functions.WriteLog(args.User.RepCodigo, $"ExecuteQuery(). No existe este usuario en los usuarios del sistema., UsuInicioSesion: {args.User.RepCodigo}.");
                        }
                        return BadRequest("No existe este usuario en los usuarios del sistema.");
                    }

                    var suscriptor = db.ReplicacionesSuscriptores.Where(x => x.UsuInicioSesion == args.User.RepCodigo && x.RepSuscriptor.ToString() == args.User.Suscriptor).FirstOrDefault();

                    if (suscriptor == null)
                    {
                        if (writeLog)
                        {
                            Functions.WriteLog(args.User.RepCodigo, $"ExecuteQuery(). No existe el suscriptor solicitado en la tabla de ReplicacionesSuscriptores, RepSuscriptor: {args.User.Suscriptor}.");
                        }

                        return BadRequest("No existe el suscriptor solicitado");
                    }


                    SqlConnection conn = (SqlConnection)(db.Database.GetDbConnection());

                    if (!string.IsNullOrWhiteSpace(args.DeleteQuery))
                    {
                        SqlCommand cmd = new SqlCommand("[sp_wsExecQuery] @query", conn)
                        {
                            CommandTimeout = 300
                        };

                        cmd.Parameters.AddWithValue("@query", args.DeleteQuery);
                        await cmd.ExecuteNonQueryAsync();
                    }

                    var query = string.Format("select top({0}) RscKey, RepID, RscTabla, RscTipTran, RscFechaActualizacion, RscScript, RscTablarowguid, UsuInicioSesion, RepSuscriptor from [ReplicacionesSuscriptoresCambios{1}] with(nolock) WHERE RscTipTran <> 'F' order by RscFechaActualizacion ASC, rscTabla ", args.Limit, args.User.Suscriptor);


                    var currentTransaction = db.Database.CurrentTransaction;
                    SqlCommand command1 = new SqlCommand(query, conn)
                    {
                        CommandTimeout = 300
                    };
                    command1.Transaction = currentTransaction != null ? (SqlTransaction)currentTransaction.GetDbTransaction() : command1.Transaction;

                    SqlDataAdapter adapt = new SqlDataAdapter(command1);
                    DataTable result = new DataTable();

                    adapt.Fill(result);

                    conn.Close();

                    var list = (ReplicacionesSuscriptoresCambios)result.Rows.AsQueryable();


                    if (writeLog && list == null)
                    {
                        Functions.WriteLog(args.User.RepCodigo, $"SuscriptoresCambiosLeer(). No existen mas cambios en la tabla de SuscriptoresCambios. RepCodigo: {args.User.RepCodigo}, RepSuscriptor: {args.User.Suscriptor}");
                    }

                    if (writeLog)
                    {
                        Functions.WriteLog(args.User.RepCodigo, $"SuscriptoresCambiosLeer(). Fin. RepCodigo: {args.User.RepCodigo}, RepSuscriptor: {args.User.Suscriptor}");
                    }

                    return Ok(list);

                }
            }
            catch (Exception e)
            {
                string msgError = e.Message;

                if (writeLog)
                {
                    if (e.InnerException != null)
                    {
                        msgError += Environment.NewLine + "InnerException: " + e.InnerException.Message;
                    }

                    Functions.WriteLog(args.User.RepCodigo, $"SuscriptoresCambiosLeer() - Error obteniendo cambios en MovilBusiness. RepCodigo: {args.User.RepCodigo}, Error: {msgError}");
                }

                return BadRequest(e.Message);
            }

        }

        [HttpPost]
        [Route("api/ReplicacionesSuscriptores/SuscriptoresCambiosLeerV8")]
        public IActionResult SuscriptoresCambiosLeerV8([FromBody] ReplicacionesSuscriptoresCambiosLeerArgs args)
        {
            var writeLog = false;

            try
            {
                using (MBContext db = new MBContext())
                {
                    writeLog = db.Parametros.Where(x => x.ParReferencia.Trim().ToUpper() == "WRITELOG" && x.ParValor.Trim() == "1").FirstOrDefault() != null;

                    if (writeLog)
                    {
                        Functions.WriteLog(args.User.RepCodigo, $"SuscriptoresCambiosLeer() - Iniciado. RepSuscriptor: {args.User.Suscriptor}");
                    }

                    if (!UsuarioSistema.Exists(args.User.RepCodigo, args.User.RepClave, db))
                    {
                        if (writeLog)
                        {
                            Functions.WriteLog(args.User.RepCodigo, $"ExecuteQuery(). No existe este usuario en los usuarios del sistema., UsuInicioSesion: {args.User.RepCodigo}.");
                        }
                        return BadRequest("No existe este usuario en los usuarios del sistema.");
                    }

                    var suscriptor = db.ReplicacionesSuscriptores.Where(x => x.UsuInicioSesion == args.User.RepCodigo && x.RepSuscriptor.ToString() == args.User.Suscriptor).FirstOrDefault();

                    if (suscriptor == null)
                    {
                        if (writeLog)
                        {
                            Functions.WriteLog(args.User.RepCodigo, $"ExecuteQuery(). No existe el suscriptor solicitado en la tabla de ReplicacionesSuscriptores, RepSuscriptor: {args.User.Suscriptor}.");
                        }

                        return BadRequest("No existe el suscriptor solicitado");
                    }

                    SqlConnection conn = (SqlConnection)(db.Database.GetDbConnection());
                    conn.Open();

                    if (!string.IsNullOrWhiteSpace(args.DeleteQuery))
                    {
                        SqlCommand command = new SqlCommand("[sp_wsExecQuery] @query", conn)
                        {
                            CommandTimeout = 300
                        };
                        command.Parameters.AddWithValue("@query", args.DeleteQuery);
                        command.ExecuteNonQuery();
                    }

                    string query = "";
                    if(args.IsSincronizar)
                    {
                        query = $"select top({args.Limit}) RscKey, RscScript from [ReplicacionesSuscriptoresCambios{args.User.Suscriptor}] with(nolock) WHERE RscTipTran <> 'F' order by RscFechaActualizacion ASC, rscTabla ";
                    }
                    else
                    {
                        query = $"select top({args.Limit}) RscKey, RscScript from [ReplicacionesSuscriptoresCambios{args.User.Suscriptor}] with(nolock) WHERE RscTipTran <> 'F'";
                    }

                    var currentTransaction = db.Database.CurrentTransaction;
                    SqlCommand command1 = new SqlCommand(query, conn)
                    {
                        CommandTimeout = 300
                    };
                    command1.Transaction = currentTransaction != null ? (SqlTransaction)currentTransaction.GetDbTransaction() : command1.Transaction;

                    SqlDataAdapter adapt = new SqlDataAdapter(command1);
                    DataTable result = new DataTable();

                    adapt.Fill(result);

                    conn.Close();

                    var list = (ReplicacionesSuscriptoresCambios)result.Rows.AsQueryable();

                    if (writeLog && list == null)
                    {
                        Functions.WriteLog(args.User.RepCodigo, $"SuscriptoresCambiosLeer(). No existen mas cambios en la tabla de SuscriptoresCambios. RepCodigo: {args.User.RepCodigo}, RepSuscriptor: {args.User.Suscriptor}");
                    }

                    if (writeLog)
                    {
                        Functions.WriteLog(args.User.RepCodigo, $"SuscriptoresCambiosLeer(). Fin. RepCodigo: {args.User.RepCodigo}, RepSuscriptor: {args.User.Suscriptor}");
                    }

                    return Ok(list);
                }
            }
            catch (Exception e)
            {
                if (writeLog)
                {
                    Functions.WriteLog(args.User.RepCodigo, $"SuscriptoresCambiosLeer() - Error obteniendo cambios en MovilBusiness. RepCodigo: {args.User.RepCodigo}, Error: {e.Message}");
                }

                return BadRequest(e.Message);
            }

        }


        [HttpPost]
        [Route("api/ReplicacionesSuscriptores/SuscriptoresCambiosLeerV9")]
        public IActionResult SuscriptoresCambiosLeerV9([FromBody] ReplicacionesSuscriptoresCambiosLeerArgs args)
        {
            var writeLog = false;

            try
            {
                using (MBContext db = new MBContext())
                {
                    writeLog = db.Parametros.Where(x => x.ParReferencia.Trim().ToUpper() == "WRITELOG" && x.ParValor.Trim() == "1").FirstOrDefault() != null;

                    if (writeLog)
                    {
                        Functions.WriteLog(args.User.RepCodigo, $"SuscriptoresCambiosLeer() - Iniciado. RepSuscriptor: {args.User.Suscriptor}");
                    }

                    if (!UsuarioSistema.Exists(args.User.RepCodigo, args.User.RepClave, db))
                    {
                        if (writeLog)
                        {
                            Functions.WriteLog(args.User.RepCodigo, $"ExecuteQuery(). No existe este usuario en los usuarios del sistema., UsuInicioSesion: {args.User.RepCodigo}.");
                        }
                        return BadRequest("No existe este usuario en los usuarios del sistema.");
                    }

                    var suscriptor = db.ReplicacionesSuscriptores.Where(x => x.UsuInicioSesion == args.User.RepCodigo && x.RepSuscriptor.ToString() == args.User.Suscriptor).FirstOrDefault();

                    if (suscriptor == null)
                    {
                        if (writeLog)
                        {
                            Functions.WriteLog(args.User.RepCodigo, $"ExecuteQuery(). No existe el suscriptor solicitado en la tabla de ReplicacionesSuscriptores, RepSuscriptor: {args.User.Suscriptor}.");
                        }

                        return BadRequest("No existe el suscriptor solicitado");
                    }

                    SqlConnection conn = (SqlConnection)(db.Database.GetDbConnection());
                    conn.Open();

                    if (!string.IsNullOrWhiteSpace(args.DeleteQuery))
                    {
                        SqlCommand command = new SqlCommand("[sp_wsExecQuery] @query", conn)
                        {
                            CommandTimeout = 300
                        };
                        command.Parameters.AddWithValue("@query", args.DeleteQuery);
                        command.ExecuteNonQuery();
                    }

                    string query = "";
                    if (args.IsSincronizar)
                    {
                        query = $"select top({args.Limit}) RscKey, RscScript from [ReplicacionesSuscriptoresCambios{args.User.Suscriptor}] with(nolock) WHERE RscTipTran <> 'F' order by RscFechaActualizacion ASC, rscTabla ";
                    }
                    else
                    {
                        query = $"select top({args.Limit}) RscKey, RscScript from [ReplicacionesSuscriptoresCambios{args.User.Suscriptor}] with(nolock) WHERE RscTipTran <> 'F' order by RscConTranID";
                    }

                    var currentTransaction = db.Database.CurrentTransaction;
                    SqlCommand command1 = new SqlCommand(query, conn)
                    {
                        CommandTimeout = 300
                    };
                    command1.Transaction = currentTransaction != null ? (SqlTransaction)currentTransaction.GetDbTransaction() : command1.Transaction;

                    SqlDataAdapter adapt = new SqlDataAdapter(command1);
                    DataTable result = new DataTable();

                    adapt.Fill(result);

                    conn.Close();

                    var list = (ReplicacionesSuscriptoresCambios)result.Rows.AsQueryable();

                    if (writeLog && list == null)
                    {
                        Functions.WriteLog(args.User.RepCodigo, $"SuscriptoresCambiosLeer(). No existen mas cambios en la tabla de SuscriptoresCambios. RepCodigo: {args.User.RepCodigo}, RepSuscriptor: {args.User.Suscriptor}");
                    }

                    if (writeLog)
                    {
                        Functions.WriteLog(args.User.RepCodigo, $"SuscriptoresCambiosLeer(). Fin. RepCodigo: {args.User.RepCodigo}, RepSuscriptor: {args.User.Suscriptor}");
                    }

                    return Ok(list);
                }
            }
            catch (Exception e)
            {
                if (writeLog)
                {
                    Functions.WriteLog(args.User.RepCodigo, $"SuscriptoresCambiosLeer() - Error obteniendo cambios en MovilBusiness. RepCodigo: {args.User.RepCodigo}, Error: {e.Message}");
                }

                return BadRequest(e.Message);
            }

        }

        [HttpPost]
        [Route("api/ReplicacionesSuscriptores/SuscriptoresCambiosLeerLegacy")]
        public IActionResult SuscriptoresCambiosLeerLegacy([FromBody] ReplicacionesSuscriptoresCambiosLeerArgs args)
        {
            var writeLog = false;

            try
            {
                using (MBContext db = new MBContext())
                {
                    writeLog = db.Parametros.Where(x => x.ParReferencia.Trim().ToUpper() == "WRITELOG" && x.ParValor.Trim() == "1").FirstOrDefault() != null;

                    if (writeLog)
                    {
                        Functions.WriteLog(args.User.RepCodigo, $"SuscriptoresCambiosLeerLegacy() - Iniciado. RepSuscriptor: {args.User.Suscriptor}");
                    }

                    if (!UsuarioSistema.Exists(args.User.RepCodigo, args.User.RepClave, db))
                    {
                        if (writeLog)
                        {
                            Functions.WriteLog(args.User.RepCodigo, $"SuscriptoresCambiosLeerLegacy(). No existe este usuario en los usuarios del sistema., UsuInicioSesion: '{args.User.RepCodigo}'. UsuClave: '{args.User.RepClave}'");
                        }

                        return BadRequest("No existe este usuario en los usuarios del sistema.");
                    }

                    var suscriptor = db.ReplicacionesSuscriptores.Where(x => x.UsuInicioSesion.Trim() == args.User.RepCodigo.Trim() && x.RepSuscriptor.ToString().Trim().ToUpper() == args.User.Suscriptor.Trim().ToUpper()).FirstOrDefault();

                    if (suscriptor == null)
                    {
                        if (writeLog)
                        {
                            Functions.WriteLog(args.User.RepCodigo, $"SuscriptoresCambiosLeerLegacy(). No existe el suscriptor solicitado en la tabla de ReplicacionesSuscriptores, RepSuscriptor: {args.User.Suscriptor}, UsuInicioSesion: {args.User.RepCodigo}");
                        }

                        return BadRequest("No existe el suscriptor solicitado");
                    }


                    SqlConnection conn = (SqlConnection)(db.Database.GetDbConnection());
                    SqlCommand command = new SqlCommand("sp_wsPresupuestosCargarOnline", conn)
                    {
                        CommandTimeout = 300
                    };
                    conn.Open();

                    if (!string.IsNullOrWhiteSpace(args.DeleteQuery))
                    {
                        command.CommandText = "[sp_wsExecQuery] @query";
                        command.Parameters.AddWithValue("@query", args.DeleteQuery);
                        command.ExecuteNonQuery();
                    }

                    var query = string.Format($"select top({args.Limit}) RscKey, RepID, RscTabla, RscTipTran, RscFechaActualizacion, RscScript, RscTablarowguid, UsuInicioSesion, RepSuscriptor from [ReplicacionesSuscriptoresCambios] with(nolock) where RepSuscriptor = '{args.User.Suscriptor}' and RscTipTran <> 'F' order by RscFechaActualizacion ASC, rscTabla ");               

                    var currentTransaction = db.Database.CurrentTransaction;
                    command.Transaction = currentTransaction != null ? (SqlTransaction)currentTransaction.GetDbTransaction() : command.Transaction;

                    SqlDataAdapter adapt = new SqlDataAdapter(command);
                    DataTable result = new DataTable();

                    adapt.Fill(result);

                    conn.Close();

                    var list = (ReplicacionesSuscriptoresCambios)result.Rows.AsQueryable();

                    if (writeLog && list == null)
                    {
                        Functions.WriteLog(args.User.RepCodigo, $"SuscriptoresCambiosLeer(). No existen mas cambios en la tabla de SuscriptoresCambios. RepCodigo: {args.User.RepCodigo}, RepSuscriptor: {args.User.Suscriptor}");
                    }

                    return Ok(list);
                }
            }
            catch (Exception e)
            {
                string msgError = e.Message;

                if (writeLog)
                {
                    if (e.InnerException != null)
                    {
                        msgError += Environment.NewLine + "InnerException: " + e.InnerException.Message;
                    }

                    Functions.WriteLog(args.User.RepCodigo, $"SuscriptoresCambiosLeer() - Error obteniendo cambios en MovilBusiness. RepCodigo: {args.User.RepCodigo}, Error: {msgError}");
                }

                return BadRequest(e.Message);
            }

        }

        [HttpPost]
        [Route("api/ReplicacionesSuscriptores/EmpresasCargar")]
        public IActionResult EmpresasCargar([FromBody] UsuarioArgs args)
        {
            try
            {
                using (MBContext db = new MBContext())
                {
                    if (!UsuarioSistema.Exists(args.RepCodigo, args.RepClave, db))
                    {
                        //Functions.WriteLog(args.RepCodigo, $"ExecuteQuery(). No existe este usuario en los usuarios del sistema., UsuInicioSesion: {args.RepCodigo}.");
                        return BadRequest("No existe este usuario en los usuarios del sistema.");
                    }

                    var list = db.Empresa.ToList();

                    return Ok(list);
                }

            }
            catch (Exception e)
            {
                return BadRequest(e.InnerException == null || string.IsNullOrWhiteSpace(e.InnerException.Message) ? e.Message : e.InnerException.Message);
            }

        }

        [HttpPost]
        [Route("api/ReplicacionesSuscriptores/ExecUpdate")]
        public IActionResult ExecUpdate([FromBody]ExecUpdateArgs args)
        {
            //string CurrentScript = "";

            if (args == null)
            {
                return Ok();
            }

            if (args.User == null)
            {
                return BadRequest("Usuario invalido");
            }

            var writeLog = false;

            try
            {
                using (MBContext db = new MBContext())
                {
                    writeLog = db.Parametros.Where(x => x.ParReferencia.Trim().ToUpper() == "WRITELOG" && x.ParValor.Trim() == "1").FirstOrDefault() != null;

                    if (!UsuarioSistema.Exists(args.User.RepCodigo, args.User.RepClave, db))
                    {
                        if (writeLog)
                        {
                            Functions.WriteLog(args.User.RepCodigo, $"ExecuteQuery(). No existe este usuario en los usuarios del sistema., UsuInicioSesion: {args.User.RepCodigo}.");
                        }

                        return BadRequest("No existe este usuario en los usuarios del sistema");
                    }

                    try
                    {
                        SqlConnection conn = (SqlConnection)(db.Database.GetDbConnection());
                        SqlCommand cmd = new SqlCommand("[sp_wsExecQuery] @query", conn)
                        {
                            CommandTimeout = 300
                        };
                        cmd.Parameters.AddWithValue("@query", args.Query);
                        cmd.ExecuteNonQuery();
                    }
                    catch (Exception ex)
                    {
                        string msgError = ex.Message;

                        if (writeLog)
                        {
                            if (ex.InnerException != null)
                            {
                                msgError += Environment.NewLine + "InnerException" + ex.InnerException.Message;
                            }

                            Functions.WriteLog(args.User.RepCodigo, $"ExecUpdate() - Error ejecutando cambios. RepCodigo: {args.User.RepCodigo}, Query: {args.Query}, Error: {msgError}, Query: " + args.Query); ;
                        }

                        throw ex;
                    }

                    return Ok();
                }

            }
            catch (Exception e)
            {
                /*Functions.Writelog(RepCodigo, "execQuery2() - Error ejecutando cambios en MovilVisitas. " &
                         "RepCodigo: " & RepCodigo & ", RepClave: " & RepClave &
                         ", Query: " & query &
                         ". Error: " & ex.Message.ToString)*/
                string msgError = e.Message;

                if (writeLog)
                {
                    if (e.InnerException != null)
                    {
                        msgError += Environment.NewLine + "InnerException: " + e.InnerException.Message;
                    }

                    Functions.WriteLog(args.User.RepCodigo, $"ExecUpdate() - Error ejecutando cambios. RepCodigo: {args.User.RepCodigo}, Query: {args.Query}, Error: {msgError}"); ;
                }

                return BadRequest(e.Message);
            }

        }

        [HttpPost]
        [Route("api/ReplicacionesSuscriptores/RawQuery")]
        public IActionResult RawQuery([FromBody]QueryArgs args)
        {
            var writeLog = false;

            try
            {
                if (args.Query == null || args.Query.Trim().Length == 0)
                {
                    return BadRequest("El query a ejecutar no puede estar vacio");
                }

                if (args.Query.ToLower().Contains("alter") || args.Query.ToLower().Contains("drop"))
                {
                    return BadRequest("El query a ejecutar contiene sentencias invalidas");
                }

                using (MBContext db = new MBContext())
                {
                    writeLog = db.Parametros.Where(x => x.ParReferencia.Trim().ToUpper() == "WRITELOG" && x.ParValor.Trim() == "1").FirstOrDefault() != null;

                    if (writeLog)
                    {
                        Functions.WriteLog(args.User.RepCodigo, "RawQuery() - Iniciando - RepCodigo: " + args.User.RepCodigo + ". EN Fecha: " + DateTime.Now.ToString("dd-MM-yyyy hh:mm:ss"));
                    }

                    if (!UsuarioSistema.Exists(args.User.RepCodigo, args.User.RepClave, db))
                    {
                        if (writeLog)
                        {
                            Functions.WriteLog(args.User.RepCodigo, $"ExecuteQuery(). No existe este usuario en los usuarios del sistema., UsuInicioSesion: {args.User.RepCodigo}.");
                        }

                        return BadRequest("No existe este usuario en los usuarios del sistema");
                    }

                    SqlConnection connection = (SqlConnection)db.Database.GetDbConnection();

                    DataTable dataSet = new DataTable();

                    SqlDataAdapter adapter = new SqlDataAdapter(args.Query, connection);

                    adapter.Fill(dataSet);

                    return Ok(dataSet);
                }

            }
            catch (Exception e)
            {
                string msgError = e.Message;

                if (writeLog)
                {
                    if (e.InnerException != null)
                    {
                        msgError += Environment.NewLine + "InnerException: " + e.InnerException.Message;
                    }

                    Functions.WriteLog(args.User.RepCodigo, "RawQuery() - Error procesando - RepCodigo: " + args.User.RepCodigo + ". Mensaje: " + msgError);
                }

                return BadRequest(e.Message);
            }
        }

        [Route("api/ReplicacionesSuscriptores/CargarPresupuestosBySupervisorSync")]
        [HttpPost]
        public IActionResult CargarPresupuestosBySupervisorSync([FromBody]PresupuestosArgs args)
        {
            var result = new List<PresupuestosResult>();

            try
            {
                using (MBContext db = new MBContext())
                {
                    if (!UsuarioSistema.Exists(args.RepCodigo, args.RepClave, db))
                    {
                        return BadRequest("No existe este usuario en los usuarios del sistema");
                    }
                    SqlConnection conn = (SqlConnection)db.Database.GetDbConnection();
                    SqlCommand command = new SqlCommand("sp_wsCargarPresupuestosSync", conn)
                    {
                        CommandTimeout = 300,
                    };
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandTimeout = 0;
                    command.Parameters.Add(new SqlParameter("@RepCodigo", args.RepCodigo));
                    command.Parameters.Add(new SqlParameter("@FechaUltimaActualizacion", args.FechaUltimaActualizacion));

                    var adapter = new SqlDataAdapter(command);

                    var dataSet = new DataSet();

                    adapter.Fill(dataSet);

                    var iterator = dataSet.Tables[0].Rows.GetEnumerator();

                    while (iterator.MoveNext())
                    {
                        var current = (DataRow)iterator.Current;

                        var presu = new PresupuestosResult();

                        presu.RepCodigo = current["RepCodigo"].ToString();
                        presu.RepNombre = current["RepNombre"].ToString();
                        presu.ZonDescripcion = current["ZonDescripcion"].ToString();
                        presu.PreTipo = current["PreTipo"].ToString();
                        presu.PreAnio = short.Parse(current["PreAnio"].ToString());
                        presu.PreMes = short.Parse(current["PreMes"].ToString());
                        presu.ProNombre = current["ProNombre"].ToString();
                        presu.ProDescripcion = current["ProDescripcion"].ToString();
                        presu.LinID = int.Parse(current["LinID"].ToString());
                        presu.LinDescripcion = current["LinDescripcion"].ToString();
                        presu.PreReferencia = current["PreReferencia"].ToString();
                        presu.PreDescripcion = current["PreDescripcion"].ToString();
                        presu.PrePresupuesto = decimal.Parse(current["PrePresupuesto"].ToString());
                        presu.PreEjecutado = decimal.Parse(current["PreEjecutado"].ToString());
                        presu.PreCumplimiento = decimal.Parse(current["PreCumplimiento"].ToString());

                        result.Add(presu);
                    }

                    return Ok(result);
                }

            }
            catch (Exception e)
            {
                Console.Write(e.Message);
                return Ok(result);
            }
        }

        [Route("api/ReplicacionesSuscriptores/CoberturaConsultar")]
        [HttpPost]
        public IActionResult CoberturaConsultar([FromBody]CoberturaArgs args)
        {
            string str = null;
            IEnumerator enumerator = null;
            IEnumerator enumerator1 = null;
            SqlConnection sqlConnection = null;
            SqlCommand sqlCommand = null;
            SqlDataReader sqlDataReader = null;
            DataTable dataTable = new DataTable();
            CoberturaConsultarStruct count = new CoberturaConsultarStruct();

            try
            {
                try
                {

                    using (var db = new MBContext())
                    {
                        sqlConnection = (SqlConnection)db.Database.GetDbConnection();
                        sqlConnection.Open();
                        sqlCommand = sqlConnection.CreateCommand();
                        sqlCommand.CommandType = CommandType.StoredProcedure;
                        sqlCommand.CommandText = "sp_wsCoberturaConsultar";
                        sqlCommand.Parameters.AddWithValue("@CobeTipo", args.CobeTipo);
                        sqlCommand.Parameters.AddWithValue("@RepCodigo", args.RepCodigo);
                        sqlCommand.Parameters.AddWithValue("@CicID", args.CicID);
                        sqlCommand.Parameters.AddWithValue("@Parametros", args.Parametros);
                        sqlDataReader = sqlCommand.ExecuteReader();
                        var strs = new List<string>();
                        var strs1 = new List<string>();
                        var dictionaries = new List<Dictionary<string, string>>();
                        var num = 0;
                        dataTable.Load(sqlDataReader);

                        try
                        {
                            enumerator = dataTable.Columns.GetEnumerator();

                            while (enumerator.MoveNext())
                            {
                                strs.Add(((DataColumn)enumerator.Current).ColumnName);
                                strs1.Add(dataTable.Rows[0][num].ToString());
                                num++;
                            }
                        }
                        catch (Exception e)
                        {
                            str = e.Message + ": 1";
                            throw e;
                        }
                        finally
                        {
                            if (enumerator.GetType() is IDisposable)
                            {
                                ((IDisposable)enumerator).Dispose();
                            }
                        }

                        var num1 = 0;
                        var count1 = dataTable.Rows.Count - 1;
                        num = 1;

                        do
                        {
                            IDictionary dictionaries1 = new Dictionary<string, string>();

                            try
                            {
                                enumerator1 = dataTable.Columns.GetEnumerator();

                                while (enumerator1.MoveNext())
                                {
                                    var current = enumerator1.Current as DataColumn;
                                    dictionaries1.Add(num1.ToString(), dataTable.Rows[num][num1].ToString());
                                    num1++;
                                }
                            }
                            catch (Exception e)
                            {
                                str = e.Message + ": 2";
                                throw e;
                            }
                            finally
                            {
                                if (enumerator1.GetType() is IDisposable)
                                {
                                    ((IDisposable)enumerator1).Dispose();
                                }
                            }

                            dictionaries.Add((Dictionary<string, string>)dictionaries1);
                            num1 = 0;
                            num++;

                        } while (num < count1);

                        count.Count = dataTable.Rows.Count - 1;
                        count.Columnas = strs;
                        count.Col_Alig = strs1;
                        count.Registros = dictionaries;

                        //str = JsonConvert.SerializeObject(count);
                        return Ok(count);
                    }


                }
                catch (Exception ex)
                {
                    Console.Write(ex.Message);
                    if (str == null)
                    {
                        str = ex.Message + ": 3";
                    }
                }

            }
            finally
            {
                if (sqlConnection != null)
                {
                    sqlConnection.Close();
                }
                if (sqlDataReader != null)
                {
                    sqlDataReader.Close();
                }
            }

            return Ok();
        }

        [Route("api/ReplicacionesSuscriptores/ImagenesInsertar")]
        [HttpPost]
        public IActionResult TransaccionesImagenesInsertar([FromBody] ImagenesInsertarArgs args)
        {
            var writeLog = false;

            try
            {
                if (args == null || args.User == null || args.Imagenes == null || args.Imagenes.Count == 0)
                {
                    return Ok();
                }

                using (MBContext db = new MBContext())
                {
                    writeLog = db.Parametros.Where(x => x.ParReferencia.Trim().ToUpper() == "WRITELOG" && x.ParValor.Trim() == "1").FirstOrDefault() != null;

                    if (!UsuarioSistema.Exists(args.User.RepCodigo, args.User.RepClave, db))
                    {
                        if (writeLog)
                        {
                            Functions.WriteLog(args.User.RepCodigo, $"TransaccionesImagenesInsertar(). No existe este usuario en los usuarios del sistema., UsuInicioSesion: {args.User.RepCodigo}.");
                        }

                        return BadRequest("No existe este usuario en los usuarios del sistema.");
                    }

                    var currentTransaction = db.Database.CurrentTransaction;

                    try
                    {                        
                        foreach (TransaccionesImagenesTablasTemp img in args.Imagenes)
                        {
                            SqlConnection conn = (SqlConnection)(db.Database.GetDbConnection());
                            SqlCommand cmd = new SqlCommand("sp_TransaccionesImagenesTablasInsertar", conn)
                            {
                                CommandTimeout = 300
                            };

                            cmd.Transaction = currentTransaction != null ? (SqlTransaction)currentTransaction.GetDbTransaction() : cmd.Transaction;
                            cmd.CommandText = "sp_TransaccionesImagenesTablasInsertar";
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.CommandTimeout = 0;
                            cmd.Parameters.Add(new SqlParameter("@RepCodigo", args.User.RepCodigo));
                            cmd.Parameters.Add(new SqlParameter("@RepTabla", img.RepTabla));
                            cmd.Parameters.Add(new SqlParameter("@RepTablaKey", img.RepTablaKey));
                            cmd.Parameters.Add(new SqlParameter("@TraPosicion", img.TraPosicion));
                            cmd.Parameters.Add(new SqlParameter("@TraImagen", img.TraImagen));
                            cmd.Parameters.Add(new SqlParameter("@TraFormato", img.TraFormato));
                            cmd.Parameters.Add(new SqlParameter("@TraTamano", img.TraTamano));
                            cmd.Parameters.Add(new SqlParameter("@TitId", img.TitId));
                            cmd.Parameters.Add(new SqlParameter("@IsForFirma", img.ForFirma));

                            if (cmd.Connection.State != ConnectionState.Open)
                            {
                                cmd.Connection.Open();
                            }

                            cmd.ExecuteNonQuery();
                        }
                    }
                    catch (Exception e)
                    {
                        throw e;
                    }

                }

                return Ok();
            }
            catch (Exception e)
            {
                string msgError = e.Message;

                if (writeLog)
                {
                    if (e.InnerException != null)
                    {
                        msgError += Environment.NewLine + "InnerException: " + e.InnerException.Message;
                    }

                    Functions.WriteLog(args.User.RepCodigo, $"TransaccionesImagenesInsertar() Error. {msgError}, RepCodigo: {args.User.RepCodigo}.");
                }

                return BadRequest(e.Message);
            }
        }

        [Route("api/ReplicacionesSuscriptores/SuscriptoresCambiosLeerImagenes")]
        [HttpPost]
        public IActionResult SuscriptoresCambiosLeerImagenes([FromBody] ReplicacionesSuscriptoresCambiosLeerArgs args)
        {
            var writeLog = false;

            try
            {
                using (MBContext db = new MBContext())
                {
                    writeLog = db.Parametros.Where(x => x.ParReferencia.Trim().ToUpper() == "WRITELOG" && x.ParValor.Trim() == "1").FirstOrDefault() != null;

                    if (writeLog)
                    {
                        Functions.WriteLog(args.User.RepCodigo, $"SuscriptoresCambiosLeerImagenes() - Iniciado. RepSuscriptor: {args.User.Suscriptor}");
                    }

                    if (!UsuarioSistema.Exists(args.User.RepCodigo, args.User.RepClave, db))
                    {
                        return BadRequest("No existe este usuario en los usuarios del sistema.");
                    }

                    var suscriptor = db.ReplicacionesSuscriptores.Where(x => x.UsuInicioSesion.Trim() == args.User.RepCodigo.Trim() && x.RepSuscriptor.ToString().Trim().ToUpper() == args.User.Suscriptor.Trim().ToUpper()).FirstOrDefault();

                    if (suscriptor == null)
                    {
                        if (writeLog)
                        {
                            Functions.WriteLog(args.User.RepCodigo, $"SuscriptoresCambiosLeerImagenes(). No existe el suscriptor solicitado en la tabla de ReplicacionesSuscriptores, RepSuscriptor: {args.User.Suscriptor}.");
                        }

                        return BadRequest("No existe el suscriptor solicitado");
                    }

                    SqlConnection connection = (SqlConnection)(db.Database.GetDbConnection());

                    if (connection.State != ConnectionState.Open)
                        connection.Open();

                    DataTable dataSet = new DataTable();

                    SqlCommand cmd = new SqlCommand("[sp_wsReplicacionesSuscriptoresCambiosLeerImagenes] @RepID, @RepSuscriptor, @RepCantidad");
                    cmd.Parameters.Add(new SqlParameter("@RepID", suscriptor.RepID));
                    cmd.Parameters.Add(new SqlParameter("@RepSuscriptor", suscriptor.RepSuscriptor));
                    cmd.Parameters.Add(new SqlParameter("@RepCantidad", args.Limit));
                    cmd.Connection = connection;

                    SqlDataAdapter adapter = new SqlDataAdapter(cmd);

                    adapter.Fill(dataSet);

                    connection.Close();

                    if (writeLog && dataSet.Rows.Count == 0)
                    {
                        Functions.WriteLog(args.User.RepCodigo, $"SuscriptoresCambiosLeerImagenes(). No existen mas cambios en la tabla de SuscriptoresCambios. RepCodigo: {args.User.RepCodigo}, RepSuscriptor: {args.User.Suscriptor}");
                    }

                    if (writeLog)
                    {
                        Functions.WriteLog(args.User.RepCodigo, $"SuscriptoresCambiosLeerImagenes(). Fin. RepCodigo: {args.User.RepCodigo}, RepSuscriptor: {args.User.Suscriptor}");
                    }

                    return Ok(dataSet);
                }
            }
            catch (Exception e)
            {
                string msgError = e.Message;

                if (writeLog)
                {
                    if (e.InnerException != null)
                    {
                        msgError += Environment.NewLine + "InnerException: " + e.InnerException.Message;
                    }

                    Functions.WriteLog(args.User.RepCodigo, $"SuscriptoresCambiosLeerImagenes() - Error obteniendo cambios en MovilBusiness. RepCodigo: {args.User.RepCodigo}, Error: {msgError}");
                }

                return BadRequest(e.Message);
            }
        }

        [Route("api/ReplicacionesSuscriptores/SuscriptoresCambiosLeerImagenesLegacy")]
        [HttpPost]
        public IActionResult SuscriptoresCambiosLeerImagenesLegacy([FromBody] ReplicacionesSuscriptoresCambiosLeerArgs args)
        {
            return SuscriptoresCambiosLeerImagenes(args);
        }

        [Route("api/ReplicacionesSuscriptores/ImagenesCargar")]
        [HttpPost]
        public IActionResult ImagenesCargar([FromBody] ImagenesCargarArgs args)
        {
            try
            {
                using (MBContext db = new MBContext())
                {
                    if (!UsuarioSistema.Exists(args.User.RepCodigo, args.User.RepClave, db))
                    {
                        return BadRequest("No existe este usuario en los usuarios del sistema");
                    }

                    /*if (!File.Exists(args.ImagePath))
                    {
                        return BadRequest("La imagen solicitada no existe");
                    }*/

                    var img = Image.FromFile(args.ImagePath);

                    // return Ok(File.ReadAllBytes(args.ImagePath));

                    string result = "";

                    using (var stream = new MemoryStream())
                    {
                        img.Save(stream, ImageFormat.Jpeg);


                        result = Convert.ToBase64String(stream.ToArray());
                    }

                    return Ok(result);

                }

            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [Route("api/ReplicacionesSuscriptores/SubirSqliteDb")]
        [HttpPost]
        public IActionResult SubirSqliteDb([FromBody] SubirSqliteDbArgs args)
        {
            if (args == null || args.User == null)
            {
                return BadRequest("Las credenciales del usuario no son validas");
            }

            try
            {
                using (var db = new MBContext())
                {
                    if (!UsuarioSistema.Exists(args.User.RepCodigo, args.User.RepClave, db))
                    {
                        return BadRequest("No existe este usuario en los usuarios del sistema");
                    }

                    var suscriptor = db.ReplicacionesSuscriptores.Where(x => x.RepSuscriptor.ToString() == args.User.Suscriptor).FirstOrDefault();

                    var repId = 1;

                    if (suscriptor != null)
                    {
                        repId = suscriptor.RepID;
                    }
                    else
                    {
                        return BadRequest("El suscriptor no es valido");
                    }

                    var rsbSecuencia = (db.ReplicacionesSuscriptoresBaseDatos.Select(x => new { x.RepSuscriptor, x.RepID, x.rsbSecuencia }).Where(x => x.RepSuscriptor.ToString() == args.User.Suscriptor && x.RepID == repId).Max(x => (int?)x.rsbSecuencia) ?? 0) + 1;

                    Guid.TryParse(args.User.Suscriptor, out Guid rowguid);

                    var reg = new ReplicacionesSuscriptoresBaseDatos
                    {
                        RepID = repId,
                        RepSuscriptor = rowguid,
                        RepFecha = DateTime.Now,
                        RepFechaActualizacion = DateTime.Now,
                        RepBaseDatos = Convert.FromBase64String(args.Base64DataBase),
                        rsbSecuencia = rsbSecuencia,
                        UsuIniciosesion = args.User.RepCodigo
                    };

                    db.ReplicacionesSuscriptoresBaseDatos.Add(reg);
                    db.SaveChanges();

                    return Ok();
                }
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [Route("api/ReplicacionesSuscriptores/CambiarContraseñaUsuario")]
        [HttpPost]
        public IActionResult CambiarContraseñaUsuario([FromBody]CambiarContrasenaArgs args)
        {
            var writeLog = false;

            if (string.IsNullOrWhiteSpace(args.OldPass))
            {
                return BadRequest("La contraseña actual no puede estar vacia");
            }

            if (string.IsNullOrWhiteSpace(args.NewPass))
            {
                return BadRequest("La nueva contraseña no puede estar vacia");
            }

            if (string.IsNullOrWhiteSpace(args.Suscriptor))
            {
                return BadRequest("El suscriptor no es valido");
            }

            try
            {
                using (var db = new MBContext())
                {
                    writeLog = db.Parametros.Where(x => x.ParReferencia.Trim().ToUpper() == "WRITELOG" && x.ParValor.Trim() == "1").FirstOrDefault() != null;

                    UsuarioSistema usuarioSistema = null;
                    Representantes representante = null;

                    var usuarios = db.UsuarioSistema.Where(x => x.UsuInicioSesion.Trim().ToUpper() == args.RepCodigo.Trim().ToUpper()).ToList();

                    foreach (var user in usuarios)
                    {
                        if (user.UsuInicioSesion.Trim().ToUpper() == args.RepCodigo.Trim().ToUpper() && user.UsuClave.Trim().ToUpper() == Functions.StringToMd5(args.OldPass).ToUpper())
                        {
                            usuarioSistema = user;
                            break;
                        }
                    }

                    if (usuarioSistema == null)
                    {
                        return BadRequest("La contraseña actual es incorrecta");
                    }

                    var representantes = db.Representantes.Where(x => x.RepCodigo.Trim().ToUpper() == args.RepCodigo.Trim().ToUpper()).ToList();

                    foreach (var rep in representantes)
                    {
                        if (rep.RepCodigo.Trim().ToUpper() == args.RepCodigo.Trim().ToUpper() && rep.RepClave.Trim().ToUpper() == args.OldPass.Trim().ToUpper())
                        {
                            representante = rep;
                            break;
                        }
                    }

                    if (representante == null)
                    {
                        return BadRequest("La contraseña actual es incorrecta");
                    }

                    var suscriptor = db.ReplicacionesSuscriptores.Where(x => x.RepSuscriptor.ToString().ToUpper() == args.Suscriptor.Trim().ToUpper() && x.UsuInicioSesion.Trim().ToUpper() == args.RepCodigo.Trim().ToUpper()).FirstOrDefault();

                    if (suscriptor == null)
                    {
                        return BadRequest("El suscriptor no existe");
                    }

                    if (suscriptor.RsuTipo != 1)
                    {
                        return BadRequest("Este suscriptor es de prueba y no puede realizar cambios");
                    }

                    usuarioSistema.UsuClave = Functions.StringToMd5(args.NewPass);
                    usuarioSistema.UsuInicioSesion = args.RepCodigo;

                    representante.RepClave = args.NewPass;
                    representante.UsuInicioSesion = args.RepCodigo;

                    db.SaveChanges();

                    return Ok();
                }
            }
            catch (Exception e)
            {
                string msgError = e.Message;

                if (writeLog)
                {
                    if (e.InnerException != null)
                    {
                        msgError += Environment.NewLine + "InnerException: " + e.InnerException.Message;
                    }

                    Functions.WriteLog(args.RepCodigo, "CambiarContraseñaRepresentante() - Error procesando - RepCodigo: " + args.RepCodigo + ". Mensaje: " + msgError);
                }

                return BadRequest(e.Message);
            }
        }

        [HttpPost]
        [Route("api/ReplicacionesSuscriptores/PresupuestosCargarOnline")]
        public IActionResult PresupuestosCargarOnline([FromBody]PresupuestosOnlineArgs args)
        {
            var writeLog = false;
            var result = new List<PresupuestosResult>();
            try
            {
                if (string.IsNullOrEmpty(args.PreTipo))
                {
                    return BadRequest("El tipo de presupuesto no puede estar vacio!");
                }

                if (string.IsNullOrEmpty(args.PreAnio))
                {
                    return BadRequest("El año no puede estar vacio!");
                }
                if (string.IsNullOrEmpty(args.PreMes))
                {
                    return BadRequest("El mes no puede estar vacio!");
                }
                using (MBContext db = new MBContext())
                {
                    writeLog = db.Parametros.Where(x => x.ParReferencia.Trim().ToUpper() == "WRITELOG" && x.ParValor.Trim() == "1").FirstOrDefault() != null;

                    if (!UsuarioSistema.Exists(args.User.RepCodigo, args.User.RepClave, db))
                    {
                        return BadRequest("Usuario o contraseña invalidos");
                    }

                    SqlConnection conn = (SqlConnection)(db.Database.GetDbConnection());
                    SqlCommand command = new SqlCommand("sp_wsPresupuestosCargarOnline", conn)
                    {
                        CommandTimeout = 300
                    };
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandTimeout = 0;
                    command.Parameters.Add(new SqlParameter("@RepCodigo", args.User.RepCodigo));
                    command.Parameters.Add(new SqlParameter("@CliID", args.Cliid));
                    command.Parameters.Add(new SqlParameter("@PreTipo", args.PreTipo));
                    command.Parameters.Add(new SqlParameter("@PreAnio", args.PreAnio));
                    command.Parameters.Add(new SqlParameter("@PreMes", args.PreMes));

                    var adapter = new SqlDataAdapter(command);

                    var dataSet = new DataSet();

                    adapter.Fill(dataSet);

                    if (dataSet.Tables.Count > 0)
                    {
                        var iterator = dataSet.Tables[0].Rows.GetEnumerator();

                        while (iterator.MoveNext())
                        {
                            var current = (DataRow)iterator.Current;

                            var presu = new PresupuestosResult();

                            presu.PrePresupuesto = decimal.Parse(current["PrePresupuesto"].ToString());
                            presu.PreEjecutado = decimal.Parse(current["PreEjecutado"].ToString());
                            presu.Descripcion = current["PreReferencia"].ToString();

                            result.Add(presu);
                        }

                        return Ok(result);
                    }
                    else
                    {
                        return BadRequest("No existen datos");
                    }

                }

            }
            catch (Exception e)
            {
                string msgError = e.Message;

                if (writeLog)
                {
                    if (e.InnerException != null)
                    {
                        msgError += Environment.NewLine + "InnerException: " + e.InnerException.Message;
                    }

                    Functions.WriteLog(args.User.RepCodigo, $"PresupuestosCargarOnline() - Error Cargando presupuesto. Usuario: {args.User.RepCodigo}, Mensaje: {msgError}");
                }
                return BadRequest(e.Message);
            }
        }

        [HttpPost]
        [Route("api/ReplicacionesSuscriptores/GetClientesUrl")]
        public IActionResult GetClientesUrl([FromBody]GetClientesUrlArgs args)
        {
            try
            {
                if (args == null || string.IsNullOrWhiteSpace(args.Key))
                {
                    throw new Exception("El key no puede estar vacio");
                }

                using (var db = new MBContext())
                {

                    var key = db.ClientesReplicacionesUrl.Where(x => x.CliUrlKey.Trim() == args.Key.Trim()).FirstOrDefault();

                    if (key == null)
                    {
                        throw new Exception("Key invalido");
                    }

                    return Ok(key.CliUrl);

                }
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpPost]
        [Route("api/ReplicacionesSuscriptores/GetClientesVersion")]
        public IActionResult GetClientesVersion([FromBody]GetClientesUrlArgs args)
        {
            try
            {
                if (args == null || string.IsNullOrWhiteSpace(args.Key))
                {
                    throw new Exception("El key no puede estar vacio");
                }

                using (var db = new MBContext())
                {

                    var key = db.ClientesReplicacionesUrl.Where(x => x.CliUrlKey.Trim() == args.Key.Trim()).FirstOrDefault();

                    if (key == null)
                    {
                        throw new Exception("Key invalido");
                    }

                    return Ok(key.CliReplicacionVersion);

                }
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        /// <summary>
        /// 0 = General, 1 = Clientes
        /// 1 = Tipos de presupuestos
        /// 2 = Lista de Años, segun el tipo de presupuesto
        /// 3 = Lista de meses que tiene este presupuesto
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        [Route("api/ReplicacionesSuscriptores/PresupuestosCombos")]
        [HttpPost]
        public IActionResult PresupuestosCombos([FromBody]PresupuestosCombosArgs args)
        {
            var writeLog = false;
            var result = new List<PresupuestosCombosResult>();
            using (MBContext db = new MBContext())
            {
                writeLog = db.Parametros.Where(x => x.ParReferencia.Trim().ToUpper() == "WRITELOG" && x.ParValor.Trim() == "1").FirstOrDefault() != null;

                if (!UsuarioSistema.Exists(args.User.RepCodigo, args.User.RepClave, db))
                {
                    return BadRequest("Usuario o contraseña invalidos");
                }

                SqlConnection conn = (SqlConnection)(db.Database.GetDbConnection());
                SqlCommand command = new SqlCommand("sp_wsPresupuestosCargarComboOnline", conn)
                {
                    CommandTimeout = 300
                };
                command.CommandType = CommandType.StoredProcedure;
                command.CommandTimeout = 0;
                command.Parameters.Add(new SqlParameter("@Tipo", args.Tipo));
                command.Parameters.Add(new SqlParameter("@Campo", args.Campo));
                command.Parameters.Add(new SqlParameter("@RepCodigo", args.User.RepCodigo));
                command.Parameters.Add(new SqlParameter("@PreTipo", args.PreTipo));
                command.Parameters.Add(new SqlParameter("@PreAnio", args.PreAnio));


                var adapter = new SqlDataAdapter(command);

                var dataSet = new DataSet();

                adapter.Fill(dataSet);

                if (dataSet.Tables.Count > 0)
                {
                    var iterator = dataSet.Tables[0].Rows.GetEnumerator();

                    while (iterator.MoveNext())
                    {
                        var current = (DataRow)iterator.Current;

                        var combo = new PresupuestosCombosResult();
                        if (args.Campo == 1)
                        {
                            combo.CodigoUso = current["CodigoUso"].ToString();
                            combo.Descripcion = current["Descripcion"].ToString();
                        }
                        else if (args.Campo == 2)
                        {
                            combo.Value = current["PreAnio"].ToString();
                        }
                        else if (args.Campo == 3)
                        {
                            combo.CodigoUso = current["CodigoUso"].ToString();
                            combo.Descripcion = current["Descripcion"].ToString();
                            combo.CodigoGrupo = current["CodigoGrupo"].ToString();
                        }

                        result.Add(combo);
                    }

                    return Ok(result);
                }
                else
                {
                    return BadRequest("No existen datos");
                }


            }

        }
    }
}
