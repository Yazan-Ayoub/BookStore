﻿using BookStore.Domain.Abstract;
using System;
using System.Text;
using BookStore.Domain.Entities;
using System.Net.Mail;
using System.Net;
using System.Diagnostics;

namespace BookStore.Domain.Concrete
{
   public class EmailSettings
    {
        public string MailToAddress = "yazan.aioub@outlook.com";
        public string MailFromAddress = "yaioub001@gmail.com";
        public bool UseSsl = true;
        public string Username= "yaioub@gmail.com";
        public string Password = "";
        public string ServerName ="smtp.gmail.com";
        public int ServerPort =587;
        public bool WriteAsFile =false;
        public string FileLocation = @"C:\order_bookstore_emails";
    }

    public class EmailOrderProcessor : IOrderProcessor
    {
        private EmailSettings emailSetting;
        public EmailOrderProcessor(EmailSettings setting)
        {
            emailSetting = setting;
        }
        public void ProcessOrder(Cart cart, ShippingDetails shippingDetails)
        {
            using (var smtpClient = new SmtpClient())
            {
                smtpClient.EnableSsl = emailSetting.UseSsl;
                smtpClient.Host =emailSetting.ServerName;
                smtpClient.Port = emailSetting.ServerPort;
                smtpClient.UseDefaultCredentials =false;
                smtpClient.Credentials = new NetworkCredential(emailSetting.Username,emailSetting.Password);
                if (emailSetting.WriteAsFile)
                {
                    smtpClient.DeliveryMethod = SmtpDeliveryMethod.SpecifiedPickupDirectory;
                    smtpClient.PickupDirectoryLocation = emailSetting.FileLocation;
                    smtpClient.EnableSsl = false;
                }
                StringBuilder body = new StringBuilder()
                .AppendLine("A new Order Has been Submitted !")
                .AppendLine("-----------------------------")
                .AppendLine("Books:");
                foreach (var line in cart.Lines)
                {
                    var subtotal = line.Book.Price * line.Quantity;
                    body.AppendFormat("{0} x {1} (Subtotal:{2:c})", line.Quantity, line.Book.Title, subtotal);
                }
                body.AppendFormat("Total order value:{0:c}", cart.ComputeTotalValue())
                .AppendLine("-------------- ")
                .AppendLine("Ship To:")
                .AppendLine("-------------- ")
                .AppendLine(shippingDetails.Name)
                .AppendLine(shippingDetails.Line1)
                .AppendLine(shippingDetails.Line2)
                .AppendLine(shippingDetails.Mobile)
                .AppendLine(shippingDetails.City)
                .AppendLine(shippingDetails.Country)
                .AppendLine("---------------- ")
                .AppendFormat("GiftWrap:{0}", shippingDetails.GiftWrap ? "Yes" : "No");
                
                    MailMessage mailMessage = new MailMessage(
                    emailSetting.MailFromAddress,
                    emailSetting.MailToAddress,
                    "New Order Submitted",
                    body.ToString());

                if (emailSetting.WriteAsFile)
                    mailMessage.BodyEncoding = Encoding.ASCII;
                try
                {
                smtpClient.Send(mailMessage);

                }
                catch(Exception ex)
                {

                    Debug.Print(ex.Message);
                }

               
                }
        }
    }
}

