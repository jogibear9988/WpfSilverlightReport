﻿/************************************************************************
 * Copyright: Seaking
 *
 * License:  This software abides by the LGPL license terms. For further
 *           licensing information please visit http://silverlightreport.codeplex.com/license.
 *
 * Author:   Seaking
 *
 ************************************************************************/

using System;
using System.Windows;
using System.Windows.Controls;

namespace Report
{
    public partial class Preview : IPrinter
    {
        public Preview()
        {
            InitializeComponent();

#if SILVERLIGHT
            this.Height = Application.Current.Host.Content.ActualHeight;
            this.Width = Application.Current.Host.Content.ActualWidth;
#endif

            this.Loaded += Preview_Loaded;
            //this.Closing += Preview_Closing;
        }

        public void Show()
        {
#if SILVERLIGHT
            var wnd = new ChildWindow();
            wnd.Content = this;
            wnd.Show();
#else
            var wnd = new Window();
            wnd.Content = this;
            wnd.ShowDialog();
#endif
        }

        private int pageNo;
        private int pageCount;

        public event EventHandler<XPrintPageEventArgs> PrintPage;
        public event EventHandler<XBeginPrintEventArgs> BeginPrint;
        public event EventHandler<EventArgs> EndPrint;

        void Preview_Loaded(object sender, RoutedEventArgs e)
        {
            if (this.BeginPrint != null)
            {
                var args = new XBeginPrintEventArgs();
                this.BeginPrint(this, args);

                this.pageCount = args.PageCount;
                this.txbPageCount.Text = args.PageCount.ToString();
            }

            this.PrintPageNo(1);
        }

        void Preview_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (this.EndPrint != null) this.EndPrint(this, EventArgs.Empty);
        }

        private void PrintPageNo(int pageNo)
        {
            if (this.PrintPage == null) throw new Exception("PrintPage event is empty.");

            XPrintPageEventArgs args = new XPrintPageEventArgs(pageNo);
            this.PrintPage(this, args);

            this.page.Child = args.PageVisual;

            this.pageNo = args.PageNo;
            this.txtPageNo.Text = args.PageNo.ToString();

            this.btnPrevious.IsEnabled = pageNo > 1;
            this.btnNext.IsEnabled = pageNo < this.pageCount;
        }

        private void btnPrevious_Click(object sender, RoutedEventArgs e)
        {
            if (this.pageNo > 1) this.PrintPageNo(this.pageNo - 1);
        }

        private void btnNext_Click(object sender, RoutedEventArgs e)
        {
            if (this.pageNo < this.pageCount) this.PrintPageNo(this.pageNo + 1);
        }
    }
}
