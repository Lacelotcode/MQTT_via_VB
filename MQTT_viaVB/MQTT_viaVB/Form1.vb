Imports System
Imports System.Text
Imports uPLibrary.Networking.M2Mqtt
Imports uPLibrary.Networking.M2Mqtt.Messages


Public Class Form1

    Dim Mqclient As MqttClient
    Dim brokeraddress As String = "192.168.107.114"
    Dim brokerport As Integer = 2005
    Dim Status_connect As Integer = 0
    'create thread for auto connect'
    Dim T1 As System.Threading.Thread
    Dim T2 As System.Threading.Thread

    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Mqclient = New MqttClient(brokeraddress, brokerport, False, Nothing, Nothing, MqttSslProtocols.None)
        Mqclient.Connect("pub", "", "")
        Try
            If Mqclient.IsConnected Then
                T1 = New System.Threading.Thread(AddressOf Auto_connect)
                T2 = New System.Threading.Thread(AddressOf MqSubscribe)
                T1.Start()
                T2.Start()
                'Delete comment here'
            End If

        Catch ex As Exception

        End Try
    End Sub

    Private Sub MsgReceived(sender As Object, e As MqttMsgPublishEventArgs)
        Dim topic As String = e.Topic
        Dim message As String = Encoding.UTF8.GetString(e.Message)
        If topic = "PLC" Then
            TextBox1.BeginInvoke(Sub() TextBox1.Text += message)
        ElseIf topic = "PLCsend" Then
            TextBox1.BeginInvoke(Sub() TextBox1.Text += message)
        End If
    End Sub

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        Mqclient.Publish("PLC", Encoding.UTF8.GetBytes(TextBox2.Text))

    End Sub

    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        T1.Abort()
        T2.Abort()
        Mqclient.Disconnect()
    End Sub
    Sub Auto_connect()
        Dim topic(0) As String
        Dim Qos(0) As Byte
        topic(0) = "PLC"
        Qos(0) = MqttMsgBase.QOS_LEVEL_AT_LEAST_ONCE
        Do While Status_connect = 0

            If Not Mqclient.IsConnected Then
                Mqclient.Connect("pub", "", "")
                Status_connect = 1
            End If
        Loop
        'AddHandler Mqclient.MqttMsgPublishReceived, AddressOf MsgReceived
        Mqclient.Subscribe(topic, Qos)
        Status_connect = 0
    End Sub
    Sub MqSubscribe()

        Dim topic(0) As String
        Dim Qos(0) As Byte
        topic(0) = "PLC"
        Qos(0) = MqttMsgBase.QOS_LEVEL_AT_LEAST_ONCE
        AddHandler Mqclient.MqttMsgPublishReceived, AddressOf MsgReceived
        Mqclient.Subscribe(topic, Qos)
        'topic(0) = "PLC"
        'topic(1) = "PLCsend"
        'Qos(0) = MqttMsgBase.QOS_LEVEL_AT_LEAST_ONCE
        'Qos(1) = MqttMsgBase.QOS_LEVEL_AT_LEAST_ONCE

    End Sub
End Class
