using Microsoft.AspNetCore.Razor.TagHelpers;

namespace IdentityApp.Web.TagHelpers
{
    public class UserPictureTagHelper:TagHelper
    {
        public string? PictreUrl { get; set; }
        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            output.TagName = "img";
            if (string.IsNullOrEmpty(PictreUrl))
            {
                output.Attributes.SetAttribute("src", "/UserPictures/defaultpp.jpeg");
            }
            else
            {
                output.Attributes.SetAttribute("src", $"/UserPictures/{PictreUrl}");
            }
        }
    }
}
