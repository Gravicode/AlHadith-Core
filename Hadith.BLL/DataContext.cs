using System;
using System.Collections.Generic;
using System.Text;
using Hadith.DAL;
using Microsoft.EntityFrameworkCore;

namespace Hadith.BLL {
    public class DataContext : DbContext {
        public DbSet<Bookmark> Bookmarks { get; set; }
        public DbSet<hadith> hadiths { get; set; }
        public DbSet<hadithchapter> hadithchapters { get; set; }
        public DbSet<hadithcontent> hadithcontents { get; set; }
        public DbSet<hadithindex> hadithindexs { get; set; }
        public DbSet<hadithpage> hadithpages { get; set; }
        public DbSet<language> languages { get; set; }
      
        public string DbPath { get; }

        public DataContext() {

            //var folder = Environment.SpecialFolder.LocalApplicationData;
            //DbPath = quran_data.Conn;
            //string.IsNullOrEmpty(quran_data.Conn) ? Environment.GetFolderPath(folder) : 
            //DbPath = System.IO.Path.Join(path, "/model-builder");
            //if (!Directory.Exists(DbPath))
            //    Directory.CreateDirectory(DbPath);
            //DbPath = System.IO.Path.Join(DbPath, "/ml.db");
        }

        // The following configures EF to create a Sqlite database file in the
        // special "local" folder for your platform.
        protected override void OnConfiguring(DbContextOptionsBuilder options)
            => options.UseSqlite(HadithData.Conn);
    }
   
}