using CommunityToolkit.HighPerformance;
using ECommerce.Data;
using ECommerce.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Minio;
using Minio;
using Minio.DataModel;
using Minio.DataModel.Args;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Threading.Tasks;

namespace ECommerce.Controllers
{
    public class RasoiController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IMinioClient _minioClient;
        private readonly string _bucketName = "bucket-s3"; // Nome del bucket
        public RasoiController(ApplicationDbContext context, IMinioClient minioClient)
        {
            _context = context;
            _minioClient = minioClient;
        }
        private async Task EnsureBucketExistsAsync()
        {
            bool found = await _minioClient.BucketExistsAsync(new BucketExistsArgs().WithBucket(_bucketName));
            if (!found)
            {
                await _minioClient.MakeBucketAsync(new MakeBucketArgs().WithBucket(_bucketName));
            }
        }

        private async Task UploadImageToMinioAsync(IFormFile imageFile, string imageKey)
        {
            await EnsureBucketExistsAsync();

            using var memoryStream = new MemoryStream();
            await imageFile.CopyToAsync(memoryStream);
            memoryStream.Seek(0, SeekOrigin.Begin); // Reset del puntatore

            var metaData = new Dictionary<string, string>(StringComparer.Ordinal)
    {
        { "Original-FileName", imageFile.FileName }
    };

            var args = new PutObjectArgs()
                .WithBucket(_bucketName)
                .WithObject(imageKey)
                .WithStreamData(memoryStream)
                .WithObjectSize(memoryStream.Length)
                .WithContentType(imageFile.ContentType ?? "application/octet-stream")
                .WithHeaders(metaData);

            await _minioClient.PutObjectAsync(args);
        }



        private async Task DownloadImageFromMinioAsync(string imageKey, Stream destination)
        {
            await _minioClient.GetObjectAsync(new GetObjectArgs()
                .WithBucket(_bucketName)
                .WithObject(imageKey)
                .WithCallbackStream(stream =>
                {
                    stream.CopyTo(destination);
                }));
        }

        // --- AZIONI HTTP (pubbliche) ---

        [HttpPost]
        public async Task<IActionResult> UploadImage(IFormFile imageFile)
        {
            if (imageFile == null || imageFile.Length == 0)
                return BadRequest("File non valido");

            string imageKey = Guid.NewGuid().ToString() + Path.GetExtension(imageFile.FileName);
            await UploadImageToMinioAsync(imageFile, imageKey);

            // Puoi salvare imageKey nel database o restituirlo al client
            return Ok($"Immagine caricata: {imageKey}");
        }

        [HttpGet]
        public async Task<IActionResult> GetImage(string imageKey)
        {
            var memoryStream = new MemoryStream();
            await DownloadImageFromMinioAsync(imageKey, memoryStream);
            memoryStream.Seek(0, SeekOrigin.Begin);

            return File(memoryStream, "image/jpeg"); // o image/png, ecc.
        }



        // GET: Rasoi
        public async Task<IActionResult> Index()
        {
            return View(await _context.Rasois.ToListAsync());
        }

        // GET: Rasoi/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var rasoi = await _context.Rasois
                .FirstOrDefaultAsync(m => m.Id == id);
            if (rasoi == null)
            {
                return NotFound();
            }

            return View(rasoi);
        }

        


        private bool IsValidImageFile(IFormFile file)
        {
            var permittedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();

            return !string.IsNullOrEmpty(extension) && permittedExtensions.Contains(extension);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Rasoi rasoi, IFormFile ImageFile)
        {
            if (ModelState.IsValid)
            {
                if (ImageFile != null && ImageFile.Length > 0)
                {
                    // Validazione del file
                    if (!IsValidImageFile(ImageFile))
                    {
                        ModelState.AddModelError("ImageFile", "Formato file non supportato. Usa JPG, PNG o GIF.");
                        return View(rasoi);
                    }

                    // Genera un nome univoco per l'immagine
                    var imageFileName = $"{Guid.NewGuid()}{Path.GetExtension(ImageFile.FileName)}";
                    rasoi.ImageKey = imageFileName;

                    // Salva il file localmente nella cartella wwwroot/images
                    await UploadImageToMinioAsync(ImageFile, imageFileName);

                }

                // Salva i dati nel DB
                _context.Add(rasoi);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            return View(rasoi);
        }


        // GET: Rasoi/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var rasoi = await _context.Rasois.FindAsync(id);
            if (rasoi == null)
            {
                return NotFound();
            }
            return View(rasoi);
        }

        // POST: Rasoi/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Marca,Modello,Prezzo,Tipo")] Rasoi rasoi)
        {
            if (id != rasoi.Id)
            {
                return NotFound();
            }
            
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(rasoi);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!RasoiExists(rasoi.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(rasoi);
        }

        // GET: Rasoi/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var rasoi = await _context.Rasois
                .FirstOrDefaultAsync(m => m.Id == id);
            if (rasoi == null)
            {
                return NotFound();
            }

            return View(rasoi);
        }

        // POST: Rasoi/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var rasoi = await _context.Rasois.FindAsync(id);
            if (rasoi != null)
            {
                _context.Rasois.Remove(rasoi);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool RasoiExists(int id)
        {
            return _context.Rasois.Any(e => e.Id == id);
        }
    }
}
