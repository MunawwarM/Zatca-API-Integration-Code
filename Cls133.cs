using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Threading;
using System.Collections;
using System.Globalization;
using System.Text.RegularExpressions;
using System.ComponentModel;
using System.Data.OleDb;
using System.Security.Cryptography;
using System.Net;
using System.Drawing;
using System.Diagnostics;
using System.Timers;
using System.Net.Mail;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using Microsoft.Win32;
using Newtonsoft.Json;
using EInvoices.ViewModel;
using System.Xml.Linq;
using Newtonsoft.Json.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using RestSharp;
using System.Security.Policy;
using IdentityModel.Client;
using Formatting = Newtonsoft.Json.Formatting;
using static IdentityModel.OidcConstants;
using System.Threading.Tasks;
using TokenResponse = IdentityModel.Client.TokenResponse;
using SAPbobsCOM;
using System.Security.Cryptography.X509Certificates;
using System.Runtime.InteropServices.ComTypes;
using System.Web;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Web.Configuration;
using RestSharp.Extensions;
using System.Net.NetworkInformation;
using Microsoft.AspNetCore.Mvc;

namespace Zatca_EInvoice.Classes
{
    class Cls133
    {
        #region variables
        Utilities.clsVariables oVariables = new Utilities.clsVariables();
        Utilities.clsDataMethods DM = new Utilities.clsDataMethods();
        public static SAPbouiCOM.MenuItem oMenuP;
        public static Boolean GenerateINVFlag=true;
        SAPbouiCOM.DBDataSource DB, DB1;
        public static SAPbouiCOM.Application __app = null;
        #endregion

        class JSONResp
        {
            public string Irn { get; set; }
            public string AckNo { get; set; }
            public string AckDt { get; set; }
            public string SignedQRCode { get; set; }
            public string message { get; set; }
            public string EwbNo { get; set; }
            public string EwbDt { get; set; }
            public string EwbValidTill { get; set; }
            public string detailedpdfUrl { get; set; }
            public string ewayBillNo { get; set; }
            public string ewayBillDate { get; set; }
            public string validUpto { get; set; }
        }

        class TokenResp
        {
            public string access_token { get; set; }
            public string scope { get; set; }
            public string resourceOwnerId { get; set; }
            public string expires_in { get; set; }
            public string token_type { get; set; }
            public string orgId { get; set; }
        }

        class EmailCollection
        {
            public string Email { get; set; }
        }

        public void SBO_Application_FormDataEvent(ref SAPbouiCOM.BusinessObjectInfo oBusinessObjectInfo, out bool BubbleEvent)
        {
            BubbleEvent = true;

            if ((oBusinessObjectInfo.EventType == SAPbouiCOM.BoEventTypes.et_FORM_DATA_ADD ) && oBusinessObjectInfo.BeforeAction == true)
            {
                try
                {
                    #region Generate Zatca-Invoice Validation
                    oVariables.Soft_Form = clsMainClass.SBO_Application.Forms.GetForm(oBusinessObjectInfo.FormTypeEx, clsMainClass.SBO_Application.Forms.ActiveForm.TypeCount);
                    SAPbobsCOM.Recordset ors3 = clsMainClass.oCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset);
                    string SQuery1 = "";
                    oVariables.oEdit = (SAPbouiCOM.EditText)oVariables.oForm.Items.Item("4").Specific;
                    string cardcode = oVariables.oEdit.Value;
                    if (clsMainClass.oCompany.DbServerType == SAPbobsCOM.BoDataServerTypes.dst_HANADB)
                    {
                        SQuery1 = @"Select C.""CardFName"" as ""Buyer Name"",C.""CardName"" as ""Buyer Name AR"",'CRN' as ""Buyer Schema ID"",C.""LicTradNum"" as ""Party ID"",
                                    C.""RegNum"" as ""Buyer ID"",D.""Address"" as ""Buyer Street Name"",""Address3"" as ""Buyer AdlStrName"",D.""Street"" as ""Buyer Building"",
                                    D.""City"" as ""Buyer CityNm"",D.""ZipCode"" as ""Buyer ZipCode"",D.""County"",(Select ""Building"" from ADM1) as ""Building No AR"",
                                    (Select CRD1.""Address"" from CRD1 where C.""CardCode""=CRD1.""CardCode"" and ""AdresType""='S') as ""Buyer Street Name AR"",
                                    (Select CRD1.""Address3"" from CRD1 where C.""CardCode""=CRD1.""CardCode"" and ""AdresType""='S') as ""Buyer AdlStrName AR"",
                                    (Select CRD1.""Street"" from CRD1 where C.""CardCode""=CRD1.""CardCode"" and ""AdresType""='S') as ""Buyer Building AR"",
                                    (Select CRD1.""City"" from CRD1 where C.""CardCode""=CRD1.""CardCode"" and ""AdresType""='S') as ""Buyer CityNm AR"",
                                    (Select CRD1.""ZipCode"" from CRD1 where C.""CardCode""=CRD1.""CardCode"" and ""AdresType""='S') as ""Buyer ZipCode AR""                                                           
                                    from OCRD C 
                                    INNER JOIN CRD1 D on C.""CardCode"" = D.""CardCode"" and ""AdresType"" = 'B'
                                    where C.""CardCode""='" + cardcode + "'";
                    }
                    else
                    {
                        SQuery1 = @"Select C.""CardFName"" as ""Buyer Name"",C.""CardName"" as ""Buyer Name AR"",'CRN' as ""Buyer Schema ID"",C.""LicTradNum"" as ""Party ID"",
                                    C.""RegNum"" as ""Buyer ID"",D.""Address"" as ""Buyer Street Name"",""Address3"" as ""Buyer AdlStrName"",D.""Street"" as ""Buyer Building"",
                                    D.""City"" as ""Buyer CityNm"",D.""ZipCode"" as ""Buyer ZipCode"",D.""County"",(Select ""Building"" from ADM1) as ""Building No AR"",
                                    (Select CRD1.""Address"" from CRD1 where C.""CardCode""=CRD1.""CardCode"" and ""AdresType""='S') as ""Buyer Street Name AR"",
                                    (Select CRD1.""Address3"" from CRD1 where C.""CardCode""=CRD1.""CardCode"" and ""AdresType""='S') as ""Buyer AdlStrName AR"",
                                    (Select CRD1.""Street"" from CRD1 where C.""CardCode""=CRD1.""CardCode"" and ""AdresType""='S') as ""Buyer Building AR"",
                                    (Select CRD1.""City"" from CRD1 where C.""CardCode""=CRD1.""CardCode"" and ""AdresType""='S') as ""Buyer CityNm AR"",
                                    (Select CRD1.""ZipCode"" from CRD1 where C.""CardCode""=CRD1.""CardCode"" and ""AdresType""='S') as ""Buyer ZipCode AR""                                                           
                                    from OCRD C 
                                    INNER JOIN CRD1 D on C.""CardCode"" = D.""CardCode"" and ""AdresType"" = 'B'
                                    where C.""CardCode""='" + cardcode + "'";
                    }

                    ors3.DoQuery(SQuery1);
                    int i = 0;
                    if (ors3.RecordCount > 0)
                    {
                        string BuyerName = ors3.Fields.Item("Buyer Name").Value.ToString();
                        string BuyerNameAR = ors3.Fields.Item("Buyer Name AR").Value.ToString();
                        string BSchemaID = ors3.Fields.Item("Buyer Schema ID").Value.ToString();
                        string BuyerID = ors3.Fields.Item("Buyer ID").Value.ToString();
                        string BStreetNM = ors3.Fields.Item("Buyer Street Name").Value.ToString();
                        string BAdlStrName = ors3.Fields.Item("Buyer AdlStrName").Value.ToString();
                        string BBuildingNo = ors3.Fields.Item("Buyer Building").Value.ToString();
                        string BCity = ors3.Fields.Item("Buyer CityNm").Value.ToString();
                        string BZipCode = ors3.Fields.Item("Buyer ZipCode").Value.ToString();
                        string BStreetNMAR = ors3.Fields.Item("Buyer Street Name AR").Value.ToString();
                        string BAdlStrNameAR = ors3.Fields.Item("Buyer AdlStrName AR").Value.ToString();
                        string BBuildingNoAR = ors3.Fields.Item("Buyer Building AR").Value.ToString();
                        string BCityAR = ors3.Fields.Item("Buyer CityNm AR").Value.ToString();
                        string BZipCodeAR = ors3.Fields.Item("Buyer ZipCode AR").Value.ToString();
                        string BPartyID = ors3.Fields.Item("Party ID").Value.ToString();

                        if (BuyerName == "")
                        {
                            //clsMainClass.StatusMessage = "Buyer Name should not be blank";
                            //BubbleEvent = false;
                            //return;
                        }
                        if (BuyerNameAR == "")
                        {
                            clsMainClass.StatusMessage = "Buyer Name in arabic should not be blank";
                            BubbleEvent = false;
                            return;
                        }
                        if (BSchemaID == "")
                        {
                            clsMainClass.StatusMessage = "Buyer Schema ID should not be blank";
                            BubbleEvent = false;
                            return;
                        }
                        if (BuyerID == "")
                        {
                            //clsMainClass.StatusMessage = "Buyer ID should not be blank";
                            //BubbleEvent = false;
                            //return;
                        }
                        if (BStreetNM == "")
                        {
                            clsMainClass.StatusMessage = "Buyer Address should not be blank";
                            BubbleEvent = false;
                            return;
                        }
                        if (BAdlStrName == "")
                        {
                            clsMainClass.StatusMessage = "Buyer Adress 3 should not be blank";
                            BubbleEvent = false;
                            return;
                        }
                        if (BBuildingNo == "")
                        {
                            clsMainClass.StatusMessage = "Buyer Street should not be blank";
                            BubbleEvent = false;
                            return;
                        }
                        if (BCity == "")
                        {
                            clsMainClass.StatusMessage = "Buyer city should not be blank";
                            BubbleEvent = false;
                            return;
                        }
                        if (BZipCode == "")
                        {
                            clsMainClass.StatusMessage = "Buyer zipcode should not be blank";
                            BubbleEvent = false;
                            return;
                        }
                        if (BStreetNMAR == "")
                        {
                            clsMainClass.StatusMessage = "Buyer Adress in arabic should not be blank";
                            BubbleEvent = false;
                            return;
                        }
                        if (BAdlStrNameAR == "")
                        {
                            clsMainClass.StatusMessage = "Buyer Address 3 in arabic should not be blank";
                            BubbleEvent = false;
                            return;
                        }
                        if (BBuildingNoAR == "")
                        {
                            clsMainClass.StatusMessage = "Buyer Street in arabic should not be blank";
                            BubbleEvent = false;
                            return;
                        }                       
                        if (BCityAR == "")
                        {
                            clsMainClass.StatusMessage = "Buyer city in arabic should not be blank";
                            BubbleEvent = false;
                            return;
                        }
                        if (BPartyID == "")
                        {
                            clsMainClass.StatusMessage = "Buyer Party ID should not be blank";
                            BubbleEvent = false;
                            return;
                        }
                    }
                    #endregion
                }
                catch (Exception ex)
                {
                    clsMainClass.SBO_Application.StatusBar.SetText(ex.Message, SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Error);
                    BubbleEvent = false;
                    return;
                }
            }

            if ((oBusinessObjectInfo.EventType == SAPbouiCOM.BoEventTypes.et_FORM_DATA_ADD ) && oBusinessObjectInfo.BeforeAction == false && oBusinessObjectInfo.ActionSuccess == true)
            {
                try
                {
                    #region Zatca-Invoice

                    oVariables.oForm = clsMainClass.SBO_Application.Forms.GetForm(oBusinessObjectInfo.FormTypeEx, clsMainClass.SBO_Application.Forms.ActiveForm.TypeCount);
                    System.Xml.XmlDocument oXml = null;
                    oXml = new XmlDocument();
                    oXml.LoadXml(oBusinessObjectInfo.ObjectKey);
                    string DocEntry = oXml.SelectSingleNode("/DocumentParams/DocEntry").InnerText;

                    string fromPlace1 = "", fromState1 = "", fromPlace11 = "", fromState11 = "", docType1 = "", transMode1 = "", transporterName1 = "",
                           transporterId1 = "", transDocNo1 = "", transDocDate1 = "", vehicleNo1 = "", vehicleType1 = "";
                    double transDistance1 = 0.0, mndis = 0.0;
                    oVariables.oEdit = (SAPbouiCOM.EditText)oVariables.oForm.Items.Item("8").Specific;
                    string invdocentry = oVariables.oEdit.Value;
                    string formID = oVariables.oForm.TypeEx;
                    string SQuery = "";
                    SAPbobsCOM.Recordset ors2 = clsMainClass.oCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset);
                    SAPbobsCOM.Recordset oRs = clsMainClass.oCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset);
                    oRs = (SAPbobsCOM.Recordset)clsMainClass.oCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset);
                    try
                    {
                        try
                        {
                            string RefNum = "", Finyr = "", ThirdPartyInvoice = "0", NominalInvoice = "", ExportInvoice = "", SummaryInvoice = "",
                                   SelfBilledinvoice = "", SellerName = "", SSchemaID = "", SellerID = "", SStreetNM = "", SStreetAdlNM = "",
                                   SBuildingNo = "", SCity = "", companyid = "", SZipCode = "", SSubDivisionNm = "", VatID = "", SSubDivisionNmAR = "",
                                   SStreetNMAR = "", SStreetAdlNMAR = "", SBuildingNoAR = "", SCityAR = "", SZipCodeAR = "", SellerNameAR = "",
                                   BuyerName = "", BuyerNameAR = "", BSchemaID = "", BuyerID = "", BStreetNM = "", BBuildingNo = "", BCity = "", BPartyID = "",
                                   BZipCode = "", BSubDivisionNm = "", ItemCode = "", ItemDesc = "", ItemDescAR = "", Companyname = "", UOM = "", BAdlStrName = "",
                                   BStreetNMAR = "", BBuildingNoAR = "", BCityAR = "", BZipCodeAR = "", BAdlStrNameAR = "", SVatId = "", VatGroup = "",
                                   InvTypCd = "", InvSubtype = "", deldate = "",remarks="",email="";

                            int fromPincode = 0, fromPincode1 = 0, fromStateCode = 0, fromStateCode1 = 0, toPincodeS = 0,
                                toPincodeB = 0, toStateCodeS = 0, toStateCodeB = 0;
                            double totalValue = 0, othvalue = 0, cgstValue = 0, dis = 0, linetotal = 0, sgstValue = 0, igstValue = 0,
                                   utgstValue = 0, totInvValue = 0, taxableAmount = 0, sgstRate = 0, cgstRate = 0, igstRate = 0,
                                   utgstRate = 0, dtotal = 0, discamt = 0, Vatrate = 0, taxprcnt = 0, quantity = 0, price = 0;

                            string query = "";
                            if (clsMainClass.oCompany.DbServerType == SAPbobsCOM.BoDataServerTypes.dst_HANADB)
                            {
                                query = "Select \"U_API\",\"U_ClientID\",\"U_ClientSecret\" from \"@API_HEADER\" A\r\n" +
                                        "INNER JOIN \"@API_DETAIL\" B on A.\"DocEntry\" = B.\"DocEntry\"\r\n" +
                                        "WHERE A.\"DocEntry\" = '1'";
                            }
                            ors2.DoQuery(query);
                            string API = ors2.Fields.Item("U_API").Value.ToString();
                            string ClientID = ors2.Fields.Item("U_ClientID").Value.ToString();
                            string ClientSecret = ors2.Fields.Item("U_ClientSecret").Value.ToString();

                            oVariables.oEdit = (SAPbouiCOM.EditText)oVariables.oForm.Items.Item("10").Specific;
                            string docDate = oVariables.oEdit.Value;
                            oVariables.oEdit = (SAPbouiCOM.EditText)oVariables.oForm.Items.Item("8").Specific;
                            string docNum = oVariables.oEdit.Value;
                            string qrSelect = "";

                            if (clsMainClass.oCompany.DbServerType == SAPbobsCOM.BoDataServerTypes.dst_HANADB)
                            {
                                qrSelect = @"Call ZATCA_EINVOICE ('" + docDate + "','" + docNum + "')";
                            }

                            else if (clsMainClass.oCompany.DbServerType != SAPbobsCOM.BoDataServerTypes.dst_HANADB)
                            {
                                qrSelect = @"Exec ZATCA_EINVOICE '" + docDate + "','" + docNum + "'";
                            }
                            oRs.DoQuery(qrSelect);

                            if (!oRs.EoF)
                            {
                                try
                                {
                                    clsMainClass.SBO_Application.StatusBar.SetText("Zatca-Invoice generating please wait " + docNum, SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_None);
                                    deldate = oRs.Fields.Item("date").Value.ToString("yyyy-MM-dd");
                                    RefNum = oRs.Fields.Item("Ref Num").Value.ToString();
                                    Finyr = oRs.Fields.Item("Financial Year").Value.ToString();
                                    InvTypCd = oRs.Fields.Item("InvTypeCd").Value.ToString();
                                    InvSubtype = oRs.Fields.Item("InvSubtype").Value.ToString();
                                    NominalInvoice = oRs.Fields.Item("NominalInvoice").Value.ToString();
                                    ExportInvoice = oRs.Fields.Item("ExportInvoice").Value.ToString();
                                    SummaryInvoice = oRs.Fields.Item("SummaryInvoice").Value.ToString();
                                    SelfBilledinvoice = oRs.Fields.Item("SelfBilledinvoice").Value.ToString();
                                    SellerName = oRs.Fields.Item("Seller Name").Value.ToString();
                                    SSchemaID = oRs.Fields.Item("Seller Schema ID").Value.ToString();
                                    SellerID = oRs.Fields.Item("Seller ID").Value.ToString();
                                    SStreetNM = oRs.Fields.Item("Seller Street Name").Value.ToString();
                                    SStreetAdlNM = oRs.Fields.Item("Seller AdlStreet Name").Value.ToString();
                                    SBuildingNo = oRs.Fields.Item("Building No").Value.ToString();
                                    SCity = oRs.Fields.Item("City").Value.ToString();
                                    SZipCode = oRs.Fields.Item("ZipCode").Value.ToString();
                                    SSubDivisionNm = oRs.Fields.Item("City SubDivision Name").Value.ToString();
                                    SVatId = oRs.Fields.Item("SVatId").Value.ToString();

                                    companyid = oRs.Fields.Item("Company ID").Value.ToString();
                                    Companyname = oRs.Fields.Item("Company name").Value.ToString();
                                    SStreetNMAR = oRs.Fields.Item("Street Name AR").Value.ToString();
                                    SStreetAdlNMAR = oRs.Fields.Item("Seller AdlStreet Name AR").Value.ToString();
                                    SBuildingNoAR = oRs.Fields.Item("Building No AR").Value.ToString();
                                    SCityAR = oRs.Fields.Item("City AR").Value.ToString();
                                    SZipCodeAR = oRs.Fields.Item("ZipCode").Value.ToString();
                                    SSubDivisionNmAR = oRs.Fields.Item("City SubDivision Name AR").Value.ToString();

                                    BuyerName = oRs.Fields.Item("Buyer Name").Value.ToString();
                                    BuyerNameAR = oRs.Fields.Item("Buyer Name AR").Value.ToString();
                                    BSchemaID = oRs.Fields.Item("Buyer Schema ID").Value.ToString();
                                    BuyerID = oRs.Fields.Item("Buyer ID").Value.ToString();
                                    BStreetNM = oRs.Fields.Item("Buyer Street Name").Value.ToString();
                                    BAdlStrName = oRs.Fields.Item("Buyer AdlStrName").Value.ToString();
                                    BBuildingNo = oRs.Fields.Item("Buyer Building").Value.ToString();
                                    BCity = oRs.Fields.Item("Buyer CityNm").Value.ToString();
                                    BZipCode = oRs.Fields.Item("Buyer ZipCode").Value.ToString();
                                    BStreetNMAR = oRs.Fields.Item("Buyer Street Name AR").Value.ToString();
                                    BAdlStrNameAR = oRs.Fields.Item("Buyer AdlStrName AR").Value.ToString();
                                    BBuildingNoAR = oRs.Fields.Item("Buyer Building AR").Value.ToString();
                                    BCityAR = oRs.Fields.Item("Buyer CityNm AR").Value.ToString();
                                    BZipCodeAR = oRs.Fields.Item("Buyer ZipCode AR").Value.ToString();
                                    BPartyID = oRs.Fields.Item("Party ID").Value.ToString();
                                    remarks = oRs.Fields.Item("Remarks").Value.ToString();
                                    email = oRs.Fields.Item("E_Mail").Value.ToString();

                                    Zatca_Details_VM Zatca_Details_VM_obj = new Zatca_Details_VM();
                                    Zatca_Details_VM_obj.ReferenceNumber = docNum;
                                    Zatca_Details_VM_obj.FinancialYear = Finyr;
                                    Zatca_Details_VM_obj.InvTypeCd = InvTypCd;
                                    Zatca_Details_VM_obj.InvSubtype = InvSubtype;
                                    Zatca_Details_VM_obj.ThirdPartyInvoice = ThirdPartyInvoice;
                                    Zatca_Details_VM_obj.NominalInvoice = NominalInvoice;
                                    Zatca_Details_VM_obj.ExportInvoice = ExportInvoice;
                                    Zatca_Details_VM_obj.SummaryInvoice = SummaryInvoice;
                                    Zatca_Details_VM_obj.SelfBilledinvoice = SelfBilledinvoice;
                                    Zatca_Details_VM_obj.Note = remarks;
                                    Zatca_Details_VM_obj.OrderRef = "";
                                    Zatca_Details_VM_obj.BlngRef = "";
                                    Zatca_Details_VM_obj.BlngRefIssueDt = "";
                                    Zatca_Details_VM_obj.ContractDocRef = "";
                                    Zatca_Details_VM_obj.Delivery_ActualDeliveryDate = deldate;
                                    Zatca_Details_VM_obj.Delivery_LatestDeliveryDate = "";
                                    Zatca_Details_VM_obj.PymtMeansCode = "";
                                    Zatca_Details_VM_obj.PymtMeans_InstructionNoteReason = "";
                                    Zatca_Details_VM_obj.CustEmailID = email;

                                    zatca_seller_detail_json zatca_seller_detail_json = new zatca_seller_detail_json();
                                    zatca_sellerpartydetails zatca_sellerpartydetails = new zatca_sellerpartydetails();
                                    List<zatca_sellerpartydetails> list = new List<zatca_sellerpartydetails>();

                                    zatca_sellerpartydetails.SchemeID = "CRN"; //Seller CR No
                                    zatca_sellerpartydetails.PartyID = "";     //Seller Group VAT No
                                    zatca_sellerpartydetails.SellerIDNumber = SellerID; //Seller CR No
                                    zatca_sellerpartydetails.SchemeID_AR = "";
                                    zatca_sellerpartydetails.PartyID_AR = "";
                                    zatca_sellerpartydetails.SellerIDNumber_AR = "";
                                    zatca_seller_detail_json.Party = zatca_sellerpartydetails;

                                    zatca_sellerpostaladdress zatca_sellerpostaladdress = new zatca_sellerpostaladdress();
                                    zatca_sellerpostaladdress.SellerCode = "";
                                    zatca_sellerpostaladdress.StrName = SStreetNM;
                                    zatca_sellerpostaladdress.AdlStrName = SStreetAdlNM;
                                    zatca_sellerpostaladdress.PlotIdentification = "";
                                    zatca_sellerpostaladdress.BldgNumber = SBuildingNo;
                                    zatca_sellerpostaladdress.CityName = SCity;
                                    zatca_sellerpostaladdress.PostalZone = SZipCode;
                                    zatca_sellerpostaladdress.CntrySubentityCd = "";
                                    zatca_sellerpostaladdress.CitySubdivisionName = SSubDivisionNm;
                                    zatca_sellerpostaladdress.StrName_AR = SStreetNMAR;
                                    zatca_sellerpostaladdress.AdlStrName_AR = SStreetAdlNMAR;
                                    zatca_sellerpostaladdress.PlotIdentification_AR = "";
                                    zatca_sellerpostaladdress.BldgNumber_AR = SBuildingNoAR;
                                    zatca_sellerpostaladdress.CityName_AR = SCityAR;
                                    zatca_sellerpostaladdress.PostalZone_AR = SZipCodeAR;
                                    zatca_sellerpostaladdress.CntrySubentityCd_AR = "";
                                    zatca_sellerpostaladdress.CitySubdivisionName_AR = "";
                                    zatca_seller_detail_json.PostalAddress = zatca_sellerpostaladdress;
                                    Zatca_Details_VM_obj.ActngSuplParty = zatca_seller_detail_json;

                                    zatca_sellerpartytaxscheme zatca_sellerpartytaxscheme = new zatca_sellerpartytaxscheme();
                                    zatca_sellerpartytaxscheme.CompanyID = companyid; //VAT ID
                                    zatca_sellerpartytaxscheme.CompanyID_AR = "";
                                    zatca_seller_detail_json.PartyTaxScheme = zatca_sellerpartytaxscheme;

                                    zatca_sellerpartylegalentity zatca_sellerpartylegalentity = new zatca_sellerpartylegalentity();
                                    zatca_sellerpartylegalentity.RegName = SellerName; //Company Name in English
                                    zatca_sellerpartylegalentity.RegName_AR = Companyname; //Company Name in Arabic
                                    zatca_seller_detail_json.PartyLegalEntity = zatca_sellerpartylegalentity;

                                    zatca_buyer_detail_json zatca_buyer_detail_json = new zatca_buyer_detail_json();
                                    zatca_buyerpartydetails zatca_buyerpartydetails = new zatca_buyerpartydetails();
                                    zatca_buyerpartydetails.SchemeID = "CRN";
                                    zatca_buyerpartydetails.PartyID = ""; //Buyer Group VAT Reg No
                                    zatca_buyerpartydetails.BuyerIDNumber = BuyerID; //Buyer CR No
                                    zatca_buyerpartydetails.SchemeID_AR = "";
                                    zatca_buyerpartydetails.PartyID_AR = "";
                                    zatca_buyerpartydetails.BuyerIDNumber_AR = "";
                                    zatca_buyer_detail_json.Party = zatca_buyerpartydetails;

                                    zatca_buyerpostaladdress zatca_buyerpostaladdress = new zatca_buyerpostaladdress();
                                    zatca_buyerpostaladdress.BuyerCode = "";
                                    zatca_buyerpostaladdress.StrName = BStreetNM;
                                    zatca_buyerpostaladdress.AdlStrName = "ABCD";// BAdlStrName;
                                    zatca_buyerpostaladdress.PlotIdentification = "ABCD";
                                    zatca_buyerpostaladdress.BldgNumber = BBuildingNo;
                                    zatca_buyerpostaladdress.CityName = BCity;
                                    zatca_buyerpostaladdress.PostalZone = BZipCode;
                                    zatca_buyerpostaladdress.CntrySubentityCd = "RY";
                                    zatca_buyerpostaladdress.CitySubdivisionName = "AB";
                                    zatca_buyerpostaladdress.Cntry = "SA";
                                    zatca_buyerpostaladdress.StrName_AR = BStreetNMAR;
                                    zatca_buyerpostaladdress.AdlStrName_AR = BAdlStrNameAR;
                                    zatca_buyerpostaladdress.PlotIdentification_AR = "ABCD";
                                    zatca_buyerpostaladdress.BldgNumber_AR = BBuildingNoAR;
                                    zatca_buyerpostaladdress.CityName_AR = BCityAR;
                                    zatca_buyerpostaladdress.PostalZone_AR = BZipCodeAR;
                                    zatca_buyerpostaladdress.CntrySubentityCd_AR = "RY";
                                    zatca_buyerpostaladdress.CitySubdivisionName_AR = "AB";
                                    zatca_buyer_detail_json.PostalAddress = zatca_buyerpostaladdress;
                                    Zatca_Details_VM_obj.ActngCustomerParty = zatca_buyer_detail_json;

                                    zatca_buyerpartytaxscheme zatca_buyerpartytaxscheme = new zatca_buyerpartytaxscheme();
                                    zatca_buyerpartytaxscheme.CompanyID = BPartyID; //Buyer VAT No
                                    zatca_buyerpartytaxscheme.CompanyID_AR = "";
                                    zatca_buyer_detail_json.PartyTaxScheme = zatca_buyerpartytaxscheme;

                                    zatca_buyerpartylegalentity zatca_buyerpartylegalentity = new zatca_buyerpartylegalentity();
                                    zatca_buyerpartylegalentity.RegName = BuyerName;
                                    zatca_buyerpartylegalentity.RegName_AR = BuyerNameAR;
                                    zatca_buyer_detail_json.PartyLegalEntity = zatca_buyerpartylegalentity;

                                    double linetotal1 = 0, igstValue1 = 0, sgstValue1 = 0, cgstValue1 = 0, utgstValue1 = 0, othValue1 = 0;

                                    zatca_itemlist_detail_vm zatca_itemlist_detail_vm;
                                    List<zatca_itemlist_detail_vm> list5 = new List<zatca_itemlist_detail_vm>();

                                    for (int k = 0; k < oRs.RecordCount; k++)
                                    {
                                        #region Item Type
                                        zatca_itemlist_detail_vm = new zatca_itemlist_detail_vm();
                                        ItemCode = oRs.Fields.Item("ItemCode").Value.ToString();
                                        ItemDesc = oRs.Fields.Item("ItemName").Value.ToString();
                                        ItemDescAR = oRs.Fields.Item("ItemName AR").Value.ToString();
                                        quantity = double.Parse(oRs.Fields.Item("Quantity").Value.ToString());
                                        UOM = oRs.Fields.Item("UomCode").Value.ToString();
                                        price = double.Parse(oRs.Fields.Item("Price").Value.ToString());
                                        linetotal = double.Parse(oRs.Fields.Item("LineTotal").Value.ToString());
                                        discamt = double.Parse(oRs.Fields.Item("DiscAmt").Value.ToString());
                                        Vatrate = double.Parse(oRs.Fields.Item("Rate").Value.ToString());
                                        VatGroup = oRs.Fields.Item("VAT").Value.ToString();
                                        taxprcnt = double.Parse(oRs.Fields.Item("Tax Percent").Value.ToString());
                                        if (VatGroup.Contains("S"))
                                        {
                                            VatGroup = "S";
                                        }
                                        else if (VatGroup.Contains("E"))
                                        {
                                            VatGroup = "E";
                                        }
                                        else if (VatGroup.Contains("Z"))
                                        {
                                            VatGroup = "Z";
                                        }
                                        else if (VatGroup.Contains("O"))
                                        {
                                            VatGroup = "O";
                                        }

                                        zatca_itemlist_detail_vm.ID = k + 1;
                                        zatca_itemlist_detail_vm.ItemCode = ItemCode;
                                        zatca_itemlist_detail_vm.Note = ItemDesc;
                                        zatca_itemlist_detail_vm.InvQtyUom = UOM;
                                        zatca_itemlist_detail_vm.InvdQty = quantity.ToString();
                                        zatca_itemlist_detail_vm.LineExtAmt = (quantity * price).ToString();
                                        zatca_itemlist_detail_vm.PrepaymentID = "";
                                        zatca_itemlist_detail_vm.PrepaymentID_UID = "";
                                        zatca_itemlist_detail_vm.PrepaymentIssueDate = "";
                                        zatca_itemlist_detail_vm.PrepaymentIssueTime = "";
                                        zatca_itemlist_detail_vm.PrepaymentDocType = "";
                                        zatca_itemlist_detail_vm.PaidVATCategoryTaxableAmt = "";
                                        zatca_itemlist_detail_vm.PaidVATCategoryTaxAmt = "";

                                        zatca_item_alwchg zatca_item_alwchg = new zatca_item_alwchg();
                                        List<zatca_item_alwchg> list1 = new List<zatca_item_alwchg>();
                                        zatca_item_alwchg.Indicator = "";
                                        zatca_item_alwchg.AlwChgReason = "";
                                        zatca_item_alwchg.Amt = discamt.ToString();
                                        zatca_item_alwchg.BaseAmt = (price).ToString();
                                        zatca_item_alwchg.MFN = "0";
                                        list1.Add(zatca_item_alwchg);
                                        zatca_itemlist_detail_vm.AlwChg = list1;

                                        zatca_item_taxtotal zatca_item_taxtotal = new zatca_item_taxtotal();
                                        zatca_item_taxtotal.TaxAmt = ((quantity * price) * Vatrate / 100).ToString();
                                        zatca_item_taxtotal.RoundingAmt = "0";
                                        zatca_itemlist_detail_vm.TaxTotal = zatca_item_taxtotal;

                                        zatca_itemlist_item zatca_itemlist_item = new zatca_itemlist_item();
                                        zatca_itemlist_item.Name = ItemDesc;
                                        zatca_itemlist_item.SellersItemID = "";
                                        zatca_itemlist_item.BuyerItemID = "";
                                        zatca_itemlist_item.StdItemID = "";
                                        zatca_itemlist_item.Name_AR = ItemDescAR;
                                        zatca_itemlist_item.SellersItemID_AR = "";
                                        zatca_itemlist_item.BuyerItemID_AR = "";
                                        zatca_itemlist_item.StdItemID_AR = "";

                                        zatca_itemlist_item_clastaxcat zatca_itemlist_item_clastaxcat = new zatca_itemlist_item_clastaxcat();
                                        zatca_itemlist_item_clastaxcat.ID = VatGroup;
                                        zatca_itemlist_item_clastaxcat.Percent = Vatrate.ToString();
                                        zatca_itemlist_item_clastaxcat.TaxExemptionReasonCd = "";
                                        zatca_itemlist_item_clastaxcat.TaxExemptionReason = "";
                                        zatca_itemlist_item_clastaxcat.ID_AR = VatGroup;
                                        zatca_itemlist_item_clastaxcat.Percent_AR = Vatrate.ToString();
                                        zatca_itemlist_item_clastaxcat.TaxExemptionReasonCd_AR = "";
                                        zatca_itemlist_item_clastaxcat.TaxExemptionReason_AR = "";
                                        zatca_itemlist_item.ClasTaxCat = zatca_itemlist_item_clastaxcat;

                                        zatca_itemlist_item_price zatca_itemlist_item_price = new zatca_itemlist_item_price();
                                        zatca_itemlist_item_price.PriceAmt = price;
                                        zatca_itemlist_item_price.BaseQty = "1";
                                        zatca_itemlist_item_price.BaseQtyUoM = "";
                                        zatca_itemlist_item_price.BaseQtyUoM_AR = "";
                                        zatca_itemlist_item.Price = zatca_itemlist_item_price;

                                        zatca_itemlist_item_alwchg zatca_itemlist_item_alwchg = new zatca_itemlist_item_alwchg();
                                        zatca_itemlist_item_alwchg.AlwChgReason = "";
                                        zatca_itemlist_item_alwchg.Amt = "0";
                                        zatca_itemlist_item_alwchg.BaseAmt = ( price).ToString();
                                        zatca_itemlist_item_alwchg.BaseAmt_AR = "";
                                        zatca_itemlist_item.AlwChg = zatca_itemlist_item_alwchg;

                                        zatca_itemlist_detail_vm.Item = zatca_itemlist_item;
                                        list5.Add(zatca_itemlist_detail_vm);
                                        Zatca_Details_VM_obj.InvLine = list5;
                                        oRs.MoveNext();
                                        #endregion
                                    }

                                    zatca_alwchg zatca_Alwchg = new zatca_alwchg();
                                    List<zatca_alwchg> list6 = new List<zatca_alwchg>();
                                    zatca_Alwchg.Amt = "0";
                                    zatca_Alwchg.BaseAmt = "0";
                                    zatca_Alwchg.MFN = "0";
                                    zatca_Alwchg.AlwChgReason = "";
                                    zatca_Alwchg.Indicator = "";
                                    list6.Add(zatca_Alwchg);
                                    Zatca_Details_VM_obj.AlwChg = list6;

                                    zatca_legalmonetarytotal zatca_legalmonetarytotal = new zatca_legalmonetarytotal();
                                    zatca_legalmonetarytotal.LineExtAmt = "0";
                                    zatca_legalmonetarytotal.AlwTotalAmt = "0";
                                    zatca_legalmonetarytotal.TaxExclAmt = "0";
                                    zatca_legalmonetarytotal.TaxInclAmt = "0";
                                    zatca_legalmonetarytotal.PrepaidAmt = "0";
                                    zatca_legalmonetarytotal.PayableAmt = "0";
                                    zatca_legalmonetarytotal.ChgTotalAmt = "0";
                                    Zatca_Details_VM_obj.LegalMonetaryTotal = zatca_legalmonetarytotal;

                                    var main = JsonConvert.SerializeObject(Zatca_Details_VM_obj);
                                    main = main.Replace("_", ".");
                                    string qry = "";
                                    if (clsMainClass.oCompany.DbServerType == SAPbobsCOM.BoDataServerTypes.dst_HANADB)
                                    {
                                        qry = "Select \"U_API\",(Select \"U_API\" from \"@API_HEADER\" WHERE \"DocEntry\" = '3') as \"PDFAPI\" " +
                                               "from \"@API_HEADER\" A\r\n" +
                                               "left JOIN \"@API_DETAIL\" B on A.\"DocEntry\" = B.\"DocEntry\"\r\n" +
                                               "WHERE A.\"DocEntry\" = '2'";
                                    }
                                    ors2.DoQuery(qry);
                                    string INVAPI = ors2.Fields.Item("U_API").Value.ToString();
                                    string PDFAPI = ors2.Fields.Item("PDFAPI").Value.ToString();

                                    var token = GetToken(API, ClientID, ClientSecret);
                                    ZatcaInvoiceDataResponse(main, token, formID, INVAPI, RefNum);
                                    FileIndex(PDFAPI, token, Finyr, InvTypCd, docNum,RefNum);
                                }
                                catch (Exception ex)
                                {
                                    clsMainClass.SBO_Application.StatusBar.SetText(ex.Message, SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Warning);
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                        }
                    }
                    catch { }

                    if (oVariables.oForm.Mode == SAPbouiCOM.BoFormMode.fm_UPDATE_MODE)
                    {
                        oVariables.oForm.Mode = SAPbouiCOM.BoFormMode.fm_OK_MODE;
                    }

                    clsMainClass.SBO_Application.Menus.Item("1304").Activate();
                    #endregion
                }
                catch (Exception ex)
                {
                    clsMainClass.SBO_Application.StatusBar.SetText(ex.Message, SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Error);
                    BubbleEvent = false;
                    return;
                }
            }
        }

        public void SBO_Application_ItemEvent(string FormTypeEx, ref SAPbouiCOM.ItemEvent pVal, out bool BubbleEvent)
        {
            BubbleEvent = true;
            try
            {
                #region pVal.BeforeAction == true
                if (pVal.BeforeAction == true)
                {
                    if (pVal.EventType == SAPbouiCOM.BoEventTypes.et_CLICK)
                    {
                        if (pVal.ItemUID == "Zatca")
                        {
                            #region Generate Zatca-Invoice Validation
                            //oVariables.Soft_Form = clsMainClass.SBO_Application.Forms.GetForm(oBusinessObjectInfo.FormTypeEx, clsMainClass.SBO_Application.Forms.ActiveForm.TypeCount);
                            oVariables.oForm = clsMainClass.SBO_Application.Forms.GetForm(FormTypeEx, clsMainClass.SBO_Application.Forms.ActiveForm.TypeCount);
                            SAPbobsCOM.Recordset ors3 = clsMainClass.oCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset);
                            string SQuery1 = "";
                            oVariables.oEdit = (SAPbouiCOM.EditText)oVariables.oForm.Items.Item("4").Specific;
                            string cardcode = oVariables.oEdit.Value;
                            if (clsMainClass.oCompany.DbServerType == SAPbobsCOM.BoDataServerTypes.dst_HANADB)
                            {
                                SQuery1 = @"Select C.""CardFName"" as ""Buyer Name"",C.""CardName"" as ""Buyer Name AR"",'CRN' as ""Buyer Schema ID"",C.""LicTradNum"" as ""Party ID"",
                                    C.""RegNum"" as ""Buyer ID"",D.""Address"" as ""Buyer Street Name"",""Address3"" as ""Buyer AdlStrName"",D.""Street"" as ""Buyer Building"",
                                    D.""City"" as ""Buyer CityNm"",D.""ZipCode"" as ""Buyer ZipCode"",D.""County"",(Select ""Building"" from ADM1) as ""Building No AR"",
                                    (Select CRD1.""Address"" from CRD1 where C.""CardCode""=CRD1.""CardCode"" and ""AdresType""='S') as ""Buyer Street Name AR"",
                                    (Select CRD1.""Address3"" from CRD1 where C.""CardCode""=CRD1.""CardCode"" and ""AdresType""='S') as ""Buyer AdlStrName AR"",
                                    (Select CRD1.""Street"" from CRD1 where C.""CardCode""=CRD1.""CardCode"" and ""AdresType""='S') as ""Buyer Building AR"",
                                    (Select CRD1.""City"" from CRD1 where C.""CardCode""=CRD1.""CardCode"" and ""AdresType""='S') as ""Buyer CityNm AR"",
                                    (Select CRD1.""ZipCode"" from CRD1 where C.""CardCode""=CRD1.""CardCode"" and ""AdresType""='S') as ""Buyer ZipCode AR""                                                           
                                    from OCRD C 
                                    INNER JOIN CRD1 D on C.""CardCode"" = D.""CardCode"" and ""AdresType"" = 'B'
                                    where C.""CardCode""='" + cardcode + "'";
                            }
                            else
                            {
                                SQuery1 = @"Select C.""CardFName"" as ""Buyer Name"",C.""CardName"" as ""Buyer Name AR"",'CRN' as ""Buyer Schema ID"",C.""LicTradNum"" as ""Party ID"",
                                    C.""RegNum"" as ""Buyer ID"",D.""Address"" as ""Buyer Street Name"",""Address3"" as ""Buyer AdlStrName"",D.""Street"" as ""Buyer Building"",
                                    D.""City"" as ""Buyer CityNm"",D.""ZipCode"" as ""Buyer ZipCode"",D.""County"",(Select ""Building"" from ADM1) as ""Building No AR"",
                                    (Select CRD1.""Address"" from CRD1 where C.""CardCode""=CRD1.""CardCode"" and ""AdresType""='S') as ""Buyer Street Name AR"",
                                    (Select CRD1.""Address3"" from CRD1 where C.""CardCode""=CRD1.""CardCode"" and ""AdresType""='S') as ""Buyer AdlStrName AR"",
                                    (Select CRD1.""Street"" from CRD1 where C.""CardCode""=CRD1.""CardCode"" and ""AdresType""='S') as ""Buyer Building AR"",
                                    (Select CRD1.""City"" from CRD1 where C.""CardCode""=CRD1.""CardCode"" and ""AdresType""='S') as ""Buyer CityNm AR"",
                                    (Select CRD1.""ZipCode"" from CRD1 where C.""CardCode""=CRD1.""CardCode"" and ""AdresType""='S') as ""Buyer ZipCode AR""                                                           
                                    from OCRD C 
                                    INNER JOIN CRD1 D on C.""CardCode"" = D.""CardCode"" and ""AdresType"" = 'B'
                                    where C.""CardCode""='" + cardcode + "'";
                            }

                            ors3.DoQuery(SQuery1);
                            int i = 0;
                            if (ors3.RecordCount > 0)
                            {
                                string BuyerName = ors3.Fields.Item("Buyer Name").Value.ToString();
                                string BuyerNameAR = ors3.Fields.Item("Buyer Name AR").Value.ToString();
                                string BSchemaID = ors3.Fields.Item("Buyer Schema ID").Value.ToString();
                                string BuyerID = ors3.Fields.Item("Buyer ID").Value.ToString();
                                string BStreetNM = ors3.Fields.Item("Buyer Street Name").Value.ToString();
                                string BAdlStrName = ors3.Fields.Item("Buyer AdlStrName").Value.ToString();
                                string BBuildingNo = ors3.Fields.Item("Buyer Building").Value.ToString();
                                string BCity = ors3.Fields.Item("Buyer CityNm").Value.ToString();
                                string BZipCode = ors3.Fields.Item("Buyer ZipCode").Value.ToString();
                                string BStreetNMAR = ors3.Fields.Item("Buyer Street Name AR").Value.ToString();
                                string BAdlStrNameAR = ors3.Fields.Item("Buyer AdlStrName AR").Value.ToString();
                                string BBuildingNoAR = ors3.Fields.Item("Buyer Building AR").Value.ToString();
                                string BCityAR = ors3.Fields.Item("Buyer CityNm AR").Value.ToString();
                                string BZipCodeAR = ors3.Fields.Item("Buyer ZipCode AR").Value.ToString();
                                string BPartyID = ors3.Fields.Item("Party ID").Value.ToString();

                                if (BuyerName == "")
                                {
                                    clsMainClass.SBO_Application.StatusBar.SetText("Buyer Name should not be blank", SAPbouiCOM.BoMessageTime.bmt_Long, SAPbouiCOM.BoStatusBarMessageType.smt_Error);
                                    //clsMainClass.StatusMessage = "Buyer Name should not be blank";
                                    BubbleEvent = false;
                                    return;
                                }
                                if (BuyerNameAR == "")
                                {
                                    clsMainClass.SBO_Application.StatusBar.SetText("Buyer Name in arabice should not be blank", SAPbouiCOM.BoMessageTime.bmt_Long, SAPbouiCOM.BoStatusBarMessageType.smt_Error);
                                    //clsMainClass.SBO_Application.StatusBar.SetText("Buyer ID should not be blank", SAPbouiCOM.BoMessageTime.bmt_Long, SAPbouiCOM.BoStatusBarMessageType.smt_Error);
                                    //clsMainClass.StatusMessage = "Buyer Name in arabic should not be blank";
                                    BubbleEvent = false;
                                    return;
                                }
                                if (BSchemaID == "")
                                {
                                    clsMainClass.SBO_Application.StatusBar.SetText("Buyer Schema ID should not be blank", SAPbouiCOM.BoMessageTime.bmt_Long, SAPbouiCOM.BoStatusBarMessageType.smt_Error);
                                    //clsMainClass.StatusMessage = "Buyer Schema ID should not be blank";
                                    BubbleEvent = false;
                                    return;
                                }
                                if (BuyerID == "")
                                {
                                    //clsMainClass.SBO_Application.StatusBar.SetText("Buyer ID should not be blank", SAPbouiCOM.BoMessageTime.bmt_Long, SAPbouiCOM.BoStatusBarMessageType.smt_Error);
                                    //clsMainClass.StatusMessage = "Buyer ID should not be blank";
                                    //BubbleEvent = false;
                                    //return;
                                }
                                if (BStreetNM == "")
                                {
                                    clsMainClass.SBO_Application.StatusBar.SetText("Buyer Address should not be blank", SAPbouiCOM.BoMessageTime.bmt_Long, SAPbouiCOM.BoStatusBarMessageType.smt_Error);
                                    //clsMainClass.StatusMessage = "Buyer Address should not be blank";
                                    BubbleEvent = false;
                                    return;
                                }
                                if (BAdlStrName == "")
                                {
                                    clsMainClass.SBO_Application.StatusBar.SetText("Buyer Address 3 should not be blank", SAPbouiCOM.BoMessageTime.bmt_Long, SAPbouiCOM.BoStatusBarMessageType.smt_Error);
                                    //clsMainClass.StatusMessage = "Buyer Adress 3 should not be blank";
                                    BubbleEvent = false;
                                    return;
                                }
                                if (BBuildingNo == "")
                                {
                                    clsMainClass.SBO_Application.StatusBar.SetText("Buyer Street should not be blank", SAPbouiCOM.BoMessageTime.bmt_Long, SAPbouiCOM.BoStatusBarMessageType.smt_Error);
                                    //clsMainClass.StatusMessage = "Buyer Street should not be blank";
                                    BubbleEvent = false;
                                    return;
                                }
                                if (BCity == "")
                                {
                                    clsMainClass.SBO_Application.StatusBar.SetText("Buyer city should not be blank", SAPbouiCOM.BoMessageTime.bmt_Long, SAPbouiCOM.BoStatusBarMessageType.smt_Error);
                                    //clsMainClass.StatusMessage = "Buyer city should not be blank";
                                    BubbleEvent = false;
                                    return;
                                }
                                if (BZipCode == "")
                                {
                                    clsMainClass.SBO_Application.StatusBar.SetText("Buyer zipcode should not be blank", SAPbouiCOM.BoMessageTime.bmt_Long, SAPbouiCOM.BoStatusBarMessageType.smt_Error);
                                    //clsMainClass.StatusMessage = "Buyer zipcode should not be blank";
                                    BubbleEvent = false;
                                    return;
                                }
                                if (BStreetNMAR == "")
                                {
                                    clsMainClass.SBO_Application.StatusBar.SetText("Buyer Adress in arabic should not be blank", SAPbouiCOM.BoMessageTime.bmt_Long, SAPbouiCOM.BoStatusBarMessageType.smt_Error);
                                    BubbleEvent = false;
                                    return;
                                }
                                if (BAdlStrNameAR == "")
                                {
                                    //clsMainClass.SBO_Application.StatusBar.SetText("Buyer ID should not be blank", SAPbouiCOM.BoMessageTime.bmt_Long, SAPbouiCOM.BoStatusBarMessageType.smt_Error);
                                    clsMainClass.SBO_Application.StatusBar.SetText("Buyer Address 3 in arabic should not be blank", SAPbouiCOM.BoMessageTime.bmt_Long, SAPbouiCOM.BoStatusBarMessageType.smt_Error);
                                    BubbleEvent = false;
                                    return;
                                }
                                if (BBuildingNoAR == "")
                                {
                                    clsMainClass.SBO_Application.StatusBar.SetText("Buyer Street in arabic should not be blank", SAPbouiCOM.BoMessageTime.bmt_Long, SAPbouiCOM.BoStatusBarMessageType.smt_Error);
                                    BubbleEvent = false;
                                    return;
                                }
                                if (BCityAR == "")
                                {
                                    clsMainClass.SBO_Application.StatusBar.SetText("Buyer city in arabic should not be blank", SAPbouiCOM.BoMessageTime.bmt_Long, SAPbouiCOM.BoStatusBarMessageType.smt_Error);
                                    BubbleEvent = false;
                                    return;
                                }
                                if (BPartyID == "")
                                {
                                    clsMainClass.SBO_Application.StatusBar.SetText("Buyer Party ID should not be blank", SAPbouiCOM.BoMessageTime.bmt_Long, SAPbouiCOM.BoStatusBarMessageType.smt_Error);
                                    BubbleEvent = false;
                                    return;
                                }
                            }
                            #endregion
                        }
                    }
                }
                #endregion
                
                #region pVal.BeforeAction == false
                if (pVal.BeforeAction == false)
                {
                    #region SAPbouiCOM.BoEventTypes.et_FORM_LOAD
                    if (pVal.EventType == SAPbouiCOM.BoEventTypes.et_FORM_LOAD)
                    {
                        oVariables.oForm = clsMainClass.SBO_Application.Forms.GetForm(FormTypeEx, pVal.FormTypeCount);
                        SAPbobsCOM.Recordset ors2 = clsMainClass.oCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset);
                        string SQuery = "";

                        #region Generate Zatca-Invoicing Button
                        oVariables.oItem = oVariables.oForm.Items.Add("Zatca", SAPbouiCOM.BoFormItemTypes.it_BUTTON);
                        oVariables.oForm.Items.Item("Zatca").Left = oVariables.oForm.Items.Item("10000330").Left;
                        oVariables.oForm.Items.Item("Zatca").Top = oVariables.oForm.Items.Item("10000330").Top - oVariables.oForm.Items.Item("10000330").Height - 5;
                        oVariables.oForm.Items.Item("Zatca").Height = oVariables.oForm.Items.Item("10000330").Height;
                        oVariables.oForm.Items.Item("Zatca").Width = oVariables.oForm.Items.Item("10000330").Width;
                        oVariables.oButton = oVariables.oItem.Specific;
                        oVariables.oButton.Item.DisplayDesc = true;
                        oVariables.oButton.Caption = "Zatca-Invoice";
                        #endregion
                    }
                    #endregion

                    #region SAPbouiCOM.BoEventTypes.et_COMBO_SELECT
                    if (pVal.EventType == SAPbouiCOM.BoEventTypes.et_CLICK)
                    {
                        oVariables.oForm = clsMainClass.SBO_Application.Forms.GetForm(FormTypeEx, clsMainClass.SBO_Application.Forms.ActiveForm.TypeCount);
                        try
                        {
                            #region Zatca Invoice Generate
                            if (pVal.ItemUID == "Zatca")
                            {
                                oVariables.oEdit = (SAPbouiCOM.EditText)oVariables.oForm.Items.Item("10").Specific;
                                string docdate = oVariables.oEdit.Value;
                                oVariables.oEdit = (SAPbouiCOM.EditText)oVariables.oForm.Items.Item("8").Specific;
                                string DocNum = oVariables.oEdit.String;

                                #region Zatca-Invoice

                                oVariables.oForm = clsMainClass.SBO_Application.Forms.GetForm(pVal.FormTypeEx, clsMainClass.SBO_Application.Forms.ActiveForm.TypeCount);
                                string fromPlace1 = "", fromState1 = "", fromPlace11 = "", fromState11 = "", docType1 = "", transMode1 = "", transporterName1 = "", 
                                       transporterId1 = "", transDocNo1 = "", transDocDate1 = "", vehicleNo1 = "", vehicleType1 = "";
                                double transDistance1 = 0.0, mndis = 0.0;
                                oVariables.oEdit = (SAPbouiCOM.EditText)oVariables.oForm.Items.Item("8").Specific;
                                string invdocentry = oVariables.oEdit.Value;
                                string formID = oVariables.oForm.TypeEx;
                                string SQuery = "";
                                SAPbobsCOM.Recordset ors2 = clsMainClass.oCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset);
                                SAPbobsCOM.Recordset oRs = clsMainClass.oCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset);
                                oRs = (SAPbobsCOM.Recordset)clsMainClass.oCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset);
                                try
                                {
                                    try
                                    {
                                        string RefNum = "", Finyr = "", ThirdPartyInvoice = "0", NominalInvoice = "", ExportInvoice = "", SummaryInvoice = "",
                                               SelfBilledinvoice = "", SellerName = "", SSchemaID = "", SellerID = "", SStreetNM = "", SStreetAdlNM = "",
                                               SBuildingNo = "", SCity = "", companyid = "", SZipCode = "", SSubDivisionNm = "", VatID = "", SSubDivisionNmAR = "",
                                               SStreetNMAR = "", SStreetAdlNMAR = "", SBuildingNoAR = "", SCityAR = "", SZipCodeAR = "", SellerNameAR = "",
                                               BuyerName = "", BuyerNameAR = "", BSchemaID = "", BuyerID = "", BStreetNM = "", BBuildingNo = "", BCity = "", BPartyID = "",
                                               BZipCode = "", BSubDivisionNm = "", ItemCode = "", ItemDesc = "", ItemDescAR = "", Companyname = "", UOM = "", BAdlStrName = "",
                                               BStreetNMAR = "", BBuildingNoAR = "", BCityAR = "",BCountry="", BZipCodeAR = "", BAdlStrNameAR = "", SVatId = "", VatGroup = "",
                                               InvTypCd = "", InvSubtype = "", deldate = "",remarks="",email="";

                                        int fromPincode = 0, fromPincode1 = 0, fromStateCode = 0, fromStateCode1 = 0, toPincodeS = 0,
                                            toPincodeB = 0, toStateCodeS = 0, toStateCodeB = 0;
                                        double totalValue = 0, othvalue = 0, cgstValue = 0, dis = 0, linetotal = 0, sgstValue = 0, igstValue = 0,
                                               utgstValue = 0, totInvValue = 0, taxableAmount = 0, sgstRate = 0, cgstRate = 0, igstRate = 0,
                                               utgstRate = 0, dtotal = 0, discamt = 0, Vatrate = 0, taxprcnt = 0, quantity = 0, price = 0;

                                        string query = "";
                                        if (clsMainClass.oCompany.DbServerType == SAPbobsCOM.BoDataServerTypes.dst_HANADB)
                                        {
                                            query = "Select \"U_API\",\"U_ClientID\",\"U_ClientSecret\" from \"@API_HEADER\" A\r\n" +
                                                    "INNER JOIN \"@API_DETAIL\" B on A.\"DocEntry\" = B.\"DocEntry\"\r\n" +
                                                    "WHERE A.\"DocEntry\" = '1'";
                                        }
                                        ors2.DoQuery(query);
                                        string API = ors2.Fields.Item("U_API").Value.ToString();
                                        string ClientID = ors2.Fields.Item("U_ClientID").Value.ToString();
                                        string ClientSecret = ors2.Fields.Item("U_ClientSecret").Value.ToString();
                                        string qrSelect = "";

                                        if (clsMainClass.oCompany.DbServerType == SAPbobsCOM.BoDataServerTypes.dst_HANADB)
                                        {
                                            qrSelect = @"Call ZATCA_EINVOICE ('" + docdate + "','" + DocNum + "')";
                                        }

                                        else if (clsMainClass.oCompany.DbServerType != SAPbobsCOM.BoDataServerTypes.dst_HANADB)
                                        {
                                            qrSelect = @"Exec ZATCA_EINVOICE '" + docdate + "','" + DocNum + "'";
                                        }
                                        oRs.DoQuery(qrSelect);

                                        if (!oRs.EoF)
                                        {
                                            try
                                            {
                                                clsMainClass.SBO_Application.StatusBar.SetText("Zatca-Invoice generating please wait for Document No - " + DocNum, SAPbouiCOM.BoMessageTime.bmt_Long, SAPbouiCOM.BoStatusBarMessageType.smt_Success);
                                                deldate = oRs.Fields.Item("date").Value.ToString("yyyy-MM-dd");
                                                RefNum = oRs.Fields.Item("Ref Num").Value.ToString();
                                                Finyr = oRs.Fields.Item("Financial Year").Value.ToString();
                                                InvTypCd = oRs.Fields.Item("InvTypeCd").Value.ToString();
                                                InvSubtype = oRs.Fields.Item("InvSubtype").Value.ToString();
                                                NominalInvoice = oRs.Fields.Item("NominalInvoice").Value.ToString();
                                                ExportInvoice = oRs.Fields.Item("ExportInvoice").Value.ToString();
                                                SummaryInvoice = oRs.Fields.Item("SummaryInvoice").Value.ToString();
                                                SelfBilledinvoice = oRs.Fields.Item("SelfBilledinvoice").Value.ToString();
                                                SellerName = oRs.Fields.Item("Seller Name").Value.ToString();
                                                SSchemaID = oRs.Fields.Item("Seller Schema ID").Value.ToString();
                                                SellerID = oRs.Fields.Item("Seller ID").Value.ToString();
                                                SStreetNM = oRs.Fields.Item("Seller Street Name").Value.ToString();
                                                SStreetAdlNM = oRs.Fields.Item("Seller AdlStreet Name").Value.ToString();
                                                SBuildingNo = oRs.Fields.Item("Building No").Value.ToString();
                                                SCity = oRs.Fields.Item("City").Value.ToString();
                                                SZipCode = oRs.Fields.Item("ZipCode").Value.ToString();
                                                SSubDivisionNm = oRs.Fields.Item("City SubDivision Name").Value.ToString();
                                                SVatId = oRs.Fields.Item("SVatId").Value.ToString();

                                                companyid = oRs.Fields.Item("Company ID").Value.ToString();
                                                Companyname = oRs.Fields.Item("Company name").Value.ToString();
                                                SStreetNMAR = oRs.Fields.Item("Street Name AR").Value.ToString();
                                                SStreetAdlNMAR = oRs.Fields.Item("Seller AdlStreet Name AR").Value.ToString();
                                                SBuildingNoAR = oRs.Fields.Item("Building No AR").Value.ToString();
                                                SCityAR = oRs.Fields.Item("City AR").Value.ToString();
                                                SZipCodeAR = oRs.Fields.Item("ZipCode").Value.ToString();
                                                SSubDivisionNmAR = oRs.Fields.Item("City SubDivision Name AR").Value.ToString();

                                                BuyerName = oRs.Fields.Item("Buyer Name").Value.ToString();
                                                BuyerNameAR = oRs.Fields.Item("Buyer Name AR").Value.ToString();
                                                BSchemaID = oRs.Fields.Item("Buyer Schema ID").Value.ToString();
                                                BuyerID = oRs.Fields.Item("Buyer ID").Value.ToString();
                                                BStreetNM = oRs.Fields.Item("Buyer Street Name").Value.ToString();
                                                BAdlStrName = oRs.Fields.Item("Buyer AdlStrName").Value.ToString();
                                                BBuildingNo = oRs.Fields.Item("Buyer Building").Value.ToString();
                                                BCity = oRs.Fields.Item("Buyer CityNm").Value.ToString();
                                                BCountry = oRs.Fields.Item("Country").Value.ToString();
                                                BZipCode = oRs.Fields.Item("Buyer ZipCode").Value.ToString();
                                                BStreetNMAR = oRs.Fields.Item("Buyer Street Name AR").Value.ToString();
                                                BAdlStrNameAR = oRs.Fields.Item("Buyer AdlStrName AR").Value.ToString();
                                                BBuildingNoAR = oRs.Fields.Item("Buyer Building AR").Value.ToString();
                                                BCityAR = oRs.Fields.Item("Buyer CityNm AR").Value.ToString();
                                                BZipCodeAR = oRs.Fields.Item("Buyer ZipCode AR").Value.ToString();
                                                BPartyID = oRs.Fields.Item("Party ID").Value.ToString();
                                                remarks = oRs.Fields.Item("Remarks").Value.ToString();
                                                email = oRs.Fields.Item("E_Mail").Value.ToString();

                                                Zatca_Details_VM Zatca_Details_VM_obj = new Zatca_Details_VM();
                                                Zatca_Details_VM_obj.ReferenceNumber = DocNum;
                                                Zatca_Details_VM_obj.FinancialYear = Finyr;
                                                Zatca_Details_VM_obj.InvTypeCd = InvTypCd;
                                                Zatca_Details_VM_obj.InvSubtype = InvSubtype;
                                                Zatca_Details_VM_obj.ThirdPartyInvoice = ThirdPartyInvoice;
                                                Zatca_Details_VM_obj.NominalInvoice = NominalInvoice;
                                                Zatca_Details_VM_obj.ExportInvoice = ExportInvoice;
                                                Zatca_Details_VM_obj.SummaryInvoice = SummaryInvoice;
                                                Zatca_Details_VM_obj.SelfBilledinvoice = SelfBilledinvoice;
                                                Zatca_Details_VM_obj.Note = remarks;
                                                Zatca_Details_VM_obj.OrderRef = "";
                                                Zatca_Details_VM_obj.BlngRef = "";
                                                Zatca_Details_VM_obj.BlngRefIssueDt = "";
                                                Zatca_Details_VM_obj.ContractDocRef = "";
                                                Zatca_Details_VM_obj.Delivery_ActualDeliveryDate = deldate;
                                                Zatca_Details_VM_obj.Delivery_LatestDeliveryDate = "";
                                                Zatca_Details_VM_obj.PymtMeansCode = "";
                                                Zatca_Details_VM_obj.PymtMeans_InstructionNoteReason = "";
                                                Zatca_Details_VM_obj.CustEmailID = email;

                                                zatca_seller_detail_json zatca_seller_detail_json = new zatca_seller_detail_json();
                                                zatca_sellerpartydetails zatca_sellerpartydetails = new zatca_sellerpartydetails();
                                                List<zatca_sellerpartydetails> list = new List<zatca_sellerpartydetails>();

                                                zatca_sellerpartydetails.SchemeID = "CRN"; //Seller CR No
                                                zatca_sellerpartydetails.PartyID = "";     //Seller Group VAT No
                                                zatca_sellerpartydetails.SellerIDNumber = SellerID; //Seller CR No
                                                zatca_sellerpartydetails.SchemeID_AR = "";
                                                zatca_sellerpartydetails.PartyID_AR = "";
                                                zatca_sellerpartydetails.SellerIDNumber_AR = "";
                                                zatca_seller_detail_json.Party = zatca_sellerpartydetails;

                                                zatca_sellerpostaladdress zatca_sellerpostaladdress = new zatca_sellerpostaladdress();
                                                zatca_sellerpostaladdress.SellerCode = "";
                                                zatca_sellerpostaladdress.StrName = SStreetNM;
                                                zatca_sellerpostaladdress.AdlStrName = SStreetAdlNM;
                                                zatca_sellerpostaladdress.PlotIdentification = "";
                                                zatca_sellerpostaladdress.BldgNumber = SBuildingNo;
                                                zatca_sellerpostaladdress.CityName = SCity;
                                                zatca_sellerpostaladdress.PostalZone = SZipCode;
                                                zatca_sellerpostaladdress.CntrySubentityCd = "";
                                                zatca_sellerpostaladdress.CitySubdivisionName = SSubDivisionNm;
                                                zatca_sellerpostaladdress.StrName_AR = SStreetNMAR;
                                                zatca_sellerpostaladdress.AdlStrName_AR = SStreetAdlNMAR;
                                                zatca_sellerpostaladdress.PlotIdentification_AR = "";
                                                zatca_sellerpostaladdress.BldgNumber_AR = SBuildingNoAR;
                                                zatca_sellerpostaladdress.CityName_AR = SCityAR;
                                                zatca_sellerpostaladdress.PostalZone_AR = SZipCodeAR;
                                                zatca_sellerpostaladdress.CntrySubentityCd_AR = "";
                                                zatca_sellerpostaladdress.CitySubdivisionName_AR = "";
                                                zatca_seller_detail_json.PostalAddress = zatca_sellerpostaladdress;
                                                Zatca_Details_VM_obj.ActngSuplParty = zatca_seller_detail_json;

                                                zatca_sellerpartytaxscheme zatca_sellerpartytaxscheme = new zatca_sellerpartytaxscheme();
                                                zatca_sellerpartytaxscheme.CompanyID = companyid; //VAT ID
                                                zatca_sellerpartytaxscheme.CompanyID_AR = "";
                                                zatca_seller_detail_json.PartyTaxScheme = zatca_sellerpartytaxscheme;

                                                zatca_sellerpartylegalentity zatca_sellerpartylegalentity = new zatca_sellerpartylegalentity();
                                                zatca_sellerpartylegalentity.RegName = SellerName; //Company Name in English
                                                zatca_sellerpartylegalentity.RegName_AR = Companyname; //Company Name in Arabic
                                                zatca_seller_detail_json.PartyLegalEntity = zatca_sellerpartylegalentity;

                                                zatca_buyer_detail_json zatca_buyer_detail_json = new zatca_buyer_detail_json();
                                                zatca_buyerpartydetails zatca_buyerpartydetails = new zatca_buyerpartydetails();
                                                if (BuyerID != "")                                              
                                                    zatca_buyerpartydetails.SchemeID = "CRN";
                                                
                                                else 
                                                    zatca_buyerpartydetails.SchemeID = "";

                                                zatca_buyerpartydetails.PartyID = ""; //Buyer Group VAT Reg No
                                                zatca_buyerpartydetails.BuyerIDNumber = BuyerID; //Buyer CR No
                                                zatca_buyerpartydetails.SchemeID_AR = "";
                                                zatca_buyerpartydetails.PartyID_AR = "";
                                                zatca_buyerpartydetails.BuyerIDNumber_AR = "";
                                                zatca_buyer_detail_json.Party = zatca_buyerpartydetails;

                                                zatca_buyerpostaladdress zatca_buyerpostaladdress = new zatca_buyerpostaladdress();
                                                zatca_buyerpostaladdress.BuyerCode = "";
                                                zatca_buyerpostaladdress.StrName = BStreetNM;
                                                zatca_buyerpostaladdress.AdlStrName = BAdlStrName;
                                                zatca_buyerpostaladdress.PlotIdentification = "";
                                                zatca_buyerpostaladdress.BldgNumber = BBuildingNo;
                                                zatca_buyerpostaladdress.CityName = BCity;
                                                zatca_buyerpostaladdress.PostalZone = BZipCode;
                                                zatca_buyerpostaladdress.CntrySubentityCd = "";
                                                zatca_buyerpostaladdress.CitySubdivisionName = BAdlStrName;
                                                zatca_buyerpostaladdress.Cntry = BCountry;
                                                zatca_buyerpostaladdress.StrName_AR = BStreetNMAR;
                                                zatca_buyerpostaladdress.AdlStrName_AR = BAdlStrNameAR;
                                                zatca_buyerpostaladdress.PlotIdentification_AR = "";
                                                zatca_buyerpostaladdress.BldgNumber_AR = BBuildingNoAR;
                                                zatca_buyerpostaladdress.CityName_AR = BCityAR;
                                                zatca_buyerpostaladdress.PostalZone_AR = BZipCodeAR;
                                                zatca_buyerpostaladdress.CntrySubentityCd_AR = "";
                                                zatca_buyerpostaladdress.CitySubdivisionName_AR = BAdlStrNameAR;
                                                zatca_buyer_detail_json.PostalAddress = zatca_buyerpostaladdress;
                                                Zatca_Details_VM_obj.ActngCustomerParty = zatca_buyer_detail_json;

                                                zatca_buyerpartytaxscheme zatca_buyerpartytaxscheme = new zatca_buyerpartytaxscheme();
                                                zatca_buyerpartytaxscheme.CompanyID = BPartyID; //Buyer VAT No
                                                zatca_buyerpartytaxscheme.CompanyID_AR = "";
                                                zatca_buyer_detail_json.PartyTaxScheme = zatca_buyerpartytaxscheme;

                                                zatca_buyerpartylegalentity zatca_buyerpartylegalentity = new zatca_buyerpartylegalentity();
                                                zatca_buyerpartylegalentity.RegName = BuyerName;
                                                zatca_buyerpartylegalentity.RegName_AR = BuyerNameAR;
                                                zatca_buyer_detail_json.PartyLegalEntity = zatca_buyerpartylegalentity;

                                                double linetotal1 = 0, igstValue1 = 0, sgstValue1 = 0, cgstValue1 = 0, utgstValue1 = 0, othValue1 = 0;

                                                zatca_itemlist_detail_vm zatca_itemlist_detail_vm;
                                                List<zatca_itemlist_detail_vm> list5 = new List<zatca_itemlist_detail_vm>();

                                                for (int k = 0; k < oRs.RecordCount; k++)
                                                {
                                                    #region Item Type
                                                    zatca_itemlist_detail_vm = new zatca_itemlist_detail_vm();
                                                    ItemCode = oRs.Fields.Item("ItemCode").Value.ToString();
                                                    ItemDesc = oRs.Fields.Item("ItemName").Value.ToString();
                                                    ItemDescAR = oRs.Fields.Item("ItemName AR").Value.ToString();
                                                    quantity = double.Parse(oRs.Fields.Item("Quantity").Value.ToString());
                                                    UOM = oRs.Fields.Item("UomCode").Value.ToString();
                                                    price = double.Parse(oRs.Fields.Item("Price").Value.ToString());
                                                    linetotal = double.Parse(oRs.Fields.Item("LineTotal").Value.ToString());
                                                    discamt = double.Parse(oRs.Fields.Item("DiscAmt").Value.ToString());
                                                    Vatrate = double.Parse(oRs.Fields.Item("Rate").Value.ToString());
                                                    VatGroup = oRs.Fields.Item("VAT").Value.ToString();
                                                    taxprcnt = double.Parse(oRs.Fields.Item("Tax Percent").Value.ToString());
                                                    if (VatGroup.Contains("S"))
                                                    {
                                                        VatGroup = "S";
                                                    }
                                                    else if (VatGroup.Contains("E"))
                                                    {
                                                        VatGroup = "E";
                                                    }
                                                    else if (VatGroup.Contains("Z"))
                                                    {
                                                        VatGroup = "Z";
                                                    }
                                                    else if (VatGroup.Contains("O"))
                                                    {
                                                        VatGroup = "O";
                                                    }

                                                    zatca_itemlist_detail_vm.ID = k + 1;
                                                    zatca_itemlist_detail_vm.ItemCode = ItemCode;
                                                    zatca_itemlist_detail_vm.Note = ItemDesc;
                                                    zatca_itemlist_detail_vm.InvQtyUom = UOM;
                                                    zatca_itemlist_detail_vm.InvdQty = quantity.ToString();
                                                    zatca_itemlist_detail_vm.LineExtAmt = (quantity * price).ToString();
                                                    zatca_itemlist_detail_vm.PrepaymentID = "";
                                                    zatca_itemlist_detail_vm.PrepaymentID_UID = "";
                                                    zatca_itemlist_detail_vm.PrepaymentIssueDate = "";
                                                    zatca_itemlist_detail_vm.PrepaymentIssueTime = "";
                                                    zatca_itemlist_detail_vm.PrepaymentDocType = "";
                                                    zatca_itemlist_detail_vm.PaidVATCategoryTaxableAmt = "";
                                                    zatca_itemlist_detail_vm.PaidVATCategoryTaxAmt = "";

                                                    zatca_item_alwchg zatca_item_alwchg = new zatca_item_alwchg();
                                                    List<zatca_item_alwchg> list1 = new List<zatca_item_alwchg>();
                                                    zatca_item_alwchg.Indicator = "";
                                                    zatca_item_alwchg.AlwChgReason = "";
                                                    zatca_item_alwchg.Amt = discamt.ToString();
                                                    zatca_item_alwchg.BaseAmt = (price).ToString();
                                                    zatca_item_alwchg.MFN = "0";
                                                    list1.Add(zatca_item_alwchg);
                                                    zatca_itemlist_detail_vm.AlwChg = list1;

                                                    zatca_item_taxtotal zatca_item_taxtotal = new zatca_item_taxtotal();
                                                    zatca_item_taxtotal.TaxAmt = ((quantity * price) * Vatrate / 100).ToString();
                                                    zatca_item_taxtotal.RoundingAmt = "0";
                                                    zatca_itemlist_detail_vm.TaxTotal = zatca_item_taxtotal;

                                                    zatca_itemlist_item zatca_itemlist_item = new zatca_itemlist_item();
                                                    zatca_itemlist_item.Name = ItemDesc;
                                                    zatca_itemlist_item.SellersItemID = "";
                                                    zatca_itemlist_item.BuyerItemID = "";
                                                    zatca_itemlist_item.StdItemID = "";
                                                    zatca_itemlist_item.Name_AR = ItemDescAR;
                                                    zatca_itemlist_item.SellersItemID_AR = "";
                                                    zatca_itemlist_item.BuyerItemID_AR = "";
                                                    zatca_itemlist_item.StdItemID_AR = "";

                                                    zatca_itemlist_item_clastaxcat zatca_itemlist_item_clastaxcat = new zatca_itemlist_item_clastaxcat();
                                                    zatca_itemlist_item_clastaxcat.ID = VatGroup;
                                                    zatca_itemlist_item_clastaxcat.Percent = Vatrate.ToString();
                                                    zatca_itemlist_item_clastaxcat.TaxExemptionReasonCd = "";
                                                    zatca_itemlist_item_clastaxcat.TaxExemptionReason = "";
                                                    zatca_itemlist_item_clastaxcat.ID_AR = VatGroup;
                                                    zatca_itemlist_item_clastaxcat.Percent_AR = Vatrate.ToString();
                                                    zatca_itemlist_item_clastaxcat.TaxExemptionReasonCd_AR = "";
                                                    zatca_itemlist_item_clastaxcat.TaxExemptionReason_AR = "";
                                                    zatca_itemlist_item.ClasTaxCat = zatca_itemlist_item_clastaxcat;

                                                    zatca_itemlist_item_price zatca_itemlist_item_price = new zatca_itemlist_item_price();
                                                    zatca_itemlist_item_price.PriceAmt = price;
                                                    zatca_itemlist_item_price.BaseQty = "1";
                                                    zatca_itemlist_item_price.BaseQtyUoM = "";
                                                    zatca_itemlist_item_price.BaseQtyUoM_AR = "";
                                                    zatca_itemlist_item.Price = zatca_itemlist_item_price;

                                                    zatca_itemlist_item_alwchg zatca_itemlist_item_alwchg = new zatca_itemlist_item_alwchg();
                                                    zatca_itemlist_item_alwchg.AlwChgReason = "";
                                                    zatca_itemlist_item_alwchg.Amt = "0";
                                                    zatca_itemlist_item_alwchg.BaseAmt = (price).ToString();
                                                    zatca_itemlist_item_alwchg.BaseAmt_AR = "";
                                                    zatca_itemlist_item.AlwChg = zatca_itemlist_item_alwchg;

                                                    zatca_itemlist_detail_vm.Item = zatca_itemlist_item;
                                                    list5.Add(zatca_itemlist_detail_vm);
                                                    Zatca_Details_VM_obj.InvLine = list5;
                                                    oRs.MoveNext();
                                                    #endregion
                                                }

                                                zatca_alwchg zatca_Alwchg = new zatca_alwchg();
                                                List<zatca_alwchg> list6 = new List<zatca_alwchg>();
                                                zatca_Alwchg.Amt = "0";
                                                zatca_Alwchg.BaseAmt = "0";
                                                zatca_Alwchg.MFN = "0";
                                                zatca_Alwchg.AlwChgReason = "";
                                                zatca_Alwchg.Indicator = "";
                                                list6.Add(zatca_Alwchg);
                                                Zatca_Details_VM_obj.AlwChg = list6;

                                                zatca_legalmonetarytotal zatca_legalmonetarytotal = new zatca_legalmonetarytotal();
                                                zatca_legalmonetarytotal.LineExtAmt = "0";
                                                zatca_legalmonetarytotal.AlwTotalAmt = "0";
                                                zatca_legalmonetarytotal.TaxExclAmt = "0";
                                                zatca_legalmonetarytotal.TaxInclAmt = "0";
                                                zatca_legalmonetarytotal.PrepaidAmt = "0";
                                                zatca_legalmonetarytotal.PayableAmt = "0";
                                                zatca_legalmonetarytotal.ChgTotalAmt = "0";
                                                Zatca_Details_VM_obj.LegalMonetaryTotal = zatca_legalmonetarytotal;

                                                var main = JsonConvert.SerializeObject(Zatca_Details_VM_obj);
                                                main = main.Replace("_", ".");
                                                string qry = "";
                                                if (clsMainClass.oCompany.DbServerType == SAPbobsCOM.BoDataServerTypes.dst_HANADB)
                                                {
                                                    qry = "Select \"U_API\",(Select \"U_API\" from \"@API_HEADER\" WHERE \"DocEntry\" = '3') as \"PDFAPI\" " +
                                                           "from \"@API_HEADER\" A\r\n" +
                                                           "left JOIN \"@API_DETAIL\" B on A.\"DocEntry\" = B.\"DocEntry\"\r\n" +
                                                           "WHERE A.\"DocEntry\" = '2'";
                                                }
                                                ors2.DoQuery(qry);
                                                string INVAPI = ors2.Fields.Item("U_API").Value.ToString();
                                                string PDFAPI = ors2.Fields.Item("PDFAPI").Value.ToString();

                                                var token = GetToken(API, ClientID, ClientSecret);
                                                ZatcaInvoiceDataResponse(main, token, formID, INVAPI, RefNum);
                                                FileIndex(PDFAPI,token, Finyr, InvTypCd, DocNum,RefNum);
                                                clsMainClass.SBO_Application.Menus.Item("1304").Activate();
                                            }
                                            catch (Exception ex)
                                            {
                                                clsMainClass.SBO_Application.StatusBar.SetText(ex.Message, SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Warning);
                                            }
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                    }
                                }
                                catch { }

                                if (oVariables.oForm.Mode == SAPbouiCOM.BoFormMode.fm_UPDATE_MODE)
                                {
                                    oVariables.oForm.Mode = SAPbouiCOM.BoFormMode.fm_OK_MODE;
                                }

                                clsMainClass.SBO_Application.Menus.Item("1304").Activate();
                                #endregion
                            }
                            #endregion
                        }
                        catch (Exception ex)
                        {
                            clsMainClass.SBO_Application.StatusBar.SetText(ex.Message, SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Error);
                            return;
                        }
                    }
                    #endregion
                }
                #endregion
            }
            catch (Exception ex)
            {
                clsMainClass.SBO_Application.StatusBar.SetText(ex.Message, SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Error);
                return;
            }
        }

        #region Methods
        public string ZatcaInvoiceDataResponse(string JsonData, string token, string formid, string url,string DocEntry)
        {
            ServicePointManager.Expect100Continue = true;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
            ServicePointManager.ServerCertificateValidationCallback = (sender, x509Certificate, chain, sslPolicyErrors) => true;

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "POST";
            request.KeepAlive = false;
            request.AllowAutoRedirect = false;
            request.Accept = "*/*";
            request.ContentType = "application/json";
            request.Headers.Add("Authorization", "Bearer " + token + "");
            try
            {
                byte[] bytes = Encoding.UTF8.GetBytes(JsonData);
                request.ContentLength = bytes.Length;
                using (var writer = request.GetRequestStream())
                    writer.Write(bytes, 0, bytes.Length);

                WebResponse response = request.GetResponse();
                string result = new StreamReader(response.GetResponseStream()).ReadToEnd();
                dynamic stuff = JsonConvert.DeserializeObject(result);
                string msg = stuff.msg;
                string requestid = stuff.response.requestId;
                if (requestid != "")
                {
                    SAPbobsCOM.Documents ARInvoice;
                    ARInvoice = (SAPbobsCOM.Documents)clsMainClass.oCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oInvoices);
                    if (ARInvoice.GetByKey(Convert.ToInt32(DocEntry)))
                    {                        
                        ARInvoice.UserFields.Fields.Item("U_ReqID").Value = requestid.ToString();
                        ARInvoice.UserFields.Fields.Item("U_msg").Value = msg.ToString();
                        ARInvoice.UserFields.Fields.Item("U_JSON").Value = JsonData.ToString();
                        int lretcode = ARInvoice.Update();
                        if (lretcode != 0)
                        {
                            int Errcode; string ErrMsg;
                            clsMainClass.oCompany.GetLastError(out Errcode, out ErrMsg);
                        }
                    }
                }
                return result;
            }
            catch (WebException ex)
            {
                string message = ex.Message;
                return message;
            }
        }   

        public string GetToken(string url, string clientid, string clientsecret)
        {
            string data = "grant_type=client_credentials&client_id=" + clientid + "&client_secret=" + clientsecret + "";
            using (WebClient client = new WebClient())
            {
                client.Headers[HttpRequestHeader.ContentType] = "application/x-www-form-urlencoded";
                string result = client.UploadString("" + url + "", data);
                dynamic stuff = JsonConvert.DeserializeObject(result);
                string Token = stuff.access_token;
                return Token;
            }
        }

        public string SignedXMLInvoice(string token,string finyr,string invtycd,string refnum)
        {
            //string zatcaencddata = "";
            //int check = 1;
            //string url = "https://ksa.taxilla.com/process/v1/einvoicearksa/reports/KSA-eInvoice-XML-Ouput?financialyear=" + finyr + "&ref_nm=" + refnum + "&invoicetypecode=" + invtycd + "";
            //ServicePointManager.Expect100Continue = true;
            //ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
            //ServicePointManager.ServerCertificateValidationCallback = (sender, x509Certificate, chain, sslPolicyErrors) => true;

            //HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            //request.Method = "GET";
            //request.KeepAlive = false;
            //request.AllowAutoRedirect = false;
            //request.Headers.Add("Authorization", "Bearer " + token + "");
            //try
            //{
            //    WebResponse response = request.GetResponse();
            //    string result = new StreamReader(response.GetResponseStream()).ReadToEnd();

            //    XmlDocument doc = new XmlDocument();
            //    doc.LoadXml(result);
            //    foreach (XmlNode node in doc.DocumentElement.ChildNodes)
            //    {
            //        if (check == 1)
            //        {
            //            string text = node.InnerXml;
            //            String[] spearator = { "<ds:X509Certificate>", "</ds:X509Certificate>" };
            //            Int32 count = 2;
            //            String[] sep = { "</ds:X509Certificate>", "</ds:X509Certificate>" };
            //            Int32 cnt = 2;
            //            String[] strlist = text.Split(spearator, count,
            //                   StringSplitOptions.RemoveEmptyEntries);

            //            foreach (String s in strlist)
            //            {
            //                string dat = s;
            //                String[] strlist1 = dat.Split(sep, cnt,
            //                   StringSplitOptions.RemoveEmptyEntries);

            //                zatcaencddata = strlist1[0];
            //                check++;
            //            }
            //        }
            //    }
            //}

            //catch { }
            return "";// zatcaencddata;
        }

        public string PDFInvoiceResponse(string token, string finyr, string invtycd, string refnum)
        {
            //string url = "https://ksa.taxilla.com/process/v1/einvoicearksa/reports/KSA-eInvoice-A3-PDF?invoicetypecode=" + invtycd + "&financialyear=" + finyr + "&ref_nm=" + refnum + "";

            //ServicePointManager.Expect100Continue = true;
            //ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
            //ServicePointManager.ServerCertificateValidationCallback = (sender, x509Certificate, chain, sslPolicyErrors) => true;

            //HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            //request.Method = "GET";
            //request.ContentType = "application/pdf";
            //request.Headers.Add("Authorization", "Bearer " + token + ""); 
            //var fileName = @"D:\Zatca Development Utkryst\Zatca1.pdf";
            
            //HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            
            //StreamReader streamReader = new StreamReader(response.GetResponseStream());
            //StreamWriter streamWriter = new StreamWriter(fileName, false, Encoding.Default);
            //streamWriter.Write(streamReader.ReadToEnd());
            //streamWriter.Close();


            //FileStream MyFileStream;
            //long FileSize;

            //MyFileStream = new FileStream(fileName, FileMode.Open);
            //FileSize = MyFileStream.Length;

            //byte[] Buffer = new byte[System.Convert.ToInt32(FileSize) + 1];
            //MyFileStream.Read(Buffer, 0, System.Convert.ToInt32(FileSize));
            //MyFileStream.Close();

            //response.ContentType = "application/pdf";                  
            //response.OutputStream.Write(Buffer, 0, FileSize);            
            //response.Close();

            return "";
        }

        public static async Task FileIndex(string url,string token, string finyr, string invtycd, string refnum, string docentry)
        {
            HttpClientHandler clientHandler = new HttpClientHandler();
            var requestUri = "" + url + "invoicetypecode=" + invtycd + "&financialyear=" + finyr + "&ref_nm=" + refnum + "";

            HttpClient client = new HttpClient(clientHandler);
            client.SetBearerToken(token);
            HttpResponseMessage response = await client.GetAsync(requestUri);

            try
            {
                if (response.IsSuccessStatusCode)
                {
                    var output = await response.Content.ReadAsByteArrayAsync();
                    string pdfname = DateTime.Now.ToString("dd-M-yyyy", CultureInfo.InvariantCulture) + '_' + finyr + '_' + refnum;
                    var path = "\\\\192.168.20.100\\shr_other\\SAP\\Zatca Invoice PDF Folder\\" + pdfname + ".pdf";
                    System.IO.File.WriteAllBytes(path, output);
                    clsMainClass.SBO_Application.StatusBar.SetText("PDF created successfully", SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Success);
                    SAPbobsCOM.Documents ARInvoice;
                    ARInvoice = (SAPbobsCOM.Documents)clsMainClass.oCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oInvoices);
                    if (ARInvoice.GetByKey(Convert.ToInt32(docentry)))
                    {
                        ARInvoice.UserFields.Fields.Item("U_Error").Value = "PDF saved successfully";
                        int lretcode = ARInvoice.Update();
                        if (lretcode != 0)
                        {
                            int Errcode; string ErrMsg;
                            clsMainClass.oCompany.GetLastError(out Errcode, out ErrMsg);
                        }
                    }
                }
                else
                {
                    var result = await response.Content.ReadAsStringAsync();
                    SAPbobsCOM.Documents ARInvoice;
                    ARInvoice = (SAPbobsCOM.Documents)clsMainClass.oCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oInvoices);
                    if (ARInvoice.GetByKey(Convert.ToInt32(docentry)))
                    {
                        ARInvoice.UserFields.Fields.Item("U_Error").Value = result.ToString();
                        int lretcode = ARInvoice.Update();
                        if (lretcode != 0)
                        {
                            int Errcode; string ErrMsg;
                            clsMainClass.oCompany.GetLastError(out Errcode, out ErrMsg);
                        }
                    }
                }
            }
            catch (Exception ex) {
                clsMainClass.SBO_Application.StatusBar.SetText("PDF generation error", SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Error);
            }
        }
        #endregion
    }
}
