using Microsoft.Reporting.WinForms;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Zatca_EInvoice
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        public void Form1_Load(object sender, EventArgs e)
        {
            this.reportViewer1.RefreshReport();
        }

        public void reportViewer1_Load(object sender, EventArgs e)
        {
            
        }

        private void button1_Click(object sender, EventArgs e)
        {
            QRCoder.QRCodeGenerator qRCodeGenerator = new QRCoder.QRCodeGenerator();
            QRCoder.QRCodeData qRCodeData = qRCodeGenerator.CreateQrCode("eyJhbGciOiJSUzI1NiIsImtpZCI6IkVEQzU3REUxMzU4QjMwMEJBOUY3OTM0MEE2Njk2ODMxRjNDODUwNDciLCJ0eXAiOiJKV1QiLCJ4NXQiOiI3Y1Y5NFRXTE1BdXA5NU5BcG1sb01mUElVRWMifQ.eyJkYXRhIjoie1wiU2VsbGVyR3N0aW5cIjpcIjI0QUFBUEkzMTgyTTAwMlwiLFwiQnV5ZXJHc3RpblwiOlwiMjdBQUFQSTMxODJNMDAyXCIsXCJEb2NOb1wiOlwiMTA4MjNcIixcIkRvY1R5cFwiOlwiSU5WXCIsXCJEb2NEdFwiOlwiMDIvMTEvMjAyMFwiLFwiVG90SW52VmFsXCI6MTYyNjQ1LjAwLFwiSXRlbUNudFwiOjEsXCJNYWluSHNuQ29kZVwiOlwiMTUxMjE5MTBcIixcIklyblwiOlwiMTYyMTYyMTAyMTJmMjlmZTI1YTFlZjQ5ZmVkZWQ2ODMyZTRjOWQxZjVkYjRhMzI5YjA3MDE2ZTE2ZThjZGU4ZVwiLFwiSXJuRHRcIjpcIjIwMjAtMTEtMDIgMTM6Mzc6MDBcIn0iLCJpc3MiOiJOSUMifQ.oCCnjv3C3kxBI8WZErxgQtpNRo6ojwx3Zg2vlpS6Rlnm7c4pfMaoz8I4PcGUaYamTGtyDY76G-QjO-3-0AN7QhWVszrdAbRs_rZkUNTq6HOWPlsqQNAXGDNxrvTiXSJ9v9elYLuVic3ClzQpkqvMeRY13ZlHZq_p7MPvI1NwViXwZobl_FQ1yjXs6yx0N9ArbH3Oqwo0pnJr9ttaWKXD6__u1jV3iHJ5pL9Fpqvw6Je4WcdfvjBThJW3zdrV8V4ZvkaFLLW4fglW2raGc-CiIUM3OVfbT6KDWsjXyHgl3WprORWkVoQtSRKgd0OPUIL8HzJPqdeNoExbf-I1WeMNfA", QRCoder.QRCodeGenerator.ECCLevel.Q);
            QRCoder.QRCode qRCode = new QRCoder.QRCode(qRCodeData);
            Bitmap bmp = qRCode.GetGraphic(7);
            using (MemoryStream ms = new MemoryStream())
            {
                bmp.Save(ms, ImageFormat.Bmp);
                ReportData reportData = new ReportData();
                ReportData.QRCodeRow qRCodeRow = reportData.QRCode.NewQRCodeRow();
                qRCodeRow.Image = ms.ToArray();
                reportData.QRCode.AddQRCodeRow(qRCodeRow);

                ReportDataSource reportDataSource = new ReportDataSource();
                reportDataSource.Name = "ReportData";
                reportDataSource.Value = reportData.QRCode;
                //string dt = reportDataSource;
                ReportViewer reportViewer1 = new ReportViewer();
                reportViewer1.LocalReport.DataSources.Clear();
                reportViewer1.LocalReport.DataSources.Add(reportDataSource);
                //reportViewer1.RefreshReport();
            }
        }
    }
}
