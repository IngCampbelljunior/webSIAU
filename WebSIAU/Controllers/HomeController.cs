﻿using LiloSoft.DataBase;
using LiloSoft.DataBase.ConectaDB;
using LiloSoft.Siesa.Interfaz;
using LiloSoft.Siesa.Interfaz.Controllers;
using LiloSoft.Types.Data;
using LiloSoft.Web.ProveedorWeb;
using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using WebSIAU.Models;
using SIS.EsculapioWeb.HistoriaClinica.Models.PruebaEsculapio;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Web.Mvc;

namespace WebSIAU.Controllers
{
    public class HomeController : BaseInterfazController
    {

        public string NomEmpresa;

        public string userId;
        public string IpServidor;
        public string Puerto;
        public string BaseDatos;
        public string Usuario;
        public string Clave;
        public string Folderarchivo;

        public ActionResult Index()
        {
            var model = new EsculapioDetCuentaInputModel();
            model.Empresa = Empresa;

            //-- Obtener Empresa por Ip de Conexion  ==  IPAddressClient; //Request.UserHostAddress; // 

            string userId = (string)Session["userID"];
            if (string.IsNullOrEmpty(userId))
            {
                Session["CodEmpresa"] = "";
                Session["IpConexion"] = "";
                Session["IpPuerto"] = "";
                Session["BaseDatos"] = "";
                Session["userconexion"] = "";
                Session["passconexion"] = "";
                Session["userRole"] = "";
                Session["UserName"] = "";
                Session["NameCompany"] = "";
                Get_EmpresasWeb();
            }

            if (userId == null)
            {
                return RedirectToAction("LogIn");
            }
            else
            {
                return RedirectToAction("ConsultarSolicitudes");
            }
        }

        //.... LOGIN
        #region  LOGIN USUARIOS
        // Login en view partial
        public ActionResult LogIn()
        {

            var cnxEmpresa = Session["CodEmpresa"]; //System.Web.HttpContext.Current.Session["Empresa"];
            //ViewBag.CodEmpresa = cnxEmpresa;
            ViewBag.MessageError = "";
            return View();
        }

        public ActionResult RegistrarInvestigador()
        {
            var codigoEmp = (string)Session["CodEmpresa"];
            var dataBase = new ConsultasEsculapioDB(SqlDbMysql);
            List<Entidades> result = dataBase.GetAseguradoras(codigoEmp);
            ViewBag.aseguradora = new SelectList(result, "nitentidad", "nombre_entidad");
            return View();
        }

        public ActionResult ConsultarSolicitudes()
        {
            var codigoEmp = (string)Session["CodEmpresa"];
            var dataBase = new ConsultasEsculapioDB(SqlDbMysql);
            List<Entidades> result = dataBase.GetAseguradoras(codigoEmp);
            ViewBag.aseguradora = new SelectList(result, "nitentidad", "nombre_entidad");
            return View();
        }

        public ActionResult DatosSolicitud()
        {
            var codigoEmp = (string)Session["CodEmpresa"];
            var dataBase = new ConsultasEsculapioDB(SqlDbMysql);
            List<Entidades> result = dataBase.GetAseguradoras(codigoEmp);
            ViewBag.aseguradora = new SelectList(result, "nitentidad", "nombre_entidad");
            return View();
        }

        public ActionResult ConsultarCasosPaciente()
        {
            var codigoEmp = (string)Session["CodEmpresa"];
            var dataBase = new ConsultasEsculapioDB(SqlDbMysql);
            List<Entidades> result = dataBase.GetAseguradoras(codigoEmp);
            ViewBag.aseguradora = new SelectList(result, "nitentidad", "nombre_entidad");
            return View();
        }
        //[HttpPost]
        //public ActionResult LovDatosSolicitud()
        //{
        //    var codigoEmp = (string)Session["CodEmpresa"];
        //    var dataBase = new ConsultasEsculapioDB(SqlDbMysql);
        //    List<solicitud_doc_siau> lstInvestig = dataBase.GetSolicitudDocSiau(codigoEmp);
        //    return Json(JsonConvert.SerializeObject(lstInvestig), JsonRequestBehavior.AllowGet);
        //}


        [HttpPost]
        public ActionResult GetDatosPaciente(string CodEmpresa)
        {

            List<SelectListItem> li = new List<SelectListItem>();
            var codigoEmp = (string)Session["CodEmpresa"];
            var dataBase = new ConsultasEsculapioDB(SqlDbMysql);
            List<ConsultaSolicitudPendiente> result = dataBase.GetSolicitudPendiente(codigoEmp);

            List<ConsultaSolicitudPendiente> model = new List<ConsultaSolicitudPendiente>();
            foreach (ConsultaSolicitudPendiente i in result)
            {
                ConsultaSolicitudPendiente docEVM = new ConsultaSolicitudPendiente()
                {
                    TipoDocumento = i.TipoDocumento,
                    NoDocumento = i.NoDocumento,
                    Nombre1 = i.Nombre1,
                    Nombre2 = i.Nombre2,
                    Apellido1 = i.Apellido1,
                    Apellido2 = i.Apellido2,
                    fecha_solicitud = i.fecha_solicitud

                };
                model.Add(docEVM);
            }
            return Json(JsonConvert.SerializeObject(model), JsonRequestBehavior.AllowGet);
            //return View(model);
        }

        [HttpPost]
        public ActionResult DatosSolicitud(string nombre1_solicita, string nombre2_solicita, string apellido1_solicita, string apellido2_solicita, string tipo_doc_solicita, string num_doc_solicta, string exp_doc_solicita, string parentesco_solicita, string NoIdentificacion, string tel_paciente, string motivo_solicitud, string email_paciente, string caso, string envioalcorreo, Fecha fechaIngreso, Fecha fechaEgreso, string hist_clinica, string imagen_diag, string lectura_rx, string laboratorio, string certificado, string furips, string num_folio, Fecha fecha_hora_entrega)
        {

            ObtenerConexion();
            string CodEmpre = (string)Session["CodEmpresa"];
            var resultado = string.Empty;
            var dataDb = new ConsultasEsculapioDB(SqlDbMysql);
            var sql = "Insert_solicitudDocSiau";

            List<Parametro> param = new List<Parametro>();
            param.AddParametro("CodEmpre", CodEmpre);
            param.AddParametro("nombre1_solicita", nombre1_solicita);
            param.AddParametro("nombre2_solicita", nombre2_solicita);
            param.AddParametro("apellido1_solicita", apellido1_solicita);
            param.AddParametro("apellido2_solicita", apellido2_solicita);
            param.AddParametro("tipo_doc_solicita", tipo_doc_solicita);
            param.AddParametro("num_doc_solicta", num_doc_solicta);
            param.AddParametro("exp_doc_solicita", exp_doc_solicita);
            param.AddParametro("parentesco_solicita", parentesco_solicita);
            param.AddParametro("NoIdentificacion", NoIdentificacion);
            param.AddParametro("tel_paciente", tel_paciente);
            param.AddParametro("motivo_solicitud", motivo_solicitud);
            param.AddParametro("email_paciente", email_paciente);
            param.AddParametro("envioalcorreo", envioalcorreo);
            param.AddParametro("caso", Convert.ToDecimal(caso));
            param.AddParametro("fechaIngreso", fechaIngreso);
            param.AddParametro("fechaEgreso", fechaEgreso);
            param.AddParametro("hist_clinica", hist_clinica);
            param.AddParametro("imagen_diag", imagen_diag);
            param.AddParametro("lectura_rx", lectura_rx);
            param.AddParametro("laboratorio", laboratorio);
            param.AddParametro("certificado", certificado);
            param.AddParametro("furips", furips);
            param.AddParametro("num_folio", num_folio);
            param.AddParametro("fecha_hora_entrega", fecha_hora_entrega);

            if (!SqlDbMysql.EjecutarComando(sql, true, param.ToArray()))
            {
                throw SqlDbMysql.UltimaExcepcion;
            }

            resultado = "OK";
            return Json(JsonConvert.SerializeObject(resultado), JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult GetCasosPaciente(string criterio)
        {
           
            List<SelectListItem> li = new List<SelectListItem>();
            var dataBase = new ConsultasEsculapioDB(SqlDbMysql);
            var codigoEmp = (string)Session["CodEmpresa"];

            List<ConsultaCasosPaciente> result = dataBase.GetCasosPaciente(codigoEmp,criterio);

            List<ConsultaCasosPaciente> model = new List<ConsultaCasosPaciente>();
            foreach (ConsultaCasosPaciente i in result)
            {
                ConsultaCasosPaciente docEVM = new ConsultaCasosPaciente()
                {
                    Nocuenta = i.Nocuenta,
                    NoAdmision = i.NoAdmision,
                    fechaIngreso = i.fechaIngreso,
                    horaingreso = i.horaingreso,
                    Servicio = i.Servicio,
                    estado = i.estado,
                    fechaegreso = i.fechaegreso,
                    horaEgreso = i.horaEgreso,
                    EstAdm = i.EstAdm,
                    no_Autorizacion = i.no_Autorizacion,
                    autorizado_Por = i.autorizado_Por
                };
                model.Add(docEVM);
            }
            return Json(JsonConvert.SerializeObject(model), JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult ViewFormato(string NoCuenta)
        {
            var archivo = Path.Combine(Server.MapPath("~/Documentos/" + NoCuenta + ".pdf"));
            byte[] FileBytes = null;
            string bitToBase64 = string.Empty;

            if (System.IO.File.Exists(archivo))
            {
                FileBytes = System.IO.File.ReadAllBytes(archivo);
                bitToBase64 = Convert.ToBase64String(FileBytes);
            }
            else
            {
                bitToBase64 = "";
            }
            return Json(bitToBase64, JsonRequestBehavior.AllowGet);
        }
        
        //[HttpPost]
        //public ActionResult SaveImagenTemp(string datauri)
        //{

        //    string urlArchivo = string.Empty;

        //    string pathTest = Path.Combine(Server.MapPath("~/Documentos"), "testfirm.jpeg");
        //    string pathImg = Path.Combine(Server.MapPath("~/Documentos"), "firma_output.jpeg");

        //    bool FicheroBloqueado = false;
        //    Stream retVal = null;
        //    var swfTest = 0;

        //    try
        //    {
        //        FileStream fs = System.IO.File.OpenWrite(pathTest);
        //        fs.Close();
        //    }
        //    catch (Exception ex)
        //    {
        //        FicheroBloqueado = true;
        //    }

        //    if (FicheroBloqueado)
        //    {
        //        Folderarchivo = pathImg;
        //    }
        //    else
        //    {
        //        swfTest = 1;
        //        Folderarchivo = pathTest;
        //    }

        //    //File.WriteAllBytes(Folderarchivo, bytes);      
        //    if (System.IO.File.Exists(Folderarchivo))
        //    {
        //        byte[] fileData = System.IO.File.ReadAllBytes(Folderarchivo);
        //        retVal = new MemoryStream(fileData);
        //        retVal.Close();
        //        retVal.Dispose();

        //        FileStream s = new FileStream(Folderarchivo, FileMode.Open);
        //        s.Close();
        //        s.Dispose();

        //        System.IO.File.Delete(Folderarchivo);
        //    }


        //    byte[] bitmapData = null;
        //    string bitToBase64 = string.Empty;
        //    if (datauri.Length > 0)
        //    {
        //        bitmapData = new byte[datauri.Length];
        //        bitmapData = Convert.FromBase64String(FixBases64ForImage(datauri));
        //        bitToBase64 = Convert.ToBase64String(bitmapData);

        //        using (FileStream fs = new FileStream(Folderarchivo, FileMode.Create))
        //        {
        //            using (BinaryWriter bw = new BinaryWriter(fs))
        //            {
        //                byte[] data = Convert.FromBase64String(datauri);
        //                bw.Write(data);
        //                bw.Close();
        //                bw.Dispose();
        //            }
        //        }

        //        if (swfTest == 1)
        //        {
        //            System.IO.File.Copy(Folderarchivo, pathImg, true);
        //        }
        //    }
        //    return Json(bitToBase64, JsonRequestBehavior.AllowGet);
        //}

        //[HttpPost]
        //public void GeneraFormato(string Empresa, string nocaso, Fecha FechaHoy, string Usuario)
        //{
        //    ObtenerConexion();
        //    var Investigador = string.Empty;
        //    var DocInvestigador = string.Empty;
        //    var AgenciaInvest = string.Empty;
        //    var dataDb = new ConsultasEsculapioDB(SqlDbMysql);
        //    Image imgFirmaFun = null;
        //    List<investigador_reporte> datosFrm = dataDb.GetDatoFormatoInvestigador(Empresa, Convert.ToDecimal(nocaso), FechaHoy, Usuario);

        //    byte[] firmaUsu = null;

        //    string[] FechaIng = null;
        //    string[] FechaAcc = null;
        //    string[] FechaElabora = null;

        //    string CedFuncionario = string.Empty;
        //    string NomFuncionario = string.Empty;

        //    // Replace Texto <<>> por Campos
        //    if (datosFrm.Count == 0)
        //    {
        //        var result = "0";
        //        return;
        //    }

        //    var formNoCaso = nocaso;
        //    var tipoFormato = "INV";

        //    string pathImg = Path.Combine(Server.MapPath("~/Documentos"), "firma_output.jpeg");

        //    Spire.Doc.Document doc = new Spire.Doc.Document();
        //    Spire.Doc.Document filePDF = new Spire.Doc.Document();
        //    doc.LoadFromFile(Server.MapPath("~/Documentos/REPOR_INV.docx"));



        //    foreach (var item in datosFrm) //investigador_reporte
        //    {
        //        var Paciente = item.paciente;
        //        string nullFechaIng = item.FechaIngreso == "" ? item.FechaIngreso : item.FechaIngreso;
        //        var nullHoraIng = item.HoraIngreso == "" ? item.HoraIngreso : item.HoraIngreso;
        //        string nullFechaAcc = item.Fecha_acc == "" ? item.Fecha_acc : item.Fecha_acc;
        //        var nullHoraAcc = item.Hora_acc == "" ? item.Hora_acc : item.Hora_acc;
        //        string nullFechaElabora = item.Fecha_elaboracion == "" ? item.Fecha_elaboracion : item.Fecha_elaboracion;
        //        Investigador = item.investigadores;
        //        DocInvestigador = item.Doc_Investigador;
        //        AgenciaInvest = item.AgenciaInvest;

        //        if (nullFechaIng != "" && nullFechaIng != null)
        //        {
        //            FechaIng = nullFechaIng.Split('/');
        //        }

        //        if (nullFechaAcc != "" && nullFechaAcc != null)
        //        {
        //            FechaAcc = nullFechaAcc.Split('/');
        //        }

        //        if (nullFechaElabora != "" && nullFechaElabora != null)
        //        {
        //            FechaElabora = nullFechaElabora.Split('/');
        //        }

        //        //Convierte los meses a sus nombres según el número
        //        DateTime now = DateTime.Now; //mes de la Fecha actual
        //        DateTime mesIngreso = item.FechaIngreso; //mes de la fecha de ingreso
        //        DateTime mesAccidente = item.Fecha_acc; //mes de la fecha de accidente

        //        doc.Replace("<<HOYDIA>>", FechaElabora[0], true, true);
        //        doc.Replace("<<HOYMES>>", now.ToString("MMMM"), true, true); //mes de la Fecha actual
        //        doc.Replace("<<HOYANNO>>", FechaElabora[2], true, true);
        //        doc.Replace("<<ASEGURA>>", item.nombre_entidad, true, true);
        //        doc.Replace("<<IGDIA>>", FechaIng[0], true, true);
        //        doc.Replace("<<IGMES>>", mesIngreso.ToString("MMMM"), true, true); //mes de la fecha de ingreso
        //        doc.Replace("<<IGANNO>>", FechaIng[2], true, true);
        //        doc.Replace("<<IGHH>>", item.HoraIngreso, true, true);
        //        doc.Replace("<<PACIENTE>>", Paciente, true, true);
        //        doc.Replace("<<TCC>>", item.TipoDocumento, true, true);
        //        doc.Replace("<<NOCC>>", item.NoDocumento, true, true);
        //        doc.Replace("<<CIUCC>>", item.CiudadExp, true, true);
        //        doc.Replace("<<NOCASO>>", item.Caso, true, true);
        //        doc.Replace("<<DIAACC>>", FechaAcc[0], true, true);
        //        doc.Replace("<<MESACC>>", mesAccidente.ToString("MMMM"), true, true); //mes de la fecha de accidente
        //        doc.Replace("<<ANNOACC>>", FechaAcc[2], true, true);
        //        doc.Replace("<<HHACC>>", item.Hora_acc, true, true);
        //        doc.Replace("<<DX>>", item.Diagnostico, true, true);
        //        doc.Replace("<<RELATO>>", item.Relato, true, true);
        //        doc.Replace("<<INVESTIGADOR>>", Investigador, true, true);
        //        doc.Replace("<<NODOCUMENTO>>", item.Doc_Investigador, true, true);
        //        doc.Replace("<<EMPRESAINVEST>>", AgenciaInvest, true, true);
        //        doc.Replace("<<FECHA>>", item.Fecha_elaboracion, true, true);
        //        //doc.Replace("<<FIRMAUSUARIO>>", item.FirmaUsuario, true, true);

        //        if (nullFechaElabora != "" && nullFechaElabora != null)
        //        {
        //            doc.Replace("<<HOYANNO>>", FechaElabora[0], true, true);
        //            doc.Replace("<<HOYMES>>", now.ToString(), true, true);
        //            doc.Replace("<<HOYDIA>>", FechaElabora[2], true, true);
        //        }
        //        else
        //        {
        //            doc.Replace("<<HOYANNO>>", "", true, true);
        //            doc.Replace("<<HOYMES>>", "", true, true);
        //            doc.Replace("<<HOYDIA>>", "", true, true);
        //        }

        //        if (nullFechaIng != "" && nullFechaIng != null)
        //        {
        //            doc.Replace("<<IGANNO>>", FechaAcc[0], true, true);
        //            doc.Replace("<<IGMES>>", FechaAcc[1], true, true);
        //            doc.Replace("<<IGDIA>>", FechaAcc[2], true, true);
        //        }
        //        else
        //        {
        //            doc.Replace("<<IGANNO>>", "", true, true);
        //            doc.Replace("<<IGMES>>", "", true, true);
        //            doc.Replace("<<IGDIA>>", "", true, true);
        //        }

        //        if (nullFechaAcc != "" && nullFechaAcc != null)
        //        {
        //            doc.Replace("<<ANNOACC>>", FechaAcc[0], true, true);
        //            doc.Replace("<<MESACC>>", FechaAcc[1], true, true);
        //            doc.Replace("<<DIAACC>>", FechaAcc[2], true, true);
        //        }
        //        else
        //        {
        //            doc.Replace("<<ANNOACC>>", "", true, true);
        //            doc.Replace("<<MESACC>>", "", true, true);
        //            doc.Replace("<<DIAACC>>", "", true, true);
        //        }


        //        // Firma Investigador...

        //        Spire.Doc.Section sec = doc.Sections[0];
        //        int index = 0;
        //        TextRange range = null;
        //        TextSelection[] selections = doc.FindAllString("FIRMAFUN", true, true);

        //        if (doc.TextBoxes.Count > 0)
        //        {
        //            TextBox tb = doc.TextBoxes[0];
        //            tb.Format.FillEfects.Type = BackgroundType.Picture;
        //            tb.Format.FillEfects.Picture = Image.FromFile(Server.MapPath("~/Documentos/firma_output.jpeg"));
        //        }

        //        firmaUsu = item.FirmaUsuario;

        //        string imagenBase64 = Convert.ToBase64String(firmaUsu);
        //        imgFirmaFun = Base64ToImage(imagenBase64);
        //        System.IO.File.WriteAllBytes(Server.MapPath("~/Documentos/firma_output.jpeg"), Convert.FromBase64String(imagenBase64));

        //        index = 0;
        //        range = null;
        //        selections = doc.FindAllString("FIRMAUSUARIO", true, true);

        //        if (doc.TextBoxes.Count > 0)
        //        {
        //            TextBox tb = doc.TextBoxes[1];
        //            tb.Format.FillEfects.Type = BackgroundType.Picture;
        //            tb.Format.FillEfects.Picture = Image.FromFile(Server.MapPath("~/Documentos/firma_output.jpeg"));
        //        }

        //    }
        //    // Convertir a PDF  ~
        //    doc.SaveToFile(Server.MapPath("~/Documentos/" + formNoCaso + ".docx"), FileFormat.Docx);

        //    filePDF.LoadFromFile(Server.MapPath("~/Documentos/" + formNoCaso + ".docx"));
        //    filePDF.SaveToFile(Server.MapPath("~/Documentos/" + formNoCaso + ".pdf"), FileFormat.PDF);
        //    System.IO.File.Delete(Server.MapPath("~/Documentos/" + formNoCaso + ".docx"));

        //    Console.WriteLine("Genero PDF");

        //}


        // Visuliazacion en PDF de Reporte

        public string FixBases64ForImage(string image)
        {
            System.Text.StringBuilder sbText = new System.Text.StringBuilder(image, image.Length);

            sbText.Replace("\r\n", String.Empty);
            sbText.Replace(" ", String.Empty);
            sbText.Replace("\"", String.Empty);

            return sbText.ToString();
        }

        private Image Base64ToImage(string base64String)
        {
            byte[] imageBytes = Convert.FromBase64String(base64String);
            using (var ms = new MemoryStream(imageBytes, 0, imageBytes.Length))
            {
                Image image = Image.FromStream(ms, true);
                return image;
            }

        }

        [HttpPost]
        public ActionResult RegistrarInvestigador(string Empresa, string nodocumento, string tipodocumento, string ciudadexp, string nombres, string apellidos, string correoinvest, string agenciainvest, string aseguradora)
        {
            ObtenerConexion();
            string CodEmpre = (string)Session["CodEmpresa"];
            var resultado = string.Empty;
            var dataDb = new ConsultasEsculapioDB(SqlDbMysql);
            var sql = "Insert_RegistrarInvestigador";

            List<Parametro> param = new List<Parametro>();
            param.AddParametro("Empresa", CodEmpre);
            param.AddParametro("nodocumento", nodocumento);
            param.AddParametro("tipodocumento", tipodocumento);
            param.AddParametro("ciudadexp", ciudadexp);
            param.AddParametro("nombres", nombres);
            param.AddParametro("apellidos", apellidos);
            param.AddParametro("correoinves", correoinvest);
            param.AddParametro("agenciainves", agenciainvest);
            param.AddParametro("aseguradora", aseguradora);

            if (!SqlDbMysql.EjecutarComando(sql, true, param.ToArray()))
            {
                throw SqlDbMysql.UltimaExcepcion;
            }

            resultado = "OK";
            return Json(JsonConvert.SerializeObject(resultado), JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult Authorise(UsuariosModels user, FormCollection formcollection)
        {
            string CodigoEmp = Request.Form["hfCodigoEmp"].ToString();

            GetEmpresaEsculapio(CodigoEmp.Substring(0, 2)); //"01"
            user.usuario = Request.Form["usuario"].ToString();
            user.PassWordUsu = Request.Form["PassWordUsu"].ToString();
            try
            {
                DB dbValida = ObtenerConexion();
                if (!dbValida.AbrirConexion())
                {
                    ViewBag.MessageError = "Invalido Usuario y/o Password";
                    return View("LogIn", user);
                }
                else if (ValidaUsuario(user.usuario, user.PassWordUsu))
                {

                    Session["userID"] = user.usuario;
                    return RedirectToAction("ConsultarSolicitudes");
                }
                else
                {
                    ViewBag.MessageError = "Usuario no tiene perfil para este modulo";
                    return View("LogIn", user);
                }
            }
            catch (Exception exception)
            {
                throw exception;
            }
        }

        private bool ValidaUsuario(string usuario, string passusu)
        {
            bool bool_resp = false;
            string CodigoEmpresa = Session["CodEmpresa"].ToString();
            DataTable dtUsuario = new DataTable();
            string IdUsuario = string.Empty;
            string IdRole = string.Empty;
            string NomUsua = string.Empty;
            ObtenerConexion();
            if (CodigoEmpresa != "C30")
            {
                MySqlConnection DBConnect = new MySqlConnection("server=" + IpServidor + ";user id=" + usuario + "; Password=" + passusu + ";port=" + Puerto + ";database=" + BaseDatos + ";");
                string sql = "SELECT a.usuario,a.estado,b.Nombre FROM usuarios_repor_invest a Inner Join usuarios b On a.usuario=b.usuario ";
                MySqlCommand comm = new MySqlCommand(sql + "WHERE a.usuario='" + usuario + "' And a.Empresa='" + CodigoEmpresa + "' AND a.Estado='A'", DBConnect);
                DBConnect.Open();
                dtUsuario.Load(comm.ExecuteReader());
                DBConnect.Close();
                List<ConsultaUsuariosAdv> lstUsuario = dtUsuario.AsEnumerable().Select(m => new ConsultaUsuariosAdv()
                {
                    Role = m.Field<string>("Role"),
                    Nombre = m.Field<string>("Nombre"),
                    Usuario = m.Field<string>("Usuario")
                }).ToList();

                foreach (ConsultaUsuariosAdv item in lstUsuario)
                {
                    IdUsuario = item.Usuario;
                    IdRole = item.Role;
                    NomUsua = item.Nombre;
                }
            }
            else
            {
                foreach (ConsultaUsuariosAdv item in (new ConsultasEsculapioDB(SqlDbMysql).GetUsuarioReporInvestigacion(CodigoEmpresa, usuario)))
                {
                    IdUsuario = item.Usuario;
                    IdRole = item.Role;
                    NomUsua = item.Nombre;
                }
            }
            if (IdUsuario != "")
            {
                Session["userRole"] = IdRole;
                Session["UserName"] = NomUsua;
                bool_resp = true;
            }
            return bool_resp;
        }

        public ActionResult LogOut()
        {
            string userId = (string)Session["userID"];
            Session.Abandon();
            Session["userID"] = null;
            return RedirectToAction("Login", "Home");
        }

        ///
        #endregion
        //---------------------------------------
        //// Captura Empresa de Conexion
        [HttpPost]
        public ActionResult Get_EmpresasWeb()
        {
            ConsultaInstitucionDB dataDb = new ConsultaInstitucionDB(SqlDb);
            List<SelectListItem> li = new List<SelectListItem>();
            List<EmpresasWeb> result = dataDb.Get_EmpresasWeb();
            return Json(JsonConvert.SerializeObject(result), JsonRequestBehavior.AllowGet);
        }

        private void GetEmpresaEsculapio(string CodigoEmp)
        {
            ConsultaInstitucionDB dataDb = new ConsultaInstitucionDB(SqlDb);
            List<SelectListItem> li = new List<SelectListItem>();
            foreach (EmpresasWeb item in dataDb.Get_UnaEmpresasWeb(CodigoEmp))
            {
                ViewBag.EsculapioEMP = item.EmpresaEsculapio;
                ViewBag.ipConn = item.Servidor;
                ViewBag.ipport = item.Puerto;
                ViewBag.ipBd = item.BaseDatos;
                Session["NameCompany"] = item.NombreEmpresa;
            }
            Session["CodEmpresa"] = ViewBag.EsculapioEMP;
            Session["IpConexion"] = Request.Form["hfIpConexion"].ToString();
            Session["BaseDatos"] = Request.Form["hfBaseDB"].ToString();
            Session["IpPuerto"] = Request.Form["hfPuerto"].ToString();
            Session["userconexion"] = Request.Form["usuario"].ToString();
            Session["passconexion"] = Request.Form["PassWordUsu"].ToString();
            ObtenerConexion();
        }

        private DB ObtenerConexion()
        {
            IpServidor = (string)Session["IpConexion"];
            Puerto = (string)Session["IpPuerto"];
            BaseDatos = (string)Session["BaseDatos"];
            Usuario = (string)Session["userconexion"];
            Clave = (string)Session["passconexion"];
            SqlDbMysql.Servidor = IpServidor;
            SqlDbMysql.Puerto = Puerto;
            SqlDbMysql.BaseDatos = BaseDatos;
            SqlDbMysql.Usuario = Usuario;
            SqlDbMysql.Clave = Clave;
            DB db = DB.NuevaDB(ManejadorBaseDatos.MySql);
            db.Servidor = SqlDbMysql.Servidor;
            db.BaseDatos = SqlDbMysql.BaseDatos;
            db.Puerto = SqlDbMysql.Puerto;
            db.Usuario = SqlDbMysql.Usuario;
            db.Clave = Clave;
            return db;
        }

    }
}