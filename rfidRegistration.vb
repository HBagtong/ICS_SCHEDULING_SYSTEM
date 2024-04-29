﻿Imports MySql.Data.MySqlClient

Public Class RFIDREGISTRATION

    Dim fname, mname, lname, sufx, jobPosisyon, emailAdd, workStats, registeredRFID As String
    Private Sub rfidandPinRegistrationforinstructors_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        rfidscanpanel.Dock = DockStyle.Fill
        txtRfidRegister.Focus()

    End Sub







    Private Sub txtRfidRegister_TextChanged(sender As Object, e As EventArgs) Handles txtRfidRegister.TextChanged
        If txtRfidRegister.Text.Length = 10 Then
            registeredRFID = txtRfidRegister.Text
            InsertInstructorAccount()


        End If


    End Sub

    ' ayusin nalang yung pagaayos ng variables and procedures
    Public Sub instructorDataConstructor(firstName As String, middleName As String, lastName As String, suffix As String, role As String, email As String, workstatus As String)
        fname = firstName
        mname = middleName
        lname = lastName
        sufx = suffix
        emailAdd = email
        jobPosisyon = role
        workStats = workstatus
    End Sub




    Private Sub InsertInstructorAccount()
        Try

            registeredRFID = EncryptData(registeredRFID)


            DBCon()

            If con.State = ConnectionState.Open Then

                cmd.CommandText = "INSERT INTO instructor (RFID, Firstname, MiddleName, Surname, Suffix, Position, WorkStatus, email) VALUES (@rfid, @fname, @mname, @surname, @suffix, @position, @workstats, @email)"


                cmd.Parameters.Clear()
                cmd.Connection = con

                cmd.Parameters.AddWithValue("@rfid", registeredRFID)
                cmd.Parameters.AddWithValue("@fname", fname)
                cmd.Parameters.AddWithValue("@mname", mname)
                cmd.Parameters.AddWithValue("@surname", lname)
                cmd.Parameters.AddWithValue("@suffix", sufx)
                cmd.Parameters.AddWithValue("@position", jobPosisyon)
                cmd.Parameters.AddWithValue("@workstats", workStats)
                cmd.Parameters.AddWithValue("@email", emailAdd)






                If cmd.ExecuteNonQuery() > 0 Then
                    ScanDoneLogo.Visible = True
                    MessageBox.Show("Instructor account created successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information)
                    Me.Close()
                    adminRegistration.Close()
                    ScanRFIDLOGIN.Show()


                End If


                con.Close()
            End If


        Catch ex As MySqlException
            If ex.Number = 1062 Then
                If ex.Message.Contains("instructor_rfid_unique") Then
                    MessageBox.Show("Duplicate RFID! This RFID already exists.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                    adminRegistration.Show()
                    Me.Close()

                ElseIf ex.Message.Contains("instructor_email_unique") Then
                    MessageBox.Show("Duplicate email address! This email already exists.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                    adminRegistration.Show()
                    Me.Close()
                Else
                    MessageBox.Show("Duplicate entry error: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                    adminRegistration.Show()
                    Me.Close()
                End If
            Else
                MessageBox.Show("Error: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                adminRegistration.Show()
                Me.Close()
            End If

        Finally
            con.Close()
        End Try


    End Sub


End Class
