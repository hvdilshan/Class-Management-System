﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Project.Models;
using System.IO;
using System.Net;
using System.Data.Entity;
using Microsoft.Reporting.WebForms;
using System.Data.Entity.Validation;
using System.Collections;

namespace Project.Controllers
{
    public class UploadFileController : Controller
    {
        ////////////////////////////////////////////////////////////
        ////                                                    ////                    
        ////                   Admin Side                       ////
        ////                                                    ////
        ////////////////////////////////////////////////////////////

        // GET: UploadFile
        public ActionResult Index()
        {
            try
            {
                ViewBag.FileMessage = "Select file you want to upload";
                ViewBag.teachersList = new SelectList(GetteachersList(), "teacher_id", "teacher_name");
                return View();
            }
            catch(Exception ex)
            {
                return View("Error", new HandleErrorInfo(ex, "UploadFile", "Index"));
            }
           
        }

        [HandleError]
        public List<TeacherList> GetteachersList()
        {
            List<TeacherList> teachers = null;
           
            DBmodel db = new DBmodel();
            teachers = db.TeacherLists.ToList();

            return teachers;
        }

        public ActionResult GetSubjectList(int teacher_id)
        {
            try {
                DBmodel db = new DBmodel();
                List<teacher_subject> subID = db.teacher_subject.Where(x => x.teacher_id == teacher_id).ToList();
                List<subject> subjects = new List<subject>();
                foreach (var item in subID)
                {
                    int subjectId = item.subject_id;

                     subjects.Add(db.subjects.Where(x => x.subject_id == subjectId).FirstOrDefault());

                }
                ViewBag.subList = new SelectList(subjects, "subject_id", "subject1");


                return PartialView("DisplaySubjects");
            }
            catch(Exception ex)
            {
                return View("Error", new HandleErrorInfo(ex, "UploadFile", "GetSubjectList"));

            }
        }

        public ActionResult GetGradeList(int teacher_id,int subject_id)
        {
            try {
                DBmodel db = new DBmodel();
                List<teacher_grade> grades = db.teacher_grade.Where(x => x.teacher_id == teacher_id && x.subjectCode == subject_id).ToList();

                ViewBag.gradeList = new SelectList(grades, "grade_id", "grade");

                return PartialView("DisplayGrades");
            }catch(Exception ex)
            {
                return View("Error", new HandleErrorInfo(ex, "UploadFile", "GetGradeList"));
            }
        }

        [HttpPost]
        public ActionResult Index(IEnumerable<HttpPostedFileBase> files,TeacherRel model, String message) 
        {
            try {
                    DBmodel db = new DBmodel();
                    upload_file log = new upload_file();
                    upload_file_teacher log2 = new upload_file_teacher();

                    int count = 0;
                    if (!ModelState.IsValid)
                    {
                        return new JsonResult { Data = "File not uploaded" };
                    }
                    else
                    {
                        if (files != null)
                        {
                            foreach (var file in files)
                            {
                                //check is the file name is already in the database
                                string filename = file.FileName;
                                List<upload_file> list = db.upload_file.ToList();
                                foreach (var item in list)
                                {
                                    if (item.file_name.Contains(filename))
                                    {
                                        ViewBag.DuplicateFile = "File name is already used!";
                                        return new JsonResult { Data = "File not uploaded, File name is already used!" };
                                    }
                                else
                                    {
                                        ViewBag.DuplicateFile = "File name is already used!";
                                    }
                                }

                                if (file != null && file.ContentLength > 0)
                                {
                                    var fileName = file.FileName;
                                    
                                    var path = Path.Combine(Server.MapPath("~/UploadedFiles"), fileName);
                                    

                                    log.file_name = fileName;
                                    log.file_path = path;
                                    log.upload_date = DateTime.Now;

                                    int gradeid = model.grade_id;
                                    int subjectid = model.subject_id;


                                    var grades = db.GradeLists.Where(u => u.ID == gradeid)
                                                                     .Select(u => new
                                                                     {
                                                                         grade = u.Grade
                                                                     }).Single();

                                    var subjects = db.subjects.Where(u => u.subject_id == subjectid)
                                                                    .Select(u => new
                                                                    {
                                                                        subject = u.subject1
                                                                    }).Single();

                                    log.grade = grades.grade;
                                    log.subject = subjects.subject;

                                    
                                    //db.SaveChanges();

                                    int teacherid = model.teacher_id;

                                    log2.teacher_id = teacherid;

                                    db.upload_file.Add(log);
                                    db.upload_file_teacher.Add(log2);

                                    file.SaveAs(path);
                                    db.SaveChanges();

                                    count++;
                                }
                            }
                            return new JsonResult { Data = "Successfully file Uploaded" };
                        }
                        else
                            return new JsonResult { Data = "File not uploaded" };

                    }
            }
            catch(Exception ex)
            {
                return View("Error", new HandleErrorInfo(ex, "UploadFile", "Index"));
            }
        }

        public ActionResult ViewList()
        {
            try {
                DBmodel db = new DBmodel();

                List<upload_file> files = db.upload_file.ToList();

                return View(files);
            }
            catch (Exception ex)
            {
                return View("Error", new HandleErrorInfo(ex, "UploadFile", "ViewList"));
            }

        }
        
        [HandleError]
        public ActionResult DownloadFile(string fileName)
        {
            try {
                DBmodel db = new DBmodel();

                var directory = new DirectoryInfo(Server.MapPath("~/UploadedFiles"));
                
                string path = directory.FullName;
                
                //"D:\MVC\Project\Project\UploadedFiles\"
                byte[] fileBytes = System.IO.File.ReadAllBytes(@path +"\\"+ fileName);
                return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, fileName);
            }
            catch(Exception ex)
            {

                return View("Error", new HandleErrorInfo(ex, "UploadFile", "DownloadFile"));

            }


        }

        public ActionResult Delete(int? id)
        {
            try {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                DBmodel db = new DBmodel();
                upload_file file = db.upload_file.Find(id);
                UploadFile model = new UploadFile();

                if (file == null)
                {
                    return HttpNotFound();
                }
                else
                {
                    model.file_id = file.file_id;
                    model.file_name = file.file_name;
                    model.file_path = file.file_path;
                    model.grade = file.grade;
                    model.subject = file.subject;
                }
                return View(model);
            }
            catch(Exception ex)
            {
                return View("Error", new HandleErrorInfo(ex, "UploadFile", "Delete"));
            }
        }

        [HttpPost]
        [ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteSucces(int id)
        {
            try
            {
                DBmodel db = new DBmodel();
                upload_file file = db.upload_file.Find(id);


                upload_file_teacher teacher = db.upload_file_teacher.Find(id);
               
                //db.SaveChanges();

                var directory = new DirectoryInfo(Server.MapPath("~/UploadedFiles"));
                //FileInfo[] getFile = directory.GetFiles(file.file_name+".*");
                string getFile = directory.FullName + "\\" + file.file_name;

                System.IO.File.Delete(getFile);
                db.upload_file_teacher.Remove(teacher);
                db.upload_file.Remove(file);

                db.SaveChanges();

                if(Session["Role"].ToString().Contains("Teacher"))
                {
                    return RedirectToAction("TeacherViewList");
                }
                else
                {
                    return RedirectToAction("ViewList");
                }
               
            }
            catch (FileNotFoundException ex)
            {
                return View("Error", new HandleErrorInfo(ex, "UploadFile", "DeleteSucces"));
            }
            catch (Exception ex)
            {
                return View("Error", new HandleErrorInfo(ex, "UploadFile", "DeleteSucces"));
            }
        }

        public ActionResult Edit(int? id)
        {
            try
            {
                DBmodel db = new DBmodel();
                upload_file file = db.upload_file.Find(id);
                UploadFile model = new UploadFile();

                if(file == null)
                {
                    return HttpNotFound();
                }
                else
                {
                    ViewBag.teachersList = new SelectList(GetteachersList(), "teacher_id", "teacher_name");

                    model.file_id = file.file_id;
                    model.file_name = file.file_name;
                    model.file_path = file.file_path;
                    model.grade = file.grade;
                    model.subject = file.subject;
                }
                

                return View(model);
            }
            catch(Exception ex)
            {
                return View("Error", new HandleErrorInfo(ex, "UploadFile", "Edit"));
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Edit")]
        public ActionResult EditSucces(int id,UploadFile model)
        {
            try {
                if (ModelState.IsValid)
                {
                    DBmodel db = new DBmodel();

                    upload_file file = db.upload_file.Find(id);

                    string oldName = file.file_name;

                    file.file_name = model.file_name.Trim();
                   
                    int gradeID = model.grade_id;
                    int subjectID = model.subject_id;

                    var grade = db.GradeLists.Where(m => m.ID == gradeID)
                                    .Select(u => new
                                    {
                                        grade = u.Grade
                                    }).Single();

                    var subject = db.subjects.Where(m => m.subject_id == subjectID)
                                       .Select(u => new
                                       {
                                           subject = u.subject1
                                       }).Single();

                    file.grade = grade.grade;
                    file.subject = subject.subject;

                    var directory = new DirectoryInfo(Server.MapPath("~/UploadedFiles"));
                    string path = directory.FullName + "\\" + model.file_name;

                    // path = "D:\\MVC\\Project\\Project\\UploadedFiles\\" + fileName;

                    file.file_path = path.Trim();
                    

                    //db.SaveChanges();

                    int fileID = model.file_id;

                    upload_file_teacher teacher = db.upload_file_teacher.Find(fileID);

                    teacher.teacher_id = model.teacher_id;

                    

                   
                   
                    db.Entry(teacher).State = EntityState.Modified;
                    db.Entry(file).State = EntityState.Modified;
                    db.SaveChanges();
                    ChangeFileName(fileID, model.file_name, oldName);

                    return RedirectToAction("ViewList");
                }
                else
                {
                    ViewBag.teachersList = new SelectList(GetteachersList(), "teacher_id", "teacher_name");
                    return View(model);
                }
            }
            catch (Exception ex)
            {
                
                return View("Error", new HandleErrorInfo(ex, "UploadFile", "EditSucces"));
            }
        }

        [HandleError]
        public void ChangeFileName(int fileID,string fileName, string oldName)
        {
            try {
                

                var directory = new DirectoryInfo(Server.MapPath("~/UploadedFiles"));
                //FileInfo[] getFile = directory.GetFiles(oldName+".*");

                string getFile = directory.FullName + "\\" + oldName;

                //List<String> files = new List<string>();
                //var path = Path.Combine(Server.MapPath("~/UploadedFiles"), fileName);

                System.IO.File.Move(getFile, directory.FullName + "\\" + fileName);

                
            }
            catch (Exception ex)
            {
                
            }
        }

        public ActionResult Report(string ReportType)
        {
            try
            {
                DBmodel db = new DBmodel();
                LocalReport localReport = new LocalReport();
                localReport.ReportPath = Server.MapPath("~/Reports/uploadFileReport.rdlc");

                ReportDataSource reportDataSource = new ReportDataSource();
                reportDataSource.Name = "UploadFiles";
                reportDataSource.Value = db.upload_file.ToList();

                localReport.DataSources.Add(reportDataSource);

                string Rtype = ReportType;
                string fileNameExtention = "pdf";
                byte[] renderByte;

                renderByte = localReport.Render(Rtype);

                Response.AddHeader("content-disposition", "attachment:filename = upload_file_report_" + DateTime.Now + "." + fileNameExtention);
                return File(renderByte, fileNameExtention);
            }
            catch (FileNotFoundException ex)
            {
                return View("Error", new HandleErrorInfo(ex, "UploadFile", "Report"));
            }
            catch (Exception ex)
            {
                return View("Error", new HandleErrorInfo(ex, "UploadFile", "Report"));
            }
        }

        ////////////////////////////////////////////////////////////
        ////                                                    ////                    
        ////                   Student Side                     ////
        ////                                                    ////
        ////////////////////////////////////////////////////////////


        public ActionResult GetFileList()
        {
            try {
                DBmodel db = new DBmodel();

                List<subject> subjects = db.subjects.ToList();

                ViewBag.subjectList = new SelectList(subjects, "subject_id", "subject1");

                return View();
            }
            catch(Exception ex)
            {
                return View("Error", new HandleErrorInfo(ex, "UploadFile", "GetFileList"));
            }
        }

       
        public ActionResult GetFiles(TeacherRel model)
        {
            try {
                DBmodel db = new DBmodel();
                string grades = null;
                string subject = null;

                if (Session["Grade"] != null && Session["Subject"] != null)
                {
                     grades = Session["Grade"].ToString();
                     subject = Session["Subject"].ToString();
                }
                else
                {
                    ViewBag.sessionError = "Session time out";
                    return RedirectToAction("Loginpage", "Login");
                }

                List<upload_file> files = db.upload_file.Where(x=> x.grade == grades && x.subject == subject).ToList();
                return View(files);
            }
            catch (Exception ex)
            {
                return View("Error", new HandleErrorInfo(ex, "UploadFile", "GetFiles"));
            }
        }

        public ActionResult GetAllGrades()
        {
            try {
                DBmodel db = new DBmodel();
                List<grade> grades = db.grades.ToList();

                ViewBag.allgradeList = new SelectList(grades, "grade_id", "grade1");

                return PartialView("DisplayAllGrades");
            }
            catch(Exception ex)
            {
                return View("Error", new HandleErrorInfo(ex, "UploadFile", "GetAllGrades"));
            }
        }


        ////////////////////////////////////////////////////////////
        ////                                                    ////                    
        ////                   Teacher Side                     ////
        ////                                                    ////
        ////////////////////////////////////////////////////////////

        public ActionResult TeacherUpload()
        {
            try
            {
                DBmodel db = new DBmodel();
                int teacherID = Int32.Parse(Session["UserID"].ToString());
                List<teacher_subject> subID = db.teacher_subject.Where(x => x.teacher_id == teacherID).ToList();
                List<subject> subjects = new List<subject>();
                foreach (var item in subID)
                {
                    int subjectId = item.subject_id;

                    subjects.Add(db.subjects.Where(x => x.subject_id == subjectId).FirstOrDefault());

                }
                ViewBag.subList = new SelectList(subjects, "subject_id", "subject1");

                ViewBag.FileMessage = "Select file you want to upload";
                //List<subject> list = db.subjects.ToList();
                //ViewBag.subList = new SelectList(list, "subject_id", "subject1");
                return View();
            }
            catch (Exception ex)
            {
                return View("Error", new HandleErrorInfo(ex, "UploadFile", "TeacherUpload"));
            }

        }

        public ActionResult TeacherGetGradeList( int subject_id)
        {
            try
            {
                DBmodel db = new DBmodel();
                int teacher_id = Int32.Parse(Session["UserID"].ToString());
                List<teacher_grade> grades = db.teacher_grade.Where(x => x.teacher_id == teacher_id && x.subjectCode == subject_id).ToList();

                ViewBag.gradeList = new SelectList(grades, "grade_id", "grade");

                return PartialView("DisplayGrades");
            }
            catch (Exception ex)
            {
                return View("Error", new HandleErrorInfo(ex, "UploadFile", "TeacherGetGradeList"));
            }
        }

        [HttpPost]
        public ActionResult TeacherUpload(IEnumerable<HttpPostedFileBase> files, TeacherRel model, String message)
        {
            try
            {
                DBmodel db = new DBmodel();
                upload_file log = new upload_file();
                upload_file_teacher log2 = new upload_file_teacher();

                int count = 0;
                if (!ModelState.IsValid)
                {
                    return new JsonResult { Data = "File not uploaded" };
                }
                else
                {
                    if (files != null)
                    {
                        foreach (var file in files)
                        {

                            //check is the file name is already in the database
                            string filename = file.FileName;
                            List<upload_file> list = db.upload_file.ToList();
                            foreach (var item in list)
                            {
                                if (item.file_name.Contains(filename))
                                {
                                    ViewBag.DuplicateFile = "File name is already used!";
                                    return new JsonResult { Data = "File not uploaded, File name is already used!" };
                                }
                            }
                            if (file != null && file.ContentLength > 0)
                            {
                                var fileName = file.FileName;
                                var path = Path.Combine(Server.MapPath("~/UploadedFiles"), fileName);


                                log.file_name = fileName;
                                log.file_path = path;
                                log.upload_date = DateTime.Now;

                                int gradeid = model.grade_id;
                                int subjectid = model.subject_id;

                                var grades = db.GradeLists.Where(u => u.ID == gradeid)
                                                                 .Select(u => new
                                                                 {
                                                                     grade = u.Grade
                                                                 }).Single();

                                var subjects = db.subjects.Where(u => u.subject_id == subjectid)
                                                                .Select(u => new
                                                                {
                                                                    subject = u.subject1
                                                                }).Single();

                                log.grade = grades.grade;
                                log.subject = subjects.subject;


                                //db.SaveChanges();

                                int teacher_id = Int32.Parse(Session["UserID"].ToString());

                                log2.teacher_id = teacher_id;

                                db.upload_file.Add(log);
                                db.upload_file_teacher.Add(log2);

                                file.SaveAs(path);
                                db.SaveChanges();

                                count++;
                            }
                        }
                        return new JsonResult { Data = "Successfully file Uploaded" };
                    }
                    else
                        return new JsonResult { Data = "File not uploaded" };

                }
            }
            catch (Exception ex)
            {
                return View("Error", new HandleErrorInfo(ex, "UploadFile", "TeacherUpload"));
            }
        }

        public ActionResult TeacherViewList()
        {
            try
            {
                DBmodel db = new DBmodel();
                int teacher_id = Int32.Parse(Session["UserID"].ToString());

                List<upload_file_teacher> fileList = db.upload_file_teacher.Where(m => m.teacher_id == teacher_id).ToList();
                List<upload_file> files = new List<upload_file>();

                foreach (var item in fileList)
                {
                    files.Add(db.upload_file.Where(m => m.file_id == item.file_id).FirstOrDefault());
                }

                return View(files);
            }
            catch (Exception ex)
            {
                return View("Error", new HandleErrorInfo(ex, "UploadFile", "TeacherViewList"));
            }
        }

        public ActionResult TeacherEdit(int? id)
        {
            try
            {
                DBmodel db = new DBmodel();
                upload_file file = db.upload_file.Find(id);
                TeacherUpload model = new TeacherUpload();

                if (file == null)
                {
                    return HttpNotFound();
                }
                else
                {
                    int teacherID = Int32.Parse(Session["UserID"].ToString());
                    List<teacher_subject> subID = db.teacher_subject.Where(x => x.teacher_id == teacherID).ToList();
                    List<subject> subjects = new List<subject>();

                    foreach (var item in subID)
                    {
                        int subjectId = item.subject_id;

                        subjects.Add(db.subjects.Where(x => x.subject_id == subjectId).FirstOrDefault());

                    }
                    ViewBag.subList = new SelectList(subjects, "subject_id", "subject1");

                    model.file_id = file.file_id;
                    model.file_name = file.file_name;
                    model.file_path = file.file_path;
                    model.grade = file.grade;
                    model.subject = file.subject;
                }


                return View(model);
            }
            catch (Exception ex)
            {
                return View("Error", new HandleErrorInfo(ex, "UploadFile", "TeacherEditSucces"));
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("TeacherEdit")]
        public ActionResult TeacherEditSucces(int id, TeacherUpload model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    DBmodel db = new DBmodel();

                    upload_file file = db.upload_file.Find(id);

                    string oldName = file.file_name;

                    file.file_name = model.file_name.Trim();

                    int gradeID = model.grade_id;
                    int subjectID = model.subject_id;

                    var grade = db.GradeLists.Where(m => m.ID == gradeID)
                                    .Select(u => new
                                    {
                                        grade = u.Grade
                                    }).Single();

                    var subject = db.subjects.Where(m => m.subject_id == subjectID)
                                       .Select(u => new
                                       {
                                           subject = u.subject1
                                       }).Single();

                    file.grade = grade.grade;
                    file.subject = subject.subject;

                    var directory = new DirectoryInfo(Server.MapPath("~/UploadedFiles"));
                    string path = directory.FullName + "\\" + model.file_name;

                    // path = "D:\\MVC\\Project\\Project\\UploadedFiles\\" + fileName;
                    file.file_path = path.Trim();
                    //db.SaveChanges();

                    int fileID = model.file_id;
                    
                    db.Entry(file).State = EntityState.Modified;
                    db.SaveChanges();
                    ChangeFileName(fileID, model.file_name, oldName);

                    return RedirectToAction("TeacherViewList");
                }
                else
                {
                    ViewBag.teachersList = new SelectList(GetteachersList(), "teacher_id", "teacher_name");
                    return View(model);
                }
            }
            catch (Exception ex)
            {

                return View("Error", new HandleErrorInfo(ex, "UploadFile", "TeacherEditSucces"));
            }
        }
    }
}