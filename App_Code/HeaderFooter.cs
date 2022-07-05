using System;
using iTextSharp.text.pdf;
using iTextSharp.text;
namespace PDF_Tests
{
    public class HeaderFooter : PdfPageEventHelper
    {
        // This is the contentbyte object of the writer
        PdfContentByte cb;
        // we will put the final number of pages in a template
        PdfTemplate template;
        // this is the BaseFont we are going to use for the header / footer
        BaseFont bf = null;
        // This keeps track of the creation time
        DateTime PrintTime = DateTime.Now;
        #region Properties
        //private string _Title;
        //public string Title
        //{
        //    get { return _Title; }
        //    set { _Title = value; }
        //}

        //private string _HeaderLeft;
        //public string HeaderLeft
        //{
        //    get { return _HeaderLeft; }
        //    set { _HeaderLeft = value; }
        //}
        //private string _HeaderRight;
        //public string HeaderRight
        //{
        //    get { return _HeaderRight; }
        //    set { _HeaderRight = value; }
        //}
        //private Font _HeaderFont;
        //public Font HeaderFont
        //{
        //    get { return _HeaderFont; }
        //    set { _HeaderFont = value; }
        //}
        private Font _FooterFont;
        public Font FooterFont
        {
            get { return _FooterFont; }
            set { _FooterFont = value; }
        }
        #endregion
        // we override the onOpenDocument method
        public override void OnOpenDocument(PdfWriter writer, Document document)
        {
            PrintTime = DateTime.Now;
            bf = BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1252, BaseFont.NOT_EMBEDDED);
            cb = writer.DirectContent;
            template = cb.CreateTemplate(50, 50);
        }

        public override void OnEndPage(PdfWriter writer, Document document)
        {
            base.OnEndPage(writer, document);
            int pageN = writer.PageNumber;
            String text = "Page " + pageN + " of ";
            float len = bf.GetWidthPoint(text, 8);
            Rectangle pageSize = document.PageSize;
            cb.SetRGBColorFill(100, 100, 100);
            cb.BeginText();
            cb.SetFontAndSize(bf, 8);
            cb.SetTextMatrix(pageSize.GetRight(80), pageSize.GetBottom(30));
            cb.ShowText(text);
            cb.EndText();
            cb.AddTemplate(template, pageSize.GetRight(80) + len, pageSize.GetBottom(30));

            cb.BeginText();
            cb.SetFontAndSize(bf, 8);
            cb.ShowTextAligned(PdfContentByte.ALIGN_LEFT,
            "Confidential - for internal purposes only. Not to be distributed to outside parties. © 2016 Ipsos",
            pageSize.GetLeft(40),
            pageSize.GetBottom(30), 0);
            cb.EndText();
        }
        public override void OnCloseDocument(PdfWriter writer, Document document)
        {
            base.OnCloseDocument(writer, document);
            template.BeginText();
            template.SetFontAndSize(bf, 8);
            template.SetTextMatrix(0, 0);
            template.ShowText("" + (writer.PageNumber - 1));
            template.EndText();
        }
    }
}