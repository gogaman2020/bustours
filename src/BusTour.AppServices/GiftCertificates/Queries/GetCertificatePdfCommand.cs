using BusTour.Common.Config;
using BusTour.Data.Repositories.GiftCertificates;
using BusTour.Domain.Entities;
using Infrastructure.Common.Configs;
using Infrastructure.Common.DI;
using Infrastructure.Mediator;
using iText.IO.Image;
using iText.Kernel.Colors;
using iText.Kernel.Font;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Action;
using iText.Layout;
using iText.Layout.Borders;
using iText.Layout.Element;
using iText.Layout.Properties;
using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;


namespace BusTour.AppServices.GiftCertificates.Queries
{
    public class GetCertificatePdfCommand : HighLevelMediatorCommand<byte[]>
    {
        private readonly int? _certificateId;
        private readonly string _certificateNumber;

        private PageSize PageSize;
        private PdfFont MontserratMedium => GetFont("Montserrat-Medium.ttf");
        private PdfFont MontserratLight => GetFont("Montserrat-Light.ttf");


        public GetCertificatePdfCommand(int certificateId) => _certificateId = certificateId;
        public GetCertificatePdfCommand(string certificateNumber) => _certificateNumber = certificateNumber;

        public async override Task<MediatorCommandResult<byte[]>> ExecuteAsync()
        {
            PageSize = PageSize.A5.Rotate();

            var certificate = _certificateNumber is null
                ?  await IoC.GetRequiredService<IGiftCertificateRepository>().GetAsync(_certificateId ?? 0)
                : (await IoC.GetRequiredService<IGiftCertificateRepository>().FilterAsync(new GiftCertificatesFilter() { Number = _certificateNumber })).FirstOrDefault();

            byte[] pdf;

            using (var memoryStream = new MemoryStream())
            {
                var pdfWriter = new PdfWriter(memoryStream);
                var pdfDocument = new PdfDocument(pdfWriter);
                var document = new Document(pdfDocument, PageSize, true);

                var whiteColor = new DeviceRgb(255, 255, 255);

                AddImage(document, 
                    imageName: "gift_certificate.jpg", 
                    size: new PointF(PageSize.GetWidth(), PageSize.GetHeight()), 
                    position: new PointF(0, 0));

                AddParagraph(document,
                    text: certificate.Number, 
                    position: GetPercentPosition(20.2f, 59f), 
                    color: whiteColor,
                    font: MontserratLight,
                    fontSize: GetPercentFontSize(3.35f));

                AddParagraph(document,
                    text: certificate.AmountVariant.Amount.ToString() + " £",
                    position: GetPercentPosition(20.2f, 48f),
                    color: whiteColor,
                    font: MontserratLight,
                    fontSize: GetPercentFontSize(3.35f));

                AddParagraph(document,
                    text: certificate.DateEnd.ToString("dd.MM.yyyy"),
                    position: GetPercentPosition(20.2f, 37.1f),
                    color: whiteColor,
                    font: MontserratLight,
                    fontSize: GetPercentFontSize(3.35f));

                AddLink(document,
                    text: "HERE",
                    url: Config.Get<Urls>().Site,
                    position: GetPercentPosition(28.24f, 7.85f),
                    color: whiteColor,
                    font: MontserratMedium,
                    fontSize: GetPercentFontSize(3.8f));

                document.Close();

                pdf = memoryStream.ToArray();
            }

            return Success(pdf);
        }

        private static void AddParagraph(Document document, string text, PointF position, DeviceRgb color, PdfFont font, float fontSize = 14, float textWidth = 300, TextAlignment textAlignment = TextAlignment.LEFT) =>
            document.Add(new Paragraph(text)
                              .SetTextAlignment(textAlignment)
                              .SetFixedPosition(position.X, position.Y, textWidth)
                              .SetFontColor(color)
                              .SetFont(font)
                              .SetFontSize(fontSize));

        private static void AddLink(Document document, string text, string url, PointF position, DeviceRgb color, PdfFont font, float fontSize = 12, float textWidth = 300, TextAlignment textAlignment = TextAlignment.LEFT) =>
            document.Add(new Paragraph()
                              .Add(new Link(text, PdfAction.CreateURI(url)))
                              .SetBorder(Border.NO_BORDER)
                              .SetTextAlignment(textAlignment)
                              .SetFixedPosition(position.X, position.Y, textWidth)
                              .SetFontColor(color)
                              .SetFont(font)
                              .SetFontSize(fontSize)
                              .SetUnderline());

        private static void AddImage(Document document, string imageName, PointF size, PointF position) =>
            document.Add(new Image(ImageDataFactory.Create(Resources.EmbeddedResource.GetFileBytes(imageName)))
                              .SetWidth(size.X)
                              .SetHeight(size.Y)
                              .SetFixedPosition(position.X, position.Y));

        private PdfFont GetFont(string fileName) =>
            PdfFontFactory.CreateFont(Resources.EmbeddedResource.GetFileBytes(fileName), PdfFontFactory.EmbeddingStrategy.FORCE_EMBEDDED);

        private PointF GetPercentPosition(float x, float y) => new PointF(PageSize.GetWidth() / 100 * x, PageSize.GetHeight() / 100 * y);
        private float GetPercentFontSize(float percent) => PageSize.GetHeight() / 100 * percent;
    }
}
