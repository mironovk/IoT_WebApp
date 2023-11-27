// #include <Arduino.h>
// #include <SFE_BMP180.h>
// #include <Wire.h>
// #include <WiFi.h>
// #include <NTPClient.h>
// #include <WiFiUdp.h>
// // #include <MySQL_Generic.h>
// // #include <MySQL_Generic_Connection.h>
// // #include <MySQL_Generic_WiFi.h>
// #include <MySQL_Connection.h>
// #include <MySQL_Cursor.h>

// #define DELAY 5000

// // WiFi ssid & passwd
// // WiFiServer server(443);
// String hostname = "ESP32";
// char ssid[] = "Xiaomi_2DF3";    // your SSID
// char pass[] = "6ak09zr6";       // your SSID Password
// // char ssid[] = "iPhone (Кирилл) (2)";    // your SSID
// // char pass[] = "12345678";       // your SSID Password

// // Get current time & date
// WiFiClient client;            // Use this for WiFi instead of EthernetClient
// WiFiUDP ntpUDP;
// NTPClient timeClient(ntpUDP);
// String formattedDate;

// // DB initialization
// // byte mac_addr[] = { 0xDE, 0xAD, 0xBE, 0xEF, 0xFE, 0xED };

// IPAddress server_addr(192,168,31,229);  // IP of the MySQL *server* here
// // IPAddress server_addr(192,168,31,229);  // IP of the MySQL 5.7.42
// char user[] = "sensor";              	// MySQL user login username
// char password[] = "sensor";        		// MySQL user login password
// // char user[] = "root";              	// MySQL user login username
// // char password[] = "root";        		// MySQL user login password
// char database[] = "SENSOR_DB";
// MySQL_Connection conn((Client *)&client);

// String insertCmd = "INSERT INTO SENSOR_DB.SENSOR_DATA_TABLE (SensorName, DataType, Position, Value, Date) VALUES (\'%s\', \'%s\', \'%s\', \'%f\', \'%s\');";
// String SensorName = "BMP180";
// String Position = "777";

// char sqlCmd[1024] = {0};


// // BMP180 initialization
// SFE_BMP180 measure;


// void setup()
// {
// 	Serial.begin(115200);

// 	if (measure.begin()) {
// 		Serial.println("BMP180 init success!\n");
// 	}
// 	else {
// 		Serial.println("BMP180 init failed!\n\n");
// 		while(1); // Pause forever.
// 	}

// 	// Begin WiFi section
// 	WiFi.mode(WIFI_STA);
// 	WiFi.config(INADDR_NONE, INADDR_NONE, INADDR_NONE, INADDR_NONE);
// 	WiFi.setHostname(hostname.c_str()); //define hostname

// 	Serial.print("Connecting to ");
// 	Serial.print(ssid);
// 	WiFi.begin(ssid, pass);
// 	while (WiFi.status() != WL_CONNECTED) {
// 		for (int cnt = 0; cnt < 3; cnt++){
// 			delay(300);
// 			Serial.print(".");
// 		}
// 		Serial.print("\b\b\b");
// 	}

// 	// Print local IP address and start web server
// 	Serial.println("\nWiFi connected!");
// 	Serial.print("IP address: ");
// 	Serial.println(WiFi.localIP());

// 	// Initialize a NTPClient to get time
// 	timeClient.begin();
// 	timeClient.setTimeOffset(3600*3);	// GMT +3 = 3600*3

// 	Serial.print("Connecting to database");
// 	// conn.connect(server_addr, 3306, user, password);
// 	// conn.connect(server_addr, 3306, user, password, database);
// 	while (conn.connect(server_addr, 3306, user, password) != true) {
// 		// Serial.println("\nDBG");
// 		// for (int cnt = 0; cnt < 3; cnt++){
// 			// delay(500);
// 			Serial.print(".");
// 		// }
// 		// Serial.print("\b\b\b");
// 	}
// 	Serial.println("\nConnected!!!");
// 	// if (conn.connect(server_addr, 3306, user, password, "SENSOR_DB")) {
// 	// 	Serial.print("\nConnected to database!");
// 	// }
// 	// Serial.print("\nConnected to database!");
// }

// void loop()
// {
// 	//////////////////////////////////////////////////////////////////////////////////////////
// 	//// Getting Time & Date
// 	timeClient.update();
// 	formattedDate = timeClient.getFormattedDate();
// 	formattedDate.setCharAt(10, ' ');
// 	formattedDate.remove(19);
// 	Serial.println(formattedDate);
	
// 	//////////////////////////////////////////////////////////////////////////////////////////
// 	//// Measure Temperature & Pressure
// 	char status;
// 	double T, P, P_mmHg;

// 	status = measure.startTemperature();
// 	if (status == 0) {
// 		Serial.println("startTemperature failed!\n\n");
// 		while(1); // Pause forever.
// 	}

// 	// Wait for the measurement to complete:
// 	delay(status);

// 	status = measure.getTemperature(T);
// 	if (status == 0) {
// 		Serial.println("getTemperature failed!\n\n");
// 		while(1); // Pause forever.
// 	}

// 	status = measure.startPressure(3);
// 	if (status == 0) {
// 		Serial.println("startPressure failed!\n\n");
// 		while(1); // Pause forever.
// 	}
	
// 	// Wait for the measurement to complete:
// 	delay(status);

// 	status = measure.getPressure(P, T);
// 	if (status == 0) {
// 		Serial.println("getPressure failed!\n\n");
// 		while(1); // Pause forever.
// 	}
// 	P_mmHg = P*0.750063755419211;
// 	// Print out the measurement results:
// 	// Temperature
// 	Serial.print("temperature: ");
// 	Serial.print(T, 2);
// 	Serial.print(" °C\n");

// 	// Pressure
// 	Serial.print("absolute pressure: ");
// 	Serial.print(P, 2);
// 	Serial.print(" mbar, ");
// 	Serial.print(P_mmHg, 2);
// 	Serial.println(" mmHg\n");

// 	// pTemperatureChar->setValue(T);
// 	// pTemperatureChar->notify();
// 	// pPressureChar->setValue(P_mmHg);
// 	// pPressureChar->notify();
	
// 	//////////////////////////////////////////////////////////////////////////////////////////
// 	//// Insert P, T values into Data Base

// 	// // Insert Temperature
// 	// memset(sqlCmd, 0, sizeof(sqlCmd));
// 	// sprintf(sqlCmd, insertCmd.c_str(), SensorName.c_str(), "Temperature", Position.c_str(), T, formattedDate);
// 	// Serial.println(sqlCmd);
// 	// MySQL_Cursor request = MySQL_Cursor(&conn);
// 	// request.execute(sqlCmd);

// 	// // Insert Pressure
// 	// memset(sqlCmd, 0, sizeof(sqlCmd));
// 	// sprintf(sqlCmd, insertCmd.c_str(), SensorName.c_str(), "Pressure", Position.c_str(), P_mmHg, formattedDate);
// 	// Serial.println(sqlCmd);
// 	// request.execute(sqlCmd);

// 	delay(DELAY);  // Pause for 5 seconds.
// }


/*********************************************************************************************************************************
  Connect_Default_Database_WiFi.ino

  Library for communicating with a MySQL or MariaDB Server

  Based on and modified from Dr. Charles A. Bell's MySQL_Connector_Arduino Library https://github.com/ChuckBell/MySQL_Connector_Arduino
  to support nRF52, SAMD21/SAMD51, SAM DUE, STM32F/L/H/G/WB/MP1, ESP8266, ESP32, etc. boards using W5x00, ENC28J60, LAM8742A Ethernet,
  WiFiNINA, ESP-AT, built-in ESP8266/ESP32 WiFi.

  The library provides simple and easy Client interface to MySQL or MariaDB Server.

  Built by Khoi Hoang https://github.com/khoih-prog/MySQL_MariaDB_Generic
  Licensed under MIT license
 **********************************************************************************************************************************/
/*
/*
  MySQL Connector/Arduino Example : connect with default database

  This example demonstrates how to connect to a MySQL server and specifying
  the default database when connecting. 

  For more information and documentation, visit the wiki:
  https://github.com/ChuckBell/MySQL_Connector_Arduino/wiki.

  INSTRUCTIONS FOR USE

  1) Change the address of the server to the IP address of the MySQL server
  2) Change the user and password to a valid MySQL user and password
  3) Connect a USB cable to your Arduino
  4) Select the correct board and port
  5) Compile and upload the sketch to your Arduino
  6) Once uploaded, open Serial Monitor (use 115200 speed) and observe

  If you do not see messages indicating you have a connection, refer to the
  manual for troubleshooting tips. The most common issues are the server is
  not accessible from the network or the user name and password is incorrect.

  Note: The MAC address can be anything so long as it is unique on your network.

  Created by: Dr. Charles A. Bell
*/

#include "defines.h"
#include "Credentials.h"
// #include "./defines.h"

#include <MySQL_Generic.h>

#define USING_HOST_NAME     false

#if USING_HOST_NAME
  // Optional using hostname, and Ethernet built-in DNS lookup
  char server[] = "192.168.31.245@localhost"; // change to your server's hostname/URL
#else
  IPAddress server(192, 168, 31, 245);
#endif

uint16_t server_port = 3306;    //3306;

char default_database[]   = "SENSOR_DB";

MySQL_Connection conn((Client *)&client);

void setup()
{
  Serial.begin(115200);
  while (!Serial && millis() < 5000); // wait for serial port to connect

  MYSQL_DISPLAY1("\nStarting Connect_Default_Database_WiFi on", BOARD_NAME);
  MYSQL_DISPLAY(MYSQL_MARIADB_GENERIC_VERSION);

  // Remember to initialize your WiFi module
#if ( USING_WIFI_ESP8266_AT  || USING_WIFIESPAT_LIB ) 
  #if ( USING_WIFI_ESP8266_AT )
    MYSQL_DISPLAY("Using ESP8266_AT/ESP8266_AT_WebServer Library");
  #elif ( USING_WIFIESPAT_LIB )
    MYSQL_DISPLAY("Using WiFiEspAT Library");
  #endif
  
  // initialize serial for ESP module
  EspSerial.begin(115200);
  // initialize ESP module
  WiFi.init(&EspSerial);

  MYSQL_DISPLAY(F("WiFi shield init done"));

  // check for the presence of the shield
  if (WiFi.status() == WL_NO_SHIELD)
  {
    MYSQL_DISPLAY(F("WiFi shield not present"));
    // don't continue
    while (true);
  }
#endif

  // Begin WiFi section
  MYSQL_DISPLAY1("Connecting to", ssid);

  WiFi.begin(ssid, pass);
  
  while (WiFi.status() != WL_CONNECTED) 
  {
    delay(500);
    MYSQL_DISPLAY0(".");
  }

  // print out info about the connection:
  MYSQL_DISPLAY1("Connected to network. My IP address is:", WiFi.localIP());

  MYSQL_DISPLAY3("Connecting to SQL Server @", server, ", Port =", server_port);
  MYSQL_DISPLAY5("User =", user, ", PW =", password, ", DB =", default_database);
}

void loop()
{
  MYSQL_DISPLAY("Connecting...");
  
  //if (conn.connect(server, server_port, user, password, default_database))
  if (conn.connectNonBlocking(server, server_port, user, password, default_database) != RESULT_FAIL)
  {
    MYSQL_DISPLAY("Closing connection...");
    conn.close();                     // close the connection
  } 
  else 
  {
    MYSQL_DISPLAY("\nConnect failed. Trying again on next iteration.");
  }

  MYSQL_DISPLAY("\nSleeping...");
  MYSQL_DISPLAY("================================================");
 
  delay(60000);
}
