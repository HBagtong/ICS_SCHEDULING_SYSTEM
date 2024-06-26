﻿Public Class Dashboard
    Private Sub closeBtn_Click(sender As Object, e As EventArgs)
        Application.Exit()
    End Sub

    Private Sub minimizeBtn_Click(sender As Object, e As EventArgs)
        WindowState = FormWindowState.Minimized
    End Sub



    Private Sub btnNewSched_Click(sender As Object, e As EventArgs) Handles btnNewSched.Click
        CreateScheduleForm.Show()
        CreateScheduleForm.GetInstructor()
        CreateScheduleForm.getSchedules()
        CreateScheduleForm.GetSubject()
        CreateScheduleForm.GetRoom()
        CreateScheduleForm.GetSection()
        Me.Hide()
    End Sub

    Private Sub btnNewInstructor_Click(sender As Object, e As EventArgs) Handles btnNewInstructor.Click
        addNewInstructor.Show()
        Me.Hide()
    End Sub



    Private Sub btnViewFacilities_Click(sender As Object, e As EventArgs) Handles btnViewFacilities.Click
        viewRooms.Show()
        viewRooms.GetRooms()
        Me.Hide()

    End Sub

    Private Sub btnViewSection_Click(sender As Object, e As EventArgs) Handles btnViewSection.Click
        Me.Hide()
        viewSection.Show()
        viewSection.GetSections()
    End Sub

    Private Sub btnViewInstructor_Click(sender As Object, e As EventArgs) Handles btnViewInstructor.Click
        Me.Hide()
        viewInstructors.Show()
        viewInstructors.GetInstructors()
    End Sub

    Private Sub btnViewSubject_Click(sender As Object, e As EventArgs) Handles btnViewSubject.Click
        Me.Hide()
        SubjectForm.Show()
    End Sub





    Private Sub btnManagement_Click(sender As Object, e As EventArgs) Handles btnManagement.Click
        Me.Hide()
        ManagementScheduleForm.Show()
    End Sub

    Private Sub LogoutBtn_Click_2(sender As Object, e As EventArgs) Handles LogoutBtn.Click
        currentUser = ""
        lblcurrentuser.Text = ""
        ScanRFIDLOGIN.Show()
        Me.Close()
    End Sub

    Private Sub btnViewSummary_Click(sender As Object, e As EventArgs) Handles btnViewSummary.Click

        Summary.Show()
        Me.Hide()
    End Sub

    Private Sub Dashboard_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        lblcurrentuser.Text = currentUser
    End Sub


End Class
