using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO.Ports;
using System.Text;

public class SerialCommunication
{
    public SerialCommunication()
    { 
    }
}

public class serial_communication : MonoBehaviour
{

    private SerialPort _serialPort;
    private string defaultPortName = "COM8";

    // Set Serial Port to connect with Arduino.
    private void SetSerialPort( SerialPort _serialPort )
    {
        // Create a new Serial Port object with default settings.
        _serialPort = new SerialPort();

        // Allow the user to set the appropriate properties.
        _serialPort.PortName = defaultPortName;
        _serialPort.BaudRate = 9600;

        _serialPort.ReadTimeout = 500;
        _serialPort.WriteTimeout = 500;

        _serialPort.Open();

        if (_serialPort.IsOpen) Debug.Log("Serial Port Open");
    }

    // Display Available Port values.
    public void CheckNowAvailablePortName()
    {

        Debug.Log("Available Ports : ");
        foreach (string s in SerialPort.GetPortNames())
        {
            Debug.Log( s );
        }
    }

    // Read Serial Port and classified Data.
    public void ClassifiedSerialPort( SerialPort _serialPort )
    {
        string data = "";

        if (_serialPort.IsOpen)
        {
            try
            {
                // Read 2 char
                data += _serialPort.ReadChar();
                data += _serialPort.ReadChar();

                switch( data )
                {
                    case "GY" :
                        break;
                    case "AC" :
                        break;
                    case "FI" :
                        break;
                }

                if (data != "")
                    Debug.Log("data : " + data);
            }
            catch (System.Exception)
            {
                Debug.Log("exception");
            }
        }
        else
        {
            Debug.Log("Serial Port Close");
        }

    }

    // Send Vibration Motor Signal.
    public void SendVibrationMotorSignal( SerialPort _serialPort )
    {

    }

    // Set Gyro Value by hand type and axis type.
    private void SetGyroValue( char hand, char axis, string value )
    {
    }

    // Set Accel Value by hand type and axis type.
    private void SetAccelValue( char hand, char axis, string value )
    {

    }

    // Set Each Finger Value according to the type of hand.
    private void SetFingerValue( char hand, string value )
    {

    }

    void Start ()
    {
	}
	
	void Update ()
    {
        data = "";

	}

    
}
