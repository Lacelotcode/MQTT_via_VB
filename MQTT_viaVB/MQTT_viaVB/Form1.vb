Imports System
Imports System.Text
Imports uPLibrary.Networking.M2Mqtt
Imports uPLibrary.Networking.M2Mqtt.Messages


Public Class Form1

    Dim Mqclient As MqttClient
    'Dim brokeraddress As String = IPaddressBox1.Text
    'Dim brokerport As Integer = Convert.ToInt32(PortBox1.Text)
    Dim Status_connect As Integer = 0
    'create thread for auto connect'
    Dim T1 As System.Threading.Thread
    Dim T2 As System.Threading.Thread

    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Init_connection()
    End Sub

    Private Sub MsgReceived(sender As Object, e As MqttMsgPublishEventArgs)
        Dim topic As String = e.Topic
        Dim message As String = Encoding.UTF8.GetString(e.Message)
        Dim dt As Date = Date.Now.ToString("yyyy/MM/dd HH:mm:ss")
        If topic = "PLC" Then
            TextBox1.BeginInvoke(Sub() TextBox1.Text += dt + "  value : " + message + vbNewLine)
        ElseIf topic = "PLCsend" Then
            TextBox1.BeginInvoke(Sub() TextBox1.Text += dt + "  value : " + message + vbNewLine)
        End If
    End Sub

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        Mqclient.Publish("PLC", Encoding.UTF8.GetBytes(TextBox2.Text))

    End Sub

    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        T1.Abort()
        T2.Abort()
        Mqclient.Disconnect()
        ToolStrip1.BackColor = Color.Red
        Status_connect = 1
    End Sub
    Private Sub Button3_Click(sender As Object, e As EventArgs) Handles Button3.Click
        If Not Mqclient.IsConnected Then
            Init_connection()
        End If
    End Sub
    Sub Auto_connect()
        Dim topic(0) As String
        Dim Qos(0) As Byte
        topic(0) = "PLC"
        Qos(0) = MqttMsgBase.QOS_LEVEL_AT_LEAST_ONCE
        Do While True
            Do While Status_connect = 0

                If Not Mqclient.IsConnected Then
                    Try
                        ToolStripLabel1.Text = "Reconnecting IP : " + IPaddressBox1.Text + "      Port : " + PortBox1.Text
                        ToolStrip1.BackColor = Color.Red
                        Mqclient.Connect("pub", "", "")
                        Status_connect = 1
                    Catch ex As Exception
                        ToolStripLabel1.Text = "connection Fail IP : " + IPaddressBox1.Text + "      Port : " + PortBox1.Text
                        Status_connect = 0
                    End Try

                Else
                    ToolStrip1.BackColor = Color.Green
                End If
            Loop
            Mqclient.Subscribe(topic, Qos)
            Status_connect = 0
        Loop

        'AddHandler Mqclient.MqttMsgPublishReceived, AddressOf MsgReceived
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
    Sub Init_connection()
        Dim brokeraddress As String = IPaddressBox1.Text
        Dim brokerport As Integer = Convert.ToInt32(PortBox1.Text)
        Mqclient = New MqttClient(brokeraddress, brokerport, False, Nothing, Nothing, MqttSslProtocols.None)
        Try
            Mqclient.Connect("pub", "", "")
            If Mqclient.IsConnected Then
                ToolStripLabel1.Text = "connected IP : " + IPaddressBox1.Text + "      Port : " + PortBox1.Text
                ToolStrip1.BackColor = Color.Green
                T1 = New System.Threading.Thread(AddressOf Auto_connect)
                T2 = New System.Threading.Thread(AddressOf MqSubscribe)
                T1.Start()
                T2.Start()
                'Delete comment here'
            End If

        Catch ex As Exception
            ToolStripLabel1.Text = "connection Fail IP : " + IPaddressBox1.Text + "      Port : " + PortBox1.Text
            ToolStrip1.BackColor = Color.Red
        End Try
    End Sub
End Class
