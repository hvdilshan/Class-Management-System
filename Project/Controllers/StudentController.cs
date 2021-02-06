﻿using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Reporting.WebForms;
using Project.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace Project.Controllers
{
    public class StudentController : Controller
    {

        // GET: Student/Home
        [AllowAnonymous]
        public ActionResult Homepage()
        {
            
            return View();
        }

        // GET: Student
        
        public ActionResult Index()
        {
            using (DBmodel dBModels = new DBmodel())
            {
                return View(dBModels.StudentTBs.ToList());
            }
        }

        
        public ActionResult Reports(string ReportType)
        {
            LocalReport localreport = new LocalReport();
            DBmodel dBModels = new DBmodel();
            localreport.ReportPath = Server.MapPath("~/Reports/StudentReport.rdlc");

            ReportDataSource reportDataSource = new ReportDataSource();
            reportDataSource.Name = "StudentDataSet1";
            reportDataSource.Value = dBModels.StudentTBs.ToList();
            localreport.DataSources.Add(reportDataSource);

            String reportType = ReportType;
            String mimeType;
            string encoding;
            String fileNameExtension;

            if (reportType == "PDF")
            {
                fileNameExtension = "pdf";
            }
            else if (reportType == "Word")
            {
                fileNameExtension = "docx";
            }

            string[] strems;
            Warning[] warnings;
            byte[] renderdByte;

            renderdByte = localreport.Render(reportType, "", out mimeType, out encoding, out fileNameExtension, out strems, out warnings);
            Response.AddHeader("content-dispostion", "attachment:filename = student_report." + fileNameExtension);
            return File(renderdByte, fileNameExtension);

        }

        // GET: Student/Details/5
        [AllowAnonymous]
        public ActionResult Details(int id)
        {
            using (DBmodel dBModels = new DBmodel())
            {
                StudentTB temp = dBModels.StudentTBs.Where(x => x.sid == id).FirstOrDefault();
                Studentmodel model = new Studentmodel();

                model.fullname = temp.fullname;
                model.gender = temp.gender;
                model.address = temp.address;
                model.email = temp.email;
                model.grade = temp.grade;
                model.parentname = temp.parentname;
                model.parenttpnum = temp.parenttpnum;
                model.password = temp.password;
                model.school = temp.school;
                model.sid = temp.sid;
                model.subject = temp.subject;
                model.tpnum = temp.tpnum;
                model.username = temp.username;

                return View(model);
            }
        }

        // GET: Student/Create
        
        public ActionResult Create()
        {
            DBmodel db = new DBmodel();

            List<GradeList> grades = db.GradeLists.ToList();
            List<subject> subjects = db.subjects.ToList();

            ViewBag.Grades = new SelectList(grades, "Grade", "Grade");
            ViewBag.Subjects = new SelectList(subjects, "subject1", "subject1");


            return View();
        }

        // POST: Student/Create
        [HttpPost]
        
        [ValidateAntiForgeryToken]
        public ActionResult Create(Studentmodel temp)
        {
            try
            {
                using (DBmodel dBModels = new DBmodel())
                {
                   if(dBModels.StudentTBs.Any(m=>m.username == temp.username) 
                        || dBModels.Teachers.Any(m=>m.Username == temp.username)
                        || dBModels.Cleaners.Any(m=>m.Username == temp.username)
                        || dBModels.Offices.Any(m=>m.Username == temp.username))
                    {
                        ViewBag.Error = "User name already exist! please use another one";

                        List<GradeList> grades = dBModels.GradeLists.ToList();
                        List<subject> subjects = dBModels.subjects.ToList();

                        ViewBag.Grades = new SelectList(grades, "Grade", "Grade");
                        ViewBag.Subjects = new SelectList(subjects, "subject1", "subject1");
                        return View(temp);
                    }
                    else
                    {
                        StudentTB model = new StudentTB();

                        model.fullname = temp.fullname;
                        model.gender = temp.gender;
                        model.address = temp.address;
                        model.email = temp.email;
                        model.grade = temp.grade;
                        model.parentname = temp.parentname;
                        model.parenttpnum = temp.parenttpnum;
                        model.password = temp.password;
                        model.school = temp.school;
                        model.sid = temp.sid;
                        model.subject = temp.subject;
                        model.tpnum = temp.tpnum;
                        model.username = temp.username;

                        dBModels.StudentTBs.Add(model);

                        if (ModelState.IsValid)
                        {
                            dBModels.SaveChanges();
                            return RedirectToAction("Index", "Student");
                        }
                        else
                        {
                            List<GradeList> grades = dBModels.GradeLists.ToList();
                            List<subject> subjects = dBModels.subjects.ToList();

                            ViewBag.Grades = new SelectList(grades, "Grade", "Grade");
                            ViewBag.Subjects = new SelectList(subjects, "subject1", "subject1");
                            return View(temp);
                        }
                    }
                }

                //return RedirectToAction("Loginpage", "Student");
            }
            catch
            {
                return View();
            }

        }

        // GET: Student/Edit/5
        [AllowAnonymous]
        public ActionResult Edit(int id)
        {
            using (DBmodel dBModels = new DBmodel())
            {
                StudentTB temp = dBModels.StudentTBs.Where(x => x.sid == id).FirstOrDefault();
                Studentmodel model = new Studentmodel();

                model.fullname = temp.fullname;
                model.gender = temp.gender;
                model.address = temp.address;
                model.email = temp.email;
                model.grade = temp.grade;
                model.parentname = temp.parentname;
                model.parenttpnum = temp.parenttpnum;
                model.password = temp.password;
                model.school = temp.school;
                model.sid = temp.sid;
                model.subject = temp.subject;
                model.tpnum = temp.tpnum;
                model.username = temp.username;

                List<GradeList> grades = dBModels.GradeLists.ToList();
                List<subject> subjects = dBModels.subjects.ToList();

                ViewBag.Grades = new SelectList(grades, "Grade", "Grade");
                ViewBag.Subjects = new SelectList(subjects, "subject1", "subject1");

                return View(model);
            }
        }

        // POST: Student/Edit/5
        [HttpPost]
        [AllowAnonymous]
        public ActionResult Edit(int id, Studentmodel temp)
        {
            try
            {
                StudentTB model = new StudentTB();
                using (DBmodel dBModels = new DBmodel())
                {
                   

                    model.fullname = temp.fullname;
                    model.gender = temp.gender;
                    model.address = temp.address;
                    model.email = temp.email;
                    model.grade = temp.grade;
                    model.parentname = temp.parentname;
                    model.parenttpnum = temp.parenttpnum;
                    model.password = temp.password;
                    model.school = temp.school;
                    model.sid = id;
                    model.subject = temp.subject;
                    model.tpnum = temp.tpnum;
                    model.username = temp.username;

                    dBModels.Entry(model).State = System.Data.Entity.EntityState.Modified;
                    dBModels.SaveChanges();
                }
                if (Session["Role"].ToString().Contains("Admin"))
                {
                    return RedirectToAction("Index");
                }
                else
                {
                    return RedirectToAction("Details", new { id = model.sid });
                }
                
            }
            catch
            {
                DBmodel dBModels = new DBmodel();
                List<GradeList> grades = dBModels.GradeLists.ToList();
                List<subject> subjects = dBModels.subjects.ToList();

                ViewBag.Grades = new SelectList(grades, "Grade", "Grade");
                ViewBag.Subjects = new SelectList(subjects, "subject1", "subject1");
                return View();
            }
        }

        // GET: Student/Delete/5
        
        public ActionResult Delete(int id)
        {
            using (DBmodel dBModels = new DBmodel())
            {
                return View(dBModels.StudentTBs.Where(x => x.sid == id).FirstOrDefault());
            }
        }

        // POST: Student/Delete/5
        [HttpPost]
        
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here
                using (DBmodel dBModels = new DBmodel())
                {
                    StudentTB studentTB = dBModels.StudentTBs.Where(x => x.sid == id).FirstOrDefault();
                    dBModels.StudentTBs.Remove(studentTB);
                    dBModels.SaveChanges();
                }

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        public ActionResult AddSubject(int id)
        {
            subjectList subjectList1 = new subjectList();

            subjectList1.Id = id;

            return View(subjectList1);

        }
        [HttpPost]
        public ActionResult AddSubject(int id, subjectList subjectList1)
        {
            try
            {
                using (DBmodel dBModels = new DBmodel())
                {


                    if (subjectList1.Maths == true)
                    {
                        StudentSubject studentSubject = new StudentSubject();

                        studentSubject.sid = id;
                        studentSubject.subject = "Maths";
                        dBModels.StudentSubjects.Add(studentSubject);
                        dBModels.SaveChanges();
                    }

                    if (subjectList1.English == true)
                    {
                        StudentSubject studentSubject = new StudentSubject();

                        studentSubject.sid = id;
                        studentSubject.subject = "English";
                        dBModels.StudentSubjects.Add(studentSubject);
                        dBModels.SaveChanges();
                    }

                    if (subjectList1.Science == true)
                    {
                        StudentSubject studentSubject = new StudentSubject();

                        studentSubject.sid = id;
                        studentSubject.subject = "Science";
                        dBModels.StudentSubjects.Add(studentSubject);
                        dBModels.SaveChanges();
                    }

                }

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
    }
}