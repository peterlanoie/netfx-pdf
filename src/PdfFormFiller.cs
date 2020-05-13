using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System.IO;
using Spire.Pdf.Widget;
using Spire.Pdf.Fields;
using Spire.Pdf;

namespace Common.Pdf
{
	public class PdfFormFiller
	{
		string _location;
		public PdfFormFiller() { }
		public PdfFormFiller(string fileLocation)
		{
			this._location = fileLocation;
		}

		public string FileLocation
		{
			get
			{
				return this._location;
			}
			set
			{
				this._location = value;
			}
		}

		public byte[] FillInFields(Dictionary<string, string> formFields)
		{
			
			if (string.IsNullOrEmpty(this._location.Trim()))
			{
				return null;
			}
			byte[] fileBytes;
			MemoryStream memStream = new MemoryStream();

			PdfReader reader = new PdfReader(this._location);
			PdfStamper stamper = new PdfStamper(reader, memStream);

            var layers = stamper.GetPdfLayers();
            AcroFields fields = stamper.AcroFields;

            fields.GenerateAppearances = true;

            foreach (var name in formFields.Keys)
            {
                var type = fields.GetFieldType(name);

                if (type == AcroFields.FIELD_TYPE_CHECKBOX)
                {
                    //fields.SetField(name, formFields[name].ToLower() == "true" ? "Yes" : "Off");
                }
                else
                {
                    fields.SetField(name, formFields[name]);
                }
            }
            stamper.Close();
            fileBytes = memStream.ToArray();
            
            return fileBytes;
		}

        public MemoryStream SpireFields(Dictionary<string, string> formFields)
        {
            if (string.IsNullOrEmpty(this._location.Trim()))
            {
                return null;
            }
            byte[] fileBytes;
                var doc = new Spire.Pdf.PdfDocument();
            doc.LoadFromFile(this._location);
            var form = doc.Form as PdfFormWidget;
            doc.Pages[0].BackgroudOpacity = 1.0f;
            form.AutoNaming = false;
            form.NeedAppearances = true;



            foreach (var field in form.FieldsWidget.List)
            {
                var formField = field as PdfField;

                if (field is PdfTextBoxFieldWidget)
                {
                    var textField = field as PdfTextBoxFieldWidget;
                    if (formFields.ContainsKey(textField.Name))
                    {
                        textField.Text = formFields[textField.Name];
                    }
                 }

                else if (field is PdfCheckBoxWidgetFieldWidget)
                {
                    var checkBoxField = field as PdfCheckBoxWidgetFieldWidget;
                    if(formFields.ContainsKey(checkBoxField.Name))
                    {
                        checkBoxField.Checked = formFields[checkBoxField.Name].ToUpper() == "TRUE" ? true : false;
                    }
                   
                }
                formField.Flatten = true;
            }

            using (var str = new MemoryStream())
            {
                doc.SaveToStream(str);
                doc.Close();

                return str;
            }
        }
    }
}
