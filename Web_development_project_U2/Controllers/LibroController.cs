﻿using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using Web_development_project_U2.Models.ViewModels;

namespace Web_development_project_U2.Controllers
{
    public class LibroController : Controller
    {
        public ActionResult libro()
        {
            nt userId = (int)Session["UserId"];
            List<ListLibroViewModel> lst;
            using (bibliotecaEntities db = new bibliotecaEntities())
            {
                lst = (from c in db.Libros
                           //where c.id == userId
                       select new ListLibroViewModel
                       {
                           id = c.id,
                           titulo = c.titulo,
                           autor = c.autor,
                           anio_publicacion = c.anio_publicacion,
                           categoria = c.categoria
                       }).ToList();
            }
            return View(lst);
        }

        public ActionResult Nuevo()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Nuevo(LibroViewModel libroModel)
        {
            try
            {
                //Sintáxis para agregar la imagen
                HttpPostedFileBase FileBase = Request.Files[0];

                if (FileBase != null && FileBase.ContentLength > 0)
                {
                    // Si el campo de imagen tiene contenido, crear el WebImage
                    WebImage image = new WebImage(FileBase.InputStream);
                    libroModel.imagen = image.GetBytes();
                }
                else
                {
                    // Si el campo de imagen está vacío, asignar un valor nulo o un arreglo de bytes vacío
                    libroModel.imagen = null; // o libroModel.imagen = new byte[0];
                }

                if (ModelState.IsValid)
                {
                    using (bibliotecaEntities db = new bibliotecaEntities())
                    {
                        var oLibro = new Libros();
                        oLibro.titulo = libroModel.titulo;
                        oLibro.autor = libroModel.autor;
                        oLibro.anio_publicacion = libroModel.anio_publicacion;
                        oLibro.categoria = libroModel.categoria;
                        oLibro.imagen = libroModel.imagen;

                        db.Libros.Add(oLibro);
                        db.SaveChanges();
                    }
                }
                return Redirect("~/Home/Admin");
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message); 
            }
        }

        public ActionResult Editar(int id)
        {
            LibroViewModel libro = new LibroViewModel();
            using (bibliotecaEntities db = new bibliotecaEntities())
            {
                var oLibro = db.Libros.Find(id);
                libro.titulo = oLibro.titulo;
                libro.autor = oLibro.autor;
                libro.anio_publicacion = oLibro.anio_publicacion;
                libro.categoria = oLibro.categoria;
                libro.id = oLibro.id;
            }
            return View(libro);
        }

        [HttpPost]
        public ActionResult Editar(LibroViewModel libroModel, HttpPostedFileBase nuevaImagen)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    using (bibliotecaEntities db = new bibliotecaEntities())
                    {
                        var oLibro = db.Libros.Find(libroModel.id);
                        oLibro.titulo = libroModel.titulo;
                        oLibro.autor = libroModel.autor;
                        oLibro.anio_publicacion = libroModel.anio_publicacion;
                        oLibro.categoria = libroModel.categoria;

                        if (nuevaImagen != null && nuevaImagen.ContentLength > 0)
                        {
                            // Si se proporciona una nueva imagen, convertirla a bytes y guardarla en el libro
                            WebImage image = new WebImage(nuevaImagen.InputStream);
                            oLibro.imagen = image.GetBytes();
                        }

                        db.Entry(oLibro).State = System.Data.Entity.EntityState.Modified;
                        db.SaveChanges();
                    }
                    return Redirect("~/Home/Admin");
                }

                return View(libroModel);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        [HttpGet]
        public ActionResult Eliminar(int id)
        {
            using (bibliotecaEntities db = new bibliotecaEntities())
            {
                var oCliente = db.Libros.Find(id);
                db.Libros.Remove(oCliente);
                db.SaveChanges();
            }
            return Redirect("~/Home/Admin");
        }

        public ActionResult getImage(int id)
        {
            bibliotecaEntities db = new bibliotecaEntities();
            Libros model = db.Libros.Find(id);
            byte[] byteImage = model.imagen;

            if (byteImage != null && byteImage.Length > 0)
            {
                MemoryStream memoryStream = new MemoryStream(byteImage);
                Image image = Image.FromStream(memoryStream);
                memoryStream = new MemoryStream();
                image.Save(memoryStream, ImageFormat.Jpeg);
                memoryStream.Position = 0;
                return File(memoryStream, "image/jpeg");
            }
            else
            {
                // Si el campo de imagen está vacío, puedes devolver una imagen de reemplazo o un mensaje de error.
                // Por ejemplo:
                // return File(Server.MapPath("~/Images/default_image.jpg"), "image/jpeg");
                // O
                // return Content("Imagen no disponible");
                // O simplemente redirigir a una vista que muestre un mensaje apropiado.
                return Content("Imagen no disponible");
            }
        }

    }
}