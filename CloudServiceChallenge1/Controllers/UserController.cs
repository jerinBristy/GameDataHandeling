
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using CloudServiceChallenge1.Models;
using System.Collections;

namespace CloudServiceChallenge1.Controllers
{
    public class UserController : Controller
    {
        private readonly GameUserContext _context;

        public UserController(GameUserContext context)
        {
            _context = context;
        }

        // GET: User
        public async Task<IActionResult> Index()
        {
            return View(await _context.Users.ToListAsync());
        }

        // GET: User/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _context.Users
                .FirstOrDefaultAsync(m => m.UserId == id);
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        // GET: User/Create
        public IActionResult Create()
        {
            return View();
        }

        /// <summary>
        /// 100データを作成します
        /// </summary>
        public async Task<IActionResult> Create100Data()
        {
            UserModel usermodel = new UserModel();

            Random ran = new Random();
            int rankPoints = 0;
            String letters = "abcdefghijklmnopqrstuvwxyz";
            int length = 8;
            int maxid = 0;
            String randomName = "";

            for (int j = 0; j < 100; j++)
            {
                rankPoints = ran.Next(100);
                randomName = "";
                var rec = _context.Users.FirstOrDefault();

                if (rec == null)
                {
                    maxid = 0;
                }
                else 
                { 
                    maxid = _context.Users.Max(p => p.UserId); 
                }
                for (int i = 0; i < length; i++)
                {
                    int c = ran.Next(26);
                    randomName = randomName + letters.ElementAt(c);
                }

                usermodel.UserId = maxid + 1;
                usermodel.RankPoints = rankPoints;
                usermodel.UserName = randomName;

                _context.Add(usermodel);
                _context.Database.OpenConnection();
                try
                {
                    _context.Database.ExecuteSqlRaw("SET IDENTITY_INSERT dbo.Users ON");
                    _context.SaveChanges();
                    _context.Database.ExecuteSqlRaw("SET IDENTITY_INSERT dbo.Users OFF");
                }
                finally
                {
                    _context.Database.CloseConnection();
                }

            }
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        /// <summary>
        /// データベース内のすべてのデータを削除します
        /// </summary>
        /// <returns>同じページにリダイレクトする</returns>
        public ActionResult DeleteAllData() {
            _context.Database.ExecuteSqlRaw("truncate Table Users;");
            return RedirectToAction("Index");

        }

        /// <summary>
        /// ランクポイントが50未満のユーザーの場合、ランクポイントを0に更新します
        /// </summary>
        /// <returns>同じページにリダイレクトする</returns>
        public ActionResult RemoveRankPoints() {
            
            _context.Database.ExecuteSqlRaw("UPDATE Users SET RankPoints=0 WHERE RankPoints<=50;");
            return RedirectToAction("Index");
        }

        /// <summary>
        /// すべてのユーザーデータをランクポイントで並べ替えます
        /// </summary>
        /// <returns>トップ10ユーザーの新しいビュー</returns>
        public ActionResult SortUserData()
        {
            IEnumerable<UserModel> userList = _context.Users.ToList();

            var result = userList.OrderByDescending(e => e.RankPoints).ThenBy(e => e.UserName).Take(10);
           
            return View(result);
        }

        // POST: User/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("UserId,UserName,RankPoints")] UserModel user)
        {
            if (ModelState.IsValid)
            {
                _context.Add(user);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(user);
        }

        // GET: User/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            return View(user);
        }

        // POST: User/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("UserId,UserName,RankPoints")] UserModel user)
        {
            if (id != user.UserId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(user);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UserExists(user.UserId))
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
            return View(user);
        }

        // GET: User/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _context.Users
                .FirstOrDefaultAsync(m => m.UserId == id);
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        // POST: User/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var user = await _context.Users.FindAsync(id);
            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool UserExists(int id)
        {
            return _context.Users.Any(e => e.UserId == id);
        }
    }
}
