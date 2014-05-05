﻿Imports System.Runtime.CompilerServices

Public Class ControlsByRegistry

    Private dictRegkeyNames As Dictionary(Of String, String)
    Private sRegKey As String

    Public Sub AddControl(ByRef ctlAny As Control, ByVal sName As String)

        Me.dictRegkeyNames.Add(ctlAny.Name, sName)

    End Sub

    Public Sub SetControlByRegKey(ByRef chklstAny As CheckedListBox)

        Using key As Microsoft.Win32.RegistryKey = Microsoft.Win32.Registry.CurrentUser.OpenSubKey(sRegKey & "\" & chklstAny.Name)
            If key Is Nothing Then
                My.Computer.Registry.CurrentUser.CreateSubKey(sRegKey & "\" & chklstAny.Name)
            End If
        End Using

        Using key As Microsoft.Win32.RegistryKey = Microsoft.Win32.Registry.CurrentUser.OpenSubKey(sRegKey & "\" & chklstAny.Name)
            Dim sTemp As String

            For Each sTemp In key.GetValueNames
                chklstAny.Items.Add(sTemp)
            Next
        End Using

    End Sub

    Public Sub SetControlByRegKey(ByRef chkAny As CheckBox, Optional bDefault As Boolean = False)

        Using key As Microsoft.Win32.RegistryKey = Microsoft.Win32.Registry.CurrentUser.OpenSubKey(sRegKey)
            Call SetControlByRegKey(key, chkAny, bDefault)
        End Using

    End Sub

    Public Sub SetControlByRegKey(ByRef regKey As Microsoft.Win32.RegistryKey, ByRef chkAny As CheckBox, Optional bDefault As Boolean = False)

        Dim sKey As String
        Dim sTemp As String

        If dictRegkeyNames.TryGetValue(chkAny.Name, sKey) = True Then
            sTemp = regKey.GetValue(sKey)

            If sTemp = "Y" Then
                chkAny.Checked = True
            Else
                If String.IsNullOrEmpty(sTemp) = True Then
                    chkAny.Checked = bDefault
                Else
                    chkAny.Checked = False
                End If
            End If
        Else
            Throw New Exception("Internal error: " & chkAny.Name & " not found in regKey dictionary.")
        End If

    End Sub

    Public Sub SetControlByRegKey(ByRef txtAny As TextBox, Optional ByVal sDefault As String = "")

        Using key As Microsoft.Win32.RegistryKey = Microsoft.Win32.Registry.CurrentUser.OpenSubKey(sRegKey)
            Call SetControlByRegKey(key, txtAny, sDefault)
        End Using

    End Sub

    Public Sub SetControlByRegKey(ByRef regKey As Microsoft.Win32.RegistryKey, ByRef txtAny As TextBox, Optional ByVal sDefault As String = "")

        Call _SetControlByRegKey(regKey, txtAny, sDefault)

    End Sub

    Public Sub SetControlByRegKey(ByRef regKey As Microsoft.Win32.RegistryKey, ByRef cmbAny As ComboBox, Optional ByVal sDefault As String = "")

        Call _SetControlByRegKey(regKey, cmbAny, sDefault)

    End Sub

    Private Sub _SetControlByRegKey(ByRef regKey As Microsoft.Win32.RegistryKey, ByRef ctlAny As Control, Optional ByVal sDefault As String = "")

        Dim sKey As String
        Dim sReturn As String

        If dictRegkeyNames.TryGetValue(ctlAny.Name, sKey) = True Then
            sReturn = regKey.GetValue(sKey)

            If String.IsNullOrEmpty(sReturn) = True Then
                ctlAny.Text = sDefault
            Else
                ctlAny.Text = sReturn
            End If
        Else
            Throw New Exception("Internal error: " & ctlAny.Name & " not found in regKey dictionary.")
        End If

    End Sub

    Public Sub SetRegKeyByControl(ByRef txtAny As TextBox)

        _SetRegKeyByControl(txtAny)

    End Sub

    Public Sub SetRegKeyByControl(ByRef txtAny As TextBox, ByVal sOverRide As String)

        _SetRegKeyByControl(txtAny, sOverRide)

    End Sub

    Public Sub SetRegKeyByControl(ByRef cmbAny As ComboBox)

        _SetRegKeyByControl(cmbAny)

    End Sub

    Private Sub _SetRegKeyByControl(ByRef ctlAny As Control, ByVal sOverRide As String)

        Dim sKeyname As String

        If dictRegkeyNames.TryGetValue(ctlAny.Name, sKeyname) = True Then
            My.Computer.Registry.SetValue("HKEY_CURRENT_USER\" & sRegKey, sKeyname, sOverRide, Microsoft.Win32.RegistryValueKind.String)
        Else
            Throw New Exception("Internal error: " & ctlAny.Name & " not found in regKey dictionary.")
        End If

    End Sub

    Private Sub _SetRegKeyByControl(ByRef ctlAny As Control)

        Dim sKeyname As String

        If dictRegkeyNames.TryGetValue(ctlAny.Name, sKeyname) = True Then
            My.Computer.Registry.SetValue("HKEY_CURRENT_USER\" & sRegKey, sKeyname, ctlAny.Text, Microsoft.Win32.RegistryValueKind.String)
        Else
            Throw New Exception("Internal error: " & ctlAny.Name & " not found in regKey dictionary.")
        End If

    End Sub

    Public Sub SetRegKeyByControl(ByRef chkLstAny As CheckedListBox)

        Dim sValue As String

        Using key As Microsoft.Win32.RegistryKey = Microsoft.Win32.Registry.CurrentUser.OpenSubKey(sRegKey & "\" & chkLstAny.Name, True)
            For Each sTemp In key.GetValueNames
                key.DeleteValue(sTemp)
            Next
        End Using

        For Each sValue In chkLstAny.Items
            My.Computer.Registry.SetValue("HKEY_CURRENT_USER\" & sRegKey & "\" & chkLstAny.Name, sValue, sValue, Microsoft.Win32.RegistryValueKind.String)
        Next

    End Sub

    Public Sub SetRegKeyByControl(ByRef chkAny As CheckBox)

        Dim sKeyname As String

        If dictRegkeyNames.TryGetValue(chkAny.Name, sKeyname) = True Then
            If chkAny.Checked = True Then
                My.Computer.Registry.SetValue("HKEY_CURRENT_USER\" & sRegKey, sKeyname, "Y", Microsoft.Win32.RegistryValueKind.String)
            Else
                My.Computer.Registry.SetValue("HKEY_CURRENT_USER\" & sRegKey, sKeyname, "N", Microsoft.Win32.RegistryValueKind.String)
            End If
        Else
            Throw New Exception("Internal error: " & chkAny.Name & " not found in regKey dictionary.")
        End If

    End Sub

    Public Sub New(ByVal csRegKey As String)

        dictRegkeyNames = New Dictionary(Of String, String)
        Me.sRegKey = csRegKey

    End Sub

End Class

Public Module Extensions

    <Extension()> Public Function IsNullOrEmpty(ByVal sString As String) As Boolean

        Return String.IsNullOrEmpty(sString)

    End Function

End Module