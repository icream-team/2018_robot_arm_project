﻿using UnityEngine;
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

        if (_serialThread != null) SetSerialThread();
    }
    public void SetSerialOpen()
    {
        if (_serialPort.IsOpen) return;

        SetSerialInformation();

        _serialPort.Open();
    }
    public void SetSerialClose()
    {
        if (!_serialPort.IsOpen) return;

        if (_serialThread != null) _serialThread.Join();

        _serialPort.Close();
    }
    public void SetStartSerial()
    {
        StartSerialThread();
    }

    // called when get data through the serial
    private void ClassifyFunction()
    {
        if (!_serialPort.IsOpen) SetSerialOpen();
        else
        {
            if (!isStart) ClassifyHandType();
            else if (!isClassify) SendStartMessage();
            else ClassifySerialValue();
        }
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
            data += _serialPort.ReadChar();
            data += _serialPort.ReadChar();
            switch (data)
            {
                case "GY":
                    data = "" + _serialPort.ReadChar();
                    values = _serialPort.ReadLine();
                    SetGyroValue(data[0], values);
                    break;
                case "AC":
                    data = "" + _serialPort.ReadChar();
                    values = _serialPort.ReadLine();
                    SetAccValue(data[0], values);
                    break;
                case "AN":
                    data = "" + _serialPort.ReadChar();
                    values = _serialPort.ReadLine();
                    SetAngValue(data[0], values);
                    break;
                case "PO":
                    data = "" + _serialPort.ReadChar();
                    values = _serialPort.ReadLine();
                    SetPosValue(data[0], values);
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
        if (!_serialPort.IsOpen) SetSerialOpen();
        else
        {
            _serialPort.Write("DMP");
            UnityEngine.Debug.Log("DMP");

            isClassify = true;
        }
    }
    public void SendVibrationMessage(int type, int strength)
    {
        if (!_serialPort.IsOpen) SetSerialOpen();
        else
        {
            _serialPort.Write("VI" + type + strength);
            UnityEngine.Debug.Log("VI" + type + strength);
        }
    }

    // set value received from the arduino
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
                ax = (float)Int32.Parse(values);
                break;
            case 'y':
                ay = (float)Int32.Parse(values);
                break;
            case 'z':
                ay = (float)Int32.Parse(values);
                break;
        }

    }
    private void SetPosValue(char axis, string values)
    {
    }
    private void SetFinValue(string values)
    {
        int intvalue = 0;

        foreach (char digit in values)
        {
            intvalue = intvalue + (int)(digit - '0');
            intvalue = intvalue << 1;
        }

        intvalue = intvalue >> 1;

        finger = intvalue;
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
    private void SetSerialThread()
    {
        _serialThread = new Thread(ClassifyFunction);
    }
    private void StartSerialThread()
    {
        _serialThread.Start();
    }
    private void StopSerialThread()
    {
        _serialThread.Join();
    }
}

public class SerialCommunication : MonoBehaviour
{
    private SerialManager mySerialManager;

    private int fingerCommand;
    private Vector3 temp_rotation, temp_position;

    // Use this for initialization
    void Start()
    {
        mySerialManager = new SerialManager();
        mySerialManager.SetSerialOpen();
    }

    // Update is called once per frame
    void Update()
    {
        temp_rotation = mySerialManager.GetAngValue();
        temp_rotation.x += 90;

        temp_position = mySerialManager.GetPosValue();
        temp_position.x += this.transform.position.x;
        temp_position.y += this.transform.position.y;
        temp_position.z += this.transform.position.z;

        this.transform.rotation = Quaternion.Euler(temp_rotation);
        this.transform.position = temp_position;
        fingerCommand = mySerialManager.GetFingerValue();
    }
}
