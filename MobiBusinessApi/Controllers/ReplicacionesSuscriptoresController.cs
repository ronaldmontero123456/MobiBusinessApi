using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MovilBusinessApiCore.Models;
using MovilBusinessApiCore.Models.Internal.Structs;
using MovilBusinessApiCore.Utils;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;

namespace MovilBusinessApiCore.Controllers
{
    public class ReplicacionesSuscriptoresController : ApiController
    {
        [HttpPost]
        [Route("api/ReplicacionesSuscriptores/VerificarSuscriptor")]
        public IHttpActionResult VerificarSuscriptor([FromBody]UsuarioArgs args)
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


                    db.Database.ExecuteSqlCommand("[sp_MDSOFTGeneraCargaInicialSuscriptoresCambios] @RepID, @RepSuscriptor",
                        new SqlParameter[] {
                            new SqlParameter() { ParameterName = "@RepID", SqlDbType = System.Data.SqlDbType.Int, Value = suscriptor.RepID },
                            new SqlParameter() { ParameterName = "@RepSuscriptor", SqlDbType = System.Data.SqlDbType.VarChar, Size = int.MaxValue, Value = suscriptor.RepSuscriptor.ToString()  }});

                    suscriptor.resEstado = 3;
                    db.SaveChanges();

                    var query = string.Format("SELECT COUNT(RepID) AS Value FROM [ReplicacionesSuscriptoresCambios{0}] with(nolock)", suscriptor.RepSuscriptor.ToString());
                    var count = db.Database.SqlQuery<ReplicacionesSuscriptoresCambiosCount>(query).FirstOrDefault();

                    int cambios = 0;
                    if (count != null)
                    {
                        cambios = count.Value;
                    }

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

        private void VerificarLicencia(MBContext db)
        {
            var par = db.Parametros.Where(x => x.ParReferencia.Trim().ToUpper() == "LICENCIA").FirstOrDefault();

            if (par == null)
            {
                throw new Exception("No dispone de licencias disponibles para realizar la carga inicial");
            }

            var value = par.ParValor.Substring(4, 1) + par.ParValor.Substring(10, 1) + par.ParValor.Substring(20, 1) + par.ParValor.Substring(31, 1);

            int.TryParse(value, out int licencias);

            var cantidadSuscriptores = db.ReplicacionesSuscriptores.Count(x => x.RsuTipo == 1);

            if (licencias < cantidadSuscriptores)
            {
                throw new Exception("No dispone de licencias disponibles para realizar la carga inicial");
            }
        }

        [HttpPost]
        [Route("api/ReplicacionesSuscriptores/VerificarSuscriptorLegacy")]
        public IHttpActionResult VerificarSuscriptorLegacy([FromBody]UsuarioArgs args)
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

                    db.Database.CommandTimeout = 5000;

                    db.Database.ExecuteSqlCommand("[sp_MDSOFTGeneraCargaInicialSuscriptoresCambios] @RepID, @RepSuscriptor",
                        new SqlParameter[] {
                            new SqlParameter() { ParameterName = "@RepID", SqlDbType = System.Data.SqlDbType.Int, Value = suscriptor.RepID },
                            new SqlParameter() { ParameterName = "@RepSuscriptor", SqlDbType = System.Data.SqlDbType.VarChar, Size = int.MaxValue, Value = suscriptor.RepSuscriptor.ToString()  }});

                    suscriptor.resEstado = 3;
                    db.SaveChanges();

                    var query = string.Format($"SELECT COUNT(RepID) AS Value FROM [ReplicacionesSuscriptoresCambios] with(nolock) where RepSuscriptor = '{suscriptor.RepSuscriptor.ToString()}'");
                    var count = db.Database.SqlQuery<ReplicacionesSuscriptoresCambiosCount>(query).FirstOrDefault();

                    int cambios = 0;
                    if (count != null)
                    {
                        cambios = count.Value;
                    }

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

        [HttpPost]
        [Route("api/ReplicacionesSuscriptores/ReplicacionesTablasLeer")]
        public IHttpActionResult ReplicacionesTablasLeer([FromBody] ReplicacionesTablasLeerArgs args)
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
        public IHttpActionResult ReplicacionesTablasLeerNew([FromBody] ReplicacionesTablasLeerArgs args)
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
        public IHttpActionResult ExecuteQuery([FromBody] ExecuteQueryArgs args)
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
                    var noValidarSuscriptor = db.Parametros.Where(x => x.ParReferencia.Trim().ToUpper() == "SUSCRIPNOVAL" && x.ParValor.Trim() == "1").FirstOrDefault() != null;

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
                    var existsSuscriptor = true;

                    if (!noValidarSuscriptor)
                    {
                        existsSuscriptor = db.ReplicacionesSuscriptores.Where(x => x.UsuInicioSesion.Trim() == args.User.RepCodigo.Trim() && x.RepSuscriptor.ToString() == args.User.Suscriptor).FirstOrDefault() != null;

                        if (!existsSuscriptor)
                        {
                            if (writeLog)
                            {
                                Functions.WriteLog(args.User.RepCodigo, $"ExecuteQuery(). No existe el suscriptor solicitado en la tabla de ReplicacionesSuscriptores, RepSuscriptor: {args.User.Suscriptor}.");
                            }

                            return BadRequest("No existe el suscriptor solicitado");
                        }

                        var suscriptorn = db.ReplicacionesSuscriptores.Where(x => x.UsuInicioSesion.Trim() == args.User.RepCodigo.Trim() && x.RepSuscriptor.ToString() == args.User.Suscriptor).FirstOrDefault();
                        if (suscriptorn.resEstado == 0)
                        {
                            return BadRequest("Suscriptor Bloqueado.");
                        }
                    }

                    string CurrentTable = "", CurrentScript = "";

                    try
                    {
                        //using (var dbTran = db.Database.BeginTransaction())
                        //{
                        try
                        {
                            db.Database.CommandTimeout = 500;
                            //db.Database.BeginTransaction();
                            foreach (ExecuteQueryValues value in args.Values)
                            {
                                //CurrentTable = value.TableName;
                                //CurrentScript = value.Query;

                                //db.Database.CommandTimeout = 500;

                                //if (!string.IsNullOrWhiteSpace(value.TipoScript) && value.TipoScript.Trim().ToUpper() == "I")
                                //{
                                //    db.Database.ExecuteSqlCommand("[sp_wsExecQueryInsert] @query, @Tabla, @rowguid", new SqlParameter[]
                                //    {
                                //            new SqlParameter(){ ParameterName = "@query", SqlDbType = System.Data.SqlDbType.VarChar, Size = -1, Value = value.Query },
                                //            new SqlParameter(){ ParameterName = "@Tabla", SqlDbType = System.Data.SqlDbType.VarChar, Size = 100, Value = value.TableName },
                                //            new SqlParameter(){ ParameterName = "@rowguid", SqlDbType = System.Data.SqlDbType.VarChar, Size = 100, Value = value.rowguid }
                                //    });
                                //}
                                //else
                                //{
                                //    db.Database.ExecuteSqlCommand("[sp_wsExecQuery] @query", new SqlParameter[] { new SqlParameter() { ParameterName = "@query", SqlDbType = System.Data.SqlDbType.VarChar, Size = -1, Value = value.Query } });
                                //}

                                db.Database.ExecuteSqlCommand("[sp_wsExecQuery] @query", new SqlParameter[] { new SqlParameter() { ParameterName = "@query", SqlDbType = System.Data.SqlDbType.VarChar, Size = -1, Value = value.Query } });
                            }

                            //db.SaveChanges();
                            //db.Database.CurrentTransaction.Commit();
                            // dbTran.Commit();
                        }
                        catch (Exception e)
                        {
                            //if (db.Database.CurrentTransaction != null)
                            //{
                            //    db.Database.CurrentTransaction.Rollback();
                            //}
                            //  dbTran.Rollback();
                            throw e;
                        }
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
        public IHttpActionResult SuscriptoresCambiosLeer([FromBody] ReplicacionesSuscriptoresCambiosLeerArgs args)
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

                    if (!string.IsNullOrWhiteSpace(args.DeleteQuery))
                    {
                        db.Database.ExecuteSqlCommand("[sp_wsExecQuery] @query", new SqlParameter("@query", args.DeleteQuery));
                        Task.Delay(1000).Wait();
                    }

                    var query = string.Format("select top({0}) RscKey, RepID, RscTabla, RscTipTran, RscFechaActualizacion, RscScript, RscTablarowguid, UsuInicioSesion, RepSuscriptor from [ReplicacionesSuscriptoresCambios{1}] with(nolock) WHERE RscTipTran <> 'F' order by RscFechaActualizacion ASC, rscTabla ", args.Limit, args.User.Suscriptor);

                    var list = db.Database.SqlQuery<ReplicacionesSuscriptoresCambios>(query).ToList();

                    if (writeLog && list.Count == 0)
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
        public IHttpActionResult SuscriptoresCambiosLeerV8([FromBody] ReplicacionesSuscriptoresCambiosLeerArgs args)
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

                    if (!string.IsNullOrWhiteSpace(args.DeleteQuery))
                    {
                        db.Database.ExecuteSqlCommand("[sp_wsExecQuery] @query", new SqlParameter("@query", args.DeleteQuery));
                        Task.Delay(1000).Wait();
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
                    
                    var list = db.Database.SqlQuery<ReplicacionesSuscriptoresCambiosV8>(query).ToList();

                    if (writeLog && list.Count == 0)
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
        public IHttpActionResult SuscriptoresCambiosLeerV9([FromBody] ReplicacionesSuscriptoresCambiosLeerArgs args)
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

                    if (!string.IsNullOrWhiteSpace(args.DeleteQuery))
                    {
                        db.Database.ExecuteSqlCommand("[sp_wsExecQuery] @query", new SqlParameter("@query", args.DeleteQuery));
                        Task.Delay(1000).Wait();
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

                    var list = db.Database.SqlQuery<ReplicacionesSuscriptoresCambiosV8>(query).ToList();

                    if (writeLog && list.Count == 0)
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
        public IHttpActionResult SuscriptoresCambiosLeerLegacy([FromBody] ReplicacionesSuscriptoresCambiosLeerArgs args)
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

                    if (!string.IsNullOrWhiteSpace(args.DeleteQuery))
                    {
                        db.Database.ExecuteSqlCommand("[sp_wsExecQuery] @query", new SqlParameter("@query", args.DeleteQuery));

                    }

                    var query = string.Format($"select top({args.Limit}) RscKey, RepID, RscTabla, RscTipTran, RscFechaActualizacion, RscScript, RscTablarowguid, UsuInicioSesion, RepSuscriptor from [ReplicacionesSuscriptoresCambios] with(nolock) where RepSuscriptor = '{args.User.Suscriptor}' and RscTipTran <> 'F' order by RscFechaActualizacion ASC, rscTabla ");

                    var list = db.Database.SqlQuery<ReplicacionesSuscriptoresCambios>(query).ToList();

                    if (writeLog && list.Count == 0)
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
        public IHttpActionResult EmpresasCargar([FromBody] UsuarioArgs args)
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
        public IHttpActionResult ExecUpdate([FromBody]ExecUpdateArgs args)
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

                    DbContextTransaction tran = db.Database.BeginTransaction();

                    try
                    {
                        db.Database.ExecuteSqlCommand("[sp_wsExecQuery] @query", new SqlParameter("@query", args.Query));
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

                        tran.Rollback();
                        tran.Dispose();
                        throw ex;
                    }

                    tran.Commit();
                    tran.Dispose();

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
        public IHttpActionResult RawQuery([FromBody]QueryArgs args)
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

                    SqlConnection connection = db.Database.Connection as SqlConnection;

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
        public IHttpActionResult CargarPresupuestosBySupervisorSync([FromBody]PresupuestosArgs args)
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

                    SqlCommand command = (SqlCommand)db.Database.Connection.CreateCommand();
                    command.CommandText = "sp_wsCargarPresupuestosSync";
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
        public IHttpActionResult CoberturaConsultar([FromBody]CoberturaArgs args)
        {
            string str = null;
            IEnumerator enumerator = null;
            IEnumerator enumerator1 = null;
            System.Data.SqlClient.SqlConnection sqlConnection = null;
            SqlCommand sqlCommand = null;
            SqlDataReader sqlDataReader = null;
            DataTable dataTable = new DataTable();
            CoberturaConsultarStruct count = new CoberturaConsultarStruct();

            try
            {
                try
                {
                    sqlConnection = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["MBConnectionString"].ToString());
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
        public IHttpActionResult TransaccionesImagenesInsertar([FromBody] ImagenesInsertarArgs args)
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

                    DbContextTransaction transaction = null;

                    try
                    {
                        transaction = db.Database.BeginTransaction();

                        foreach (TransaccionesImagenesTablasTemp img in args.Imagenes)
                        {
                            var cmd = db.Database.Connection.CreateCommand() as SqlCommand;

                            cmd.Transaction = (SqlTransaction)transaction.UnderlyingTransaction;
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

                        transaction.Commit();
                        transaction.Dispose();
                        db.Database.Connection.Close();

                    }
                    catch (Exception e)
                    {
                        try
                        {
                            if (transaction != null)
                            {
                                transaction.Rollback();
                                transaction.Dispose();
                                db.Database.Connection.Close();
                            }
                        }
                        catch (Exception) { }
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
        public IHttpActionResult SuscriptoresCambiosLeerImagenes([FromBody] ReplicacionesSuscriptoresCambiosLeerArgs args)
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

                    SqlConnection connection = db.Database.Connection as SqlConnection;

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
        public IHttpActionResult SuscriptoresCambiosLeerImagenesLegacy([FromBody] ReplicacionesSuscriptoresCambiosLeerArgs args)
        {
            return SuscriptoresCambiosLeerImagenes(args);
        }

        [Route("api/ReplicacionesSuscriptores/ImagenesCargar")]
        [HttpPost]
        public IHttpActionResult ImagenesCargar([FromBody] ImagenesCargarArgs args)
        {
            try
            {
                using (MBContext db = new MBContext())
                {
                    if (!UsuarioSistema.Exists(args.User.RepCodigo, args.User.RepClave, db))
                    {
                        return BadRequest("No existe este usuario en los usuarios del sistema");
                    }

                    if (!File.Exists(args.ImagePath))
                    {
                        return BadRequest("La imagen solicitada no existe");
                    }

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
        public IHttpActionResult SubirSqliteDb([FromBody] SubirSqliteDbArgs args)
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
        public IHttpActionResult CambiarContraseñaUsuario([FromBody]CambiarContrasenaArgs args)
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
        public IHttpActionResult PresupuestosCargarOnline([FromBody]PresupuestosOnlineArgs args)
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

                    SqlCommand command = (SqlCommand)db.Database.Connection.CreateCommand();
                    command.CommandText = "sp_wsPresupuestosCargarOnline";
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
        public IHttpActionResult GetClientesUrl([FromBody]GetClientesUrlArgs args)
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
        public IHttpActionResult GetClientesVersion([FromBody]GetClientesUrlArgs args)
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
        public IHttpActionResult PresupuestosCombos([FromBody]PresupuestosCombosArgs args)
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

                SqlCommand command = (SqlCommand)db.Database.Connection.CreateCommand();
                command.CommandText = "sp_wsPresupuestosCargarComboOnline";
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
