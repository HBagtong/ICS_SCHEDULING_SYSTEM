﻿Imports System.Collections.Specialized.BitVector32
Imports System.Globalization

Public Class instructorSched
    Dim instructor As String

    Private Sub instructorSched_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Try
            ' Establish database connection
            DBCon()

            ' Retrieve the instructor name from the viewInstructors form
            instructor = viewInstructors.Instructor

            ' Set label text to display the instructor name
            Label1.Text = $"Instructor: {instructor}"


            cmd.Connection = con
            cmd.CommandText = "SELECT InstructorName, Section, Subject, CONCAT(TIME_FORMAT(StartTime, '%h:%i %p'),'-',TIME_FORMAT(EndTime, '%h:%i %p')) AS Time, Day, RoomNumber, Semester FROM schedules WHERE InstructorName = @instructor ORDER BY DAYOFWEEK(Day) ASC"

            ' Clear parameters and add the instructor parameter
            cmd.Parameters.Clear()
            cmd.Parameters.AddWithValue("@instructor", instructor)

            ' Fill data into a DataTable
            Dim data As New DataTable()
            dataReader.SelectCommand = cmd
            dataReader.Fill(data)

            ' Bind data to the printing DataGridView
            printingdgv.DataSource = data

            ' Clear columns and add columns for each day of the week
            dgvInstructorSched.Columns.Clear()
            Dim daysOfWeek As String() = {"MONDAY", "TUESDAY", "WEDNESDAY", "THURSDAY", "FRIDAY", "SATURDAY", "SUNDAY"}
            For Each day As String In daysOfWeek
                dgvInstructorSched.Columns.Add(day, day)
            Next

            ' Populate DataGridView with schedule data
            For Each row As DataRow In data.Rows
                Dim TimeDuration As String = row("Time")
                Dim day As String = row("Day").ToString().Trim()
                Dim Subject As String = row("Subject").ToString().Trim()
                Dim roomNumber As String = row("RoomNumber").ToString().Trim()
                Dim Semester As String = row("Semester").ToString().Trim()
                Dim Section As String = row("Section").ToString().Trim()
                Dim columnIndex As Integer = Array.IndexOf(daysOfWeek, day)


                ' Check if the day is valid and if the instructor name is not empty
                If columnIndex <> -1 AndAlso Not String.IsNullOrEmpty(instructor) Then
                    Dim rowIndex As Integer = dgvInstructorSched.Rows.Add()
                    Dim CellValue As String = $"Semester: {Semester}" & vbCrLf & $"Section: {Section}" & vbCrLf & $"Time: {TimeDuration}" & vbCrLf & $"Room: {roomNumber}" & vbCrLf & $"Subject: {Subject}"
                    dgvInstructorSched.Rows(rowIndex).Cells(columnIndex).Value = CellValue
                    dgvInstructorSched.Rows(rowIndex).Cells(columnIndex).Style.BackColor = Color.Green
                End If
            Next

            ' Set DataGridView properties
            dgvInstructorSched.AllowUserToAddRows = False
            dgvInstructorSched.ReadOnly = True
            dgvInstructorSched.AutoSizeColumnsMode = DataGridViewAutoSizeColumnMode.ColumnHeader
            dgvInstructorSched.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
            dgvInstructorSched.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells
            dgvInstructorSched.DefaultCellStyle.WrapMode = DataGridViewTriState.True
            printingdgv.AutoSizeColumnsMode = DataGridViewAutoSizeColumnMode.ColumnHeader
            printingdgv.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
            printingdgv.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells
            printingdgv.DefaultCellStyle.WrapMode = DataGridViewTriState.True

        Catch ex As Exception
            MessageBox.Show("An error occurred while loading instructor schedule data. Please try again or contact support for assistance.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)

        Finally
            con.Close()
        End Try
    End Sub


    Private Sub backbtn_Click(sender As Object, e As EventArgs) Handles backbtn.Click
        Me.Close()
        viewInstructors.Show()

    End Sub


    Private rowIndexToPrint As Integer = 0 ' Track the index of the next row to print
    Private isNewPage As Boolean = True

    Private Sub printer_PrintPage(sender As Object, e As Printing.PrintPageEventArgs) Handles printer.PrintPage
        Try
            Dim StrFormat As New StringFormat()
            StrFormat.Alignment = StringAlignment.Center

            Dim image1 As Image = My.Resources.cmdtransparent
            e.Graphics.DrawImage(image1, 60, 50, 100, 100)

            Dim Image2 As Image = My.Resources.icstransparent
            e.Graphics.DrawImage(Image2, 930, 35, 120, 120)




            e.Graphics.DrawString("Colegio De Montalban", New Font("Calibri", 14, FontStyle.Bold), Brushes.Black, New PointF(570, 80), StrFormat)
            e.Graphics.DrawString("ICS Schedules", New Font("Calibri", 14, FontStyle.Bold), Brushes.Black, New PointF(570, 100), StrFormat)

            e.Graphics.DrawString($"Instructor:", New Font("Calibri", 16, FontStyle.Bold), Brushes.Black, New PointF(100, 200), StrFormat)
            e.Graphics.DrawString("___________________________________________________________________", New Font("Calibri", 10, FontStyle.Regular), Brushes.Black, New PointF(380, 210), StrFormat)
            e.Graphics.DrawString(instructor, New Font("Calibri", 16, FontStyle.Bold), Brushes.Black, New PointF(220, 203), StrFormat)

            Dim Format As New StringFormat(StringFormatFlags.LineLimit)
            Format.LineAlignment = StringAlignment.Center
            Format.Trimming = StringTrimming.EllipsisCharacter
            Format.Alignment = StringAlignment.Center


            Dim y As Integer = 230
            Dim x As Integer = 0
            Dim h As Integer = 0
            Dim recta As Rectangle
            Dim row As DataGridViewRow

            If isNewPage Then
                row = printingdgv.Rows(rowIndexToPrint)
                x = 53
                'Print Header Row
                For Each cell As DataGridViewCell In row.Cells
                    If cell.Visible Then


                        recta = New Rectangle(x, y, cell.Size.Width, cell.Size.Height)
                        e.Graphics.FillRectangle(Brushes.LightYellow, recta)
                        e.Graphics.DrawRectangle(Pens.Black, recta)

                        Dim headerText As String = ""
                        Select Case cell.ColumnIndex
                            Case 0
                                headerText = "Name"
                            Case 1
                                headerText = "Section"
                            Case 2
                                headerText = "Subject"
                            Case 3
                                headerText = "Time"
                            Case 4
                                headerText = "Day"
                            Case 5
                                headerText = "Room"
                            Case 6
                                headerText = "Semester"
                        End Select

                        Dim centerHeaderFormat As New StringFormat(Format)
                        centerHeaderFormat.Alignment = StringAlignment.Center

                        e.Graphics.DrawString(headerText, New Font("Calibri", 14, FontStyle.Bold), Brushes.Black, recta, centerHeaderFormat)


                        x += recta.Width
                        h = Math.Max(h, recta.Height)

                    End If
                Next
                y += h
            End If

            isNewPage = False
            Dim dNow As Integer

            'Print Rows
            For dNow = rowIndexToPrint To printingdgv.RowCount - 1
                row = printingdgv.Rows(dNow)
                x = 53
                h = 0

                For Each cell As DataGridViewCell In row.Cells
                    If cell.Visible Then
                        recta = New Rectangle(x, y, cell.Size.Width, cell.Size.Height)
                        e.Graphics.DrawRectangle(Pens.Black, recta)

                        Format.Alignment = StringAlignment.Center
                        recta.Offset(5, 0)





                        e.Graphics.DrawString(cell.FormattedValue.ToString(), New Font("Calibri", 12, FontStyle.Regular), Brushes.Black, recta, Format)


                        x += recta.Width
                        h = Math.Max(h, recta.Height)
                    End If
                Next

                y += h

                rowIndexToPrint = dNow + 1
                If y + h > e.MarginBounds.Bottom Then
                    e.HasMorePages = True
                    isNewPage = True
                    Return
                End If




            Next
        Catch ex As Exception
            MessageBox.Show("An error occurred while printing the schedule. Please try again or contact support for assistance.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub





    'PRINT WHEN CLICK
    Private Sub Printbtn_Click(sender As Object, e As EventArgs) Handles Printbtn.Click



        If printingdgv.Rows.Count = 1 AndAlso dgvInstructorSched.Rows.Count = 0 Then
            MessageBox.Show("Both the document and printing data are empty.", "Empty Data", MessageBoxButtons.OK, MessageBoxIcon.Information)
            Return
        End If

        printer.DefaultPageSettings.Landscape = True
        PrintPreviewDialog.WindowState = WindowState.Maximized
        PrintPreviewDialog.Document = printer
        PrintPreviewDialog.ShowDialog()
        printingdgv.DataSource = Nothing
        viewInstructors.Show()
        Me.Close()



    End Sub


End Class