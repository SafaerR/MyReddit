using System.Reflection.Metadata;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MyReddit.Models;
using Microsoft.AspNetCore.Http;
using System.Text;
using System.Data;

namespace MyReddit.Controllers
{
    public class HomeController : Controller
    {
          private readonly dbSiteContext context;
        private readonly ISession session;
         public HomeController(dbSiteContext context, IHttpContextAccessor accessor)
        {
            this.context = context;
            this.session = accessor.HttpContext.Session;
        }


        public IActionResult Index(string username, string email, string pwd)
        {
            //recuperation de variable de session
            ViewBag.Username=session.GetString("Username");
            ViewBag.email = session.GetString("email");
            ViewBag.password = session.GetString("password");

            var result = context.Posts.ToList();
            ViewBag.listcomments = context.Comments.ToList();
            ViewBag.listUser=context.Users.ToList();
            // ViewBag.Cpt =context.Comments.ToList().Count() ;
            var lienplusinteractif = context.Posts.OrderByDescending(p => p.Upvote - p.DownVote).ToList();

            if(ViewBag.Username!= null){
                return View(lienplusinteractif);
            }
            else{
                return RedirectToAction("Login");
            }
             
        }

        //----creation de l'action du formulaire login
        public IActionResult Login(string msg){

            ViewBag.Message = msg;
            return View();
        }
        //---Methode Authentication
        [HttpPost]
        public IActionResult Login(string email, string password){
            ViewBag.Message = "";

            if(email == null || password==null) {            
                ViewBag.Message = "email or password is incorrect";
                return RedirectToAction("Login", new{msg=ViewBag.Message});
             }
            else{
                  var u = context.Users.FirstOrDefault(u => u.Email == email && u.Pwd == password);
                    if (u != null){

                        //ViewBag.Username = u.UserName;
                        session.SetString("Username", u.UserName);
                        session.SetString("email", email);
                        session.SetString("password", password);

                      return RedirectToAction("Index", new {username= u.UserName, email= email, pwd= password});
                
                    }

                     else{
                          ViewBag.Message = "email or password is incorrect";
                        return RedirectToAction("Login", new{msg=ViewBag.Message}); 
                }
            }          
                
        }

        //formulaire pour nouveau user
            public IActionResult Signup(string msg){
                ViewBag.Duplicateuser = msg;
            return View();
        }

        //action d'ajout d'un nouveau user
        public IActionResult AddUser(string pseudo, string email, string password) {
            ViewBag.Duplicateuser = "";
            ViewBag.Message = "";
            if(pseudo == null || email == null || password==null) {
                ViewBag.Message = "Veuillez remplir tous les champs";
                  return RedirectToAction("Signup",new{msg=ViewBag.Message});
            }
                
            else
                if( context.Users.Any(u => u.UserName == pseudo)){
                ViewBag.Duplicateuser = "Ce pseudo existe deja!!";

                return RedirectToAction("Signup",new{msg=ViewBag.Duplicateuser});
            }
            else{
                User user = new User(){
                    UserName = pseudo,
                    Email = email,
                    Pwd = password
                    };
                context.AddNewUser(user);
                ViewBag.Message = "Vous etes bien enregistré";
                return RedirectToAction("Login", new{msg=ViewBag.Message});    
             }
        }

        //Ajouter un lien
        public IActionResult AddLink(string username, string msg){
            ViewBag.Username = username;
            var u = context.Users.FirstOrDefault(u => u.UserName == username);
            //var u = context.Users.Any(u => u.UserName == username);
            ViewBag.Userid=u.Id;
            ViewBag.Message = msg;
            return View();
        }

        //
     [HttpPost]
         public IActionResult AddLink(string link, string description, DateTime publicationdate, int upvote, int downvote, int userid){         
         publicationdate= System.DateTime.Now;

            ViewBag.Username=session.GetString("Username");
            ViewBag.Message = "";

            if (link == null || description == null){

          //return BadRequest(new { message = "Username or password is incorrect." });    
             ViewBag.Message = "Veuillez remplir tous les champs";   
                return RedirectToAction("AddLink", new{username=ViewBag.Username, msg=ViewBag.Message});
            }
            else{

              Post post = new Post(){
                Link = link,
                Description = description,
                PublicationDate = publicationdate,
                Upvote = upvote,
                DownVote = downvote,
                UserId = userid
            };
                context.AddNewLink(post);
                 //ViewBag.Message = "Votre lien est bien ajouté";
                return RedirectToAction("MyLinks", new{userid=userid});   
            }
     }
        
        //Afficher tous mes liens
        public IActionResult MyLinks(){
            ViewBag.Username=session.GetString("Username");
            var u = context.Users.FirstOrDefault(u => u.UserName == session.GetString("Username"));
            //ViewBag.Username = context.Users.Find(userid).UserName;
            ViewBag.Userid = u.Id;

            var resultat = context.getLinkForUser(u.Id).ToList();
            if (resultat.Count() != 0)
            {
                ViewBag.listliens = resultat.Count();
                var c = context.Comments.Where(c => c.PostId == resultat.FirstOrDefault().Id).ToList();
                ViewBag.comm = c.Count();
                ViewBag.listcomments = context.Comments.ToList();
                ViewBag.Message = "";
            }

           
            return View(resultat);
            
           
        }

        //afficher le link
         public IActionResult Link(int linkid, int userid){
         
            ViewBag.Auteur = context.Users.Find(userid).UserName;
            ViewBag.Username=session.GetString("Username");
            ViewBag.Userid = context.Users.FirstOrDefault(u => u.UserName == session.GetString("Username")).Id;
            
            ViewBag.TotalComments= context.getCommentForLink(linkid).Count();//---pour afficher le total des commentaires           

      
            ViewBag.comments = context.getCommentForLink(linkid).OrderByDescending(c=>c.PublicationDate).ToList();//la list des commentaire d'un lien
            ///ViewBag.idusercomment = ViewBag.comments.Find().Id;


            var p = context.Posts.Where(p => p.UserId == userid && p.Id == linkid);
            ViewBag.upvote = context.Posts.Find(linkid).Upvote;
            ViewBag.downvote = context.Posts.Find(linkid).DownVote;

            //ViewBag.Message = "Vous ne pouvez pas commenter";

            var resultat = context.getInfosLink(linkid);
            

                ViewBag.listcomments = context.Comments.ToList();
                ViewBag.listUser=context.Users.ToList();
            

            return View(resultat);
        }

[HttpPost]
   //ajout de l"action des votes
          public IActionResult Link(int linkid, string vot){
            var link = context.Posts.Find(linkid);

            if(vot =="UpVote")
                link.Upvote++; 
            
            else
                link.DownVote++; 
            

            context.UpdateLink(link);

            return RedirectToAction("Link", new {linkid=link.Id,userid=link.UserId});
        }

        public IActionResult AddCommentaire(string description, DateTime publicationdate, int userid, int postid){
            //string format = "dd/MM/yyyy HH:mm:ss";

            publicationdate = System.DateTime.Now;

            Comment comment = new Comment(){
                Description = description,
                PublicationDate = publicationdate,
                UserId = userid,
                PostId=postid
            };
                context.AddNewComment(comment);
                 //ViewBag.Message = "Votre lien est bien ajouté";
                return RedirectToAction("Link", new{linkid=postid, userid=userid});   
        }

        //delete link
         public IActionResult DeleteLink(int linkid){
  
            context.Comments.RemoveRange(context.Comments.Where(c =>c.PostId == linkid));//supprimer tt d'abord les commentaire du lien
            context.SaveChanges();

            var link = context.Posts.Find(linkid);
            context.DeleteLink(link);
            return RedirectToAction("Mylinks");  
        }

        //Deconnexion
         public IActionResult Logout(){
            session.Clear();
            return RedirectToAction("Index");
        }
    }
}
