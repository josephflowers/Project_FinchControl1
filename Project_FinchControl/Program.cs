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
    // Description: Starter solution with the helper methods,
    //              opening and closing screens, and the menu
    // Application Type: Console
    // Author: Joseph Flowers. With Starter Solution by John Velis
    // Dated Created: 11/15/2020
    // Last Modified: 11/22/2020
    //
    // **************************************************

    class Program
    {
        /// <summary>
        /// first method run when the app starts up
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {
            SetTheme();

            DisplayWelcomeScreen();
            DisplayMenuScreen();
            DisplayClosingScreen();
        }

        /// <summary>
        /// setup the console theme
        /// </summary>
        static void SetTheme()
        {
            Console.ForegroundColor = ConsoleColor.DarkBlue;
            Console.BackgroundColor = ConsoleColor.White;
        }

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
                Console.WriteLine("\ta) Connect Finch Robot");
                Console.WriteLine("\tb) Talent Show");
                Console.WriteLine("\tc) Data Recorder");
                Console.WriteLine("\td) Alarm System");
                Console.WriteLine("\te) User Programming");
                Console.WriteLine("\tf) Disconnect Finch Robot");
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

        #region LEDTEST

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
            Console.WriteLine("Press any key to continue");
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

        ///my test
        ///
        /// 
        static void TestLeds(Finch roboF)
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
        /// end of my test
        ///   

        #endregion

        #region SOUNDS

        private static void RoboSounds(Finch roboF)
        {
            int[] sawInt =
            {
                5, 5, 5, 5, 7, 7, 7, 7, 7, 7, 7, 5, 5, 15, 20, 5, 5, 15, 20, 5, 5, 15, 20, 5, 5, 25, 50, 5, 5, 15, 20, 5, 5, 15, 20, 5, 5, 15, 20, 5, 5,
                25, 200,  
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

        #endregion

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
                        //TalentShowDisplayDance(roboF);
                        break;

                    case "c":
                        //TalentShowDisplayMixingItUp(roboF);
                        break;

                    case "d":
                        //TalentShowDisplayMysterious(roboF);
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

        /// <summary>
        /// *****************************************************************
        /// *               Talent Show > Light and Sound                   *
        /// *****************************************************************
        /// </summary>
        /// <param name="roboF">finch robot object</param>
        static void TalentShowDisplayLightAndSound(Finch roboF)
        {
            Console.CursorVisible = false;

            DisplayScreenHeader("Light and Sound");

            Console.WriteLine("\tThe Finch robot will not show off its glowing talent!");
            DisplayContinuePrompt();
            /////
            ///
            /// proving grounds
            ///
            ///
            ///
            Console.WriteLine("Leds Up");
            ledUp(roboF);
            Console.WriteLine("Leds Each Main Color");
            TestLeds(roboF);
            Console.WriteLine("Leds Randomized");
            LedRandom(roboF);
            Console.WriteLine("Robot Music");
            RoboSounds(roboF);
            Console.WriteLine("Press any key to continue");
            Console.ReadLine();

            Console.WriteLine("Modified light and sound up");
            for (int lightSoundLevel = 0; lightSoundLevel < 255; lightSoundLevel++)
            {
                roboF.setLED(lightSoundLevel, lightSoundLevel, lightSoundLevel);
                roboF.noteOn(lightSoundLevel * 9);
            }

            roboF.noteOff();
            Console.WriteLine("Press any key to continue");
            Console.ReadLine();

        }

        #endregion

        #region DATARECORDER
        /// 
        /// Data recorder is set to measure tempuratures from the finch
        /// 
        /// 

        private static void DataRecorderDisplayMenuScreen(Finch roboF)
        {
            Console.CursorVisible = true;
            int numberOfDataReadings = 0;
            double readingFrequency = 0;
            double[] temperatures = null;

            bool quitData = false; /// exit strategy
            do /// data recorder menu
            
            {
                DisplayScreenHeader("Data Recorder:");
                Console.WriteLine("\n\tChoose an option for temperature readings:");
                Console.WriteLine("\ta) How many readings are we taking?");
                Console.WriteLine("\tb) How often in seconds would you like to take a reading?");
                Console.WriteLine("\tc) Get readings:");
                Console.WriteLine("\td) Show results:");
                Console.WriteLine("\tq) Return to main menu");
                DisplayChooseAnOption();
                string MenuChoice = Console.ReadLine().ToLower();
                switch (MenuChoice)
                {
                    case "a":
                        numberOfDataReadings = DataRecorderDisplayGetNumberOfReadings();
                        break;

                    case "b":
                        readingFrequency = DataRecorderDisplayGetReadingFrequency();
                        break;

                    case "c":
                        temperatures = DataRecorderDisplayGetReadings(numberOfDataReadings, readingFrequency, roboF);
                        break;

                    case "d":
                        DataRecorderDisplayGetReadings(temperatures);
                        break;

                    case "q":
                        quitData = true;
                        break;

                    default:
                        DisplayWrongInput();
                        break;
                }
            } while (!quitData);
        }

        private static int DataRecorderDisplayGetNumberOfReadings()
        {
            Console.CursorVisible = true;
            DisplayScreenHeader("Option a: Get the number of readings:");

            Console.Write("\n\tEnter the number of readings we are taking: ");
            string userResponse = Console.ReadLine();

            int.TryParse(userResponse, out int numDataPoints); /// includes validation

            DisplayContinuePrompt();
            return numDataPoints;
        }

        private static double DataRecorderDisplayGetReadingFrequency()
        {
            Console.CursorVisible = true;
            DisplayScreenHeader("Option B : frequency of readings in seconds:");

            double.TryParse(Console.ReadLine(), out double readingFrequency); /// includes validation

            DisplayContinuePrompt();
            return readingFrequency;
        }

        private static double[] DataRecorderDisplayGetReadings(int numberOfDataReadings, double readingFrequency,
            Finch roboF)
        {
            Console.CursorVisible = false;
            double[] temperatures = new double[numberOfDataReadings];
            
            DisplayScreenHeader("Option c: Get readings:");
            Console.WriteLine($"\tNumber of data points: {numberOfDataReadings}");
            Console.WriteLine($"\tData point frequency: {readingFrequency}");
            Console.WriteLine();
            Console.WriteLine("\tReady to record temperature data.");
            DisplayContinuePrompt();

            for (int t = 0; t < numberOfDataReadings; t++)
            {
                temperatures[t] = roboF.getTemperature();
                Console.WriteLine($"\tReading {t + 1}: {temperatures[t]:n2} ");
                int waitInSeconds = (int) ((readingFrequency) * 1000); /// converting to seconds from miliseconds
                roboF.wait(waitInSeconds);
            }

            DisplayContinuePrompt();

            return temperatures;
        }

        private static void DataRecorderDisplayGetReadings(double[] temperatures)
        {
            DataRecorderDisplayTable(temperatures);
            DisplayContinuePrompt();
        }

        private static void DataRecorderDisplayTable(double[] temperatures)
        {
            Console.CursorVisible = false;
            DisplayScreenHeader("Option D : Show results");  /// Output screen

            Console.WriteLine(
                "************".PadLeft(21) + "************".PadLeft(20)
            );

            Console.WriteLine(
                "Recording".PadLeft(20) + "Temperature".PadLeft(21)
            );
            Console.WriteLine(
                "************".PadLeft(21) + "************".PadLeft(20)
            );

            for (int t = 0; t < temperatures.Length; t++)
            {
                Console.WriteLine(
                    (t + 1).ToString().PadLeft(20) +
                    temperatures[t].ToString("n2").PadLeft(20)
                );
            }
        }

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
                Console.WriteLine("\tc) Set minumum/maximum threshold");
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
        /// subfunctions in alarms
        /// 
        private static string DisplayScreenHeaderLightAlarmDisplaySetSensorsToMonitorInput()
        {
            List<string> correctSensors = new List<string>() { "a", "b", "c" };
            string sensorsToMonitorInput;
            
            
            DisplayScreenHeader("Sensors To Monitor");

            Console.WriteLine("\ta) Left");
            Console.WriteLine("\tb) Right");
            Console.WriteLine("\tc) Both");
            Console.Write("\tWhich sensor(s) would you like to monitor? ");
            sensorsToMonitorInput = Console.ReadLine().ToLower();

            

            
            

            if (correctSensors.Contains(sensorsToMonitorInput))
            {
                return sensorsToMonitorInput;
            }
            else
            {
                DisplayScreenHeader("Please input \"a\" for left, \"b\" for right, or \"c\" for both");
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
            /// I struggled with a few different ways to convert the abc menu to the words i wanted and settled on Replace
            /// 
            string rangeTypeOut = "og";
            if (rangeType == "a")
            {
                rangeTypeOut = rangeTypeOut.Replace("og", "Minimum");
            }
            if (rangeType == "b")
            {
                rangeTypeOut = rangeTypeOut.Replace("og", "Maximum");
            }
            DisplayScreenHeader("Minimum/Maximum Threshold Value");

            Console.WriteLine($"\tLeft light sensor current value: {roboF.getLeftLightSensor()}");
            Console.WriteLine($"\tRight light sensor current value: {roboF.getRightLightSensor()}");
            Console.WriteLine();
            Console.Write($"\tEnter The {rangeTypeOut} light value: ");

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
            int currentLightSensorValue = 0;
            ///
            /// again with the Replace
            /// 
            string sensorsToMonitor = "og";
            string rangeTypeOut = "og";
            if (sensorsToMonitorInput == "a")
            {
                sensorsToMonitor = sensorsToMonitor.Replace("og","Left");
            }
            if (sensorsToMonitorInput == "b")
            {
                sensorsToMonitor = sensorsToMonitor.Replace("og","Right");
            }
            if (sensorsToMonitorInput == "c")
            {
                sensorsToMonitor = sensorsToMonitor.Replace("og","Both");
            }

            if (rangeType == "a")
            {
                rangeTypeOut = rangeTypeOut.Replace("og", "Minimum");
            }
            if (rangeType == "b")
            {
                rangeTypeOut = rangeTypeOut.Replace("og", "Maximum");
            }

            DisplayScreenHeader("Set Light Alarm");
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
                        currentLightSensorValue = roboF.getLeftLightSensor();
                        break;

                    case "b":

                        currentLightSensorValue = roboF.getRightLightSensor();
                        break;

                    case "c":
                        currentLightSensorValue = (roboF.getRightLightSensor() + roboF.getLeftLightSensor()) / 2;
                        break;
                }
                switch (rangeType)
                {
                    case "a":
                        if (currentLightSensorValue < minMaxThresholdValue)
                        {
                            thresholdExceeded = true;
                        }
                        break;

                    case "b":
                        if (currentLightSensorValue > minMaxThresholdValue)
                        {
                            thresholdExceeded = true;
                        }
                        break;
                }
                roboF.wait(1000);
                secondsElapsed++;
                Console.WriteLine("\tCurrent Light Value: {0} ", currentLightSensorValue);
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

        private static void UserProgrammingDisplayMenuScreen(Finch roboF)
        {
            
            bool quitUserProgramming = false;

            do
            {
                DisplayScreenHeader("User Programming");
                Console.WriteLine("\tThis area is still under construction");
                Console.WriteLine("\ta) ");
                Console.WriteLine("\tb) ");
                Console.WriteLine("\tc) ");
                Console.WriteLine("\tq) Return to main menu");
                DisplayChooseAnOption();
                string MenuChoice = Console.ReadLine().ToLower();
                switch (MenuChoice)
                {
                    case "a":
                        
                        break;

                    case "b":
                        
                        break;

                    case "c":
                        
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


        #endregion

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

            Console.WriteLine("\tThe Finch robot is now disconnect.");

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

            // TODO test connection and provide user feedback - text, lights, sounds
            Random randomtest = new System.Random();
            Console.WriteLine("Performing a connection test with lights:");
            for (int count = 0; count < 10; count++)
            {
                int ledValue0 = randomtest.Next(1, 255);
                int ledValue1 = randomtest.Next(1, 255);
                int ledValue2 = randomtest.Next(1, 255);
                roboF.setLED(ledValue0, ledValue1, ledValue2);
                int randWait = randomtest.Next(200, 1000);
                roboF.wait(randWait);
                Console.WriteLine("led r {0}: " +
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
            Console.WriteLine("Press any key:");
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
