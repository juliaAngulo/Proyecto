using CapaVista.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using Newtonsoft.Json;
using RestSharp;
using Microsoft.VisualBasic;
using Proyecto1.Models;
using System.Net.NetworkInformation;
using Microsoft.AspNetCore.Http;

namespace CapaVista.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly LibrosContext _baseDatos;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Menu()
        {
            if(HttpContext.Session.GetString("Usuario") == null){
                return Redirect("Index");
            }
            ViewBag.IdUsuario = HttpContext.Session.GetString("Usuario");
            return View();
        }


        [HttpGet]
        public IActionResult Libros(int id)
        {
            var usuario = HttpContext.Session.GetString("Usuario");
            if (usuario == null)
            {
                return Redirect("Index");
            }
            ViewBag.IdUsuario = usuario;
            // Asignar el valor de id a ViewBag
            ViewBag.Id = id;
            //LINKS DE LOS SERVICIOS:
            string Usuarios = "https://localhost:7159/api/Publicar/ListaLibro";

            RestClient Client = new RestClient(Usuarios);
            string Respuesta = "";

            RestRequest Request = new RestRequest();

            var Response = Client.Get(Request);

            Respuesta = Response.Content;

            List<Publicar> oResultado = JsonConvert.DeserializeObject<List<Publicar>>(Respuesta);

            var Libro = oResultado.FirstOrDefault(u => u.IdPublicar == id); ;

            ViewBag.NombreLibro = Libro.NombreLibro;
            return View();
        }



        [HttpPost]
        public IActionResult Login([FromBody] Log usuario)
        {
            //LINKS DE LOS SERVICIOS:
            string Usuarios = "https://localhost:7159/api/Usuario/ListaUsuarios";
            //
            

            RestClient Client = new RestClient(Usuarios);
            string Respuesta = "";

            RestRequest Request = new RestRequest();

            var Response = Client.Get(Request);

            Respuesta = Response.Content;
            
            List<Usuario> oResultado = JsonConvert.DeserializeObject<List<Usuario>>(Respuesta);

            var UsuarioEncontrado = oResultado.FirstOrDefault(u => u.NombreUsuario == usuario.usuario  && u.Contraseña == usuario.password); ;

            if (UsuarioEncontrado == null)
            {
                return Json("Usuario No encontrado");
            }
            else
            {
                HttpContext.Session.SetString("Usuario", UsuarioEncontrado.IdUsuario.ToString());
                return Json("Ok");
            }
  
        }

        //método que cierrra sesión:
        [HttpGet]
        public IActionResult CerrarSesion(int id)
        {
            HttpContext.Session.Clear();
            if (HttpContext.Session.GetString("Usuario") == null)
            {
                return Redirect("Index");
            }
            else
            {
                return View();
            }

        }

    }
    public class Log
    {
        public string usuario { get; set; }
        public string password { get; set; }
    }
}