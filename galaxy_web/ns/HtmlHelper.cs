using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using System.Web.Mvc;
using System.Web.Routing;
using System.Text;

using System.Web.Helpers;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;

namespace System.Web.Mvc.Html { 


public static partial class CheckBoxListExtensions
{
    #region CheckBoxList
     public static MvcHtmlString CheckBoxList(this HtmlHelper htmlHelper, string name, List<ListItem> listInfo)
    {
        return htmlHelper.CheckBoxList
        (
            name,
            listInfo,
            (IDictionary<string, object>)null,
            0
        );
    }
    public static MvcHtmlString CheckBoxList(this HtmlHelper htmlHelper, string name, List<ListItem> listInfo, object htmlAttributes)
    {
        return htmlHelper.CheckBoxList
        (
            name,
            listInfo,
            (IDictionary<string, object>)new RouteValueDictionary(htmlAttributes),
            0
        );
    }
     public static MvcHtmlString CheckBoxList(this HtmlHelper htmlHelper, string name, List<ListItem> listInfo, IDictionary<string, object> htmlAttributes)
    {
        if (String.IsNullOrEmpty(name))
        {
            throw new ArgumentException("必須給這些CheckBoxList一個Tag Name", "name");
        }
        if (listInfo == null)
        {
            throw new ArgumentNullException("必須要給List<ListItem> listInfo");
        }
        if (listInfo.Count < 1)
        {
            throw new ArgumentException("List<ListItem> listInfo 至少要有一組資料", "listInfo");
        }
        StringBuilder sb = new StringBuilder();
        int lineNumber = 0;
        foreach (ListItem info in listInfo)
        {
            lineNumber++;
            TagBuilder builder = new TagBuilder("input");
            if (info.IsChecked)
            {
                builder.MergeAttribute("checked", "checked");
            }
            builder.MergeAttributes<string, object>(htmlAttributes);
            builder.MergeAttribute("type", "checkbox");
            builder.MergeAttribute("value", info.Value);
            builder.MergeAttribute("name", name);
            builder.InnerHtml = string.Format(" {0} ", info.DisplayText);
            sb.Append(builder.ToString(TagRenderMode.Normal));
            sb.Append("</br>");
        }
        return MvcHtmlString.Create(sb.ToString());
    }
    public static MvcHtmlString CheckBoxList(this HtmlHelper htmlHelper, string name, List<ListItem> listInfo, IDictionary<string, object> htmlAttributes, int number)
    {
        if (String.IsNullOrEmpty(name))
        {
            throw new ArgumentException("必須給這些CheckBoxList一個Tag Name", "name");
        }
        if (listInfo == null)
        {
            throw new ArgumentNullException("必須要給List<CheckBoxListInfo> listInfo");
        }
        if (listInfo.Count < 1)
        {
            throw new ArgumentException("List<CheckBoxListInfo> listInfo 至少要有一組資料", "listInfo");
        }
        StringBuilder sb = new StringBuilder();
        int lineNumber = 0;
        foreach (ListItem info in listInfo)
        {
            lineNumber++;
            TagBuilder builder = new TagBuilder("input");
            if (info.IsChecked)
            {
                builder.MergeAttribute("checked", "checked");
            }
            builder.MergeAttributes<string, object>(htmlAttributes);
            builder.MergeAttribute("type", "checkbox");
            builder.MergeAttribute("value", info.Value);
            builder.MergeAttribute("name", name);
            builder.InnerHtml = string.Format(" {0} ", info.DisplayText);
            sb.Append(builder.ToString(TagRenderMode.Normal));
            if (number == 0)
            {
                //sb.Append("<br />");
            }
            else if (lineNumber % number == 0)
            {
                sb.Append("<br />");
            }
        }
        return MvcHtmlString.Create(sb.ToString());
    }
    #endregion
}

public static partial class RadioBoxListExtensions
{
    #region RadioBoxList
    public static MvcHtmlString RadioBoxList(this HtmlHelper htmlHelper, string name, List<ListItem> listInfo)
    {
        return htmlHelper.RadioBoxList
        (
            name,
            listInfo,
            (IDictionary<string, object>)null,
            0
        );
    }
    public static MvcHtmlString RadioBoxList(this HtmlHelper htmlHelper, string name, List<ListItem> listInfo, object htmlAttributes)
    {
        return htmlHelper.RadioBoxList
        (
            name,
            listInfo,
            (IDictionary<string, object>)new RouteValueDictionary(htmlAttributes),
            0
        );
    }
    public static MvcHtmlString RadioBoxList(this HtmlHelper htmlHelper, string name, List<ListItem> listInfo, IDictionary<string, object> htmlAttributes)
    {
        if (String.IsNullOrEmpty(name))
        {
            throw new ArgumentException("必須給這些RadioBoxList一個Tag Name", "name");
        }
        if (listInfo == null)
        {
            throw new ArgumentNullException("必須要給List<ListItem> listInfo");
        }
        if (listInfo.Count < 1)
        {
            throw new ArgumentException("List<ListItem> listInfo 至少要有一組資料", "listInfo");
        }
        StringBuilder sb = new StringBuilder();
        int lineNumber = 0;
        foreach (ListItem info in listInfo)
        {
            lineNumber++;
            TagBuilder builder = new TagBuilder("input");
            if (info.IsChecked)
            {
                builder.MergeAttribute("checked", "checked");
            }
            builder.MergeAttributes<string, object>(htmlAttributes);
            builder.MergeAttribute("type", "radio");
            builder.MergeAttribute("value", info.Value);
            builder.MergeAttribute("name", name);
            builder.InnerHtml = string.Format(" {0} ", info.DisplayText);
            sb.Append(builder.ToString(TagRenderMode.Normal));
            sb.Append("</br>");
        }
        return MvcHtmlString.Create(sb.ToString());
    }
    public static MvcHtmlString RadioBoxList(this HtmlHelper htmlHelper, string name, List<ListItem> listInfo, IDictionary<string, object> htmlAttributes, int number)
    {
        if (String.IsNullOrEmpty(name))
        {
            throw new ArgumentException("必須給這些CheckBoxList一個Tag Name", "name");
        }
        if (listInfo == null)
        {
            throw new ArgumentNullException("必須要給List<CheckBoxListInfo> listInfo");
        }
        if (listInfo.Count < 1)
        {
            throw new ArgumentException("List<CheckBoxListInfo> listInfo 至少要有一組資料", "listInfo");
        }
        StringBuilder sb = new StringBuilder();
        int lineNumber = 0;
        foreach (ListItem info in listInfo)
        {
            lineNumber++;
            TagBuilder builder = new TagBuilder("input");
            if (info.IsChecked)
            {
                builder.MergeAttribute("checked", "checked");
            }
            builder.MergeAttributes<string, object>(htmlAttributes);
            builder.MergeAttribute("type", "radio");
            builder.MergeAttribute("value", info.Value);
            builder.MergeAttribute("name", name);
            builder.InnerHtml = string.Format(" {0} ", info.DisplayText);
            sb.Append(builder.ToString(TagRenderMode.Normal));
            if (number == 0)
            {
                //sb.Append("<br />");
            }
            else if (lineNumber % number == 0)
            {
                sb.Append("<br />");
            }
        }
        return MvcHtmlString.Create(sb.ToString());
    }
    #endregion
}

public static partial class SelectListExtensions
{
    #region Select
    public static MvcHtmlString SelectList(this HtmlHelper htmlHelper, string name, List<ListItem> listInfo)
    {
        return htmlHelper.SelectList
        (
            name,
            listInfo,
            (IDictionary<string, object>)null
        );
    }
    public static MvcHtmlString SelectList(this HtmlHelper htmlHelper, string name, List<ListItem> listInfo, object htmlAttributes)
    {
        return htmlHelper.SelectList
        (
            name,
            listInfo,
            (IDictionary<string, object>)new RouteValueDictionary(htmlAttributes)
        );
    }
    public static MvcHtmlString SelectList(this HtmlHelper htmlHelper, string name, List<ListItem> listInfo, IDictionary<string, object> htmlAttributes)
    {
        //if (!htmlAttributes.HasValue)
        //{
        //    htmlAttributes = new Dictionary<string, object>();
        //    htmlAttributes.Value.Add("class", "span2");
        //}

        if (String.IsNullOrEmpty(name))
        {
            throw new ArgumentException("必須給這些SelectList一個Tag Name", "name");
        }
        if (listInfo == null)
        {
            throw new ArgumentNullException("必須要給List<ListItem> listInfo");
        }
        if (listInfo.Count < 1)
        {
            throw new ArgumentException("List<ListItem> listInfo 至少要有一組資料", "listInfo");
        }
        StringBuilder sb = new StringBuilder();
        TagBuilder builder = new TagBuilder("Select");
        builder.MergeAttributes<string, object>(htmlAttributes);
        //builder.MergeAttribute("type", "radio");
        builder.MergeAttribute("name", name);
        builder.MergeAttribute("id", name);
        foreach (ListItem info in listInfo)
        {
            TagBuilder builder2 = new TagBuilder("option");
            builder2.MergeAttribute("value", info.Value);
            builder2.InnerHtml = string.Format(" {0} ", info.DisplayText);
           if (info.IsChecked)
            {
                builder2.MergeAttribute("selected", "selected");
            }
           builder.InnerHtml+=builder2.ToString(TagRenderMode.Normal);
        }
        sb.Append(builder.ToString(TagRenderMode.Normal));
        return MvcHtmlString.Create(sb.ToString());
    }
    #endregion
}
}

public static partial class SelectListChainedExtensions
{
    #region SelectListChained
    public static MvcHtmlString SelectListChained(this HtmlHelper htmlHelper, string name, List<ListItem> listInfoChained, List<ListItem> listInfo)
    {
        return htmlHelper.SelectListChained
        (
            name,
            listInfo,
            listInfoChained,
            (IDictionary<string, object>)null
        );
    }
    public static MvcHtmlString SelectListChained(this HtmlHelper htmlHelper, string name, List<ListItem> listInfoChained, List<ListItem> listInfo, object htmlAttributes)
    {
        return htmlHelper.SelectListChained
        (
            name,
            listInfo,
            listInfoChained,
            (IDictionary<string, object>)new RouteValueDictionary(htmlAttributes)
        );
    }
    public static MvcHtmlString SelectListChained(this HtmlHelper htmlHelper, string name, List<ListItem> listInfoChained, List<ListItem> listInfo, IDictionary<string, object> htmlAttributes)
    {
        if (String.IsNullOrEmpty(name))
        {
            throw new ArgumentException("必須給這些SelectList一個Tag Name", "name");
        }
        if (listInfo == null)
        {
            throw new ArgumentNullException("必須要給List<ListItem> listInfo");
        }
        if (listInfo.Count < 1)
        {
            throw new ArgumentException("List<ListItem> listInfo 至少要有一組資料", "listInfo");
        }

        StringBuilder sb = new StringBuilder();
        //第一層
        TagBuilder builder_chained = new TagBuilder("Select");
        builder_chained.MergeAttributes<string, object>(htmlAttributes);
        //builder.MergeAttribute("type", "radio");
        builder_chained.MergeAttribute("name", name + "_chained");
        builder_chained.MergeAttribute("id", name + "_chained");
        foreach (ListItem info in listInfoChained)
        {
            TagBuilder builder_chained2 = new TagBuilder("option");
            builder_chained2.MergeAttribute("value", info.Value);
            builder_chained2.InnerHtml = string.Format(" {0} ", info.DisplayText);
            if (info.IsChecked)
            {
                builder_chained2.MergeAttribute("selected", "selected");
            }
            builder_chained.InnerHtml += builder_chained2.ToString(TagRenderMode.Normal);
        }
        sb.Append(builder_chained.ToString(TagRenderMode.Normal));

        sb.Append("&nbsp;");

        //第二層
        TagBuilder builder = new TagBuilder("Select");
        builder.MergeAttributes<string, object>(htmlAttributes);
        //builder.MergeAttribute("type", "radio");
        builder.MergeAttribute("name", name);
        builder.MergeAttribute("id", name);
        foreach (ListItem info in listInfo)
        {
            TagBuilder builder2 = new TagBuilder("option");
            builder2.MergeAttribute("value", info.Value);
            builder2.InnerHtml = string.Format(" {0} ", info.DisplayText);
            builder2.MergeAttribute("class", info.className);
           if (info.IsChecked)
            {
                builder2.MergeAttribute("selected", "selected");
            }
           builder.InnerHtml+=builder2.ToString(TagRenderMode.Normal);
        }
        sb.Append(builder.ToString(TagRenderMode.Normal));

        //第一層
        //StringBuilder sb_chained = new StringBuilder();

        return MvcHtmlString.Create(sb.ToString());
    }
    #endregion
}

public static partial class SelectListChained3Extensions
{
    #region SelectListChained3
    public static MvcHtmlString SelectListChained3(this HtmlHelper htmlHelper, string name, List<ListItem> listInfoChained, List<ListItem> listInfoChained2, List<ListItem> listInfo)
    {
        return htmlHelper.SelectListChained3
        (
            name,
            listInfo,
            listInfoChained,
            listInfoChained2,
            (IDictionary<string, object>)null
        );
    }
    public static MvcHtmlString SelectListChained3(this HtmlHelper htmlHelper, string name, List<ListItem> listInfoChained, List<ListItem> listInfoChained2, List<ListItem> listInfo, object htmlAttributes)
    {
        return htmlHelper.SelectListChained3
        (
            name,
            listInfo,
            listInfoChained,
            listInfoChained2,
            (IDictionary<string, object>)new RouteValueDictionary(htmlAttributes)
        );
    }
    public static MvcHtmlString SelectListChained3(this HtmlHelper htmlHelper, string name, List<ListItem> listInfoChained, List<ListItem> listInfoChained2, List<ListItem> listInfo, IDictionary<string, object> htmlAttributes)
    {
        if (String.IsNullOrEmpty(name))
        {
            throw new ArgumentException("必須給這些SelectList一個Tag Name", "name");
        }
        if (listInfo == null)
        {
            throw new ArgumentNullException("必須要給List<ListItem> listInfo");
        }
        if (listInfo.Count < 1)
        {
            throw new ArgumentException("List<ListItem> listInfo 至少要有一組資料", "listInfo");
        }

        StringBuilder sb = new StringBuilder();
        //第一層
        TagBuilder builder_chained = new TagBuilder("Select");
        builder_chained.MergeAttributes<string, object>(htmlAttributes);
        //builder.MergeAttribute("type", "radio");
        builder_chained.MergeAttribute("name", name + "_chained");
        builder_chained.MergeAttribute("id", name + "_chained");
        foreach (ListItem info in listInfoChained)
        {
            TagBuilder builder_chained2 = new TagBuilder("option");
            builder_chained2.MergeAttribute("value", info.Value);
            builder_chained2.InnerHtml = string.Format(" {0} ", info.DisplayText);
            if (info.IsChecked)
            {
                builder_chained2.MergeAttribute("selected", "selected");
            }
            builder_chained.InnerHtml += builder_chained2.ToString(TagRenderMode.Normal);
        }
        sb.Append(builder_chained.ToString(TagRenderMode.Normal));

        sb.Append("&nbsp;");

        //第二層
        TagBuilder builder2_chained = new TagBuilder("Select");
        builder2_chained.MergeAttributes<string, object>(htmlAttributes);
        //builder2_chained.MergeAttribute("type", "radio");
        builder2_chained.MergeAttribute("name", name + "_chained2");
        builder2_chained.MergeAttribute("id", name + "_chained2");
        foreach (ListItem info in listInfoChained2)
        {
            TagBuilder builder2_chained2 = new TagBuilder("option");
            builder2_chained2.MergeAttribute("value", info.Value);
            builder2_chained2.InnerHtml = string.Format(" {0} ", info.DisplayText);
            builder2_chained2.MergeAttribute("class", info.className);
            if (info.IsChecked)
            {
                builder2_chained2.MergeAttribute("selected", "selected");
            }
            builder2_chained.InnerHtml += builder2_chained2.ToString(TagRenderMode.Normal);
        }
        sb.Append(builder2_chained.ToString(TagRenderMode.Normal));

        sb.Append("&nbsp;");

        //第三層
        TagBuilder builder = new TagBuilder("Select");
        builder.MergeAttributes<string, object>(htmlAttributes);
        //builder.MergeAttribute("type", "radio");
        builder.MergeAttribute("name", name);
        builder.MergeAttribute("id", name);
        foreach (ListItem info in listInfo)
        {
            TagBuilder builder2 = new TagBuilder("option");
            builder2.MergeAttribute("value", info.Value);
            builder2.InnerHtml = string.Format(" {0} ", info.DisplayText);
            builder2.MergeAttribute("class", info.className);
            if (info.IsChecked)
            {
                builder2.MergeAttribute("selected", "selected");
            }
            builder.InnerHtml += builder2.ToString(TagRenderMode.Normal);
        }
        sb.Append(builder.ToString(TagRenderMode.Normal));


        return MvcHtmlString.Create(sb.ToString());
    }
    #endregion
}



public class ListItem
{
    public string Value { get; private set; }
    public string DisplayText { get; private set; }
    public bool IsChecked { get; private set; }
    public string className { get; private set; }
    public ListItem(string value, string displayText, bool isChecked)
    {

        this.Value = value;
        this.DisplayText = displayText;
        this.IsChecked = isChecked;
    }
    public ListItem(string value, string displayText, bool isChecked, string className)
    {
        this.Value = value;
        this.DisplayText = displayText;
        this.IsChecked = isChecked;
        this.className = className;
    }
}

//#region "圖片縮圖"

//namespace Images.Helps
//{
//    public class ImageHelper
//    {
//        public static string Rebmp(string strFileName, string strFilePath, string strFilePath2, int resizedWidth, int resizedHeight)
//        {

//            //預設要縮圖的寬度與高度
//            // resizedWidth = 400;
//            //resizedHeight = 300;

//            //使用Helper判斷比例
//            int[] img = ImageHelper.ReImageSize(strFilePath, resizedWidth, resizedHeight);

//            //取得新的圖片寬度與高度
//            resizedWidth = img[0];
//            resizedHeight = img[1];

//            //填入要縮小的寬度與高度
//            Bitmap bmp = new Bitmap(resizedWidth, resizedHeight);

//            //將bimap轉為graphic來操作
//            System.Drawing.Graphics gr = System.Drawing.Graphics.FromImage(bmp);

//            //設定縮圖的品質
//            gr.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
//            gr.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
//            gr.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.High;

//            //以原始的image為來源，繪製成rectDestination的大小(0,0,resizedWidth,resizedHeight為新的大小座標)
//            System.Drawing.Rectangle rectDestination = new System.Drawing.Rectangle(0, 0, resizedWidth, resizedHeight);

//            //使用Image
//            Image newImage = Image.FromFile(strFilePath);

//            //Image、座標、原圖寬度，原圖高或，Pixel
//            gr.DrawImage(newImage, rectDestination, 0, 0, newImage.Width, newImage.Height, GraphicsUnit.Pixel);

//            //儲存並命名新檔
//            bmp.Save(strFilePath2);

//            //釋放資源
//            bmp.Dispose();
//            gr.Dispose();
//            newImage.Dispose();

//            //刪除原檔
//            System.IO.File.Delete(strFilePath);

//            return strFileName.ToString();
//        }


//        public static int[] ReImageSize(string buffer, int maxWidth, int maxHeight)
//        {
//            //WebImage img = WebImage.GetImageFromRequest();

//            Image img = Image.FromFile(buffer);

//            int[] newWH = ReSize(img.Width, img.Height, maxWidth, maxHeight);
//            img.Dispose();
//            return newWH;

//        }


//        public static int[] ReSize(int width, int height, int maxWidth, int maxHeight)
//        {
//            int[] newWH = new[] { width, height };
//            int rWidth, rHeight;

//            if (width > height && width > maxWidth)//水平圖
//            {
//                rWidth = maxWidth;
//                //rHeight = height * rWidth / width;
//                rHeight = maxHeight;
//                newWH[0] = rWidth;
//                newWH[1] = rHeight;
//            }
//            else if (height > width && height > maxHeight)//垂直圖
//            {
//                rHeight = maxHeight;
//                rWidth = width * rHeight / height;
//                newWH[0] = rWidth;
//                newWH[1] = rHeight;
//            }
//            else if (height == width && (width > maxWidth || height > maxHeight))//正方圖
//            {

//                if (maxWidth > maxHeight)
//                {
//                    newWH[0] = newWH[1] = maxHeight;
//                }
//                else if (maxHeight > maxWidth)
//                {
//                    newWH[0] = newWH[1] = maxWidth;
//                }
//                else
//                {
//                    newWH[0] = newWH[1] = maxWidth;
//                }

//            }

//            return newWH;
//        }

//    }
//}

//#endregion

namespace Images.Helps
{
    public class ImageHelper
    {
        //public static string Rebmp(string strFileName, string strFilePath, string strFilePath2, int resizedWidth, int resizedHeight)
        //轉圖 欲轉檔的圖檔、存放位置、寬度、長度
        public static string Rebmp(HttpPostedFileBase Img, string tmpPath, int resizedWidth, int resizedHeight, string strFilePath3 = null)
        {

            tmpPath = !string.IsNullOrEmpty(tmpPath) ? tmpPath : "~/Content/images/!Public"; //存放位置
            if (!Directory.Exists(tmpPath)) { Directory.CreateDirectory(tmpPath); } //建立資料夾

            string water = NW.getImgWater();
            string strFileName = water + "1" + Path.GetExtension(Img.FileName); //新的檔案名稱
            string strFilePath = tmpPath + "/" + Img.FileName; //存放檔案路徑
            string strFilePath2 = tmpPath + "/" + strFileName; //存放檔案路徑

            if (!string.IsNullOrEmpty(strFilePath3)) //如果有指定其他路徑
            {
                strFilePath2 = strFilePath3;
            }

            if (strFileName.Contains(".gif"))
            {
                Img.SaveAs(strFilePath2); //檔案存放在Server-新檔名
            }
            else
            {

                //預設要縮圖的寬度與高度
                // resizedWidth = 400;
                //resizedHeight = 300;

                Img.SaveAs(strFilePath);

                //使用Image
                Image newImage = Image.FromFile(strFilePath);

                decimal old_width = newImage.Width; //原圖的寬
                decimal old_hight = newImage.Height; //原圖的高

                if (resizedHeight == 0) //限寬不限高的時候
                {
                    resizedHeight = (int)Math.Round((resizedWidth / old_width) * old_hight, 0);
                }


                //使用Helper判斷比例
                int[] img = ImageHelper.ReImageSize(strFilePath, resizedWidth, resizedHeight);

                //取得新的圖片寬度與高度
                resizedWidth = img[0];
                resizedHeight = img[1];

                //填入要縮小的寬度與高度
                Bitmap bmp = new Bitmap(resizedWidth, resizedHeight);

                //將bimap轉為graphic來操作
                System.Drawing.Graphics gr = System.Drawing.Graphics.FromImage(bmp);

                //設定縮圖的品質
                gr.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
                gr.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
                gr.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.High;

                //以原始的image為來源，繪製成rectDestination的大小(0,0,resizedWidth,resizedHeight為新的大小座標)
                System.Drawing.Rectangle rectDestination = new System.Drawing.Rectangle(0, 0, resizedWidth, resizedHeight);

                //Image、座標、原圖寬度，原圖高或，Pixel
                gr.DrawImage(newImage, rectDestination, 0, 0, newImage.Width, newImage.Height, GraphicsUnit.Pixel);

                //儲存並命名新檔
                bmp.Save(strFilePath2);

                //釋放資源
                bmp.Dispose();
                gr.Dispose();
                newImage.Dispose();

            }

            //刪除原檔
            System.IO.File.Delete(strFilePath);

            return strFileName.ToString();
        }


        public static int[] ReImageSize(string buffer, int maxWidth, int maxHeight)
        {
            //WebImage img = WebImage.GetImageFromRequest();

            Image img = Image.FromFile(buffer);

            int[] newWH = ReSize(img.Width, img.Height, maxWidth, maxHeight);
            img.Dispose();
            return newWH;

        }


        public static int[] ReSize(int width, int height, int maxWidth, int maxHeight)
        {
            int[] newWH = new[] { width, height };
            int rWidth, rHeight;

            if (width > height && width > maxWidth)//水平圖
            {
                rWidth = maxWidth;
                rHeight = height * rWidth / width;
                newWH[0] = rWidth;
                newWH[1] = rHeight;
            }
            else if (height > width && height > maxHeight)//垂直圖
            {
                rHeight = maxHeight;
                rWidth = width * rHeight / height;
                newWH[0] = rWidth;
                newWH[1] = rHeight;
            }
            else if (height == width && (width > maxWidth || height > maxHeight))//正方圖
            {

                if (maxWidth > maxHeight)
                {
                    newWH[0] = newWH[1] = maxHeight;
                }
                else if (maxHeight > maxWidth)
                {
                    newWH[0] = newWH[1] = maxWidth;
                }
                else
                {
                    newWH[0] = newWH[1] = maxWidth;
                }

            }

            return newWH;
        }

    }
}

#region "社群分享"
public enum ShareFlag : int { FBLike = 1, Line, WeiBo, Google, Plurk, Twitter, Line_Computer };

public static partial class ShareLikeExtensions
{
    #region "Facebook"

    //新版
    public static MvcHtmlString Share(this HtmlHelper helper, ShareFlag shareFlag, string title, string url, string innerHtml, int? imgFlag)
    {
        TagBuilder builder = new TagBuilder("a");
        string href = "", html = "", imgName = "";

        //分享
        switch (shareFlag)
        {
            case ShareFlag.FBLike:
                href = "http://www.facebook.com/share.php?u=" + url; imgName = "facebook";
                break;
            case ShareFlag.Google:
                href = "https://plus.google.com/share?url=" + url; imgName = "google";
                break;
            case ShareFlag.Line:
                href = "line://msg/text/" + title + url; imgName = "line";
                break;
            case ShareFlag.Plurk:
                href = "http://plurk.com/?qualifier=shares&status=" + url + " " + "(" + title + ")"; imgName = "plurk";
                break;
            case ShareFlag.Twitter:
                href = "http://twitter.com/home/?status=" + title + " " + url; imgName = "twitter";
                break;
            case ShareFlag.WeiBo:
                href = "http://v.t.sina.com.cn/share/share.php?title=" + title + "&url=" + url; imgName = "weibo";
                break;
            case ShareFlag.Line_Computer:
                href = "http://line.naver.jp/R/msg/text/?" + title + "%0D%0A" + url; imgName = "line";
                break;
        }

        if (innerHtml != "")
        {
            html = innerHtml;
        }
        else
        {
            if (imgFlag.HasValue)
            {
                imgName += imgFlag.ToString();
            }
            html = "<img title=\"分享到" + imgName + "\" src=\"images/share/" + imgName + ".png\" border=\"0\" />";
        }

        builder.MergeAttribute("target", "_blank");
        builder.MergeAttribute("class", "nwShare");
        builder.MergeAttribute("href", href);
        builder.InnerHtml = html;

        return MvcHtmlString.Create(builder.ToString());
    }

    public static MvcHtmlString Share(this HtmlHelper helper, ShareFlag shareFlag, string url)
    {

        return ShareLikeExtensions.Share
      (
         helper, shareFlag, "", url, "", null
      );
    }

    public static MvcHtmlString Share(this HtmlHelper helper, ShareFlag shareFlag, string title, string url)
    {

        return ShareLikeExtensions.Share
      (
         helper, shareFlag, title, url, "", null
      );
    }

    public static MvcHtmlString Share(this HtmlHelper helper, ShareFlag shareFlag, string title, string url, string innerHtml)
    {

        return ShareLikeExtensions.Share
      (
         helper, shareFlag, title, url, innerHtml, null
      );
    }

    public static MvcHtmlString Share(this HtmlHelper helper, ShareFlag shareFlag, string title, string url, int imgFlag)
    {

        return ShareLikeExtensions.Share
      (
         helper, shareFlag, title, url, "", imgFlag
      );
    }


    //public static MvcHtmlString Share(this HtmlHelper helper, string openShare, string shareAddress, string shareTitle, string imgFlag)
    //{

    //    StringBuilder twitter = new StringBuilder();
    //    StringBuilder weibo = new StringBuilder();
    //    StringBuilder plurk = new StringBuilder();
    //    StringBuilder google = new StringBuilder();
    //    StringBuilder fblike = new StringBuilder();
    //    StringBuilder Line = new StringBuilder();


    //    string[] open;

    //    if (!string.IsNullOrEmpty(openShare))
    //    {
    //        open = openShare.Split(' ');
    //    }
    //    else
    //    {
    //        open = null;
    //    }

    //    if (open == null)
    //    {


    //        //Twitter
    //        twitter.Append("<a target=\"_blank\" href=\"http://twitter.com/home/?status=" + shareTitle + " " + shareAddress + "\">");
    //        twitter.Append("<img title=\"分享到Twitter\" src=\"images/share/twitter" + imgFlag + ".png\" border=\"0\" />");
    //        twitter.Append("</a>");

    //        //twitter.Append("<a  class=\"twitter-share-button\" data-url=\"www.google.com\" data-text=\"WOW www.google.com\" data-lang=\"zh-tw\" data-count=\"none\">");
    //        //twitter.Append("</a>");
    //        //twitter.Append("<script>");
    //        //twitter.Append("!function (d, s, id) { var js, fjs = d.getElementsByTagName(s)[0], p = /^http:/.test(d.location) ? 'http' : 'https'; if (!d.getElementById(id)) { js = d.createElement(s); js.id = id; js.src = p + '://platform.twitter.com/widgets.js'; fjs.parentNode.insertBefore(js, fjs); } }(document, 'script', 'twitter-wjs');");
    //        //twitter.Append("</script>");

    //        //Weibo
    //        weibo.Append("<a target=\"_blank\" href=\"http://v.t.sina.com.cn/share/share.php?title=" + shareTitle + "&url=" + shareAddress + "\">");
    //        weibo.Append("<img title=\"分享到Weibo\" src=\"images/share/weibo" + imgFlag + ".jpg\" border=\"0\" />");
    //        weibo.Append("</a>");

    //        //plurk
    //        plurk.Append("<a target=\"_blank\" href=\"http://plurk.com/?qualifier=shares&status=" + shareAddress + " " + "(" + shareTitle + ")" + "\">");
    //        plurk.Append("<img title=\"分享到Plurk\" src=\"images/share/plurk" + imgFlag + ".png\" border=\"0\" />");
    //        plurk.Append("</a>");

    //        //Google+
    //        //google.Append("<div class=\"g-plus\" data-action=\"share\" data-annotation=\"none\" data-href=\"http://www.mentor.org.tw/%E6%B5%B7%E5%A4%96%E6%89%93%E5%B7%A5%E5%81%9C%E7%9C%8B%E8%81%BD%EF%BC%81%E5%AE%89%E5%BF%83%E5%87%BA%E5%9C%8B%E5%85%8D%E7%85%A9%E6%83%B1%EF%BC%81-News-245894.html\">");
    //        //google.Append("</div>");
    //        //google.Append("<script src=\"https://apis.google.com/js/platform.js\" async defer>");
    //        //google.Append("{lang: 'zh-TW'}");
    //        //google.Append("</script>");

    //        google.Append("<a target=\"_blank\" href=\"https://plus.google.com/share?url=" + shareAddress + "\">");
    //        google.Append("<img title=\"分享到Google+\" src=\"images/share/google" + imgFlag + ".png\" border=\"0\" />");
    //        google.Append("</a>");

    //        //facebook
    //        //fblike.Append("<div class=\"fb-like\" data-href=\"http://www.mentor.org.tw/%E6%B5%B7%E5%A4%96%E6%89%93%E5%B7%A5%E5%81%9C%E7%9C%8B%E8%81%BD%EF%BC%81%E5%AE%89%E5%BF%83%E5%87%BA%E5%9C%8B%E5%85%8D%E7%85%A9%E6%83%B1%EF%BC%81-News-245895.html\" data-layout=\"standard\" data-action=\"recommend\" data-show-faces=\"false\" data-share=\"false\">");
    //        //fblike.Append("</div>");
    //        //fblike.Append("<script>");
    //        //fblike.Append("(function (d, s, id) {var js, fjs = d.getElementsByTagName(s)[0];if (d.getElementById(id)) return;js = d.createElement(s); js.id = id;js.src = \"//connect.facebook.net/zh_TW/sdk.js#xfbml=1&version=v2.0\";fjs.parentNode.insertBefore(js, fjs);}(document, 'script', 'facebook-jssdk'));");
    //        //fblike.Append("</script>");

    //        fblike.Append("<a target=\"_blank\" href=\"http://www.facebook.com/share.php?u=" + shareAddress + "\">");
    //        fblike.Append("<img title=\"分享到Facebook\" src=\"images/share/facebook" + imgFlag + ".png\" border=\"0\" />");
    //        fblike.Append("</a>");

    //    }
    //    else
    //    {
    //        if (open[0] == "1")
    //        {
    //            //Twitter
    //            twitter.Append("<a target=\"_blank\" href=\"http://twitter.com/home/?status=" + shareTitle + " " + shareAddress + "\">");
    //            twitter.Append("<img title=\"分享到Twitter\" src=\"images/share/twitter" + imgFlag + ".png\" border=\"0\" />");
    //            twitter.Append("</a>");

    //        }

    //        if (open[1] == "1")
    //        {
    //            //Weibo
    //            weibo.Append("<a target=\"_blank\" href=\"http://v.t.sina.com.cn/share/share.php?title=" + shareTitle + "&url=" + shareAddress + "\">");
    //            weibo.Append("<img title=\"分享到Weibo\" src=\"images/share/weibo" + imgFlag + ".jpg\" border=\"0\" />");
    //            weibo.Append("</a>");
    //        }

    //        if (open[2] == "1")
    //        {
    //            //plurk
    //            plurk.Append("<a target=\"_blank\" href=\"" + shareAddress + " " + "(" + shareTitle + ")" + "\">");
    //            plurk.Append("<img title=\"分享到Plurk\" src=\"images/share/plurk" + imgFlag + ".png\" border=\"0\" />");
    //            plurk.Append("</a>");
    //        }

    //        if (open[3] == "1")
    //        {
    //            //Google+
    //            google.Append("<a target=\"_blank\" href=\"https://plus.google.com/share?url=" + shareAddress + "\">");
    //            google.Append("<img title=\"分享到Google+\" src=\"images/share/google" + imgFlag + ".png\" border=\"0\" />");
    //            google.Append("</a>");

    //        }

    //        if (open[4] == "1")
    //        {
    //            //facebook
    //            fblike.Append("<a target=\"_blank\" href=\"http://www.facebook.com/share.php?u=" + shareAddress + "\">");
    //            fblike.Append("<img title=\"分享到Facebook\" src=\"images/share/facebook" + imgFlag + ".png\" border=\"0\" />");
    //            fblike.Append("</a>");

    //        }

    //        if (open[5] == "1")
    //        {
    //            //facebook
    //            //Line.Append("<a target=\"_blank\" href=\"http://line.naver.jp/R/msg/text/?" + shareAddress + "\" rel=\"nofollow\" >");
    //            Line.Append("<a target=\"_blank\" href=\"line://msg/text/" + shareAddress + "\" rel=\"nofollow\" >");
    //            Line.Append("<img title=\"分享到Line\" src=\"images/share/line" + imgFlag + ".png\" border=\"0\" />");
    //            Line.Append("</a>");
    //        }
    //    }

    //    return MvcHtmlString.Create(twitter.ToString() + weibo.ToString() + plurk.ToString() + google.ToString() + fblike.ToString() + Line.ToString());

    //}

    public static MvcHtmlString FB_Comments(this HtmlHelper helper, string shareAddress)
    {

        //var builder = new TagBuilder("div");

        StringBuilder comments = new StringBuilder();
        comments.Append("<div class=\"fb-comments\" data-href=\"" + shareAddress + "\" data-colorscheme=\"light\" data-width=\"730px\">");
        comments.Append("</div>");

        StringBuilder builder2 = new StringBuilder();
        builder2.Append("<script>");
        builder2.Append("(function (d, s, id) {var js, fjs = d.getElementsByTagName(s)[0];if (d.getElementById(id)) return;js = d.createElement(s); js.id = id;js.src = \"//connect.facebook.net/zh_TW/sdk.js#xfbml=1&version=v2.0\";fjs.parentNode.insertBefore(js, fjs);}(document, 'script', 'facebook-jssdk'));");
        builder2.Append("</script>");



        return MvcHtmlString.Create(comments.ToString() + builder2.ToString());

    }

    public static MvcHtmlString fbMeta(this HtmlHelper helper, string shareAddress, string shareTitle, string shareCont, string shareImage)
    {
        StringBuilder builder = new StringBuilder();

        builder.Append("<meta property=\"og:title\" content=\"" + shareTitle + "\"/>");
        builder.Append("<meta property=\"og:description\" content=\"" + shareCont + "\"/>");
        builder.Append("<meta property=\"og:url\" content=\"" + shareAddress + "\"/>");
        builder.Append("<meta property=\"og:image\" content=\"" + shareImage + "\"/>");

        return MvcHtmlString.Create(builder.ToString());

    }

    #endregion
}
#endregion