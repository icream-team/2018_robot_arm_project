using UnityEngine;
using System;
using System.Diagnostics;
using System.Threading;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.IO.Ports;

public class SerialManager
{
    private SerialPort _serialPort;
    private char _handType = 'R';
    private string defaultPortName = "COM8";
    private int BaudRate = 9600;
    private int ReadTimeout = 500;
    private int WriteTimeout = 500;
    private bool isStart = true;
    private bool isClassify = false;
    private bool isThread = false;
        
    private float ax = 0.0f, ay = 0.0f, az = 0.0f;
    private float px = 0.0f, py = 0.0f, pz = 0.0f;
    private int finger = 0;

    private Thread _serialThread;

    public SerialManager()
    {
        _serialPort = new SerialPort();
    }

    // set default information before serial opened
    public void SetSerialPort(string PortName)
    {
        this.defaultPortName = PortName;
    }
    public void SetBaudRate(int BaudRate)
    {
        this.BaudRate = BaudRate;
    }
    public void SetReadTimeout(int ReadTimeout)
    {
        this.ReadTimeout = ReadTimeout;
    }
    public void SetWriteTimeout(int WriteTimeout)
    {
        this.WriteTimeout = WriteTimeout;
    }
    public void SetHandType(char type)
    {
        if (type.Equals("L") || type.Equals("R"))
        {
            this._handType = type;
            isStart = true;
        }
    }

    // set serial status
    private void SetSerialInformation()
    {
        _serialPort.PortName = defaultPortName;
        _serialPort.BaudRate = BaudRate;
        _serialPort.ReadTimeout = ReadTimeout;
        _serialPort.WriteTimeout = WriteTimeout;

        SetSerialThread();

    }
    public void SetSerialOpen()
    {
        if (_serialPort.IsOpen) return;

        SetSerialInformation();

        _serialPort.Open();

        SendStartMessage();

        ClassifyFunction();
    }
    public void SetSerialClose()
    {
        if (!_serialPort.IsOpen) return;

        if (_serialThread != null) _serialThread.Join();

        _serialPort.Close();
    }

    // called when get data through the serial
    private void ClassifyFunction()
    {
        if (!_serialPort.IsOpen) return;

        UnityEngine.Debug.Log("classify function");

        //if (!isStart) ClassifyHandType();
        //else
        //{
            if (!isThread) StartSerialThread();
        //}
    }
    private void ClassifyHandType()
    {
        string data = "";

        try
        {
            data = _serialPort.ReadLine();

            if (data != null) SetHandType(data[0]);

        }
        catch (System.Exception)
        {
            UnityEngine.Debug.Log("System.Exception in ClassifyHandType");
        }
    }
    private void ClassifySerialValue()
    {
        string data = "";
        string values;

        try
        {
            data += (char)_serialPort.ReadChar();
            data += (char)_serialPort.ReadChar();
            UnityEngine.Debug.Log( "data : " + data);
            switch (data)
            {
                case "GY":
                    data = "" + _serialPort.ReadChar();
                    values = _serialPort.ReadLine();
                    SetGyroValue((char)data[0], values);
                    break;
                case "AC":
                    data = "" + _serialPort.ReadChar();
                    values = _serialPort.ReadLine();
                    SetAccValue((char)data[0], values);
                    break;
                case "AN":
                    data = "" + _serialPort.ReadChar();
                    values = _serialPort.ReadLine();
                    SetAngValue((char)data[0], values);
                    break;
                case "PO":
                    data = "" + _serialPort.ReadChar();
                    values = _serialPort.ReadLine();
                    SetPosValue((char)data[0], values);
                    break;
                case "FI":
                    data = "" + _serialPort.ReadChar();
                    values = _serialPort.ReadLine();
                    SetFinValue(values);
                    break;
            }
        }
        catch (System.Exception)
        {
            UnityEngine.Debug.Log("System.Exception in ClassifySerialValue");
        }
    }

    // send message to arduino
    private void SendStartMessage()
    {
        if (!_serialPort.IsOpen) return;

        _serialPort.Write("DMP");
        UnityEngine.Debug.Log("DMP");

        isClassify = true;
    }
    public void SendVibrationMessage(int type, int strength)
    {
        if (!_serialPort.IsOpen) return;

        _serialPort.Write("VI" + type + strength);
        UnityEngine.Debug.Log("VI" + type + strength);
    }

    public void SendVibrationMessage(int type)
    {
        if (!_serialPort.IsOpen) return;

        _serialPort.Write("V" + type);
        UnityEngine.Debug.Log("V" + type);
    }

    // set value received from the arduino
    private float SetStringValuesToFloatValue(string values)
    {
        string integer_part = "";
        string decimal_part = "";

        bool is_negative = false;
        bool is_there_decimal = false;

        foreach (char character in values)
        {
            if (character == '-') is_negative = true;
            else if (character == '.') is_there_decimal = true;
            else
            {
                if (is_there_decimal) decimal_part += character;
                else integer_part += character;
            }
        }

        return ((float)(Int32.Parse(integer_part) + (is_there_decimal ? Int32.Parse(decimal_part) * Math.Pow(10, decimal_part.Length * -1) : 0)) * ( is_negative ? -1 : 1 ));
    }
    private int SetBinaryValuesToIntValue(string values)
    {
        int intvalue = 0;

        foreach (char digit in values)
        {
            intvalue += (int)(digit - '0');
            intvalue <<= 1;
        }

        intvalue >>= 1;

        return intvalue;
    }
    private void SetGyroValue(char axis, string values)
    {

    }
    private void SetAccValue(char axis, string values)
    {

    }
    private void SetAngValue(char axis, string values)
    {
        switch (axis)
        {
            case 'x':
                ax = SetStringValuesToFloatValue(values);
                break;
            case 'y':
                ay = SetStringValuesToFloatValue(values);
                break;
            case 'z':
                az = SetStringValuesToFloatValue(values);
                break;
        }

        UnityEngine.Debug.Log(axis + " axis pos value  : " + values);

    }
    private void SetPosValue(char axis, string values)
    {
        switch (axis)
        {
            case 'x':
                px = SetStringValuesToFloatValue(values);
                break;
            case 'y':
                py = SetStringValuesToFloatValue(values);
                break;
            case 'z':
                pz = SetStringValuesToFloatValue(values);
                break;
        }

        UnityEngine.Debug.Log(axis + " axis pos value  : " + values);
    }
    private void SetFinValue(string values)
    {
        finger = SetBinaryValuesToIntValue(values);

        UnityEngine.Debug.Log( "finger signal : " + finger );
    }

    // get measure value
    public char GetHandType()
    {
        return _handType;
    }
    public Vector3 GetAngValue()
    {
        return new Vector3(ax, ay, az);
    }
    public Vector3 GetPosValue()
    {
        return new Vector3(px, py, pz);
    }
    public int GetFingerValue()
    {
        return finger;
    }

    // thread
    private void SerialThread()
    {
        while (isThread)
            ClassifySerialValue();
    }
    private void SetSerialThread()
    {
        _serialThread = new Thread(SerialThread);
    }
    private void StartSerialThread()
    {
        if (isThread) return;

        _serialThread.Start();
        isThread = true;
    }
    public void StopSerialThread()
    {
        isThread = false;
    }
}
