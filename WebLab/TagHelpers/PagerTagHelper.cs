using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Razor.TagHelpers;
using WebLab.Domain.Entities;

namespace WebLab.TagHelpers
{
    [HtmlTargetElement("Pager")]
    public class PagerTagHelper : TagHelper
    {
        private readonly LinkGenerator _linkGenerator;
        private readonly HttpContext _httpContext;

        public PagerTagHelper(LinkGenerator linkGenerator, IHttpContextAccessor httpContextAccessor)
        {
            _linkGenerator = linkGenerator;
            _httpContext = httpContextAccessor.HttpContext!;
        }

        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
        public string? BeerType { get; set; } = null;
        public bool Admin { get; set; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {

            TagBuilder result = new TagBuilder("ul");
            result.AddCssClass("pagination justify-content-center");

            var prevPageNo = Math.Max(CurrentPage - 1, 1);
            var liPrev = GenerateListItem(prevPageNo, "«");
            if (CurrentPage == 1)
            {
                liPrev.AddCssClass("disabled");
            }
            result.InnerHtml.AppendHtml(liPrev);

            for (int pageNo = 1; pageNo <= TotalPages; pageNo++)
            {
                TagBuilder li = GenerateListItem(pageNo, pageNo.ToString());
                if (pageNo == CurrentPage)
                {
                    li.AddCssClass("active");
                }
                result.InnerHtml.AppendHtml(li);
            }

            var nextPageNo = Math.Max(CurrentPage + 1, TotalPages);
            var liNext = GenerateListItem(nextPageNo, "»");
            if (CurrentPage == TotalPages)
            {
                liNext.AddCssClass("disabled");
            }

            result.InnerHtml.AppendHtml(liNext);
            output.Content.AppendHtml(result);
        }

        private TagBuilder GenerateListItem(int i, string innerText)
        {
            var li = new TagBuilder("li");
            li.AddCssClass("page-item");
            TagBuilder a = GenerateLink(i, innerText);
            li.InnerHtml.AppendHtml(a);
            return li;
        }

        private TagBuilder GenerateLink(int pageNo, string innerText)
        {
            var a = new TagBuilder("a");
            a.AddCssClass("page-link");
            a.MergeAttribute("href", GetUrlByPageNumber(pageNo));
            a.InnerHtml.Append(innerText);
            return a;
        }

        private string? GetUrlByPageNumber(int pageNo)
        {
            return Admin
                            ? _linkGenerator.GetPathByPage(_httpContext, page: "/index", values: new { area = "Admin", pageNo })
                            : _linkGenerator.GetUriByAction(_httpContext, "index", "Beer", new { pageNo, beerType=BeerType });
        }
    }
}
