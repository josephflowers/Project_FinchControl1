using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using FinchAPI;
using System.Linq;


namespace Project_FinchControl
{

    // **************************************************
    //
    // Title: Finch Control
    // Description: Interactive Finch Robot Control System
    // Application Type: Console
    // Author: Joseph Flowers. With Starter Solution by John Velis
    // Dated Created: 11/15/2020
    // Last Modified: 11/28/2020
    //
    // **************************************************

    class Program
    {

        #region ENUM COMMAND




        public enum Command
        {
            NONE,
            MOVEFORWARD,
            MOVEBACKWARD,
            STOPMOTORS,
            WAIT,
            TURNRIGHT,
            TURNLEFT,
            LEDON,
            LEDOFF,
            TEMPERATURE,
            NOTEON,
            NOTEOFF,
            DONE
        }

        #endregion

        /// <summary>
        /// first method run when the app starts up
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {
            SetTheme();

            

            DisplayWelcomeScreen();
            DisplayLoginRegister();

            DisplayMenuScreen();
            DisplayClosingScreen();
        }

        /// <summary>
        /// setup the console theme
        /// </summary>

        static void SetTheme() //now sets theme from stored theme data
        {

            (ConsoleColor foregroundColor, ConsoleColor backgroundColor) themeColors;
            Console.CursorVisible = true;
            themeColors = ThemeReadColorSelection();
            Console.ForegroundColor = themeColors.foregroundColor;
            Console.BackgroundColor = themeColors.backgroundColor;
            //Console.ForegroundColor = ConsoleColor.DarkBlue;
            //Console.BackgroundColor = ConsoleColor.White;
        }

        static string userName;
        

        /// <summary>
        /// *****************************************************************
        /// *                     Main Menu                                 *
        /// *****************************************************************
        /// </summary>
        static void DisplayMenuScreen()
        {
            Console.CursorVisible = true;

            bool quitApplication = false;
            string menuChoice;

            Finch roboF = new Finch();
            
            do
            {
                DisplayScreenHeader("Main Menu");

                //
                // get user menu choice
                //
                Console.WriteLine($"\t Hello {userName}\n");
                Console.WriteLine("\ta) Connect Finch Robot");
                Console.WriteLine("\tb) Talent Show");
                Console.WriteLine("\tc) Data Recorder");
                Console.WriteLine("\td) Alarm System");
                Console.WriteLine("\te) User Programming");
                Console.WriteLine("\tf) Disconnect Finch Robot");
                Console.WriteLine("\tg) Change Theme Settings");
                Console.WriteLine("\tq) Quit");
                Console.Write("\t\tEnter Choice:");
                menuChoice = Console.ReadLine().ToLower();

                //
                // process user menu choice
                //
                switch (menuChoice)
                {
                    case "a":
                        DisplayConnectroboF(roboF);
                        break;

                    case "b":
                        TalentShowDisplayMenuScreen(roboF);
                        break;

                    case "c":
                        DataRecorderDisplayMenuScreen(roboF);
                        break;

                    case "d":
                        AlarmSystemDisplayMenuScreen(roboF);
                        break;

                    case "e":
                        UserProgrammingDisplayMenuScreen(roboF);
                        break;

                    case "f":
                        DisplayDisconnectroboF(roboF);
                        break;

                    case "g":
                        ThemeDisplaySetTheme();
                        break;

                    case "q":
                        DisplayDisconnectroboF(roboF);
                        quitApplication = true;
                        break;

                    default:
                        Console.WriteLine();
                        Console.WriteLine("\tPlease enter a letter for the menu choice.");
                        DisplayContinuePrompt();
                        break;
                }

            } while (!quitApplication);
        }

        
        #region LOGIN REGISTER
        /// <summary>
        /// *****************************************************************
        /// *                 Login/Register Screen                         *
        /// *****************************************************************
        /// </summary>
        static void DisplayLoginRegister()
        {
            DisplayScreenHeader("Login/Register");

            Console.Write("\tAre you a registered user [ yes | no ]?");
            if (Console.ReadLine().ToLower() == "yes")
            {
                DisplayLogin();
            }
            else
            {
                DisplayRegisterUser();
                DisplayLogin();
            }
        }

        /// <summary>
        /// *****************************************************************
        /// *                          Login Screen                         *
        /// *****************************************************************
        /// </summary>
        static void DisplayLogin()
        {
            //string userName;
            string password;
            
            bool validLogin;

            do
            {
                DisplayScreenHeader("Login");

                Console.WriteLine();
                Console.Write("\tEnter your user name:");
                userName = Console.ReadLine();
                Console.Write("\tEnter your password:");
                password = Console.ReadLine();

                validLogin = IsValidLoginInfo(userName, password);
                
                Console.WriteLine();
                if (validLogin)
                {
                    Console.WriteLine("\tYou are now logged in.");
                }
                else
                {
                    Console.WriteLine("\tIt appears either the user name or password is incorrect.");
                    Console.WriteLine("\tPlease try again.");
                }

                DisplayContinuePrompt();
            } while (!validLogin);
        }

        /// <summary>
        /// check user login
        /// </summary>
        /// <param name="userName">user name entered</param>
        /// <param name="password">password entered</param>
        /// <returns>true if valid user</returns>
        static bool IsValidLoginInfo(string userName, string password)
        {
            List<(string userName, string password, string finchName)> registeredUserLoginInfo = new List<(string userName, string password, string finchName)>();
            bool validUser = false;

            registeredUserLoginInfo = ReadLoginInfoData();

            //
            // loop through the list of registered user login tuples and check each one against the login info
            //
            foreach ((string userName, string password, string finchName) userLoginInfo in registeredUserLoginInfo)
            {
                if ((userLoginInfo.userName == userName) && (userLoginInfo.password == password))
                {
                    validUser = true;
                    break;
                }
            }

            return validUser;
        }

        /// <summary>
        /// *****************************************************************
        /// *                       Register Screen                         *
        /// *****************************************************************
        /// write login info to data file
        /// </summary>
        static void DisplayRegisterUser()
        {
            string userName;
            string password;
            string finchName;
            DisplayScreenHeader("Register");

            Console.Write("\tEnter your user name:");
            userName = Console.ReadLine();
            Console.Write("\tEnter your password:");
            password = Console.ReadLine();
            Console.Write("\tName your Finch Robot:");
            finchName = Console.ReadLine();

            WriteLoginInfoData(userName, password, finchName);

            Console.WriteLine();
            Console.WriteLine("\tYou entered the following information and it has be saved.");
            Console.WriteLine($"\tUser name: {userName}");
            Console.WriteLine($"\tPassword: {password}");
            Console.WriteLine($"\tFinch name: {finchName}");
            DisplayContinuePrompt();
        }

        /// <summary>
        /// read login info from data file
        /// Note: no error or validation checking
        /// </summary>
        /// <returns>list of tuple of user name and password</returns>
        static List<(string userName, string password, string finchName)> ReadLoginInfoData()
        {
            string dataPath = @"Data/UserData.txt";

            string[] loginInfoArray;
            (string userName, string password, string finchName) loginInfoTuple;

            List<(string userName, string password, string finchName)> registeredUserLoginInfo = new List<(string userName, string password, string finchName)>();

            loginInfoArray = File.ReadAllLines(dataPath);

            //
            // loop through the array
            // split the user name and password into a tuple
            // add the tuple to the list
            //
            foreach (string loginInfoText in loginInfoArray)
            {
                //
                // use the Split method to separate the user name and password into an array
                //
                loginInfoArray = loginInfoText.Split(',');

                loginInfoTuple.userName = loginInfoArray[0];
                loginInfoTuple.password = loginInfoArray[1];
                loginInfoTuple.finchName = loginInfoArray[2];
                registeredUserLoginInfo.Add(loginInfoTuple);

            }

            return registeredUserLoginInfo;
        }

        /// <summary>
        /// write login info to data file
        /// Note: no error or validation checking
        /// </summary>
        static void WriteLoginInfoData(string userName, string password, string finchName)
        {
            string dataPath = @"Data/UserData.txt";
            string loginInfoText;

            loginInfoText = userName + "," + password + "," +finchName+"\n";

            //
            // use the AppendAllText method to not overwrite the existing logins
            //
            File.AppendAllText(dataPath, loginInfoText);
        }


        #endregion LOGIN REGISTER




        #region USER THEME
        /// <summary>
        /// setup the console theme
        /// </summary>
        static void ThemeDisplaySetTheme()
        {
            (ConsoleColor foregroundColor, ConsoleColor backgroundColor) themeColors;
            bool colorsChosen = false;
            Console.CursorVisible = true;
            themeColors = ThemeReadColorSelection();
            Console.ForegroundColor = themeColors.foregroundColor;
            Console.BackgroundColor = themeColors.backgroundColor;
            Console.Clear();

            DisplayScreenHeader("Theme Chooser:");
            Console.WriteLine($"\n\t The current foreground color is: {Console.ForegroundColor}");
            Console.WriteLine($"\t The current background color is: {Console.BackgroundColor}\n");

            do
            {
                Console.Write($"\t {userName} Would you like to change the current theme colors? (yes/no) ");
                string userResponse = Console.ReadLine().ToLower();
                switch (userResponse)
                {
                    case "yes":
                        themeColors.foregroundColor = ThemeGetForegroundColor();
                        themeColors.backgroundColor = ThemeGetBackgroundColor();
                        Console.ForegroundColor = themeColors.foregroundColor;
                        Console.BackgroundColor = themeColors.backgroundColor;
                        Console.Clear();
                        DisplayScreenHeader("New Theme Colors");
                        Console.WriteLine($"\t Foreground color is set to {Console.ForegroundColor}");
                        Console.WriteLine($"\t Background color is set to {Console.BackgroundColor}\n");
                        Console.WriteLine($"\t {userName}Keep color selections?");
                        if (Console.ReadLine().ToLower() == "yes")
                        {
                            colorsChosen = true;
                            ThemeWriteColorSelection(themeColors.foregroundColor, themeColors.backgroundColor);
                        }
                        break;

                    case "no":
                        colorsChosen = false;
                        break;

                    default:
                        Console.WriteLine($"\t {userName} Please enter yes or no");
                        break;
                }


            } while (!colorsChosen);
        }

        /// <summary>
        ///                             User set background color
        /// </summary>
        /// <returns></returns>
        static ConsoleColor ThemeGetBackgroundColor()
        {
            Console.Clear();
            ConsoleColor userColor;
            int count = 0;
            bool validColor;
            Console.CursorVisible = true;
            DisplayScreenHeader("New Background Color");
            Console.WriteLine("\t Available colors are:");

            foreach (string colorOptions in Enum.GetNames(typeof(ConsoleColor)))
            {
                Console.WriteLine($"\t{count}: {colorOptions.ToLower()}");
                count++;
            }
            do
            {
                Console.WriteLine();
                Console.WriteLine("\t Enter a new background color");
                Console.Write("\t ");
                validColor = Enum.TryParse<ConsoleColor>(Console.ReadLine(), true, out userColor);

                if (!validColor) 
                {
                    Console.WriteLine($"\t {userName} That is not an option, try again.");
                }
                
                else
                {
                    validColor = true;
                }
            } while (!validColor);
            ///
            ///                         user feedback 
            /// 
            string exceptionMessage;
            string catchColor = userColor.ToString();
            catchColor = ThemeCatchException(out exceptionMessage);
            DisplayStringWithBorder($"{exceptionMessage}");
            Console.WriteLine("\t Press any key to continue");
            Console.ReadKey();
            return userColor;
        }
        /// <summary>
        ///                         User sets Foreground color
        /// </summary>
        /// <returns></returns>
        static ConsoleColor ThemeGetForegroundColor()
        {
            Console.CursorVisible = true;
            Console.Clear();
            ConsoleColor userColor;
            int count = 0;
            bool validColor;
            DisplayScreenHeader("New foreground Color");
            Console.WriteLine("\t   Valid colors are:");
            foreach (string colorOptions in Enum.GetNames(typeof(ConsoleColor)))
            {
                Console.WriteLine($"\t {count}: {colorOptions.ToLower()}");
                count++;
            }
            do
            {
                Console.WriteLine();
                Console.WriteLine("\t Set foreground color");
                Console.Write("\t ");
                validColor = Enum.TryParse<ConsoleColor>(Console.ReadLine(), true, out userColor);

                if (!validColor)
                {
                    Console.WriteLine($"\t {userName}That is not a valid color, Please try again");
                }
                else
                {
                    validColor = true;
                }
            } while (!validColor);
            ///
            ///                         User Feedback
            /// 
            string exceptionMessage;
            string catchColor = userColor.ToString();
            catchColor = ThemeCatchException(out exceptionMessage);
            Console.WriteLine();
            Console.WriteLine($"\t *{exceptionMessage}*");
            Console.WriteLine();
            Console.WriteLine("\t Press any key to continue");
            Console.ReadKey();
            return userColor;
        }


        /// <summary>
        ///             Feedback and catch exception area
        /// </summary>
        /// <param name="exceptionMessage"></param>
        /// <returns></returns>
        static string ThemeCatchException(out string exceptionMessage)
        {
            string path = @"Data/Theme.txt";
            string userColor;

            try
            {
                userColor = File.ReadAllText(path);
                exceptionMessage = "Colors Updated";
            }
            catch (DirectoryNotFoundException)
            {
                exceptionMessage = "Cannot find directory location";
            }
            catch (FileNotFoundException)
            {
                exceptionMessage = "Cannot find file";
            }
            catch (Exception)
            {
                exceptionMessage = "Error: Cannot locate file";
            }
            return exceptionMessage;
        }


        /// <summary>
        ///                 Read the Theme info from the theme data file
        /// </summary>
        /// <returns></returns>
        static (ConsoleColor foregroundColor, ConsoleColor backgroundColor) ThemeReadColorSelection()
        {
            string themeDataPath = @"Data/Theme.txt";
            string[] themeColors;
            ConsoleColor foregroundColor;
            ConsoleColor backgroundColor;

            themeColors = File.ReadAllLines(themeDataPath);
            Enum.TryParse(themeColors[0], true, out foregroundColor);
            Enum.TryParse(themeColors[1], true, out backgroundColor);

            return (foregroundColor, backgroundColor);
        }

        /// <summary>
        ///                 Write Theme info into the theme data file
        /// </summary>
        /// <param name="foregroundColor"></param>
        /// <param name="backgroundColor"></param>
        static void ThemeWriteColorSelection(ConsoleColor foregroundColor, ConsoleColor backgroundColor)
        {
            string themeDataPath = @"Data/Theme.txt";

            //              I like the short way better
            //string[] themeColors = new string[2];
            //string foreColor = foregroundColor.ToString();
            //string backColor = backgroundColor.ToString();
            //themeColors[0] = foreColor;
            //themeColors[1] = backColor;
            //File.WriteAllText(themeDataPath, themeColors[0] + "\n" + themeColors[1]);
            File.WriteAllText(themeDataPath, foregroundColor.ToString() + "\n");
            File.AppendAllText(themeDataPath, backgroundColor.ToString());
        }
        #endregion



        #region LED

        ///
        /// random
        ///
        static void LedRandom(Finch roboF)
        {
            Random random = new System.Random();
            
            for (int count = 0; count < 10; count++)
            {
                int ledValue0 = random.Next(1, 255);
                int ledValue1 = random.Next(1, 255);
                int ledValue2 = random.Next(1, 255);
                roboF.setLED(ledValue0, ledValue1, ledValue2);
                int randWait = random.Next(200, 1000);
                roboF.wait(randWait);
            }
            Console.WriteLine($"Press any key to continue {userName}");
            Console.ReadLine();
        }

        ///
        ///
        /// 
        ///
        /// led up
        ///
        static void ledUp(Finch roboF)
        {
            for (int ledValue = 0; ledValue < 255; ledValue++)
            {
                roboF.setLED(ledValue, ledValue, ledValue);
                roboF.wait(20);
            }

            Console.WriteLine("Press any key to continue.");
            Console.ReadKey();
        }
        ///
        ///         Led Cycle
        ///
        /// 
        static void LedCycle(Finch roboF)
        {
            roboF.setLED(255, 0, 0); //red
            roboF.wait(500);
            roboF.setLED(255, 255, 255); //white-blue-green
            roboF.wait(500);
            roboF.setLED(255, 255, 0); //yellow-green
            roboF.wait(500);
            roboF.setLED(255, 0, 255); //purple
            roboF.wait(500);
            roboF.setLED(0, 255, 0); //green
            roboF.wait(500);
            roboF.setLED(0, 255, 255); //blue-green
            roboF.wait(500);
            roboF.setLED(0, 0, 255); //blue
            roboF.wait(500);
            roboF.setLED(128, 128, 128); //low-white-blend
            roboF.wait(500);
            Console.WriteLine("press any key");
            Console.ReadKey();
        }

        ///
        /// end of LEDCYCLE
        ///   

        #endregion

        #region SOUNDS

        private static void RoboSounds(Finch roboF)
        {
            int[] sawInt =
            {
                5, 5, 5, 5, 17, 17, 17, 17, 17, 17, 17, 10, 10, 15, 40, 10, 10, 15, 40, 10, 10, 15, 40, 10, 10, 15, 40,
                19, 19, 29, 50, 10, 10, 15, 40, 10, 10, 15, 40, 10, 10, 15, 40, 250  
            };
            Array.ForEach(sawInt, freqSaw);

            void freqSaw(int sawIn)
            {
                for (int saw = 0; saw < sawIn; saw++)
                {
                    int s = ((saw * 5) * 2) - 1;
                    roboF.noteOn(s);
                    roboF.wait(5);
                }

                for (int saw = 20; saw > 0; saw--)
                {
                    int s = ((saw * 2) * 2) - 1;
                    roboF.noteOn(s);
                    roboF.wait(5);
                }

                roboF.noteOff();
            }
        }

        #endregion SOUND

        #region TALENT SHOW

        /// <summary>
        /// *****************************************************************
        /// *                     Talent Show Menu                          *
        /// *****************************************************************
        /// </summary>
        static void TalentShowDisplayMenuScreen(Finch roboF)
        {
            Console.CursorVisible = true;

            bool quitTalentShowMenu = false;
            string menuChoice;

            do
            {
                DisplayScreenHeader("Talent Show Menu");

                //
                // get user menu choice
                //
                Console.WriteLine($"{userName} Lets have some fun.");
                Console.WriteLine("\ta) Light and Sound");
                Console.WriteLine("\tb) To Dance. We Dance.");
                Console.WriteLine("\tc) Mixing it up. Party Time.");
                Console.WriteLine("\td) Mysterious");
                Console.WriteLine("\tq) Main Menu");
                Console.Write("\t\tEnter Choice:");
                menuChoice = Console.ReadLine().ToLower();

                //
                // process user menu choice
                //
                switch (menuChoice)
                {
                    case "a":
                        TalentShowDisplayLightAndSound(roboF);
                        break;

                    case "b":
                        TalentShowDisplayDance(roboF);
                        break;

                    case "c":
                        TalentShowDisplayMixingItUp(roboF);
                        break;

                    case "d":
                        TalentShowDisplayMysterious(roboF);
                        break;

                    case "q":
                        quitTalentShowMenu = true;
                        break;

                    default:
                        Console.WriteLine();
                        Console.WriteLine("\tPlease enter a letter for the menu choice.");
                        DisplayContinuePrompt();
                        break;
                }

                

            } while (!quitTalentShowMenu);

        }

        #region MYSTERIOUS

        static void TalentShowDisplayMysterious(Finch roboF)
        {
            Console.Clear();
            DisplayScreenHeader("Mysterious");
            Console.WriteLine("\tA Light show");
            Random random = new System.Random();

            for (int count = 0; count < 120; count++)
            {
                int ledValue0 = random.Next(1, 255);
                int ledValue1 = random.Next(1, 255);
                int ledValue2 = random.Next(1, 255);
                roboF.setLED(ledValue0, ledValue1, ledValue2);
                int randWait = random.Next(200, 1000);
                roboF.wait(randWait);
            }
            Console.WriteLine("Press any key to continue");
            Console.ReadLine();
        }

        #endregion
        #region DANCE
        /// 
        /// *****************************************************************
        /// *               Talent Show > Dance you dance                   *
        /// *****************************************************************
        /// 
        /// 



        static void TalentShowDisplayDance(Finch roboF)
        {
            DisplayScreenHeader("Dance");
            Console.WriteLine("\tSquare dancing... ish. Get ready to tangle the cord!"); /// get ready to tangle the cord
            DisplayContinuePrompt();
            
                roboF.setMotors(255, 255);
                roboF.wait(1000);
                roboF.setMotors(255, -50);
                roboF.wait(1000);
                roboF.setMotors(255, 255);
                roboF.wait(1000);
                roboF.setMotors(255, -50);
                roboF.wait(1000);
                roboF.setMotors(255, 255);
                roboF.wait(1000);
                roboF.setMotors(255, -50);
                roboF.wait(1000);
                roboF.setMotors(255, 255);
                roboF.wait(1000);
                roboF.setMotors(255, -50);
                roboF.wait(1000);
                roboF.setMotors(255, 255);
                roboF.wait(1000);
                roboF.setMotors(255, -50);
                roboF.wait(1000);
                roboF.setMotors(255, 255);
                roboF.wait(1000);
                roboF.setMotors(255, -50);
                roboF.wait(1000);
                roboF.setMotors(255, 255);
                roboF.wait(1000);
                roboF.setMotors(255, -50);
                roboF.wait(1000);
                roboF.setMotors(255, 255);
                roboF.wait(1000);
                roboF.setMotors(255, -50);
                roboF.wait(1000);
                roboF.setMotors(0,0);

        }
        #endregion

        /// 
        /// *****************************************************************
        /// *               Talent Show > Light and Sound                   *
        /// *****************************************************************
        /// 
        /// 
        static void TalentShowDisplayLightAndSound(Finch roboF)
        {
            Console.CursorVisible = false;

            DisplayScreenHeader("Light and Sound");

            Console.WriteLine("\tThe Finch robot will now show off its glowing talents!");
            DisplayContinuePrompt();
            /////
            ///
            /// proving grounds
            ///
            ///
            ///
            Console.WriteLine("\tLeds Up");
            ledUp(roboF);
            Console.WriteLine("\tLeds Each Main Color");
            LedCycle(roboF);
            Console.WriteLine("\tLeds Randomized");
            LedRandom(roboF);
            Console.WriteLine("\tRobot Music");
            RoboSounds(roboF);
            Console.WriteLine("\tPress any key to continue");
            Console.ReadLine();

            Console.WriteLine("\tModified light and sound up");
            for (int lightSoundLevel = 0; lightSoundLevel < 255; lightSoundLevel++)
            {
                roboF.setLED(lightSoundLevel, lightSoundLevel, lightSoundLevel);
                roboF.noteOn(lightSoundLevel * 9);
            }

            roboF.noteOff();
            Console.WriteLine("\tPress any key to continue");
            Console.ReadLine();

        }

        #endregion


        #region MIXINGITUP

        

        
        /// 
        ///  Talent Show > Mixing it up 
        ///  mixing with some random
        /// 
        static void TalentShowDisplayMixingItUp(Finch roboF)
        {
            Console.Clear();
            DisplayScreenHeader("Mixing It Up");
            Console.WriteLine($"\n\t{userName} Lets Mix it up with some randomness");
            Random random = new System.Random();

            for (int count = 0; count < 10; count++)
            {
                int ledValue0 = random.Next(1, 255);
                int ledValue1 = random.Next(1, 255);
                int ledValue2 = random.Next(1, 255);
                roboF.setLED(ledValue0, ledValue1, ledValue2);
                int randWait = random.Next(200, 1000);
                roboF.noteOn(random.Next(1,800));
                int motor1 = random.Next(-255, 255);
                int motor2 = random.Next(-255, 255);
                roboF.setMotors(motor1,motor2);
                roboF.wait(randWait);
            }
            roboF.noteOff();
            roboF.setMotors(0,0);
            Console.WriteLine("\tPress any key to continue");
            Console.ReadLine();
        }
        #endregion

        #region DATARECORDER
        /// 
        /// Data recorder is set to measure temperatures from the finch
        /// 
        /// 

        private static void DataRecorderDisplayMenuScreen(Finch roboF)
        {
            Console.CursorVisible = true;
            int numberOfReadings = 0;
            double readingFrequency = 0;
            double[] celsiusTemp = null;

            bool quitRecorder = false; /// exit strategy
            do /// data recorder menu
            
            {
                DisplayScreenHeader("Data Recorder:");
                Console.WriteLine($"\n\t{userName} Choose an option for temperature readings:");
                Console.WriteLine("\ta) How many readings are we taking?");
                Console.WriteLine("\tb) How often in seconds would you like to take a reading?");
                Console.WriteLine("\tc) Get readings:");
                Console.WriteLine("\td) Show results:");
                //Console.WriteLine("\te) Save results:");

                Console.WriteLine("\tq) Return to main menu");
                DisplayChooseAnOption();
                string MenuChoice = Console.ReadLine().ToLower();
                switch (MenuChoice)
                {
                    case "a":
                        numberOfReadings = DataRecorderDisplayGetNumberOfReadings();
                        break;

                    case "b":
                        readingFrequency = DataRecorderDisplayGetReadingFrequency();
                        break;

                    case "c":
                        celsiusTemp = DataRecorderDisplayGetReadings(numberOfReadings, readingFrequency, roboF);
                        break;

                    case "d":
                        DataRecorderDisplayGetReadings(celsiusTemp);
                        break;
                    /*case "e":
                        DataRecorderDisplayTableOut(celsiusTemp);
                        break;
                    */
                    case "q":
                        quitRecorder = true;
                        break;

                    default:
                        DisplayWrongInput();
                        break;
                }
            } while (!quitRecorder);
        }

        private static int DataRecorderDisplayGetNumberOfReadings()
        {
            Console.CursorVisible = true;
            DisplayScreenHeader("Option a: Get the number of readings:");

            Console.Write("\n\tEnter the number of readings we are taking: ");
            string userResponse = Console.ReadLine();

            int.TryParse(userResponse, out int numberOfReadings); /// includes validation kindof

            DisplayContinuePrompt();
            return numberOfReadings;
        }

        private static double DataRecorderDisplayGetReadingFrequency()
        {
            Console.CursorVisible = true;
            DisplayScreenHeader("Option B : frequency of readings in seconds:");

            double.TryParse(Console.ReadLine(), out double readingFrequency); /// includes validation sortof

            DisplayContinuePrompt();
            return readingFrequency;
        }

        
        
        private static double[] DataRecorderDisplayGetReadings(int numberOfReadings, double readingFrequency,
            Finch roboF)
        {
            Console.CursorVisible = false;
            double[] celsiusTemp = new double[numberOfReadings];
            
            DisplayScreenHeader("Option c: Get readings:");
            Console.WriteLine($"\tNumber of readings: {numberOfReadings}");
            Console.WriteLine($"\tFrequency of readings: {readingFrequency}");
            Console.WriteLine();
            Console.WriteLine("\tReady to record temperature data.");
            DisplayContinuePrompt();

            for (int t = 0; t < numberOfReadings; t++)
            {
                celsiusTemp[t] = roboF.getTemperature();
                double fTemp = (celsiusTemp[t] * 9 / 5 + 32);  /// converting to farenhiet
                Console.WriteLine($"\tReading {t + 1} C*: {celsiusTemp[t]:n2} F*:{fTemp} ");
                int waitInSeconds = (int) ((readingFrequency) * 1000); /// converting to seconds from miliseconds
                roboF.wait(waitInSeconds);
            }

            DisplayContinuePrompt();

            return celsiusTemp;
            
            
        }

        private static void DataRecorderDisplayGetReadings(double[] celsiusTemp)
        {
            DataRecorderDisplayTable(celsiusTemp);
            DisplayContinuePrompt();
        }

        private static void DataRecorderDisplayTable(double[] celsiusTemp)
        {
            Console.CursorVisible = false;
            DisplayScreenHeader("Option D : Show results");  /// Output screen

            Console.WriteLine(
                "************".PadLeft(21) + "**************".PadLeft(20)
            );

            Console.WriteLine(
                "Recording".PadLeft(20) + "Temperature F*".PadLeft(21)
            );
            Console.WriteLine(
                "************".PadLeft(21) + "**************".PadLeft(20)
            );

            for (int t = 0; t < celsiusTemp.Length; t++)
            {
                Console.WriteLine(
                    (t + 1).ToString().PadLeft(20) +
                    (celsiusTemp[t] * 9 / 5 + 32).ToString("n2").PadLeft(20) /// converted to farenheit 
                );
            }
        }

        /*private static void DataRecorderDisplayTableOut(double[] celsiusTemp)
        {
            DisplayScreenHeader("Save Results");
            Console.WriteLine($"\t{userName} press any key to save results.");
            Console.ReadKey();
            string dataPath = @"Data/TemperatureLog.txt";
            
            for (int t = 0; t < celsiusTemp.Length; t++)
            {
                File.AppendAllText(dataPath, celsiusTemp[t].ToString());
            }

            Console.WriteLine("\tResults saved.");
            DisplayContinuePrompt();
        }
        */
        #endregion DATARECORDER


        #region ALARM SYSTEM

        /// 
        /// Alarm system is set to alert the user if the light value goes over a set maximum or under a set minimum  
        /// 
        private static void AlarmSystemDisplayMenuScreen(Finch roboF)
        {
            Console.CursorVisible = true;
            bool quitAlarm = false;
            string sensorsToMonitorInput = "";
            string rangeType = "";
            int minMaxThresholdValue = 0;
            int timeToMonitor = 0;

            do   /// alarm system menu
            {
                DisplayScreenHeader("Alarm System");

                Console.WriteLine("\ta) Set sensors to monitor");
                Console.WriteLine("\tb) Set range type");
                Console.WriteLine("\tc) Set minimum/maximum threshold");
                Console.WriteLine("\td) Set time to monitor");
                Console.WriteLine("\te) Set alarm");
                Console.WriteLine("\tq) Return to main menu");
                DisplayChooseAnOption();
                string MenuChoice = Console.ReadLine().ToLower();

                switch (MenuChoice)
                {
                    case "a":
                        sensorsToMonitorInput = DisplayScreenHeaderLightAlarmDisplaySetSensorsToMonitorInput();
                        break;

                    case "b":
                        rangeType = LightAlarmDisplaySetRangeType();
                        break;

                    case "c":
                        minMaxThresholdValue = LightAlarmDisplaySetMinMaxThresholdValue(roboF, rangeType);
                        break;

                    case "d":
                        timeToMonitor = LightAlarmDisplaySetTimeToMonitor();
                        break;

                    case "e":
                        LightAlarmDisplaySetAlarm(roboF, timeToMonitor, rangeType, minMaxThresholdValue,sensorsToMonitorInput);
                        break;

                    case "q":
                        quitAlarm = true;
                        break;

                    default:
                        DisplayWrongInput();
                        break;
                }
            } while (!quitAlarm);
        }



        /// 
        /// subs in alarms
        /// 
        private static string DisplayScreenHeaderLightAlarmDisplaySetSensorsToMonitorInput()
        {
            List<string> correctSensors = new List<string>() { "a", "b", "c", "d" };
            string sensorsToMonitorInput;
            
            
            DisplayScreenHeader("Sensors To Monitor");

            Console.WriteLine("\ta) Left Light Sensor");
            Console.WriteLine("\tb) Right Light Sensor");
            Console.WriteLine("\tc) Both Light Sensors");
            Console.WriteLine("\td) Temperature");
            Console.Write("\tWhich sensor(s) would you like to monitor? ");
            sensorsToMonitorInput = Console.ReadLine().ToLower();

            

            
            

            if (correctSensors.Contains(sensorsToMonitorInput))
            {
                return sensorsToMonitorInput;
            }
            else
            {
                DisplayScreenHeader("Please input \"a\" for left, \"b\" for right,\"c\" for both, or \"d\" for temp");
                DisplayContinuePrompt();
                return DisplayScreenHeaderLightAlarmDisplaySetSensorsToMonitorInput();

            }
            

        }

        



        private static string LightAlarmDisplaySetRangeType()
        {
            string[] correctRange = new string[] { "a", "b" };
            string rangeType;
            DisplayScreenHeader("Set Range Type");

            Console.WriteLine("\ta) Minimum");
            Console.WriteLine("\tb) Maximum");
            rangeType = Console.ReadLine().ToLower();
            
            
            if (correctRange.Contains(rangeType))
            {
                return rangeType;
            }
            else
            {
                DisplayScreenHeader("Please input \"a\" for minimum or \"b\" for maximum:");
                DisplayContinuePrompt();
                return LightAlarmDisplaySetRangeType();
            }
        }

        private static int LightAlarmDisplaySetMinMaxThresholdValue(Finch roboF, string rangeType)
        {
            int minMaxThresholdValue;
            bool minMaxThresholdValueValid;
            ///  abc menu to the words 
            /// 
            string rangeTypeOut = null;
            if (rangeType == "a")
            {
                rangeTypeOut = "Minimum";
            }
            if (rangeType == "b")
            {
                rangeTypeOut =  "Maximum";
            }
            DisplayScreenHeader("Minimum/Maximum Threshold Value");

            Console.WriteLine($"\tLeft light sensor current value: {roboF.getLeftLightSensor()}");
            Console.WriteLine($"\tRight light sensor current value: {roboF.getRightLightSensor()}");
            Console.WriteLine($"\tTemperature sensor current value: {(int)roboF.getTemperature()}");
            Console.WriteLine();
            Console.Write($"\tEnter The {rangeTypeOut} light or temp value: ");

            minMaxThresholdValueValid = int.TryParse(Console.ReadLine(), out minMaxThresholdValue);
            ///
            /// validate response
            /// 
            if (!minMaxThresholdValueValid)
            {
                DisplayScreenHeader("Please enter an integer");
                DisplayContinuePrompt();
                return LightAlarmDisplaySetTimeToMonitor();
            }
            else
            {
                return minMaxThresholdValue;
            }
        }

        private static int LightAlarmDisplaySetTimeToMonitor()
        {
            int timeToMonitor;
            bool timeToMonitorValid;
            DisplayScreenHeader("Set Time To Monitor");

            Console.Write("\tHow long would you like to monitor in seconds? ");
            timeToMonitorValid = int.TryParse(Console.ReadLine(), out timeToMonitor);

            /// Validation
            /// 
            if (!timeToMonitorValid)
            {
                DisplayScreenHeader("Please enter an integer");
                DisplayContinuePrompt();
                return LightAlarmDisplaySetTimeToMonitor();
            }
            else
            {
                return timeToMonitor;
            }
        }

        private static void LightAlarmDisplaySetAlarm(Finch roboF,
                                                    int timeToMonitor,
                                                    string rangeType,
                                                    int minMaxThresholdValue,
                                                    string sensorsToMonitorInput)
        {
            int secondsElapsed = 0;
            bool thresholdExceeded = false;
            int currentSensorValue = 0;
            ///
            /// again with the Replace
            /// 
            string sensorsToMonitor = null;
            string rangeTypeOut = null;
            if (sensorsToMonitorInput == "a")
            {
                sensorsToMonitor = "Left";
            }
            if (sensorsToMonitorInput == "b")
            {
                sensorsToMonitor = "Right";
            }
            if (sensorsToMonitorInput == "c")
            {
                sensorsToMonitor = "Both";
            }

            if (rangeType == "a")
            { 
                rangeTypeOut = "Minimum";
            }
            if (rangeType == "b")
            {
                rangeTypeOut =  "Maximum";
            }

            DisplayScreenHeader("Set Light or Temperature Alarm");
            Console.WriteLine("\tSensors to monitor:{0}",sensorsToMonitor);
            Console.WriteLine("\tRange Type: {0}", rangeTypeOut);
            Console.WriteLine("\t{0} Threshold Value: {1}",rangeTypeOut, minMaxThresholdValue);
            Console.WriteLine($"\tTime to monitor: {timeToMonitor}");
            Console.WriteLine();

            Console.WriteLine("\tPress any key to begin monitoring.");
            Console.ReadKey();

            while (secondsElapsed < timeToMonitor && !thresholdExceeded)
            {
                switch (sensorsToMonitorInput)
                {
                    case "a":
                        currentSensorValue = roboF.getLeftLightSensor();
                        break;

                    case "b":

                        currentSensorValue = roboF.getRightLightSensor();
                        break;

                    case "c":
                        currentSensorValue = (roboF.getRightLightSensor() + roboF.getLeftLightSensor()) / 2;
                        break;
                    
                    case "d":
                        currentSensorValue = (int)roboF.getTemperature();
                        break;
                }
                switch (rangeType)
                {
                    case "a":
                        if (currentSensorValue < minMaxThresholdValue)
                        {
                            thresholdExceeded = true;
                        }
                        break;

                    case "b":
                        if (currentSensorValue > minMaxThresholdValue)
                        {
                            thresholdExceeded = true;
                        }
                        break;
                }
                roboF.wait(1000);
                secondsElapsed++;
                Console.WriteLine("\tCurrent Value: {0} ", currentSensorValue);
            }

            if (thresholdExceeded)
            {
                Console.WriteLine($"\tThe {rangeTypeOut} threshold value was exceeded!");
                roboF.setLED(255,0,0);
                roboF.noteOn(100);
                roboF.wait(1000);
                roboF.setLED(0,0,0);
                roboF.noteOff();
            }
            else
            {
                Console.WriteLine($"\tThe {rangeTypeOut} threshold value was not exceeded.");
                roboF.setLED(0, 255, 0);
                roboF.wait(1000);
                roboF.setLED(0, 0, 0);
            }
            DisplayContinuePrompt();

            return;
        }

        #endregion ALARM SYSTEM
        
        
        /// <summary>
        ///
        ///
        /// next up User Programming
        /// 
        /// </summary>
        /// <param name="fn"></param>
        
        
        #region USER PROGRAMMING
        ///
        /// User Programming
        ///
        /// 
        static void UserProgrammingDisplayMenuScreen(Finch roboF)
        {
            Console.Clear();
            Console.CursorVisible = true;
            bool quitUserProgramming = false;
            string menuChoice;

            //
            // tuple thingy
            // Holds the three command parameters
            //
            (int motorSpeed, int ledBrightness, double waitSeconds) commandParameters;
            commandParameters.ledBrightness = 0;
            commandParameters.motorSpeed = 0;
            commandParameters.waitSeconds = 0;

            List<Command> commands = new List<Command>();


            do
            {
                // menu choices

                DisplayScreenHeader("User Programming");
                Console.WriteLine("\ta) Set Command Parameters");
                Console.WriteLine("\tb) Add Commands");
                Console.WriteLine("\tc) View Commands");
                Console.WriteLine("\td) Execute Commands");
                Console.WriteLine("\te) Interactive Driving");
                Console.WriteLine("\tq) Return to Main Menu");
                DisplayChooseAnOption();
                string MenuChoice = Console.ReadLine().ToLower();
                switch (MenuChoice)
                {
                    case "a":
                        commandParameters = UserProgrammingDisplayGetParameters();
                        break;

                    case "b":
                        UserProgramingDisplayGetFinchCommands(commands);
                        break;

                    case "c":
                        UserProgrammingDisplayFinchCommands(commands);
                        break;

                    case "d":
                        UserProgrammingDisplayExecuteFinchCommands(roboF, commands, commandParameters);
                        break;

                    case "e":
                        UserProgramingDisplayInteractiveDriving (roboF, commands, commandParameters);
                        break;


                    case "q":
                        quitUserProgramming = true;
                        break;

                    default:
                        DisplayWrongInput();
                        break;
                }
            } while (!quitUserProgramming);

        }

        /// <summary>
        ///                 Interactive Driving
        /// </summary>
        /// <param name="roboF"></param>
        /// <param name="commands"></param>
        /// <param name="commandParameters"></param>
        
        static void UserProgramingDisplayInteractiveDriving (
            Finch roboF,
            List<Command> commands,
            (int motorSpeed, int ledBrightness, double waitSeconds) commandParameters)

        {
            int motorSpeed = commandParameters.motorSpeed;
            //int ledBrightness = commandParameters.ledBrightness;
            //int waitMiliSeconds = (int)(commandParameters.waitSeconds * 1000);      Didnt end up using these
            const int TURNING_MOTOR_SPEED = 150;
            bool doneDriving = false;
            string driveShow = null;
            ///
            ///     Menu Interactive Driving
            /// 
            DisplayScreenHeader("Interactive Driving");
            Console.WriteLine();
            DisplayStringWithBorder("WARNING: Do not hold the key down!");
            Console.WriteLine("\tRobot will drive forward and backwards at user defined speed");
            Console.WriteLine("\tfrom User Command Parameters.\n");
            Console.WriteLine("\t   Command Options:");
            Console.WriteLine("\tw) Drive Forwards");
            Console.WriteLine("\ta) Turn Left");
            Console.WriteLine("\ts) Drive Backwards");
            Console.WriteLine("\td) Turn Right");
            Console.WriteLine("\te) Toggle Random LED Value");
            Console.WriteLine("\tb) Random Beep");
            Console.WriteLine("\tq) Quit");
            DisplayContinuePrompt();

            do
            {           // Reads the key and executes the command immediately
                string drive = Console.ReadKey().KeyChar.ToString(); 
                
                switch (drive)
                {
                    case "w":
                        roboF.setMotors(motorSpeed, motorSpeed);
                        driveShow = Command.MOVEFORWARD.ToString();
                        roboF.wait(500);
                        roboF.setMotors(0,0);

                        break;
                    case "a":
                        roboF.setMotors(-TURNING_MOTOR_SPEED, TURNING_MOTOR_SPEED);
                        driveShow = Command.TURNLEFT.ToString();
                        roboF.wait(500);
                        roboF.setMotors(0, 0);
                        break;
                    case "s":
                        roboF.setMotors(-motorSpeed, -motorSpeed);
                        driveShow = Command.MOVEBACKWARD.ToString();
                        roboF.wait(500);
                        roboF.setMotors(0, 0);
                        break;
                    case "d":
                        roboF.setMotors(TURNING_MOTOR_SPEED, -TURNING_MOTOR_SPEED);
                        driveShow = Command.TURNRIGHT.ToString();
                        roboF.wait(500);
                        roboF.setMotors(0, 0);
                        break;
                    case "e":
                        Random random = new System.Random();   // with all the randoms I probably should have made a random Method...
                        int ledValue0 = random.Next(1, 255);
                        int ledValue1 = random.Next(1, 255);
                        int ledValue2 = random.Next(1, 255);
                        roboF.setLED(ledValue0, ledValue1, ledValue2);
                        driveShow= $"led r:{ledValue0} led g:{ledValue1} led b:{ledValue2}  ";
                        break;
                    case "b":
                        Random randomSound = new System.Random();
                        int soundValue = randomSound.Next(1, 800);
                        roboF.noteOn(soundValue);
                        roboF.wait(200);
                        roboF.noteOff();
                        driveShow = $"Beeping at a frequency :{soundValue}";
                        break;

                    case "q":
                        doneDriving = true;
                        break;
                    default:
                        DisplayWrongInput();
                        Console.WriteLine("\t| w: Forward | a: Left | s: Back | d: Right | e: LED | b: Beep | q: Quit |");
                        break;

                }

                Console.WriteLine($"\t{driveShow}");

            } while (!doneDriving);


        }




        /// <summary>
        ///             Executing the commands
        /// </summary>
        /// <param name="roboF"></param>
        /// <param name="commands"></param>
        /// <param name="commandParameters"></param>

        static void UserProgrammingDisplayExecuteFinchCommands(
            Finch roboF,
            List<Command> commands,
            (int motorSpeed, int ledBrightness, double waitSeconds) commandParameters)
        {
            int motorSpeed = commandParameters.motorSpeed;
            int ledBrightness = commandParameters.ledBrightness;
            int waitMiliSeconds = (int)(commandParameters.waitSeconds * 1000);
            string commandFeedback = "";
            const int TURNING_MOTOR_SPEED = 150;

            DisplayScreenHeader("Execute Commands");

            Console.WriteLine("The finch robot will now execute the commands");
            DisplayContinuePrompt();

            foreach (Command command in commands)
            {
                switch (command)
                {
                    case Command.NONE:
                        break;

                    case Command.MOVEFORWARD:
                        roboF.setMotors(motorSpeed, motorSpeed);
                        commandFeedback = Command.MOVEFORWARD.ToString();
                        break;

                    case Command.MOVEBACKWARD:
                        roboF.setMotors(-motorSpeed, -motorSpeed);
                        commandFeedback = Command.MOVEBACKWARD.ToString();
                        break;

                    case Command.STOPMOTORS:
                        roboF.setMotors(0, 0);
                        commandFeedback = Command.STOPMOTORS.ToString();
                        break;

                    case Command.WAIT:
                        roboF.wait(waitMiliSeconds);
                        commandFeedback = Command.WAIT.ToString();
                        break;

                    case Command.TURNRIGHT:
                        roboF.setMotors(TURNING_MOTOR_SPEED, -TURNING_MOTOR_SPEED);
                        commandFeedback = Command.TURNRIGHT.ToString();
                        break;

                    case Command.TURNLEFT:
                        roboF.setMotors(-TURNING_MOTOR_SPEED, TURNING_MOTOR_SPEED);
                        commandFeedback = Command.TURNLEFT.ToString();
                        break;

                    case Command.LEDON:
                        roboF.setLED(ledBrightness, ledBrightness, ledBrightness);
                        commandFeedback = Command.LEDON.ToString();
                        break;

                    case Command.LEDOFF:
                        roboF.setLED(ledBrightness, ledBrightness, ledBrightness);
                        commandFeedback = Command.LEDOFF.ToString();
                        break;

                    case Command.TEMPERATURE:
                        commandFeedback = $"temperature: {roboF.getTemperature().ToString("n2")}\n";
                        break;

                    /*case Command.DONE:
                        commandFeedback = Command.DONE.ToString(); //wasnt needed
                        break;
                    */
                    default:

                        break;
                }
                Console.WriteLine($"\t{commandFeedback}");
            }
            ///
            /// Just in case lets turn everything off
            /// 
            roboF.noteOff();
            roboF.setMotors(0,0);
            roboF.setLED(0,0,0);
            DisplayContinuePrompt();
        }

        static void UserProgrammingDisplayFinchCommands(List<Command> commands)
        {
            DisplayScreenHeader("Show Commands");

            foreach (Command command in commands)
            {
                Console.WriteLine($"\t{command}");
            }


            DisplayContinuePrompt();
        }

        static void UserProgramingDisplayGetFinchCommands(List<Command> commands)
        {
            Command command = Command.NONE;

            DisplayScreenHeader("Finch Robot Commands");

            int commandCount = 1;
            DisplayStringWithBorder("List of available commands");
            
            foreach (string commandName in Enum.GetNames(typeof(Command)))
            {
                Console.Write($"\t--{commandName.ToLower()}");
                if (commandCount % 1 == 0) Console.Write("\n");
                commandCount++;
            }

            Console.WriteLine();

            while (command != Command.DONE)
            {
                Console.Write("\tEnter command: ");

                if (Enum.TryParse(Console.ReadLine().ToUpper(), out command))
                {
                    commands.Add(command);
                }
                else
                {
                    DisplayStringWithBorder("Please enter a command from the list.");
                }
            }
        
    }
        /// <summary>
        ///
        ///                             Get command parameters from user
        /// 
        /// </summary>
        /// Its a tuple
        /// 
        static (int motorSpeed, int ledBrightness, double waitSeconds) UserProgrammingDisplayGetParameters()
        {

            DisplayScreenHeader("Command Parameters");

            //
            // das tuple
            //
            (int motorSpeed, int ledBrightness, double waitSeconds) commandParameters;
            commandParameters.ledBrightness = 0;
            commandParameters.motorSpeed = 0;
            commandParameters.waitSeconds = 0;

            DisplayScreenHeader("User Command Parameters");

            ValidationGetValidInteger("\tEnter Motor Speed, Range (1 - 255): ", 1, 255, out commandParameters.motorSpeed);
            ValidationGetValidInteger("\tEnter LED brightness, Range (1 - 255): ", 1, 255, out commandParameters.ledBrightness);
            ValidationGetValidDouble("\tEnter time to wait in seconds, Range (1 - 10): ", 0, 10, out commandParameters.waitSeconds); 
            // actually accepts anything greater than 0 and less than 10


            Console.WriteLine($"\tThe motor speed is set to: {commandParameters.motorSpeed}");
            Console.WriteLine($"\tThe wait time is set to: {commandParameters.waitSeconds} seconds");
            Console.WriteLine($"\tThe LED brightness is set to: {commandParameters.ledBrightness}");

            DisplayContinuePrompt();

            return commandParameters;
        }



        #endregion

        #region VALIDATION
        /// <summary>
        /// What a great idea . Methods for validation!
        /// </summary>
        /// <param name="valid1"></param>
        /// <param name="valid2"></param>
        /// <param name="valid3"></param>
        /// <param name="motorSpeed"></param>

        static void ValidationGetValidInteger(string valid1, int valid2, int valid3, out int motorSpeed)  //for user programming section
        {
            bool validResponse= false;
            do
            {
                Console.Write(valid1);
                bool isNumber = Int32.TryParse(Console.ReadLine(), out motorSpeed);

                if (isNumber == true && motorSpeed <= 255 && motorSpeed >= 1)
                {
                    validResponse = true;
                }
                else
                {
                    Console.Clear();
                    Console.WriteLine("\tEnter integer between 1 and 255\n");
                }
            } while (!validResponse);
        }

        static void ValidationGetValidDouble(string valid1, int valid2, int valid3, out double waitSeconds) //for user programming section
        {
            bool validResponse = false;
            do
            {
                Console.Write(valid1);
                bool isDouble = Double.TryParse(Console.ReadLine(), out waitSeconds);

                if (isDouble == false || waitSeconds < 0 || waitSeconds > 10)
                {
                    Console.Clear();
                    Console.WriteLine("Please enter an number between 0 and 10");
                }
                else
                {
                    validResponse = true;
                }
            } while (!validResponse);
        }

        #endregion validation

        #region FINCH ROBOT MANAGEMENT

        /// <summary>
        /// *****************************************************************
        /// *               Disconnect the Finch Robot                      *
        /// *****************************************************************
        /// </summary>
        /// <param name="roboF">finch robot object</param>
        static void DisplayDisconnectroboF(Finch roboF)
        {
            Console.CursorVisible = false;

            DisplayScreenHeader("Disconnect Finch Robot");

            Console.WriteLine("\tAbout to disconnect from the Finch robot.");
            DisplayContinuePrompt();

            roboF.disConnect();

            Console.WriteLine("\tThe Finch robot is now disconnected.");

            DisplayMenuPrompt("Main Menu");
        }

        /// <summary>
        /// *****************************************************************
        /// *                  Connect the Finch Robot                      *
        /// *****************************************************************
        /// </summary>
        /// <param name="roboF">finch robot object</param>
        /// <returns>notify if the robot is connected</returns>
        static bool DisplayConnectroboF(Finch roboF)
        {
            Console.CursorVisible = false;

            bool robotConnected;

            DisplayScreenHeader("Connect Finch Robot");

            Console.WriteLine(
                "\tAbout to connect to Finch robot. Please be sure the USB cable is connected to the robot and computer now.");
            DisplayContinuePrompt();

            robotConnected = roboF.connect();

            // provide user feedback - text, lights, sounds
            Random randomtest = new System.Random();
            roboF.noteOn(200);
            roboF.wait(300);
            roboF.noteOff();
            Console.WriteLine("\tPerforming a connection test with lights:");
            for (int count = 0; count < 5; count++)
            {
                int ledValue0 = randomtest.Next(1, 255);
                int ledValue1 = randomtest.Next(1, 255);
                int ledValue2 = randomtest.Next(1, 255);
                roboF.setLED(ledValue0, ledValue1, ledValue2);
                int randWait = randomtest.Next(200, 1000);
                roboF.wait(randWait);
                Console.WriteLine("\tled r {0}:  " +
                                  "led g {1}:  " +
                                  "led b {2}:  ",
                    ledValue0,
                    ledValue1,
                    ledValue2);
                
            }
            DisplayMenuPrompt("Main Menu");

            //
            // reset finch robot
            //
            roboF.setLED(0, 0, 0);
            roboF.noteOff();

            return robotConnected;
        }

        #endregion

        #region USER INTERFACE

        /// <summary>
        /// *****************************************************************
        /// *                     Welcome Screen                            *
        /// *****************************************************************
        /// </summary>
        static void DisplayWelcomeScreen()
        {
            Console.CursorVisible = false;

            Console.Clear();
            Console.WriteLine();
            DisplayStringWithBorder("Finch Control");
            Console.WriteLine();

            DisplayContinuePrompt();
        }

        /// <summary>
        /// *****************************************************************
        /// *                     Closing Screen                            *
        /// *****************************************************************
        /// </summary>
        static void DisplayClosingScreen()
        {
            Console.CursorVisible = false;

            Console.Clear();
            Console.WriteLine();
            DisplayScreenHeader("Thank you for using Finch Control!");
            Console.WriteLine();

            DisplayContinuePrompt();
        }

        /// <summary>
        /// display continue prompt
        /// </summary>
        static void DisplayContinuePrompt()
        {
            Console.WriteLine();
            Console.WriteLine("\tPress any key to continue.");
            Console.ReadKey();
        }

        /// <summary>
        /// display menu prompt
        /// </summary>
        static void DisplayMenuPrompt(string menuName)
        {
            Console.WriteLine();
            Console.WriteLine($"\tPress any key to return to the {menuName} Menu.");
            Console.ReadKey();
        }

        /// <summary>
        /// display screen header
        /// </summary>
        static void DisplayScreenHeader(string headerText)
        {
            Console.Clear();
            Console.WriteLine();
            DisplayStringWithBorder(headerText);
            Console.WriteLine();
        }

        private static void DisplayChooseAnOption()
        {
            Console.Write("\tChoose an Option: ");
        }

        private static void DisplayWrongInput()
        {
            Console.Clear();
            Console.WriteLine("\n\tThat's NOT one of the options.");
            Console.WriteLine("\ttry again...");
            Console.WriteLine("\tPress any key:");
            Console.ReadKey();
        }

        public static void DisplayStringWithBorder(string word) //added padding for menus
        {
            const int EXTRA_STARS = 4;
            const string SYMBOL = "*";
            int size = word.Length + EXTRA_STARS;
            int x;
            Console.Write("\t\t");
            for (x = 0; x < size; ++x)
                Console.Write(SYMBOL);
            Console.WriteLine();
            Console.WriteLine("\t\t"+ SYMBOL + " " + word + " " + SYMBOL);
            Console.Write("\t\t");
            for (x = 0; x < size; ++x)
                Console.Write(SYMBOL);
            Console.WriteLine();

            #endregion


        }
    }
}
///
// trying to pull finchname out to use but my brain is now mush
///
/*
static string fname;





(string userName)
{
List<(string userName, string password, string finchName)> registeredUserLoginInfo = new List<(string userName, string password, string finchName)>();


registeredUserLoginInfo = ReadLoginInfoData();

    //
    // loop through the list of registered user login tuples and check each one against the login info
    //
    foreach ((string userName, string password, string finchName) userLoginInfo in registeredUserLoginInfo)
{
    if (userLoginInfo.userName == userName)
    {
        fname = userLoginInfo.finchName;
        userName = userLoginInfo.userName;

        break;
    }
}

return fname;
}*/