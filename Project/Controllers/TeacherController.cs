﻿
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Project.Models;
using System.Data.Entity;
using Microsoft.Reporting.WebForms;
using System.Data.Entity.Infrastructure;
using System.Threading.Tasks;

namespace Staff_Management1.Controllers
{
    public class TeacherController : Controller
    {


        public ActionResult TeacherList()
        {
            using (DBmodel dbModel = new DBmodel())
            {
                return View(dbModel.Teachers.ToList());
            }
        }

        public ActionResult Reports(string ReportType)
        {
            using (DBmodel dbModel = new DBmodel())
            {
                LocalReport localreport = new LocalReport();
                localreport.ReportPath = Server.MapPath("~/Reports/TeachersReport.rdlc");

                ReportDataSource reportDataSource = new ReportDataSource();
                reportDataSource.Name = "TeachersDataSet";
                reportDataSource.Value = dbModel.Teachers.ToList();
                localreport.DataSources.Add(reportDataSource);
                string reportType = ReportType;
                string mimeType;
                string encoding;
                string fileNameExtension;

                if (reportType == "Excel")
                {
                    fileNameExtension = "xlsx";
                }
                else if (reportType == "Word")
                {
                    fileNameExtension = "docx";
                }
                else if (reportType == "PDF")
                {
                    fileNameExtension = "pdf";
                }
                else
                {
                    fileNameExtension = "jpg";
                }

                string[] streams;
                Warning[] warnings;
                byte[] renderByte;
                renderByte = localreport.Render(reportType, "", out mimeType, out encoding, out fileNameExtension, out streams, out warnings);
                Response.AddHeader("content-disposition", "attachment-filename = student_report." + fileNameExtension);
                return File(renderByte, fileNameExtension);

            }

        }




        // GET: Teacher
        public ActionResult Index()
        {
            using (DBmodel dbModel = new DBmodel())
            {
                return View(dbModel.Teachers.ToList());
            }
        }

        // GET: Teacher/Details/5
        public ActionResult Details(int id)
        {
            using (DBmodel dbModel = new DBmodel())
            {
                return View(dbModel.Teachers.Where(x => x.UserID == id).FirstOrDefault());
            }

        }

        // GET: Teacher/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Teacher/Create
        [HttpPost]
        public ActionResult Create(Teacher teacher)
        {
            try
            {
                using (DBmodel dbModel = new DBmodel())
                {
                    if (dbModel.StudentTBs.Any(m => m.username == teacher.Username)
                        && dbModel.Teachers.Any(m => m.Username == teacher.Username)
                        && dbModel.Cleaners.Any(m => m.Username == teacher.Username)
                        && dbModel.Offices.Any(m => m.Username == teacher.Username))
                    {
                        ViewBag.Error = "User name already exist! please use another one";
                        return View(teacher);
                    }
                    else
                    {

                        dbModel.Teachers.Add(teacher);
                        dbModel.SaveChanges();

                        TeacherList log = new TeacherList();
                        List<Teacher> list = dbModel.Teachers.ToList();
                        Teacher model = list.Last();

                        log.teacher_id = model.UserID;
                        log.teacher_name = teacher.Name;

                        dbModel.TeacherLists.Add(log);
                        dbModel.SaveChanges();
                    }
                }
                // TODO: Add insert logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: Teacher/Edit/5
        public ActionResult Edit(int id)
        {
            using (DBmodel dbModel = new DBmodel())
            {
                return View(dbModel.Teachers.Where(x => x.UserID == id).FirstOrDefault());
            }

        }

        // POST: Teacher/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, Teacher teacher)
        {
            try
            {
                using (DBmodel dbModel = new DBmodel())
                {
                    teacher.UserID = id;
                    dbModel.Entry(teacher).State = EntityState.Modified;
                    dbModel.SaveChanges();

                    TeacherList model = dbModel.TeacherLists.Find(id);
                    model.teacher_name = teacher.Name;

                    dbModel.Entry(model).State = EntityState.Modified;
                    dbModel.SaveChanges();


                   
                }
                // TODO: Add update logic here

                if (Session["Role"].ToString().Contains("Admin"))
                {
                    return RedirectToAction("Index");
                }
                else
                {
                    return RedirectToAction("Details", new { id = teacher.UserID });
                }
            }
            catch
            {
                return View();
            }
        }

        // GET: Teacher/Delete/5
        public ActionResult Delete(int id)
        {
            using (DBmodel dbModel = new DBmodel())
            {
                return View(dbModel.Teachers.Where(x => x.UserID == id).FirstOrDefault());
            }
        }

        // POST: Teacher/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, Teacher model)
        {
            
            try
            {
                // TODO: Add delete logic here
                using (DBmodel dbModel = new DBmodel())
                {
                    Teacher teacher = dbModel.Teachers.Where(x => x.UserID == id).FirstOrDefault();
                    TeacherList mod = dbModel.TeacherLists.Where(x => x.teacher_id == id).FirstOrDefault();
                    TeacherList mod1 = dbModel.TeacherLists.Where(x => x.teacher_id == id).FirstOrDefault();
                    TeacherList mod2 = dbModel.TeacherLists.Where(x => x.teacher_id == id).FirstOrDefault();
                    TeacherList mod3 = dbModel.TeacherLists.Where(x => x.teacher_id == id).FirstOrDefault();
                    TeacherList mod4 = dbModel.TeacherLists.Where(x => x.teacher_id == id).FirstOrDefault();
                    TeacherList mod5 = dbModel.TeacherLists.Where(x => x.teacher_id == id).FirstOrDefault();

                    string name = teacher.Name;
                    string school = teacher.Shool;

                    dbModel.Teachers.Remove(teacher);
                    


                    dbModel.SaveChanges();
                    

                }
                return RedirectToAction("Index");
            }
            catch
            {
                return RedirectToAction("Delete",id);
            }
        }
    }


}
