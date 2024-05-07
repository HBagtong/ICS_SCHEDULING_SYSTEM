﻿Imports System.Collections.Specialized.BitVector32
Imports System.Globalization
Imports Google.Protobuf.WellKnownTypes

Public Class roomSchedule
    Dim room As String
    Private Sub roomSchedule_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        DBCon()
        room = viewRooms.ClickRoomNumber
        Try
            Label1.Text = $"ROOM: {room}"
            cmd.Connection = con
            cmd.CommandText = "SELECT InstructorName, Section, Subject, CONCAT(TIME_FORMAT(StartTime, '%h:%i %p'),'-',TIME_FORMAT(EndTime, '%h:%i %p')) AS Time, Day, RoomNumber, Semester FROM schedules WHERE RoomNumber = @roomnumber ORDER BY DAYOFWEEK(day) ASC;"
            cmd.Parameters.Clear()
            cmd.Parameters.AddWithValue("@roomnumber", room)

            Dim data As New DataTable()
            dataReader.SelectCommand = cmd
            dataReader.Fill(data)

            printingdgv.DataSource = data

            dgvRoomSched.Columns.Clear()

            Dim daysOfWeek As String() = {"MONDAY", "TUESDAY", "WEDNESDAY", "THURSDAY", "FRIDAY", "SATURDAY", "SUNDAY"}

            For Each day As String In daysOfWeek
                dgvRoomSched.Columns.Add(day, day)
            Next

            For Each row As DataRow In data.Rows
                Dim TimeDuration As String = row("Time")
                Dim day As String = row("Day").ToString().Trim()
                Dim section As String = row("Section").ToString().Trim()
                Dim instructor As String = row("InstructorName").ToString().Trim()
                Dim Subject As String = row("Subject").ToString().Trim()
                Dim Semester As String = row("Semester").ToString.Trim()

                Dim columnIndex As Integer = Array.IndexOf(daysOfWeek, day)

                If columnIndex <> -1 AndAlso Not String.IsNullOrEmpty(section) Then
                    Dim rowIndex As Integer = dgvRoomSched.Rows.Add()
                    Dim CellValue As String = $"Semester: {Semester}" & vbCrLf & $"Time: {TimeDuration}" & vbCrLf & $"Instructor: {instructor}" & vbCrLf & $"Section: {section}" & vbCrLf & $"Subject: {Subject}"

                    dgvRoomSched.Rows(rowIndex).Cells(columnIndex).Value = CellValue
                    dgvRoomSched.Rows(rowIndex).Cells(columnIndex).Style.BackColor = Color.Green
                End If
            Next

            ' Set DataGridView properties
            dgvRoomSched.AllowUserToAddRows = False
            dgvRoomSched.ReadOnly = True

            dgvRoomSched.AutoSizeColumnsMode = DataGridViewAutoSizeColumnMode.ColumnHeader
            dgvRoomSched.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
            dgvRoomSched.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells
            dgvRoomSched.DefaultCellStyle.WrapMode = DataGridViewTriState.True

            printingdgv.AutoSizeColumnsMode = DataGridViewAutoSizeColumnMode.ColumnHeader
            printingdgv.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
            printingdgv.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells
            printingdgv.DefaultCellStyle.WrapMode = DataGridViewTriState.True

        Catch ex As Exception
            MessageBox.Show("An error occurred while loading the room schedule. Please try again later.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)

        Finally
            con.Close()
        End Try
    End Sub


    Private Sub backbtn_Click(sender As Object, e As EventArgs) Handles backbtn.Click

        Me.Close()
        viewRooms.Show()
    End Sub




    Private rowIndexToPrint As Integer = 0 ' Track the index of the next row to print
    Private isNewPage As Boolean = True

    Private Sub printer_PrintPage(sender As Object, e As Printing.PrintPageEventArgs) Handles printer.PrintPage
        Try
            Dim StrFormat As New StringFormat()
            StrFormat.Alignment = StringAlignment.Center

            ' Draw images
            Dim image1 As Image = My.Resources.cmdtransparent
            e.Graphics.DrawImage(image1, 60, 50, 100, 100)

            Dim Image2 As Image = My.Resources.icstransparent
            e.Graphics.DrawImage(Image2, 930, 35, 120, 120)

            ' Draw headers
            e.Graphics.DrawString("Colegio De Montalban", New Font("Calibri", 14, FontStyle.Bold), Brushes.Black, New PointF(570, 80), StrFormat)
            e.Graphics.DrawString("ICS Schedules", New Font("Calibri", 14, FontStyle.Bold), Brushes.Black, New PointF(570, 100), StrFormat)
            e.Graphics.DrawString($"Room: {room}", New Font("Calibri", 16, FontStyle.Bold), Brushes.Black, New PointF(80, 200), StrFormat)
            e.Graphics.DrawString("___________________________________________________________________", New Font("Calibri", 10, FontStyle.Regular), Brushes.Black, New PointF(320, 210), StrFormat)

            ' Define format for data
            Dim Format As New StringFormat(StringFormatFlags.LineLimit)
            Format.LineAlignment = StringAlignment.Center
            Format.Trimming = StringTrimming.EllipsisCharacter
            Format.Alignment = StringAlignment.Center

            Dim y As Integer = 230
            Dim x As Integer = 0
            Dim h As Integer = 0
            Dim recta As Rectangle
            Dim row As DataGridViewRow

            ' Print header row and rows
            If isNewPage Then
                row = printingdgv.Rows(rowIndexToPrint)
                x = 53

                ' Print Header Row
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

            ' Print Rows
            For dNow As Integer = rowIndexToPrint To printingdgv.RowCount - 1
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
            Console.WriteLine(ex.ToString()) ' Log the exception for debugging purposes
        End Try
    End Sub






    'PRINT WHEN CLICK
    Private Sub Printbtn_Click(sender As Object, e As EventArgs) Handles Printbtn.Click

        If dgvRoomSched.Rows.Count = 0 AndAlso printingdgv.Rows.Count = 1 Then
            MessageBox.Show("Both the document and printing data are empty.", "Empty Data", MessageBoxButtons.OK, MessageBoxIcon.Information)
            Return
        End If

        printer.DefaultPageSettings.Landscape = True
        PrintPreviewDialog.WindowState = WindowState.Maximized
        PrintPreviewDialog.Document = printer
        PrintPreviewDialog.ShowDialog()
        printingdgv.DataSource = Nothing
        viewRooms.Show()
        Me.Close()



    End Sub
End Class