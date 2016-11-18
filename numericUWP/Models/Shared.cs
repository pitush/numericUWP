using System;
using System.Linq;

namespace SharedClasses
{
    public class ClsNumTextTagIn
    {
        private class cTxtBoxProperty
        {
            public int iBeg { get; set; }
            public int iSel { get; set; }
            public int iLen { get; set; }
            public string sLef { get; set; }
            public string sRig { get; set; }
            public string sSel { get; set; }
            public string sTxt { get; set; }
            public cTxtBoxProperty(Windows.UI.Xaml.Controls.TextBox sender)
            {
                iBeg = sender.SelectionStart;
                iSel = sender.SelectionLength;
                iLen = sender.Text.Length;
                sLef = sender.Text.Substring(0, sender.SelectionStart);
                sRig = sender.Text.Substring(sender.SelectionStart + sender.SelectionLength);
                sSel = sender.SelectedText;
                sTxt = sender.Text;
            }
        }

        private cTxtBoxProperty org { get; set; }
        public string MyPattern { get; set; }
        public bool IsNumOnly { get; set; }
        public bool IsDotSepa { get; set; }
        public bool CanNegNum { get; set; }
        public System.Collections.Generic.List<string> LstRemStr { get; set; }
        public object Tag { get; set; } //use this Tag

        public ClsNumTextTagIn()
        {
            //org = new cTxtBoxProperty(sender);
            MyPattern = "N2";
            IsNumOnly = true;
            IsDotSepa = true;
            CanNegNum = true;
            LstRemStr = new System.Collections.Generic.List<string>();
            //Tag = anyTag; //use this Tag
        }
        /// <summary>
        /// Routine to test numeric if current TextChanging is numeric and display accordinly to current culture.
        /// </summary>
        /// <param name="sender">Target TextBox</param>
        /// <param name="pattern">Currently only Numeric Pattern is supported eg: "N2", "N6"...etc. N stands for number.</param>
        /// <param name="numbersOnly">true = allow numbers only; false = leave text if its not numeric decimal.</param>
        /// <param name="dotAsDecimalSeparator">true = "." counts as decimal separator (regardless of culture) and it will go for decimal part of pattern (if any).</param>
        /// <param name="allowNegativeNumbers">true = can have negative numbers, false = prevents negative numbers.</param>
        /// <param name="lstRemoveStrings">List of any other strings you wish remove eg: "(", " / ", ")", " - ", " + "...etc, if none just pass "null" parameter.</param>
        /// <param name="anyTag">Use paramenter to pass some object in Tag, if none just pass "null" parameter.</param>
        public ClsNumTextTagIn(Windows.UI.Xaml.Controls.TextBox sender, string pattern, bool numbersOnly, bool dotAsDecimalSeparator, bool allowNegativeNumbers, System.Collections.Generic.List<string> lstRemoveStrings, System.Object anyTag)
        {
            org = new cTxtBoxProperty(sender);
            MyPattern = pattern;
            IsNumOnly = numbersOnly;
            IsDotSepa = dotAsDecimalSeparator;
            CanNegNum = allowNegativeNumbers;
            LstRemStr = lstRemoveStrings;
            Tag = anyTag; //use this Tag
        }
        public void Refresh(Windows.UI.Xaml.Controls.TextBox sender)
        {
            org = new cTxtBoxProperty(sender);
        }
        public void NumericText(Windows.UI.Xaml.Controls.TextBox sender)
        { //logics
            if (org == null)
            {
                org = new cTxtBoxProperty(sender);
            }

            System.Globalization.CultureInfo ci = System.Globalization.CultureInfo.CurrentUICulture;
            cTxtBoxProperty edi = new cTxtBoxProperty(sender);
            string sPtn = MyPattern;
            string sAdd = string.Empty;
            string sSub = string.Empty;
            int iChg = edi.iLen - org.iBeg - org.sRig.Length;

            if (iChg >= 0)
            {
                sAdd = edi.sTxt.Substring(org.iBeg, iChg);
                sSub = org.sSel;
            }
            else
            { //backspace?
                if (org.iLen >= edi.iLen)
                {
                    if (org.iBeg - (org.iLen - edi.iLen) >= 0)
                    {
                        sSub = org.sTxt.Substring(org.iBeg - (org.iLen - edi.iLen), (org.iLen - edi.iLen));
                    }
                }
            }

            int iBgx = edi.iBeg;
            int iSlx = 0;
            string sFnl = org.sTxt;

            int iNeg = sFnl.IndexOf(ci.NumberFormat.NegativeSign);
            int iDot = sFnl.IndexOf(ci.NumberFormat.NumberDecimalSeparator);

            if (sAdd == ci.NumberFormat.NegativeSign)
            {
                if (iNeg == -1)
                { //add
                    if (CanNegNum)
                    {//accepts negative numbers
                        if (ci.TextInfo.IsRightToLeft)
                        {
                            sFnl = sFnl + ci.NumberFormat.NegativeSign;
                            iBgx = edi.iBeg - ci.NumberFormat.NegativeSign.Length;
                        }
                        else
                        {
                            sFnl = ci.NumberFormat.NegativeSign + sFnl;
                            //iBgx = edi.iBeg;
                        }
                    }
                    else
                    {//does not accepts negative numbers


                        if (ci.TextInfo.IsRightToLeft)
                        {
                            iBgx = edi.iBeg - ci.NumberFormat.NegativeSign.Length;
                        }
                        else
                        {
                            iBgx = edi.iBeg;
                        }

                        
                    }
                }
                else
                {//remove
                    sFnl = sFnl.Remove(iNeg, ci.NumberFormat.NegativeSign.Length);
                    if (iNeg >= iBgx)
                    {
                        iBgx = edi.iBeg - ci.NumberFormat.NegativeSign.Length;
                    }
                    else
                    {
                        if (ci.TextInfo.IsRightToLeft)
                        {
                            iBgx = edi.iBeg - ci.NumberFormat.NegativeSign.Length;
                        }
                        else
                        {
                            iBgx = edi.iBeg - ci.NumberFormat.NegativeSign.Length - ci.NumberFormat.NegativeSign.Length;
                        }
                    }
                }
            } //end NegativeSign
            else if (sAdd == ci.NumberFormat.NumberDecimalSeparator)
            { //NumberDecimalSeparator
                if (iDot == -1)
                { //add
                    sFnl = edi.sTxt;
                }
                else
                { //go to point
                    sFnl = org.sTxt;
                    iBgx = iDot + ci.NumberFormat.NumberDecimalSeparator.Length;
                }
            }
            else if (sAdd == ".")
            { //dotAsDecimalSeparator
                if (IsDotSepa)
                {
                    if (iDot == -1)
                    { //add
                        sFnl = edi.sTxt;
                    }
                    else
                    { //go to point
                        sFnl = org.sTxt;
                        iBgx = iDot + ci.NumberFormat.NumberDecimalSeparator.Length;
                    }
                }
                else
                {
                    sFnl = edi.sTxt; //override
                }
            }
            else if (sSub == ci.NumberFormat.NumberGroupSeparator & org.iSel == 0)
            { //backspace and GroupSeparator
                if (edi.iBeg == 0)
                {
                    sFnl = edi.sTxt;
                }
                else
                {
                    sFnl = edi.sLef.Replace(ci.NumberFormat.NumberGroupSeparator, string.Empty);
                    int iRem = edi.sLef.Length - sFnl.Length;
                    sFnl = sFnl.Substring(0, edi.sLef.Length - iRem - 1) + edi.sRig;
                    iBgx = iBgx - iRem - 1;
                }
            }//end backspace and GroupSeparator
            else
            {
                sFnl = edi.sTxt;
            }

            if (iBgx > sFnl.Length)
            {
                iBgx = sFnl.Length;
            }
            string sLfx = sFnl.Substring(0, iBgx);
            string sRgx = sFnl.Substring(iBgx);

            sRgx = sRgx.Replace(ci.NumberFormat.NumberGroupSeparator, string.Empty);
            sFnl = sFnl.Replace(ci.NumberFormat.NumberGroupSeparator, string.Empty);

            if (LstRemStr != null)
            {
                foreach (string sRem in LstRemStr)
                {
                    sRgx = sRgx.Replace(sRem, string.Empty); //automatically trimmed
                    sFnl = sFnl.Replace(sRem, string.Empty); //automatically trimmed
                }
            }

            //s += "Rgx: " + sRgx.ToString() + Environment.NewLine;
            //s += "Fnl: " + sFnl.ToString() + Environment.NewLine;

            if (!CanNegNum)
            {//remove negative sign
                sRgx = sRgx.Replace(ci.NumberFormat.NegativeSign, string.Empty);
                sFnl = sFnl.Replace(ci.NumberFormat.NegativeSign, string.Empty);
            }

            decimal dFnl = decimal.Zero;
            decimal dRgx = decimal.Zero;
            if (decimal.TryParse(sFnl, out dFnl))
            {

                int iDtx = sFnl.IndexOf(ci.NumberFormat.NumberDecimalSeparator);
                int iDif = sFnl.Length - dFnl.ToString().Length;

                if (iDtx == -1)
                {//DecimalSeparator does not exist
                    sFnl = dFnl.ToString(sPtn);
                    int iTmp = sFnl.IndexOf(ci.NumberFormat.NumberDecimalSeparator);
                    if (iTmp == -1)
                    {//DecimalSeparator not found after apply pattern
                        if (sRgx == string.Empty)
                        {
                            iBgx = sFnl.Length;
                        }
                        else
                        {
                            if (sRgx.StartsWith(decimal.Zero.ToString()))
                            {
                                sRgx = decimal.One.ToString() + sRgx;
                                if (decimal.TryParse(sRgx, out dRgx))
                                {
                                    sRgx = dRgx.ToString(sPtn);
                                    if (sRgx.StartsWith(decimal.One.ToString() + ci.NumberFormat.NumberDecimalSeparator))
                                    {
                                        iBgx = sFnl.Length - sRgx.Length + (decimal.One.ToString() + ci.NumberFormat.NumberDecimalSeparator).Length;
                                    }
                                    else
                                    {
                                        iBgx = sFnl.Length - sRgx.Length + decimal.One.ToString().Length;
                                    }
                                }
                            }
                            else
                            {
                                if (decimal.TryParse(sRgx, out dRgx))
                                {
                                    sRgx = dRgx.ToString(sPtn);
                                    iBgx = sFnl.Length - sRgx.Length;
                                }
                            }
                        }
                    }
                    else
                    {//DecimalSeparator found after apply pattern
                        if (sRgx == string.Empty)
                        {
                            iBgx = iTmp;
                        }
                        else
                        {
                            if (sRgx.StartsWith(decimal.Zero.ToString()))
                            {
                                sRgx = decimal.One.ToString() + sRgx;
                                if (decimal.TryParse(sRgx, out dRgx))
                                {
                                    sRgx = dRgx.ToString(sPtn);
                                    if (sRgx.StartsWith(decimal.One.ToString() + ci.NumberFormat.NumberDecimalSeparator))
                                    {
                                        iBgx = sFnl.Length - sRgx.Length + (decimal.One.ToString() + ci.NumberFormat.NumberDecimalSeparator).Length;
                                    }
                                    else
                                    {
                                        iBgx = sFnl.Length - sRgx.Length + decimal.One.ToString().Length;
                                    }
                                }
                            }
                            else
                            {
                                if (decimal.TryParse(sRgx, out dRgx))
                                {
                                    sRgx = dRgx.ToString(sPtn);
                                    iBgx = sFnl.Length - sRgx.Length;
                                }
                            }
                        }
                    }

                    if (iBgx < 0)
                    {
                        iBgx = 0;
                    }
                }
                else
                {//DecimalSeparator does exist
                    if (sFnl.Length - sRgx.Length <= iDtx)
                    {//before DecimalSeparator
                        sFnl = dFnl.ToString(sPtn);
                        if (sRgx.StartsWith(decimal.Zero.ToString()))
                        {
                            sRgx = decimal.One.ToString() + sRgx;
                            if (decimal.TryParse(sRgx, out dRgx))
                            {
                                sRgx = dRgx.ToString(sPtn);
                                if (sRgx.StartsWith(decimal.One.ToString() + ci.NumberFormat.NumberDecimalSeparator))
                                {
                                    iBgx = sFnl.Length - sRgx.Length + (decimal.One.ToString() + ci.NumberFormat.NumberDecimalSeparator).Length;
                                }
                                else
                                {
                                    iBgx = sFnl.Length - sRgx.Length + decimal.One.ToString().Length;
                                }
                            }
                        }
                        else if (sRgx.StartsWith(ci.NumberFormat.NumberDecimalSeparator))
                        {
                            if (decimal.TryParse(sRgx, out dRgx))
                            {
                                sRgx = dRgx.ToString(sPtn);
                                iBgx = sFnl.Length - sRgx.Length + ci.NumberFormat.NumberDecimalSeparator.Length;
                            }
                        }
                        else
                        {
                            if (decimal.TryParse(sRgx, out dRgx))
                            {
                                sRgx = dRgx.ToString(sPtn);
                                iBgx = sFnl.Length - sRgx.Length;
                            }
                        }

                        if (iBgx < 0)
                        {
                            iBgx = 0;
                        }
                    }
                    else
                    {//after DecimalSeparator
                        sFnl = dFnl.ToString(sPtn);
                        int iTmp = sFnl.IndexOf(ci.NumberFormat.NumberDecimalSeparator); //override
                        if (iTmp == -1)
                        {// pattern does not accept DecimalSeparator
                            iBgx = sFnl.Length;
                        }
                        else
                        {
                            if (sSub != string.Empty & sAdd == string.Empty)
                            {// if backspace (after decimal separator)
                                if (iTmp + 1 < iBgx)
                                {
                                    iBgx = iBgx - 1;
                                }
                                else
                                {
                                    iBgx = iTmp;
                                }
                            }

                            if (sFnl.Length > iBgx)
                            {
                                if (iBgx > iTmp)
                                {
                                    iSlx = 1;
                                }
                            }
                        }
                    }
                }
                sender.Text = sFnl;
                sender.SelectionStart = iBgx;
                sender.SelectionLength = iSlx;
            }//end of if decimal
            else
            {//not decimal
                if (IsNumOnly)
                { //IsNumOnly =true
                    decimal dOrg = decimal.Zero;
                    if (sFnl != string.Empty && decimal.TryParse(org.sTxt, out dOrg))
                    {
                        sender.Text = org.sTxt;
                        sender.SelectionStart = org.iBeg;
                        sender.SelectionLength = org.iSel;
                    }
                    else
                    {
                        decimal dAdd = decimal.Zero;
                        decimal.TryParse(sAdd, out dAdd);
                        sFnl = dAdd.ToString(sPtn);
                        int iTmp = sFnl.IndexOf(ci.NumberFormat.NumberDecimalSeparator);

                        if (iTmp == -1)
                        {
                            iBgx = sFnl.Length;
                        }
                        else
                        {
                            iBgx = iTmp;
                        }
                        sender.Text = sFnl;
                        sender.SelectionStart = iBgx;
                        sender.SelectionLength = 0;
                    }
                }
                else
                { //numbersOnly false
                    sender.Text = edi.sTxt;
                    sender.SelectionStart = edi.iBeg;
                    sender.SelectionLength = edi.iSel;
                }
            }
        }
    }
}
