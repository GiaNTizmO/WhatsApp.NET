using HtmlAgilityPack;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;

namespace WhatsApp.NET
{
    internal class Program
    {
        public static List<ChatObject> Chats = new List<ChatObject>();

        private static void Main(string[] args)
        {
            Console.WriteLine("[+] Hello World!");
            var driver = new ChromeDriver();

            driver.Navigate().GoToUrl("https://web.whatsapp.com");

            Thread.Sleep(5000); //ghetto waiting

            try
            {
                driver.FindElement(By.Id("app"));
            }
            catch (NoSuchElementException)
            {
                Console.WriteLine("[-] Could not find application html ID tag!");
                driver.Quit();
                Console.ReadLine();
                Environment.Exit(-1);
            }

            if (!driver.Title.Contains("Whatsapp"))
            {
                Console.WriteLine("[-] Could not load whatsapp web page! [Missmatch title check]");
            }
            try
            {
                var searchBox = driver.FindElement(By.ClassName("landing-header"));
                Console.WriteLine("[!] Need to log-in. Waiting...");
                while (isNeedLogin(driver)) //*[@id="app"]/div/div/div[2]/div[1]/div/div[2]/div/svg - XPath for QR Ajax loader
                {
                    Console.WriteLine("[!] Waiting auth...");
                    Thread.Sleep(5000);
                }
                Console.WriteLine("[+] Welcome back, " + getUserName(driver) + " !");
            }
            catch (NoSuchElementException)
            {
                Console.WriteLine("[?] Already logged?");
            }
            Thread.Sleep(2000);
            EnumerateChats(driver);
            Console.WriteLine("Breakme");
            //driver.Quit();
        }

        public static bool isNeedLogin(ChromeDriver driver)
        {
            try
            {
                var searchBox = driver.FindElement(By.ClassName("landing-header"));
                return true;
            }
            catch (NoSuchElementException)
            {
                return false;
            }
            return false;
        }

        public static string getUserName(ChromeDriver driver)
        {
            var userBox = driver.FindElement(By.XPath("//*[@id=\"side\"]/header/div[1]/div/div/span"));
            userBox.Click();
            Thread.Sleep(1500);
            var userNameInputbox = driver.FindElement(By.XPath("//*[@id=\"app\"]/div/div/div[2]/div[1]/span/div/span/div/div/div[2]/div[2]/div[1]/div/div/div[2]"));
            string username = userNameInputbox.Text;
            var userNameBackButton = driver.FindElement(By.XPath("//*[@id=\"app\"]/div/div/div[2]/div[1]/span/div/span/div/header/div/div[1]/button"));
            Thread.Sleep(1000);
            userNameBackButton.Click();
            return username;
        }

        public static ChatObject[] EnumerateChats(ChromeDriver driver) //div[@aria-label='Список чатов']/div[@class='_3uIPm WYyr1'
        {//div[@aria-label='Список чатов']/div
            var userchats = driver.FindElements(By.XPath("//div[@aria-label='Список чатов']/div"));
            foreach (var t in userchats)
            {
                //Chats.Add(new ChatObject(""));
                var test1 = driver.FindElement(By.CssSelector("div[style='" + t.GetAttribute("style") + "']"));
                //test1.Click();
                Console.WriteLine(t.Text);
                Console.WriteLine("TAGNAME: " + t.GetAttribute("style"));
            }
            return null;
        }

        public class ChatObject
        {
            public string ChatName;
            public int ChatUnreadedMessagesCounter;
            public string ChatAvatar;
            public string ChatLastMessage;

            public string ChatStyleID;

            public ChatObject()
            { }

            public ChatObject(string name, int unreadcount, string avatar, string lastmsg, string styleId)
            {
                ChatName = name;
                ChatUnreadedMessagesCounter = unreadcount;
                ChatAvatar = avatar;
                ChatLastMessage = lastmsg;
                ChatStyleID = styleId;
            }
        }
    }
}