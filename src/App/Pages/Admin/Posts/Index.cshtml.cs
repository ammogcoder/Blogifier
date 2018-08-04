﻿using Core.Data;
using Core.Helpers;
using Core.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace App.Pages.Admin.Posts
{
    public class IndexModel : AdminPageModel
    {
        [BindProperty]
        public IEnumerable<PostItem> Posts { get; set; }

        [BindProperty]
        public Pager Pager { get; set; }

        IUnitOfWork _db;
        ISearchService _ss;

        public IndexModel(IUnitOfWork db, ISearchService ss)
        {
            _db = db;
            _ss = ss;
            Pager = new Pager(1);
        }

        public async Task<IActionResult> OnGetAsync(int page = 1, string status = "A", string search = "")
        {
            Author = await _db.Authors.GetItem(a => a.AppUserName == User.Identity.Name);
            Expression<Func<BlogPost, bool>> predicate = p => p.AuthorId == Author.Id;
            Pager = new Pager(page);

            if (!string.IsNullOrEmpty(search))
            {
                Posts = await _ss.Find(Pager, search);
            }
            else
            {
                if (status == "P")
                    predicate = p => p.Published > DateTime.MinValue && p.AuthorId == Author.Id;
                if (status == "D")
                    predicate = p => p.Published == DateTime.MinValue && p.AuthorId == Author.Id;

                Posts = await _db.BlogPosts.Find(predicate, Pager);
            }
            return Page();
        }
    }
}