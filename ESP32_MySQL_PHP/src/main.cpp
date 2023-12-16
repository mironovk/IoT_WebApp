#include <Arduino.h>
#include <WiFi.h>
#include <WiFiClientSecure.h>
#include <HTTPClient.h>
#include <NTPClient.h>
#include <SFE_BMP180.h>

String URL = "http://192.168.31.64/sensor_prj/test.php/";

#define DELAY 1000

// WiFi ssid & passwd
char ssid[] = "Xiaomi_2DF3";    // your SSID
char pass[] = "6ak09zr6";       // your SSID Password
// char ssid[] = "iPhone (Кирилл) (2)";    // your SSID
// char pass[] = "12345678";       // your SSID Password
// char ssid[] = "ASUS-466";    // your SSID
// char pass[] = "ZyXEL7bbQ";       // your SSID Password

// Get current time & date
WiFiClient client;            // Use this for WiFi instead of EthernetClient
WiFiUDP ntpUDP;
NTPClient timeClient(ntpUDP);
String formattedDate;
HTTPClient http;

// DB initialization

// IPAddress server_addr(192,168,1,219);  // IP of the MySQL *server* here
// IPAddress server_addr(192,168,31,43);  // IP of the MySQL *server* here
// IPAddress server_addr(192,168,31,194);  // IP of the MySQL *server* here
// const char user[] = "sensor";              	// MySQL user login username
// const char password[] = "sensor";        		// MySQL user login password
// char user[] = "root";              	// MySQL user login username
// char password[] = "root";        		// MySQL user login password
// const char database[] = "SENSOR_DB";
// MySQL_Connection conn((Client *)&client);

// String insertCmd = "INSERT INTO SENSOR_DB.SENSOR_DATA_TABLE (SensorName, DataType, Position, Value, Date) VALUES (\'%s\', \'%s\', \'%s\', \'%f\', \'%s\');";
String SensorName = "BMP180";
String Position = "1";

// char sqlCmd[1024] = {0};


// BMP180 initialization
SFE_BMP180 measure;


void setup() {
	Serial.begin(115200);

	if (measure.begin()) {
		Serial.println("BMP180 init success!\n");
	}
	else {
		Serial.println("BMP180 init failed!\n\n");
		while(1); // Pause forever.
	}

	// Begin WiFi section

	Serial.print("Connecting to ");
	Serial.print(ssid);
	WiFi.begin(ssid, pass);
	while (WiFi.status() != WL_CONNECTED) {
		for (int cnt = 0; cnt < 3; cnt++){
			Serial.print(".");
			delay(500);
		}
		Serial.print("\b\b\b   \b\b\b");
	}

	// Print local IP address and start web server
	Serial.println("\nWiFi connected!");
	Serial.print("IP address: ");
	Serial.println(WiFi.localIP());

	// Initialize a NTPClient to get time
	timeClient.begin();
	timeClient.setTimeOffset(3600*3);	// GMT +3 = 3600*3
}

void loop() {
	while (WiFi.status() != WL_CONNECTED) {
		for (int cnt = 0; cnt < 3; cnt++){
			Serial.print(".");
			delay(500);
		}
		Serial.print("\b\b\b   \b\b\b");
	}

	//////////////////////////////////////////////////////////////////////////////////////////
	//// Getting Time & Date
	timeClient.update();
	formattedDate = timeClient.getFormattedDate();
	formattedDate.setCharAt(10, ' ');
	formattedDate.remove(19);
	Serial.println(formattedDate);
	
	//////////////////////////////////////////////////////////////////////////////////////////
	//// Measure Temperature & Pressure
	char status;
	double T, P, P_mmHg;

	status = measure.startTemperature();
	if (status == 0) {
		Serial.println("startTemperature failed!\n\n");
		while(1); // Pause forever.
	}

	// Wait for the measurement to complete:
	delay(status);

	status = measure.getTemperature(T);
	if (status == 0) {
		Serial.println("getTemperature failed!\n\n");
		while(1); // Pause forever.
	}

	status = measure.startPressure(3);
	if (status == 0) {
		Serial.println("startPressure failed!\n\n");
		while(1); // Pause forever.
	}
	
	// Wait for the measurement to complete:
	delay(status);

	status = measure.getPressure(P, T);
	if (status == 0) {
		Serial.println("getPressure failed!\n\n");
		while(1); // Pause forever.
	}
	P_mmHg = P*0.750063755419211;
	// Print out the measurement results:
	// Temperature
	Serial.print("temperature: ");
	Serial.print(T, 2);
	Serial.print(" °C\n");

	// Pressure
	Serial.print("absolute pressure: ");
	Serial.print(P, 2);
	Serial.print(" mbar, ");
	Serial.print(P_mmHg, 2);
	Serial.println(" mmHg\n");

	//////////////////////////////////////////////////////////////////////////////////////////
	//// Send Temperature & Pressure To MySQL Server

	String postData = "name=" + SensorName + "&pos=" + Position + "&temp=" + String(T) + "&press=" + String(P_mmHg) + "&dat=" + String(formattedDate) + "";
	// String postData = "name='BMP180'&pos='" + String(1) + "'&temp='" + String(20.5) + "'&press='" + String(765.60) + "'&dat='2023-12-06 01:25:51'";
	// String postData = "name=BMP180";

	
	// WiFiClientSecure *client = new WiFiClientSecure;
    // client->setInsecure();

	// http.begin(URL);
	http.begin(client, URL);
	// http.addHeader("Content-Type", "text/plain");
	http.addHeader("Content-Type", "application/x-www-form-urlencoded");

	int httpCode = http.POST(postData);
	String payload = http.getString();

	Serial.print("URL: "); Serial.println(URL);
	Serial.print("Data: "); Serial.println(postData);
	Serial.print("httpCode: "); Serial.println(httpCode);
	Serial.print("payload: "); Serial.println(payload);
	Serial.println("-----------------------------");

	http.end();
	delay(DELAY);
}