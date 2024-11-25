using System;
using System.Windows;
using System.Collections;
using Onix.Client.Model;
using Onix.Client.Report;
using Onix.OnixHttpClient;
using System.Collections.ObjectModel;
using System.Printing;
using Onix.Client.Helper;
using System.Windows.Controls;
using Onix.ClientCenter.UI.HumanResource.PayrollDocument;
using Onix.ClientCenter.UI.HumanResource.OTDocument;
using Microsoft.Office.Interop.Excel;
using Onix.Client.Controller;
using Onix.ClientCenter.Forms.AcDesign.HRPayrollSlip;

namespace Onix.ClientCenter.Forms.AcDesign.HROtDetails
{
    public class CFormOtDetails : CBaseReport
    {
        public CFormOtDetails() : base()
        {
        }

        protected override UserControl createPageObject(Size s, int pageIdx, int pageCount, CReportPageParam param)
        {
            var m = (MVOTDocument) dataSource;
            int idx = pageIdx - 1;

            UserControl page = new UFormOtDetailsP1(m, pageIdx, pageCount, rptCfg, param);
            if (pageIdx == 2)
            {
                page = new UFormOtDetailsP2(m, pageIdx, pageCount, rptCfg, param);
            }

            page.Width = rptCfg.AreaWidthDot;
            page.Height = rptCfg.AreaHeightDot;
            page.Measure(s);

            return (page);
        }

        public override PageOrientation GetPageOrientation()
        {
            return (PageOrientation.Portrait);
        }

        protected override ArrayList createPageParam()
        {
            ArrayList arr = new ArrayList();

            var param1 = new CReportPageParam
            {
                StartIndex = 1,
                EndIndex = 1,
                TotalItemCount = 1,
            };

            var param2 = new CReportPageParam
            {
                StartIndex = 1,
                EndIndex = 1,
                TotalItemCount = 1,
            };

            arr.Add(param1);
            arr.Add(param2);

            return (arr);
        }

        protected override void initPageCreateFlow()
        {
            IsPageRangeSupport = true;
        }
        
        public override MReportConfig CreateDefaultConfigValues()
        {
            MReportConfig rc = new MReportConfig(new CTable(""));

            rc.SetConfigValue("DocumentTypeThai", "ใบแจ้งเงินเดือน", "String", "Document Type Thai");
            rc.SetConfigValue("DocumentTypeEng", "Payroll Slip", "String", "Document Type Eng");

            rc.SetConfigValue("Filler1Height", "10", "String", "Heigh of top space filler");
            rc.SetConfigValue("Filler2Height", "10", "String", "Heigh of area above tax ID");
            rc.SetConfigValue("Filler3Height", "0", "String", "Heigh of area below tax ID");

            populateDefaultReportConfig(rc);

            return (rc);
        }
    }
}
